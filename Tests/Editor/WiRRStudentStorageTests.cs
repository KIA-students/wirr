using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using KIA.WiRR;
using NUnit.Framework;
using UnityEngine;

public sealed class WiRRStudentStorageTests
{
    private static readonly string[] Indices = { "999991" };
    private static WiRRStudentSession Session() => new WiRRStudentSession { block = 1, group = "G1", studentIndices = Indices };
    private static void InFolder(Action<string> action)
    {
        var folder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")); Directory.CreateDirectory(folder);
        try { action(folder); } finally { Directory.Delete(folder, true); }
    }
    [Test] public void MissingSessionCreatesOnlyTheRequestedProfile()
    {
        InFolder(folder => {
            var session = WiRRStudentStorage.Load(Path.Combine(folder, "session.json"), Indices, "G1", 1, out var backup);
            Assert.IsFalse(backup); Assert.AreEqual(1, session.block); CollectionAssert.AreEqual(Indices, session.studentIndices);
            Assert.AreEqual(0, Directory.GetFiles(folder).Length);
        });
    }
    [Test] public void CorruptPrimaryRestoresValidBackupAndPreservesOriginalBytes()
    {
        InFolder(folder => {
            var path = Path.Combine(folder, "session.json");
            WiRRStudentEvidence.AtomicWrite(path, JsonUtility.ToJson(Session()));
            WiRRStudentEvidence.AtomicWrite(path, "broken");
            var loaded = WiRRStudentStorage.Load(path, Indices, "G1", 1, out var backup);
            Assert.IsTrue(backup); Assert.AreEqual(1, loaded.block);
            Assert.AreEqual("broken", File.ReadAllText(Directory.GetFiles(folder, "*.corrupt.*").Single()));
            Assert.IsTrue(File.Exists(path + ".bak"));
            WiRRStudentEvidence.AtomicWrite(path, JsonUtility.ToJson(loaded));
            WiRRStudentStorage.Validate(JsonUtility.FromJson<WiRRStudentSession>(File.ReadAllText(path + ".bak")));
        });
    }
    [Test] public void InvalidBackupCannotMoveOrOverwriteEitherFile()
    {
        InFolder(folder => {
            var path = Path.Combine(folder, "session.json"); File.WriteAllText(path, "broken"); File.WriteAllText(path + ".bak", "also broken");
            Assert.Throws<ArgumentException>(() => WiRRStudentStorage.Load(path, Indices, "G1", 1, out _));
            Assert.AreEqual("broken", File.ReadAllText(path)); Assert.AreEqual("also broken", File.ReadAllText(path + ".bak"));
            Assert.AreEqual(2, Directory.GetFiles(folder).Length);
        });
    }
    [Test] public void AnotherProfileIsRejectedWithoutOverwritingTheSession()
    {
        InFolder(folder => {
            var path = Path.Combine(folder, "session.json"); var json = JsonUtility.ToJson(Session()); File.WriteAllText(path, json);
            Assert.Throws<ArgumentException>(() => WiRRStudentStorage.Load(path, new[] { "999992" }, "G1", 1, out _));
            Assert.AreEqual(json, File.ReadAllText(path));
        });
    }
    [Test] public void NullDuplicateAndNonfiniteMetricsAreRejectedBeforeExport()
    {
        var session = Session(); var trial = new WiRRStudentTrial { condition = "baseline", deviceMode = "DESKTOP_SIM" };
        session.Lab(1).trials.Add(trial); trial.metrics.Add(null);
        Assert.Throws<ArgumentException>(() => WiRRStudentStorage.Validate(session)); trial.metrics.Clear();
        trial.metrics.Add(WiRRStudentMetric.Measured("errors", 0, "count")); trial.metrics.Add(WiRRStudentMetric.Measured("errors", 1, "count"));
        Assert.Throws<ArgumentException>(() => WiRRStudentStorage.Validate(session)); trial.metrics.RemoveAt(1);
        trial.metrics[0].value = double.NaN;
        Assert.Throws<ArgumentException>(() => WiRRStudentStorage.Validate(session));
    }
    [Test] public void EachExportHasTwoReportsVerifiedHashesAndItsOwnDirectory()
    {
        InFolder(folder => {
            var session = Session(); var first = WiRRStudentStorage.ExportBlock(session, folder, "test");
            var previous = File.ReadAllText(Path.Combine(first, "block.json"));
            var second = WiRRStudentStorage.ExportBlock(session, folder, "test"); Assert.AreNotEqual(first, second);
            Assert.AreEqual(previous, File.ReadAllText(Path.Combine(first, "block.json")));
            foreach (var report in new[] { "WiRR-lab01.json", "WiRR-lab02.json" })
                using (var sha = SHA256.Create())
                    StringAssert.Contains(BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(Path.Combine(second, report)))).Replace("-", "").ToLowerInvariant(), File.ReadAllText(Path.Combine(second, "block.json")));
            StringAssert.Contains("LOCAL_ONLY", previous);
        });
    }
    [Test] public void FailedSecondReportHasNoManifestAndDoesNotChangePreviousExport()
    {
        InFolder(folder => {
            var session = Session(); var first = WiRRStudentStorage.ExportBlock(session, folder, "test");
            var previous = File.ReadAllText(Path.Combine(first, "block.json")); var writes = 0;
            Assert.Throws<IOException>(() => WiRRStudentStorage.ExportBlock(session, folder, "test", (path, json) => {
                if (++writes == 2) throw new IOException("Injected storage failure"); WiRRStudentEvidence.AtomicWrite(path, json);
            }));
            Assert.AreEqual(1, Directory.GetFiles(folder, "block.json", SearchOption.AllDirectories).Length);
            Assert.AreEqual(previous, File.ReadAllText(Path.Combine(first, "block.json")));
        });
    }
    [Test] public void CorruptedManifestIsQuarantinedInsteadOfMarkingAnExportComplete()
    {
        InFolder(folder => {
            Assert.Throws<IOException>(() => WiRRStudentStorage.ExportBlock(Session(), folder, "test", (path, json) =>
                WiRRStudentEvidence.AtomicWrite(path, Path.GetFileName(path) == "block.json" ? "broken" : json)));
            Assert.AreEqual(0, Directory.GetFiles(folder, "block.json", SearchOption.AllDirectories).Length);
            Assert.AreEqual(1, Directory.GetFiles(folder, "block.json.incomplete", SearchOption.AllDirectories).Length);
        });
    }
}

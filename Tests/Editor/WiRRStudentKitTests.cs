using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KIA.WiRR;
using KIA.WiRR.Editor;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.SceneManagement;
using UnityEngine;

public sealed class WiRRStudentKitTests
{
    [Serializable] private sealed class StringProbe { public string text; }
    [Test] public void PercentilesHandleEvenSamplesAndRejectMissingData()
    {
        Assert.AreEqual(2.5, WiRRStudentEvidence.Percentile(new double[] { 4, 2, 1, 3 }, 0.5));
        Assert.AreEqual(3.85, WiRRStudentEvidence.Percentile(new double[] { 4, 2, 1, 3 }, 0.95), 1e-9);
        Assert.Throws<ArgumentException>(() => WiRRStudentEvidence.Percentile(Array.Empty<double>(), 0.5));
    }

    [Test] public void IdentitySeparatesBlocksAndTeamsButNotPairOrder()
    {
        var pair = new[] { "999991", "999992" };
        Assert.AreEqual(WiRRStudentEvidence.IdentityKey(pair, "G1", 1), WiRRStudentEvidence.IdentityKey(pair.Reverse(), "G1", 1));
        Assert.AreNotEqual(WiRRStudentEvidence.IdentityKey(pair, "G1", 1), WiRRStudentEvidence.IdentityKey(pair, "G1", 2));
        Assert.IsFalse(WiRRStudentEvidence.ValidIndices(new[] { "999991", "999991" }));
        Assert.IsFalse(WiRRStudentEvidence.ValidIndices(new[] { "1e5" }));
    }

    [Test] public void ReportsStayIndependentAndPreserveMissingValues()
    {
        var session = new WiRRStudentSession { block = 1, studentIndices = new[] { "999991" }, group = "G1" };
        session.Lab(1).trials.Add(new WiRRStudentTrial { deviceMode = "DESKTOP_SIM", condition = "baseline",
            metrics = new List<WiRRStudentMetric> { WiRRStudentMetric.Measured("errors", 0, "count"),
                new WiRRStudentMetric { name = "rtt", unit = "ms", unavailableReason = "Nie mierzono RTT" } } });
        var json = WiRRStudentEvidence.Json(WiRRStudentEvidence.Report(session, 1, Application.unityVersion));
        StringAssert.Contains("\"value\":0", json);
        StringAssert.Contains("\"value\":null", json);
        StringAssert.Contains("\"proposed_grade\":null", json);
        Assert.AreEqual(0, session.Lab(2).trials.Count);
        Assert.Throws<ArgumentOutOfRangeException>(() => session.Lab(3));
        // This real C# adapter output can be checked with scripts~/wirr_grade_report.py.
        File.WriteAllText(Path.Combine(TestContext.CurrentContext.WorkDirectory, "student-kit-report.json"), json);
    }

    [Test] public void JsonEscapesControlCharactersAndRejectsNonfiniteNumbers()
    {
        var expected = "a\n\"\\\t";
        var json = WiRRStudentEvidence.Json(new Dictionary<string, object> { { "text", expected } });
        Assert.AreEqual(expected, JsonUtility.FromJson<StringProbe>(json).text);
        Assert.Throws<ArgumentException>(() => WiRRStudentEvidence.Json(double.NaN));
        Assert.Throws<ArgumentException>(() => WiRRStudentEvidence.Json(float.PositiveInfinity));
    }

    [Test] public void AtomicSaveKeepsRecoverablePreviousVersion()
    {
        var folder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var path = Path.Combine(folder, "session.json");
        try
        {
            WiRRStudentEvidence.AtomicWrite(path, "first");
            WiRRStudentEvidence.AtomicWrite(path, "second");
            Assert.AreEqual("second", File.ReadAllText(path));
            Assert.AreEqual("first", File.ReadAllText(path + ".bak"));
        }
        finally { if (Directory.Exists(folder)) Directory.Delete(folder, true); }
    }

    [Test] public void ReplayMapsNamesRatherThanColumnOrder()
    {
        var frames = WiRRStudentLab.ReadReplay("time_s,joint2_rad,joint1_rad\n0,0.2,0.7\n0.1,0.3,0.8\n");
        Assert.AreEqual(0.7f, frames[0].joint1);
        Assert.AreEqual(0.2f, frames[0].joint2);
        Assert.Throws<ArgumentException>(() => WiRRStudentLab.ReadReplay("time_s,joint2_rad,joint1_rad\n0,0,0\n0,0,0"));
    }

    [TestCase(0f, 0)] [TestCase(5.99f, 0)] [TestCase(6f, 1)]
    [TestCase(9.99f, 1)] [TestCase(10f, 2)] [TestCase(50f, 2)]
    public void DistanceLodUsesExplicitMetreThresholds(float distance, int expected)
    {
        Assert.AreEqual(expected, WiRRStudentWorld.DistanceLod(distance, 6, 10));
    }

    [Test] public void DistanceLodRejectsUnknownOrReversedDistances()
    {
        foreach (var distance in new[] { -1f, float.NaN, float.PositiveInfinity })
            Assert.Throws<ArgumentOutOfRangeException>(() => WiRRStudentWorld.DistanceLod(distance, 6, 10));
        Assert.Throws<ArgumentOutOfRangeException>(() => WiRRStudentWorld.DistanceLod(4, 10, 6));
        Assert.Throws<ArgumentOutOfRangeException>(() => WiRRStudentWorld.DistanceLod(4, 0, 6));
        Assert.Throws<ArgumentOutOfRangeException>(() => WiRRStudentWorld.DistanceLod(4, 3, float.NaN));
    }

    [Test] public void InterruptedTrialSurvivesSerializationWithoutInventingTime()
    {
        var session = new WiRRStudentSession { block = 1, studentIndices = new[] { "999991" }, group = "G1" };
        session.BeginTrial(1, "DESKTOP_SIM", "SOLO", "baseline");
        var id = session.pendingTrial.trial.id;
        var loaded = JsonUtility.FromJson<WiRRStudentSession>(JsonUtility.ToJson(session));
        Assert.IsTrue(loaded.RecoverInterruptedTrial());
        var trial = loaded.Lab(1).trials.Single();
        Assert.AreEqual(id, trial.id);
        Assert.AreEqual("FAILED", trial.validity);
        Assert.IsFalse(trial.metrics.Single().available);
        Assert.IsFalse(loaded.RecoverInterruptedTrial());
        Assert.AreEqual(1, loaded.Lab(1).trials.Count);
        Assert.AreEqual(0, loaded.Lab(2).trials.Count);
        var json = WiRRStudentEvidence.Json(WiRRStudentEvidence.Report(loaded, 1, Application.unityVersion));
        StringAssert.Contains("\"value\":null", json);
    }

    [Test] public void CompletedTrialKeepsStartIdentityAndCannotBeCompletedTwice()
    {
        var session = new WiRRStudentSession { block = 1 };
        session.BeginTrial(1, "DESKTOP_SIM", "SOLO", "baseline");
        var started = session.pendingTrial.trial;
        Assert.Throws<InvalidOperationException>(() => session.BeginTrial(2, "DESKTOP_SIM", "SOLO", "baseline"));
        Assert.Throws<InvalidOperationException>(() => WiRRStudentEvidence.Report(session, 1, Application.unityVersion));
        var result = new WiRRStudentTrial();
        session.CompleteTrial(1, result);
        Assert.AreEqual(started.id, result.id);
        Assert.AreEqual(started.timestampUtc, result.timestampUtc);
        Assert.AreEqual("baseline", result.condition);
        Assert.IsNull(session.pendingTrial);
        Assert.Throws<InvalidOperationException>(() => session.CompleteTrial(1, result));
        Assert.IsFalse(session.RecoverInterruptedTrial());
    }

    [TestCase(96, true)] [TestCase(24, true)] [TestCase(6, false)]
    public void PreparedMeshesHaveValidNondegenerateTriangles(int segments, bool hole)
    {
        var mesh = WiRRStudentWorld.RingMesh(segments, hole);
        try
        {
            var vertices = mesh.vertices; var triangles = mesh.triangles;
            Assert.Greater(triangles.Length, 0);
            for (var i = 0; i < triangles.Length; i += 3)
            {
                Assert.That(triangles[i], Is.InRange(0, vertices.Length - 1));
                var a = vertices[triangles[i]]; var b = vertices[triangles[i + 1]]; var c = vertices[triangles[i + 2]];
                Assert.Greater(Vector3.Cross(b - a, c - a).sqrMagnitude, 1e-12f);
            }
            Assert.AreEqual(hole, vertices.All(v => new Vector2(v.x, v.y).magnitude > 0.29f));
        }
        finally { UnityEngine.Object.DestroyImmediate(mesh); }
    }

    [Test] public void InteractionSeriesRecordsFiveCyclesAndRetainsErrors()
    {
        var series = new WiRRInteractionSeries(100);
        for (var cycle = 1; cycle <= 5; cycle++)
        {
            series.Error();
            series.CompleteCycle(100 + cycle * 2, cycle != 3);
        }
        Assert.IsTrue(series.IsComplete);
        Assert.AreEqual(4, series.Successes);
        Assert.AreEqual(5, series.TotalErrors);
        CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 5 }, series.Cycles.Select(c => c.number));
        Assert.IsTrue(series.Cycles.All(c => c.seconds == 2 && c.errors == 1));
        Assert.Throws<InvalidOperationException>(() => series.Error());
        Assert.Throws<InvalidOperationException>(() => series.CompleteCycle(112, true));
    }

    [Test] public void AbortedSeriesDoesNotFabricateTheRemainingCycles()
    {
        var series = new WiRRInteractionSeries(0);
        series.CompleteCycle(2, true);
        series.Error();
        series.Abort(5);
        series.Abort(6);
        Assert.IsTrue(series.IsAborted);
        Assert.IsFalse(series.IsComplete);
        Assert.AreEqual(2, series.Cycles.Count);
        Assert.AreEqual(3, series.Cycles[1].seconds);
        Assert.IsFalse(series.Cycles[1].success);
        var metrics = series.Metrics();
        Assert.IsFalse(metrics.Single(m => m.name == "cycle_3_time").available);
        Assert.IsFalse(metrics.Single(m => m.name == "cycle_5_success").available);
        Assert.AreEqual(1, series.TotalErrors);
    }

    [Test] public void InteractionSeriesRejectsNonmonotonicAndUnknownClocks()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new WiRRInteractionSeries(double.NaN));
        Assert.Throws<ArgumentOutOfRangeException>(() => new WiRRInteractionSeries(0, 0));
        var series = new WiRRInteractionSeries(10);
        Assert.Throws<ArgumentOutOfRangeException>(() => series.CompleteCycle(9, true));
        Assert.Throws<ArgumentOutOfRangeException>(() => series.CurrentSeconds(double.PositiveInfinity));
        Assert.AreEqual(0, series.Cycles.Count);
    }

    [Test] public void FailedCyclesDoNotBecomeFastSuccessfulCycles()
    {
        var series = new WiRRInteractionSeries(0);
        series.CompleteCycle(1, false);
        Assert.IsFalse(series.Metrics().Single(m => m.name == "cycle_time_median_successful").available);
        series.CompleteCycle(6, true);
        Assert.AreEqual(5, series.Metrics().Single(m => m.name == "cycle_time_median_successful").value);
        Assert.AreEqual(1, series.Successes);
    }

    [Test] public void RecoveryRetainsCompletedCyclesFromAnInterruptedSeries()
    {
        var session = new WiRRStudentSession { block = 1 };
        session.BeginTrial(2, "DESKTOP_SIM", "SOLO", "Direct_PC");
        var series = new WiRRInteractionSeries(0);
        series.Error(); series.CompleteCycle(2, false);
        session.pendingTrial.trial.metrics = series.Metrics();
        var restored = JsonUtility.FromJson<WiRRStudentSession>(JsonUtility.ToJson(session));
        restored.RecoverInterruptedTrial();
        var metrics = restored.Lab(2).trials.Single().metrics;
        Assert.AreEqual(2, metrics.Single(m => m.name == "cycle_1_time").value);
        Assert.AreEqual(1, metrics.Single(m => m.name == "cycle_1_errors").value);
        Assert.AreEqual(0, metrics.Single(m => m.name == "cycle_1_success").value);
        Assert.IsFalse(metrics.Single(m => m.name == "cycle_2_time").available);
        Assert.IsFalse(metrics.Single(m => m.name == "elapsed").available);
    }

    [Test] public void AllEightScenesImportWithTheCorrectRuntimeScript()
    {
        var package = UnityEditor.PackageManager.PackageInfo.FindForPackageName("pl.prz.kia.wirr");
        var folder = "Assets/WiRRStudentKitTest-" + Guid.NewGuid().ToString("N");
        Directory.CreateDirectory(folder);
        AssetDatabase.Refresh();
        try
        {
            for (var lab = 1; lab <= 8; lab++)
            {
                var name = WiRRStudentMaterialsWindow.SceneNames[lab - 1] + ".unity";
                var path = folder + "/" + name;
                File.Copy(Path.Combine(package.resolvedPath, "Samples~", "Lab" + lab.ToString("00"), "WiRR2", name), path);
                AssetDatabase.ImportAsset(path);
                var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
                try { Assert.AreEqual(lab, scene.GetRootGameObjects().Single().GetComponent<WiRRStudentLab>().labNumber); }
                finally { EditorSceneManager.CloseScene(scene, true); }
            }
        }
        finally { AssetDatabase.DeleteAsset(folder); }
    }
}

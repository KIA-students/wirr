using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace KIA.WiRR.Editor.Tests
{
    public sealed class WiRRExportSnapshotTests
    {
        private string folder;

        [SetUp]
        public void SetUp()
        {
            folder = Path.Combine(Path.GetTempPath(), "wirr-snapshot-test-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(folder);
        }

        [TearDown]
        public void TearDown()
        {
            Directory.Delete(folder, true);
        }

        private string Export(string json)
        {
            var store = typeof(global::WiRRPublicReportSubmission).Assembly.GetType(
                "KIA.WiRR.Editor.WiRRReportStore", true);
            var write = store.GetMethod("WriteExportSnapshot", BindingFlags.Static | BindingFlags.NonPublic);
            Assert.That(write, Is.Not.Null);
            return (string)write.Invoke(null, new object[] { folder, "synthetic", 1, json });
        }

        [Test]
        public void ExportingCorrectionsPreservesPreviousReportAndReceipt()
        {
            var first = Export("{\"observation\":\"pierwsza\"}");
            File.WriteAllText(first + ".submission-id", "previous-transport-id");
            File.WriteAllText(first + ".receipt.json", "{\"accepted\":true}");
            var second = Export("{\"observation\":\"poprawiona\"}");

            Assert.That(second, Is.Not.EqualTo(first));
            Assert.That(File.ReadAllText(first), Is.EqualTo("{\"observation\":\"pierwsza\"}"));
            Assert.That(File.ReadAllText(second), Is.EqualTo("{\"observation\":\"poprawiona\"}"));
            Assert.That(File.ReadAllText(first + ".submission-id"), Is.EqualTo("previous-transport-id"));
            Assert.That(File.Exists(first + ".receipt.json"), Is.True);
            Assert.That(File.Exists(second + ".submission-id"), Is.False);
            Assert.That(File.Exists(second + ".receipt.json"), Is.False);
        }

        [Test]
        public void RepeatedExportDoesNotOverwriteSnapshot()
        {
            var first = Export("{}");
            var second = Export("{}");
            Assert.That(second, Is.Not.EqualTo(first));
            Assert.That(Directory.GetFiles(folder, "*.json").Length, Is.EqualTo(2));
        }

        [Test]
        public void SnapshotKeepsLabFilenameAndUtf8Content()
        {
            var path = Export("{\"observation\":\"ąęł\"}");
            Assert.That(Path.GetFileName(path), Does.StartWith("synthetic-lab01-"));
            Assert.That(Path.GetExtension(path), Is.EqualTo(".json"));
            Assert.That(File.ReadAllText(path), Is.EqualTo("{\"observation\":\"ąęł\"}"));
        }
    }
}

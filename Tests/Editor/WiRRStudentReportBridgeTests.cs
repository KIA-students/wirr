using System;
using System.Collections.Generic;
using System.Reflection;
using KIA.WiRR;
using NUnit.Framework;

namespace KIA.WiRR.Editor.Tests
{
    public sealed class WiRRStudentReportBridgeTests
    {
        private static void Validate(WiRRReportDocument document, string[] indices, string group)
        {
            var type = typeof(global::WiRRPublicReportSubmission).Assembly.GetType(
                "KIA.WiRR.Editor.WiRRStudentReportBridge", true);
            var method = type.GetMethod("ValidateExistingOwner", BindingFlags.NonPublic | BindingFlags.Static);
            try { method.Invoke(null, new object[] { document, indices, group }); }
            catch (TargetInvocationException error) { throw error.InnerException; }
        }

        [Test]
        public void ForeignDraftWithoutTeamIdIsRejectedWithoutRelabelling()
        {
            var document = new WiRRReportDocument {
                group = "G1", studentIndices = new List<string> { "999992" }
            };
            Assert.Throws<ArgumentException>(() => Validate(document, new[] { "999991" }, "G1"));
            Assert.AreEqual("999992", document.studentIndices[0]);
        }

        [Test]
        public void PairCanBeImportedByEitherMemberWithoutChangingTheDraft()
        {
            var document = new WiRRReportDocument {
                group = "G1", studentIndices = new List<string> { "999992", "999991" }
            };
            Assert.DoesNotThrow(() => Validate(document, new[] { "999991", "999992" }, "G1"));
            CollectionAssert.AreEqual(new[] { "999992", "999991" }, document.studentIndices);
        }

        [Test]
        public void ForeignGroupIsRejectedBeforeWriting()
        {
            var document = new WiRRReportDocument {
                group = "G2", studentIndices = new List<string> { "999991" }
            };
            Assert.Throws<ArgumentException>(() => Validate(document, new[] { "999991" }, "G1"));
            Assert.AreEqual("G2", document.group);
        }
    }
}

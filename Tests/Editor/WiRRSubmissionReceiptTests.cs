using System.Reflection;
using NUnit.Framework;

namespace KIA.WiRR.Editor.Tests
{
    public sealed class WiRRSubmissionReceiptTests
    {
        private const string SubmissionId = "4ac6f686-bbb1-42f3-9668-a678d9187a52";

        private static bool IsAccepted(string body)
        {
            var parser = typeof(global::WiRRPublicReportSubmission).GetMethod(
                "IsAcceptedResponse", BindingFlags.Static | BindingFlags.NonPublic);
            Assert.That(parser, Is.Not.Null);
            return (bool)parser.Invoke(null, new object[] { body, SubmissionId });
        }

        [TestCase("{\"accepted\":true}")]
        [TestCase("{ \"accepted\" : true }")]
        [TestCase("{\n  \"accepted\": true,\n  \"submission_id\": \"4ac6f686-bbb1-42f3-9668-a678d9187a52\"\n}")]
        [TestCase("{\"submission_id\":\"4AC6F686-BBB1-42F3-9668-A678D9187A52\",\"accepted\":true}")]
        public void AcceptsReceiptRegardlessOfFormatting(string body)
        {
            Assert.That(IsAccepted(body), Is.True);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("{}")]
        [TestCase("{\"accepted\":false}")]
        [TestCase("{\"message\":\"accepted\",\"details\":{\"accepted\":true}}")]
        [TestCase("{\"accepted\":true,\"submission_id\":\"an-unrelated-submission\"}")]
        [TestCase("not JSON")]
        public void RejectsMissingRefusedUnrelatedOrMalformedReceipt(string body)
        {
            Assert.That(IsAccepted(body), Is.False);
        }
    }
}

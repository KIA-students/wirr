using System;
using System.Collections.Generic;

namespace KIA.WiRR
{
    [Serializable]
    public sealed class WiRRReportDocument
    {
        public string schemaVersion = "wirr-report/1.0";
        public string submissionId;
        public int labNumber;
        public string course = "Wirtualna i rozszerzona rzeczywistość";
        public string teamId;
        public List<string> studentIndices = new List<string>();
        public string createdAtUtc;
        public string updatedAtUtc;
        public string submittedAtUtc;
        public List<WiRRReportValue> answers = new List<WiRRReportValue>();
    }

    [Serializable]
    public sealed class WiRRReportValue
    {
        public string key;
        public string value;

        public WiRRReportValue() { }
        public WiRRReportValue(string key, string value)
        {
            this.key = key;
            this.value = value;
        }
    }
}

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
        public WiRRReportMetadata metadata = new WiRRReportMetadata();
        public WiRRTelemetryBundle telemetry = new WiRRTelemetryBundle();
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

    [Serializable]
    public sealed class WiRRReportMetadata
    {
        public string unityVersion;
        public string operatingSystem;
        public string graphicsDevice;
        public int graphicsMemoryMb;
        public string activeScene;
        public string renderPipeline;
        public string packageManifestSha256;
        public string projectStateSha256;
        public string gitCommit;
        public string gitBranch;
    }

    [Serializable]
    public sealed class WiRRTelemetryBundle
    {
        public string sessionStartedUtc;
        public string lastActivityUtc;
        public string currentStage;
        public double activeEditorSeconds;
        public double playModeSeconds;
        public int compileCount;
        public int snapshotCount;
        public List<WiRRStageDuration> stageDurations = new List<WiRRStageDuration>();
        public List<WiRRTelemetrySnapshot> snapshots = new List<WiRRTelemetrySnapshot>();
        public List<WiRRFileEvent> fileEvents = new List<WiRRFileEvent>();
        public List<WiRREditorEvent> editorEvents = new List<WiRREditorEvent>();
    }

    [Serializable]
    public sealed class WiRRStageDuration
    {
        public string stage;
        public string firstSeenUtc;
        public string lastSeenUtc;
        public double activeSeconds;
    }

    [Serializable]
    public sealed class WiRRTelemetrySnapshot
    {
        public string capturedAtUtc;
        public int labNumber;
        public string stage;
        public string scenePath;
        public bool sceneDirty;
        public bool playing;
        public int rootObjectCount;
        public int componentCount;
        public int projectFileCount;
        public long projectBytes;
        public string projectStateSha256;
    }

    [Serializable]
    public sealed class WiRRFileEvent
    {
        public string timestampUtc;
        public string stage;
        public string action;
        public string path;
        public long sizeBytes;
        public string sha256;
    }

    [Serializable]
    public sealed class WiRREditorEvent
    {
        public string timestampUtc;
        public string type;
        public string detail;
    }
}

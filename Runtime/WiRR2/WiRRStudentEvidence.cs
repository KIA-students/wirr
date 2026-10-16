using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace KIA.WiRR
{
    [Serializable] public sealed class WiRRStudentMetric
    {
        public string name, unit, unavailableReason;
        public bool available;
        public double value;
        public static WiRRStudentMetric Measured(string name, double value, string unit) =>
            new WiRRStudentMetric { name = name, value = value, unit = unit,
                available = !double.IsNaN(value) && !double.IsInfinity(value) };
    }

    [Serializable] public sealed class WiRRStudentTrial
    {
        public string id = Guid.NewGuid().ToString("N");
        public string timestampUtc = DateTime.UtcNow.ToString("O");
        public string deviceMode, participantMode = "SOLO", condition, validity = "VALID", reason;
        public List<WiRRStudentMetric> metrics = new List<WiRRStudentMetric>();
    }

    [Serializable] public sealed class WiRRStudentLabResult
    {
        public int labNumber;
        public string conclusion = "";
        public List<WiRRStudentTrial> trials = new List<WiRRStudentTrial>();
    }

    [Serializable] public sealed class WiRRStudentPendingTrial
    {
        public int labNumber;
        public WiRRStudentTrial trial;
    }

    [Serializable] public sealed class WiRRStudentSession
    {
        public string schema = "wirr-block-session/2.0-draft.1";
        public string id = Guid.NewGuid().ToString("N");
        public string createdAtUtc = DateTime.UtcNow.ToString("O");
        public string[] studentIndices;
        public string group;
        public int block;
        public List<WiRRStudentLabResult> labs = new List<WiRRStudentLabResult>();
        public WiRRStudentPendingTrial pendingTrial;

        public WiRRStudentLabResult Lab(int number)
        {
            if (number < 1 || number > 8 || (number + 1) / 2 != block) throw new ArgumentOutOfRangeException(nameof(number));
            var lab = labs.Find(x => x.labNumber == number);
            if (lab == null) { lab = new WiRRStudentLabResult { labNumber = number }; labs.Add(lab); }
            return lab;
        }

        public void BeginTrial(int number, string mode, string participant, string configuration)
        {
            Lab(number);
            if (pendingTrial != null) throw new InvalidOperationException("Poprzednia próba nie została zamknięta.");
            pendingTrial = new WiRRStudentPendingTrial { labNumber = number, trial = new WiRRStudentTrial {
                deviceMode = mode, participantMode = participant, condition = configuration,
                validity = "FAILED", reason = "Nie zarejestrowano zakończenia próby." } };
        }

        public void CompleteTrial(int number, WiRRStudentTrial result)
        {
            if (pendingTrial == null || pendingTrial.labNumber != number || result == null)
                throw new InvalidOperationException("Brak pasującej rozpoczętej próby.");
            result.id = pendingTrial.trial.id;
            result.timestampUtc = pendingTrial.trial.timestampUtc;
            result.condition = pendingTrial.trial.condition;
            var lab = Lab(number);
            if (lab.trials.Any(t => t.id == result.id)) throw new InvalidOperationException("Powtórzony identyfikator próby.");
            lab.trials.Add(result);
            pendingTrial = null;
        }

        public bool RecoverInterruptedTrial()
        {
            if (pendingTrial == null) return false;
            var trial = pendingTrial.trial;
            if (trial == null || string.IsNullOrWhiteSpace(trial.id)) throw new ArgumentException("Niepoprawny zapis rozpoczętej próby.");
            var lab = Lab(pendingTrial.labNumber);
            if (!lab.trials.Any(t => t.id == trial.id))
            {
                trial.validity = "FAILED";
                trial.reason = "Przerwana sesja: zachowano rozpoczęcie, brak potwierdzonego zakończenia.";
                if (trial.metrics == null) trial.metrics = new List<WiRRStudentMetric>();
                if (!trial.metrics.Any(m => m.name == "elapsed"))
                    trial.metrics.Add(new WiRRStudentMetric {
                        name = "elapsed", unit = "s", unavailableReason = "Brak końcowego pomiaru czasu przed restartem." });
                lab.trials.Add(trial);
            }
            pendingTrial = null;
            return true;
        }
    }

    /// <summary>Local session storage and an explicit adapter to the existing draft evidence grader.</summary>
    public static class WiRRStudentEvidence
    {
        public const string ExperimentVersion = "student-kit/0.1.6";

        public static string IdentityKey(IEnumerable<string> indices, string group, int block)
        {
            var text = group + ":" + block + ":" + string.Join(",", indices.OrderBy(x => x, StringComparer.Ordinal));
            using (var sha = SHA256.Create())
                return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(text))).Replace("-", "").ToLowerInvariant();
        }

        public static bool ValidIndices(string[] indices) => indices != null && indices.Length >= 1 && indices.Length <= 2
            && indices.Distinct().Count() == indices.Length
            && indices.All(x => x != null && System.Text.RegularExpressions.Regex.IsMatch(x, @"^[0-9]{5,12}$"));

        public static void AtomicWrite(string path, string content)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            var temporary = path + ".tmp";
            File.WriteAllText(temporary, content, new UTF8Encoding(false));
            if (File.Exists(path)) File.Replace(temporary, path, path + ".bak");
            else File.Move(temporary, path);
        }

        public static Dictionary<string, object> Report(WiRRStudentSession session, int labNumber, string unityVersion)
        {
            if (session.pendingTrial != null) throw new InvalidOperationException("Przed eksportem zamknij lub odzyskaj rozpoczętą próbę.");
            var lab = session.Lab(labNumber);
            var trials = new List<object>();
            foreach (var trial in lab.trials)
            {
                var metrics = new Dictionary<string, object>();
                foreach (var metric in trial.metrics)
                    metrics.Add(metric.name, new Dictionary<string, object> {
                        { "value", metric.available ? (object)metric.value : null }, { "unit", metric.unit },
                        { "unavailable_reason", metric.available ? null : metric.unavailableReason ?? "Brak pomiaru" } });
                trials.Add(new Dictionary<string, object> {
                    { "trial_id", trial.id }, { "timestampUtc", trial.timestampUtc },
                    { "condition", new Dictionary<string, object> { { "configuration", trial.condition } } },
                    { "device_mode", trial.deviceMode }, { "participant_mode", trial.participantMode },
                    { "validity", trial.validity }, { "reason", trial.reason }, { "metrics", metrics } });
            }
            var now = DateTime.UtcNow.ToString("O");
            return new Dictionary<string, object> {
                { "schemaVersion", "wirr-report/2.0-draft.1" },
                { "submissionId", session.id + "-lab" + labNumber.ToString("00") },
                { "teamId", session.id }, { "studentIndices", session.studentIndices }, { "group", session.group },
                { "labNumber", labNumber }, { "createdAtUtc", session.createdAtUtc },
                { "versions", new Dictionary<string, object> { { "unity", unityVersion },
                    { "package", "1.1.4" }, { "experiment", ExperimentVersion } } },
                { "activityTimeline", new object[] { new Dictionary<string, object> {
                    { "eventName", "local_evidence_exported" }, { "timestampUtc", now },
                    { "labNumber", labNumber }, { "durationSeconds", 0 } } } },
                { "activitySummary", new Dictionary<string, object> { { "eventCount", 1 } } },
                { "trials", trials }, { "checkpointEvidence", new Dictionary<string, object>() },
                { "conclusion", lab.conclusion }, { "status", "PROVISIONAL" },
                { "requiresInstructorApproval", true }, { "proposed_grade", null },
                { "achieved_checkpoints", new object[0] } };
        }

        // JsonUtility cannot serialize maps or nullable numbers. Use a narrow JSON writer
        // for the versioned adapter; local autosave uses ordinary JsonUtility DTOs.
        public static string Json(object value)
        {
            if (value == null) return "null";
            if (value is string text)
            {
                var builder = new StringBuilder("\"");
                foreach (var c in text)
                {
                    if (c == '"' || c == '\\') builder.Append('\\').Append(c);
                    else if (c < 32) builder.Append("\\u").Append(((int)c).ToString("x4"));
                    else builder.Append(c);
                }
                return builder.Append('"').ToString();
            }
            if (value is bool flag) return flag ? "true" : "false";
            if (value is IDictionary map)
            {
                var pairs = new List<string>();
                foreach (DictionaryEntry item in map) pairs.Add(Json((string)item.Key) + ":" + Json(item.Value));
                return "{" + string.Join(",", pairs) + "}";
            }
            if (value is IEnumerable sequence)
            {
                var items = new List<string>();
                foreach (var item in sequence) items.Add(Json(item));
                return "[" + string.Join(",", items) + "]";
            }
            if (value is double d && (double.IsNaN(d) || double.IsInfinity(d)))
                throw new ArgumentException("Nieskończony pomiar nie jest liczbą JSON.");
            if (value is float f && (float.IsNaN(f) || float.IsInfinity(f)))
                throw new ArgumentException("Nieskończony pomiar nie jest liczbą JSON.");
            if (value is int || value is double || value is float)
                return Convert.ToString(value, CultureInfo.InvariantCulture);
            throw new ArgumentException("Nieobsługiwany typ JSON: " + value.GetType());
        }

        public static double Percentile(IEnumerable<double> samples, double probability)
        {
            var values = samples.OrderBy(x => x).ToArray();
            if (values.Length == 0) throw new ArgumentException("Brak próbek.");
            var position = Math.Max(0, Math.Min(1, probability)) * (values.Length - 1);
            var lower = (int)Math.Floor(position);
            var upper = (int)Math.Ceiling(position);
            return values[lower] + (values[upper] - values[lower]) * (position - lower);
        }
    }
}

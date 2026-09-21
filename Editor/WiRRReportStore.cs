using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using KIA.WiRR;
using UnityEngine;

namespace KIA.WiRR.Editor
{
    internal static class WiRRReportStore
    {
        private static readonly string Root = Path.GetFullPath(Path.Combine(Application.dataPath, "../Library/WiRRReports"));

        public static WiRRReportDocument LoadOrCreate(int lab)
        {
            Directory.CreateDirectory(Root);
            var path = DraftPath(lab);
            if (File.Exists(path))
            {
                try
                {
                    var loaded = JsonUtility.FromJson<WiRRReportDocument>(File.ReadAllText(path));
                    if (loaded != null && loaded.labNumber == lab) return EnsureCollections(loaded);
                }
                catch (Exception exception) { Debug.LogWarning("[WiRR Reports] Draft load failed: " + exception.Message); }
            }
            var now = DateTime.UtcNow.ToString("O");
            var document = new WiRRReportDocument { submissionId = Guid.NewGuid().ToString("N"), labNumber = lab, createdAtUtc = now, updatedAtUtc = now };
            while (document.studentIndices.Count < 3) document.studentIndices.Add(string.Empty);
            Save(document);
            return document;
        }

        public static void Save(WiRRReportDocument document)
        {
            EnsureCollections(document);
            document.updatedAtUtc = DateTime.UtcNow.ToString("O");
            UpdateVariants(document);
            WiRRReportCalculator.Recalculate(document);
            Directory.CreateDirectory(Root);
            File.WriteAllText(DraftPath(document.labNumber), JsonUtility.ToJson(document, true));
        }

        public static string GetValue(WiRRReportDocument document, string key) => document.answers.FirstOrDefault(a => a.key == key)?.value ?? string.Empty;

        public static void SetValue(WiRRReportDocument document, string key, string value)
        {
            var item = document.answers.FirstOrDefault(a => a.key == key);
            if (item == null) document.answers.Add(new WiRRReportValue(key, value ?? string.Empty));
            else item.value = value ?? string.Empty;
        }

        public static IReadOnlyList<string> ValidateIdentity(WiRRReportDocument document)
        {
            var errors = new List<string>();
            if (document == null) { errors.Add("Brak dokumentu raportu."); return errors; }
            if (document.schemaVersion != "wirr-report/1.0") errors.Add("Nieobsługiwana wersja raportu.");
            if (document.labNumber < 1 || document.labNumber > 7) errors.Add("Nieprawidłowy numer laboratorium.");
            if (string.IsNullOrWhiteSpace(document.submissionId)) errors.Add("Brak identyfikatora raportu.");
            if (string.IsNullOrWhiteSpace(document.teamId)) errors.Add("Wpisz identyfikator zespołu.");
            var indices = (document.studentIndices ?? new List<string>()).Where(v => !string.IsNullOrWhiteSpace(v)).Select(v => v.Trim()).ToArray();
            if (indices.Length < 2 || indices.Length > 3) errors.Add("Podaj 2 albo 3 numery indeksów.");
            if (indices.Any(v => !long.TryParse(v, out _))) errors.Add("Numery indeksów mogą zawierać tylko cyfry.");
            if (indices.Distinct().Count() != indices.Length) errors.Add("Numery indeksów nie mogą się powtarzać.");
            return errors;
        }

        public static IReadOnlyList<string> Validate(WiRRReportDocument document, string throughCheckpoint = null)
        {
            var errors = new List<string>(ValidateIdentity(document));
            if (document == null || document.labNumber < 1 || document.labNumber > 7) return errors;

            foreach (var section in WiRRReportSchemaCatalog.Get(document.labNumber))
            {
                foreach (var field in section.Fields)
                {
                    var value = GetValue(document, field.Id);
                    if (string.IsNullOrWhiteSpace(value)) continue;
                    if (field.Kind == WiRRReportFieldKind.Number && !double.TryParse(value.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out _)) errors.Add($"Pole {field.Label} wymaga liczby.");
                    if (field.Kind == WiRRReportFieldKind.Integer && !long.TryParse(value, out _)) errors.Add($"Pole {field.Label} wymaga liczby całkowitej.");
                }

                if (!string.IsNullOrWhiteSpace(throughCheckpoint) && section.Checkpoint == throughCheckpoint)
                    break;
            }

            return errors;
        }

        public static string ExportFinal(WiRRReportDocument document, bool markSubmitted)
        {
            Save(document);
            if (markSubmitted) document.submittedAtUtc = DateTime.UtcNow.ToString("O");
            var folder = Path.Combine(Root, "exports");
            Directory.CreateDirectory(folder);
            var team = Sanitize(document.teamId, "team");
            var path = Path.Combine(folder, $"{team}-lab{document.labNumber:00}-{document.submissionId}.json");
            File.WriteAllText(path, JsonUtility.ToJson(document, true));
            return path;
        }

        public static string ToJson(WiRRReportDocument document) { Save(document); return JsonUtility.ToJson(document, true); }

        private static void UpdateVariants(WiRRReportDocument document)
        {
            var values = document.studentIndices.Where(v => long.TryParse(v, out _)).Select(long.Parse).ToArray();
            if (values.Length < 2)
            {
                SetValue(document, "variant.sum", string.Empty);
                for (var k = 1; k <= 5; k++) SetValue(document, $"variant.v{k}", string.Empty);
                return;
            }
            var sum = values.Sum();
            SetValue(document, "variant.sum", sum.ToString(CultureInfo.InvariantCulture));
            for (var k = 1; k <= 5; k++) SetValue(document, $"variant.v{k}", (1 + ((sum + 2L * (k - 1)) % 5L)).ToString(CultureInfo.InvariantCulture));
        }

        private static WiRRReportDocument EnsureCollections(WiRRReportDocument document)
        {
            document.answers = document.answers ?? new List<WiRRReportValue>();
            document.studentIndices = document.studentIndices ?? new List<string>();
            while (document.studentIndices.Count < 3) document.studentIndices.Add(string.Empty);
            return document;
        }

        private static string DraftPath(int lab) => Path.Combine(Root, $"lab{lab:00}-draft.json");

        internal static string Sanitize(string value, string fallback)
        {
            if (string.IsNullOrWhiteSpace(value)) return fallback;
            var chars = value.Trim().ToLowerInvariant().Select(c => char.IsLetterOrDigit(c) || c == '-' || c == '_' ? c : '-').ToArray();
            var result = new string(chars).Trim('-');
            return string.IsNullOrEmpty(result) ? fallback : result;
        }
    }
}

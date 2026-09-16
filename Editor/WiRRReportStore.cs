using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using KIA.WiRR;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

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
                    if (loaded != null && loaded.labNumber == lab)
                        return EnsureCollections(loaded);
                }
                catch (Exception exception) { Debug.LogWarning("[WiRR Reports] Draft load failed: " + exception.Message); }
            }

            var now = DateTime.UtcNow.ToString("O");
            var document = new WiRRReportDocument
            {
                submissionId = Guid.NewGuid().ToString("N"),
                labNumber = lab,
                createdAtUtc = now,
                updatedAtUtc = now
            };
            document.studentIndices.Add(string.Empty);
            document.studentIndices.Add(string.Empty);
            document.studentIndices.Add(string.Empty);
            Save(document);
            return document;
        }

        public static void Save(WiRRReportDocument document)
        {
            EnsureCollections(document);
            document.updatedAtUtc = DateTime.UtcNow.ToString("O");
            UpdateVariants(document);
            Directory.CreateDirectory(Root);
            File.WriteAllText(DraftPath(document.labNumber), JsonUtility.ToJson(document, true));
        }

        public static string GetValue(WiRRReportDocument document, string key)
        {
            return document.answers.FirstOrDefault(a => a.key == key)?.value ?? string.Empty;
        }

        public static void SetValue(WiRRReportDocument document, string key, string value)
        {
            var item = document.answers.FirstOrDefault(a => a.key == key);
            if (item == null)
            {
                item = new WiRRReportValue(key, value ?? string.Empty);
                document.answers.Add(item);
            }
            else item.value = value ?? string.Empty;
        }

        public static IReadOnlyList<string> Validate(WiRRReportDocument document)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(document.teamId)) errors.Add("Wpisz identyfikator zespołu.");
            var indices = document.studentIndices.Where(v => !string.IsNullOrWhiteSpace(v)).ToArray();
            if (indices.Length < 2 || indices.Length > 3) errors.Add("Zespół musi zawierać 2 albo 3 numery indeksów.");
            if (indices.Any(v => !long.TryParse(v, out _))) errors.Add("Numery indeksów muszą być liczbami.");

            foreach (var section in WiRRReportSchemaCatalog.Get(document.labNumber))
            foreach (var field in section.Fields)
            {
                if (field.Required && string.IsNullOrWhiteSpace(GetValue(document, field.Id)))
                    errors.Add($"Brak pola [{section.Checkpoint}] {field.Label}.");
                var value = GetValue(document, field.Id);
                if (string.IsNullOrWhiteSpace(value)) continue;
                if (field.Kind == WiRRReportFieldKind.Number && !double.TryParse(value.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out _))
                    errors.Add($"Pole {field.Label} wymaga liczby.");
                if (field.Kind == WiRRReportFieldKind.Integer && !long.TryParse(value, out _))
                    errors.Add($"Pole {field.Label} wymaga liczby całkowitej.");
            }
            return errors;
        }

        public static string ExportFinal(WiRRReportDocument document, bool markSubmitted)
        {
            Save(document);
            document.telemetry = WiRRTelemetryRecorder.GetCopy();
            if (markSubmitted) document.submittedAtUtc = DateTime.UtcNow.ToString("O");
            UpdateMetadata(document);
            var folder = Path.Combine(Root, "exports");
            Directory.CreateDirectory(folder);
            var team = Sanitize(document.teamId, "team");
            var path = Path.Combine(folder, $"{team}-lab{document.labNumber:00}-{document.submissionId}.json");
            File.WriteAllText(path, JsonUtility.ToJson(document, true));
            return path;
        }

        public static string ToJson(WiRRReportDocument document)
        {
            Save(document);
            document.telemetry = WiRRTelemetryRecorder.GetCopy();
            UpdateMetadata(document);
            return JsonUtility.ToJson(document, true);
        }

        private static void UpdateVariants(WiRRReportDocument document)
        {
            var values = document.studentIndices.Where(v => long.TryParse(v, out _)).Select(long.Parse).ToArray();
            if (values.Length < 2) return;
            var sum = values.Sum();
            SetValue(document, "variant.sum", sum.ToString(CultureInfo.InvariantCulture));
            for (var k = 1; k <= 5; k++)
            {
                var variant = 1 + ((sum + 2L * (k - 1)) % 5L);
                SetValue(document, $"variant.v{k}", variant.ToString(CultureInfo.InvariantCulture));
            }
        }

        private static void UpdateMetadata(WiRRReportDocument document)
        {
            document.metadata = document.metadata ?? new WiRRReportMetadata();
            document.metadata.unityVersion = Application.unityVersion;
            document.metadata.operatingSystem = SystemInfo.operatingSystem;
            document.metadata.graphicsDevice = SystemInfo.graphicsDeviceName;
            document.metadata.graphicsMemoryMb = SystemInfo.graphicsMemorySize;
            document.metadata.activeScene = SceneManager.GetActiveScene().path;
            document.metadata.renderPipeline = GraphicsSettings.defaultRenderPipeline != null ? GraphicsSettings.defaultRenderPipeline.GetType().Name : "Built-in";
            var projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            var manifest = Path.Combine(projectRoot, "Packages/manifest.json");
            document.metadata.packageManifestSha256 = File.Exists(manifest) ? HashFile(manifest) : string.Empty;
            var telemetry = WiRRTelemetryRecorder.GetCopy();
            document.metadata.projectStateSha256 = telemetry.snapshots != null ? telemetry.snapshots.LastOrDefault()?.projectStateSha256 ?? string.Empty : string.Empty;
            document.metadata.gitCommit = RunGit(projectRoot, "rev-parse HEAD");
            document.metadata.gitBranch = RunGit(projectRoot, "rev-parse --abbrev-ref HEAD");
        }

        private static string RunGit(string workingDirectory, string arguments)
        {
            try
            {
                var start = new ProcessStartInfo("git", arguments)
                {
                    WorkingDirectory = workingDirectory,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using var process = Process.Start(start);
                if (process == null) return string.Empty;
                if (!process.WaitForExit(1500)) { try { process.Kill(); } catch { } return string.Empty; }
                return process.ExitCode == 0 ? process.StandardOutput.ReadToEnd().Trim() : string.Empty;
            }
            catch { return string.Empty; }
        }

        private static WiRRReportDocument EnsureCollections(WiRRReportDocument document)
        {
            document.answers = document.answers ?? new List<WiRRReportValue>();
            document.studentIndices = document.studentIndices ?? new List<string>();
            while (document.studentIndices.Count < 3) document.studentIndices.Add(string.Empty);
            document.metadata = document.metadata ?? new WiRRReportMetadata();
            document.telemetry = document.telemetry ?? new WiRRTelemetryBundle();
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

        private static string HashFile(string path)
        {
            using var sha = SHA256.Create();
            using var stream = File.OpenRead(path);
            return BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", string.Empty).ToLowerInvariant();
        }
    }
}

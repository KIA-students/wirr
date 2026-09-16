using System;
using System.Collections.Generic;
using System.IO;
using KIA.WiRR;
using UnityEditor;
using UnityEngine;

namespace KIA.WiRR.Editor
{
    public sealed class WiRRReportWindow : EditorWindow
    {
        private const string LabPrefKey = "KIA.WiRR.SelectedLab";
        private const string RepoUrlKey = "KIA.WiRR.ReportRepoUrl";
        private const string RepoSlugKey = "KIA.WiRR.ReportRepoSlug";
        private const string BaseBranchKey = "KIA.WiRR.ReportBaseBranch";
        private const string ReportsPathKey = "KIA.WiRR.ReportPath";

        private int labNumber;
        private WiRRReportDocument document;
        private Vector2 scroll;
        private Vector2 validationScroll;
        private bool showTelemetry = true;
        private bool showSubmission;
        private bool showJson;
        private readonly Dictionary<string, bool> tableFoldouts = new Dictionary<string, bool>();
        private string[] validation = Array.Empty<string>();
        private string submissionStatus = string.Empty;

        [MenuItem("WiRR/Reports/Laboratory report form", priority = 10)]
        public static void Open()
        {
            var window = GetWindow<WiRRReportWindow>();
            window.titleContent = new GUIContent("WiRR Reports");
            window.minSize = new Vector2(620, 700);
            window.Show();
        }

        private void OnEnable()
        {
            labNumber = Mathf.Clamp(EditorPrefs.GetInt(LabPrefKey, 1), 1, 7);
            document = WiRRReportStore.LoadOrCreate(labNumber);
            WiRRTelemetryRecorder.SetStage("COMMON", $"Lab {labNumber:00} report opened");
        }

        private void OnDisable()
        {
            if (document != null) WiRRReportStore.Save(document);
        }

        private void OnGUI()
        {
            if (document == null) document = WiRRReportStore.LoadOrCreate(labNumber);
            scroll = EditorGUILayout.BeginScrollView(scroll);
            DrawHeader();
            DrawIdentity();
            DrawForm();
            DrawTelemetry();
            DrawValidation();
            DrawSubmission();
            DrawJsonPreview();
            EditorGUILayout.EndScrollView();
        }

        private void DrawHeader()
        {
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("WiRR Reports — formularz laboratoryjny", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Raport jest zapisywany automatycznie w Library/WiRRReports. Nie wpisuj nazwisk. Moduł rejestruje wyłącznie metadane projektu i względne ścieżki plików; nie zapisuje treści plików, nazwy konta systemowego ani ścieżki katalogu domowego.",
                MessageType.Info);

            var labels = WiRRLabCatalog.GetPopupLabels();
            var newLab = EditorGUILayout.Popup("Laboratorium", labNumber - 1, labels) + 1;
            if (newLab != labNumber)
            {
                WiRRReportStore.Save(document);
                labNumber = newLab;
                EditorPrefs.SetInt(LabPrefKey, labNumber);
                document = WiRRReportStore.LoadOrCreate(labNumber);
                validation = Array.Empty<string>();
                tableFoldouts.Clear();
                WiRRTelemetryRecorder.SetStage("COMMON", $"Switched to Lab {labNumber:00}");
                GUI.FocusControl(null);
            }
        }

        private void DrawIdentity()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Zespół i wariant", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            document.teamId = EditorGUILayout.TextField("Identyfikator zespołu", document.teamId ?? string.Empty);
            while (document.studentIndices.Count < 3) document.studentIndices.Add(string.Empty);
            document.studentIndices[0] = EditorGUILayout.TextField("Numer indeksu 1", document.studentIndices[0]);
            document.studentIndices[1] = EditorGUILayout.TextField("Numer indeksu 2", document.studentIndices[1]);
            document.studentIndices[2] = EditorGUILayout.TextField("Numer indeksu 3 (opcjonalnie)", document.studentIndices[2]);
            if (EditorGUI.EndChangeCheck())
            {
                WiRRTelemetryRecorder.SetStage("COMMON", "team/variant data edited");
                WiRRReportStore.Save(document);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("S", WiRRReportStore.GetValue(document, "variant.sum"), GUILayout.Width(150));
                for (var k = 1; k <= 5; k++)
                    EditorGUILayout.LabelField($"v{k}", WiRRReportStore.GetValue(document, $"variant.v{k}"), GUILayout.Width(80));
            }
        }

        private void DrawForm()
        {
            foreach (var section in WiRRReportSchemaCatalog.Get(labNumber))
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField(section.Title, EditorStyles.boldLabel);
                foreach (var field in section.Fields) DrawField(section.Checkpoint, field);
                foreach (var table in WiRRReportTableCatalog.Get(labNumber, section.Checkpoint)) DrawTable(section.Checkpoint, table);
            }
        }

        private void DrawField(string checkpoint, WiRRReportField field)
        {
            var oldValue = WiRRReportStore.GetValue(document, field.Id);
            var label = field.Label + (field.Required ? " *" : string.Empty) + (string.IsNullOrEmpty(field.Unit) ? "" : $" [{field.Unit}]");
            string newValue;
            switch (field.Kind)
            {
                case WiRRReportFieldKind.Multiline:
                    EditorGUILayout.LabelField(label);
                    newValue = EditorGUILayout.TextArea(oldValue, GUILayout.MinHeight(54));
                    break;
                case WiRRReportFieldKind.Boolean:
                    var booleanOptions = new[] { "— wybierz —", "tak", "nie" };
                    var booleanIndex = oldValue == "true" ? 1 : oldValue == "false" ? 2 : 0;
                    var selectedBoolean = EditorGUILayout.Popup(label, booleanIndex, booleanOptions);
                    newValue = selectedBoolean == 1 ? "true" : selectedBoolean == 2 ? "false" : string.Empty;
                    break;
                case WiRRReportFieldKind.Choice:
                    var choiceOptions = new string[field.Choices.Length + 1];
                    choiceOptions[0] = "— wybierz —";
                    Array.Copy(field.Choices, 0, choiceOptions, 1, field.Choices.Length);
                    var oldChoice = Array.IndexOf(field.Choices, oldValue);
                    var selectedChoice = EditorGUILayout.Popup(label, oldChoice >= 0 ? oldChoice + 1 : 0, choiceOptions);
                    newValue = selectedChoice > 0 ? field.Choices[selectedChoice - 1] : string.Empty;
                    break;
                default:
                    newValue = EditorGUILayout.TextField(label, oldValue);
                    break;
            }
            if (!string.IsNullOrWhiteSpace(field.Help)) EditorGUILayout.HelpBox(field.Help, MessageType.None);
            if (!string.Equals(oldValue, newValue, StringComparison.Ordinal))
            {
                WiRRTelemetryRecorder.SetStage(checkpoint, field.Id);
                WiRRReportStore.SetValue(document, field.Id, newValue);
                WiRRReportStore.Save(document);
            }
        }

        private void DrawTable(string checkpoint, WiRRReportTable table)
        {
            var foldoutKey = $"{labNumber}:{checkpoint}:{table.Id}";
            if (!tableFoldouts.ContainsKey(foldoutKey)) tableFoldouts[foldoutKey] = false;
            EditorGUILayout.Space(5);
            tableFoldouts[foldoutKey] = EditorGUILayout.Foldout(tableFoldouts[foldoutKey], $"Tabela: {table.Label}", true);
            if (!tableFoldouts[foldoutKey]) return;

            if (!string.IsNullOrWhiteSpace(table.Help)) EditorGUILayout.HelpBox(table.Help, MessageType.None);
            EditorGUILayout.HelpBox("Każda komórka jest zapisywana jako osobny, stabilny klucz JSON. Wpisuj dane surowe z pomiarów; mediany i wnioski pozostają w polach powyżej.", MessageType.None);

            foreach (var row in table.Rows)
            {
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    EditorGUILayout.LabelField(row.Label, EditorStyles.miniBoldLabel);
                    foreach (var column in table.Columns)
                    {
                        var key = WiRRReportTableCatalog.CellKey(table, row, column);
                        var oldValue = WiRRReportStore.GetValue(document, key);
                        var newValue = EditorGUILayout.TextField(column.Label, oldValue);
                        if (!string.Equals(oldValue, newValue, StringComparison.Ordinal))
                        {
                            WiRRTelemetryRecorder.SetStage(checkpoint, key);
                            WiRRReportStore.SetValue(document, key, newValue);
                            WiRRReportStore.Save(document);
                        }
                    }
                }
            }
        }

        private void DrawTelemetry()
        {
            EditorGUILayout.Space(12);
            showTelemetry = EditorGUILayout.Foldout(showTelemetry, "Metadane i oś czasu pracy", true);
            if (!showTelemetry) return;
            var telemetry = WiRRTelemetryRecorder.GetCopy();
            EditorGUILayout.HelpBox(
                "Snapshot wykonywany jest co 10 min oraz przy ważnych zmianach stanu. Hash SHA-256 służy do wykrywania zmian, a nie do odtwarzania zawartości pliku. Czas per etap jest przypisywany do ostatnio edytowanego checkpointu formularza.", MessageType.None);
            EditorGUILayout.LabelField("Start sesji", telemetry.sessionStartedUtc ?? "—");
            EditorGUILayout.LabelField("Bieżący etap", telemetry.currentStage ?? "—");
            EditorGUILayout.LabelField("Aktywny czas Editora", FormatSeconds(telemetry.activeEditorSeconds));
            EditorGUILayout.LabelField("Play Mode", FormatSeconds(telemetry.playModeSeconds));
            EditorGUILayout.LabelField("Kompilacje", telemetry.compileCount.ToString());
            EditorGUILayout.LabelField("Snapshoty", telemetry.snapshotCount.ToString());
            EditorGUILayout.LabelField("Zdarzenia plikowe", telemetry.fileEvents.Count.ToString());
            if (telemetry.stageDurations != null && telemetry.stageDurations.Count > 0)
            {
                EditorGUILayout.Space(4);
                EditorGUILayout.LabelField("Czas aktywny według etapu", EditorStyles.miniBoldLabel);
                foreach (var stage in telemetry.stageDurations)
                    EditorGUILayout.LabelField(stage.stage ?? "—", FormatSeconds(stage.activeSeconds));
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Zapisz snapshot teraz")) WiRRTelemetryRecorder.CaptureNow("manual-from-report-form");
                if (GUILayout.Button("Otwórz folder danych lokalnych"))
                {
                    var path = Path.GetFullPath(Path.Combine(Application.dataPath, "../Library/WiRRReports"));
                    EditorUtility.RevealInFinder(path);
                }
            }
        }

        private void DrawValidation()
        {
            EditorGUILayout.Space(12);
            EditorGUILayout.LabelField("Walidacja raportu", EditorStyles.boldLabel);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Sprawdź kompletność", GUILayout.Height(28)))
                    validation = new List<string>(WiRRReportStore.Validate(document)).ToArray();
                if (GUILayout.Button("Zapisz szkic", GUILayout.Height(28)))
                {
                    WiRRReportStore.Save(document);
                    submissionStatus = "Szkic zapisany lokalnie.";
                }
                if (GUILayout.Button("Eksportuj JSON", GUILayout.Height(28)))
                {
                    var path = WiRRReportStore.ExportFinal(document, false);
                    EditorUtility.RevealInFinder(path);
                    submissionStatus = "Wyeksportowano: " + path;
                }
            }
            if (validation.Length == 0)
                EditorGUILayout.HelpBox("Brak wykrytych braków albo walidacja nie została jeszcze uruchomiona.", MessageType.Info);
            else
            {
                validationScroll = EditorGUILayout.BeginScrollView(validationScroll, GUILayout.MaxHeight(130));
                foreach (var item in validation) EditorGUILayout.HelpBox(item, MessageType.Warning);
                EditorGUILayout.EndScrollView();
            }
        }

        private void DrawSubmission()
        {
            EditorGUILayout.Space(12);
            showSubmission = EditorGUILayout.Foldout(showSubmission, "Przesłanie do repozytorium", true);
            if (!showSubmission) return;
            EditorGUILayout.HelpBox(
                "Pakiet nie przechowuje tokenu GitHub. Wysyłka używa lokalnego Git/Git Credential Manager/SSH. Jeżeli zainstalowany i zalogowany jest GitHub CLI (gh), po pushu zostanie automatycznie utworzony Pull Request.", MessageType.Info);

            var repoUrl = EditorPrefs.GetString(RepoUrlKey, WiRRGitSubmission.DefaultRepositoryUrl);
            var repoSlug = EditorPrefs.GetString(RepoSlugKey, WiRRGitSubmission.DefaultRepositorySlug);
            var baseBranch = EditorPrefs.GetString(BaseBranchKey, WiRRGitSubmission.DefaultBaseBranch);
            var reportsPath = EditorPrefs.GetString(ReportsPathKey, WiRRGitSubmission.DefaultReportsPath);
            EditorGUI.BeginChangeCheck();
            repoUrl = EditorGUILayout.TextField("Git repository URL", repoUrl);
            repoSlug = EditorGUILayout.TextField("GitHub repo", repoSlug);
            baseBranch = EditorGUILayout.TextField("Gałąź bazowa", baseBranch);
            reportsPath = EditorGUILayout.TextField("Folder raportów", reportsPath);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetString(RepoUrlKey, repoUrl);
                EditorPrefs.SetString(RepoSlugKey, repoSlug);
                EditorPrefs.SetString(BaseBranchKey, baseBranch);
                EditorPrefs.SetString(ReportsPathKey, reportsPath);
            }

            GUI.enabled = !EditorApplication.isCompiling;
            if (GUILayout.Button("Wyślij raport i utwórz PR", GUILayout.Height(34)))
            {
                WiRRTelemetryRecorder.CaptureNow("before-submission");
                var result = WiRRGitSubmission.Submit(document, repoUrl, repoSlug, baseBranch, reportsPath);
                submissionStatus = result.Message + (result.Success ? $" Gałąź: {result.Branch}; plik: {result.RepositoryPath}" : string.Empty);
                validation = new List<string>(WiRRReportStore.Validate(document)).ToArray();
            }
            GUI.enabled = true;
            if (!string.IsNullOrWhiteSpace(submissionStatus))
                EditorGUILayout.HelpBox(submissionStatus, submissionStatus.StartsWith("Raport wysłany", StringComparison.Ordinal) ? MessageType.Info : MessageType.None);
        }

        private void DrawJsonPreview()
        {
            EditorGUILayout.Space(12);
            showJson = EditorGUILayout.Foldout(showJson, "Podgląd JSON przekazywanego do CI", true);
            if (!showJson) return;
            var json = WiRRReportStore.ToJson(document);
            EditorGUILayout.TextArea(json, GUILayout.MinHeight(180));
        }

        private static string FormatSeconds(double seconds)
        {
            var span = TimeSpan.FromSeconds(Math.Max(0, seconds));
            return $"{(int)span.TotalHours:00}:{span.Minutes:00}:{span.Seconds:00}";
        }
    }
}

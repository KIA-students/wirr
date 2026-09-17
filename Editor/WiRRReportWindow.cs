using System;
using System.Collections.Generic;
using KIA.WiRR;
using UnityEditor;
using UnityEngine;

namespace KIA.WiRR.Editor
{
    public sealed class WiRRReportWindow : EditorWindow
    {
        private const string LabPrefKey = "KIA.WiRR.SelectedLab";
        private int labNumber;
        private WiRRReportDocument document;
        private Vector2 scroll;
        private readonly Dictionary<string, bool> tableFoldouts = new Dictionary<string, bool>();
        private WiRRReportEvaluation evaluation;
        private string submissionStatus = string.Empty;

        [MenuItem("WiRR/Reports/Laboratory report form", priority = 10)]
        public static void Open()
        {
            var window = GetWindow<WiRRReportWindow>();
            window.titleContent = WiRRBranding.Title("WiRR Report");
            window.minSize = new Vector2(620, 700);
            window.Show();
        }

        private void OnEnable()
        {
            titleContent = WiRRBranding.Title("WiRR Report");
            labNumber = Mathf.Clamp(EditorPrefs.GetInt(LabPrefKey, 1), 1, 7);
            document = WiRRReportStore.LoadOrCreate(labNumber);
        }

        private void OnDisable() { if (document != null) WiRRReportStore.Save(document); }

        private void OnGUI()
        {
            if (document == null) document = WiRRReportStore.LoadOrCreate(labNumber);
            scroll = EditorGUILayout.BeginScrollView(scroll);
            DrawHeader();
            DrawIdentity();
            DrawForm();
            DrawActions();
            EditorGUILayout.EndScrollView();
        }

        private void DrawHeader()
        {
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Raport laboratoryjny WiRR", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Wypełnij raport od checkpointu 3.0 w górę. Pola oznaczone * są wymagane. Raport zapisuje się automatycznie. Nie wpisuj imion ani nazwisk.", MessageType.Info);
            var labels = WiRRLabCatalog.GetPopupLabels();
            var newLab = EditorGUILayout.Popup("Laboratorium", labNumber - 1, labels) + 1;
            if (newLab == labNumber) return;
            WiRRReportStore.Save(document);
            labNumber = newLab;
            EditorPrefs.SetInt(LabPrefKey, labNumber);
            document = WiRRReportStore.LoadOrCreate(labNumber);
            evaluation = null;
            submissionStatus = string.Empty;
            tableFoldouts.Clear();
            GUI.FocusControl(null);
        }

        private void DrawIdentity()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Zespół", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            document.teamId = EditorGUILayout.TextField("Identyfikator zespołu", document.teamId ?? string.Empty);
            while (document.studentIndices.Count < 3) document.studentIndices.Add(string.Empty);
            document.studentIndices[0] = EditorGUILayout.TextField("Numer indeksu 1", document.studentIndices[0]);
            document.studentIndices[1] = EditorGUILayout.TextField("Numer indeksu 2", document.studentIndices[1]);
            document.studentIndices[2] = EditorGUILayout.TextField("Numer indeksu 3 (opcjonalnie)", document.studentIndices[2]);
            if (EditorGUI.EndChangeCheck()) { WiRRReportStore.Save(document); evaluation = null; }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Wariant:", GUILayout.Width(55));
                for (var k = 1; k <= 5; k++) EditorGUILayout.LabelField($"v{k}={WiRRReportStore.GetValue(document, $"variant.v{k}")}", GUILayout.Width(75));
            }
        }

        private void DrawForm()
        {
            foreach (var section in WiRRReportSchemaCatalog.Get(labNumber))
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField($"Checkpoint {section.Checkpoint} — {section.Title}", EditorStyles.boldLabel);
                foreach (var field in section.Fields) DrawField(field);
                foreach (var table in WiRRReportTableCatalog.Get(labNumber, section.Checkpoint)) DrawTable(section.Checkpoint, table);
            }
        }

        private void DrawField(WiRRReportField field)
        {
            var oldValue = WiRRReportStore.GetValue(document, field.Id);
            var label = field.Label + (field.Required ? " *" : string.Empty) + (string.IsNullOrEmpty(field.Unit) ? "" : $" [{field.Unit}]");
            string newValue;
            switch (field.Kind)
            {
                case WiRRReportFieldKind.Multiline:
                    EditorGUILayout.LabelField(label); newValue = EditorGUILayout.TextArea(oldValue, GUILayout.MinHeight(54)); break;
                case WiRRReportFieldKind.Boolean:
                    var bo = new[] { "— wybierz —", "tak", "nie" }; var bi = oldValue == "true" ? 1 : oldValue == "false" ? 2 : 0;
                    var bs = EditorGUILayout.Popup(label, bi, bo); newValue = bs == 1 ? "true" : bs == 2 ? "false" : string.Empty; break;
                case WiRRReportFieldKind.Choice:
                    var choices = new string[field.Choices.Length + 1]; choices[0] = "— wybierz —"; Array.Copy(field.Choices, 0, choices, 1, field.Choices.Length);
                    var oldChoice = Array.IndexOf(field.Choices, oldValue); var selected = EditorGUILayout.Popup(label, oldChoice >= 0 ? oldChoice + 1 : 0, choices);
                    newValue = selected > 0 ? field.Choices[selected - 1] : string.Empty; break;
                default: newValue = EditorGUILayout.TextField(label, oldValue); break;
            }
            if (!string.IsNullOrWhiteSpace(field.Help)) EditorGUILayout.HelpBox(field.Help, MessageType.None);
            if (oldValue == newValue) return;
            WiRRReportStore.SetValue(document, field.Id, newValue);
            WiRRReportStore.Save(document);
            evaluation = null;
        }

        private void DrawTable(string checkpoint, WiRRReportTable table)
        {
            var key = $"{labNumber}:{checkpoint}:{table.Id}";
            if (!tableFoldouts.ContainsKey(key)) tableFoldouts[key] = false;
            tableFoldouts[key] = EditorGUILayout.Foldout(tableFoldouts[key], "Dane pomiarowe: " + table.Label, true);
            if (!tableFoldouts[key]) return;
            if (!string.IsNullOrWhiteSpace(table.Help)) EditorGUILayout.HelpBox(table.Help, MessageType.None);
            foreach (var row in table.Rows)
            {
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    EditorGUILayout.LabelField(row.Label, EditorStyles.miniBoldLabel);
                    foreach (var column in table.Columns)
                    {
                        var cell = WiRRReportTableCatalog.CellKey(table, row, column);
                        var oldValue = WiRRReportStore.GetValue(document, cell);
                        var newValue = EditorGUILayout.TextField(column.Label, oldValue);
                        if (oldValue == newValue) continue;
                        WiRRReportStore.SetValue(document, cell, newValue);
                        WiRRReportStore.Save(document);
                        evaluation = null;
                    }
                }
            }
        }

        private void DrawActions()
        {
            EditorGUILayout.Space(14);
            EditorGUILayout.LabelField("Gotowe?", EditorStyles.boldLabel);
            if (GUILayout.Button("Sprawdź raport", GUILayout.Height(32))) evaluation = WiRRReportEvaluator.Evaluate(document);
            if (evaluation != null) DrawEvaluation();

            GUI.enabled = !EditorApplication.isCompiling;
            if (GUILayout.Button("Wyślij raport", GUILayout.Height(36)))
            {
                evaluation = WiRRReportEvaluator.Evaluate(document);
                if (evaluation.BlockingIssues.Count > 0 || string.IsNullOrWhiteSpace(evaluation.SuggestedGrade))
                    submissionStatus = "Raport nie jest jeszcze gotowy do wysłania. Popraw wskazane braki.";
                else
                {
                    var result = WiRRGitSubmission.Submit(document, WiRRGitSubmission.DefaultRepositoryUrl, WiRRGitSubmission.DefaultRepositorySlug, WiRRGitSubmission.DefaultBaseBranch, WiRRGitSubmission.DefaultReportsPath);
                    submissionStatus = result.Message;
                }
            }
            GUI.enabled = true;
            if (!string.IsNullOrWhiteSpace(submissionStatus)) EditorGUILayout.HelpBox(submissionStatus, MessageType.Info);
        }

        private void DrawEvaluation()
        {
            foreach (var issue in evaluation.BlockingIssues) EditorGUILayout.HelpBox(issue, MessageType.Error);
            foreach (var cp in evaluation.Checkpoints)
            {
                var text = $"{cp.Checkpoint}: {(cp.Complete ? "OK" : "braki")}";
                if (cp.Reasons.Count > 0) text += " — " + string.Join("; ", cp.Reasons);
                EditorGUILayout.HelpBox(text, cp.Complete ? MessageType.Info : MessageType.Warning);
            }
            var grade = string.IsNullOrWhiteSpace(evaluation.SuggestedGrade) ? "brak kompletnego checkpointu" : $"kompletny zakres do {evaluation.SuggestedGrade}";
            EditorGUILayout.LabelField("Wynik kontroli: " + grade, EditorStyles.boldLabel);
        }
    }
}

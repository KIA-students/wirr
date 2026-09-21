using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly Dictionary<string, bool> sectionFoldouts = new Dictionary<string, bool>();
        private WiRRReportEvaluation evaluation;
        private string submissionStatus = string.Empty;

        [MenuItem("WiRR/Raporty/Formularz raportu laboratoryjnego", priority = 10)]
        public static void Open()
        {
            var window = GetWindow<WiRRReportWindow>();
            window.titleContent = WiRRBranding.Title("WiRR — raport");
            window.minSize = new Vector2(680, 720);
            window.Show();
        }

        private void OnEnable()
        {
            titleContent = WiRRBranding.Title("WiRR — raport");
            labNumber = Mathf.Clamp(EditorPrefs.GetInt(LabPrefKey, 1), 1, 7);
            document = WiRRReportStore.LoadOrCreate(labNumber);
            InitializeSectionFoldouts();
        }

        private void OnDisable()
        {
            if (document != null)
                WiRRReportStore.Save(document);
        }

        private void OnGUI()
        {
            if (document == null)
                document = WiRRReportStore.LoadOrCreate(labNumber);

            scroll = EditorGUILayout.BeginScrollView(scroll);
            DrawHeader();
            DrawIdentity();
            DrawForm();
            DrawActions();
            EditorGUILayout.Space(12);
            EditorGUILayout.EndScrollView();
        }

        private void DrawHeader()
        {
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Raport laboratoryjny WiRR", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Wypełniaj formularz na podstawie rzeczywiście wykonanych pomiarów. Do wysłania raportu wymagany jest kompletny etap 3.0. Etapy 3.5–5.0 są opcjonalne. Pola oznaczone „automatycznie” są wyliczane z danych w tabelach i nie trzeba ich przepisywać ręcznie.",
                MessageType.Info);

            var labels = WiRRLabCatalog.GetPopupLabels();
            var newLab = EditorGUILayout.Popup("Laboratorium", labNumber - 1, labels) + 1;
            if (newLab == labNumber)
                return;

            WiRRReportStore.Save(document);
            labNumber = newLab;
            EditorPrefs.SetInt(LabPrefKey, labNumber);
            document = WiRRReportStore.LoadOrCreate(labNumber);
            evaluation = null;
            submissionStatus = string.Empty;
            tableFoldouts.Clear();
            sectionFoldouts.Clear();
            InitializeSectionFoldouts();
            GUI.FocusControl(null);
        }

        private void DrawIdentity()
        {
            EditorGUILayout.Space(10);
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Dane zespołu", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox(
                    "Podaj identyfikator zespołu i numery indeksów 2–3 osób. Nie wpisuj imion ani nazwisk. Na podstawie numerów indeksów system automatycznie obliczy warianty v1–v5.",
                    MessageType.None);

                EditorGUI.BeginChangeCheck();
                document.teamId = EditorGUILayout.TextField(
                    new GUIContent("Identyfikator zespołu", "Krótka nazwa zespołu używana w nazwie zgłoszenia, np. ZESPOL-04."),
                    document.teamId ?? string.Empty);

                while (document.studentIndices.Count < 3)
                    document.studentIndices.Add(string.Empty);

                document.studentIndices[0] = EditorGUILayout.TextField("Numer indeksu — osoba 1", document.studentIndices[0]);
                document.studentIndices[1] = EditorGUILayout.TextField("Numer indeksu — osoba 2", document.studentIndices[1]);
                document.studentIndices[2] = EditorGUILayout.TextField("Numer indeksu — osoba 3 (opcjonalnie)", document.studentIndices[2]);

                if (EditorGUI.EndChangeCheck())
                {
                    WiRRReportStore.Save(document);
                    evaluation = null;
                }

                EditorGUILayout.Space(4);
                var sum = WiRRReportStore.GetValue(document, "variant.sum");
                EditorGUILayout.LabelField(
                    string.IsNullOrWhiteSpace(sum) ? "Suma indeksów S: —" : $"Suma indeksów S: {sum}",
                    EditorStyles.miniBoldLabel);

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Warianty:", GUILayout.Width(65));
                    for (var k = 1; k <= 5; k++)
                    {
                        var value = WiRRReportStore.GetValue(document, $"variant.v{k}");
                        EditorGUILayout.LabelField($"v{k} = {(string.IsNullOrWhiteSpace(value) ? "—" : value)}", GUILayout.Width(72));
                    }
                }
                EditorGUILayout.LabelField(
                    "Suma S oraz warianty v1–v5 są obliczane automatycznie.",
                    EditorStyles.wordWrappedMiniLabel);
            }
        }

        private void DrawForm()
        {
            foreach (var section in WiRRReportSchemaCatalog.Get(labNumber))
                DrawSection(section);
        }

        private void DrawSection(WiRRReportSection section)
        {
            var key = $"{labNumber}:{section.Checkpoint}";
            if (!sectionFoldouts.ContainsKey(key))
                sectionFoldouts[key] = section.Checkpoint == "3.0";

            EditorGUILayout.Space(8);
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                var required = section.Checkpoint == "3.0";
                var optional = section.Checkpoint != "3.0" && section.Checkpoint != "COMMON";
                var status = required ? " — wymagany do wysłania" : optional ? " — opcjonalny" : " — opcjonalne informacje";
                var title = section.Checkpoint == "COMMON"
                    ? section.Title + status
                    : $"Etap {section.Title}" + status;

                sectionFoldouts[key] = EditorGUILayout.Foldout(sectionFoldouts[key], title, true, EditorStyles.foldoutHeader);

                DrawSectionProgress(section, required);

                if (!sectionFoldouts[key])
                    return;

                EditorGUILayout.HelpBox(SectionGuidance(section), required ? MessageType.Info : MessageType.None);

                foreach (var field in section.Fields)
                    DrawField(field);

                foreach (var table in WiRRReportTableCatalog.Get(labNumber, section.Checkpoint))
                    DrawTable(section.Checkpoint, table);
            }
        }

        private void DrawSectionProgress(WiRRReportSection section, bool requiredForSubmission)
        {
            var requiredFields = section.Fields.Where(f => f.Required).ToArray();
            var filled = requiredFields.Count(f => !string.IsNullOrWhiteSpace(WiRRReportStore.GetValue(document, f.Id)));
            var suffix = requiredForSubmission ? " · wymagane do wysłania" : string.Empty;
            if (requiredFields.Length > 0)
            {
                EditorGUILayout.LabelField(
                    $"Pola wymagane: {filled}/{requiredFields.Length}{suffix}",
                    EditorStyles.miniLabel);
            }

            var tables = WiRRReportTableCatalog.Get(labNumber, section.Checkpoint);
            if (tables.Count == 0)
                return;

            var tablesWithData = 0;
            foreach (var table in tables)
            {
                var hasData = table.Rows.Any(row => table.Columns.Any(column =>
                    !string.IsNullOrWhiteSpace(WiRRReportStore.GetValue(
                        document,
                        WiRRReportTableCatalog.CellKey(table, row, column)))));
                if (hasData)
                    tablesWithData++;
            }

            EditorGUILayout.LabelField(
                $"Tabele pomiarowe z danymi: {tablesWithData}/{tables.Count}{suffix}",
                EditorStyles.miniLabel);
        }

        private string SectionGuidance(WiRRReportSection section)
        {
            if (section.Checkpoint == "COMMON")
                return "Te informacje pomagają odtworzyć warunki eksperymentu. Wypełnij je, jeśli mają znaczenie dla interpretacji wyników.";

            if (section.Checkpoint == "3.0")
                return "Najpierw wpisz surowe wyniki pomiarów w tabelach. Wartości pochodne, takie jak mediana, są obliczane automatycznie. Następnie uzupełnij wymagane pola opisowe i sformułuj wniosek oparty na danych.";

            return "Ten etap jest opcjonalny. Jeśli go realizujesz, uzupełnij dane pomiarowe i pola wymagane w tym etapie. Nieukończony etap powyżej 3.0 nie blokuje wysłania raportu.";
        }

        private void DrawField(WiRRReportField field)
        {
            EditorGUILayout.Space(4);

            var oldValue = WiRRReportStore.GetValue(document, field.Id);
            var requiredMark = field.Required ? " *" : string.Empty;
            var unit = string.IsNullOrEmpty(field.Unit) ? string.Empty : $" [{field.Unit}]";
            var automatic = WiRRReportCalculator.IsDerivedField(field.Id);
            var labelText = field.Label + unit + requiredMark + (automatic ? " — automatycznie" : string.Empty);
            var automaticDescription = automatic ? WiRRReportCalculator.DerivedFieldDescription(field.Id) : string.Empty;
            var label = new GUIContent(labelText, automatic ? automaticDescription : field.Help);

            string newValue;

            using (new EditorGUI.DisabledScope(automatic))
            {
                switch (field.Kind)
                {
                    case WiRRReportFieldKind.Multiline:
                        EditorGUILayout.LabelField(label);
                        newValue = EditorGUILayout.TextArea(oldValue, GUILayout.MinHeight(58));
                        break;

                    case WiRRReportFieldKind.Boolean:
                        var booleanOptions = new[] { "— wybierz —", "tak", "nie" };
                        var booleanIndex = oldValue == "true" ? 1 : oldValue == "false" ? 2 : 0;
                        var selectedBoolean = EditorGUILayout.Popup(label, booleanIndex, booleanOptions);
                        newValue = selectedBoolean == 1 ? "true" : selectedBoolean == 2 ? "false" : string.Empty;
                        break;

                    case WiRRReportFieldKind.Choice:
                        var choices = new string[field.Choices.Length + 1];
                        choices[0] = "— wybierz —";
                        Array.Copy(field.Choices, 0, choices, 1, field.Choices.Length);
                        var oldChoice = Array.IndexOf(field.Choices, oldValue);
                        var selected = EditorGUILayout.Popup(label, oldChoice >= 0 ? oldChoice + 1 : 0, choices);
                        newValue = selected > 0 ? field.Choices[selected - 1] : string.Empty;
                        break;

                    default:
                        newValue = EditorGUILayout.TextField(label, oldValue);
                        break;
                }
            }

            if (automatic)
            {
                EditorGUILayout.LabelField(
                    automaticDescription + " Pole zostanie uzupełnione po wpisaniu kompletu wymaganych danych źródłowych.",
                    EditorStyles.wordWrappedMiniLabel);
                return;
            }

            if (!string.IsNullOrWhiteSpace(field.Help))
                EditorGUILayout.LabelField(field.Help, EditorStyles.wordWrappedMiniLabel);

            if (oldValue == newValue)
                return;

            WiRRReportStore.SetValue(document, field.Id, newValue);
            WiRRReportStore.Save(document);
            evaluation = null;
        }

        private void DrawTable(string checkpoint, WiRRReportTable table)
        {
            var key = $"{labNumber}:{checkpoint}:{table.Id}";
            if (!tableFoldouts.ContainsKey(key))
                tableFoldouts[key] = checkpoint == "3.0";

            EditorGUILayout.Space(8);
            tableFoldouts[key] = EditorGUILayout.Foldout(
                tableFoldouts[key],
                "Dane pomiarowe — " + table.Label,
                true);

            if (!tableFoldouts[key])
                return;

            EditorGUILayout.LabelField(
                "Wpisz wyłącznie wartości zmierzone lub zaobserwowane. Pola oznaczone „automatycznie” są wyliczane przez formularz.",
                EditorStyles.wordWrappedMiniLabel);

            if (!string.IsNullOrWhiteSpace(table.Help))
                EditorGUILayout.HelpBox(table.Help, MessageType.None);

            foreach (var row in table.Rows)
            {
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    EditorGUILayout.LabelField($"Warunek / próba: {row.Label}", EditorStyles.miniBoldLabel);

                    foreach (var column in table.Columns)
                    {
                        var cell = WiRRReportTableCatalog.CellKey(table, row, column);
                        var oldValue = WiRRReportStore.GetValue(document, cell);
                        var automatic = WiRRReportCalculator.IsDerivedCell(table, column);
                        var displayLabel = automatic ? column.Label + " — automatycznie" : column.Label;
                        var content = new GUIContent(
                            displayLabel,
                            automatic ? WiRRReportCalculator.DerivedDescription(table, column) : string.Empty);

                        string newValue;
                        using (new EditorGUI.DisabledScope(automatic))
                            newValue = automatic
                                ? EditorGUILayout.TextField(content, oldValue)
                                : DrawEditableTableCell(content, column, oldValue);

                        if (automatic || oldValue == newValue)
                            continue;

                        WiRRReportStore.SetValue(document, cell, newValue);
                        WiRRReportStore.Save(document);
                        evaluation = null;
                    }
                }
            }
        }

        private static string DrawEditableTableCell(GUIContent label, WiRRTableAxis column, string oldValue)
        {
            if (column.Id == "status_pass_fail_nv")
            {
                var values = new[] { "— wybierz —", "PASS — zaliczony", "FAIL — niezaliczony", "NV — niezweryfikowany" };
                var index = oldValue.StartsWith("PASS", StringComparison.OrdinalIgnoreCase) ? 1
                    : oldValue.StartsWith("FAIL", StringComparison.OrdinalIgnoreCase) ? 2
                    : oldValue.StartsWith("NV", StringComparison.OrdinalIgnoreCase) ? 3
                    : 0;
                var selected = EditorGUILayout.Popup(label, index, values);
                return selected == 1 ? "PASS" : selected == 2 ? "FAIL" : selected == 3 ? "NV" : string.Empty;
            }

            if (column.Id == "p" || column.Id == "s" || column.Id == "ocena_1_5" || column.Id == "jakość_1_5")
                return DrawNumericChoice(label, oldValue, 1, 5);

            if (column.Id == "komfort_0_10")
                return DrawNumericChoice(label, oldValue, 0, 10);

            if (column.Id == "hit_miss")
            {
                var values = new[] { "— wybierz —", "HIT — trafienie", "MISS — brak trafienia" };
                var index = oldValue.Equals("HIT", StringComparison.OrdinalIgnoreCase) ? 1
                    : oldValue.Equals("MISS", StringComparison.OrdinalIgnoreCase) ? 2
                    : 0;
                var selected = EditorGUILayout.Popup(label, index, values);
                return selected == 1 ? "HIT" : selected == 2 ? "MISS" : string.Empty;
            }

            if (IsBooleanLikeColumn(column.Id))
            {
                var values = new[] { "— wybierz —", "tak", "nie" };
                var index = oldValue.Equals("tak", StringComparison.OrdinalIgnoreCase) || oldValue.Equals("true", StringComparison.OrdinalIgnoreCase) ? 1
                    : oldValue.Equals("nie", StringComparison.OrdinalIgnoreCase) || oldValue.Equals("false", StringComparison.OrdinalIgnoreCase) ? 2
                    : 0;
                var selected = EditorGUILayout.Popup(label, index, values);
                return selected == 1 ? "tak" : selected == 2 ? "nie" : string.Empty;
            }

            return EditorGUILayout.TextField(label, oldValue);
        }

        private static string DrawNumericChoice(GUIContent label, string oldValue, int minimum, int maximum)
        {
            var values = new string[maximum - minimum + 2];
            values[0] = "— wybierz —";
            for (var value = minimum; value <= maximum; value++)
                values[value - minimum + 1] = value.ToString();

            var selected = 0;
            if (int.TryParse(oldValue, out var current) && current >= minimum && current <= maximum)
                selected = current - minimum + 1;

            var next = EditorGUILayout.Popup(label, selected, values);
            return next == 0 ? string.Empty : (minimum + next - 1).ToString();
        }

        private static bool IsBooleanLikeColumn(string columnId)
        {
            return columnId == "blokujące" ||
                   columnId == "sukces" ||
                   columnId == "gazebo_unity_zgodne" ||
                   columnId == "model_zasłaniany" ||
                   columnId == "wymiary_ok" ||
                   columnId == "cechy_zachowane" ||
                   columnId == "hierarchia_ok" ||
                   columnId == "gc_zaobserwowane";
        }

        private void DrawActions()
        {
            EditorGUILayout.Space(14);
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Sprawdzenie i wysłanie", EditorStyles.boldLabel);

                var liveEvaluation = WiRRReportEvaluator.Evaluate(document);
                if (liveEvaluation.CanSubmit)
                {
                    EditorGUILayout.HelpBox(
                        "Etap 3.0 jest kompletny. Raport może zostać wysłany. Etapy 3.5–5.0 nie są wymagane.",
                        MessageType.Info);
                }
                else
                {
                    EditorGUILayout.HelpBox(
                        "Raport nie jest jeszcze gotowy do wysłania. Uzupełnij dane zespołu, wymagane pola etapu 3.0 i dane pomiarowe tego etapu.",
                        MessageType.Warning);
                }

                if (GUILayout.Button("Sprawdź kompletność raportu", GUILayout.Height(32)))
                    evaluation = liveEvaluation;

                if (evaluation != null)
                    DrawEvaluation();

                using (new EditorGUI.DisabledScope(EditorApplication.isCompiling || !liveEvaluation.CanSubmit))
                {
                    if (GUILayout.Button("Wyślij raport", GUILayout.Height(38)))
                    {
                        evaluation = WiRRReportEvaluator.Evaluate(document);
                        var result = WiRRGitSubmission.Submit(
                            document,
                            WiRRGitSubmission.DefaultRepositoryUrl,
                            WiRRGitSubmission.DefaultRepositorySlug,
                            WiRRGitSubmission.DefaultBaseBranch,
                            WiRRGitSubmission.DefaultReportsPath);
                        submissionStatus = result.Message;
                    }
                }

                if (!liveEvaluation.CanSubmit)
                    EditorGUILayout.LabelField(
                        "Przycisk wysyłania uaktywni się po ukończeniu etapu 3.0.",
                        EditorStyles.wordWrappedMiniLabel);

                if (!string.IsNullOrWhiteSpace(submissionStatus))
                    EditorGUILayout.HelpBox(submissionStatus, MessageType.Info);
            }
        }

        private void DrawEvaluation()
        {
            foreach (var issue in evaluation.BlockingIssues)
                EditorGUILayout.HelpBox(issue, MessageType.Error);

            foreach (var cp in evaluation.Checkpoints)
            {
                var requiredForSubmission = cp.Checkpoint == "3.0";
                var status = cp.Complete
                    ? "kompletny"
                    : requiredForSubmission
                        ? "niekompletny — wymagany do wysłania"
                        : "niekompletny — etap opcjonalny";

                var text = $"Etap {cp.Checkpoint}: {status}";
                if (cp.Reasons.Count > 0)
                    text += " — " + string.Join("; ", cp.Reasons);

                var type = cp.Complete
                    ? MessageType.Info
                    : requiredForSubmission
                        ? MessageType.Error
                        : MessageType.None;

                EditorGUILayout.HelpBox(text, type);
            }

            var completed = string.IsNullOrWhiteSpace(evaluation.SuggestedGrade)
                ? "brak kompletnego etapu 3.0"
                : $"najwyższy kompletny etap: {evaluation.SuggestedGrade}";

            EditorGUILayout.LabelField("Wynik kontroli: " + completed, EditorStyles.boldLabel);
        }

        private void InitializeSectionFoldouts()
        {
            foreach (var section in WiRRReportSchemaCatalog.Get(labNumber))
            {
                var key = $"{labNumber}:{section.Checkpoint}";
                sectionFoldouts[key] = section.Checkpoint == "3.0";
            }
        }
    }
}

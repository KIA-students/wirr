using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace KIA.WiRR.Editor
{
    public sealed class WiRRCourseWindow : EditorWindow
    {
        private const string LabPrefKey = "KIA.WiRR.SelectedLab";
        private static WiRRCourseWindow openWindow;

        private int selectedLab;
        private Vector2 scroll;
        private List<WiRRValidationResult> validationResults;

        [MenuItem("WiRR/Course Toolkit", priority = 1)]
        public static void Open()
        {
            var window = GetWindow<WiRRCourseWindow>();
            window.titleContent = new GUIContent("WiRR Toolkit");
            window.minSize = new Vector2(500, 650);
            window.Show();
        }

        [MenuItem("WiRR/Validate selected lab", priority = 20)]
        private static void ValidateFromMenu() => WiRRSceneValidator.Validate(EditorPrefs.GetInt(LabPrefKey, 1));

        [MenuItem("WiRR/Create or repair base scene", priority = 21)]
        private static void PrepareFromMenu() => WiRRSceneTools.PrepareBaseScene(EditorPrefs.GetInt(LabPrefKey, 1));

        [MenuItem("WiRR/Open structured report form", priority = 22)]
        private static void ReportFromMenu() => WiRRReportWindow.Open();

        [MenuItem("WiRR/Open legacy Markdown report template", priority = 23)]
        private static void LegacyReportFromMenu() => WiRRReportTools.CreateOrOpen(EditorPrefs.GetInt(LabPrefKey, 1));

        internal static void RepaintOpenWindow() => openWindow?.Repaint();

        private void OnEnable()
        {
            openWindow = this;
            selectedLab = Mathf.Clamp(EditorPrefs.GetInt(LabPrefKey, 1), 1, 7);
        }

        private void OnDisable()
        {
            if (openWindow == this) openWindow = null;
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("WiRR Course Toolkit", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Wybierz laboratorium. Panel instaluje zależności, importuje próbki, przygotowuje scenę, uruchamia pomiary, łączy WebSim, prowadzi raport i wykonuje walidację.",
                MessageType.Info);

            var labels = WiRRLabCatalog.GetPopupLabels();
            var newIndex = EditorGUILayout.Popup("Laboratorium", selectedLab - 1, labels);
            if (newIndex != selectedLab - 1)
            {
                selectedLab = newIndex + 1;
                EditorPrefs.SetInt(LabPrefKey, selectedLab);
                validationResults = null;
            }

            var lab = WiRRLabCatalog.Get(selectedLab);
            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField($"Lab {lab.Number:00}: {lab.Title}", EditorStyles.boldLabel);

            DrawDependencySection(lab);
            DrawSampleSection(lab);
            DrawSceneSection(lab);
            if (lab.Number == 4 || lab.Number == 6)
                DrawWebSimSection(lab);
            DrawReportSection(lab);
            DrawValidationSection(lab);
        }

        private void DrawDependencySection(WiRRLabDefinition lab)
        {
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("1. Zależności", EditorStyles.boldLabel);
            using (new EditorGUI.DisabledScope(WiRRPackageInstaller.IsBusy))
                if (GUILayout.Button("Install / repair lab dependencies", GUILayout.Height(30)))
                    WiRRPackageInstaller.InstallForLab(lab.Number);
            if (WiRRPackageInstaller.IsBusy && GUILayout.Button("Cancel queued package installation"))
                WiRRPackageInstaller.Cancel();
            EditorGUILayout.HelpBox(WiRRPackageInstaller.Status, MessageType.None);
        }

        private void DrawSampleSection(WiRRLabDefinition lab)
        {
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("2. Próbki", EditorStyles.boldLabel);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Import official Unity samples")) WiRRSampleTools.ImportOfficialSamples(lab.Number);
                if (GUILayout.Button("Import WiRR lab sample")) WiRRSampleTools.ImportCourseSample(lab.Number);
            }
            EditorGUILayout.HelpBox("Próbki WiRR są rozdzielone per laboratorium; oficjalne Samples są importowane tylko tam, gdzie są potrzebne.", MessageType.None);
        }

        private void DrawSceneSection(WiRRLabDefinition lab)
        {
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("3. Scena i pomiary", EditorStyles.boldLabel);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Create / repair base scene", GUILayout.Height(28))) WiRRSceneTools.PrepareBaseScene(lab.Number);
                if (GUILayout.Button("Add / remove metrics probe", GUILayout.Height(28))) WiRRSceneTools.ToggleMetrics(lab.Number);
            }
            if (lab.Number == 6)
                EditorGUILayout.HelpBox("Lab 06 zachowuje trzy tryby: lokalny ROS 2/Gazebo, ROS 2/Gazebo na drugim komputerze oraz WiRR WebSim przez WSS.", MessageType.Info);
        }

        private void DrawWebSimSection(WiRRLabDefinition lab)
        {
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("4. WiRR WebSim", EditorStyles.boldLabel);

            var backend = EditorGUILayout.TextField("Backend WSS", WiRRWebSimTools.Backend);
            if (backend != WiRRWebSimTools.Backend) WiRRWebSimTools.Backend = backend.Trim();
            var session = EditorGUILayout.TextField("Kod sesji", WiRRWebSimTools.Session);
            if (session != WiRRWebSimTools.Session) WiRRWebSimTools.Session = session;
            var robots = new[] { "RRBot 2R", "WiRR Arm 3R" };
            var current = WiRRWebSimTools.Robot == "wirr-arm3" ? 1 : 0;
            var next = EditorGUILayout.Popup("Robot", current, robots);
            WiRRWebSimTools.Robot = next == 1 ? "wirr-arm3" : "rrbot";

            if (GUILayout.Button(lab.Number == 4 ? "Create / repair digital shadow" : "Create / repair WebSim digital twin", GUILayout.Height(30)))
                WiRRWebSimTools.CreateOrRepairRig(lab.Number);

            using (new EditorGUI.DisabledScope(!Application.isPlaying))
            {
                if (GUILayout.Button("Connect WebSim (Play Mode)")) WiRRWebSimTools.ConnectInPlayMode();
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Motion A")) WiRRWebSimTools.SendMotion("A");
                    if (GUILayout.Button("Motion B")) WiRRWebSimTools.SendMotion("B");
                    if (GUILayout.Button("Motion C")) WiRRWebSimTools.SendMotion("C");
                }
            }

            EditorGUILayout.HelpBox(
                lab.Number == 4
                    ? "Cyfrowy cień: ruch obiektu w Unity pochodzi z zewnętrznego /joint_states. Umieść go na wykrytej płaszczyźnie i testuj okluzję / MR."
                    : "WebSim: Gazebo oblicza ruch robota, ros_gz_bridge publikuje JointState, rosbridge przekazuje go przez WSS do Unity. Ten tryb nie wymaga lokalnego Ubuntu.",
                MessageType.None);
        }

        private void DrawReportSection(WiRRLabDefinition lab)
        {
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField((lab.Number == 4 || lab.Number == 6) ? "5. Raport" : "4. Raport", EditorStyles.boldLabel);
            if (GUILayout.Button("Open WiRR Reports form", GUILayout.Height(32))) WiRRReportWindow.Open();
            EditorGUILayout.HelpBox("Formularz zapisuje wyniki, metadane projektu i oś czasu pracy do strukturalnego JSON. Markdown pozostaje wyłącznie formatem awaryjnym.", MessageType.None);
            if (GUILayout.Button("Open legacy Markdown template", GUILayout.Height(22))) WiRRReportTools.CreateOrOpen(lab.Number);
        }

        private void DrawValidationSection(WiRRLabDefinition lab)
        {
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField((lab.Number == 4 || lab.Number == 6) ? "6. Walidacja" : "5. Walidacja", EditorStyles.boldLabel);
            if (GUILayout.Button("Validate scene", GUILayout.Height(32))) validationResults = WiRRSceneValidator.Validate(lab.Number);
            if (validationResults == null) return;

            var errors = 0;
            var warnings = 0;
            foreach (var result in validationResults)
            {
                if (result.Severity == WiRRValidationSeverity.Error) errors++;
                else if (result.Severity == WiRRValidationSeverity.Warning) warnings++;
            }
            var type = errors > 0 ? MessageType.Error : warnings > 0 ? MessageType.Warning : MessageType.Info;
            EditorGUILayout.HelpBox($"Wynik: {errors} błędów, {warnings} ostrzeżeń, {validationResults.Count - errors - warnings} informacji/PASS.", type);
            scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.MinHeight(120));
            foreach (var result in validationResults)
            {
                var prefix = result.Severity == WiRRValidationSeverity.Error ? "ERROR" : result.Severity == WiRRValidationSeverity.Warning ? "WARN" : "INFO";
                EditorGUILayout.LabelField($"[{prefix}] {result.Message}", EditorStyles.wordWrappedLabel);
                EditorGUILayout.Space(2);
            }
            EditorGUILayout.EndScrollView();
        }
    }
}

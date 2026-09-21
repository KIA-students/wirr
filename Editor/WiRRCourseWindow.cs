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
        private Vector2 windowScroll;
        private Vector2 validationScroll;
        private List<WiRRValidationResult> validationResults;

        [MenuItem("WiRR/Course Toolkit", priority = 1)]
        public static void Open()
        {
            var window = GetWindow<WiRRCourseWindow>();
            window.titleContent = WiRRBranding.Title("WiRR Toolkit");
            window.minSize = new Vector2(520, 650);
            window.Show();
        }

        [MenuItem("WiRR/Lab scene/Create or repair base scene", priority = 10)]
        private static void PrepareFromMenu() =>
            WiRRSceneTools.PrepareBaseScene(EditorPrefs.GetInt(LabPrefKey, 1));

        [MenuItem("WiRR/Lab scene/Validate selected lab", priority = 20)]
        private static void ValidateFromMenu() =>
            WiRRSceneValidator.Validate(EditorPrefs.GetInt(LabPrefKey, 1));

        [MenuItem("WiRR/Reports/Open legacy Markdown report template", priority = 20)]
        private static void LegacyReportFromMenu() =>
            WiRRReportTools.CreateOrOpen(EditorPrefs.GetInt(LabPrefKey, 1));

        internal static void RepaintOpenWindow() => openWindow?.Repaint();

        private void OnEnable()
        {
            openWindow = this;
            selectedLab = Mathf.Clamp(EditorPrefs.GetInt(LabPrefKey, 1), 1, 7);
            titleContent = WiRRBranding.Title("WiRR Toolkit");
        }

        private void OnDisable()
        {
            if (openWindow == this)
                openWindow = null;
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("WiRR Course Toolkit", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Wybierz laboratorium i wykonuj kolejne kroki od góry. Import próbki WiRR automatycznie tworzy uporządkowany workspace Assets/WiRR/LabXX wraz ze sceną bazową.",
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
            windowScroll = EditorGUILayout.BeginScrollView(windowScroll);

            DrawLabStatus(lab);
            DrawDependencySection(lab);
            DrawWorkspaceSection(lab);
            DrawSceneSection(lab);
            if (lab.Number == 6)
                DrawWebSimSection(lab);
            DrawReportSection(lab);
            DrawValidationSection(lab);

            EditorGUILayout.Space(10);
            EditorGUILayout.EndScrollView();

            if (Application.isPlaying && lab.Number == 6)
                Repaint();
        }

        private void DrawLabStatus(WiRRLabDefinition lab)
        {
            EditorGUILayout.Space(8);
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField($"Lab {lab.Number:00}: {lab.Title}", EditorStyles.boldLabel);

                var sampleReady = WiRRSampleTools.IsCourseSampleImported(lab.Number);
                var workspaceReady = WiRRSceneTools.WorkspaceExists(lab.Number);
                var sceneReady = WiRRSceneTools.SceneExists(lab.Number);

                EditorGUILayout.LabelField(
                    $"Próbka WiRR: {(sampleReady ? "OK" : "brak")}    " +
                    $"Workspace: {(workspaceReady ? "OK" : "brak")}    " +
                    $"Scena: {(sceneReady ? "OK" : "brak")}",
                    EditorStyles.wordWrappedLabel);
                EditorGUILayout.LabelField(WiRRSceneTools.GetLabRootPath(lab.Number), EditorStyles.miniLabel);
            }
        }

        private void DrawDependencySection(WiRRLabDefinition lab)
        {
            EditorGUILayout.Space(8);
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("1. Zależności", EditorStyles.boldLabel);
                using (new EditorGUI.DisabledScope(WiRRPackageInstaller.IsBusy))
                {
                    if (GUILayout.Button("Zainstaluj / napraw zależności laboratorium", GUILayout.Height(30)))
                        WiRRPackageInstaller.InstallForLab(lab.Number);
                }

                if (WiRRPackageInstaller.IsBusy && GUILayout.Button("Anuluj kolejkę instalacji"))
                    WiRRPackageInstaller.Cancel();

                EditorGUILayout.HelpBox(WiRRPackageInstaller.Status, MessageType.None);
            }
        }

        private void DrawWorkspaceSection(WiRRLabDefinition lab)
        {
            EditorGUILayout.Space(8);
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("2. Próbki i workspace", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox(
                    "Po imporcie próbki tworzony jest Assets/WiRR/LabXX z folderami Scenes, Scripts, Materials, Models, Prefabs, Textures, Data, Evidence i Documentation oraz sceną LabXX.unity.",
                    MessageType.None);

                if (GUILayout.Button("Importuj próbkę WiRR + przygotuj workspace", GUILayout.Height(32)))
                    WiRRSampleTools.ImportCourseSample(lab.Number);

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Importuj oficjalne Samples Unity"))
                        WiRRSampleTools.ImportOfficialSamples(lab.Number);
                    if (GUILayout.Button("Napraw strukturę workspace"))
                        WiRRSceneTools.PrepareLabWorkspace(lab.Number);
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button($"Otwórz folder Lab{lab.Number:00}"))
                        WiRRSceneTools.OpenLabFolder(lab.Number);
                    if (GUILayout.Button($"Otwórz scenę Lab{lab.Number:00}"))
                        WiRRSceneTools.OpenLabScene(lab.Number);
                }
            }
        }

        private void DrawSceneSection(WiRRLabDefinition lab)
        {
            EditorGUILayout.Space(8);
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("3. Scena i pomiary", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox(
                    "Naprawa sceny zapewnia pojedynczy WiRRSceneMarker, Main Camera, Directional Light i WiRR_Ground z BoxCollider.",
                    MessageType.None);

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Utwórz / napraw aktywną scenę", GUILayout.Height(28)))
                        WiRRSceneTools.PrepareBaseScene(lab.Number);
                    if (GUILayout.Button("Dodaj / usuń sondę metryk", GUILayout.Height(28)))
                        WiRRSceneTools.ToggleMetrics(lab.Number);
                }

                if (lab.Number == 6)
                    EditorGUILayout.HelpBox(
                        "Lab 06 obsługuje lokalny ROS 2/Gazebo, ROS 2/Gazebo na drugim komputerze oraz WiRR WebSim przez WSS.",
                        MessageType.Info);
            }
        }

        private void DrawWebSimSection(WiRRLabDefinition lab)
        {
            EditorGUILayout.Space(8);
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("4. WiRR WebSim", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox(
                    "Podaj backend, kod sesji i model. Następnie utwórz rig, uruchom Play Mode i sprawdź status LIVE.",
                    MessageType.Info);

                var backend = EditorGUILayout.TextField("Backend WebSocket", WiRRWebSimTools.Backend);
                if (backend != WiRRWebSimTools.Backend)
                    WiRRWebSimTools.Backend = backend.Trim();

                var session = EditorGUILayout.TextField("Kod sesji / zespołu", WiRRWebSimTools.Session);
                if (session != WiRRWebSimTools.Session)
                    WiRRWebSimTools.Session = session;

                var robots = new[] { "RRBot 2R", "WiRR Arm 3R" };
                var current = WiRRWebSimTools.Robot == "wirr-arm3" ? 1 : 0;
                var next = EditorGUILayout.Popup("Robot", current, robots);
                WiRRWebSimTools.Robot = next == 1 ? "wirr-arm3" : "rrbot";

                var valid = WiRRWebSimTools.ValidateConfiguration(out var configurationMessage);
                EditorGUILayout.HelpBox(configurationMessage, valid ? MessageType.None : MessageType.Warning);

                using (new EditorGUI.DisabledScope(!valid || Application.isPlaying))
                {
                    if (GUILayout.Button("Utwórz / napraw bliźniaka WebSim", GUILayout.Height(30)))
                        WiRRWebSimTools.CreateOrRepairRig(lab.Number);
                }

                EditorGUILayout.Space(4);
                EditorGUILayout.LabelField("Połączenie", EditorStyles.miniBoldLabel);
                EditorGUILayout.HelpBox(WiRRWebSimTools.RuntimeStatus(), MessageType.None);

                using (new EditorGUI.DisabledScope(!Application.isPlaying || !valid))
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button("Connect")) WiRRWebSimTools.ConnectInPlayMode();
                        if (GUILayout.Button("Disconnect")) WiRRWebSimTools.DisconnectInPlayMode();
                    }
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button("Home")) WiRRWebSimTools.SendHome();
                        if (GUILayout.Button("Reset")) WiRRWebSimTools.SendReset();
                    }
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button("Motion A")) WiRRWebSimTools.SendMotion("A");
                        if (GUILayout.Button("Motion B")) WiRRWebSimTools.SendMotion("B");
                        if (GUILayout.Button("Motion C")) WiRRWebSimTools.SendMotion("C");
                    }
                }
            }
        }

        private void DrawReportSection(WiRRLabDefinition lab)
        {
            EditorGUILayout.Space(8);
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField(lab.Number == 6 ? "5. Raport" : "4. Raport", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox(
                    "Do wysłania raportu wystarczy kompletny checkpoint 3.0. Etapy 3.5–5.0 są opcjonalne i mogą pozostać puste.",
                    MessageType.Info);

                if (GUILayout.Button("Otwórz formularz raportu WiRR", GUILayout.Height(32)))
                    WiRRReportWindow.Open();

                if (GUILayout.Button("Otwórz awaryjny szablon Markdown", GUILayout.Height(22)))
                    WiRRReportTools.CreateOrOpen(lab.Number);
            }
        }

        private void DrawValidationSection(WiRRLabDefinition lab)
        {
            EditorGUILayout.Space(8);
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField(lab.Number == 6 ? "6. Walidacja" : "5. Walidacja", EditorStyles.boldLabel);

                if (GUILayout.Button("Waliduj wybrane laboratorium", GUILayout.Height(32)))
                    validationResults = WiRRSceneValidator.Validate(lab.Number);

                if (validationResults == null)
                    return;

                var errors = 0;
                var warnings = 0;
                foreach (var result in validationResults)
                {
                    if (result.Severity == WiRRValidationSeverity.Error) errors++;
                    else if (result.Severity == WiRRValidationSeverity.Warning) warnings++;
                }

                var type = errors > 0
                    ? MessageType.Error
                    : warnings > 0
                        ? MessageType.Warning
                        : MessageType.Info;

                EditorGUILayout.HelpBox(
                    $"Wynik: {errors} błędów, {warnings} ostrzeżeń, {validationResults.Count - errors - warnings} informacji/PASS.",
                    type);

                validationScroll = EditorGUILayout.BeginScrollView(validationScroll, GUILayout.MinHeight(120), GUILayout.MaxHeight(260));
                foreach (var result in validationResults)
                {
                    var prefix = result.Severity == WiRRValidationSeverity.Error
                        ? "ERROR"
                        : result.Severity == WiRRValidationSeverity.Warning
                            ? "WARN"
                            : "INFO";
                    EditorGUILayout.LabelField($"[{prefix}] {result.Message}", EditorStyles.wordWrappedLabel);
                    EditorGUILayout.Space(2);
                }
                EditorGUILayout.EndScrollView();
            }
        }
    }
}

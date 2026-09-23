using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace KIA.WiRR.Editor
{
    public sealed class WiRRCourseWindow : EditorWindow
    {
        private const string LabPrefKey = "KIA.WiRR.SelectedLab";
        private const string CourseUrl = "https://kia-students.github.io/wirr/";
        private const string TaskPrefPrefix = "KIA.WiRR.Task";
        private static WiRRCourseWindow openWindow;

        private static readonly Color HeaderAccent = new Color(0.12f, 0.63f, 0.86f);
        private static readonly Color DependencyAccent = new Color(0.25f, 0.52f, 0.94f);
        private static readonly Color WorkspaceAccent = new Color(0.10f, 0.70f, 0.72f);
        private static readonly Color SceneAccent = new Color(0.56f, 0.42f, 0.92f);
        private static readonly Color ExerciseAccent = new Color(0.91f, 0.45f, 0.18f);
        private static readonly Color WebSimAccent = new Color(0.95f, 0.56f, 0.18f);
        private static readonly Color ReportAccent = new Color(0.77f, 0.35f, 0.72f);
        private static readonly Color ValidationAccent = new Color(0.24f, 0.68f, 0.42f);
        private static readonly Color SuccessAccent = new Color(0.24f, 0.68f, 0.42f);
        private static readonly Color WarningAccent = new Color(0.95f, 0.62f, 0.18f);
        private static readonly Color ErrorAccent = new Color(0.90f, 0.30f, 0.28f);

        private int selectedLab;
        private Vector2 windowScroll;
        private Vector2 validationScroll;
        private List<WiRRValidationResult> validationResults;

        private GUIStyle heroTitleStyle;
        private GUIStyle heroSubtitleStyle;
        private GUIStyle sectionTitleStyle;
        private GUIStyle sectionSubtitleStyle;
        private GUIStyle stepBadgeStyle;
        private GUIStyle statusBadgeStyle;
        private GUIStyle nextStepStyle;
        private GUIStyle taskTitleStyle;
        private GUIStyle taskStepStyle;

        [MenuItem("WiRR/Narzędzia kursu", priority = 1)]
        public static void Open()
        {
            var window = GetWindow<WiRRCourseWindow>();
            window.titleContent = WiRRBranding.Title("WiRR Course Toolkit");
            window.minSize = new Vector2(560, 680);
            window.Show();
        }

        [MenuItem("WiRR/Scena laboratorium/Utwórz lub napraw scenę", priority = 10)]
        private static void PrepareFromMenu() =>
            WiRRSceneTools.PrepareBaseScene(EditorPrefs.GetInt(LabPrefKey, 1));

        [MenuItem("WiRR/Scena laboratorium/Wygeneruj środowisko dydaktyczne", priority = 11)]
        private static void TeachingEnvironmentFromMenu() =>
            WiRRTeachingAssetTools.CreateOrRepairEnvironment(EditorPrefs.GetInt(LabPrefKey, 1));

        [MenuItem("WiRR/Scena laboratorium/Wygeneruj zestaw eksperymentalny", priority = 12)]
        private static void TeachingExperimentFromMenu() =>
            WiRRTeachingAssetTools.CreateOrRepairExperimentSet(EditorPrefs.GetInt(LabPrefKey, 1));

        [MenuItem("WiRR/Scena laboratorium/Sprawdź wybrane laboratorium", priority = 20)]
        private static void ValidateFromMenu() =>
            WiRRSceneValidator.Validate(EditorPrefs.GetInt(LabPrefKey, 1));

        [MenuItem("WiRR/Raporty/Awaryjny szablon Markdown", priority = 20)]
        private static void LegacyReportFromMenu() =>
            WiRRReportTools.CreateOrOpen(EditorPrefs.GetInt(LabPrefKey, 1));

        internal static void RepaintOpenWindow() => openWindow?.Repaint();

        private void OnEnable()
        {
            openWindow = this;
            selectedLab = Mathf.Clamp(EditorPrefs.GetInt(LabPrefKey, 1), 1, 7);
            titleContent = WiRRBranding.Title("WiRR Course Toolkit");
        }

        private void OnDisable()
        {
            if (openWindow == this)
                openWindow = null;
        }

        private void OnGUI()
        {
            EnsureStyles();
            DrawHero();
            DrawLabSelector();

            var lab = WiRRLabCatalog.Get(selectedLab);
            windowScroll = EditorGUILayout.BeginScrollView(windowScroll);

            DrawPreparationOverview(lab);
            DrawDependencySection(lab);
            DrawWorkspaceSection(lab);
            DrawSceneSection(lab);
            if (lab.Number == 6)
                DrawWebSimSection(lab);
            DrawValidationSection(lab);
            DrawExerciseSection(lab);
            DrawReportSection(lab);

            EditorGUILayout.Space(12);
            EditorGUILayout.EndScrollView();

            if (Application.isPlaying && lab.Number == 6)
                Repaint();
        }

        private void EnsureStyles()
        {
            if (heroTitleStyle != null)
                return;

            heroTitleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 19,
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = ForegroundColor() }
            };

            heroSubtitleStyle = new GUIStyle(EditorStyles.wordWrappedMiniLabel)
            {
                fontSize = 11,
                alignment = TextAnchor.UpperLeft,
                normal = { textColor = MutedForegroundColor() }
            };

            sectionTitleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 13,
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = ForegroundColor() }
            };

            sectionSubtitleStyle = new GUIStyle(EditorStyles.wordWrappedMiniLabel)
            {
                alignment = TextAnchor.UpperLeft,
                normal = { textColor = MutedForegroundColor() }
            };

            stepBadgeStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 15,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white }
            };

            statusBadgeStyle = new GUIStyle(EditorStyles.miniBoldLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = ForegroundColor() }
            };

            nextStepStyle = new GUIStyle(EditorStyles.wordWrappedLabel)
            {
                fontStyle = FontStyle.Bold,
                padding = new RectOffset(8, 8, 5, 5),
                normal = { textColor = ForegroundColor() }
            };

            taskTitleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 12,
                wordWrap = true,
                normal = { textColor = ForegroundColor() }
            };

            taskStepStyle = new GUIStyle(EditorStyles.wordWrappedLabel)
            {
                wordWrap = true,
                padding = new RectOffset(4, 4, 2, 2),
                normal = { textColor = ForegroundColor() }
            };
        }

        private void DrawHero()
        {
            EditorGUILayout.Space(8);
            var rect = GUILayoutUtility.GetRect(0f, 78f, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(rect, PanelColor(HeaderAccent, 0.34f));
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, 6f, rect.height), HeaderAccent);

            var iconRect = new Rect(rect.x + 16f, rect.y + 13f, 52f, 52f);
            if (WiRRBranding.Icon != null)
                GUI.DrawTexture(iconRect, WiRRBranding.Icon, ScaleMode.ScaleToFit, true);

            var textLeft = WiRRBranding.Icon != null ? rect.x + 80f : rect.x + 18f;
            var buttonWidth = 112f;
            var titleRect = new Rect(textLeft, rect.y + 12f, Mathf.Max(120f, rect.width - (textLeft - rect.x) - buttonWidth - 22f), 26f);
            var subtitleRect = new Rect(textLeft, rect.y + 40f, Mathf.Max(120f, rect.width - (textLeft - rect.x) - buttonWidth - 22f), 30f);
            GUI.Label(titleRect, "WiRR Course Toolkit", heroTitleStyle);
            GUI.Label(subtitleRect, "Przygotuj środowisko, wykonaj zadania, pomiary i raport krok po kroku.", heroSubtitleStyle);

            var buttonRect = new Rect(rect.xMax - buttonWidth - 12f, rect.y + 23f, buttonWidth, 30f);
            var oldBackground = GUI.backgroundColor;
            GUI.backgroundColor = HeaderAccent;
            if (GUI.Button(buttonRect, "Strona kursu"))
                Application.OpenURL(CourseUrl);
            GUI.backgroundColor = oldBackground;
        }

        private void DrawLabSelector()
        {
            EditorGUILayout.Space(8);
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Wybierz laboratorium", EditorStyles.boldLabel);
                EditorGUILayout.LabelField(
                    "Panel poniżej dopasuje kolejne kroki do wybranego laboratorium.",
                    EditorStyles.wordWrappedMiniLabel);

                var labels = WiRRLabCatalog.GetPopupLabels();
                var newIndex = EditorGUILayout.Popup("Laboratorium", selectedLab - 1, labels);
                if (newIndex == selectedLab - 1)
                    return;

                selectedLab = newIndex + 1;
                EditorPrefs.SetInt(LabPrefKey, selectedLab);
                validationResults = null;
                GUI.FocusControl(null);
            }
        }

        private void DrawPreparationOverview(WiRRLabDefinition lab)
        {
            var sampleReady = WiRRSampleTools.IsCourseSampleImported(lab.Number);
            var workspaceReady = WiRRSceneTools.WorkspaceExists(lab.Number);
            var sceneReady = WiRRSceneTools.SceneExists(lab.Number);
            var preparedCount = (sampleReady ? 1 : 0) + (workspaceReady ? 1 : 0) + (sceneReady ? 1 : 0);

            EditorGUILayout.Space(8);
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField($"Laboratorium {lab.Number:00}: {lab.Title}", sectionTitleStyle);
                EditorGUILayout.Space(3);

                var progressRect = GUILayoutUtility.GetRect(0f, 20f, GUILayout.ExpandWidth(true));
                EditorGUI.ProgressBar(progressRect, preparedCount / 3f, $"Przygotowanie środowiska: {preparedCount}/3");
                EditorGUILayout.Space(5);

                using (new EditorGUILayout.HorizontalScope())
                {
                    DrawStatusBadge("Próbka", sampleReady);
                    DrawStatusBadge("Folder roboczy", workspaceReady);
                    DrawStatusBadge("Scena", sceneReady);
                }

                EditorGUILayout.Space(5);
                DrawNextStep(sampleReady, workspaceReady, sceneReady);
                EditorGUILayout.LabelField(WiRRSceneTools.GetLabRootPath(lab.Number), EditorStyles.miniLabel);
            }
        }

        private void DrawNextStep(bool sampleReady, bool workspaceReady, bool sceneReady)
        {
            string message;
            Color accent;

            if (!sampleReady)
            {
                message = "NASTĘPNY KROK · Zainstaluj zależności, a następnie zaimportuj próbkę WiRR.";
                accent = DependencyAccent;
            }
            else if (!workspaceReady)
            {
                message = "NASTĘPNY KROK · Napraw strukturę folderu roboczego laboratorium.";
                accent = WorkspaceAccent;
            }
            else if (!sceneReady)
            {
                message = "NASTĘPNY KROK · Utwórz lub napraw scenę bazową laboratorium.";
                accent = SceneAccent;
            }
            else if (validationResults == null)
            {
                message = "NASTĘPNY KROK · Sprawdź konfigurację laboratorium przed rozpoczęciem pomiarów.";
                accent = ValidationAccent;
            }
            else
            {
                var errors = CountValidation(WiRRValidationSeverity.Error);
                message = errors > 0
                    ? $"DO POPRAWY · Walidator wykrył {errors} błędów. Usuń je przed wykonaniem pomiarów."
                    : "GOTOWE · Konfiguracja nie zawiera błędów blokujących. Możesz wykonywać ćwiczenie i pomiary.";
                accent = errors > 0 ? ErrorAccent : SuccessAccent;
            }

            var rect = GUILayoutUtility.GetRect(0f, 34f, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(rect, PanelColor(accent, 0.27f));
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, 4f, rect.height), accent);
            GUI.Label(new Rect(rect.x + 8f, rect.y + 2f, rect.width - 12f, rect.height - 4f), message, nextStepStyle);
        }

        private void DrawDependencySection(WiRRLabDefinition lab)
        {
            EditorGUILayout.Space(8);
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                DrawSectionHeader(
                    1,
                    "Zależności",
                    "Najpierw przygotuj pakiety Unity wymagane przez ćwiczenie.",
                    DependencyAccent);

                EditorGUILayout.HelpBox(
                    "Narzędzie instaluje brakujące zależności i pomija pakiety już obecne w projekcie. Po instalacji poczekaj na zakończenie kompilacji Unity.",
                    MessageType.None);

                using (new EditorGUI.DisabledScope(WiRRPackageInstaller.IsBusy))
                {
                    if (PrimaryButton("Zainstaluj / napraw zależności", DependencyAccent, 34f))
                        WiRRPackageInstaller.InstallForLab(lab.Number);
                }

                if (WiRRPackageInstaller.IsBusy && SecondaryColoredButton("Anuluj kolejkę instalacji", WarningAccent))
                    WiRRPackageInstaller.Cancel();

                EditorGUILayout.HelpBox(WiRRPackageInstaller.Status, MessageType.None);
            }
        }

        private void DrawWorkspaceSection(WiRRLabDefinition lab)
        {
            EditorGUILayout.Space(8);
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                DrawSectionHeader(
                    2,
                    "Próbki i folder roboczy",
                    "Zaimportuj materiały startowe. WiRR uporządkuje pliki laboratorium automatycznie.",
                    WorkspaceAccent);

                EditorGUILayout.HelpBox(
                    "Po imporcie powstaje Assets/WiRR/LabXX z katalogami Scenes, Scripts, Materials, Models, Prefabs, Textures, Data, Evidence i Documentation oraz sceną LabXX.unity.",
                    MessageType.None);

                if (PrimaryButton("Importuj próbkę WiRR i przygotuj folder", WorkspaceAccent, 34f))
                    WiRRSampleTools.ImportCourseSample(lab.Number);

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Importuj oficjalne próbki Unity", GUILayout.Height(27)))
                        WiRRSampleTools.ImportOfficialSamples(lab.Number);
                    if (GUILayout.Button("Napraw strukturę folderu", GUILayout.Height(27)))
                        WiRRSceneTools.PrepareLabWorkspace(lab.Number);
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button($"Otwórz folder Lab{lab.Number:00}", GUILayout.Height(25)))
                        WiRRSceneTools.OpenLabFolder(lab.Number);
                    if (GUILayout.Button($"Otwórz scenę Lab{lab.Number:00}", GUILayout.Height(25)))
                        WiRRSceneTools.OpenLabScene(lab.Number);
                }
            }
        }

        private void DrawSceneSection(WiRRLabDefinition lab)
        {
            EditorGUILayout.Space(8);
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                DrawSectionHeader(
                    3,
                    "Scena i pomiary",
                    "Przygotuj minimalną scenę, a następnie włącz narzędzia pomiarowe, gdy są potrzebne.",
                    SceneAccent);

                EditorGUILayout.HelpBox(
                    "Naprawa sceny zapewnia jeden WiRRSceneMarker, kamerę główną, światło kierunkowe oraz WiRR_Ground z BoxCollider. Nie usuwa Twoich obiektów laboratoryjnych.",
                    MessageType.None);

                if (PrimaryButton("Utwórz / napraw aktywną scenę", SceneAccent, 32f))
                    WiRRSceneTools.PrepareBaseScene(lab.Number);

                if (GUILayout.Button("Dodaj / usuń sondę metryk", GUILayout.Height(27)))
                    WiRRSceneTools.ToggleMetrics(lab.Number);

                EditorGUILayout.Space(7);
                EditorGUILayout.LabelField("Materiały dydaktyczne", EditorStyles.miniBoldLabel);
                EditorGUILayout.HelpBox(
                    "Opcjonalny zestaw rozpoznawalnych obiektów zastępuje pustą scenę i ręczne modelowanie prymitywów. Prefaby są generowane w Assets/WiRR/LabXX/Prefabs/Generated. Nie dodają za studenta komponentów XRI, AR ani mapowania ROS stanowiących cel ćwiczenia.",
                    MessageType.Info);

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Dodaj środowisko", GUILayout.Height(28)))
                        WiRRTeachingAssetTools.CreateOrRepairEnvironment(lab.Number);
                    if (GUILayout.Button("Dodaj zestaw eksperymentalny", GUILayout.Height(28)))
                        WiRRTeachingAssetTools.CreateOrRepairExperimentSet(lab.Number);
                }

                if (GUILayout.Button("Usuń obiekty dydaktyczne ze sceny", GUILayout.Height(24)))
                    WiRRTeachingAssetTools.RemoveTeachingObjectsFromScene(lab.Number);

                if (WiRRTeachingAssetTools.GeneratedPrefabsExist(lab.Number))
                    DrawInlineStatus("Prefaby dydaktyczne są wygenerowane. Modyfikuj własne rozwiązania poza folderem Prefabs/Generated.", SuccessAccent);

                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Prefaby teksturowane Grid", EditorStyles.miniBoldLabel);
                EditorGUILayout.HelpBox(
                    "Drugi zestaw pomocy scenicznych korzysta z rodzin grid-1, grid_2 i grid-4 (w narzędziu: Grid1, Grid2 i Grid3). Tekstury są kopiowane do Assets/WiRR/Common/Textures/Generated i używane do generowania edytowalnych materiałów URP.",
                    MessageType.None);

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Dodaj prefaby Grid", GUILayout.Height(28)))
                        WiRRPrefabTools.CreateOrRepairGridSet(lab.Number);
                    if (GUILayout.Button("Odśwież materiały Grid", GUILayout.Height(28)))
                        WiRRPrefabTools.RefreshGridMaterials();
                }

                if (GUILayout.Button("Usuń prefaby Grid ze sceny", GUILayout.Height(24)))
                    WiRRPrefabTools.RemoveGridObjectsFromScene(lab.Number);

                if (WiRRPrefabTools.GridPrefabsExist(lab.Number))
                    DrawInlineStatus("Teksturowane prefaby Grid są wygenerowane w Prefabs/GridGenerated.", SuccessAccent);

                EditorGUILayout.Space(7);
                EditorGUILayout.LabelField("Opcjonalne laboratorium materiałów", EditorStyles.miniBoldLabel);
                EditorGUILayout.HelpBox(
                    "Dodatkowe demonstratory do Lab 01 i Lab 05, ale dostępne w każdej scenie. Pokazują wpływ rozdzielczości tekstury, normal mapy, kanałów PBR, tilingu i mipmap oraz różnice między wieloma materiałami z biblioteki WiRR.",
                    MessageType.Info);

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Galeria materiałów", GUILayout.Height(27)))
                        WiRROptionalPrefabTools.CreateMaterialGallery(lab.Number);
                    if (GUILayout.Button("Rozdzielczość 128–1024", GUILayout.Height(27)))
                        WiRROptionalPrefabTools.CreateResolutionWall(lab.Number);
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Normal map: porównanie", GUILayout.Height(27)))
                        WiRROptionalPrefabTools.CreateNormalMapLab(lab.Number);
                    if (GUILayout.Button("Kanały PBR", GUILayout.Height(27)))
                        WiRROptionalPrefabTools.CreatePbrChannelGallery(lab.Number);
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Tiling i mipmapy", GUILayout.Height(27)))
                        WiRROptionalPrefabTools.CreateTilingLab(lab.Number);
                    if (GUILayout.Button("Dodaj wszystkie demonstratory", GUILayout.Height(27)))
                        WiRROptionalPrefabTools.CreateAll(lab.Number);
                }

                if (GUILayout.Button("Usuń opcjonalne demonstratory ze sceny", GUILayout.Height(24)))
                    WiRROptionalPrefabTools.RemoveOptionalDemos(lab.Number);

                if (WiRROptionalPrefabTools.OptionalDemosExist(lab.Number))
                    DrawInlineStatus("Opcjonalne demonstratory materiałów są aktywne w tej scenie.", SuccessAccent);

                EditorGUILayout.Space(7);
                EditorGUILayout.LabelField("Ruch, fizyka i dźwięk", EditorStyles.miniBoldLabel);
                EditorGUILayout.HelpBox(
                    "Opcjonalne prefaby działające w Play Mode. Pokazują ruch zapętlony, kinematyczne Rigidbody, triggery i impulsy, dźwięk proceduralny oraz sterowanie pojazdem. Platformy zawierają tylko TeleportAreaPlaceholder: komponent XRI student dodaje samodzielnie.",
                    MessageType.Info);

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Unosząca platforma", GUILayout.Height(27)))
                        WiRRDynamicPrefabTools.CreateFloatingTeleportPlatform(lab.Number);
                    if (GUILayout.Button("Platforma wahadłowa", GUILayout.Height(27)))
                        WiRRDynamicPrefabTools.CreateShuttlePlatform(lab.Number);
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Sterowalny pojazd", GUILayout.Height(27)))
                        WiRRDynamicPrefabTools.CreateDriveableCart(lab.Number);
                    if (GUILayout.Button("Wyrzutnia fizyczna", GUILayout.Height(27)))
                        WiRRDynamicPrefabTools.CreatePhysicsLauncher(lab.Number);
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Kinetyczny beacon audio", GUILayout.Height(27)))
                        WiRRDynamicPrefabTools.CreateKineticAudioBeacon(lab.Number);
                    if (GUILayout.Button("Dodaj wszystkie dynamiczne", GUILayout.Height(27)))
                        WiRRDynamicPrefabTools.CreateAll(lab.Number);
                }

                if (GUILayout.Button("Usuń dynamiczne demonstratory ze sceny", GUILayout.Height(24)))
                    WiRRDynamicPrefabTools.RemoveDynamicDemos(lab.Number);

                if (WiRRDynamicPrefabTools.DynamicDemosExist(lab.Number))
                    DrawInlineStatus("Dynamiczne demonstratory są aktywne. Uruchom Play Mode, aby obserwować ruch, fizykę i dźwięk.", SuccessAccent);

                if (lab.Number == 6)
                    EditorGUILayout.HelpBox(
                        "Laboratorium 06 może korzystać z lokalnego ROS 2/Gazebo, ROS 2/Gazebo na drugim komputerze albo z WiRR WebSim.",
                        MessageType.Info);
            }
        }

        private void DrawWebSimSection(WiRRLabDefinition lab)
        {
            EditorGUILayout.Space(8);
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                DrawSectionHeader(
                    4,
                    "WiRR WebSim",
                    "Opcjonalne źródło stanu robota w Laboratorium 06.",
                    WebSimAccent);

                EditorGUILayout.HelpBox(
                    "Podaj adres serwera WebSocket (np. ws://127.0.0.1:9090), kod sesji i model robota. Utwórz model, uruchom tryb Play i sprawdź, czy stan połączenia zmieni się na LIVE.",
                    MessageType.Info);

                var backend = EditorGUILayout.TextField(
                    new GUIContent("Adres serwera WebSocket", "Adres backendu WebSim, np. ws://127.0.0.1:9090."),
                    WiRRWebSimTools.Backend);
                if (backend != WiRRWebSimTools.Backend)
                    WiRRWebSimTools.Backend = backend.Trim();

                var session = EditorGUILayout.TextField(
                    new GUIContent("Kod sesji / zespołu", "Krótki kod rozdzielający sesje różnych zespołów."),
                    WiRRWebSimTools.Session);
                if (session != WiRRWebSimTools.Session)
                    WiRRWebSimTools.Session = session;

                var robots = new[] { "RRBot 2R", "WiRR Arm 3R" };
                var current = WiRRWebSimTools.Robot == "wirr-arm3" ? 1 : 0;
                var next = EditorGUILayout.Popup("Robot", current, robots);
                WiRRWebSimTools.Robot = next == 1 ? "wirr-arm3" : "rrbot";

                var valid = WiRRWebSimTools.ValidateConfiguration(out var configurationMessage);
                DrawInlineStatus(configurationMessage, valid ? SuccessAccent : WarningAccent);

                using (new EditorGUI.DisabledScope(!valid || Application.isPlaying))
                {
                    if (PrimaryButton("Utwórz / napraw model WebSim", WebSimAccent, 32f))
                        WiRRWebSimTools.CreateOrRepairRig(lab.Number);
                }

                EditorGUILayout.Space(4);
                EditorGUILayout.LabelField("Sterowanie połączeniem", EditorStyles.miniBoldLabel);
                DrawInlineStatus(
                    WiRRWebSimTools.RuntimeStatus(),
                    WiRRWebSimTools.RuntimeStatus().StartsWith("LIVE") ? SuccessAccent : WarningAccent);

                using (new EditorGUI.DisabledScope(!Application.isPlaying || !valid))
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button("Połącz")) WiRRWebSimTools.ConnectInPlayMode();
                        if (GUILayout.Button("Rozłącz")) WiRRWebSimTools.DisconnectInPlayMode();
                    }

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button("Pozycja początkowa")) WiRRWebSimTools.SendHome();
                        if (GUILayout.Button("Resetuj")) WiRRWebSimTools.SendReset();
                    }

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button("Ruch A")) WiRRWebSimTools.SendMotion("A");
                        if (GUILayout.Button("Ruch B")) WiRRWebSimTools.SendMotion("B");
                        if (GUILayout.Button("Ruch C")) WiRRWebSimTools.SendMotion("C");
                    }
                }
            }
        }

        private void DrawExerciseSection(WiRRLabDefinition lab)
        {
            var step = lab.Number == 6 ? 6 : 5;
            var tasks = WiRRLabTaskCatalog.Get(lab.Number);
            var total = 0;
            var completed = 0;

            for (var taskIndex = 0; taskIndex < tasks.Count; taskIndex++)
            {
                var task = tasks[taskIndex];
                for (var stepIndex = 0; stepIndex < task.Steps.Count; stepIndex++)
                {
                    total++;
                    if (EditorPrefs.GetBool(TaskKey(lab.Number, taskIndex, stepIndex), false))
                        completed++;
                }
            }

            EditorGUILayout.Space(8);
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                DrawSectionHeader(
                    step,
                    "Zadania laboratoryjne",
                    "Wykonuj kolejne punkty kontrolne od 3.0 do 5.0. Każdy krok kończy się mierzalnym dowodem do raportu.",
                    ExerciseAccent);

                var progress = total == 0 ? 0f : completed / (float)total;
                var progressRect = GUILayoutUtility.GetRect(0f, 20f, GUILayout.ExpandWidth(true));
                EditorGUI.ProgressBar(progressRect, progress, $"Postęp checklisty: {completed}/{total}");
                EditorGUILayout.Space(4);

                EditorGUILayout.HelpBox(
                    "Checkboxy są lokalną pomocą organizacyjną. Nie stanowią automatycznego zaliczenia ani nie są wysyłane w raporcie.",
                    MessageType.None);

                for (var taskIndex = 0; taskIndex < tasks.Count; taskIndex++)
                    DrawLabTask(lab.Number, taskIndex, tasks[taskIndex]);

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Wyczyść oznaczenia checklisty", GUILayout.Height(24)))
                        ClearTaskProgress(lab.Number, tasks);

                    if (GUILayout.Button("Otwórz formularz raportu", GUILayout.Height(24)))
                        WiRRReportWindow.Open();
                }
            }
        }

        private void DrawLabTask(int labNumber, int taskIndex, WiRRLabTask task)
        {
            EditorGUILayout.Space(6);
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField($"{task.Checkpoint}: {task.Title}", taskTitleStyle);
                EditorGUILayout.LabelField(task.Goal, EditorStyles.wordWrappedMiniLabel);
                EditorGUILayout.Space(3);

                for (var stepIndex = 0; stepIndex < task.Steps.Count; stepIndex++)
                {
                    var key = TaskKey(labNumber, taskIndex, stepIndex);
                    var current = EditorPrefs.GetBool(key, false);
                    var next = EditorGUILayout.ToggleLeft(
                        $"{stepIndex + 1}. {task.Steps[stepIndex]}",
                        current,
                        taskStepStyle);

                    if (next != current)
                        EditorPrefs.SetBool(key, next);
                }

                EditorGUILayout.Space(3);
                EditorGUILayout.HelpBox(task.Evidence, MessageType.Info);
            }
        }

        private static string TaskKey(int labNumber, int taskIndex, int stepIndex) =>
            $"{TaskPrefPrefix}.{labNumber:00}.{taskIndex}.{stepIndex}";

        private static void ClearTaskProgress(int labNumber, IReadOnlyList<WiRRLabTask> tasks)
        {
            for (var taskIndex = 0; taskIndex < tasks.Count; taskIndex++)
                for (var stepIndex = 0; stepIndex < tasks[taskIndex].Steps.Count; stepIndex++)
                    EditorPrefs.DeleteKey(TaskKey(labNumber, taskIndex, stepIndex));
        }

        private void DrawReportSection(WiRRLabDefinition lab)
        {
            var step = lab.Number == 6 ? 7 : 6;

            EditorGUILayout.Space(8);
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                DrawSectionHeader(
                    step,
                    "Raport",
                    "Zapisz wyniki i wnioski. Do wysłania wystarczy kompletny etap 3.0.",
                    ReportAccent);

                EditorGUILayout.HelpBox(
                    "Etapy od 3.5 do 5.0 są opcjonalne. Formularz automatycznie oblicza wartości pochodne, jeśli wynikają jednoznacznie z danych pomiarowych.",
                    MessageType.Info);

                if (PrimaryButton("Otwórz formularz raportu WiRR", ReportAccent, 34f))
                    WiRRReportWindow.Open();

                if (GUILayout.Button("Otwórz awaryjny szablon Markdown", GUILayout.Height(24)))
                    WiRRReportTools.CreateOrOpen(lab.Number);
            }
        }

        private void DrawValidationSection(WiRRLabDefinition lab)
        {
            var step = lab.Number == 6 ? 5 : 4;

            EditorGUILayout.Space(8);
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                DrawSectionHeader(
                    step,
                    "Sprawdzenie konfiguracji",
                    "Przed pomiarami sprawdź, czy środowisko jest gotowe do wykonania ćwiczenia.",
                    ValidationAccent);

                EditorGUILayout.HelpBox(
                    "Walidator sprawdza wersję Unity, wymagane pakiety, aktywną scenę, kamerę główną, WiRRSceneMarker i typowe błędy konfiguracji.",
                    MessageType.None);

                if (PrimaryButton("Sprawdź konfigurację laboratorium", ValidationAccent, 34f))
                    validationResults = WiRRSceneValidator.Validate(lab.Number);

                if (validationResults == null)
                {
                    DrawInlineStatus("Walidacja nie została jeszcze uruchomiona.", WarningAccent);
                    return;
                }

                var errors = CountValidation(WiRRValidationSeverity.Error);
                var warnings = CountValidation(WiRRValidationSeverity.Warning);
                var information = validationResults.Count - errors - warnings;

                var summaryAccent = errors > 0 ? ErrorAccent : warnings > 0 ? WarningAccent : SuccessAccent;
                var summary = errors > 0
                    ? $"Wymaga poprawy · błędy: {errors}, ostrzeżenia: {warnings}, OK/info: {information}"
                    : warnings > 0
                        ? $"Można kontynuować po sprawdzeniu ostrzeżeń · ostrzeżenia: {warnings}, OK/info: {information}"
                        : $"Konfiguracja gotowa · OK/info: {information}";

                DrawInlineStatus(summary, summaryAccent);

                validationScroll = EditorGUILayout.BeginScrollView(
                    validationScroll,
                    GUILayout.MinHeight(120),
                    GUILayout.MaxHeight(280));

                foreach (var result in validationResults)
                    DrawValidationResult(result);

                EditorGUILayout.EndScrollView();
            }
        }

        private void DrawSectionHeader(int step, string title, string subtitle, Color accent)
        {
            var rect = GUILayoutUtility.GetRect(0f, 58f, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(rect, PanelColor(accent, 0.24f));
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, 5f, rect.height), accent);

            var badgeRect = new Rect(rect.x + 13f, rect.y + 12f, 34f, 34f);
            EditorGUI.DrawRect(badgeRect, accent);
            GUI.Label(badgeRect, step.ToString(), stepBadgeStyle);

            GUI.Label(
                new Rect(rect.x + 58f, rect.y + 8f, rect.width - 68f, 22f),
                title,
                sectionTitleStyle);

            GUI.Label(
                new Rect(rect.x + 58f, rect.y + 30f, rect.width - 68f, 23f),
                subtitle,
                sectionSubtitleStyle);
        }

        private void DrawStatusBadge(string label, bool ready)
        {
            var accent = ready ? SuccessAccent : WarningAccent;
            var text = ready ? $"GOTOWE · {label}" : $"BRAK · {label}";
            var rect = GUILayoutUtility.GetRect(0f, 24f, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(rect, PanelColor(accent, 0.32f));
            GUI.Label(rect, text, statusBadgeStyle);
        }

        private void DrawInlineStatus(string text, Color accent)
        {
            var height = Mathf.Max(28f, EditorStyles.wordWrappedLabel.CalcHeight(new GUIContent(text), position.width - 42f) + 10f);
            var rect = GUILayoutUtility.GetRect(0f, height, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(rect, PanelColor(accent, 0.25f));
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, 4f, rect.height), accent);
            GUI.Label(
                new Rect(rect.x + 9f, rect.y + 4f, rect.width - 14f, rect.height - 8f),
                text,
                EditorStyles.wordWrappedLabel);
        }

        private void DrawValidationResult(WiRRValidationResult result)
        {
            Color accent;
            string prefix;

            if (result.Severity == WiRRValidationSeverity.Error)
            {
                accent = ErrorAccent;
                prefix = "BŁĄD";
            }
            else if (result.Severity == WiRRValidationSeverity.Warning)
            {
                accent = WarningAccent;
                prefix = "OSTRZEŻENIE";
            }
            else
            {
                accent = SuccessAccent;
                prefix = "OK";
            }

            var text = $"{prefix} · {result.Message}";
            var height = Mathf.Max(31f, EditorStyles.wordWrappedLabel.CalcHeight(new GUIContent(text), position.width - 70f) + 10f);
            var rect = GUILayoutUtility.GetRect(0f, height, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(rect, PanelColor(accent, 0.18f));
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, 4f, rect.height), accent);
            GUI.Label(
                new Rect(rect.x + 10f, rect.y + 4f, rect.width - 14f, rect.height - 8f),
                text,
                EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space(3);
        }

        private bool PrimaryButton(string label, Color accent, float height)
        {
            var oldBackground = GUI.backgroundColor;
            var oldContent = GUI.contentColor;
            GUI.backgroundColor = accent;
            GUI.contentColor = Color.white;
            var pressed = GUILayout.Button(label, GUILayout.Height(height));
            GUI.backgroundColor = oldBackground;
            GUI.contentColor = oldContent;
            return pressed;
        }

        private bool SecondaryColoredButton(string label, Color accent)
        {
            var oldBackground = GUI.backgroundColor;
            GUI.backgroundColor = Color.Lerp(Color.white, accent, 0.65f);
            var pressed = GUILayout.Button(label, GUILayout.Height(25f));
            GUI.backgroundColor = oldBackground;
            return pressed;
        }

        private int CountValidation(WiRRValidationSeverity severity)
        {
            if (validationResults == null)
                return 0;

            var count = 0;
            foreach (var result in validationResults)
                if (result.Severity == severity)
                    count++;
            return count;
        }

        private static Color PanelColor(Color accent, float strength)
        {
            var baseColor = EditorGUIUtility.isProSkin
                ? new Color(0.16f, 0.16f, 0.18f)
                : new Color(0.94f, 0.94f, 0.95f);
            return Color.Lerp(baseColor, accent, strength);
        }

        private static Color ForegroundColor()
        {
            return EditorGUIUtility.isProSkin
                ? new Color(0.94f, 0.94f, 0.96f)
                : new Color(0.12f, 0.12f, 0.14f);
        }

        private static Color MutedForegroundColor()
        {
            return EditorGUIUtility.isProSkin
                ? new Color(0.76f, 0.78f, 0.82f)
                : new Color(0.30f, 0.31f, 0.34f);
        }
    }
}

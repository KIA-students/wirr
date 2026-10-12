using System;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Text;
using KIA.WiRR;
using UnityEditor;
using UnityEngine;

namespace KIA.WiRR.Editor
{
    /// <summary>
    /// Local, offline-first mission board. It never submits reports or triggers CI.
    /// A saved scene and an exported report are evidence, not an automatic grade.
    /// </summary>
    public sealed class WiRRMissionBoardWindow : EditorWindow
    {
        private static readonly int[][] Blocks =
        {
            new[] { 1, 2 },
            new[] { 3, 4 },
            new[] { 5, 6 },
            new[] { 7, 8 }
        };

        private static readonly string[] Missions =
        {
            "I. Świat reaguje",
            "II. Świat się miesza",
            "III. Bliźniak rzeczywistości",
            "IV. Wynalazca XR"
        };

        private static readonly string[] Modes =
        {
            "NIE WYBRANO",
            "UNITY_EDITOR",
            "XR_SIM",
            "QUEST_STANDALONE",
            "QUEST_LINK",
            "PHONE_AR",
            "VITURE_SBS",
            "ROS_LIVE",
            "ROS_REPLAY",
            "UNSUPPORTED"
        };

        private static readonly string[] GroupLabels = { "Nie wybrano", "G1: grupa 1", "G2: grupa 2" };

        private bool showAdvanced;
        private int blockIndex;
        private int selectedGroup;
        private int selectedMode;
        private Vector2 scroll;
        private string discovery = "";
        private string application = "";
        private string diagnosis = "";

        [MenuItem("WiRR/Misje i odkrycia (4 bloki)", priority = 12)]
        public static void Open()
        {
            var window = GetWindow<WiRRMissionBoardWindow>();
            window.titleContent = new GUIContent("Misje WiRR");
            window.minSize = new Vector2(580, 460);
            window.Show();
        }

        private void OnEnable()
        {
            blockIndex = Mathf.Clamp(EditorPrefs.GetInt("KIA.WiRR.MissionBlock", 0), 0, Blocks.Length - 1);
            selectedGroup = Mathf.Clamp(EditorPrefs.GetInt("KIA.WiRR.MissionGroup", 0), 0, 2);
            showAdvanced = EditorPrefs.GetBool("KIA.WiRR.MissionAdvanced", false);
        }

        private void OnGUI()
        {
            scroll = EditorGUILayout.BeginScrollView(scroll);
            EditorGUILayout.LabelField("WiRR: cztery wyprawy odkrywcze", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Cel: działający eksperyment, diagnoza i nowy pomysł zastosowania. " +
                "Nie oceniaj czasu pracy ani liczby kliknięć. Dane zapisujemy lokalnie, bez GitHub Actions.",
                MessageType.Info);

            var chosen = GUILayout.Toolbar(blockIndex, Missions);
            if (chosen != blockIndex)
            {
                blockIndex = chosen;
                EditorPrefs.SetInt("KIA.WiRR.MissionBlock", blockIndex);
            }

            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Wystartuj bez instalowania dodatkowych pakietów", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Otwórz minimalną scenę na PC. Możesz od razu eksperymentować na " +
                "monitorze i w symulatorze; telefon AR i Quest skonfigurujesz dopiero, " +
                "gdy wybierzesz eksperyment sprzętowy. Scena nie jest nadpisywana.",
                MessageType.Info);
            using (new EditorGUI.DisabledScope(EditorApplication.isCompiling || EditorApplication.isPlaying))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Start: Lab " + Blocks[blockIndex][0].ToString("00")))
                        WiRRSceneTools.OpenLabScene(Blocks[blockIndex][0]);
                    if (GUILayout.Button("Start: Lab " + Blocks[blockIndex][1].ToString("00")))
                        WiRRSceneTools.OpenLabScene(Blocks[blockIndex][1]);
                }
            }
            // Comfort stations belong to the testing/comfort block only.
            if (blockIndex == 3 && GUILayout.Button("Lab 07: stanowiska komfortu XR i watchdog"))
                WiRRComfortStudyTools.CreateStations();
            showAdvanced = EditorGUILayout.Foldout(showAdvanced, "Narzędzia dodatkowe", true);
            EditorPrefs.SetBool("KIA.WiRR.MissionAdvanced", showAdvanced);
            if (showAdvanced)
            {
                if (GUILayout.Button("Zależności i urządzenia"))
                    WiRRCourseWindow.Open();
                if (GUILayout.Button("Ranking Ligi XR (strona kursu)"))
                    Application.OpenURL("https://kia-students.github.io/wirr/#liga");
                if (GUILayout.Button("Edytuj grupę i nazwę drużyny"))
                    WiRRTeamIdentityWindow.Open();
            }

            EditorGUILayout.LabelField("Punkty kontrolne", EditorStyles.boldLabel);
            var preparedScenes = Blocks[blockIndex].Count(lab => ScenePaths(lab).Length > 0);
            var progressRect = GUILayoutUtility.GetRect(18, 22, GUILayout.ExpandWidth(true));
            EditorGUI.ProgressBar(progressRect, preparedScenes / (float)Blocks[blockIndex].Length,
                $"Przygotowane sceny: {preparedScenes}/{Blocks[blockIndex].Length} (nie ocena)");

            foreach (var lab in Blocks[blockIndex])
            {
                var hasScene = ScenePaths(lab).Length > 0;
                var hasExport = FindLatestExport(lab) != null;
                var status = hasScene ? "Scena zapisana" : "Brak zapisanej sceny";
                status += hasExport ? " | JSON wyeksportowany" : " | JSON jeszcze nie wyeksportowany";

                EditorGUILayout.LabelField($"Lab {lab:00}: {status}", EditorStyles.wordWrappedLabel);
            }

            EditorGUILayout.Space(8);
            if (blockIndex == 3 && GUILayout.Button("Lab 08: dodaj gotowy demonstrator inspekcji"))
                WiRRLab08Tools.AddDemonstrator();
            if (blockIndex == 3 && GUILayout.Button("Lab 08: importuj prawdziwe wyniki JSON"))
                WiRRLab08Tools.ImportResults();
            if (GUILayout.Button("Otwórz formularz WiRR Reports (Lab 01–08)"))
                WiRRReportWindow.Open();

            EditorGUILayout.HelpBox(
                "Scena i JSON są wykrywane automatycznie, ale NIE dowodzą pomiaru z urządzenia. " +
                "Wskaż rzeczywisty tryb i uzupełnij tylko trzy krótkie obserwacje.",
                MessageType.None);

            // Identity is edited in a dedicated panel, never duplicated per block.
            selectedGroup = Mathf.Clamp(EditorPrefs.GetInt("KIA.WiRR.MissionGroup", 0), 0, 2);
            selectedMode = EditorGUILayout.Popup("Tryb faktyczny", selectedMode, Modes);
            EditorGUILayout.LabelField("Jedno odkrycie z eksperymentu");
            discovery = EditorGUILayout.TextArea(discovery, GUILayout.MinHeight(46));
            EditorGUILayout.LabelField("Jedna naprawiona przeszkoda");
            diagnosis = EditorGUILayout.TextArea(diagnosis, GUILayout.MinHeight(46));
            EditorGUILayout.LabelField("Jedno nowe zastosowanie XR");
            application = EditorGUILayout.TextArea(application, GUILayout.MinHeight(46));

            using (new EditorGUI.DisabledScope(EditorApplication.isCompiling))
            {
                if (GUILayout.Button("Zapisz lokalną kartę sesji (Markdown)"))
                    SaveSessionCard();
            }

            EditorGUILayout.HelpBox(
                "Raport pozostaje na komputerze studenta. Nie zbieramy numeru indeksu, " +
                "nazwy konta systemowego, zdjęć, lokalizacji ani treści kodu. " +
                "Wartości z tabel są przepisywane TYLKO z istniejących eksportów JSON.",
                MessageType.None);
            EditorGUILayout.EndScrollView();
        }

        private static string[] ScenePaths(int lab)
        {
            var folder = $"Assets/WiRR/Lab{lab:00}";
            if (!AssetDatabase.IsValidFolder(folder))
                return Array.Empty<string>();
            return AssetDatabase.FindAssets("t:Scene", new[] { folder })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => !string.IsNullOrEmpty(path))
                .OrderBy(path => path, StringComparer.Ordinal)
                .ToArray();
        }

        private static string FindLatestExport(int lab)
        {
            var folder = Path.GetFullPath(Path.Combine(Application.dataPath, "../Library/WiRRReports/exports"));
            if (!Directory.Exists(folder))
                return null;
            var matches = Directory.GetFiles(folder, $"*-lab{lab:00}-*.json");
            return matches.OrderByDescending(File.GetLastWriteTimeUtc).FirstOrDefault();
        }

        private static bool ValidTeamName(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            var trimmed = value.Trim();
            return trimmed.Length >= 3 && trimmed.Length <= 48 &&
                trimmed.All(ch => char.IsLetterOrDigit(ch) || ch == ' ' || ch == '-');
        }

        private static string Escape(string value, int maximum = 240)
        {
            var safe = (value ?? string.Empty).Replace("|", "\\|")
                .Replace("\r", " ").Replace("\n", " ")
                .Replace("<", "&lt;").Replace(">", "&gt;").Trim();
            return safe.Length > maximum ? safe.Substring(0, maximum) + "…" : safe;
        }

        private void SaveSessionCard()
        {
            var target = EditorUtility.SaveFilePanel(
                "Zapisz kartę sesji WiRR", "", $"WiRR-blok-{blockIndex + 1:00}.md", "md");
            if (string.IsNullOrWhiteSpace(target))
                return;

            var writer = new StringBuilder();
            writer.AppendLine($"# WiRR – {Missions[blockIndex]}");
            writer.AppendLine();
            writer.AppendLine($"- Data UTC: {DateTime.UtcNow:O}");
            writer.AppendLine($"- Grupa laboratoryjna: {GroupLabels[selectedGroup]}");
            var savedTeamName = EditorPrefs.GetString("KIA.WiRR.TeamName", "");
            if (ValidTeamName(savedTeamName))
                writer.AppendLine("- Lokalna nazwa drużyny (bez zatwierdzenia): " + Escape(savedTeamName, 48));
            writer.AppendLine($"- Unity: {Application.unityVersion}");
            writer.AppendLine($"- System: {Escape(SystemInfo.operatingSystem)}");
            writer.AppendLine($"- GPU: {Escape(SystemInfo.graphicsDeviceName)}");
            writer.AppendLine($"- Tryb zadeklarowany: {Modes[selectedMode]} (student potwierdza)");
            writer.AppendLine();

            foreach (var lab in Blocks[blockIndex])
            {
                writer.AppendLine($"## Lab {lab:00}");
                var scenes = ScenePaths(lab);
                writer.AppendLine($"- Sceny w projekcie: {(scenes.Length == 0 ? "BRAK" : string.Join(", ", scenes.Select(path => Escape(path, 120))))}");
                var path = FindLatestExport(lab);
                if (path == null)
                {
                    writer.AppendLine("- Eksport WiRR JSON: BRAK. Nie zakładaj, że pomiary wykonano.");
                }
                else
                {
                    try
                    {
                        var document = JsonUtility.FromJson<WiRRReportDocument>(File.ReadAllText(path));
                        if (document == null || document.labNumber != lab)
                        {
                            writer.AppendLine("- Eksport JSON: niezgodny numer laboratorium.");
                        }
                        else
                        {
                            writer.AppendLine("- Eksport JSON: istnieje (lokalny, bez wysyłki)");
                            writer.AppendLine($"- Zdarzenia Unity: {document.activitySummary?.eventCount ?? 0}");
                            writer.AppendLine($"- Sesje Play Mode: {document.activitySummary?.playModeSessions ?? 0}");
                            var playSeconds = document.activitySummary?.playModeSeconds ?? 0.0;
                            writer.AppendLine("- Czas Play Mode [s]: " + playSeconds.ToString("F1", CultureInfo.InvariantCulture) + " (informacyjny, nie ocena)");
                            var evidence = document.answers == null
                                ? Array.Empty<WiRRReportValue>()
                                : document.answers.Where(x => x != null && x.key != null
                                  && x.key.StartsWith("table.", StringComparison.Ordinal)
                                  && !string.IsNullOrWhiteSpace(x.value)).Take(12).ToArray();
                            if (evidence.Length > 0)
                            {
                                writer.AppendLine();
                                writer.AppendLine("| Pole z tabeli | Wartość wyeksportowana |");
                                writer.AppendLine("|---|---|");
                                foreach (var item in evidence)
                                    writer.AppendLine($"| {Escape(item.key, 90)} | {Escape(item.value)} |");
                            }
                        }
                    }
                    catch (Exception exception) when (exception is IOException || exception is ArgumentException)
                    {
                        writer.AppendLine("- Nie udało się odczytać JSON: " + Escape(exception.Message));
                    }
                }
                writer.AppendLine();
            }

            writer.AppendLine("## Wnioski studenta");
            writer.AppendLine($"- Odkrycie: {Escape(discovery, 500)}");
            writer.AppendLine($"- Diagnoza: {Escape(diagnosis, 500)}");
            writer.AppendLine($"- Nowe zastosowanie: {Escape(application, 500)}");
            writer.AppendLine();
            writer.AppendLine("Brak danych nie oznacza wartości zero; nie wykonano dodatkowego pomiaru przy eksporcie.");
            File.WriteAllText(target, writer.ToString(), new UTF8Encoding(false));
            EditorUtility.RevealInFinder(target);
        }
    }
}

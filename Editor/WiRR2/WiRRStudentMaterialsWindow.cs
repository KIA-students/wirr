using System;
using System.IO;
using System.Linq;
using KIA.WiRR;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace KIA.WiRR.Editor
{
    /// <summary>Preparation belongs to the instructor. Students open already prepared scenes.</summary>
    public sealed class WiRRStudentMaterialsWindow : EditorWindow
    {
        public static readonly string[] SceneNames = {
            "Lab01_Station", "Lab02_Interaction", "Lab03_AnchorGallery", "Lab04_DepthRoom",
            "Lab05_CADWorkshop", "Lab06_DigitalTwin", "Lab07_Expedition", "Lab08_ExperienceStudio" };
        public const string Root = "Assets/WiRR/WiRR2";
        private Vector2 scroll;

        [MenuItem("WiRR/Nowe instrukcje/Materiały studenta", priority = 13)]
        public static void Open() => GetWindow<WiRRStudentMaterialsWindow>("Doświadczenia WiRR").Show();

        public static string ScenePath(int lab) => Root + "/" + SceneNames[lab - 1] + ".unity";

        [MenuItem("WiRR/Nowe instrukcje/Przygotuj osiem scen (prowadzący)", priority = 14)]
        public static void PrepareAll()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling) return;
            var package = UnityEditor.PackageManager.PackageInfo.FindForPackageName("pl.prz.kia.wirr");
            if (package == null) throw new InvalidOperationException("Nie odnaleziono pakietu WiRR.");
            Directory.CreateDirectory(Root);
            for (var lab = 1; lab <= 8; lab++)
            {
                var source = Path.Combine(package.resolvedPath, "Samples~", "Lab" + lab.ToString("00"), "WiRR2", SceneNames[lab - 1] + ".unity");
                if (!File.Exists(source)) throw new FileNotFoundException("Brak sceny materiałów studenta", source);
                // Never overwrite a student's edited scene. Unity assigns a new scene GUID.
                if (!File.Exists(ScenePath(lab))) File.Copy(source, ScenePath(lab));
            }
            AssetDatabase.Refresh();
            Debug.Log("[WiRR] Osiem scen przygotowanych w " + Root + ". Materiały wymagają odbioru PlayMode przed zajęciami.");
            Open();
        }

        private void OnGUI()
        {
            scroll = EditorGUILayout.BeginScrollView(scroll);
            EditorGUILayout.LabelField("Nowe instrukcje | cztery bloki", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Materiały pilotażowe 0.1. Desktop, SIM_AR i ROS_REPLAY. Nie potwierdzają jeszcze pełnej zgodności 40 progów ocen. Przygotowanie stanowisk wykonuje prowadzący.", MessageType.Info);
            using (new EditorGUI.DisabledScope(EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling))
            {
                for (var block = 0; block < 4; block++)
                {
                    EditorGUILayout.LabelField("Blok " + (block + 1), EditorStyles.boldLabel);
                    for (var lab = block * 2 + 1; lab <= block * 2 + 2; lab++)
                    {
                        var exists = AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath(lab)) != null;
                        using (new EditorGUI.DisabledScope(!exists))
                            if (GUILayout.Button("Otwórz " + SceneNames[lab - 1]) && EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                                EditorSceneManager.OpenScene(ScenePath(lab));
                    }
                }
                if (GUILayout.Button("Przygotuj brakujące sceny (prowadzący)")) PrepareAll();
            }
            EditorGUILayout.HelpBox("Po otwarciu sceny uruchom Play. Wybierz profil solo/para, wykonaj doświadczenie i eksportuj dwa wyniki bloku. Zapis lokalny nie oznacza wysłania.", MessageType.None);
            if (GUILayout.Button("Otwórz lokalne wyniki"))
            {
                var folder = Path.Combine(Application.persistentDataPath, "WiRR");
                Directory.CreateDirectory(folder); EditorUtility.RevealInFinder(folder);
            }
            EditorGUILayout.EndScrollView();
        }
    }
}

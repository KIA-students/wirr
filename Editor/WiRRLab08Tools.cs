using System;
using System.IO;
using System.Linq;
using System.Globalization;
using KIA.WiRR;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KIA.WiRR.Editor
{
    /// <summary>
    /// One-click baseline demo and one-click import of genuinely recorded data.
    /// Students can use XR/AR/ROS/VITURE as optional extensions to the baseline.
    /// </summary>
    internal static class WiRRLab08Tools
    {
        private const string Folder = "Assets/WiRR/Lab08/Prefabs/Generated";
        private const string PrefabName = "WiRR_Lab08_Inspection";
        private const string PrefabPath = Folder + "/" + PrefabName + ".prefab";

        [MenuItem("WiRR/Lab 08/Dodaj demonstrator inspekcji", priority = 10)]
        public static void AddDemonstrator()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorUtility.DisplayDialog("WiRR Lab08", "Zakończ Play Mode przed edycją sceny.", "OK");
                return;
            }
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            WiRRSceneTools.EnsureFolders(8);
            EnsureFolder(Folder);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            if (prefab == null)
            {
                var root = BuildPrefab();
                try
                {
                    prefab = PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
                    if (prefab == null) throw new InvalidOperationException("Nie można zapisać prefab Lab08.");
                }
                finally
                {
                    UnityEngine.Object.DestroyImmediate(root);
                }
            }
            var scene = SceneManager.GetActiveScene();
            if (!scene.IsValid()) return;
            if (!scene.GetRootGameObjects().Any(go => go.name == PrefabName))
            {
                var instance = PrefabUtility.InstantiatePrefab(prefab, scene) as GameObject;
                if (instance != null) Undo.RegisterCreatedObjectUndo(instance, "WiRR Lab08 demonstrator");
            }
            EditorSceneManager.MarkSceneDirty(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("[WiRR Lab08] Demonstrator gotowy, uruchom Play i trzy próby wariantu A, potem B.");
        }

        private static GameObject BuildPrefab()
        {
            var root = new GameObject(PrefabName);
            var targets = new Transform[3];
            for (var i = 0; i < 3; i++)
            {
                var part = GameObject.CreatePrimitive(PrimitiveType.Cube);
                part.name = "Element_inspekcji_" + (i + 1);
                part.transform.SetParent(root.transform, false);
                part.transform.localPosition = new Vector3((i - 1) * 0.65f, 1.4f, 2.8f);
                part.transform.localScale = new Vector3(0.45f, 0.45f, 0.45f);
                targets[i] = part.transform;

                var textObject = new GameObject("Etykieta_" + (i + 1));
                textObject.transform.SetParent(part.transform, false);
                textObject.transform.localPosition = new Vector3(0f, 0.7f, 0f);
                var label = textObject.AddComponent<TextMesh>();
                label.text = "Kontrola " + (i + 1);
                label.fontSize = 42;
                label.characterSize = 0.06f;
                label.anchor = TextAnchor.MiddleCenter;
            }

            var study = root.AddComponent<WiRRLab08Experiment>();
            study.Configure(targets);
            return root;
        }

        [MenuItem("WiRR/Lab 08/Importuj wyniki demonstratora do raportu", priority = 11)]
        public static void ImportResults()
        {
            var suggested = Path.Combine(Application.persistentDataPath, "WiRR-Lab08-results.json");
            var selected = EditorUtility.OpenFilePanel("Wybierz rzeczywisty wynik Lab 08", Path.GetDirectoryName(suggested), "json");
            if (string.IsNullOrWhiteSpace(selected)) return;
            WiRRLab08Session results;
            try
            {
                results = JsonUtility.FromJson<WiRRLab08Session>(File.ReadAllText(selected));
            }
            catch (Exception error) when (error is IOException || error is ArgumentException)
            {
                EditorUtility.DisplayDialog("WiRR Lab08", "Nie można wczytać raportu: " + error.Message, "OK");
                return;
            }
            if (results == null || results.schema != "wirr-lab08/1" || results.trials == null ||
                results.trials.Count == 0 || results.trials.Count > 6 ||
                results.trials.Any(trial => trial == null || trial.seconds < 0f ||
                    (trial.variant != "A" && trial.variant != "B") || trial.attempt < 1 || trial.attempt > 3))
            {
                EditorUtility.DisplayDialog("WiRR Lab08", "Niepoprawny albo pusty JSON eksperymentu. Nie wpisano danych.", "OK");
                return;
            }

            var report = WiRRReportStore.LoadOrCreate(8);
            var baseline = results.trials.Where(trial => trial.variant == "A").OrderBy(trial => trial.attempt).ToArray();
            var improved = results.trials.Where(trial => trial.variant == "B").OrderBy(trial => trial.attempt).ToArray();
            WiRRReportStore.SetValue(report, "cp30.scenario", results.scenario);
            WiRRReportStore.SetValue(report, "cp30.attempts", results.trials.Count.ToString(CultureInfo.InvariantCulture));
            WiRRReportStore.SetValue(report, "cp30.successes", results.trials.Count(trial => trial.success).ToString(CultureInfo.InvariantCulture));
            var table = WiRRReportTableCatalog.Get(8, "3.0").First();
            // Do not replace empty rows with zeros: evidence must stay honest.
            for (var i = 0; i < Math.Min(3, baseline.Length); i++)
            {
                var trial = baseline[i];
                WiRRReportStore.SetValue(report, WiRRReportTableCatalog.CellKey(table, table.Rows[i], table.Columns[0]),
                    trial.seconds.ToString("F3", CultureInfo.InvariantCulture));
                WiRRReportStore.SetValue(report, WiRRReportTableCatalog.CellKey(table, table.Rows[i], table.Columns[1]),
                    trial.success ? "TAK" : "NIE");
                WiRRReportStore.SetValue(report, WiRRReportTableCatalog.CellKey(table, table.Rows[i], table.Columns[2]),
                    trial.deviceMode ?? "UNSPECIFIED");
            }

            SetMedian(report, "cp35.median_s", baseline);
            if (baseline.Length > 0) WiRRReportStore.SetValue(report, "cp35.source", results.deviceMode + " (zgłoszony przez program)");
            SetMedian(report, "cp45.before_s", baseline);
            SetMedian(report, "cp45.after_s", improved);
            var compare = WiRRReportTableCatalog.Get(8, "4.5").First();
            if (baseline.Length > 0)
            {
                FillComparison(report, compare, 0, baseline);
            }
            if (improved.Length > 0)
            {
                FillComparison(report, compare, 1, improved);
            }
            WiRRReportStore.Save(report);
            // The report window opens on the newly imported Lab 08.
            EditorPrefs.SetInt("KIA.WiRR.SelectedLab", 8);
            EditorUtility.DisplayDialog("WiRR Lab08",
                "Wczytano " + results.trials.Count + " rzeczywistych prób do szkicu raportu Lab 08. " +
                "Uzupełnij hipotezę, interpretację i nowy pomysł. Brakujące wartości pozostały puste.", "OK");
            WiRRReportWindow.Open();
        }

        private static void FillComparison(WiRRReportDocument report, WiRRReportTable table, int row, WiRRLab08Trial[] trials)
        {
            var successes = trials.Count(trial => trial.success);
            WiRRReportStore.SetValue(report, WiRRReportTableCatalog.CellKey(table, table.Rows[row], table.Columns[1]),
                successes.ToString(CultureInfo.InvariantCulture));
            var times = trials.Where(trial => trial.success).Select(trial => (double)trial.seconds).OrderBy(time => time).ToArray();
            if (times.Length > 0)
                WiRRReportStore.SetValue(report, WiRRReportTableCatalog.CellKey(table, table.Rows[row], table.Columns[0]),
                    Median(times).ToString("F3", CultureInfo.InvariantCulture));
        }

        private static void SetMedian(WiRRReportDocument report, string key, WiRRLab08Trial[] trials)
        {
            var times = trials.Where(trial => trial.success).Select(trial => (double)trial.seconds).OrderBy(time => time).ToArray();
            if (times.Length > 0)
                WiRRReportStore.SetValue(report, key, Median(times).ToString("F3", CultureInfo.InvariantCulture));
        }

        private static double Median(double[] ordered) =>
            ordered.Length % 2 == 1
                ? ordered[ordered.Length / 2]
                : (ordered[ordered.Length / 2 - 1] + ordered[ordered.Length / 2]) / 2.0;

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            var parent = Path.GetDirectoryName(path)?.Replace('\\', '/');
            if (!string.IsNullOrWhiteSpace(parent) && !AssetDatabase.IsValidFolder(parent))
                EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, Path.GetFileName(path));
        }
    }
}

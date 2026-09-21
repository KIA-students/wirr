using System.IO;
using UnityEditor;
using UnityEngine;

namespace KIA.WiRR.Editor
{
    internal static class WiRRReportTools
    {
        public static void CreateOrOpen(int labNumber)
        {
            var sampleNullable = WiRRSampleTools.FindCourseSample(labNumber);
            if (!sampleNullable.HasValue)
            {
                Debug.LogError($"[WiRR] Nie znaleziono próbki laboratorium {labNumber:00} w pakiecie.");
                return;
            }

            var sample = sampleNullable.Value;
            var source = Path.Combine(sample.resolvedPath, "report-template.md");
            if (!File.Exists(source))
            {
                Debug.LogError($"[WiRR] Brak szablonu raportu: {source}");
                return;
            }

            WiRRSceneTools.EnsureFolders(labNumber);
            var assetPath = $"Assets/WiRR/Reports/Lab{labNumber:00}-report.md";
            var fullPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", assetPath));

            if (!File.Exists(fullPath))
            {
                File.Copy(source, fullPath);
                AssetDatabase.Refresh();
                Debug.Log($"[WiRR] Utworzono awaryjny szablon raportu: {assetPath}");
            }

            var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
            if (textAsset != null)
            {
                Selection.activeObject = textAsset;
                EditorGUIUtility.PingObject(textAsset);
                AssetDatabase.OpenAsset(textAsset);
            }
            else
            {
                EditorUtility.RevealInFinder(fullPath);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace KIA.WiRR.Editor
{
    /// <summary>
    /// Detects WiRR samples imported directly from Unity Package Manager and
    /// prepares the standardized Assets/WiRR/LabXX workspace automatically.
    /// </summary>
    internal sealed class WiRRSampleImportPostprocessor : AssetPostprocessor
    {
        private static readonly HashSet<int> PendingLabs = new HashSet<int>();
        private static bool flushScheduled;

        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            foreach (var assetPath in importedAssets)
            {
                var labNumber = DetectLabNumber(assetPath);
                if (labNumber > 0)
                    PendingLabs.Add(labNumber);
            }

            if (PendingLabs.Count == 0 || flushScheduled)
                return;

            flushScheduled = true;
            EditorApplication.delayCall += FlushPendingLabs;
        }

        private static int DetectLabNumber(string assetPath)
        {
            if (string.IsNullOrWhiteSpace(assetPath) ||
                !assetPath.StartsWith("Assets/Samples/", StringComparison.OrdinalIgnoreCase))
                return 0;

            if (assetPath.IndexOf("WiRR Course Toolkit", StringComparison.OrdinalIgnoreCase) < 0)
                return 0;

            for (var labNumber = 1; labNumber <= 7; labNumber++)
            {
                if (assetPath.IndexOf($"Lab {labNumber:00}", StringComparison.OrdinalIgnoreCase) >= 0)
                    return labNumber;
            }

            return 0;
        }

        private static void FlushPendingLabs()
        {
            flushScheduled = false;
            if (PendingLabs.Count == 0)
                return;

            var labs = new int[PendingLabs.Count];
            PendingLabs.CopyTo(labs);
            PendingLabs.Clear();

            foreach (var labNumber in labs)
            {
                try
                {
                    WiRRSceneTools.PrepareLabWorkspace(labNumber);
                    Debug.Log($"[WiRR] Wykryto import próbki Lab {labNumber:00}; przygotowano workspace {WiRRSceneTools.GetLabRootPath(labNumber)}.");
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }

            WiRRCourseWindow.RepaintOpenWindow();
        }
    }
}

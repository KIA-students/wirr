using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace KIA.WiRR.Editor
{
    /// <summary>Records coarse progress locally; never uploads identifiers to GitHub.</summary>
    [InitializeOnLoad]
    internal static class WiRRProgressTracker
    {
        [Serializable]
        private class LabProgress
        {
            public int lab;
            public bool scenePrepared;
            public bool reportExported;
        }

        [Serializable]
        private class Snapshot
        {
            public string schema = "wirr-local-progress/1";
            public string pseudonymousId;
            public string group;
            public string updatedAtUtc;
            public string unityVersion;
            public LabProgress[] labs;
        }

        private static double nextCheck;
        static WiRRProgressTracker() => EditorApplication.update += Tick;

        private static void Tick()
        {
            if (EditorApplication.timeSinceStartup < nextCheck) return;
            nextCheck = EditorApplication.timeSinceStartup + 60;
            if (Application.isBatchMode || EditorApplication.isPlayingOrWillChangePlaymode ||
                EditorApplication.isCompiling || !WiRRStudentProfile.IsRegistered) return;
            try
            {
                var labs = new LabProgress[8];
                var reports = Path.GetFullPath(Path.Combine(Application.dataPath, "../Library/WiRRReports/exports"));
                for (var lab = 1; lab <= 8; lab++)
                {
                    var sceneFolder = "Assets/WiRR/Lab" + lab.ToString("00");
                    labs[lab - 1] = new LabProgress
                    {
                        lab = lab,
                        scenePrepared = AssetDatabase.IsValidFolder(sceneFolder)
                            && AssetDatabase.FindAssets("t:Scene", new[] { sceneFolder }).Length > 0,
                        reportExported = Directory.Exists(reports)
                            && Directory.GetFiles(reports, "*-lab" + lab.ToString("00") + "-*.json").Length > 0
                    };
                }
                var root = Path.GetFullPath(Path.Combine(Application.dataPath, "../Library/WiRRProgress"));
                Directory.CreateDirectory(root);
                var target = Path.Combine(root, "progress.json");
                var snapshot = new Snapshot
                {
                    pseudonymousId = WiRRStudentProfile.Identifier,
                    group = WiRRStudentProfile.GroupLabel,
                    updatedAtUtc = DateTime.UtcNow.ToString("O"),
                    unityVersion = Application.unityVersion,
                    labs = labs
                };
                File.WriteAllText(target, JsonUtility.ToJson(snapshot, true));
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                Debug.LogWarning("[WiRR] Nie udało się zapisać lokalnego postępu: " + ex.Message);
            }
        }
    }
}

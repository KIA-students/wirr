using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace KIA.WiRR.Editor
{
    [InitializeOnLoad]
    internal static class WiRROnboarding
    {
        static WiRROnboarding()
        {
            EditorApplication.delayCall += OpenToolkitOncePerProjectAndVersion;
        }

        private static void OpenToolkitOncePerProjectAndVersion()
        {
            if (Application.isBatchMode || EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling)
                return;

            var packageInfo = PackageInfo.FindForPackageName("pl.prz.kia.wirr");
            if (packageInfo == null || string.IsNullOrWhiteSpace(packageInfo.version))
                return;

            var projectId = Hash128.Compute(Application.dataPath).ToString();
            var key = $"KIA.WiRR.Onboarding.{packageInfo.version}.{projectId}";
            if (EditorPrefs.GetBool(key, false))
                return;

            EditorPrefs.SetBool(key, true);
            WiRRCourseWindow.Open();
            Debug.Log($"[WiRR] Pierwsze uruchomienie WiRR {packageInfo.version}: otwarto panel Narzędzia kursu.");
        }
    }
}

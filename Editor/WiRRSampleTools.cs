using System.Linq;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.UI;
using UnityEngine;

namespace KIA.WiRR.Editor
{
    internal static class WiRRSampleTools
    {
        public static bool ImportCourseSample(int labNumber)
        {
            var definition = WiRRLabCatalog.Get(labNumber);
            var packageInfo = PackageInfo.FindForPackageName("pl.prz.kia.wirr");
            if (packageInfo == null)
            {
                Debug.LogError("[WiRR] Nie można odczytać informacji o pakiecie WiRR.");
                return false;
            }

            var samples = Sample.FindByPackage(packageInfo.name, packageInfo.version);
            var sample = samples.FirstOrDefault(s => s.displayName == definition.SampleName);
            if (string.IsNullOrEmpty(sample.displayName))
            {
                Debug.LogError($"[WiRR] Brak próbki: {definition.SampleName}");
                return false;
            }

            var result = sample.Import(Sample.ImportOptions.OverridePreviousImports);
            Debug.Log(result
                ? $"[WiRR] Zaimportowano próbkę: {sample.displayName}"
                : $"[WiRR] Import próbki nie został wykonany: {sample.displayName}");
            return result;
        }

        public static void ImportOfficialSamples(int labNumber)
        {
            var definition = WiRRLabCatalog.Get(labNumber);
            if (definition.ExternalSamples.Count == 0)
            {
                Debug.Log($"[WiRR] Lab {labNumber:00} nie wymaga dodatkowych oficjalnych próbek Unity.");
                return;
            }

            foreach (var requested in definition.ExternalSamples)
            {
                var packageInfo = PackageInfo.FindForPackageName(requested.PackageName);
                if (packageInfo == null)
                {
                    Debug.LogWarning($"[WiRR] Najpierw zainstaluj pakiet {requested.PackageName}.");
                    continue;
                }

                var samples = Sample.FindByPackage(packageInfo.name, packageInfo.version);
                var sample = samples.FirstOrDefault(s => s.displayName == requested.PreferredSampleName);
                if (string.IsNullOrEmpty(sample.displayName) && !string.IsNullOrEmpty(requested.FallbackSampleName))
                    sample = samples.FirstOrDefault(s => s.displayName == requested.FallbackSampleName);

                if (string.IsNullOrEmpty(sample.displayName))
                {
                    Debug.LogWarning($"[WiRR] Nie znaleziono próbki '{requested.PreferredSampleName}' w {packageInfo.name}@{packageInfo.version}.");
                    continue;
                }

                if (sample.isImported)
                {
                    Debug.Log($"[WiRR] Próbka już zaimportowana: {sample.displayName}");
                    continue;
                }

                var imported = sample.Import(Sample.ImportOptions.None);
                Debug.Log(imported
                    ? $"[WiRR] Zaimportowano oficjalną próbkę: {sample.displayName}"
                    : $"[WiRR] Nie udało się zaimportować próbki: {sample.displayName}");
            }
        }

        public static Sample? FindCourseSample(int labNumber)
        {
            var definition = WiRRLabCatalog.Get(labNumber);
            var packageInfo = PackageInfo.FindForPackageName("pl.prz.kia.wirr");
            if (packageInfo == null)
                return null;

            var sample = Sample.FindByPackage(packageInfo.name, packageInfo.version)
                .FirstOrDefault(s => s.displayName == definition.SampleName);
            return string.IsNullOrEmpty(sample.displayName) ? null : sample;
        }
    }
}

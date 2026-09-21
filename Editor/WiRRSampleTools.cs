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
            if (result)
            {
                WiRRSceneTools.PrepareLabWorkspace(labNumber);
                Debug.Log($"[WiRR] Zaimportowano próbkę i przygotowano folder roboczy laboratorium {labNumber:00}: {WiRRSceneTools.GetLabRootPath(labNumber)}");
            }
            else
            {
                Debug.Log($"[WiRR] Import próbki nie został wykonany: {sample.displayName}");
            }
            return result;
        }

        public static void ImportOfficialSamples(int labNumber)
        {
            var definition = WiRRLabCatalog.Get(labNumber);
            if (definition.ExternalSamples.Count == 0)
            {
                Debug.Log($"[WiRR] Laboratorium {labNumber:00} nie wymaga dodatkowych oficjalnych próbek Unity.");
                return;
            }

            var importedAny = false;
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
                importedAny |= imported;
                Debug.Log(imported
                    ? $"[WiRR] Zaimportowano oficjalną próbkę: {sample.displayName}"
                    : $"[WiRR] Nie udało się zaimportować próbki: {sample.displayName}");
            }

            if (importedAny)
                WiRRSceneTools.PrepareLabWorkspace(labNumber);
        }

        public static bool IsCourseSampleImported(int labNumber)
        {
            var sampleNullable = FindCourseSample(labNumber);
            return sampleNullable.HasValue && sampleNullable.Value.isImported;
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

using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace KIA.WiRR.Editor
{
    internal static class WiRRAdditionalModelTools
    {
        private const string WiRRPackageName = "pl.prz.kia.wirr";
        private const string UnitreePackageFile = "RoboAnimation.unitypackage";

        public static bool UnitreePackageAvailable => TryGetUnitreePackagePath(out _);

        public static void ImportUnitreeG1()
        {
            if (!TryGetUnitreePackagePath(out var packagePath))
            {
                EditorUtility.DisplayDialog(
                    "WiRR — brak modelu Unitree G1 EDU",
                    $"Nie znaleziono {UnitreePackageFile} w zainstalowanym pakiecie WiRR. Zainstaluj pełne wydanie pakietu i spróbuj ponownie.",
                    "OK");
                return;
            }

            if (!EditorUtility.DisplayDialog(
                    "Import modelu Unitree G1 EDU",
                    "Zostanie otwarte standardowe okno importu Unity dla pakietu RoboAnimation.unitypackage. Pakiet zawiera model 3D humanoidalnego robota Unitree G1 EDU oraz przykładowe animacje. Przejrzyj listę zasobów i wybierz Import, aby dodać je do własnego projektu.",
                    "Otwórz import",
                    "Anuluj"))
                return;

            AssetDatabase.ImportPackage(packagePath, true);
            Debug.Log($"[WiRR] Otwarto import dodatkowego modelu Unitree G1 EDU: {packagePath}");
        }

        public static void RevealUnitreePackage()
        {
            if (!TryGetUnitreePackagePath(out var packagePath))
            {
                Debug.LogWarning($"[WiRR] Nie znaleziono {UnitreePackageFile} w zainstalowanym pakiecie.");
                return;
            }

            EditorUtility.RevealInFinder(packagePath);
        }

        private static bool TryGetUnitreePackagePath(out string packagePath)
        {
            packagePath = string.Empty;
            var packageInfo = PackageInfo.FindForPackageName(WiRRPackageName);
            if (packageInfo == null || string.IsNullOrWhiteSpace(packageInfo.resolvedPath))
                return false;

            packagePath = Path.Combine(packageInfo.resolvedPath, UnitreePackageFile);
            return File.Exists(packagePath);
        }
    }
}

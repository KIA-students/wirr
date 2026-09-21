using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KIA.WiRR;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace KIA.WiRR.Editor
{
    internal enum WiRRValidationSeverity { Info, Warning, Error }

    internal readonly struct WiRRValidationResult
    {
        public WiRRValidationSeverity Severity { get; }
        public string Message { get; }

        public WiRRValidationResult(WiRRValidationSeverity severity, string message)
        {
            Severity = severity;
            Message = message;
        }
    }

    internal static class WiRRSceneValidator
    {
        public static List<WiRRValidationResult> Validate(int labNumber)
        {
            var results = new List<WiRRValidationResult>();
            var definition = WiRRLabCatalog.Get(labNumber);
            ValidateUnity(results);
            ValidatePackages(results, definition);
            ValidateScene(results, labNumber);
            ValidateLabSpecific(results, labNumber);

            foreach (var result in results)
            {
                var prefix = $"[WiRR][{result.Severity}] ";
                if (result.Severity == WiRRValidationSeverity.Error)
                    Debug.LogError(prefix + result.Message);
                else if (result.Severity == WiRRValidationSeverity.Warning)
                    Debug.LogWarning(prefix + result.Message);
                else
                    Debug.Log(prefix + result.Message);
            }
            return results;
        }

        private static void ValidateUnity(List<WiRRValidationResult> results)
        {
            if (Application.unityVersion.StartsWith("6000.6.", StringComparison.Ordinal))
                Pass(results, $"Unity {Application.unityVersion} — wersja referencyjna 6000.6.x.");
            else
                Warn(results, $"Unity {Application.unityVersion}; kurs referencyjnie używa 6000.6.x.");

            if (GraphicsSettings.defaultRenderPipeline != null)
                Pass(results, "Aktywny potok renderowania SRP — projekt nie korzysta z trybu Built-in.");
            else
                Error(results, "Brak aktywnego zasobu Render Pipeline. Utwórz projekt Universal 3D (URP) lub przypisz właściwy zasób URP.");
        }

        private static void ValidatePackages(List<WiRRValidationResult> results, WiRRLabDefinition definition)
        {
            var manifestPath = Path.GetFullPath(Path.Combine(Application.dataPath, "../Packages/manifest.json"));
            var manifest = File.Exists(manifestPath) ? File.ReadAllText(manifestPath) : string.Empty;
            RequireManifest(results, manifest, "com.unity.render-pipelines.universal", "URP");
            RequireManifest(results, manifest, "com.unity.inputsystem", "Input System");

            foreach (var identifier in definition.Packages)
            {
                var packageName = PackageNameFromIdentifier(identifier);
                RequireManifest(results, manifest, packageName, packageName);
            }
        }

        private static void ValidateScene(List<WiRRValidationResult> results, int labNumber)
        {
            var scene = SceneManager.GetActiveScene();
            if (!scene.IsValid())
            {
                Error(results, "Brak aktywnej sceny.");
                return;
            }

            if (string.IsNullOrWhiteSpace(scene.path))
                Warn(results, "Scena nie została jeszcze zapisana.");
            else
                Pass(results, $"Scena: {scene.path}");

            var mainCameras = UnityEngine.Object.FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .Count(c => c.CompareTag("MainCamera") && c.gameObject.activeInHierarchy);
            if (mainCameras == 1)
                Pass(results, "W scenie znajduje się dokładnie jedna aktywna kamera oznaczona tagiem MainCamera.");
            else
                Error(results, $"Wymagana jest dokładnie jedna aktywna kamera z tagiem MainCamera; wykryto: {mainCameras}.");

            var markers = UnityEngine.Object.FindObjectsByType<WiRRSceneMarker>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .Where(m => m.gameObject.scene == scene)
                .ToArray();
            if (markers.Length == 1 && markers[0].LabNumber == labNumber)
                Pass(results, $"Scena zawiera dokładnie jeden WiRRSceneMarker dla Lab {labNumber:00}.");
            else if (markers.Length == 0)
                Error(results, $"Brak znacznika WiRRSceneMarker dla laboratorium {labNumber:00}. Użyj polecenia „Utwórz / napraw aktywną scenę”.");
            else if (markers.Length == 1)
                Error(results, $"Znacznik WiRRSceneMarker wskazuje laboratorium {markers[0].LabNumber:00}, a wybrano laboratorium {labNumber:00}. Użyj polecenia „Utwórz / napraw aktywną scenę”.");
            else
                Error(results, $"W aktywnej scenie powinien znajdować się dokładnie jeden WiRRSceneMarker; wykryto: {markers.Length}. Użyj polecenia „Utwórz / napraw aktywną scenę”.");

            var missingScripts = 0;
            foreach (var root in scene.GetRootGameObjects())
            foreach (var transform in root.GetComponentsInChildren<Transform>(true))
                missingScripts += GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(transform.gameObject);

            if (missingScripts == 0)
                Pass(results, "Nie wykryto komponentów z brakującym skryptem (Missing Script).");
            else
                Error(results, $"Wykryto komponenty z brakującym skryptem (Missing Script): {missingScripts}.");
        }

        private static void ValidateLabSpecific(List<WiRRValidationResult> results, int labNumber)
        {
            if (labNumber == 1 || labNumber == 2)
            {
                ValidateComponentCount(results, "Unity.XR.CoreUtils.XROrigin, Unity.XR.CoreUtils", "XR Origin", exactlyOne: true);
                Info(results, "Sprawdź również, czy aktywne jest środowisko OpenXR oraz XR Interaction Simulator zgodnie z instrukcją.");
            }
            else if (labNumber == 3 || labNumber == 4)
            {
                ValidateComponentCount(results, "UnityEngine.XR.ARFoundation.ARSession, Unity.XR.ARFoundation", "AR Session", exactlyOne: true);
                ValidateComponentCount(results, "Unity.XR.CoreUtils.XROrigin, Unity.XR.CoreUtils", "XR Origin", exactlyOne: true);
                if (labNumber == 4)
                {
                    ValidateComponentCount(results, "UnityEngine.XR.ARFoundation.ARPlaneManager, Unity.XR.ARFoundation", "AR Plane Manager", exactlyOne: false);
                    ValidateWebSim(results, required: false, role: "cyfrowego cienia");
                }
            }
            else if (labNumber == 6)
            {
                ValidateWebSim(results, required: false, role: "bliźniaka cyfrowego");
                Info(results, "Dozwolone są trzy źródła stanu: lokalny ROS 2/Gazebo, ROS 2/Gazebo na drugim komputerze albo WiRR WebSim przez WSS.");
            }
            else if (labNumber == 7)
            {
                Info(results, "Uruchom także testy EditMode i PlayMode wymagane w laboratorium 07. Walidator WiRR nie zastępuje narzędzia Unity Test Runner.");
            }
        }

        private static void ValidateWebSim(List<WiRRValidationResult> results, bool required, string role)
        {
            var sources = UnityEngine.Object.FindObjectsByType<WebSimStateSource>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            if (sources.Length == 0)
            {
                if (required)
                    Error(results, $"Brak WebSimStateSource dla {role}.");
                else
                    Info(results, $"WebSim nie jest skonfigurowany. Dla {role} jest to opcjonalne źródło stanu.");
                return;
            }

            if (sources.Length > 1)
                Warn(results, $"Wykryto {sources.Length} komponentów WebSimStateSource; zwykle powinien być dokładnie jeden.");
            else
                Pass(results, "Wykryto WebSimStateSource.");

            var source = sources[0];
            if (string.IsNullOrWhiteSpace(source.BackendWebSocketUrl))
                Warn(results, "WebSimStateSource nie ma ustawionego adresu backendu WSS.");
            else if (source.BackendWebSocketUrl.StartsWith("wss://", StringComparison.OrdinalIgnoreCase))
                Pass(results, $"Backend WebSim używa WSS: {source.BackendWebSocketUrl}");
            else
                Warn(results, "Serwer WebSim nie używa wss://. Adres ws:// jest przeznaczony głównie do testów lokalnych lub w sieci LAN.");

            if (!string.IsNullOrWhiteSpace(source.SessionCode))
                Pass(results, $"Kod sesji WebSim: {source.SessionCode}.");
            else
                Error(results, "Brak kodu sesji WebSim.");

            var rigs = UnityEngine.Object.FindObjectsByType<WiRRRobotRig>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            if (rigs.Length == 1)
                Pass(results, "Dokładnie jeden komponent WiRRRobotRig odwzorowuje dane z /joint_states.");
            else if (rigs.Length == 0)
                Error(results, "Brak komponentu WiRRRobotRig. Użyj przycisku „Utwórz / napraw model WebSim”.");
            else
                Warn(results, $"Wykryto więcej niż jeden komponent WiRRRobotRig: {rigs.Length}.");
        }

        private static void ValidateComponentCount(List<WiRRValidationResult> results, string qualifiedTypeName, string label, bool exactlyOne)
        {
            var type = Type.GetType(qualifiedTypeName);
            if (type == null)
            {
                Warn(results, $"Nie można rozpoznać typu {label}; sprawdź, czy wymagany pakiet jest zainstalowany.");
                return;
            }

            var scene = SceneManager.GetActiveScene();
            var count = UnityEngine.Object.FindObjectsByType(type, FindObjectsInactive.Include, FindObjectsSortMode.None)
                .OfType<Component>()
                .Count(component => component.gameObject.scene == scene);

            if (exactlyOne && count == 1)
                Pass(results, $"Dokładnie jeden {label}.");
            else if (exactlyOne)
                Error(results, $"Wymagany jest dokładnie jeden {label}; wykryto: {count}.");
            else if (count >= 1)
                Pass(results, $"Wykryto {label}: {count}.");
            else
                Warn(results, $"Nie wykryto {label}; zweryfikuj wymagania instrukcji.");
        }

        private static void RequireManifest(List<WiRRValidationResult> results, string manifest, string packageName, string label)
        {
            if (manifest.Contains($"\"{packageName}\"", StringComparison.Ordinal))
                Pass(results, $"Pakiet obecny: {label}.");
            else
                Error(results, $"Brak wymaganego pakietu: {label} ({packageName}).");
        }

        private static string PackageNameFromIdentifier(string identifier)
        {
            if (identifier.Contains("ROS-TCP-Connector", StringComparison.OrdinalIgnoreCase))
                return "com.unity.robotics.ros-tcp-connector";
            var at = identifier.IndexOf('@');
            return at > 0 ? identifier.Substring(0, at) : identifier;
        }

        private static void Pass(List<WiRRValidationResult> results, string message) => results.Add(new WiRRValidationResult(WiRRValidationSeverity.Info, "OK — " + message));
        private static void Info(List<WiRRValidationResult> results, string message) => results.Add(new WiRRValidationResult(WiRRValidationSeverity.Info, message));
        private static void Warn(List<WiRRValidationResult> results, string message) => results.Add(new WiRRValidationResult(WiRRValidationSeverity.Warning, message));
        private static void Error(List<WiRRValidationResult> results, string message) => results.Add(new WiRRValidationResult(WiRRValidationSeverity.Error, message));
    }
}

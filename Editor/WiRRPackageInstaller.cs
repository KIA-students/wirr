using System;
using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace KIA.WiRR.Editor
{
    [InitializeOnLoad]
    internal static class WiRRPackageInstaller
    {
        private const string PendingKey = "KIA.WiRR.PendingPackages";
        private const string CurrentKey = "KIA.WiRR.CurrentPackage";
        private const string StatusKey = "KIA.WiRR.InstallStatus";
        private static AddRequest activeRequest;

        static WiRRPackageInstaller()
        {
            EditorApplication.delayCall += Resume;
        }

        public static string Status => SessionState.GetString(StatusKey, "Brak aktywnej instalacji.");

        public static bool IsBusy =>
            activeRequest != null ||
            !string.IsNullOrWhiteSpace(SessionState.GetString(CurrentKey, string.Empty)) ||
            !string.IsNullOrWhiteSpace(SessionState.GetString(PendingKey, string.Empty));

        public static void InstallForLab(int labNumber)
        {
            if (IsBusy)
            {
                Debug.LogWarning("[WiRR] Instalator jest już zajęty.");
                return;
            }

            var definition = WiRRLabCatalog.Get(labNumber);
            SessionState.SetString(PendingKey, string.Join("\n", definition.Packages));
            SessionState.SetString(CurrentKey, string.Empty);
            SetStatus(definition.Packages.Count == 0
                ? $"Lab {labNumber:00}: brak dodatkowych zależności do instalacji."
                : $"Lab {labNumber:00}: rozpoczęto instalację zależności.");
            Resume();
        }

        public static void Cancel()
        {
            activeRequest = null;
            EditorApplication.update -= PollActiveRequest;
            SessionState.EraseString(PendingKey);
            SessionState.EraseString(CurrentKey);
            SetStatus("Instalacja została anulowana. Aktywna operacja UPM może dokończyć się w tle.");
        }

        private static void Resume()
        {
            if (activeRequest != null)
                return;

            var current = SessionState.GetString(CurrentKey, string.Empty);
            if (!string.IsNullOrWhiteSpace(current))
            {
                if (IsAlreadyInstalled(current))
                {
                    SessionState.SetString(CurrentKey, string.Empty);
                    Resume();
                    return;
                }

                StartAdd(current);
                return;
            }

            var queue = SessionState.GetString(PendingKey, string.Empty);
            if (string.IsNullOrWhiteSpace(queue))
                return;

            var items = queue.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (items.Length == 0)
            {
                SessionState.EraseString(PendingKey);
                SetStatus("Instalacja zależności zakończona.");
                return;
            }

            var next = items[0].Trim();
            var remaining = items.Length > 1 ? string.Join("\n", items, 1, items.Length - 1) : string.Empty;
            SessionState.SetString(PendingKey, remaining);
            SessionState.SetString(CurrentKey, next);

            if (IsAlreadyInstalled(next))
            {
                SetStatus($"Już zainstalowano: {next}");
                SessionState.SetString(CurrentKey, string.Empty);
                Resume();
                return;
            }

            StartAdd(next);
        }

        private static void StartAdd(string identifier)
        {
            try
            {
                SetStatus($"Instalowanie: {identifier}");
                activeRequest = Client.Add(identifier);
                EditorApplication.update -= PollActiveRequest;
                EditorApplication.update += PollActiveRequest;
            }
            catch (Exception exception)
            {
                Fail(identifier, exception.Message);
            }
        }

        private static void PollActiveRequest()
        {
            if (activeRequest == null || !activeRequest.IsCompleted)
                return;

            EditorApplication.update -= PollActiveRequest;
            var current = SessionState.GetString(CurrentKey, string.Empty);

            if (activeRequest.Status == StatusCode.Success)
            {
                var installed = activeRequest.Result != null
                    ? $"{activeRequest.Result.name}@{activeRequest.Result.version}"
                    : current;
                SessionState.SetString(CurrentKey, string.Empty);
                activeRequest = null;
                SetStatus($"Zainstalowano: {installed}");
                EditorApplication.delayCall += Resume;
                return;
            }

            var error = activeRequest.Error != null
                ? activeRequest.Error.message
                : "Nieznany błąd Unity Package Manager.";
            activeRequest = null;
            Fail(current, error);
        }

        private static void Fail(string identifier, string error)
        {
            EditorApplication.update -= PollActiveRequest;
            SessionState.EraseString(PendingKey);
            SessionState.EraseString(CurrentKey);
            activeRequest = null;
            SetStatus($"Błąd instalacji {identifier}: {error}");
            Debug.LogError($"[WiRR] Nie udało się zainstalować {identifier}: {error}");
        }

        private static bool IsAlreadyInstalled(string identifier)
        {
            if (identifier.Contains("ROS-TCP-Connector", StringComparison.OrdinalIgnoreCase))
                return UnityEditor.PackageManager.PackageInfo.IsPackageRegistered("com.unity.robotics.ros-tcp-connector");

            var manifestPath = Path.GetFullPath(Path.Combine(Application.dataPath, "../Packages/manifest.json"));
            if (!File.Exists(manifestPath))
                return false;

            var manifest = File.ReadAllText(manifestPath);
            var at = identifier.IndexOf('@');
            if (at <= 0)
                return UnityEditor.PackageManager.PackageInfo.IsPackageRegistered(identifier);

            var packageName = identifier.Substring(0, at);
            var version = identifier.Substring(at + 1);
            return manifest.Contains($"\"{packageName}\": \"{version}\"", StringComparison.Ordinal);
        }

        private static void SetStatus(string message)
        {
            SessionState.SetString(StatusKey, message);
            Debug.Log($"[WiRR] {message}");
            WiRRCourseWindow.RepaintOpenWindow();
        }
    }
}

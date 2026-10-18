using System;
using KIA.WiRR;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KIA.WiRR.Editor
{
    internal static class WiRRWebSimTools
    {
        public const string BackendPref = "KIA.WiRR.WebSim.Backend";
        public const string SessionPref = "KIA.WiRR.WebSim.Session";
        public const string RobotPref = "KIA.WiRR.WebSim.Robot";

        public static string Backend { get => EditorPrefs.GetString(BackendPref, string.Empty); set => EditorPrefs.SetString(BackendPref, value ?? string.Empty); }
        public static string Session { get => EditorPrefs.GetString(SessionPref, "TEAM01"); set => EditorPrefs.SetString(SessionPref, NormalizeSession(value)); }
        public static string Robot { get => EditorPrefs.GetString(RobotPref, "rrbot"); set => EditorPrefs.SetString(RobotPref, value == "wirr-arm3" ? "wirr-arm3" : "rrbot"); }

        [MenuItem("WiRR/WebSim/Utwórz lub napraw model robota", priority = 30)]
        public static void CreateFromMenu()
        {
            CreateOrRepairRig(6);
        }

        public static bool ValidateConfiguration(out string message)
        {
            if (!Uri.TryCreate(Backend, UriKind.Absolute, out var uri) || (uri.Scheme != "ws" && uri.Scheme != "wss"))
            {
                message = "Podaj adres backendu zaczynający się od ws:// lub wss://.";
                return false;
            }
            message = $"Backend: {uri.Host}, sesja: {Session}, robot: {Robot}.";
            return true;
        }

        public static WebSimStateSource FindSource() => UnityEngine.Object.FindFirstObjectByType<WebSimStateSource>();

        public static string RuntimeStatus()
        {
            if (!Application.isPlaying) return "Uruchom tryb Play.";
            var source = FindSource();
            if (source == null) return "Brak modelu WebSim: najpierw go utwórz.";
            if (source.IsConnecting) return "ŁĄCZENIE…";
            if (!source.IsConnected) return "ROZŁĄCZONO";
            return source.IsStale ? "STALE: dane nie są aktualizowane" : "LIVE: dane są aktualne";
        }

        public static void CreateOrRepairRig(int labNumber)
        {
            WiRRSceneTools.PrepareBaseScene(labNumber);
            var root = GameObject.Find("WiRR_WebSim") ?? new GameObject("WiRR_WebSim");
            var source = root.GetComponent<WebSimStateSource>() ?? Undo.AddComponent<WebSimStateSource>(root);
            source.Configure(Backend, Session, Robot);

            var robotRoot = GameObject.Find("WiRR_WebSim_Robot");
            if (robotRoot != null) Undo.DestroyObjectImmediate(robotRoot);
            robotRoot = new GameObject("WiRR_WebSim_Robot");
            robotRoot.transform.SetParent(root.transform, false);

            var names = Robot == "wirr-arm3" ? new[] { "joint1", "joint2", "joint3" } : new[] { "joint1", "joint2" };
            var lengths = Robot == "wirr-arm3" ? new[] { 0.8f, 0.65f, 0.45f } : new[] { 0.9f, 0.75f };
            var joints = BuildRobot(robotRoot.transform, names, lengths);
            var rig = robotRoot.AddComponent<WiRRRobotRig>();
            rig.Configure(source, names, joints, Vector3.forward);
            robotRoot.transform.position = Vector3.zero;
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        public static async void ConnectInPlayMode()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("[WiRR] Uruchom tryb Play.");
                return;
            }
            if (!ValidateConfiguration(out var message))
            {
                Debug.LogWarning("[WiRR] " + message);
                return;
            }
            var source = FindSource();
            if (source == null) { Debug.LogWarning("[WiRR] Najpierw utwórz model WebSim."); return; }
            source.Configure(Backend, Session, Robot);
            try { await source.ConnectAsync(); } catch (Exception e) { Debug.LogError("[WiRR] " + e.Message); }
        }

        public static async void DisconnectInPlayMode() { var source = FindSource(); if (source != null) await source.DisconnectAsync(); }
        public static async void SendMotion(string motion) { var source = FindSource(); if (source != null && source.IsConnected) await source.SendMotionAsync(motion); }
        public static async void SendHome() { var source = FindSource(); if (source != null && source.IsConnected) await source.SendHomeAsync(); }
        public static async void SendReset() { var source = FindSource(); if (source != null && source.IsConnected) await source.SendResetAsync(); }

        private static Transform[] BuildRobot(Transform parent, string[] names, float[] lengths)
        {
            var result = new Transform[names.Length];
            var current = parent;
            for (var i = 0; i < names.Length; i++)
            {
                var joint = new GameObject(names[i]);
                joint.transform.SetParent(current, false);
                joint.transform.localPosition = i == 0 ? Vector3.zero : new Vector3(0f, lengths[i - 1], 0f);
                result[i] = joint.transform;
                var link = GameObject.CreatePrimitive(PrimitiveType.Cube);
                link.name = $"link{i + 1}";
                link.transform.SetParent(joint.transform, false);
                link.transform.localPosition = new Vector3(0f, lengths[i] / 2f, 0f);
                link.transform.localScale = new Vector3(0.12f, lengths[i], 0.12f);
                current = joint.transform;
            }
            return result;
        }

        private static string NormalizeSession(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "TEAM01";
            var result = string.Empty;
            foreach (var c in value.Trim().ToUpperInvariant()) if (char.IsLetterOrDigit(c) || c == '_' || c == '-') result += c;
            return result.Length > 12 ? result.Substring(0, 12) : result;
        }
    }
}
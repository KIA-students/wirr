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

        public static string Backend
        {
            get => EditorPrefs.GetString(BackendPref, string.Empty);
            set => EditorPrefs.SetString(BackendPref, value ?? string.Empty);
        }

        public static string Session
        {
            get => EditorPrefs.GetString(SessionPref, "TEAM01");
            set => EditorPrefs.SetString(SessionPref, NormalizeSession(value));
        }

        public static string Robot
        {
            get => EditorPrefs.GetString(RobotPref, "rrbot");
            set => EditorPrefs.SetString(RobotPref, value == "wirr-arm3" ? "wirr-arm3" : "rrbot");
        }

        [MenuItem("WiRR/WebSim/Create or repair digital shadow", priority = 30)]
        public static void CreateFromMenu()
        {
            var lab = EditorPrefs.GetInt("KIA.WiRR.SelectedLab", 6);
            CreateOrRepairRig(lab == 4 ? 4 : 6);
        }

        public static void CreateOrRepairRig(int labNumber)
        {
            WiRRSceneTools.PrepareBaseScene(labNumber);
            var root = GameObject.Find("WiRR_WebSim");
            if (root == null)
            {
                root = new GameObject("WiRR_WebSim");
                Undo.RegisterCreatedObjectUndo(root, "Create WiRR WebSim root");
            }

            var source = root.GetComponent<WebSimStateSource>();
            if (source == null)
                source = Undo.AddComponent<WebSimStateSource>(root);
            source.Configure(Backend, Session, Robot);
            EditorUtility.SetDirty(source);

            var robotRoot = GameObject.Find("WiRR_WebSim_Robot");
            if (robotRoot != null)
                Undo.DestroyObjectImmediate(robotRoot);
            robotRoot = new GameObject("WiRR_WebSim_Robot");
            Undo.RegisterCreatedObjectUndo(robotRoot, "Create WiRR robot");
            robotRoot.transform.SetParent(root.transform, false);

            var lengths = Robot == "wirr-arm3" ? new[] { 0.8f, 0.65f, 0.45f } : new[] { 0.9f, 0.75f };
            var names = Robot == "wirr-arm3" ? new[] { "joint1", "joint2", "joint3" } : new[] { "joint1", "joint2" };
            var joints = BuildRobot(robotRoot.transform, names, lengths);

            var rig = robotRoot.AddComponent<WiRRRobotRig>();
            rig.Configure(source, names, joints, Vector3.forward);
            EditorUtility.SetDirty(rig);

            robotRoot.transform.position = labNumber == 4 ? new Vector3(0f, 0f, 1.5f) : Vector3.zero;
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            Debug.Log($"[WiRR] WebSim {(labNumber == 4 ? "digital shadow" : "digital twin")} ready: robot={Robot}, session={Session}.");
        }

        public static async void ConnectInPlayMode()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("[WiRR] Połączenie WebSim uruchamiaj w Play Mode.");
                return;
            }

            var source = UnityEngine.Object.FindFirstObjectByType<WebSimStateSource>();
            if (source == null)
            {
                Debug.LogError("[WiRR] Brak WebSimStateSource. Najpierw utwórz cyfrowy cień/bliźniak.");
                return;
            }

            source.Configure(Backend, Session, Robot);
            try
            {
                await source.ConnectAsync();
            }
            catch (Exception exception)
            {
                Debug.LogError("[WiRR] WebSim connect failed: " + exception.Message);
            }
        }

        public static async void SendMotion(string motion)
        {
            if (!Application.isPlaying)
                return;
            var source = UnityEngine.Object.FindFirstObjectByType<WebSimStateSource>();
            if (source == null || !source.IsConnected)
            {
                Debug.LogWarning("[WiRR] WebSim nie jest połączony.");
                return;
            }
            await source.SendMotionAsync(motion);
        }

        private static Transform[] BuildRobot(Transform parent, string[] names, float[] lengths)
        {
            var baseObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            baseObject.name = "base_link";
            baseObject.transform.SetParent(parent, false);
            baseObject.transform.localScale = new Vector3(0.35f, 0.12f, 0.35f);
            baseObject.transform.localPosition = new Vector3(0f, 0.12f, 0f);

            var result = new Transform[names.Length];
            var currentParent = parent;
            var currentHeight = 0.24f;
            for (var i = 0; i < names.Length; i++)
            {
                var joint = new GameObject(names[i]);
                joint.transform.SetParent(currentParent, false);
                joint.transform.localPosition = i == 0 ? new Vector3(0f, currentHeight, 0f) : new Vector3(0f, lengths[i - 1], 0f);
                result[i] = joint.transform;

                var link = GameObject.CreatePrimitive(PrimitiveType.Cube);
                link.name = $"link{i + 1}";
                link.transform.SetParent(joint.transform, false);
                link.transform.localPosition = new Vector3(0f, lengths[i] * 0.5f, 0f);
                link.transform.localScale = new Vector3(0.12f, lengths[i], 0.12f);
                currentParent = joint.transform;
            }
            return result;
        }

        private static string NormalizeSession(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "TEAM01";
            var result = string.Empty;
            foreach (var character in value.Trim().ToUpperInvariant())
                if (char.IsLetterOrDigit(character) || character == '_' || character == '-')
                    result += character;
            if (result.Length < 3) result += "001";
            return result.Length > 12 ? result.Substring(0, 12) : result;
        }
    }
}

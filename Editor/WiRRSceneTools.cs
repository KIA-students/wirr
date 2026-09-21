using System.IO;
using KIA.WiRR;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KIA.WiRR.Editor
{
    internal static class WiRRSceneTools
    {
        public static void PrepareBaseScene(int labNumber)
        {
            var scene = SceneManager.GetActiveScene();
            if (!scene.IsValid())
            {
                Debug.LogError("[WiRR] Brak aktywnej sceny.");
                return;
            }

            EnsureFolders(labNumber);
            var rootName = $"WiRR_Lab{labNumber:00}";
            var root = GameObject.Find(rootName);
            if (root == null)
            {
                root = new GameObject(rootName);
                Undo.RegisterCreatedObjectUndo(root, "Create WiRR lab root");
            }

            EnsureSingleSceneMarker(scene, root, labNumber);

            EnsureMainCamera();
            EnsureDirectionalLight();
            EnsureGround();
            EditorSceneManager.MarkSceneDirty(scene);

            if (string.IsNullOrWhiteSpace(scene.path))
            {
                var scenePath = $"Assets/WiRR/Lab{labNumber:00}/Scenes/Lab{labNumber:00}.unity";
                EditorSceneManager.SaveScene(scene, scenePath);
                Debug.Log($"[WiRR] Zapisano scenę bazową: {scenePath}");
            }
            else
            {
                Debug.Log($"[WiRR] Naprawiono scenę bazową dla Lab {labNumber:00}.");
            }
        }

        public static void ToggleMetrics(int labNumber)
        {
            EnsureFolders(labNumber);
            var tools = GameObject.Find("WiRR_Tools");
            if (tools == null)
            {
                tools = new GameObject("WiRR_Tools");
                Undo.RegisterCreatedObjectUndo(tools, "Create WiRR tools");
            }

            var metrics = tools.GetComponent<WiRRFrameMetrics>();
            if (metrics == null)
            {
                Undo.AddComponent<WiRRFrameMetrics>(tools);
                Debug.Log("[WiRR] Dodano WiRRFrameMetrics.");
            }
            else
            {
                Undo.DestroyObjectImmediate(metrics);
                Debug.Log("[WiRR] Usunięto WiRRFrameMetrics.");
            }

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        public static void EnsureFolders(int labNumber)
        {
            var paths = new[]
            {
                "Assets/WiRR",
                $"Assets/WiRR/Lab{labNumber:00}",
                $"Assets/WiRR/Lab{labNumber:00}/Scenes",
                $"Assets/WiRR/Lab{labNumber:00}/Evidence",
                "Assets/WiRR/Reports"
            };

            foreach (var path in paths)
                EnsureFolder(path);
        }

        private static void EnsureMainCamera()
        {
            var cameras = Object.FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var candidate in cameras)
            {
                if (candidate.CompareTag("MainCamera"))
                    return;
            }

            var go = new GameObject("Main Camera");
            Undo.RegisterCreatedObjectUndo(go, "Create Main Camera");
            go.AddComponent<Camera>();
            go.tag = "MainCamera";
            go.transform.position = new Vector3(0f, 1.6f, -3f);
            go.transform.rotation = Quaternion.Euler(10f, 0f, 0f);
        }

        private static void EnsureDirectionalLight()
        {
            var lights = Object.FindObjectsByType<Light>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var light in lights)
            {
                if (light.type == LightType.Directional)
                    return;
            }

            var go = new GameObject("Directional Light");
            Undo.RegisterCreatedObjectUndo(go, "Create Directional Light");
            var lightComponent = go.AddComponent<Light>();
            lightComponent.type = LightType.Directional;
            lightComponent.intensity = 1f;
            go.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }

        private static void EnsureGround()
        {
            var ground = GameObject.Find("WiRR_Ground");
            if (ground == null)
            {
                ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
                Undo.RegisterCreatedObjectUndo(ground, "Create WiRR Ground");
                ground.name = "WiRR_Ground";
                ground.transform.localScale = new Vector3(2f, 1f, 2f);
            }

            // A primitive Plane comes with a MeshCollider. In Unity 6000.6 this can
            // produce a warning about missing pre-baked triangle collision data.
            // The course ground is flat, so a BoxCollider is simpler and deterministic.
            foreach (var meshCollider in ground.GetComponents<MeshCollider>())
                Undo.DestroyObjectImmediate(meshCollider);

            var boxCollider = ground.GetComponent<BoxCollider>();
            if (boxCollider == null)
                boxCollider = Undo.AddComponent<BoxCollider>(ground);
            boxCollider.center = new Vector3(0f, -0.01f, 0f);
            boxCollider.size = new Vector3(10f, 0.02f, 10f);
            boxCollider.isTrigger = false;
            EditorUtility.SetDirty(boxCollider);
        }

        private static void EnsureSingleSceneMarker(Scene scene, GameObject root, int labNumber)
        {
            var rootMarker = root.GetComponent<WiRRSceneMarker>();
            if (rootMarker == null)
                rootMarker = Undo.AddComponent<WiRRSceneMarker>(root);

            rootMarker.LabNumber = labNumber;
            EditorUtility.SetDirty(rootMarker);

            var markers = Object.FindObjectsByType<WiRRSceneMarker>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var marker in markers)
            {
                if (marker == null || marker == rootMarker || marker.gameObject.scene != scene)
                    continue;

                Undo.DestroyObjectImmediate(marker);
            }
        }

        private static void EnsureFolder(string assetPath)
        {
            if (AssetDatabase.IsValidFolder(assetPath))
                return;

            var parent = Path.GetDirectoryName(assetPath)?.Replace('\\', '/');
            var name = Path.GetFileName(assetPath);
            if (string.IsNullOrEmpty(parent))
                return;
            if (!AssetDatabase.IsValidFolder(parent))
                EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, name);
        }
    }
}

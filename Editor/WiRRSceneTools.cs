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
        private static readonly string[] WorkspaceFolders =
        {
            "Scenes",
            "Scripts",
            "Materials",
            "Models",
            "Prefabs",
            "Textures",
            "Data",
            "Evidence",
            "Documentation"
        };

        public static string GetLabRootPath(int labNumber) => $"Assets/WiRR/Lab{labNumber:00}";
        public static string GetScenePath(int labNumber) => $"{GetLabRootPath(labNumber)}/Scenes/Lab{labNumber:00}.unity";

        public static bool WorkspaceExists(int labNumber)
        {
            var root = GetLabRootPath(labNumber);
            if (!AssetDatabase.IsValidFolder(root))
                return false;

            foreach (var folder in WorkspaceFolders)
                if (!AssetDatabase.IsValidFolder($"{root}/{folder}"))
                    return false;

            return true;
        }

        public static bool SceneExists(int labNumber) => AssetDatabase.LoadAssetAtPath<SceneAsset>(GetScenePath(labNumber)) != null;

        public static void PrepareLabWorkspace(int labNumber)
        {
            EnsureFolders(labNumber);
            EnsureWorkspaceScene(labNumber);
            AssetDatabase.Refresh();
        }

        public static void PrepareBaseScene(int labNumber)
        {
            var scene = SceneManager.GetActiveScene();
            if (!scene.IsValid())
            {
                Debug.LogError("[WiRR] Brak aktywnej sceny.");
                return;
            }

            EnsureFolders(labNumber);
            ConfigureBaseScene(scene, labNumber);
            EditorSceneManager.MarkSceneDirty(scene);

            if (string.IsNullOrWhiteSpace(scene.path))
            {
                var scenePath = GetScenePath(labNumber);
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
            EnsureFolder("Assets/WiRR");
            var root = GetLabRootPath(labNumber);
            EnsureFolder(root);
            foreach (var folder in WorkspaceFolders)
                EnsureFolder($"{root}/{folder}");
            EnsureFolder("Assets/WiRR/Reports");
        }

        public static void OpenLabFolder(int labNumber)
        {
            EnsureFolders(labNumber);
            var folder = AssetDatabase.LoadAssetAtPath<DefaultAsset>(GetLabRootPath(labNumber));
            if (folder == null) return;
            Selection.activeObject = folder;
            EditorGUIUtility.PingObject(folder);
        }

        public static void OpenLabScene(int labNumber)
        {
            PrepareLabWorkspace(labNumber);
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return;
            EditorSceneManager.OpenScene(GetScenePath(labNumber), OpenSceneMode.Single);
        }

        private static void EnsureWorkspaceScene(int labNumber)
        {
            var scenePath = GetScenePath(labNumber);
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath) != null)
                return;

            var previousActive = SceneManager.GetActiveScene();
            var workspaceScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            SceneManager.SetActiveScene(workspaceScene);
            ConfigureBaseScene(workspaceScene, labNumber);
            EditorSceneManager.SaveScene(workspaceScene, scenePath);
            EditorSceneManager.CloseScene(workspaceScene, true);

            if (previousActive.IsValid() && previousActive.isLoaded)
                SceneManager.SetActiveScene(previousActive);

            Debug.Log($"[WiRR] Utworzono workspace Lab {labNumber:00}: {GetLabRootPath(labNumber)}");
        }

        private static void ConfigureBaseScene(Scene scene, int labNumber)
        {
            var rootName = $"WiRR_Lab{labNumber:00}";
            var root = FindGameObjectInScene(scene, rootName);
            if (root == null)
            {
                root = new GameObject(rootName);
                SceneManager.MoveGameObjectToScene(root, scene);
                Undo.RegisterCreatedObjectUndo(root, "Create WiRR lab root");
            }

            EnsureSingleSceneMarker(scene, root, labNumber);
            EnsureMainCamera(scene);
            EnsureDirectionalLight(scene);
            EnsureGround(scene);
        }

        private static void EnsureMainCamera(Scene scene)
        {
            foreach (var root in scene.GetRootGameObjects())
            foreach (var candidate in root.GetComponentsInChildren<Camera>(true))
                if (candidate.CompareTag("MainCamera"))
                    return;

            var go = new GameObject("Main Camera");
            SceneManager.MoveGameObjectToScene(go, scene);
            Undo.RegisterCreatedObjectUndo(go, "Create Main Camera");
            go.AddComponent<Camera>();
            go.tag = "MainCamera";
            go.transform.position = new Vector3(0f, 1.6f, -3f);
            go.transform.rotation = Quaternion.Euler(10f, 0f, 0f);
        }

        private static void EnsureDirectionalLight(Scene scene)
        {
            foreach (var root in scene.GetRootGameObjects())
            foreach (var light in root.GetComponentsInChildren<Light>(true))
                if (light.type == LightType.Directional)
                    return;

            var go = new GameObject("Directional Light");
            SceneManager.MoveGameObjectToScene(go, scene);
            Undo.RegisterCreatedObjectUndo(go, "Create Directional Light");
            var lightComponent = go.AddComponent<Light>();
            lightComponent.type = LightType.Directional;
            lightComponent.intensity = 1f;
            go.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }

        private static void EnsureGround(Scene scene)
        {
            var ground = FindGameObjectInScene(scene, "WiRR_Ground");
            if (ground == null)
            {
                ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
                SceneManager.MoveGameObjectToScene(ground, scene);
                Undo.RegisterCreatedObjectUndo(ground, "Create WiRR Ground");
                ground.name = "WiRR_Ground";
                ground.transform.localScale = new Vector3(2f, 1f, 2f);
            }

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

        private static GameObject FindGameObjectInScene(Scene scene, string objectName)
        {
            foreach (var root in scene.GetRootGameObjects())
            foreach (var transform in root.GetComponentsInChildren<Transform>(true))
                if (transform.name == objectName)
                    return transform.gameObject;
            return null;
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

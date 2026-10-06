using UnityEditor;
using UnityEngine;

namespace KIA.WiRR.Editor
{
    internal static class WiRRVitureTools
    {
        private const string RootName = "WiRR_VITURE_FullSBS";
        private const string DemoRoot = "Assets/WiRR/VITURE/Demos";

        [MenuItem("WiRR/VITURE XR/Utwórz przykład Full SBS 3840x1080", priority = 40)]
        public static void CreateFullSbsRig()
        {
            var existing = GameObject.Find(RootName);
            if (existing != null)
            {
                Selection.activeGameObject = existing;
                EditorGUIUtility.PingObject(existing);
                Debug.Log("[WiRR][VITURE] Przykład Full SBS już istnieje w scenie.");
                return;
            }

            var source = Camera.main;
            var root = new GameObject(RootName);
            Undo.RegisterCreatedObjectUndo(root, "Utwórz przykład VITURE Full SBS");

            if (source != null)
            {
                root.transform.position = source.transform.position;
                root.transform.rotation = source.transform.rotation;
            }

            var leftObject = new GameObject("VITURE_LeftEye");
            var rightObject = new GameObject("VITURE_RightEye");
            Undo.RegisterCreatedObjectUndo(leftObject, "Utwórz lewą kamerę VITURE");
            Undo.RegisterCreatedObjectUndo(rightObject, "Utwórz prawą kamerę VITURE");
            leftObject.transform.SetParent(root.transform, false);
            rightObject.transform.SetParent(root.transform, false);

            var left = Undo.AddComponent<Camera>(leftObject);
            var right = Undo.AddComponent<Camera>(rightObject);
            if (source != null)
            {
                left.CopyFrom(source);
                right.CopyFrom(source);
                Undo.RecordObject(source, "Wyłącz kamerę bazową dla Full SBS");
                source.enabled = false;
            }

            left.tag = "MainCamera";
            right.tag = "Untagged";

            var rig = Undo.AddComponent<WiRRVitureSbsRig>(root);
            rig.Configure(left, right, 0.064f);

            Selection.activeGameObject = root;
            EditorSceneManagerCompat.MarkActiveSceneDirty();
            Debug.Log("[WiRR][VITURE] Utworzono Full SBS. Dla natywnego trybu 3D VITURE ustaw wynik 3840x1080 i przełącz okulary w tryb 3D.");
        }

        [MenuItem("WiRR/VITURE XR/Wygeneruj 3 sceny demonstracyjne", priority = 41)]
        public static void CreateDemoScenes()
        {
            if (!UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return;

            EnsureAssetFolder("Assets/WiRR");
            EnsureAssetFolder("Assets/WiRR/VITURE");
            EnsureAssetFolder(DemoRoot);

            CreateDemoScene("VITURE_01_StereoDepthLayers.unity", WiRRVitureDemoPreset.StereoDepthLayers);
            CreateDemoScene("VITURE_02_StereoComfort.unity", WiRRVitureDemoPreset.StereoComfort);
            CreateDemoScene("VITURE_03_Immersive2D_Source.unity", WiRRVitureDemoPreset.Immersive2DSource);

            AssetDatabase.Refresh();
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(DemoRoot + "/VITURE_01_StereoDepthLayers.unity");
            Debug.Log("[WiRR][VITURE] Wygenerowano trzy sceny demonstracyjne w Assets/WiRR/VITURE/Demos.");
        }

        [MenuItem("WiRR/VITURE XR/Instrukcja VITURE XR", priority = 42)]
        public static void OpenGuide() => WiRRVitureGuideWindow.Open();

        private static void CreateDemoScene(string fileName, WiRRVitureDemoPreset preset)
        {
            var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(
                UnityEditor.SceneManagement.NewSceneSetup.EmptyScene,
                UnityEditor.SceneManagement.NewSceneMode.Single);

            var root = new GameObject(System.IO.Path.GetFileNameWithoutExtension(fileName));
            var bootstrap = root.AddComponent<WiRRVitureDemoBootstrap>();
            bootstrap.Configure(preset);

            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene, DemoRoot + "/" + fileName);
        }

        private static void EnsureAssetFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
                return;

            var parent = System.IO.Path.GetDirectoryName(path)?.Replace('\\', '/');
            var name = System.IO.Path.GetFileName(path);
            if (!string.IsNullOrWhiteSpace(parent) && !AssetDatabase.IsValidFolder(parent))
                EnsureAssetFolder(parent);
            if (!string.IsNullOrWhiteSpace(parent))
                AssetDatabase.CreateFolder(parent, name);
        }
    }

    internal static class EditorSceneManagerCompat
    {
        public static void MarkActiveSceneDirty()
        {
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (scene.IsValid())
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);
        }
    }
}

using UnityEditor;
using UnityEngine;

namespace KIA.WiRR.Editor
{
    internal static class WiRRVitureTools
    {
        private const string RootName = "WiRR_VITURE_FullSBS";

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

        [MenuItem("WiRR/VITURE XR/Instrukcja VITURE XR", priority = 41)]
        public static void OpenGuide() => WiRRVitureGuideWindow.Open();
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

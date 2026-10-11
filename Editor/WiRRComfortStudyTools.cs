using System;
using System.IO;
using KIA.WiRR;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KIA.WiRR.Editor
{
    /// <summary>
    /// Creates two self-contained, conservative comfort-study prefabs.
    /// No additional XR/AR packages or external models are needed.
    /// Scenes are not played automatically and no account is required.
    /// </summary>
    internal static class WiRRComfortStudyTools
    {
        private const string Folder = "Assets/WiRR/Common/Prefabs/ComfortStudy";
        private const string OpticName = "WiRR_Comfort_OpticFlow";
        private const string DepthName = "WiRR_Comfort_DepthCues";

        [MenuItem("WiRR/Komfort XR/Utwórz 2 prefaby i stanowiska obserwacyjne", priority = 80)]
        public static void CreateStations()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorUtility.DisplayDialog("WiRR", "Wyjdź z Play Mode przed tworzeniem prefabów.", "OK");
                return;
            }
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

            EnsureFolder(Folder);
            var optic = SavePrefab(OpticName, BuildVisualMotion());
            var depth = SavePrefab(DepthName, BuildDepthCues());

            if (SceneManager.GetActiveScene().IsValid())
            {
                PlaceIfMissing(optic, OpticName);
                PlaceIfMissing(depth, DepthName);
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[WiRR] Gotowe prefaby: " + optic.name + ", " + depth.name +
                      ". Student rozpoczyna pokaz ręcznie, Space/A; Esc/B przerywa. Nie uruchamiać dla wywoływania objawów.");
            EditorUtility.DisplayDialog("WiRR, komfort XR",
                "Utworzono dwa prefaby i stanowiska obserwacyjne. Pokazy są krótkie i wymagają ręcznego uruchomienia. " +
                "Nie służą do wywoływania nudności ani rywalizacji w odporności. Wymagany jest obserwator i możliwość natychmiastowego przerwania.", "OK");
        }

        private static GameObject BuildVisualMotion()
        {
            var root = new GameObject(OpticName);
            var background = new GameObject("Wzorzec_ruchomy");
            background.transform.SetParent(root.transform, false);
            background.transform.localPosition = new Vector3(-1.6f, 0.0f, 3.8f);
            for (var i = 0; i < 7; i++)
            {
                var stripe = MakePrimitive(background.transform, "Pasek_" + (i + 1),
                    PrimitiveType.Cube, new Vector3(i * 0.5f, 1.5f, 0f),
                    new Vector3(0.12f, 2.2f, 0.06f));
                stripe.GetComponent<Renderer>().sharedMaterial = GetMaterial(i % 2 == 0 ? "Turkus" : "Bialy");
            }
            Label(root.transform, "Tekst_staly", "Porownaj: nieruchomy znak vs powolny ruch wzorca",
                new Vector3(-1.9f, 2.85f, 3.5f));
            var component = root.AddComponent<WiRRVisualMotionStudy>();
            component.Configure(background.transform);
            return root;
        }

        private static GameObject BuildDepthCues()
        {
            var root = new GameObject(DepthName);
            var reference = MakePrimitive(root.transform, "Obiekt_staly",
                PrimitiveType.Cube, new Vector3(-1.2f, 1.5f, 2.4f), Vector3.one * 0.35f);
            reference.GetComponent<Renderer>().sharedMaterial = GetMaterial("Bialy");
            var marker = MakePrimitive(root.transform, "Obiekt_zmienna_glebia",
                PrimitiveType.Sphere, new Vector3(0f, 1.5f, 2.4f), Vector3.one * 0.34f);
            marker.GetComponent<Renderer>().sharedMaterial = GetMaterial("Turkus");
            Label(root.transform, "Tekst_staly", "Jeden ekran optyczny, rozne odleglosci wirtualne",
                new Vector3(-1.9f, 2.6f, 2.9f));
            var component = root.AddComponent<WiRRDepthCueStudy>();
            component.Configure(marker.transform);
            return root;
        }

        private static GameObject SavePrefab(string name, GameObject root)
        {
            try
            {
                var path = Folder + "/" + name + ".prefab";
                // Re-running the tool must never overwrite a student-edited prefab.
                var existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (existing != null) return existing;
                var saved = PrefabUtility.SaveAsPrefabAsset(root, path);
                if (saved == null) throw new InvalidOperationException("Nie udało się utworzyć " + path);
                return saved;
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
            }
        }

        private static GameObject MakePrimitive(Transform parent, string name,
            PrimitiveType shape, Vector3 position, Vector3 scale)
        {
            var gameObject = GameObject.CreatePrimitive(shape);
            gameObject.name = name;
            gameObject.transform.SetParent(parent, false);
            gameObject.transform.localPosition = position;
            gameObject.transform.localScale = scale;
            var collider = gameObject.GetComponent<Collider>();
            if (collider != null) UnityEngine.Object.DestroyImmediate(collider);
            return gameObject;
        }

        private static void Label(Transform parent, string name, string content, Vector3 position)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = position;
            var text = go.AddComponent<TextMesh>();
            text.text = content;
            text.fontSize = 42;
            text.characterSize = 0.015f;
            text.color = Color.white;
            text.anchor = TextAnchor.MiddleLeft;
        }

        private static Material GetMaterial(string name)
        {
            var path = Folder + "/WiRR_" + name + ".mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material != null) return material;
            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            if (shader == null) throw new InvalidOperationException("Nie znaleziono shadera URP/Lit ani Standard.");
            material = new Material(shader) { name = "WiRR_" + name };
            var color = name == "Turkus" ? new Color(0.12f, 0.82f, 0.73f) : new Color(0.95f, 0.96f, 1f);
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            else material.color = color;
            AssetDatabase.CreateAsset(material, path);
            return material;
        }

        private static void PlaceIfMissing(GameObject prefab, string name)
        {
            var scene = SceneManager.GetActiveScene();
            foreach (var root in scene.GetRootGameObjects())
                if (root.name == name) return;

            var instance = PrefabUtility.InstantiatePrefab(prefab, scene) as GameObject;
            if (instance == null) return;
            Undo.RegisterCreatedObjectUndo(instance, "WiRR " + name);
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            var parent = Path.GetDirectoryName(path)?.Replace('\\', '/');
            if (!string.IsNullOrWhiteSpace(parent) && !AssetDatabase.IsValidFolder(parent))
                EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, Path.GetFileName(path));
        }
    }
}

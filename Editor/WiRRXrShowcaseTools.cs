using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KIA.WiRR.Editor
{
    /// <summary>
    /// Generates self-contained XR showcase prefabs. Each prefab already contains
    /// runtime behaviour and can run without XRI. Public methods on the runtime
    /// components are intentionally exposed for later XRI / hand-tracking wiring.
    /// </summary>
    internal static class WiRRXrShowcaseTools
    {
        private const string TeachingRootName = "WiRR_TeachingAssets";
        private const string ShowcaseRootName = "XRShowcase";
        private const string PrefabRoot = "Assets/WiRR/Common/Prefabs/XRShowcase";

        private sealed class Definition
        {
            public string Name { get; }
            public Vector3 Position { get; }
            public Func<GameObject> Builder { get; }

            public Definition(string name, Vector3 position, Func<GameObject> builder)
            {
                Name = name;
                Position = position;
                Builder = builder;
            }
        }

        public static void CreateAll(int labNumber)
        {
            foreach (var definition in Definitions())
                CreateOne(labNumber, definition);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void CreateParallaxPortal(int labNumber) =>
            CreateOne(labNumber, Find("XR_ParallaxPortal"));

        public static void CreateGazeBloom(int labNumber) =>
            CreateOne(labNumber, Find("XR_GazeBloom"));

        public static void CreateTelekinesisOrb(int labNumber) =>
            CreateOne(labNumber, Find("XR_TelekinesisOrb"));

        public static void CreateDiegeticHud(int labNumber) =>
            CreateOne(labNumber, Find("XR_DiegeticHUD"));

        public static void CreateWorldScaleTotem(int labNumber) =>
            CreateOne(labNumber, Find("XR_WorldScaleTotem"));

        public static bool ShowcaseExists(int labNumber)
        {
            if (labNumber < 1 || labNumber > 7)
                return false;

            var scene = SceneManager.GetActiveScene();
            if (!scene.IsValid())
                return false;

            var labRoot = FindInScene(scene, $"WiRR_Lab{labNumber:00}");
            var showcase = labRoot?.transform.Find($"{TeachingRootName}/{ShowcaseRootName}");
            return showcase != null && showcase.childCount > 0;
        }

        public static void RemoveShowcase(int labNumber)
        {
            ValidateLabNumber(labNumber);
            var scene = SceneManager.GetActiveScene();
            if (!scene.IsValid())
                return;

            var labRoot = FindInScene(scene, $"WiRR_Lab{labNumber:00}");
            var showcase = labRoot?.transform.Find($"{TeachingRootName}/{ShowcaseRootName}");
            if (showcase == null)
                return;

            Undo.DestroyObjectImmediate(showcase.gameObject);
            EditorSceneManager.MarkSceneDirty(scene);
            Debug.Log($"[WiRR] Usunięto demonstratory XR z Lab {labNumber:00}.");
        }

        private static IEnumerable<Definition> Definitions()
        {
            return new[]
            {
                new Definition("XR_ParallaxPortal", new Vector3(-3.0f, 0f, 2.8f), BuildParallaxPortal),
                new Definition("XR_GazeBloom", new Vector3(-1.45f, 0f, 2.5f), BuildGazeBloom),
                new Definition("XR_TelekinesisOrb", new Vector3(0f, 0f, 2.65f), BuildTelekinesisOrb),
                new Definition("XR_DiegeticHUD", new Vector3(1.55f, 1.35f, 2.25f), BuildDiegeticHud),
                new Definition("XR_WorldScaleTotem", new Vector3(3.05f, 0f, 2.75f), BuildWorldScaleTotem)
            };
        }

        private static Definition Find(string name)
        {
            foreach (var definition in Definitions())
                if (definition.Name == name)
                    return definition;

            throw new ArgumentException($"Nieznany demonstrator XR: {name}", nameof(name));
        }

        private static GameObject BuildParallaxPortal()
        {
            var root = new GameObject("XR_ParallaxPortal");
            var metal = WiRRSurfaceTextureLibrary.GetMaterial(WiRRSurfaceFamily.Metal044A);
            var dark = WiRRSurfaceTextureLibrary.GetMaterial(WiRRSurfaceFamily.DiamondPlate);
            var accent = WiRRPrefabTextureLibrary.GetMaterial(WiRRGridTextureSet.Grid3, WiRRGridMaterialRole.Accent);

            Box(root.transform, "Frame_Left", new Vector3(-0.78f, 1.15f, 0f), new Vector3(0.12f, 2.30f, 0.18f), metal, true);
            Box(root.transform, "Frame_Right", new Vector3(0.78f, 1.15f, 0f), new Vector3(0.12f, 2.30f, 0.18f), metal, true);
            Box(root.transform, "Frame_Top", new Vector3(0f, 2.25f, 0f), new Vector3(1.68f, 0.12f, 0.18f), metal, true);
            Box(root.transform, "Frame_Bottom", new Vector3(0f, 0.05f, 0f), new Vector3(1.68f, 0.12f, 0.18f), metal, true);

            var layerNear = new GameObject("Layer_Near");
            layerNear.transform.SetParent(root.transform, false);
            layerNear.transform.localPosition = new Vector3(0f, 1.15f, 0.18f);
            for (var i = 0; i < 8; i++)
            {
                var angle = i * Mathf.PI * 2f / 8f;
                Primitive(
                    layerNear.transform,
                    $"NearNode_{i + 1}",
                    PrimitiveType.Sphere,
                    new Vector3(Mathf.Cos(angle) * 0.56f, Mathf.Sin(angle) * 0.82f, 0f),
                    Vector3.one * 0.10f,
                    accent,
                    false);
            }

            var layerMid = new GameObject("Layer_Mid");
            layerMid.transform.SetParent(root.transform, false);
            layerMid.transform.localPosition = new Vector3(0f, 1.15f, 0.40f);
            for (var x = -1; x <= 1; x++)
            for (var y = -2; y <= 2; y++)
            {
                Primitive(
                    layerMid.transform,
                    $"Mid_{x}_{y}",
                    PrimitiveType.Cube,
                    new Vector3(x * 0.34f, y * 0.31f, 0f),
                    Vector3.one * 0.07f,
                    dark,
                    false);
            }

            var layerFar = new GameObject("Layer_Far");
            layerFar.transform.SetParent(root.transform, false);
            layerFar.transform.localPosition = new Vector3(0f, 1.15f, 0.72f);
            var core = Primitive(
                layerFar.transform,
                "PortalCore",
                PrimitiveType.Sphere,
                Vector3.zero,
                Vector3.one * 0.58f,
                accent,
                false);

            var spin = core.AddComponent<WiRRLoopMotion>();
            spin.Configure(WiRRLoopMotionMode.Rotate, new Vector3(0.3f, 1f, 0.2f), 8f, 0f, true);

            var parallax = root.AddComponent<WiRRHeadParallax>();
            parallax.Configure(
                new[] { layerNear.transform, layerMid.transform, layerFar.transform },
                new[] { 0.95f, 0.52f, 0.20f },
                0.28f);

            Note(root.transform, "XR_6DoF_move_head_sideways_and_vertically_to_reveal_parallax");
            Note(root.transform, "Educational_head_coupled_perspective_without_XR_SDK_dependency");
            return root;
        }

        private static GameObject BuildGazeBloom()
        {
            var root = new GameObject("XR_GazeBloom");
            var concrete = WiRRSurfaceTextureLibrary.GetMaterial(WiRRSurfaceFamily.Concrete);
            var metal = WiRRSurfaceTextureLibrary.GetMaterial(WiRRSurfaceFamily.Metal004);
            var accent = WiRRPrefabTextureLibrary.GetMaterial(WiRRGridTextureSet.Grid2, WiRRGridMaterialRole.Accent);

            Box(root.transform, "Pedestal", new Vector3(0f, 0.14f, 0f), new Vector3(0.75f, 0.28f, 0.75f), concrete, true);

            var bloomRoot = new GameObject("Bloom");
            bloomRoot.transform.SetParent(root.transform, false);
            bloomRoot.transform.localPosition = new Vector3(0f, 1.05f, 0f);

            var core = Primitive(
                bloomRoot.transform,
                "Core",
                PrimitiveType.Sphere,
                Vector3.zero,
                Vector3.one * 0.34f,
                accent,
                false);

            var petals = new Transform[10];
            for (var i = 0; i < petals.Length; i++)
            {
                var angle = i * Mathf.PI * 2f / petals.Length;
                var petal = Primitive(
                    bloomRoot.transform,
                    $"Petal_{i + 1}",
                    PrimitiveType.Capsule,
                    new Vector3(Mathf.Cos(angle) * 0.47f, Mathf.Sin(angle) * 0.47f, 0f),
                    new Vector3(0.10f, 0.22f, 0.10f),
                    i % 2 == 0 ? metal : accent,
                    false);
                petal.transform.localRotation = Quaternion.Euler(0f, 0f, -angle * Mathf.Rad2Deg);
                petals[i] = petal.transform;
            }

            var gaze = bloomRoot.AddComponent<WiRRGazeBloom>();
            gaze.Configure(core.transform, petals, 0.42f);

            var audio = bloomRoot.AddComponent<WiRRProceduralAudio>();
            audio.Configure(WiRRProceduralAudioProfile.Hum, 105f, 0.035f, 1f);

            Note(root.transform, "Look_at_the_core_then_look_away_to_compare_implicit_gaze_feedback");
            Note(root.transform, "XRI_or_hand_tracking_can_call_SetExternalActivation_0_to_1");
            return root;
        }

        private static GameObject BuildTelekinesisOrb()
        {
            var root = new GameObject("XR_TelekinesisOrb");
            var concrete = WiRRSurfaceTextureLibrary.GetMaterial(WiRRSurfaceFamily.Concrete);
            var metal = WiRRSurfaceTextureLibrary.GetMaterial(WiRRSurfaceFamily.Metal044A);
            var accent = WiRRPrefabTextureLibrary.GetMaterial(WiRRGridTextureSet.Grid1, WiRRGridMaterialRole.Accent);

            Box(root.transform, "Pedestal", new Vector3(0f, 0.16f, 0f), new Vector3(0.72f, 0.32f, 0.72f), concrete, true);
            Primitive(root.transform, "EmitterRing", PrimitiveType.Cylinder, new Vector3(0f, 0.36f, 0f), new Vector3(0.43f, 0.035f, 0.43f), metal, false);

            var orb = new GameObject("MovableOrb");
            orb.transform.SetParent(root.transform, false);
            orb.transform.localPosition = new Vector3(0f, 1.05f, 0f);

            Primitive(orb.transform, "Core", PrimitiveType.Sphere, Vector3.zero, Vector3.one * 0.34f, accent, false);
            for (var i = 0; i < 3; i++)
            {
                var ring = Primitive(
                    orb.transform,
                    $"Ring_{i + 1}",
                    PrimitiveType.Cylinder,
                    Vector3.zero,
                    new Vector3(0.48f + i * 0.07f, 0.018f, 0.48f + i * 0.07f),
                    metal,
                    false);
                ring.transform.localRotation = Quaternion.Euler(90f + i * 28f, i * 38f, 0f);
            }

            var line = root.AddComponent<LineRenderer>();
            line.useWorldSpace = true;
            line.positionCount = 2;
            line.sharedMaterial = accent;
            line.textureMode = LineTextureMode.Stretch;
            line.numCapVertices = 3;
            line.enabled = false;

            var telekinesis = root.AddComponent<WiRRTelekinesisOrb>();
            telekinesis.Configure(orb.transform, line, 0.78f, 0.85f);

            Note(root.transform, "Gaze_at_orb_for_0_85s_to_pull_it_toward_the_head");
            Note(root.transform, "Look_away_to_release_or_connect_XRI_Select_to_BeginHold_and_Release");
            return root;
        }

        private static GameObject BuildDiegeticHud()
        {
            var root = new GameObject("XR_DiegeticHUD");
            var metal = WiRRSurfaceTextureLibrary.GetMaterial(WiRRSurfaceFamily.Metal004);
            var fabric = WiRRSurfaceTextureLibrary.GetMaterial(WiRRSurfaceFamily.Fabric023);
            var accent = WiRRPrefabTextureLibrary.GetMaterial(WiRRGridTextureSet.Grid3, WiRRGridMaterialRole.Accent);

            Box(root.transform, "Panel", Vector3.zero, new Vector3(0.86f, 0.48f, 0.045f), fabric, false);
            Box(root.transform, "Header", new Vector3(0f, 0.19f, -0.035f), new Vector3(0.72f, 0.045f, 0.035f), accent, false);

            for (var i = 0; i < 4; i++)
            {
                Box(
                    root.transform,
                    $"DataBar_{i + 1}",
                    new Vector3(-0.23f + i * 0.15f, -0.03f, -0.04f),
                    new Vector3(0.08f, 0.21f + i * 0.035f, 0.028f),
                    i % 2 == 0 ? accent : metal,
                    false);
            }

            var status = Primitive(
                root.transform,
                "StatusOrb",
                PrimitiveType.Sphere,
                new Vector3(0.31f, 0.11f, -0.08f),
                Vector3.one * 0.10f,
                accent,
                false);
            var orbit = status.AddComponent<WiRRLoopMotion>();
            orbit.Configure(WiRRLoopMotionMode.Bob, new Vector3(0f, 0.05f, 0f), 1.8f, 0f, true);

            var follower = root.AddComponent<WiRRHeadFollower>();
            follower.Configure(1.05f, 0.42f, -0.08f);

            Note(root.transform, "Soft_follow_reduces_discomfort_vs_rigid_HMD_parenting");
            Note(root.transform, "Call_Pin_or_Unpin_to_compare_world_locked_and_head_referenced_UI");
            return root;
        }

        private static GameObject BuildWorldScaleTotem()
        {
            var root = new GameObject("XR_WorldScaleTotem");
            var concrete = WiRRSurfaceTextureLibrary.GetMaterial(WiRRSurfaceFamily.Concrete);
            var metal = WiRRSurfaceTextureLibrary.GetMaterial(WiRRSurfaceFamily.DiamondPlate);
            var accent = WiRRPrefabTextureLibrary.GetMaterial(WiRRGridTextureSet.Grid2, WiRRGridMaterialRole.Accent);

            Box(root.transform, "Base", new Vector3(0f, 0.09f, 0f), new Vector3(0.92f, 0.18f, 0.92f), concrete, true);

            var content = new GameObject("ScaleContent");
            content.transform.SetParent(root.transform, false);
            content.transform.localPosition = new Vector3(0f, 0.18f, 0f);

            Box(content.transform, "Body", new Vector3(0f, 0.58f, 0f), new Vector3(0.42f, 0.72f, 0.28f), metal, false);
            Primitive(content.transform, "Head", PrimitiveType.Sphere, new Vector3(0f, 1.05f, 0f), Vector3.one * 0.30f, accent, false);

            for (var side = -1; side <= 1; side += 2)
            {
                Box(content.transform, side < 0 ? "Arm_L" : "Arm_R", new Vector3(side * 0.34f, 0.62f, 0f), new Vector3(0.16f, 0.58f, 0.16f), accent, false);
                Box(content.transform, side < 0 ? "Leg_L" : "Leg_R", new Vector3(side * 0.14f, 0.18f, 0f), new Vector3(0.15f, 0.48f, 0.18f), metal, false);
            }

            var scale = root.AddComponent<WiRRProximityScale>();
            scale.Configure(content.transform, 0.16f, 1.75f);

            var hum = root.AddComponent<WiRRProceduralAudio>();
            hum.Configure(WiRRProceduralAudioProfile.Hum, 62f, 0.028f, 1f);

            Note(root.transform, "Walk_toward_the_totem_to_experience_tabletop_to_room_scale_transition");
            Note(root.transform, "XRI_slider_or_pinch_can_call_SetExternalFactor_0_to_1");
            return root;
        }

        private static GameObject Primitive(
            Transform parent,
            string name,
            PrimitiveType type,
            Vector3 localPosition,
            Vector3 localScale,
            Material material,
            bool keepCollider)
        {
            var go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPosition;
            go.transform.localScale = localScale;

            var renderer = go.GetComponent<Renderer>();
            if (renderer != null)
                renderer.sharedMaterial = material;

            if (!keepCollider)
            {
                var collider = go.GetComponent<Collider>();
                if (collider != null)
                    UnityEngine.Object.DestroyImmediate(collider);
            }

            return go;
        }

        private static GameObject Box(
            Transform parent,
            string name,
            Vector3 localPosition,
            Vector3 localScale,
            Material material,
            bool keepCollider)
        {
            return Primitive(parent, name, PrimitiveType.Cube, localPosition, localScale, material, keepCollider);
        }

        private static void Note(Transform parent, string name)
        {
            var note = new GameObject(name);
            note.transform.SetParent(parent, false);
        }

        private static GameObject SavePrefab(string path, Func<GameObject> builder)
        {
            EnsureFolder(Path.GetDirectoryName(path)?.Replace('\\', '/'));
            var temporary = builder();

            try
            {
                temporary.transform.position = Vector3.zero;
                temporary.transform.rotation = Quaternion.identity;
                PrefabUtility.SaveAsPrefabAsset(temporary, path, out var success);
                if (!success)
                    throw new InvalidOperationException($"[WiRR] Nie udało się zapisać prefabu: {path}");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(temporary);
            }

            return AssetDatabase.LoadAssetAtPath<GameObject>(path);
        }

        private static void CreateOne(int labNumber, Definition definition)
        {
            ValidateLabNumber(labNumber);
            WiRRSceneTools.PrepareBaseScene(labNumber);
            EnsureFolder(PrefabRoot);

            var prefab = SavePrefab($"{PrefabRoot}/{definition.Name}.prefab", definition.Builder);
            PlaceInScene(labNumber, prefab, definition.Position);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[WiRR] Dodano demonstrator XR {definition.Name}.");
        }

        private static void PlaceInScene(int labNumber, GameObject prefab, Vector3 localPosition)
        {
            if (prefab == null)
                throw new InvalidOperationException("[WiRR] Wygenerowany prefab XR nie jest dostępny.");

            var scene = SceneManager.GetActiveScene();
            if (!scene.IsValid())
                throw new InvalidOperationException("[WiRR] Brak aktywnej sceny.");

            var labRoot = FindInScene(scene, $"WiRR_Lab{labNumber:00}");
            if (labRoot == null)
                throw new InvalidOperationException("[WiRR] Brak głównego obiektu laboratorium.");

            var teaching = EnsureChild(labRoot.transform, TeachingRootName);
            var showcase = EnsureChild(teaching.transform, ShowcaseRootName);

            var existing = showcase.transform.Find(prefab.name);
            if (existing != null)
                Undo.DestroyObjectImmediate(existing.gameObject);

            var instance = PrefabUtility.InstantiatePrefab(prefab, scene) as GameObject;
            if (instance == null)
                throw new InvalidOperationException($"[WiRR] Nie udało się umieścić {prefab.name} w scenie.");

            Undo.RegisterCreatedObjectUndo(instance, $"Add {prefab.name}");
            instance.transform.SetParent(showcase.transform, false);
            instance.transform.localPosition = localPosition;
            EditorSceneManager.MarkSceneDirty(scene);
        }

        private static GameObject EnsureChild(Transform parent, string name)
        {
            var existing = parent.Find(name);
            if (existing != null)
                return existing.gameObject;

            var child = new GameObject(name);
            Undo.RegisterCreatedObjectUndo(child, $"Create {name}");
            child.transform.SetParent(parent, false);
            return child;
        }

        private static GameObject FindInScene(Scene scene, string name)
        {
            foreach (var root in scene.GetRootGameObjects())
            foreach (var transform in root.GetComponentsInChildren<Transform>(true))
                if (transform.name == name)
                    return transform.gameObject;
            return null;
        }

        private static void EnsureFolder(string assetPath)
        {
            if (string.IsNullOrWhiteSpace(assetPath) || AssetDatabase.IsValidFolder(assetPath))
                return;

            var parent = Path.GetDirectoryName(assetPath)?.Replace('\\', '/');
            var name = Path.GetFileName(assetPath);
            if (string.IsNullOrWhiteSpace(parent) || string.IsNullOrWhiteSpace(name))
                return;

            if (!AssetDatabase.IsValidFolder(parent))
                EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, name);
        }

        private static void ValidateLabNumber(int labNumber)
        {
            if (labNumber < 1 || labNumber > 7)
                throw new ArgumentOutOfRangeException(nameof(labNumber), "Numer laboratorium musi należeć do zakresu 1–7.");
        }
    }
}

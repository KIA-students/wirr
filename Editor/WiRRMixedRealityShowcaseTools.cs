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
    /// Generates five mixed-reality teaching prefabs for camera imagery, tracked
    /// hands, people awareness, depth/spatial surfaces and wall-anchored content.
    /// The prefabs have working fallback behaviour and provider-neutral runtime APIs.
    /// </summary>
    internal static class WiRRMixedRealityShowcaseTools
    {
        private const string TeachingRootName = "WiRR_TeachingAssets";
        private const string ShowcaseRootName = "MixedRealityShowcase";
        private const string PrefabRoot = "Assets/WiRR/Common/Prefabs/MixedReality";

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

        public static void CreateCameraWindow(int labNumber) =>
            CreateOne(labNumber, Find("MR_CameraWindow"));

        public static void CreateHandAura(int labNumber) =>
            CreateOne(labNumber, Find("MR_HandAura"));

        public static void CreatePeopleAwareness(int labNumber) =>
            CreateOne(labNumber, Find("MR_PeopleAwareness"));

        public static void CreateSpatialScanner(int labNumber) =>
            CreateOne(labNumber, Find("MR_SpatialSurfaceScanner"));

        public static void CreateWallPortal(int labNumber) =>
            CreateOne(labNumber, Find("MR_WallPortal"));

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
            Debug.Log($"[WiRR] Usunięto demonstratory Mixed Reality z Lab {labNumber:00}.");
        }

        private static IEnumerable<Definition> Definitions()
        {
            return new[]
            {
                new Definition("MR_CameraWindow", new Vector3(-3.1f, 1.35f, 4.0f), BuildCameraWindow),
                new Definition("MR_HandAura", new Vector3(-1.55f, 0f, 3.8f), BuildHandAura),
                new Definition("MR_PeopleAwareness", new Vector3(0f, 0f, 4.1f), BuildPeopleAwareness),
                new Definition("MR_SpatialSurfaceScanner", new Vector3(1.65f, 0f, 4.0f), BuildSpatialScanner),
                new Definition("MR_WallPortal", new Vector3(3.15f, 1.3f, 4.2f), BuildWallPortal)
            };
        }

        private static Definition Find(string name)
        {
            foreach (var definition in Definitions())
                if (definition.Name == name)
                    return definition;

            throw new ArgumentException($"Nieznany demonstrator MR: {name}", nameof(name));
        }

        private static GameObject BuildCameraWindow()
        {
            var root = new GameObject("MR_CameraWindow");
            var metal = WiRRSurfaceTextureLibrary.GetMaterial(WiRRSurfaceFamily.Metal044A);
            var fallback = WiRRPrefabTextureLibrary.GetMaterial(WiRRGridTextureSet.Grid1, WiRRGridMaterialRole.Panel);
            var accent = WiRRPrefabTextureLibrary.GetMaterial(WiRRGridTextureSet.Grid3, WiRRGridMaterialRole.Accent);

            Box(root.transform, "Frame_Left", new Vector3(-0.91f, 0f, 0f), new Vector3(0.10f, 1.10f, 0.08f), metal, false);
            Box(root.transform, "Frame_Right", new Vector3(0.91f, 0f, 0f), new Vector3(0.10f, 1.10f, 0.08f), metal, false);
            Box(root.transform, "Frame_Top", new Vector3(0f, 0.55f, 0f), new Vector3(1.92f, 0.09f, 0.08f), metal, false);
            Box(root.transform, "Frame_Bottom", new Vector3(0f, -0.55f, 0f), new Vector3(1.92f, 0.09f, 0.08f), metal, false);

            var screen = Primitive(
                root.transform,
                "RealCameraFeed",
                PrimitiveType.Quad,
                new Vector3(0f, 0f, 0.035f),
                new Vector3(1.72f, 0.96f, 1f),
                fallback,
                false);

            Box(root.transform, "Reticle_H", new Vector3(0f, 0f, -0.015f), new Vector3(0.34f, 0.012f, 0.015f), accent, false);
            Box(root.transform, "Reticle_V", new Vector3(0f, 0f, -0.015f), new Vector3(0.012f, 0.34f, 0.015f), accent, false);

            var mixer = root.AddComponent<WiRRCameraFeedMixer>();
            mixer.Configure(screen.GetComponent<Renderer>(), true);

            Note(root.transform, "PC_or_phone_WebCamTexture_starts_after_camera_permission");
            Note(root.transform, "Quest_passthrough_is_a_platform_layer_external_provider_may_call_SetExternalTexture");
            Note(root.transform, "Do_not_record_or_identify_people_without_a_clear_experimental_need_and_consent");
            return root;
        }

        private static GameObject BuildHandAura()
        {
            var root = new GameObject("MR_HandAura");
            var accent = WiRRPrefabTextureLibrary.GetMaterial(WiRRGridTextureSet.Grid2, WiRRGridMaterialRole.Accent);
            var metal = WiRRSurfaceTextureLibrary.GetMaterial(WiRRSurfaceFamily.Metal004);

            var wrist = Primitive(root.transform, "WristMarker", PrimitiveType.Sphere, Vector3.zero, Vector3.one * 0.075f, metal, false);
            var fingertips = new Transform[5];
            var lines = new LineRenderer[5];

            for (var i = 0; i < 5; i++)
            {
                var tip = Primitive(
                    root.transform,
                    $"Fingertip_{i + 1}",
                    PrimitiveType.Sphere,
                    Vector3.zero,
                    Vector3.one * (i < 2 ? 0.065f : 0.052f),
                    accent,
                    false);
                fingertips[i] = tip.transform;

                var lineObject = new GameObject($"FingerRay_{i + 1}");
                lineObject.transform.SetParent(root.transform, false);
                var line = lineObject.AddComponent<LineRenderer>();
                line.useWorldSpace = true;
                line.positionCount = 2;
                line.startWidth = 0.012f;
                line.endWidth = 0.005f;
                line.sharedMaterial = accent;
                lines[i] = line;
            }

            var pinchCore = Primitive(
                root.transform,
                "PinchCore",
                PrimitiveType.Sphere,
                Vector3.zero,
                Vector3.one * 0.04f,
                accent,
                false);

            var aura = root.AddComponent<WiRRHandAura>();
            aura.Configure(wrist.transform, fingertips, lines, pinchCore.transform);

            Note(root.transform, "Fallback_simulates_a_hand_near_the_head_in_Play_Mode");
            Note(root.transform, "Hand_provider_calls_SetHandPose_with_wrist_and_5_fingertips");
            Note(root.transform, "Pinch01_is_derived_from_thumb_index_distance");
            return root;
        }

        private static GameObject BuildPeopleAwareness()
        {
            var root = new GameObject("MR_PeopleAwareness");
            var accent = WiRRPrefabTextureLibrary.GetMaterial(WiRRGridTextureSet.Grid3, WiRRGridMaterialRole.Accent);
            var metal = WiRRSurfaceTextureLibrary.GetMaterial(WiRRSurfaceFamily.Metal044A);

            var markers = new Transform[4];
            for (var i = 0; i < markers.Length; i++)
            {
                var marker = new GameObject($"PersonSlot_{i + 1}");
                marker.transform.SetParent(root.transform, false);
                markers[i] = marker.transform;

                Primitive(marker.transform, "BodyProxy", PrimitiveType.Capsule, new Vector3(0f, 0.9f, 0f), new Vector3(0.34f, 0.72f, 0.34f), metal, false);
                Primitive(marker.transform, "HeadProxy", PrimitiveType.Sphere, new Vector3(0f, 1.75f, 0f), Vector3.one * 0.30f, accent, false);
                Primitive(marker.transform, "SafetyHalo", PrimitiveType.Cylinder, new Vector3(0f, 0.018f, 0f), new Vector3(0.85f, 0.018f, 0.85f), accent, false);
            }

            var awareness = root.AddComponent<WiRRPeopleAwareness>();
            awareness.Configure(markers);

            Note(root.transform, "Provider_sends_only_world_positions_via_SetPeople_or_SetPerson");
            Note(root.transform, "No_identity_face_name_or_biometric_data_are_required");
            Note(root.transform, "Fallback_simulates_two_moving_people_for_editor_demonstration");
            return root;
        }

        private static GameObject BuildSpatialScanner()
        {
            var root = new GameObject("MR_SpatialSurfaceScanner");
            var accent = WiRRPrefabTextureLibrary.GetMaterial(WiRRGridTextureSet.Grid1, WiRRGridMaterialRole.Accent);

            var markers = new Transform[64];
            for (var i = 0; i < markers.Length; i++)
            {
                var marker = Primitive(
                    root.transform,
                    $"SurfaceSample_{i + 1:00}",
                    PrimitiveType.Quad,
                    Vector3.zero,
                    Vector3.one * 0.035f,
                    accent,
                    false);
                marker.SetActive(false);
                markers[i] = marker.transform;
            }

            var scanner = root.AddComponent<WiRRSpatialSurfaceScanner>();
            scanner.Configure(markers, 6f);

            Note(root.transform, "Without_depth_API_the_prefab_scans_existing_scene_colliders");
            Note(root.transform, "Depth_or_spatial_mesh_provider_calls_SubmitSurfaceSample");
            Note(root.transform, "Samples_decay_so_the_visualization_shows_recent_environment_geometry");
            return root;
        }

        private static GameObject BuildWallPortal()
        {
            var root = new GameObject("MR_WallPortal");
            var metal = WiRRSurfaceTextureLibrary.GetMaterial(WiRRSurfaceFamily.Metal004);
            var accent = WiRRPrefabTextureLibrary.GetMaterial(WiRRGridTextureSet.Grid2, WiRRGridMaterialRole.Accent);
            var dark = WiRRSurfaceTextureLibrary.GetMaterial(WiRRSurfaceFamily.DiamondPlate);

            var content = new GameObject("WallAttachedContent");
            content.transform.SetParent(root.transform, false);

            Box(content.transform, "Frame_Left", new Vector3(-0.78f, 0f, 0f), new Vector3(0.10f, 1.36f, 0.10f), metal, false);
            Box(content.transform, "Frame_Right", new Vector3(0.78f, 0f, 0f), new Vector3(0.10f, 1.36f, 0.10f), metal, false);
            Box(content.transform, "Frame_Top", new Vector3(0f, 0.68f, 0f), new Vector3(1.66f, 0.10f, 0.10f), metal, false);
            Box(content.transform, "Frame_Bottom", new Vector3(0f, -0.68f, 0f), new Vector3(1.66f, 0.10f, 0.10f), metal, false);

            var layerNear = new GameObject("VirtualLayer_Near");
            layerNear.transform.SetParent(content.transform, false);
            layerNear.transform.localPosition = new Vector3(0f, 0f, -0.08f);
            for (var i = 0; i < 8; i++)
            {
                var a = i * Mathf.PI * 2f / 8f;
                Primitive(layerNear.transform, $"Near_{i + 1}", PrimitiveType.Sphere,
                    new Vector3(Mathf.Cos(a) * 0.56f, Mathf.Sin(a) * 0.46f, 0f),
                    Vector3.one * 0.075f, accent, false);
            }

            var layerMid = new GameObject("VirtualLayer_Mid");
            layerMid.transform.SetParent(content.transform, false);
            layerMid.transform.localPosition = new Vector3(0f, 0f, -0.28f);
            for (var i = -2; i <= 2; i++)
                Box(layerMid.transform, $"Mid_{i}", new Vector3(i * 0.22f, i % 2 == 0 ? 0.18f : -0.18f, 0f), new Vector3(0.10f, 0.10f, 0.10f), dark, false);

            var layerFar = new GameObject("VirtualLayer_Far");
            layerFar.transform.SetParent(content.transform, false);
            layerFar.transform.localPosition = new Vector3(0f, 0f, -0.52f);
            var core = Primitive(layerFar.transform, "FarCore", PrimitiveType.Sphere, Vector3.zero, Vector3.one * 0.42f, accent, false);
            var spin = core.AddComponent<WiRRLoopMotion>();
            spin.Configure(WiRRLoopMotionMode.Rotate, new Vector3(0.2f, 1f, 0.3f), 7f, 0f, true);

            var parallax = root.AddComponent<WiRRHeadParallax>();
            parallax.Configure(
                new[] { layerNear.transform, layerMid.transform, layerFar.transform },
                new[] { 0.82f, 0.42f, 0.16f },
                0.22f);

            var anchor = root.AddComponent<WiRRWallAnchor>();
            anchor.Configure(content.transform, new Vector2(1.7f, 1.45f));

            Note(root.transform, "Scene_understanding_provider_calls_SetWallPlane_center_normal_size");
            Note(root.transform, "Editor_fallback_TryAnchorFromViewerRay_uses_scene_colliders");
            Note(root.transform, "Wall_normal_should_point_into_the_room_toward_the_user_side");
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
            Debug.Log($"[WiRR] Dodano demonstrator Mixed Reality {definition.Name}.");
        }

        private static void PlaceInScene(int labNumber, GameObject prefab, Vector3 localPosition)
        {
            if (prefab == null)
                throw new InvalidOperationException("[WiRR] Wygenerowany prefab MR nie jest dostępny.");

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

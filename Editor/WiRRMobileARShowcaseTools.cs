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
    /// Generates five smartphone-oriented AR teaching prefabs. Runtime behaviours are
    /// provider-neutral and can receive AR Foundation data through small adapters.
    /// </summary>
    internal static class WiRRMobileARShowcaseTools
    {
        private const string TeachingRootName = "WiRR_TeachingAssets";
        private const string ShowcaseRootName = "MobileARShowcase";
        private const string PrefabRoot = "Assets/WiRR/Common/Prefabs/MobileAR";

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

        public static void CreateTapPlacement(int labNumber) =>
            CreateOne(labNumber, Find("AR_TapPlacement"));

        public static void CreateImagePortal(int labNumber) =>
            CreateOne(labNumber, Find("AR_ImageMarkerPortal"));

        public static void CreateRuler(int labNumber) =>
            CreateOne(labNumber, Find("AR_WorldRuler"));

        public static void CreateLightMatch(int labNumber) =>
            CreateOne(labNumber, Find("AR_LightMatchObject"));

        public static void CreateSurfacePainter(int labNumber) =>
            CreateOne(labNumber, Find("AR_SurfacePainter"));

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
            Debug.Log($"[WiRR] Usunięto demonstratory Mobile AR z Lab {labNumber:00}.");
        }

        private static IEnumerable<Definition> Definitions()
        {
            return new[]
            {
                new Definition("AR_TapPlacement", new Vector3(-3.2f, 0f, 5.4f), BuildTapPlacement),
                new Definition("AR_ImageMarkerPortal", new Vector3(-1.6f, 0.02f, 5.2f), BuildImagePortal),
                new Definition("AR_WorldRuler", new Vector3(0f, 0.02f, 5.2f), BuildRuler),
                new Definition("AR_LightMatchObject", new Vector3(1.65f, 0f, 5.25f), BuildLightMatch),
                new Definition("AR_SurfacePainter", new Vector3(3.25f, 0f, 5.2f), BuildSurfacePainter)
            };
        }

        private static Definition Find(string name)
        {
            foreach (var definition in Definitions())
                if (definition.Name == name)
                    return definition;

            throw new ArgumentException($"Nieznany demonstrator Mobile AR: {name}", nameof(name));
        }

        private static GameObject BuildTapPlacement()
        {
            var root = new GameObject("AR_TapPlacement");
            var accent = WiRRPrefabTextureLibrary.GetMaterial(WiRRGridTextureSet.Grid2, WiRRGridMaterialRole.Accent);
            var metal = WiRRSurfaceTextureLibrary.GetMaterial(WiRRSurfaceFamily.Metal044A);
            var fabric = WiRRSurfaceTextureLibrary.GetMaterial(WiRRSurfaceFamily.Fabric066);

            var reticle = new GameObject("PlacementReticle");
            reticle.transform.SetParent(root.transform, false);

            for (var i = 0; i < 4; i++)
            {
                var angle = i * 90f;
                var rad = angle * Mathf.Deg2Rad;
                var segment = Box(
                    reticle.transform,
                    $"Corner_{i + 1}",
                    new Vector3(Mathf.Cos(rad) * 0.16f, 0.006f, Mathf.Sin(rad) * 0.16f),
                    new Vector3(0.12f, 0.012f, 0.025f),
                    accent,
                    false);
                segment.transform.localRotation = Quaternion.Euler(0f, -angle, 0f);
            }

            var content = new GameObject("PlacedContent");
            content.transform.SetParent(root.transform, false);
            Box(content.transform, "Base", new Vector3(0f, 0.05f, 0f), new Vector3(0.42f, 0.10f, 0.42f), fabric, false);
            Primitive(content.transform, "Core", PrimitiveType.Sphere, new Vector3(0f, 0.34f, 0f), Vector3.one * 0.26f, accent, false);
            for (var i = 0; i < 3; i++)
            {
                var ring = Primitive(
                    content.transform,
                    $"Ring_{i + 1}",
                    PrimitiveType.Cylinder,
                    new Vector3(0f, 0.34f, 0f),
                    new Vector3(0.34f + i * 0.055f, 0.012f, 0.34f + i * 0.055f),
                    metal,
                    false);
                ring.transform.localRotation = Quaternion.Euler(90f + i * 22f, i * 35f, 0f);
            }

            var placement = root.AddComponent<WiRRMobileARTapPlacement>();
            placement.Configure(reticle.transform, content.transform);

            Note(root.transform, "Phone_AR_adapter_updates_SetSurfaceHit_from_screen_raycast");
            Note(root.transform, "Tap_confirms_preview_pose_into_PlacedContent");
            Note(root.transform, "Editor_fallback_uses_Physics_Raycast_on_scene_colliders");
            return root;
        }

        private static GameObject BuildImagePortal()
        {
            var root = new GameObject("AR_ImageMarkerPortal");
            var accent = WiRRPrefabTextureLibrary.GetMaterial(WiRRGridTextureSet.Grid3, WiRRGridMaterialRole.Accent);
            var dark = WiRRSurfaceTextureLibrary.GetMaterial(WiRRSurfaceFamily.DiamondPlate);
            var metal = WiRRSurfaceTextureLibrary.GetMaterial(WiRRSurfaceFamily.Metal004);

            var content = new GameObject("TrackedImageContent");
            content.transform.SetParent(root.transform, false);

            Box(content.transform, "MarkerOutline", new Vector3(0f, 0.006f, 0f), new Vector3(0.22f, 0.012f, 0.22f), dark, false);
            for (var i = 0; i < 8; i++)
            {
                var a = i * Mathf.PI * 2f / 8f;
                Primitive(
                    content.transform,
                    $"PortalNode_{i + 1}",
                    PrimitiveType.Sphere,
                    new Vector3(Mathf.Cos(a) * 0.16f, 0.11f + Mathf.Sin(a) * 0.16f, 0f),
                    Vector3.one * 0.035f,
                    accent,
                    false);
            }

            var core = Primitive(content.transform, "PortalCore", PrimitiveType.Sphere, new Vector3(0f, 0.12f, 0f), Vector3.one * 0.16f, accent, false);
            var spin = core.AddComponent<WiRRLoopMotion>();
            spin.Configure(WiRRLoopMotionMode.Rotate, new Vector3(0.2f, 1f, 0.3f), 8f, 0f, true);

            Box(content.transform, "InfoStem", new Vector3(0f, 0.29f, 0f), new Vector3(0.025f, 0.16f, 0.025f), metal, false);
            Box(content.transform, "InfoPlate", new Vector3(0f, 0.40f, 0f), new Vector3(0.26f, 0.09f, 0.025f), accent, false);

            var tracker = root.AddComponent<WiRRMobileARImageAnchor>();
            tracker.Configure(content.transform, 0.20f);

            Note(root.transform, "ARTrackedImage_adapter_calls_SetTrackedImagePose_name_pose_physicalSize");
            Note(root.transform, "Use_a_real_reference_image_library_on_the_phone");
            Note(root.transform, "Fallback_simulates_a_tracked_marker_in_the_Editor");
            return root;
        }

        private static GameObject BuildRuler()
        {
            var root = new GameObject("AR_WorldRuler");
            var accent = WiRRPrefabTextureLibrary.GetMaterial(WiRRGridTextureSet.Grid1, WiRRGridMaterialRole.Accent);
            var metal = WiRRSurfaceTextureLibrary.GetMaterial(WiRRSurfaceFamily.Metal004);

            var pointA = Primitive(root.transform, "Point_A", PrimitiveType.Sphere, Vector3.zero, Vector3.one * 0.055f, accent, false);
            var pointB = Primitive(root.transform, "Point_B", PrimitiveType.Sphere, Vector3.zero, Vector3.one * 0.055f, accent, false);
            var candidate = Primitive(root.transform, "Candidate", PrimitiveType.Cylinder, Vector3.zero, new Vector3(0.07f, 0.008f, 0.07f), metal, false);

            var lineObject = new GameObject("MeasurementLine");
            lineObject.transform.SetParent(root.transform, false);
            var line = lineObject.AddComponent<LineRenderer>();
            line.useWorldSpace = true;
            line.positionCount = 2;
            line.startWidth = 0.012f;
            line.endWidth = 0.012f;
            line.sharedMaterial = accent;
            line.enabled = false;

            var ticks = new Transform[30];
            for (var i = 0; i < ticks.Length; i++)
            {
                var tick = Box(
                    root.transform,
                    $"Tick_{i + 1:00}",
                    Vector3.zero,
                    new Vector3(0.008f, 0.055f, 0.008f),
                    i % 5 == 4 ? accent : metal,
                    false);
                tick.SetActive(false);
                ticks[i] = tick.transform;
            }

            var ruler = root.AddComponent<WiRRMobileARRuler>();
            ruler.Configure(pointA.transform, pointB.transform, candidate.transform, line, ticks);

            Note(root.transform, "First_tap_sets_A_second_tap_sets_B_third_tap_starts_new_measurement");
            Note(root.transform, "DistanceMeters_is_available_for_UI_or_report_export");
            Note(root.transform, "AR_adapter_updates_SetCandidatePoint_from_plane_or_depth_raycast");
            return root;
        }

        private static GameObject BuildLightMatch()
        {
            var root = new GameObject("AR_LightMatchObject");
            var concrete = WiRRSurfaceTextureLibrary.GetMaterial(WiRRSurfaceFamily.Concrete);
            var metal = WiRRSurfaceTextureLibrary.GetMaterial(WiRRSurfaceFamily.Metal044A);
            var fabric = WiRRSurfaceTextureLibrary.GetMaterial(WiRRSurfaceFamily.Fabric023);
            var accent = WiRRPrefabTextureLibrary.GetMaterial(WiRRGridTextureSet.Grid2, WiRRGridMaterialRole.Accent);

            Box(root.transform, "Pedestal", new Vector3(0f, 0.10f, 0f), new Vector3(0.82f, 0.20f, 0.82f), concrete, true);

            var sampleRoot = new GameObject("LightSensitiveSample");
            sampleRoot.transform.SetParent(root.transform, false);
            sampleRoot.transform.localPosition = new Vector3(0f, 0.52f, 0f);

            var renderers = new List<Renderer>();
            renderers.Add(Primitive(sampleRoot.transform, "MetalSphere", PrimitiveType.Sphere, new Vector3(-0.24f, 0.08f, 0f), Vector3.one * 0.28f, metal, false).GetComponent<Renderer>());
            renderers.Add(Primitive(sampleRoot.transform, "FabricCube", PrimitiveType.Cube, new Vector3(0.22f, 0.04f, 0f), Vector3.one * 0.28f, fabric, false).GetComponent<Renderer>());
            renderers.Add(Primitive(sampleRoot.transform, "AccentCapsule", PrimitiveType.Capsule, new Vector3(0f, 0.28f, 0f), new Vector3(0.15f, 0.24f, 0.15f), accent, false).GetComponent<Renderer>());

            var lightObject = new GameObject("EstimatedMainLight");
            lightObject.transform.SetParent(root.transform, false);
            lightObject.transform.localRotation = Quaternion.Euler(50f, -30f, 0f);
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1f;

            var matcher = root.AddComponent<WiRRMobileARLightMatch>();
            matcher.Configure(light, renderers.ToArray());

            Note(root.transform, "ARCameraManager_light_estimation_adapter_calls_SetLightEstimate");
            Note(root.transform, "Compare_virtual_object_before_and_after_real_light_matching");
            Note(root.transform, "Fallback_cycles_intensity_color_and_direction_in_Play_Mode");
            return root;
        }

        private static GameObject BuildSurfacePainter()
        {
            var root = new GameObject("AR_SurfacePainter");
            var accent = WiRRPrefabTextureLibrary.GetMaterial(WiRRGridTextureSet.Grid3, WiRRGridMaterialRole.Accent);

            var markers = new Transform[120];
            for (var i = 0; i < markers.Length; i++)
            {
                var marker = Primitive(
                    root.transform,
                    $"Paint_{i + 1:000}",
                    PrimitiveType.Quad,
                    Vector3.zero,
                    Vector3.one * 0.04f,
                    accent,
                    false);
                marker.SetActive(false);
                markers[i] = marker.transform;
            }

            var painter = root.AddComponent<WiRRMobileARSurfacePainter>();
            painter.Configure(markers);

            Note(root.transform, "Drag_a_finger_to_paint_virtual_marks_on_real_planes");
            Note(root.transform, "AR_adapter_updates_SetSurfaceHit_for_the_current_touch_position");
            Note(root.transform, "Pool_has_120_marks_and_wraps_without_runtime_instantiation");
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
            Debug.Log($"[WiRR] Dodano demonstrator Mobile AR {definition.Name}.");
        }

        private static void PlaceInScene(int labNumber, GameObject prefab, Vector3 localPosition)
        {
            if (prefab == null)
                throw new InvalidOperationException("[WiRR] Wygenerowany prefab Mobile AR nie jest dostępny.");

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

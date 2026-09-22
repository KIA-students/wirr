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
    /// Optional visual laboratories for teaching texture resolution, normal mapping,
    /// PBR channels, tiling and material diversity. They are intentionally separate
    /// from the mandatory lab assets.
    /// </summary>
    internal static class WiRROptionalPrefabTools
    {
        private const string TeachingRootName = "WiRR_TeachingAssets";
        private const string OptionalRootName = "OptionalMaterialDemos";
        private const string PrefabRoot = "Assets/WiRR/Common/Prefabs/Optional";

        private sealed class OptionalDefinition
        {
            public string Name { get; }
            public Vector3 Position { get; }
            public Func<GameObject> Builder { get; }

            public OptionalDefinition(string name, Vector3 position, Func<GameObject> builder)
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

        public static void CreateMaterialGallery(int labNumber) =>
            CreateOne(labNumber, Find("OPT_MaterialGallery"));

        public static void CreateResolutionWall(int labNumber) =>
            CreateOne(labNumber, Find("OPT_TextureResolutionWall"));

        public static void CreateNormalMapLab(int labNumber) =>
            CreateOne(labNumber, Find("OPT_NormalMapLab"));

        public static void CreatePbrChannelGallery(int labNumber) =>
            CreateOne(labNumber, Find("OPT_PBRChannelGallery"));

        public static void CreateTilingLab(int labNumber) =>
            CreateOne(labNumber, Find("OPT_TilingAndMipLab"));

        public static bool OptionalDemosExist(int labNumber)
        {
            if (labNumber < 1 || labNumber > 7)
                return false;

            var scene = SceneManager.GetActiveScene();
            if (!scene.IsValid())
                return false;

            var labRoot = FindInScene(scene, $"WiRR_Lab{labNumber:00}");
            var optional = labRoot?.transform.Find($"{TeachingRootName}/{OptionalRootName}");
            return optional != null && optional.childCount > 0;
        }

        public static void RemoveOptionalDemos(int labNumber)
        {
            ValidateLabNumber(labNumber);
            var scene = SceneManager.GetActiveScene();
            if (!scene.IsValid())
                return;

            var labRoot = FindInScene(scene, $"WiRR_Lab{labNumber:00}");
            var optional = labRoot?.transform.Find($"{TeachingRootName}/{OptionalRootName}");
            if (optional == null)
                return;

            Undo.DestroyObjectImmediate(optional.gameObject);
            EditorSceneManager.MarkSceneDirty(scene);
            Debug.Log($"[WiRR] Usunięto opcjonalne demonstratory materiałów z Lab {labNumber:00}.");
        }

        private static void CreateOne(int labNumber, OptionalDefinition definition)
        {
            ValidateLabNumber(labNumber);
            WiRRSceneTools.PrepareBaseScene(labNumber);
            EnsureFolder(PrefabRoot);

            var prefab = SavePrefab($"{PrefabRoot}/{definition.Name}.prefab", definition.Builder);
            PlaceInScene(labNumber, prefab, definition.Position);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[WiRR] Dodano demonstrator {definition.Name}.");
        }

        private static IEnumerable<OptionalDefinition> Definitions()
        {
            return new[]
            {
                new OptionalDefinition("OPT_MaterialGallery", new Vector3(0f, 0f, -2.6f), BuildMaterialGallery),
                new OptionalDefinition("OPT_TextureResolutionWall", new Vector3(0f, 0f, -1.3f), BuildResolutionWall),
                new OptionalDefinition("OPT_NormalMapLab", new Vector3(-1.7f, 0f, -0.2f), BuildNormalMapLab),
                new OptionalDefinition("OPT_PBRChannelGallery", new Vector3(1.6f, 0f, -0.2f), BuildPbrChannelGallery),
                new OptionalDefinition("OPT_TilingAndMipLab", new Vector3(0f, 0f, 1.0f), BuildTilingLab)
            };
        }

        private static OptionalDefinition Find(string name)
        {
            foreach (var definition in Definitions())
                if (definition.Name == name)
                    return definition;
            throw new ArgumentException($"Nieznany demonstrator: {name}", nameof(name));
        }

        private static GameObject BuildMaterialGallery()
        {
            var root = new GameObject("OPT_MaterialGallery");
            var families = new[]
            {
                WiRRSurfaceFamily.AcousticFoam,
                WiRRSurfaceFamily.Concrete,
                WiRRSurfaceFamily.DiamondPlate,
                WiRRSurfaceFamily.Fabric023,
                WiRRSurfaceFamily.Fabric066,
                WiRRSurfaceFamily.Metal004,
                WiRRSurfaceFamily.Metal044A,
                WiRRSurfaceFamily.PaintedWood,
                WiRRSurfaceFamily.Sign,
                WiRRSurfaceFamily.Grass001,
                WiRRSurfaceFamily.WoodFloor034,
                WiRRSurfaceFamily.WoodFloor040
            };

            var baseMaterial = WiRRSurfaceTextureLibrary.GetMaterial(
                WiRRSurfaceFamily.Concrete,
                WiRRSurfaceMaterialVariant.AlbedoOnly);

            Box(root.transform, "GalleryBase", new Vector3(0f, 0.04f, 0f), new Vector3(3.0f, 0.08f, 1.95f), baseMaterial, true);

            for (var i = 0; i < families.Length; i++)
            {
                var row = i / 4;
                var col = i % 4;
                var x = -1.05f + col * 0.70f;
                var z = -0.58f + row * 0.58f;
                var family = families[i];
                var material = WiRRSurfaceTextureLibrary.GetMaterial(family);

                Box(
                    root.transform,
                    $"Pedestal_{family}",
                    new Vector3(x, 0.13f, z),
                    new Vector3(0.42f, 0.18f, 0.42f),
                    baseMaterial,
                    true);

                var specimen = Primitive(
                    root.transform,
                    $"Surface_{family}",
                    i % 3 == 0 ? PrimitiveType.Sphere : i % 3 == 1 ? PrimitiveType.Cube : PrimitiveType.Cylinder,
                    new Vector3(x, 0.44f, z),
                    Vector3.one * 0.34f,
                    material,
                    false);

                if (specimen.GetComponent<MeshRenderer>() != null)
                    specimen.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            }

            Note(root.transform, "Compare_colour_microstructure_roughness_metalness_and_normal_response");
            return root;
        }

        private static GameObject BuildResolutionWall()
        {
            var root = new GameObject("OPT_TextureResolutionWall");
            var concrete = WiRRSurfaceTextureLibrary.GetMaterial(
                WiRRSurfaceFamily.Concrete,
                WiRRSurfaceMaterialVariant.AlbedoOnly);
            Box(root.transform, "Frame", new Vector3(0f, 0.82f, 0.08f), new Vector3(3.2f, 1.55f, 0.08f), concrete, true);

            var sizes = new[] { 128, 256, 512, 1024 };
            for (var i = 0; i < sizes.Length; i++)
            {
                var material = WiRRSurfaceTextureLibrary.GetResolutionMaterial(WiRRSurfaceFamily.Sign, sizes[i]);
                Box(
                    root.transform,
                    $"Texture_{sizes[i]}px",
                    new Vector3(-1.14f + i * 0.76f, 0.84f, -0.08f),
                    new Vector3(0.64f, 1.18f, 0.035f),
                    material,
                    false);
                Note(root.transform, $"Resolution_{sizes[i]}px_same_geometry_same_UV");
            }

            Note(root.transform, "Observe_close_range_then_move_back_until_differences_disappear");
            Note(root.transform, "Compare_visual_quality_against_texture_memory_cost");
            return root;
        }

        private static GameObject BuildNormalMapLab()
        {
            var root = new GameObject("OPT_NormalMapLab");
            var baseMaterial = WiRRSurfaceTextureLibrary.GetMaterial(
                WiRRSurfaceFamily.Concrete,
                WiRRSurfaceMaterialVariant.AlbedoOnly);
            Box(root.transform, "Bench", new Vector3(0f, 0.12f, 0f), new Vector3(2.0f, 0.20f, 0.82f), baseMaterial, true);

            var variants = new[]
            {
                WiRRSurfaceMaterialVariant.AlbedoOnly,
                WiRRSurfaceMaterialVariant.AlbedoNormal,
                WiRRSurfaceMaterialVariant.FullPbr
            };

            for (var i = 0; i < variants.Length; i++)
            {
                var material = WiRRSurfaceTextureLibrary.GetMaterial(WiRRSurfaceFamily.DiamondPlate, variants[i]);
                Primitive(
                    root.transform,
                    $"DiamondPlate_{variants[i]}",
                    PrimitiveType.Sphere,
                    new Vector3(-0.62f + i * 0.62f, 0.54f, 0f),
                    Vector3.one * 0.48f,
                    material,
                    false);
            }

            var lightObject = new GameObject("ComparisonLight_move_me");
            lightObject.transform.SetParent(root.transform, false);
            lightObject.transform.localPosition = new Vector3(-0.8f, 1.4f, -0.8f);
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Point;
            light.range = 4.5f;
            light.intensity = 3.0f;
            light.shadows = LightShadows.Soft;

            Note(root.transform, "Left_albedo_only");
            Note(root.transform, "Middle_albedo_plus_normal");
            Note(root.transform, "Right_full_PBR");
            Note(root.transform, "Move_the_light_to_observe_how_normal_map_changes_reflections_without_geometry");
            return root;
        }

        private static GameObject BuildPbrChannelGallery()
        {
            var root = new GameObject("OPT_PBRChannelGallery");
            var frame = WiRRSurfaceTextureLibrary.GetMaterial(
                WiRRSurfaceFamily.Concrete,
                WiRRSurfaceMaterialVariant.AlbedoOnly);
            Box(root.transform, "Frame", new Vector3(0f, 0.74f, 0.08f), new Vector3(2.65f, 1.45f, 0.08f), frame, true);

            var channels = new[]
            {
                WiRRSurfaceChannel.Color,
                WiRRSurfaceChannel.Normal,
                WiRRSurfaceChannel.Roughness,
                WiRRSurfaceChannel.AmbientOcclusion,
                WiRRSurfaceChannel.Metalness,
                WiRRSurfaceChannel.Displacement
            };

            for (var i = 0; i < channels.Length; i++)
            {
                var row = i / 3;
                var col = i % 3;
                var material = WiRRSurfaceTextureLibrary.GetChannelPreviewMaterial(
                    WiRRSurfaceFamily.DiamondPlate,
                    channels[i]);
                if (material == null)
                    continue;

                Box(
                    root.transform,
                    $"Channel_{channels[i]}",
                    new Vector3(-0.82f + col * 0.82f, 0.96f - row * 0.62f, -0.08f),
                    new Vector3(0.67f, 0.48f, 0.025f),
                    material,
                    false);
            }

            Note(root.transform, "All_panels_come_from_the_same_DiamondPlate_material");
            Note(root.transform, "Ask_what_information_each_map_encodes_and_where_it_enters_the_shader");
            return root;
        }

        private static GameObject BuildTilingLab()
        {
            var root = new GameObject("OPT_TilingAndMipLab");
            var concrete = WiRRSurfaceTextureLibrary.GetMaterial(
                WiRRSurfaceFamily.Concrete,
                WiRRSurfaceMaterialVariant.AlbedoOnly);

            Box(root.transform, "ObservationDeck", new Vector3(0f, 0.03f, 0.6f), new Vector3(3.2f, 0.06f, 2.7f), concrete, true);

            var tilings = new[] { 0.5f, 1f, 4f, 12f };
            for (var i = 0; i < tilings.Length; i++)
            {
                var material = WiRRSurfaceTextureLibrary.GetTiledMaterial(WiRRSurfaceFamily.WoodFloor034, tilings[i]);
                Box(
                    root.transform,
                    $"Wood_tiling_{tilings[i]:0.##}x",
                    new Vector3(-1.08f + i * 0.72f, 0.08f, 0.55f),
                    new Vector3(0.62f, 0.04f, 2.25f),
                    material,
                    false);
            }

            Note(root.transform, "Compare_texel_density_repetition_mipmaps_and_grazing_angle");
            Note(root.transform, "Walk_along_the_strips_and_observe_when_repetition_becomes_obvious");
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

        private static void PlaceInScene(int labNumber, GameObject prefab, Vector3 localPosition)
        {
            if (prefab == null)
                throw new InvalidOperationException("[WiRR] Wygenerowany prefab opcjonalny nie jest dostępny.");

            var scene = SceneManager.GetActiveScene();
            if (!scene.IsValid())
                throw new InvalidOperationException("[WiRR] Brak aktywnej sceny.");

            var labRoot = FindInScene(scene, $"WiRR_Lab{labNumber:00}");
            if (labRoot == null)
                throw new InvalidOperationException("[WiRR] Brak głównego obiektu laboratorium.");

            var teaching = EnsureChild(labRoot.transform, TeachingRootName);
            var optional = EnsureChild(teaching.transform, OptionalRootName);
            var existing = optional.transform.Find(prefab.name);
            if (existing != null)
                Undo.DestroyObjectImmediate(existing.gameObject);

            var instance = PrefabUtility.InstantiatePrefab(prefab, scene) as GameObject;
            if (instance == null)
                throw new InvalidOperationException($"[WiRR] Nie udało się umieścić {prefab.name} w scenie.");

            Undo.RegisterCreatedObjectUndo(instance, $"Add {prefab.name}");
            instance.transform.SetParent(optional.transform, false);
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

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace KIA.WiRR.Editor
{
    internal enum WiRRSurfaceFamily
    {
        AcousticFoam,
        Concrete,
        DiamondPlate,
        Fabric023,
        Fabric066,
        Metal004,
        Metal044A,
        PaintedWood,
        Sign,
        Grass001,
        WoodFloor034,
        WoodFloor040
    }

    internal enum WiRRSurfaceMaterialVariant
    {
        FullPbr,
        AlbedoNormal,
        AlbedoOnly
    }

    internal enum WiRRSurfaceChannel
    {
        Color,
        Normal,
        Roughness,
        AmbientOcclusion,
        Metalness,
        Displacement,
        Opacity
    }

    /// <summary>
    /// Builds editable URP materials from the curated surface library.
    /// Source textures remain read-only in the package. Working copies, packed maps
    /// and quality-comparison variants are generated under Assets/WiRR/Common.
    /// </summary>
    internal static class WiRRSurfaceTextureLibrary
    {
        private const string GeneratedTextureRoot = "Assets/WiRR/Common/Textures/Generated/Surfaces";
        private const string GeneratedQualityRoot = "Assets/WiRR/Common/Textures/Generated/Quality";
        private const string GeneratedMaterialRoot = "Assets/WiRR/Common/Materials/Generated/Surfaces";
        private const string GeneratedQualityMaterialRoot = "Assets/WiRR/Common/Materials/Generated/Quality";

        private sealed class Definition
        {
            public WiRRSurfaceFamily Family { get; }
            public string Key { get; }
            public string DevelopmentFolder { get; }
            public string Prefix { get; }
            public bool HasAo { get; }
            public bool HasMetalness { get; }
            public bool HasOpacity { get; }
            public float DefaultTiling { get; }

            public Definition(
                WiRRSurfaceFamily family,
                string key,
                string developmentFolder,
                string prefix,
                bool hasAo,
                bool hasMetalness,
                bool hasOpacity,
                float defaultTiling)
            {
                Family = family;
                Key = key;
                DevelopmentFolder = developmentFolder;
                Prefix = prefix;
                HasAo = hasAo;
                HasMetalness = hasMetalness;
                HasOpacity = hasOpacity;
                DefaultTiling = defaultTiling;
            }
        }

        private static readonly Dictionary<WiRRSurfaceFamily, Definition> Definitions = new()
        {
            { WiRRSurfaceFamily.AcousticFoam, new Definition(WiRRSurfaceFamily.AcousticFoam, "AcousticFoam", "AcousticFoam", "AcousticFoam003_1K-JPG", false, true, false, 2.0f) },
            { WiRRSurfaceFamily.Concrete, new Definition(WiRRSurfaceFamily.Concrete, "Concrete", "Concrete", "Concrete032_1K-JPG", true, false, false, 2.0f) },
            { WiRRSurfaceFamily.DiamondPlate, new Definition(WiRRSurfaceFamily.DiamondPlate, "DiamondPlate", "DiamondPlate", "DiamondPlate005D_1K-JPG", true, true, false, 2.0f) },
            { WiRRSurfaceFamily.Fabric023, new Definition(WiRRSurfaceFamily.Fabric023, "Fabric023", "Fabric", "Fabric023_1K-JPG", false, false, false, 3.0f) },
            { WiRRSurfaceFamily.Fabric066, new Definition(WiRRSurfaceFamily.Fabric066, "Fabric066", "Fabric_-66", "Fabric066_1K-JPG", true, false, false, 3.0f) },
            { WiRRSurfaceFamily.Metal004, new Definition(WiRRSurfaceFamily.Metal004, "Metal004", "Metal_004", "Metal004_1K-JPG", false, true, false, 2.0f) },
            { WiRRSurfaceFamily.Metal044A, new Definition(WiRRSurfaceFamily.Metal044A, "Metal044A", "metal", "Metal044A_1K-JPG", false, true, false, 2.0f) },
            { WiRRSurfaceFamily.PaintedWood, new Definition(WiRRSurfaceFamily.PaintedWood, "PaintedWood", "PaintedWood", "PaintedWood007A_1K-JPG", true, false, false, 2.0f) },
            { WiRRSurfaceFamily.Sign, new Definition(WiRRSurfaceFamily.Sign, "Sign", "Sign", "Sign002_1K-JPG", false, true, true, 1.0f) },
            { WiRRSurfaceFamily.Grass001, new Definition(WiRRSurfaceFamily.Grass001, "Grass001", "grass_001", "Grass001_1K-JPG", true, false, false, 2.5f) },
            { WiRRSurfaceFamily.WoodFloor034, new Definition(WiRRSurfaceFamily.WoodFloor034, "WoodFloor034", "wood_036", "WoodFloor034_1K-JPG", true, false, false, 2.0f) },
            { WiRRSurfaceFamily.WoodFloor040, new Definition(WiRRSurfaceFamily.WoodFloor040, "WoodFloor040", "wood_040", "WoodFloor040_1K-JPG", true, false, false, 2.0f) }
        };

        public static IEnumerable<WiRRSurfaceFamily> Families => Definitions.Keys;

        public static void RefreshAll()
        {
            EnsureFolders();
            foreach (var family in Families)
            {
                EnsureSurfaceSet(family, true);
                EnsureMaterial(family, WiRRSurfaceMaterialVariant.FullPbr, true);
                EnsureMaterial(family, WiRRSurfaceMaterialVariant.AlbedoNormal, true);
                EnsureMaterial(family, WiRRSurfaceMaterialVariant.AlbedoOnly, true);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[WiRR] Odświeżono bibliotekę materiałów powierzchniowych i warianty dydaktyczne.");
        }

        public static Material GetMaterial(
            WiRRSurfaceFamily family,
            WiRRSurfaceMaterialVariant variant = WiRRSurfaceMaterialVariant.FullPbr)
        {
            EnsureFolders();
            EnsureSurfaceSet(family, false);
            return EnsureMaterial(family, variant, false);
        }

        public static Material GetResolutionMaterial(WiRRSurfaceFamily family, int maxTextureSize)
        {
            if (maxTextureSize != 128 && maxTextureSize != 256 && maxTextureSize != 512 && maxTextureSize != 1024)
                throw new ArgumentOutOfRangeException(nameof(maxTextureSize), "Dozwolone rozdzielczości: 128, 256, 512, 1024.");

            EnsureFolders();
            var definition = Definitions[family];
            var source = ResolveSourcePath(definition, "Color");
            if (string.IsNullOrWhiteSpace(source))
                throw new InvalidOperationException($"[WiRR] Brak mapy Color dla {family}.");

            var folder = $"{GeneratedQualityRoot}/Resolution/{definition.Key}/{maxTextureSize}";
            EnsureFolder(folder);
            var extension = Path.GetExtension(source);
            var destination = $"{folder}/{definition.Key}_Color_{maxTextureSize}{extension}";

            if (AssetDatabase.LoadAssetAtPath<Texture2D>(destination) == null)
            {
                if (!AssetDatabase.CopyAsset(source, destination))
                    throw new InvalidOperationException($"[WiRR] Nie udało się utworzyć kopii tekstury {destination}.");
            }

            ConfigureImporter(destination, TextureRole.Color, maxTextureSize, false);
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(destination);

            var materialFolder = $"{GeneratedQualityMaterialRoot}/Resolution";
            EnsureFolder(materialFolder);
            var materialPath = $"{materialFolder}/{definition.Key}_{maxTextureSize}.mat";
            var material = EnsureLitMaterialAsset(materialPath, $"WiRR_{definition.Key}_{maxTextureSize}");
            ResetLitMaterial(material);
            SetTexture(material, "_BaseMap", "_MainTex", texture, definition.DefaultTiling);
            SetBaseColor(material, Color.white);
            SetSmoothness(material, 0.3f);
            EditorUtility.SetDirty(material);
            return material;
        }

        public static Material GetTiledMaterial(WiRRSurfaceFamily family, float tiling)
        {
            if (tiling <= 0f || tiling > 32f)
                throw new ArgumentOutOfRangeException(nameof(tiling));

            var definition = Definitions[family];
            var baseMaterial = GetMaterial(family, WiRRSurfaceMaterialVariant.FullPbr);
            var label = tiling.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture).Replace('.', '_');
            var folder = $"{GeneratedQualityMaterialRoot}/Tiling";
            EnsureFolder(folder);
            var path = $"{folder}/{definition.Key}_Tiling_{label}.mat";

            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                material = new Material(baseMaterial) { name = $"WiRR_{definition.Key}_Tiling_{label}" };
                AssetDatabase.CreateAsset(material, path);
            }
            else
            {
                material.CopyPropertiesFromMaterial(baseMaterial);
            }

            SetScaleIfPresent(material, "_BaseMap", tiling);
            SetScaleIfPresent(material, "_MainTex", tiling);
            SetScaleIfPresent(material, "_BumpMap", tiling);
            SetScaleIfPresent(material, "_OcclusionMap", tiling);
            SetScaleIfPresent(material, "_MetallicGlossMap", tiling);
            EditorUtility.SetDirty(material);
            return material;
        }

        public static Material GetChannelPreviewMaterial(WiRRSurfaceFamily family, WiRRSurfaceChannel channel)
        {
            EnsureFolders();
            var definition = Definitions[family];
            var role = ChannelRole(channel, definition);
            if (role == null)
                return null;

            var source = ResolveSourcePath(definition, role);
            if (string.IsNullOrWhiteSpace(source))
                return null;

            var folder = $"{GeneratedQualityRoot}/Channels/{definition.Key}";
            EnsureFolder(folder);
            var extension = Path.GetExtension(source);
            var destination = $"{folder}/{definition.Key}_{channel}{extension}";

            if (AssetDatabase.LoadAssetAtPath<Texture2D>(destination) == null)
            {
                if (!AssetDatabase.CopyAsset(source, destination))
                    return null;
            }

            ConfigureImporter(destination, channel == WiRRSurfaceChannel.Color ? TextureRole.Color : TextureRole.Data, 1024, false);
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(destination);

            var materialFolder = $"{GeneratedQualityMaterialRoot}/Channels";
            EnsureFolder(materialFolder);
            var materialPath = $"{materialFolder}/{definition.Key}_{channel}.mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
            if (material == null)
            {
                var shader = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Unlit/Texture");
                if (shader == null)
                    throw new InvalidOperationException("[WiRR] Nie znaleziono shadera Unlit do podglądu kanałów.");
                material = new Material(shader) { name = $"WiRR_{definition.Key}_{channel}" };
                AssetDatabase.CreateAsset(material, materialPath);
            }

            if (material.HasProperty("_BaseMap"))
                material.SetTexture("_BaseMap", texture);
            if (material.HasProperty("_MainTex"))
                material.SetTexture("_MainTex", texture);
            if (material.HasProperty("_BaseColor"))
                material.SetColor("_BaseColor", Color.white);
            material.color = Color.white;
            EditorUtility.SetDirty(material);
            return material;
        }

        private enum TextureRole
        {
            Color,
            Normal,
            Data
        }

        private static void EnsureSurfaceSet(WiRRSurfaceFamily family, bool force)
        {
            var definition = Definitions[family];
            var roles = new List<string> { "Color", "Displacement", "NormalGL", "Roughness" };
            if (definition.HasAo) roles.Add("AmbientOcclusion");
            if (definition.HasMetalness) roles.Add("Metalness");
            if (definition.HasOpacity) roles.Add("Opacity");

            var folder = $"{GeneratedTextureRoot}/{definition.Key}";
            EnsureFolder(folder);

            foreach (var role in roles)
            {
                var source = ResolveSourcePath(definition, role);
                if (string.IsNullOrWhiteSpace(source))
                {
                    Debug.LogWarning($"[WiRR] Brak mapy {role} dla materiału {family}.");
                    continue;
                }

                var extension = Path.GetExtension(source);
                var destination = $"{folder}/{definition.Key}_{role}{extension}";

                if (force && AssetDatabase.LoadAssetAtPath<Texture2D>(destination) != null)
                    AssetDatabase.DeleteAsset(destination);

                if (AssetDatabase.LoadAssetAtPath<Texture2D>(destination) == null &&
                    !AssetDatabase.CopyAsset(source, destination))
                {
                    Debug.LogWarning($"[WiRR] Nie udało się skopiować {source}.");
                    continue;
                }

                var textureRole = role == "Color"
                    ? TextureRole.Color
                    : role == "NormalGL"
                        ? TextureRole.Normal
                        : TextureRole.Data;
                ConfigureImporter(destination, textureRole, 2048, false);
            }

            EnsureMetallicSmoothness(definition, force);
        }

        private static Material EnsureMaterial(
            WiRRSurfaceFamily family,
            WiRRSurfaceMaterialVariant variant,
            bool force)
        {
            var definition = Definitions[family];
            var path = $"{GeneratedMaterialRoot}/{definition.Key}_{variant}.mat";

            if (force && AssetDatabase.LoadAssetAtPath<Material>(path) != null)
                AssetDatabase.DeleteAsset(path);

            var material = EnsureLitMaterialAsset(path, $"WiRR_{definition.Key}_{variant}");
            ResetLitMaterial(material);

            var color = LoadGenerated(definition, "Color");
            SetTexture(material, "_BaseMap", "_MainTex", color, definition.DefaultTiling);
            SetBaseColor(material, Color.white);

            if (variant != WiRRSurfaceMaterialVariant.AlbedoOnly)
            {
                var normal = LoadGenerated(definition, "NormalGL");
                if (normal != null && material.HasProperty("_BumpMap"))
                {
                    material.SetTexture("_BumpMap", normal);
                    material.SetTextureScale("_BumpMap", Vector2.one * definition.DefaultTiling);
                    if (material.HasProperty("_BumpScale"))
                        material.SetFloat("_BumpScale", 1f);
                    material.EnableKeyword("_NORMALMAP");
                }
            }

            if (variant == WiRRSurfaceMaterialVariant.FullPbr)
            {
                var ao = LoadGenerated(definition, "AmbientOcclusion");
                if (ao != null && material.HasProperty("_OcclusionMap"))
                {
                    material.SetTexture("_OcclusionMap", ao);
                    material.SetTextureScale("_OcclusionMap", Vector2.one * definition.DefaultTiling);
                    if (material.HasProperty("_OcclusionStrength"))
                        material.SetFloat("_OcclusionStrength", 1f);
                }

                var metallicSmoothness = AssetDatabase.LoadAssetAtPath<Texture2D>(
                    $"{GeneratedTextureRoot}/{definition.Key}/{definition.Key}_MetallicSmoothness.png");
                if (metallicSmoothness != null && material.HasProperty("_MetallicGlossMap"))
                {
                    material.SetTexture("_MetallicGlossMap", metallicSmoothness);
                    material.SetTextureScale("_MetallicGlossMap", Vector2.one * definition.DefaultTiling);
                    material.EnableKeyword("_METALLICSPECGLOSSMAP");
                    if (material.HasProperty("_Metallic"))
                        material.SetFloat("_Metallic", 1f);
                    SetSmoothness(material, 1f);
                }
                else
                {
                    if (material.HasProperty("_Metallic"))
                        material.SetFloat("_Metallic", definition.HasMetalness ? 0.65f : 0f);
                    SetSmoothness(material, 0.35f);
                }
            }
            else
            {
                if (material.HasProperty("_Metallic"))
                    material.SetFloat("_Metallic", 0f);
                SetSmoothness(material, variant == WiRRSurfaceMaterialVariant.AlbedoNormal ? 0.28f : 0.2f);
            }

            EditorUtility.SetDirty(material);
            return material;
        }

        private static void EnsureMetallicSmoothness(Definition definition, bool force)
        {
            var roughnessPath = GeneratedMapPath(definition, "Roughness");
            var metalnessPath = GeneratedMapPath(definition, "Metalness");
            var roughness = AssetDatabase.LoadAssetAtPath<Texture2D>(roughnessPath);
            var metalness = AssetDatabase.LoadAssetAtPath<Texture2D>(metalnessPath);
            if (roughness == null && metalness == null)
                return;

            var outputPath = $"{GeneratedTextureRoot}/{definition.Key}/{definition.Key}_MetallicSmoothness.png";
            if (!force && AssetDatabase.LoadAssetAtPath<Texture2D>(outputPath) != null)
                return;

            if (roughness != null) SetReadable(roughnessPath, true);
            if (metalness != null) SetReadable(metalnessPath, true);
            roughness = AssetDatabase.LoadAssetAtPath<Texture2D>(roughnessPath);
            metalness = AssetDatabase.LoadAssetAtPath<Texture2D>(metalnessPath);

            var reference = roughness != null ? roughness : metalness;
            if (reference == null)
                return;

            var width = reference.width;
            var height = reference.height;
            var roughPixels = roughness != null ? roughness.GetPixels32() : null;
            var metalPixels = metalness != null ? metalness.GetPixels32() : null;
            var output = new Color32[width * height];

            for (var i = 0; i < output.Length; i++)
            {
                var metallic = metalPixels != null && i < metalPixels.Length ? metalPixels[i].r : (byte)0;
                var rough = roughPixels != null && i < roughPixels.Length ? roughPixels[i].r : (byte)160;
                output[i] = new Color32(metallic, 0, 0, (byte)(255 - rough));
            }

            var packed = new Texture2D(width, height, TextureFormat.RGBA32, false, true);
            packed.SetPixels32(output);
            packed.Apply(false, false);

            var relative = outputPath.Substring("Assets/".Length).Replace('/', Path.DirectorySeparatorChar);
            var absolute = Path.Combine(Application.dataPath, relative);
            Directory.CreateDirectory(Path.GetDirectoryName(absolute) ?? Application.dataPath);
            File.WriteAllBytes(absolute, packed.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(packed);
            AssetDatabase.ImportAsset(outputPath, ImportAssetOptions.ForceSynchronousImport);
            ConfigureImporter(outputPath, TextureRole.Data, 2048, false);

            if (roughness != null) SetReadable(roughnessPath, false);
            if (metalness != null) SetReadable(metalnessPath, false);
        }

        private static void SetReadable(string assetPath, bool readable)
        {
            if (AssetImporter.GetAtPath(assetPath) is not TextureImporter importer || importer.isReadable == readable)
                return;
            importer.isReadable = readable;
            importer.SaveAndReimport();
        }

        private static void ConfigureImporter(
            string assetPath,
            TextureRole role,
            int maxTextureSize,
            bool readable)
        {
            if (AssetImporter.GetAtPath(assetPath) is not TextureImporter importer)
                return;

            var changed = false;
            var desiredType = role == TextureRole.Normal ? TextureImporterType.NormalMap : TextureImporterType.Default;
            if (importer.textureType != desiredType)
            {
                importer.textureType = desiredType;
                changed = true;
            }

            var desiredSrgb = role == TextureRole.Color;
            if (importer.sRGBTexture != desiredSrgb)
            {
                importer.sRGBTexture = desiredSrgb;
                changed = true;
            }

            if (importer.isReadable != readable)
            {
                importer.isReadable = readable;
                changed = true;
            }

            if (importer.maxTextureSize != maxTextureSize)
            {
                importer.maxTextureSize = maxTextureSize;
                changed = true;
            }

            if (importer.wrapMode != TextureWrapMode.Repeat)
            {
                importer.wrapMode = TextureWrapMode.Repeat;
                changed = true;
            }

            if (importer.filterMode != FilterMode.Trilinear)
            {
                importer.filterMode = FilterMode.Trilinear;
                changed = true;
            }

            if (!importer.mipmapEnabled)
            {
                importer.mipmapEnabled = true;
                changed = true;
            }

            if (role == TextureRole.Normal && importer.flipGreenChannel)
            {
                // NormalGL is Y+ and matches Unity's expected convention.
                importer.flipGreenChannel = false;
                changed = true;
            }

            if (changed)
                importer.SaveAndReimport();
        }

        private static string ResolveSourcePath(Definition definition, string role)
        {
            var fileName = $"{definition.Prefix}_{role}.jpg";
            var developmentPath = $"Assets/Textures/{definition.DevelopmentFolder}/{fileName}";
            if (AssetDatabase.LoadAssetAtPath<Texture2D>(developmentPath) != null)
                return developmentPath;

            var package = UnityEditor.PackageManager.PackageInfo.FindForAssembly(typeof(WiRRSurfaceTextureLibrary).Assembly);
            if (package != null && !string.IsNullOrWhiteSpace(package.assetPath))
            {
                var packagePath = $"{package.assetPath}/Textures/Surfaces/{definition.Key}/{fileName}";
                if (AssetDatabase.LoadAssetAtPath<Texture2D>(packagePath) != null)
                    return packagePath;
            }

            return null;
        }

        private static string GeneratedMapPath(Definition definition, string role)
        {
            var source = ResolveSourcePath(definition, role);
            var extension = string.IsNullOrWhiteSpace(source) ? ".jpg" : Path.GetExtension(source);
            return $"{GeneratedTextureRoot}/{definition.Key}/{definition.Key}_{role}{extension}";
        }

        private static Texture2D LoadGenerated(Definition definition, string role)
        {
            return AssetDatabase.LoadAssetAtPath<Texture2D>(GeneratedMapPath(definition, role));
        }

        private static string ChannelRole(WiRRSurfaceChannel channel, Definition definition)
        {
            return channel switch
            {
                WiRRSurfaceChannel.Color => "Color",
                WiRRSurfaceChannel.Normal => "NormalGL",
                WiRRSurfaceChannel.Roughness => "Roughness",
                WiRRSurfaceChannel.AmbientOcclusion => definition.HasAo ? "AmbientOcclusion" : null,
                WiRRSurfaceChannel.Metalness => definition.HasMetalness ? "Metalness" : null,
                WiRRSurfaceChannel.Displacement => "Displacement",
                WiRRSurfaceChannel.Opacity => definition.HasOpacity ? "Opacity" : null,
                _ => null
            };
        }

        private static Material EnsureLitMaterialAsset(string path, string name)
        {
            EnsureFolder(Path.GetDirectoryName(path)?.Replace('\\', '/'));
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material != null)
                return material;

            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            if (shader == null)
                throw new InvalidOperationException("[WiRR] Nie znaleziono shadera URP/Lit ani Standard.");

            material = new Material(shader) { name = name };
            AssetDatabase.CreateAsset(material, path);
            return material;
        }

        private static void ResetLitMaterial(Material material)
        {
            material.DisableKeyword("_NORMALMAP");
            material.DisableKeyword("_METALLICSPECGLOSSMAP");
            if (material.HasProperty("_BumpMap")) material.SetTexture("_BumpMap", null);
            if (material.HasProperty("_OcclusionMap")) material.SetTexture("_OcclusionMap", null);
            if (material.HasProperty("_MetallicGlossMap")) material.SetTexture("_MetallicGlossMap", null);
        }

        private static void SetBaseColor(Material material, Color color)
        {
            if (material.HasProperty("_BaseColor"))
                material.SetColor("_BaseColor", color);
            material.color = color;
        }

        private static void SetSmoothness(Material material, float value)
        {
            if (material.HasProperty("_Smoothness"))
                material.SetFloat("_Smoothness", value);
            if (material.HasProperty("_Glossiness"))
                material.SetFloat("_Glossiness", value);
        }

        private static void SetTexture(Material material, string primary, string fallback, Texture texture, float tiling)
        {
            if (texture == null)
                return;
            if (material.HasProperty(primary))
            {
                material.SetTexture(primary, texture);
                material.SetTextureScale(primary, Vector2.one * tiling);
            }
            if (material.HasProperty(fallback))
            {
                material.SetTexture(fallback, texture);
                material.SetTextureScale(fallback, Vector2.one * tiling);
            }
        }

        private static void SetScaleIfPresent(Material material, string property, float tiling)
        {
            if (material.HasProperty(property) && material.GetTexture(property) != null)
                material.SetTextureScale(property, Vector2.one * tiling);
        }

        private static void EnsureFolders()
        {
            EnsureFolder("Assets/WiRR");
            EnsureFolder("Assets/WiRR/Common");
            EnsureFolder("Assets/WiRR/Common/Textures");
            EnsureFolder("Assets/WiRR/Common/Textures/Generated");
            EnsureFolder(GeneratedTextureRoot);
            EnsureFolder(GeneratedQualityRoot);
            EnsureFolder("Assets/WiRR/Common/Materials");
            EnsureFolder("Assets/WiRR/Common/Materials/Generated");
            EnsureFolder(GeneratedMaterialRoot);
            EnsureFolder(GeneratedQualityMaterialRoot);
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
    }
}

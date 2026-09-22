using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace KIA.WiRR.Editor
{
    internal enum WiRRGridTextureSet
    {
        Grid1 = 1,
        Grid2 = 2,
        Grid3 = 3
    }

    internal enum WiRRGridMaterialRole
    {
        Floor,
        Wall,
        Panel,
        Accent
    }

    /// <summary>
    /// Resolves the grid texture sets from the development project's Assets/Textures
    /// and falls back to the package-visible Textures/Grid directory for UPM installs.
    /// Texture maps are copied into the student's Assets folder before materials are
    /// generated, so importer settings can be configured safely.
    /// </summary>
    internal static class WiRRPrefabTextureLibrary
    {
        private const string GeneratedTextureRoot = "Assets/WiRR/Common/Textures/Generated";
        private const string GeneratedMaterialRoot = "Assets/WiRR/Common/Materials/Generated/Grid";

        private static readonly string[] MapSuffixes =
        {
            "albedo",
            "ambient",
            "displacement",
            "normal",
            "specular"
        };

        public static void RefreshAll()
        {
            EnsureFolders();
            foreach (WiRRGridTextureSet set in Enum.GetValues(typeof(WiRRGridTextureSet)))
                EnsureTextureSet(set, true);

            foreach (WiRRGridTextureSet set in Enum.GetValues(typeof(WiRRGridTextureSet)))
            foreach (WiRRGridMaterialRole role in Enum.GetValues(typeof(WiRRGridMaterialRole)))
                EnsureMaterial(set, role, true);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[WiRR] Odświeżono tekstury grid i materiały prefabów.");
        }

        public static Material GetMaterial(WiRRGridTextureSet set, WiRRGridMaterialRole role)
        {
            EnsureFolders();
            EnsureTextureSet(set, false);
            return EnsureMaterial(set, role, false);
        }

        public static string DescribeSource(WiRRGridTextureSet set)
        {
            return set switch
            {
                WiRRGridTextureSet.Grid1 => "Assets/Textures/grid_1_* (fallback: grid-1_*)",
                WiRRGridTextureSet.Grid2 => "Assets/Textures/grid_2_*",
                WiRRGridTextureSet.Grid3 => "Assets/Textures/grid_3_* (fallback: grid-4_*)",
                _ => throw new ArgumentOutOfRangeException(nameof(set))
            };
        }

        private static void EnsureTextureSet(WiRRGridTextureSet set, bool force)
        {
            foreach (var suffix in MapSuffixes)
            {
                var destinationFileName = $"{CanonicalPrefix(set)}_{suffix}.png";
                var destination = $"{GeneratedTextureRoot}/{destinationFileName}";

                if (force && AssetDatabase.LoadAssetAtPath<Texture2D>(destination) != null)
                    AssetDatabase.DeleteAsset(destination);

                if (AssetDatabase.LoadAssetAtPath<Texture2D>(destination) == null)
                {
                    var source = ResolveSourcePath(set, suffix);
                    if (string.IsNullOrWhiteSpace(source))
                    {
                        Debug.LogWarning($"[WiRR] Nie znaleziono mapy {CanonicalPrefix(set)}_{suffix}.png. Sprawdź Assets/Textures lub katalog Textures/Grid pakietu.");
                        continue;
                    }

                    if (!AssetDatabase.CopyAsset(source, destination))
                    {
                        Debug.LogWarning($"[WiRR] Nie udało się skopiować tekstury {source} do {destination}.");
                        continue;
                    }
                }

                ConfigureImporter(destination, suffix);
            }
        }

        private static string ResolveSourcePath(WiRRGridTextureSet set, string suffix)
        {
            // Development project: MatPomGit/wirr/Project~/Assets/Textures becomes Assets/Textures.
            foreach (var prefix in SourcePrefixes(set))
            {
                var fileName = $"{prefix}_{suffix}.png";
                var developmentPath = $"Assets/Textures/{fileName}";
                if (AssetDatabase.LoadAssetAtPath<Texture2D>(developmentPath) != null)
                    return developmentPath;
            }

            var package = UnityEditor.PackageManager.PackageInfo.FindForAssembly(typeof(WiRRPrefabTextureLibrary).Assembly);
            if (package != null && !string.IsNullOrWhiteSpace(package.assetPath))
            {
                foreach (var prefix in SourcePrefixes(set))
                {
                    var fileName = $"{prefix}_{suffix}.png";
                    var packagePath = $"{package.assetPath}/Textures/Grid/{fileName}";
                    if (AssetDatabase.LoadAssetAtPath<Texture2D>(packagePath) != null)
                        return packagePath;
                }
            }

            return null;
        }

        private static void ConfigureImporter(string assetPath, string suffix)
        {
            if (AssetImporter.GetAtPath(assetPath) is not TextureImporter importer)
                return;

            var isNormal = string.Equals(suffix, "normal", StringComparison.OrdinalIgnoreCase);
            var isColor = string.Equals(suffix, "albedo", StringComparison.OrdinalIgnoreCase);

            var changed = false;
            var desiredType = isNormal ? TextureImporterType.NormalMap : TextureImporterType.Default;
            if (importer.textureType != desiredType)
            {
                importer.textureType = desiredType;
                changed = true;
            }

            if (importer.sRGBTexture != isColor)
            {
                importer.sRGBTexture = isColor;
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

            if (importer.maxTextureSize > 2048)
            {
                importer.maxTextureSize = 2048;
                changed = true;
            }

            if (changed)
                importer.SaveAndReimport();
        }

        private static Material EnsureMaterial(
            WiRRGridTextureSet set,
            WiRRGridMaterialRole role,
            bool force)
        {
            var path = $"{GeneratedMaterialRoot}/WiRR_{set}_{role}.mat";
            if (force && AssetDatabase.LoadAssetAtPath<Material>(path) != null)
                AssetDatabase.DeleteAsset(path);

            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
                if (shader == null)
                    throw new InvalidOperationException("[WiRR] Nie znaleziono shadera URP/Lit ani Standard.");

                material = new Material(shader)
                {
                    name = $"WiRR_{set}_{role}"
                };
                AssetDatabase.CreateAsset(material, path);
            }

            ConfigureMaterial(material, set, role);
            EditorUtility.SetDirty(material);
            return material;
        }

        private static void ConfigureMaterial(
            Material material,
            WiRRGridTextureSet set,
            WiRRGridMaterialRole role)
        {
            var albedo = LoadGenerated(set, "albedo");
            var ambient = LoadGenerated(set, "ambient");
            var normal = LoadGenerated(set, "normal");
            var specular = LoadGenerated(set, "specular");

            var tint = RoleTint(role);
            var tiling = RoleTiling(role);

            if (material.HasProperty("_BaseColor"))
                material.SetColor("_BaseColor", tint);
            material.color = tint;

            SetTexture(material, "_BaseMap", "_MainTex", albedo, tiling);

            if (normal != null && material.HasProperty("_BumpMap"))
            {
                material.SetTexture("_BumpMap", normal);
                material.SetTextureScale("_BumpMap", Vector2.one * tiling);
                if (material.HasProperty("_BumpScale"))
                    material.SetFloat("_BumpScale", role == WiRRGridMaterialRole.Panel ? 0.55f : 0.35f);
                material.EnableKeyword("_NORMALMAP");
            }

            if (ambient != null && material.HasProperty("_OcclusionMap"))
            {
                material.SetTexture("_OcclusionMap", ambient);
                material.SetTextureScale("_OcclusionMap", Vector2.one * tiling);
                if (material.HasProperty("_OcclusionStrength"))
                    material.SetFloat("_OcclusionStrength", 0.85f);
            }

            if (specular != null && material.HasProperty("_SpecGlossMap"))
            {
                if (material.HasProperty("_WorkflowMode"))
                    material.SetFloat("_WorkflowMode", 0f);
                material.SetTexture("_SpecGlossMap", specular);
                material.SetTextureScale("_SpecGlossMap", Vector2.one * tiling);
                material.EnableKeyword("_SPECGLOSSMAP");
            }

            if (material.HasProperty("_Smoothness"))
                material.SetFloat("_Smoothness", role == WiRRGridMaterialRole.Panel ? 0.45f : 0.28f);
            if (material.HasProperty("_Metallic"))
                material.SetFloat("_Metallic", role == WiRRGridMaterialRole.Accent ? 0.15f : 0.02f);
        }

        private static void SetTexture(
            Material material,
            string urpProperty,
            string legacyProperty,
            Texture texture,
            float tiling)
        {
            if (texture == null)
                return;

            if (material.HasProperty(urpProperty))
            {
                material.SetTexture(urpProperty, texture);
                material.SetTextureScale(urpProperty, Vector2.one * tiling);
            }

            if (material.HasProperty(legacyProperty))
            {
                material.SetTexture(legacyProperty, texture);
                material.SetTextureScale(legacyProperty, Vector2.one * tiling);
            }
        }

        private static Texture2D LoadGenerated(WiRRGridTextureSet set, string suffix)
        {
            return AssetDatabase.LoadAssetAtPath<Texture2D>(
                $"{GeneratedTextureRoot}/{CanonicalPrefix(set)}_{suffix}.png");
        }

        private static string CanonicalPrefix(WiRRGridTextureSet set)
        {
            return set switch
            {
                WiRRGridTextureSet.Grid1 => "grid_1",
                WiRRGridTextureSet.Grid2 => "grid_2",
                WiRRGridTextureSet.Grid3 => "grid_3",
                _ => throw new ArgumentOutOfRangeException(nameof(set))
            };
        }

        private static string[] SourcePrefixes(WiRRGridTextureSet set)
        {
            return set switch
            {
                WiRRGridTextureSet.Grid1 => new[] { "grid_1", "grid-1" },
                WiRRGridTextureSet.Grid2 => new[] { "grid_2" },
                WiRRGridTextureSet.Grid3 => new[] { "grid_3", "grid-4" },
                _ => throw new ArgumentOutOfRangeException(nameof(set))
            };
        }

        private static float RoleTiling(WiRRGridMaterialRole role)
        {
            return role switch
            {
                WiRRGridMaterialRole.Floor => 5.0f,
                WiRRGridMaterialRole.Wall => 3.0f,
                WiRRGridMaterialRole.Panel => 1.5f,
                WiRRGridMaterialRole.Accent => 1.0f,
                _ => 1.0f
            };
        }

        private static Color RoleTint(WiRRGridMaterialRole role)
        {
            return role switch
            {
                WiRRGridMaterialRole.Floor => new Color(0.78f, 0.80f, 0.84f),
                WiRRGridMaterialRole.Wall => new Color(0.86f, 0.88f, 0.91f),
                WiRRGridMaterialRole.Panel => Color.white,
                WiRRGridMaterialRole.Accent => new Color(0.78f, 0.92f, 1.0f),
                _ => Color.white
            };
        }

        private static void EnsureFolders()
        {
            EnsureFolder("Assets/WiRR");
            EnsureFolder("Assets/WiRR/Common");
            EnsureFolder("Assets/WiRR/Common/Textures");
            EnsureFolder(GeneratedTextureRoot);
            EnsureFolder("Assets/WiRR/Common/Materials");
            EnsureFolder("Assets/WiRR/Common/Materials/Generated");
            EnsureFolder(GeneratedMaterialRoot);
        }

        private static void EnsureFolder(string assetPath)
        {
            if (AssetDatabase.IsValidFolder(assetPath))
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

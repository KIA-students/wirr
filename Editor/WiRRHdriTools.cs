using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace KIA.WiRR.Editor
{
    internal sealed class WiRRHdriDefinition
    {
        public string Name { get; }
        public string RelativePath { get; }
        public string SuggestedUse { get; }

        public WiRRHdriDefinition(string name, string relativePath, string suggestedUse)
        {
            Name = name;
            RelativePath = relativePath;
            SuggestedUse = suggestedUse;
        }
    }

    internal static class WiRRHdriTools
    {
        private const string PackageRoot = "Packages/pl.prz.kia.wirr/Textures/HDRI";
        private const string MaterialRoot = "Assets/WiRR/Common/Skyboxes";

        public static readonly WiRRHdriDefinition[] Environments =
        {
            new("Amsterdam — środowisko miejskie", "Amsterdam/DayEnvironmentHDRI101_2K_HDR.exr",
                "Sceny miejskie, test czytelności interfejsu na złożonym tle, odbicia na metalach i szkle."),
            new("Clean Horizon — czysty horyzont", "CleanHorizon/DaySkyHDRI006B_2K_HDR.exr",
                "Neutralne testy oświetlenia, benchmarki i porównania materiałów, gdy tło ma możliwie mało rozpraszać."),
            new("Day Sky — jasne niebo", "DaySky/DaySkyHDRI046A_2K_HDR.exr",
                "Sceny zewnętrzne w dzień, kontrola ekspozycji, cieni i kontrastu obiektów."),
            new("Evening Environment — wieczór", "EveningEnvironment/EveningEnvironmentHDRI003_2K_HDR.exr",
                "Testy scen o niższym kontraście, percepcji głębi i czytelności UI przy cieplejszym świetle."),
            new("Forrest — las", "Forrest/DayEnvironmentHDRI078_2K_HDR.exr",
                "Naturalne otoczenie, testy materiałów organicznych, AR/VR z bogatym wizualnie tłem."),
            new("Indoor Environment — wnętrze", "IndoorEnvironment/IndoorEnvironmentHDRI012_2K_HDR.exr",
                "Sceny laboratoryjne i wnętrza, test odbić oraz zachowania materiałów w oświetleniu pośrednim."),
            new("Near Lake — okolice jeziora", "NearLake/DayEnvironmentHDRI096_2K_HDR.exr",
                "Testy odbić, powierzchni metalicznych i scen z jasnym niebem oraz ciemniejszym terenem."),
            new("Night Sky — noc", "NightSky/NightSkyHDRI008_2K_HDR.exr",
                "Sceny nocne, test emisji, źródeł światła, kontrastu i dostępności interfejsu w ciemnym otoczeniu."),
            new("Tower — otwarty krajobraz", "Tower/DaySkyHDRI039A_2K_HDR.exr",
                "Neutralne sceny zewnętrzne, ocena skali, sylwetki obiektów i widoczności na dalekim tle.")
        };

        public static bool ApplySkybox(int index)
        {
            if (index < 0 || index >= Environments.Length)
                return false;

            var definition = Environments[index];
            var texturePath = $"{PackageRoot}/{definition.RelativePath}";
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
            if (texture == null)
            {
                Debug.LogError($"[WiRR] Nie znaleziono HDRI: {texturePath}");
                return false;
            }

            EnsureFolder("Assets", "WiRR");
            EnsureFolder("Assets/WiRR", "Common");
            EnsureFolder("Assets/WiRR/Common", "Skyboxes");

            var safeName = definition.Name.Split('—')[0].Trim().Replace(" ", "_");
            var materialPath = $"{MaterialRoot}/WiRR_Skybox_{safeName}.mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
            if (material == null)
            {
                var shader = Shader.Find("Skybox/Panoramic");
                if (shader == null)
                {
                    Debug.LogError("[WiRR] Brak shadera Skybox/Panoramic.");
                    return false;
                }

                material = new Material(shader) { name = $"WiRR_Skybox_{safeName}" };
                AssetDatabase.CreateAsset(material, materialPath);
            }

            material.SetTexture("_MainTex", texture);
            if (material.HasProperty("_ImageType"))
                material.SetFloat("_ImageType", 0f);
            if (material.HasProperty("_Exposure"))
                material.SetFloat("_Exposure", 1f);
            if (material.HasProperty("_Rotation"))
                material.SetFloat("_Rotation", 0f);

            EditorUtility.SetDirty(material);
            AssetDatabase.SaveAssets();

            RenderSettings.skybox = material;
            DynamicGI.UpdateEnvironment();
            if (EditorSceneManager.GetActiveScene().IsValid())
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

            Selection.activeObject = material;
            EditorGUIUtility.PingObject(material);
            Debug.Log($"[WiRR] Ustawiono skybox HDRI: {definition.Name}. Materiał: {materialPath}");
            return true;
        }

        public static void RevealHdris()
        {
            var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(PackageRoot);
            if (asset != null)
            {
                Selection.activeObject = asset;
                EditorGUIUtility.PingObject(asset);
            }
            else
            {
                Debug.LogWarning($"[WiRR] Nie można otworzyć katalogu HDRI: {PackageRoot}");
            }
        }

        private static void EnsureFolder(string parent, string child)
        {
            var path = $"{parent}/{child}";
            if (!AssetDatabase.IsValidFolder(path))
                AssetDatabase.CreateFolder(parent, child);
        }
    }
}

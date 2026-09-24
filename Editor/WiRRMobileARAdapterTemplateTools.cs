using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace KIA.WiRR.Editor
{
    internal static class WiRRMobileARAdapterTemplateTools
    {
        private const string FolderName = "MobileARAdapters";

        public static void CreateAll(int labNumber)
        {
            WiRRSceneTools.EnsureFolders(labNumber);
            var root = $"{WiRRSceneTools.GetLabRootPath(labNumber)}/Scripts/{FolderName}";
            EnsureFolder(root);

            var templates = new Dictionary<string, string>
            {
                ["TapPlacementARFoundationAdapterStarter.cs"] = TapPlacementTemplate(),
                ["ImageTrackingARFoundationAdapterStarter.cs"] = ImageTrackingTemplate(),
                ["RulerARFoundationAdapterStarter.cs"] = RulerTemplate(),
                ["LightEstimationARFoundationAdapterStarter.cs"] = LightTemplate(),
                ["SurfacePainterARFoundationAdapterStarter.cs"] = PainterTemplate()
            };

            var created = 0;
            foreach (var pair in templates)
            {
                var path = $"{root}/{pair.Key}";
                if (System.IO.File.Exists(path))
                    continue;

                System.IO.File.WriteAllText(path, pair.Value, new System.Text.UTF8Encoding(false));
                created++;
            }

            AssetDatabase.Refresh();
            OpenFolder(labNumber);
            Debug.Log(created > 0
                ? $"[WiRR] Utworzono {created} starterów Mobile AR w {root}."
                : $"[WiRR] Startery Mobile AR już istnieją w {root}; niczego nie nadpisano.");
        }

        public static void OpenFolder(int labNumber)
        {
            WiRRSceneTools.EnsureFolders(labNumber);
            var root = $"{WiRRSceneTools.GetLabRootPath(labNumber)}/Scripts/{FolderName}";
            EnsureFolder(root);
            var folder = AssetDatabase.LoadAssetAtPath<DefaultAsset>(root);
            if (folder == null)
                return;

            Selection.activeObject = folder;
            EditorGUIUtility.PingObject(folder);
        }

        private static void EnsureFolder(string path)
        {
            var parts = path.Split('/');
            var current = parts[0];
            for (var i = 1; i < parts.Length; i++)
            {
                var next = $"{current}/{parts[i]}";
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }

        private static string Header(string className, string purpose) =>
$@"using KIA.WiRR;
using UnityEngine;

// STARTER MOBILE AR WiRR
// Cel: {purpose}
// Dodaj zależności AR Foundation tylko w projekcie studenta.
// W komentarzach TODO pokazano miejsce integracji z ARRaycastManager,
// ARTrackedImageManager albo ARCameraManager.

public sealed class {className} : MonoBehaviour
{{
";

        private static string TapPlacementTemplate() =>
Header("TapPlacementARFoundationAdapterStarter", "raycast ekranu telefonu do plane/depth i podanie pozy do AR_TapPlacement") +
@"    [SerializeField] private WiRRMobileARTapPlacement target;

    public void PushHit(Vector3 point, Vector3 normal, bool valid, float confidence = 1f)
    {
        if (target != null)
            target.SetSurfaceHit(point, normal, valid, confidence);
    }

    // TODO AR Foundation:
    // 1. Dodaj ARRaycastManager.
    // 2. Odczytaj bieżącą pozycję dotyku.
    // 3. Raycastuj do TrackableType.PlaneWithinPolygon lub depth hit.
    // 4. Z pierwszego trafienia pobierz pose.position i pose.up.
    // 5. Wywołaj PushHit(...).
    // 6. Sam komponent WiRR obsługuje tap i ConfirmPlacement().
}
";

        private static string ImageTrackingTemplate() =>
Header("ImageTrackingARFoundationAdapterStarter", "mapowanie ARTrackedImage na AR_ImageMarkerPortal") +
@"    [SerializeField] private WiRRMobileARImageAnchor target;

    public void PushImage(
        string imageName,
        Vector3 position,
        Quaternion rotation,
        Vector2 physicalSizeMeters,
        bool tracked,
        float confidence = 1f)
    {
        if (target != null)
            target.SetTrackedImagePose(
                imageName,
                position,
                rotation,
                physicalSizeMeters,
                tracked,
                confidence);
    }

    public void LostImage()
    {
        if (target != null)
            target.LostTracking();
    }

    // TODO AR Foundation:
    // 1. Dodaj ARTrackedImageManager i Reference Image Library.
    // 2. Obsłuż trackablesChanged / updated images.
    // 3. Przekaż referenceImage.name, transform pose i referenceImage.size.
    // 4. Gdy trackingState przestaje być Tracking, wywołaj LostImage().
}
";

        private static string RulerTemplate() =>
Header("RulerARFoundationAdapterStarter", "raycast powierzchni pod dotykiem do AR_WorldRuler") +
@"    [SerializeField] private WiRRMobileARRuler target;

    public void PushCandidate(Vector3 point, Vector3 normal, bool valid)
    {
        if (target != null)
            target.SetCandidatePoint(point, normal, valid);
    }

    public void ResetRuler()
    {
        if (target != null)
            target.ResetMeasurement();
    }

    // TODO AR Foundation:
    // 1. Użyj ARRaycastManager z pozycją aktualnego dotyku.
    // 2. Preferuj plane/depth zgodnie z celem eksperymentu.
    // 3. Przekaż hit.position oraz hit.rotation * Vector3.up.
    // 4. WiRR sam interpretuje pierwszy i drugi tap jako A/B.
    // 5. Wyświetl target.DistanceMeters w swoim UI.
}
";

        private static string LightTemplate() =>
Header("LightEstimationARFoundationAdapterStarter", "przekazanie estymacji oświetlenia kamery do AR_LightMatchObject") +
@"    [SerializeField] private WiRRMobileARLightMatch target;

    public void PushEstimate(
        float normalizedIntensity,
        Color lightColor,
        Vector3 mainLightDirection,
        float confidence = 1f)
    {
        if (target != null)
            target.SetLightEstimate(
                normalizedIntensity,
                lightColor,
                mainLightDirection,
                confidence);
    }

    // TODO AR Foundation:
    // 1. Dodaj ARCameraManager.
    // 2. Włącz wymagane Light Estimation w ARCameraManager.
    // 3. W frameReceived odczytaj dostępne: averageBrightness /
    //    averageColorTemperature / colorCorrection / mainLightColor /
    //    mainLightDirection / mainLightIntensityLumens.
    // 4. Znormalizuj jasność do zakresu 0..1 dla tego ćwiczenia.
    // 5. Przekaż tylko wartości faktycznie wspierane przez urządzenie.
}
";

        private static string PainterTemplate() =>
Header("SurfacePainterARFoundationAdapterStarter", "raycast śledzący palec i nanoszenie wirtualnego śladu na realną powierzchnię") +
@"    [SerializeField] private WiRRMobileARSurfacePainter target;

    public void PushHit(Vector3 point, Vector3 normal, bool valid)
    {
        if (target != null)
            target.SetSurfaceHit(point, normal, valid);
    }

    public void ClearDrawing()
    {
        if (target != null)
            target.ClearPaint();
    }

    // TODO AR Foundation:
    // 1. W każdej klatce, gdy palec dotyka ekranu, wykonaj ARRaycastManager.Raycast.
    // 2. Przekaż hit.position i normalną powierzchni przez PushHit().
    // 3. WiRR sam wykonuje PaintAtCurrentHit() podczas drag.
    // 4. Porównaj plane raycast z depth raycast: ciągłość, jitter i koszt.
}
";
    }
}

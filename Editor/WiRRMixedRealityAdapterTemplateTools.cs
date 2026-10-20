using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace KIA.WiRR.Editor
{
    internal static class WiRRMixedRealityAdapterTemplateTools
    {
        private const string FolderName = "MixedRealityAdapters";

        public static void CreateAll(int labNumber)
        {
            WiRRSceneTools.EnsureFolders(labNumber);
            var root = $"{WiRRSceneTools.GetLabRootPath(labNumber)}/Scripts/{FolderName}";
            EnsureFolder(root);

            var templates = new Dictionary<string, string>
            {
                ["CameraFeedAdapterStarter.cs"] = CameraTemplate(),
                ["HandTrackingAdapterStarter.cs"] = HandTemplate(),
                ["PeopleDetectorAdapterStarter.cs"] = PeopleTemplate(),
                ["SpatialDepthAdapterStarter.cs"] = SpatialTemplate(),
                ["WallPlaneAdapterStarter.cs"] = WallTemplate()
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
                ? $"[WiRR] Utworzono {created} starterów adapterów MR w {root}."
                : $"[WiRR] Startery adapterów MR już istnieją w {root}; niczego nie nadpisano.");
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
            if (parts.Length < 2)
                return;

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

// STARTER DYDAKTYCZNY WiRR
// Cel: {purpose}
// Ten plik należy do projektu studenta. Możesz go swobodnie zmieniać.
// Zasada architektury: SDK / źródło danych -> ten adapter -> niezależny od dostawcy danych komponent WiRR.
// Nie dodawaj zależności od Meta/AR Foundation do warstwy Runtime pakietu WiRR.
// Dodaj je tutaj, w kodzie własnego projektu.

public sealed class {className} : MonoBehaviour
{{
";

        private static string CameraTemplate() =>
Header("CameraFeedAdapterStarter", "podłączenie wybranego źródła obrazu do MR_CameraWindow") +
@"    [SerializeField] private WiRRCameraFeedMixer target;

    // Wywołaj tę metodę, gdy dostawca danych dostarczy nową lub zaktualizowaną Texture.
    public void PushFrame(Texture texture)
    {
        if (target != null && texture != null)
            target.SetExternalTexture(texture);
    }

    // Użyteczne np. po zatrzymaniu sesji AR/passthrough.
    public void ClearFrame()
    {
        if (target != null)
            target.ClearExternalTexture();
    }

    // TODO:
    // 1. Dodaj using/nazwy typów wybranego SDK.
    // 2. Zasubskrybuj callback/zdarzenie, które udostępnia klatkę lub Texture.
    // 3. Wywołaj PushFrame(texture).
    // 4. Sprawdź orientację obrazu; w razie potrzeby użyj target.SetMirror(...).
    // 5. Obsłuż OnEnable/OnDisable i zwolnij zasoby dostawcy danych.
}
";

        private static string HandTemplate() =>
Header("HandTrackingAdapterStarter", "mapowanie przegubów dłoni z modułu śledzenia na MR_HandAura") +
@"    [SerializeField] private WiRRHandAura target;

    public void PushPose(
        Vector3 wrist,
        Quaternion wristRotation,
        Vector3 thumbTip,
        Vector3 indexTip,
        Vector3 middleTip,
        Vector3 ringTip,
        Vector3 littleTip,
        bool tracked)
    {
        if (target == null)
            return;

        target.SetHandPose(
            wrist,
            wristRotation,
            thumbTip,
            indexTip,
            middleTip,
            ringTip,
            littleTip,
            tracked);
    }

    public void LostTracking()
    {
        if (target != null)
            target.SetTracked(false);
    }

    // TODO:
    // 1. Odczytaj nadgarstek i 5 opuszków z XR Hands / Meta Hand Tracking.
    // 2. Przelicz pozycje do przestrzeni świata Unity i metrów.
    // 3. Przekazuj aktualizację tylko wtedy, gdy dane śledzenia są ważne.
    // 4. Porównaj Pinch01 z gestem pinch raportowanym przez SDK.
    // 5. Dodaj filtrację zależną od pewności i drgań (jitteru), ale mierz opóźnienie wprowadzone przez filtr.
}
";

        private static string PeopleTemplate() =>
Header("PeopleDetectorAdapterStarter", "przekazanie anonimowych pozycji osób do MR_PeopleAwareness") +
@"    [SerializeField] private WiRRPeopleAwareness target;

    public void PushPeople(Vector3[] worldPositions)
    {
        if (target != null)
            target.SetPeople(worldPositions);
    }

    public void ClearPeople()
    {
        if (target != null)
            target.ClearPeople();
    }

    // TODO:
    // 1. Moduł detekcji powinien zwracać pozycję osoby lub szkieletu, a nie jej tożsamość.
    // 2. Wybierz stabilny punkt reprezentatywny, np. pelvis/torso/środek bbox po projekcji 3D.
    // 3. Przelicz wynik do układu świata Unity.
    // 4. Przy utracie detekcji usuń slot albo wywołaj ClearPeople().
    // 5. Nie zapisuj twarzy, nazw ani embeddingów, jeśli eksperyment ich nie wymaga.
}
";

        private static string SpatialTemplate() =>
Header("SpatialDepthAdapterStarter", "przekazanie punktów depth/spatial mesh do MR_SpatialSurfaceScanner") +
@"    [SerializeField] private WiRRSpatialSurfaceScanner target;

    public void PushSample(Vector3 worldPoint, Vector3 worldNormal, float confidence = 1f)
    {
        if (target != null)
            target.SubmitSurfaceSample(worldPoint, worldNormal, confidence);
    }

    public void PushBatch(Vector3[] worldPoints, Vector3[] worldNormals)
    {
        if (target != null)
            target.SubmitSurfaceSamples(worldPoints, worldNormals);
    }

    // TODO:
    // 1. Pobierz depth hit / spatial mesh / scene mesh z wybranego SDK.
    // 2. Przekształć punkt i normalną do układu świata Unity.
    // 3. Ogranicz gęstość próbek; nie wysyłaj całej mapy co klatkę.
    // 4. Jeżeli SDK podaje poziom pewności, przekaż go do PushSample().
    // 5. Porównaj gęstość próbkowania, opóźnienie i stabilność normalnych.
}
";

        private static string WallTemplate() =>
Header("WallPlaneAdapterStarter", "mapowanie wykrytej ściany/plane na MR_WallPortal") +
@"    [SerializeField] private WiRRWallAnchor target;

    public void PushWall(Vector3 center, Vector3 normal, Vector2 sizeMeters)
    {
        if (target != null)
            target.SetWallPlane(center, normal, sizeMeters);
    }

    public void ClearWall()
    {
        if (target != null)
            target.ClearAnchor();
    }

    // TODO:
    // 1. Odczytaj listę plane/scene anchors z AR Foundation lub Scene Understanding.
    // 2. Odfiltruj powierzchnie niebędące ścianami (orientacja/semantic label).
    // 3. Wybierz ścianę wskazaną przez użytkownika lub najbliższą rayowi.
    // 4. Upewnij się, że normalna wskazuje do wnętrza pomieszczenia / w stronę użytkownika.
    // 5. Przekaż center, normal i size w metrach.
    // 6. Zmierz dryf kotwicy po ruchu głowy i po ponownym wykryciu ściany.
}
";
    }
}

using UnityEditor;
using UnityEngine;

namespace KIA.WiRR.Editor
{
    public sealed class WiRRVitureGuideWindow : EditorWindow
    {
        private const string AcademyUrl = "https://www.viture.com/academy";
        private const string DownloadUrl = "https://www.viture.com/academy/download";
        private const string Immersive3DUrl = "https://www.viture.com/blog/a-worlds-first-turn-2d-into-magical-3d-in-real-time";
        private Vector2 scroll;
        private int tab;

        public static void Open()
        {
            var window = GetWindow<WiRRVitureGuideWindow>();
            window.titleContent = new GUIContent("WiRR · VITURE XR");
            window.minSize = new Vector2(650, 580);
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("VITURE XR: 3D SBS i Immersive 3D", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "VITURE może działać jako stereoskopowy wyświetlacz Full SBS. Tryb 2D-to-3D (Immersive 3D) jest osobną funkcją programową SpaceWalker / VITURE i nie zastępuje natywnego renderowania stereo w Unity.",
                MessageType.Info);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Utwórz rig Full SBS", GUILayout.Height(28)))
                    WiRRVitureTools.CreateFullSbsRig();
                if (GUILayout.Button("VITURE Academy", GUILayout.Height(28)))
                    Application.OpenURL(AcademyUrl);
                if (GUILayout.Button("Pobierz SpaceWalker", GUILayout.Height(28)))
                    Application.OpenURL(DownloadUrl);
            }

            tab = GUILayout.Toolbar(tab, new[] { "Unity Full SBS", "Film 3D SBS", "2D → 3D", "Sterowanie" });
            scroll = EditorGUILayout.BeginScrollView(scroll);
            EditorGUILayout.Space(8);

            if (tab == 0) DrawUnity();
            else if (tab == 1) DrawVideo();
            else if (tab == 2) DrawImmersive();
            else DrawControls();

            EditorGUILayout.EndScrollView();
        }

        private static void DrawUnity()
        {
            Heading("Przykład Unity: Full SBS 3840×1080");
            Step("1. Rig", "Użyj przycisku „Utwórz rig Full SBS”. WiRR tworzy dwie kamery z równoległymi osiami i rozstawem początkowym 64 mm.");
            Step("2. Obraz", "Lewa kamera renderuje lewą połowę, prawa prawą połowę ekranu. Każde oko otrzymuje 1920×1080 w docelowym framebufferze 3840×1080.");
            Step("3. Build", "Dla PC ustaw okno lub pełny ekran 3840×1080. Nie używaj zwykłego 1920×1080 jako docelowego obrazu Full SBS, jeśli chcesz zachować pełną rozdzielczość na oko.");
            Step("4. Okulary", "Podłącz VITURE jako ekran USB-C/DisplayPort i włącz tryb 3D odpowiednim przyciskiem dla danego modelu.");
            Step("5. Walidacja", "Sprawdź, czy obiekt blisko kamery ma większą dysparację niż obiekt daleki. Jeżeli obraz powoduje dyskomfort, zmniejsz bazę stereo/IPD lub głębokość sceny.");
            Note("To jest demonstrator stereoskopii, nie pełny headset VR. Nie zakłada automatycznie 6DoF ani kontrolerów.");
        }

        private static void DrawVideo()
        {
            Heading("Odtwarzanie filmu 3D SBS");
            Step("1. Format", "Najbardziej przenośnym formatem dla okularów VITURE jest Full SBS 3840×1080, czyli dwa obrazy 1920×1080 obok siebie.");
            Step("2. Player", "Na PC producent rekomenduje m.in. Kodi; na Androidzie dobrym wyborem jest VLC. SpaceWalker może dodatkowo wykrywać zgodne treści 3D na obsługiwanych platformach.");
            Step("3. Tryb okularów", "Uruchom film SBS i przełącz okulary w tryb 3D. Jeśli nadal widzisz dwa obrazy obok siebie, okulary pozostają w trybie 2D lub player nie podaje prawidłowego obrazu SBS.");
            Step("4. Proporcje", "Nie rozciągaj pojedynczego oka do całego ekranu. Player powinien zachować układ side-by-side.");
            Note("Nie każdy format określany jako „SBS” jest Full SBS. Half-SBS może wymagać odtwarzacza lub urządzenia pośredniczącego, które odpowiednio go przetworzy.");
        }

        private static void DrawImmersive()
        {
            Heading("Immersive 3D: 2D → 3D");
            Step("1. To inny mechanizm", "Immersive 3D analizuje zwykłą treść 2D i syntetyzuje parę stereoskopową w czasie rzeczywistym. Nie wymaga, aby źródłowy film był zapisany jako SBS.");
            Step("2. Desktop", "Na Windows i macOS użyj aplikacji Immersive 3D / narzędzi VITURE. Producent opisuje wsparcie dla treści desktopowych, streamingu i gier na obsługiwanych konfiguracjach.");
            Step("3. Mobile", "W SpaceWalker funkcja Immersive 3D jest dostępna na obsługiwanych urządzeniach mobilnych. Dla zdjęcia lub filmu wybierz funkcję Immersive 3D, a następnie oglądaj wynik w trybie 3D okularów.");
            Step("4. Głębokość", "Dostępne są poziomy głębokości, m.in. Soft, Standard i Enhanced. Do dłuższych sesji dydaktycznych zacznij od łagodniejszej konfiguracji.");
            Step("5. Tryb pracy", "Movie Mode jest przeznaczony do materiałów filmowych, a Game Mode ogranicza opóźnienie podczas grania.");
            if (GUILayout.Button("Otwórz oficjalny opis Immersive 3D", GUILayout.Height(28)))
                Application.OpenURL(Immersive3DUrl);
            Note("2D-to-3D tworzy estymowaną głębię. Nie traktuj jej jako pomiaru geometrii sceny ani danych referencyjnych do eksperymentów metrologicznych.");
        }

        private static void DrawControls()
        {
            Heading("Przełączanie 2D / 3D");
            Step("VITURE One / Pro", "Naciśnij i przytrzymaj przycisk Mode, czyli krótszy przycisk na lewym zauszniku. Funkcje mogą się różnić wraz z firmware, dlatego aktualizuj okulary.");
            Step("VITURE Luma", "Naciśnij i przytrzymaj R1, aby przełączyć między 2D i 3D.");
            Step("SpaceWalker i adaptery", "Na wspieranych konfiguracjach SpaceWalker może wykryć treść Full SBS lub Immersive 3D i automatycznie przełączyć okulary w tryb 3D.");
            Note("Jeśli model lub firmware zachowuje się inaczej, użyj bieżącej instrukcji VITURE Academy dla konkretnego modelu.");
        }

        private static void Heading(string text)
        {
            EditorGUILayout.LabelField(text, EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
        }

        private static void Step(string title, string body)
        {
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            EditorGUILayout.LabelField(body, EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space(6);
        }

        private static void Note(string text)
        {
            EditorGUILayout.HelpBox(text, MessageType.Warning);
            EditorGUILayout.Space(6);
        }
    }
}

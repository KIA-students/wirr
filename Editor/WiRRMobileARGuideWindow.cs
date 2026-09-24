using UnityEditor;
using UnityEngine;

namespace KIA.WiRR.Editor
{
    public sealed class WiRRMobileARGuideWindow : EditorWindow
    {
        private static readonly string[] Tabs =
        {
            "Start",
            "Placement",
            "Image",
            "Ruler",
            "Light",
            "Painter",
            "Rozbudowa"
        };

        private Vector2 scroll;
        private int tab;
        private int labNumber = 3;

        [MenuItem("WiRR/Pomoc/Mobile AR: telefon", priority = 52)]
        public static void Open()
        {
            Open(3);
        }

        public static void Open(int selectedLab)
        {
            var window = GetWindow<WiRRMobileARGuideWindow>();
            window.titleContent = new GUIContent("WiRR · Mobile AR");
            window.minSize = new Vector2(760, 650);
            window.labNumber = Mathf.Clamp(selectedLab, 1, 7);
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Mobile AR na smartfonie", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Te demonstratory są projektowane pod ekran telefonu: kamera z tyłu urządzenia, dotyk, raycast z pozycji palca, ARCore/AR Foundation i pionowy lub poziomy ekran. Fallback w Editorze służy wyłącznie do sprawdzenia logiki; wynik AR oceniaj na realnym telefonie.",
                MessageType.Info);

            labNumber = EditorGUILayout.IntSlider("Laboratorium / workspace", labNumber, 1, 7);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Utwórz 5 starterów AR Foundation", GUILayout.Height(30)))
                    WiRRMobileARAdapterTemplateTools.CreateAll(labNumber);

                if (GUILayout.Button("Pokaż folder adapterów", GUILayout.Height(30)))
                    WiRRMobileARAdapterTemplateTools.OpenFolder(labNumber);
            }

            tab = GUILayout.Toolbar(tab, Tabs);
            scroll = EditorGUILayout.BeginScrollView(scroll);
            EditorGUILayout.Space(8);

            switch (tab)
            {
                case 0: DrawStart(); break;
                case 1: DrawPlacement(); break;
                case 2: DrawImage(); break;
                case 3: DrawRuler(); break;
                case 4: DrawLight(); break;
                case 5: DrawPainter(); break;
                default: DrawExtensions(); break;
            }

            EditorGUILayout.EndScrollView();
        }

        private static void DrawStart()
        {
            Heading("Jak uruchomić na telefonie");
            Step("1. Android", "Doinstaluj Android Build Support, SDK/NDK i OpenJDK. Włącz debugowanie USB w telefonie.");
            Step("2. AR Foundation", "Dla Lab 03–04 zainstaluj zależności przez WiRR. W scenie użyj AR Session i XR Origin odpowiednich dla AR Foundation/ARCore.");
            Step("3. Kamera i uprawnienia", "Build Android musi mieć dostęp do kamery. Testuj na urządzeniu wspierającym wybrane funkcje ARCore.");
            Step("4. Dodaj demonstrator", "W Course Toolkit dodaj pojedynczy prefab albo cały zestaw Mobile AR. Najpierw sprawdź fallback w Editorze.");
            Step("5. Wygeneruj adapter", "Utwórz startery AR Foundation. W kodzie studenta podłącz manager właściwy dla danego elementu.");
            Step("6. Build And Run", "Przełącz platformę na Android, wybierz telefon w Run Device i użyj Build And Run.");
            Note("Nie oceniaj jakości trackingu na podstawie Game View. Plane detection, camera tracking, light estimation i image tracking muszą być zweryfikowane na fizycznym urządzeniu.");
        }

        private static void DrawPlacement()
        {
            Heading("AR_TapPlacement");
            Step("Cel", "Reticle śledzi powierzchnię pod palcem. Tap umieszcza obiekt w rzeczywistym otoczeniu.");
            Step("Provider", "ARRaycastManager wykonuje raycast dla pozycji dotyku do plane/depth i przekazuje point + normal przez SetSurfaceHit().");
            Step("Interakcja", "Komponent WiRR sam wykrywa tap i wywołuje ConfirmPlacement(). Trzeba więc dostarczać aktualny hit, a nie implementować drugi system tap.");
            Step("Walidacja", "Sprawdź skalę 1:1, ustawienie na podłodze/stole, zachowanie przy krawędzi plane i po chwilowej utracie trackingu.");
            Challenge("Rozbudowa", "Dodaj pinch-to-scale i twist-to-rotate po umieszczeniu obiektu.");
            Challenge("Rozbudowa", "Dodaj ghost preview czerwony/zielony zależny od poprawności powierzchni.");
            Challenge("Rozbudowa", "Porównaj PlaneWithinPolygon z depth hit i zmierz błąd pozycji.");
        }

        private static void DrawImage()
        {
            Heading("AR_ImageMarkerPortal");
            Step("Cel", "Rozpoznany rzeczywisty obraz staje się kotwicą dla wirtualnej zawartości.");
            Step("Provider", "ARTrackedImageManager + Reference Image Library. Przekaż nazwę obrazu, pose, rozmiar fizyczny i stan trackingu.");
            Step("Skala", "Rozmiar fizyczny obrazu jest używany do skalowania zawartości. Błędnie podana szerokość reference image powoduje błędną skalę AR.");
            Step("Walidacja", "Sprawdź tracking z różnych kątów, dystansów i przy częściowym zasłonięciu.");
            Challenge("Rozbudowa", "Różne markery uruchamiają różne prefaby lub dane.");
            Challenge("Rozbudowa", "Dodaj stan TRACKING LIMITED i płynne wygaszanie treści.");
            Challenge("Rozbudowa", "Zbuduj miniaturową animowaną scenę 'wychodzącą' z kartki.");
        }

        private static void DrawRuler()
        {
            Heading("AR_WorldRuler");
            Step("Cel", "Pierwszy tap ustala punkt A, drugi B. DistanceMeters zwraca dystans w metrach.");
            Step("Provider", "Raycast plane/depth pod bieżącym dotykiem aktualizuje SetCandidatePoint().");
            Step("UI", "Wyświetl DistanceMeters w swoim Canvas/UI. WiRR nie wymusza TextMeshPro w Runtime.");
            Step("Walidacja", "Zmierz obiekt o znanym wymiarze kilka razy z różnych odległości i kątów.");
            Challenge("Rozbudowa", "Dodaj pomiar polilinii i obwodu.");
            Challenge("Rozbudowa", "Dodaj pomiar powierzchni wielokąta.");
            Challenge("Rozbudowa", "Wyznacz niepewność wyniku z rozrzutu kilku raycastów depth.");
        }

        private static void DrawLight()
        {
            Heading("AR_LightMatchObject");
            Step("Cel", "Wirtualny obiekt reaguje na jasność, kolor i kierunek realnego oświetlenia.");
            Step("Provider", "ARCameraManager light estimation. Znormalizuj jasność do 0–1 i przekaż SetLightEstimate().");
            Step("Kompatybilność", "Nie każde urządzenie zwraca wszystkie składniki estymacji. Obsłuż brak mainLightDirection lub color correction.");
            Step("Walidacja", "Przenieś telefon między jasnym i ciemnym miejscem oraz zmień stronę względem źródła światła.");
            Challenge("Rozbudowa", "Dodaj automatyczne dopasowanie ekspozycji/post-processingu.");
            Challenge("Rozbudowa", "Porównaj wirtualny obiekt z realnym wzorcem o podobnym materiale.");
            Challenge("Rozbudowa", "Loguj dostępność poszczególnych pól Light Estimation na różnych telefonach.");
        }

        private static void DrawPainter()
        {
            Heading("AR_SurfacePainter");
            Step("Cel", "Przesuwanie palca po ekranie pozostawia wirtualny ślad na wykrytej realnej powierzchni.");
            Step("Provider", "Podczas drag wykonuj ARRaycastManager.Raycast dla aktualnej pozycji palca i podawaj hit do SetSurfaceHit().");
            Step("Wydajność", "Prefab używa puli 120 markerów; nie tworzy GameObject dla każdego punktu w trakcie rysowania.");
            Step("Walidacja", "Rysuj przez granice kilku plane, przy szybkim ruchu telefonu i z użyciem depth raycast.");
            Challenge("Rozbudowa", "Dodaj wybór grubości, kształtu i materiału pędzla.");
            Challenge("Rozbudowa", "Zapisz rysunek względem anchorów i odtwórz go po restarcie.");
            Challenge("Rozbudowa", "Zamiast punktów generuj wygładzony mesh/stroke.");
        }

        private static void DrawExtensions()
        {
            Heading("Pomysły na dalszą rozbudowę Mobile AR");
            Challenge("Occlusion-aware placement", "Ukrywaj fragment wirtualnego obiektu za realną geometrią z depth.");
            Challenge("Persistent placement", "Zapisz anchor i odtwórz po ponownym uruchomieniu aplikacji.");
            Challenge("Multi-touch manipulation", "Połącz tap placement z pinch scale, rotate i przesuwaniem po plane.");
            Challenge("Semantic placement", "Dopuszczaj obiekt tylko na podłodze, stole albo ścianie.");
            Challenge("Image-to-world handoff", "Użyj markera tylko do inicjalizacji, potem przenieś zawartość na world anchor.");
            Challenge("Measurement confidence", "Pokazuj nie tylko wynik ruler, ale także niepewność.");
            Challenge("Cross-device test", "Porównaj dwa telefony: czas wykrycia plane, dryf, ruler error i stabilność image tracking.");
            Note("Najlepsza rozbudowa ma baseline, jedną zmianę i metrykę. Dla Mobile AR użyteczne są: czas pierwszej detekcji, błąd położenia [cm], dryf [cm/min], FPS, latency i błąd pomiaru.");
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
            EditorGUILayout.Space(7);
        }

        private static void Challenge(string title, string body)
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
                EditorGUILayout.LabelField(body, EditorStyles.wordWrappedLabel);
            }
            EditorGUILayout.Space(3);
        }

        private static void Note(string body)
        {
            EditorGUILayout.HelpBox(body, MessageType.Warning);
            EditorGUILayout.Space(6);
        }
    }
}

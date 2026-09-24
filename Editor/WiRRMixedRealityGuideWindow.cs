using UnityEditor;
using UnityEngine;

namespace KIA.WiRR.Editor
{
    public sealed class WiRRMixedRealityGuideWindow : EditorWindow
    {
        private static readonly string[] Tabs =
        {
            "Architektura",
            "Camera",
            "Hand",
            "People",
            "Spatial",
            "Wall",
            "Rozbudowa"
        };

        private Vector2 scroll;
        private int tab;
        private int labNumber = 4;

        [MenuItem("WiRR/Pomoc/Mixed Reality: implementacja i rozbudowa", priority = 51)]
        public static void Open()
        {
            Open(4);
        }

        public static void Open(int selectedLab)
        {
            var window = GetWindow<WiRRMixedRealityGuideWindow>();
            window.titleContent = new GUIContent("WiRR · MR Guide");
            window.minSize = new Vector2(760, 680);
            window.labNumber = Mathf.Clamp(selectedLab, 1, 7);
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Mixed Reality: jak zaimplementować i jak rozbudować", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Gotowe prefaby są punktem startowym, a nie końcem ćwiczenia. Najważniejszy wzorzec to: sensor lub SDK → mały adapter w projekcie studenta → komponent WiRR → wizualizacja. Dzięki temu można zmienić AR Foundation na Meta SDK albo własny model CV bez przepisywania logiki prefabu.",
                MessageType.Info);

            labNumber = EditorGUILayout.IntSlider("Laboratorium / obszar roboczy", labNumber, 1, 7);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Utwórz 5 starterów adapterów", GUILayout.Height(30)))
                    WiRRMixedRealityAdapterTemplateTools.CreateAll(labNumber);

                if (GUILayout.Button("Pokaż folder adapterów", GUILayout.Height(30)))
                    WiRRMixedRealityAdapterTemplateTools.OpenFolder(labNumber);
            }

            EditorGUILayout.HelpBox(
                $"Startery trafiają do Assets/WiRR/Lab{labNumber:00}/Scripts/MixedRealityAdapters i nigdy nie są automatycznie nadpisywane. To kod studenta: można dodawać zależności konkretnego SDK i eksperymentować bez modyfikowania pakietu WiRR.",
                MessageType.None);

            tab = GUILayout.Toolbar(tab, Tabs);
            scroll = EditorGUILayout.BeginScrollView(scroll);
            EditorGUILayout.Space(8);

            switch (tab)
            {
                case 0: DrawArchitecture(); break;
                case 1: DrawCamera(); break;
                case 2: DrawHand(); break;
                case 3: DrawPeople(); break;
                case 4: DrawSpatial(); break;
                case 5: DrawWall(); break;
                default: DrawExtensions(); break;
            }

            EditorGUILayout.EndScrollView();
        }

        private static void DrawArchitecture()
        {
            Heading("Wzorzec implementacyjny: dostawca danych → adapter → komponent WiRR");

            Step("1. Dodaj prefab MR",
                "W WiRR → Narzędzia kursu → Scena i pomiary dodaj wybrany demonstrator. Uruchom tryb Play i najpierw sprawdź działanie trybu zastępczego. Dzięki temu wiesz, że warstwa wizualna działa zanim dołączysz sensor.");

            Step("2. Obejrzyj komponent Runtime",
                "W Inspectorze znajdź komponent WiRR na korzeniu prefabu. Nie zaczynaj od edycji pakietu. Zapisz, jakiej metody wejściowej oczekuje: Texture, joints dłoni, pozycje osób, point+normal albo plane ściany.");

            Step("3. Wygeneruj starter adaptera",
                "Kliknij „Utwórz 5 starterów adapterów”. Otwórz tylko plik dotyczący Twojego elementu. Starter kompiluje się bez vendor SDK i zawiera metodę Push..., która już wywołuje właściwe API WiRR.");

            Step("4. Dodaj SDK po stronie projektu",
                "Dopiero w adapterze studenta dodaj dyrektywy `using` oraz typy konkretnego SDK: AR Foundation, XR Hands, Meta SDK, własny moduł CV lub danych głębi. Warstwa Runtime WiRR pozostaje niezależna od dostawcy danych.");

            Step("5. Przelicz układy współrzędnych",
                "Pozycje muszą trafić do układu świata Unity, w metrach i w tej samej konwencji osi co scena. Dla normalnych transformuj kierunek, nie punkt. Dla danych ekranowych wykonaj ray/depth unprojection do 3D.");

            Step("6. Obsłuż lifecycle i utratę śledzenia",
                "Podłącz dostawcę danych w `OnEnable`, a odłącz w `OnDisable`. Przy utracie danych nie zostawiaj starej pozycji jako „prawdziwej”: użyj SetTracked(false), ClearPeople(), ClearAnchor() albo odpowiedniego resetu.");

            Step("7. Najpierw poprawność, potem filtracja",
                "Najpierw sprawdź surowe dane. Dopiero później dodaj wygładzanie, predykcję lub odrzucanie danych na podstawie poziomu pewności. Każdy filtr zmniejsza drgania (jitter) kosztem dodatkowego opóźnienia — to dobra zmienna eksperymentalna.");

            Step("8. Waliduj na urządzeniu docelowym",
                "Okno `Game View` nie zastępuje telefonu ani Quest 3. Sprawdź orientację, skalę 1 m, opóźnienie, utratę śledzenia, ponowne wykrycie i zachowanie po wznowieniu aplikacji.");

            Note("Nie kopiuj kodu konkretnego SDK do Runtime pakietu WiRR. Adapter jest świadomie granicą zależności. To ułatwia replikację eksperymentu i porównanie dwóch dostawców danych.");
        }

        private static void DrawCamera()
        {
            Heading("MR_CameraWindow — dokładna implementacja");

            Step("1. Utwórz Camera Window",
                "Dodaj prefab. W Play Mode domyślnie spróbuje użyć pierwszej kamery dostępnej przez WebCamTexture i poprosi system o zgodę. Sprawdź, czy wirtualna ramka i reticle pozostają stabilne.");

            Step("2. Wybierz model integracji",
                "PC/smartfon: możesz użyć WebCamTexture. Jeżeli SDK udostępnia Texture/RenderTexture, wyłącz lokalną kamerę i dostarczaj klatkę przez SetExternalTexture(Texture). Quest passthrough często działa jako warstwa compositora — wtedy traktuj prefab jako wirtualny overlay nad passthrough zamiast oczekiwać zwykłej tekstury kamery.");

            Step("3. Podłącz adapter",
                "W CameraFeedAdapterStarter przypisz referencję do WiRRCameraFeedMixer. W callbacku dostawcy danych wywołaj PushFrame(texture). Przy zatrzymaniu sesji wywołaj ClearFrame().");

            Step("4. Sprawdź geometrię obrazu",
                "Zweryfikuj aspect ratio, rotację urządzenia, front/back camera i mirror. Jeżeli obraz jest odbity, użyj SetMirror(horizontal, vertical). Nie naprawiaj orientacji przez przypadkowe obracanie całego prefabu.");

            Step("5. Zmierz opóźnienie",
                "Wyświetl w kadrze szybko zmieniający się bodziec rzeczywisty (np. licznik lub migający znacznik) i porównaj z jego wirtualnym odpowiednikiem. Zapisz opóźnienie kamera → ekran oraz jego zmienność.");

            Challenge("Rozbudowa 1 — kalibracja", "Dodaj parametry intrinsics i poprawne mapowanie punktu obrazu na ray 3D.");
            Challenge("Rozbudowa 2 — vision overlay", "Dodaj bbox/segmentację/etykiety obiektów jako osobną warstwę, ale oddziel wynik CV od tekstury źródłowej.");
            Challenge("Rozbudowa 3 — opóźnienie", "Zbuduj miernik znaczników czasu: przechwycenie → odbiór → renderowanie.");
            Challenge("Rozbudowa 4 — prywatność", "Dodaj tryb automatycznego maskowania twarzy albo regionów wrażliwych przed zapisem.");
            Challenge("Rozbudowa 5 — passthrough styling", "Porównaj neutralny passthrough z wirtualnym gradingiem/LUT lub selektywnym przyciemnieniem tła.");
        }

        private static void DrawHand()
        {
            Heading("MR_HandAura — dokładna implementacja");

            Step("1. Utwórz Hand Aura",
                "Tryb zastępczy pokaże proceduralną dłoń przed kamerą i okresowy gest szczypnięcia (pinch). Sprawdź, jak zachowują się markery nadgarstka, opuszków i obiekt `PinchCore`.");

            Step("2. Zidentyfikuj przeguby (jointy) zwracane przez moduł śledzenia",
                "Potrzebujesz co najmniej wrist oraz tip: thumb, index, middle, ring, little. Jeżeli SDK podaje pełny szkielet, na początku użyj tylko tych sześciu punktów.");

            Step("3. Konwersja przestrzeni",
                "Pobierz pozycje w przestrzeni dostawcy danych i przekształć je do układu świata Unity. Sprawdź skalę w metrach oraz orientację i zwrot osi współrzędnych. Jeden błąd transformacji może wyglądać jak „błędne śledzenie”.");

            Step("4. Przekazuj pose",
                "W HandTrackingAdapterStarter wywołuj PushPose(...) tylko dla ważnego śledzenia. Gdy śledzenie znika, wywołaj LostTracking(). Nie zamrażaj starej dłoni jako aktualnej.");

            Step("5. Porównaj pinch",
                "WiRR oblicza `Pinch01` z odległości kciuk–palec wskazujący. Jeżeli SDK udostępnia własną siłę gestu pinch, zapisz oba sygnały i porównaj próg, histerezę oraz opóźnienie.");

            Step("6. Dodaj filtr świadomie",
                "Zmierz drgania (jitter) bez filtra. Następnie dodaj filtr dolnoprzepustowy, One Euro albo Kalmana po stronie adaptera i ponownie zmierz drgania (jitter) oraz opóźnienie.");

            Challenge("Rozbudowa 1 — pełny szkielet", "Dodaj 21/26 jointów i wirtualny hand mesh zamiast pięciu linii.");
            Challenge("Rozbudowa 2 — pewność", "Kolor i przezroczystość markerów uzależnij od poziomu pewności każdego przegubu.");
            Challenge("Rozbudowa 3 — gesty", "Rozpoznaj chwycenie (`grab`), wskazanie (`point`), kciuk w górę (`thumbs-up`) i otwartą dłoń (`open hand`) i zdefiniuj stanową maszynę gestów.");
            Challenge("Rozbudowa 4 — dotyk przestrzenny", "Dodaj near-interaction z realną dłonią i wirtualnym przyciskiem z odkształceniem/feedbackiem.");
            Challenge("Rozbudowa 5 — bimanual", "Dodaj drugą dłoń i gesty dwuręczne: skalowanie, obrót i rozciąganie obiektu.");
        }

        private static void DrawPeople()
        {
            Heading("MR_PeopleAwareness — dokładna implementacja");

            Step("1. Utwórz People Awareness",
                "Tryb zastępczy symuluje dwie osoby. Zwróć uwagę, że wizualizacja reaguje na odległość, ale nie wymaga tożsamości.");

            Step("2. Wybierz reprezentację pozycji",
                "Najlepiej użyć pelvis/torso albo stabilnego środka bbox po rekonstrukcji 3D. Nie używaj punktu twarzy jako jedynego położenia osoby, jeśli celem jest proxemics lub bezpieczeństwo.");

            Step("3. Z 2D do 3D",
                "Jeżeli detektor pracuje na obrazie, potrzebujesz depth, ray-plane intersection lub body tracker 3D. Sam środek bbox w pikselach nie jest pozycją w świecie.");

            Step("4. Minimalizuj dane",
                "Do WiRR przekazuj Vector3[] worldPositions. Nie przekazuj obrazu twarzy, nazw ani embeddingów, jeżeli nie są wymagane przez hipotezę badawczą.");

            Step("5. Aktualizuj i usuwaj",
                "Wywołuj PushPeople na każdej sensownej aktualizacji detektora. Przy braku ludzi wywołaj ClearPeople(). Ustal timeout i odróżnij „brak człowieka” od „brak danych sensora”.");

            Step("6. Waliduj odległość",
                "Umieść człowieka w znanych punktach 1 m / 2 m / 3 m i porównaj pozycję wirtualnego markera. To prosty test błędu rejestracji.");

            Challenge("Rozbudowa 1 — prędkość", "Estymuj wektor prędkości i pokaż krótki predykowany tor ruchu.");
            Challenge("Rozbudowa 2 — proxemics", "Zastąp kołowe halo elipsą zależną od kierunku ruchu i orientacji osoby.");
            Challenge("Rozbudowa 3 — wiele osób", "Dodaj chwilowe, anonimowe track-id tylko do zachowania ciągłości trajektorii.");
            Challenge("Rozbudowa 4 — HRI safety", "Połącz pozycję człowieka ze stanem robota i wizualizuj dynamiczną strefę bezpieczeństwa.");
            Challenge("Rozbudowa 5 — social cues", "Dodaj kierunek ciała/spojrzenia bez identyfikacji osoby i zbadaj czytelność sygnałów społecznych.");
        }

        private static void DrawSpatial()
        {
            Heading("MR_SpatialSurfaceScanner — dokładna implementacja");

            Step("1. Utwórz Spatial Scanner",
                "W Editorze scanner używa rozproszonych raycastów do colliderów. Porusz kamerą i sprawdź, czy próbki pojawiają się na ścianach/podłodze i zanikają po czasie.");

            Step("2. Wybierz źródło geometrii",
                "Może to być pojedynczy depth hit, depth image po unprojection, spatial mesh albo scene mesh. Do pierwszej integracji nie wysyłaj całej siatki: wybierz niewielką liczbę próbek.");

            Step("3. Przekazuj point + normal",
                "`SpatialDepthAdapterStarter` przyjmuje `worldPoint`, `worldNormal` i opcjonalny poziom `confidence`. Normalna musi być kierunkiem w układzie świata Unity, a nie pozycją.");

            Step("4. Ogranicz częstotliwość",
                "Próbkowanie depth w każdej klatce dla tysięcy punktów może zdominować CPU/GPU. Zacznij od kilkudziesięciu punktów na sekundę i zmierz koszt.");

            Step("5. Waliduj na geometrii referencyjnej",
                "Użyj płaskiej ściany i podłogi. Sprawdź odchylenie punktów od płaszczyzny i rozrzut normalnych. Dopiero potem testuj złożone otoczenie.");

            Step("6. Oddziel wizualizację od danych",
                "Punkty WiRR są diagnostyką. Jeżeli później zbudujesz mesh do okluzji/fizyki, trzymaj strukturę danych osobno od markerów wizualnych.");

            Challenge("Rozbudowa 1 — mapa cieplna pewności", "Skaluj/koloruj punkty według pewności sensora.");
            Challenge("Rozbudowa 2 — mesh reconstruction", "Zbuduj lekki mesh z próbek i aktualizuj go inkrementalnie.");
            Challenge("Rozbudowa 3 — okluzja", "Użyj realnej geometrii jako depth-only occludera dla wirtualnych obiektów.");
            Challenge("Rozbudowa 4 — semantyka", "Klasyfikuj `floor`/`wall`/`table` (podłoga/ściana/stół) i użyj innego zachowania wirtualnych obiektów.");
            Challenge("Rozbudowa 5 — fizyka", "Pozwól wirtualnej piłce odbić się od realnego stołu i zmierz błąd kontaktu.");
        }

        private static void DrawWall()
        {
            Heading("MR_WallPortal — dokładna implementacja");

            Step("1. Utwórz Wall Portal",
                "Prefab zawiera WiRRWallAnchor oraz WiRRHeadParallax. Bez Scene Understanding możesz wywołać TryAnchorFromViewerRay(), jeżeli w scenie istnieje collider reprezentujący ścianę.");

            Step("2. Pobierz plane/scene anchor",
                "Z dostawcy danych odczytaj środek, orientację/normalną i rozmiar płaszczyzny. Odfiltruj podłogę i sufit; użyj etykiety semantycznej `wall`, jeśli SDK ją udostępnia.");

            Step("3. Ustal kierunek normalnej",
                "Normalna powinna wskazywać na stronę pomieszczenia/użytkownika. Jeżeli portal odwraca się do ściany, odwróć normalną przed PushWall.");

            Step("4. Przekaż metry",
                "WallPlaneAdapterStarter wywołuje SetWallPlane(center, normal, sizeMeters). Rozmiar wpływa na skalowanie zawartości, więc nie przekazuj pikseli ani jednostek centymetrowych.");

            Step("5. Testuj stabilność kotwicy",
                "Obejdź portal z boku, odwróć głowę, chwilowo zasłoń ścianę, wróć. Obserwuj dryf i skoki po ponownym wykryciu.");

            Step("6. Testuj granice ściany",
                "Portal powinien mieścić się na płaszczyźnie. Dodaj margines od krawędzi, jeśli dostawca danych podaje granice wielokąta (`polygon`/`extent`), a nie tylko środek i rozmiar.");

            Challenge("Rozbudowa 1 — trwałe kotwice (persistent anchors)", "Zapisz kotwicę i sprawdź, czy portal wraca w to samo miejsce po restarcie aplikacji.");
            Challenge("Rozbudowa 2 — wiele ścian", "Pozwól użytkownikowi wybrać spośród kilku wykrytych ścian rayem lub gestem.");
            Challenge("Rozbudowa 3 — krawędzie portalu", "Dodaj efekt rozcinania/feathering przy kontakcie wirtualnej głębi z realną ścianą.");
            Challenge("Rozbudowa 4 — portal fizyczny", "Połącz realną ścianę z wirtualnym pokojem i obsłuż kolizje obiektów przechodzących przez portal.");
            Challenge("Rozbudowa 5 — dekoracja proceduralna", "Generuj półki, ekrany lub obrazy dopasowane do wykrytych wymiarów ściany.");
        }

        private static void DrawExtensions()
        {
            Heading("Nie kończ na gotowym prefabie");

            EditorGUILayout.HelpBox(
                "Dobra rozbudowa nie polega na dodaniu przypadkowego efektu. Wybierz jedną cechę MR, sformułuj pytanie techniczne lub HCI i zrób zmianę, której efekt da się zmierzyć.",
                MessageType.Info);

            Challenge("Projekt A — budżet opóźnienia",
                "Dla wybranego prefabu rozbij opóźnienie na: sensor lub dostawca danych → adapter → filtr → komponent WiRR → renderowanie. Zmierz medianę i p95.");

            Challenge("Projekt B — uncertainty-aware MR",
                "Przenieś pewność dostawcy danych do wizualizacji: przezroczystość, rozmiar, kolor, pulsowanie albo stabilność. Porównaj z interfejsem, który ukrywa niepewność.");

            Challenge("Projekt C — graceful degradation",
                "Zaprojektuj zachowanie przy utracie sensora: zamrożenie ostatniej pozycji, wygaszenie, ukrycie, predykcję albo tryb zastępczy. Uzasadnij wybór i zmierz czas odzyskania.");

            Challenge("Projekt D — porównanie dostawców danych",
                "Podłącz dwa źródła do tego samego komponentu WiRR, np. AR Foundation i Meta. Porównaj dokładność, opóźnienie, stabilność i nakład implementacyjny.");

            Challenge("Projekt E — multimodalność",
                "Połącz MR z dźwiękiem przestrzennym, haptics, gaze lub gestem. Niech drugi kanał przekazuje inną informację, a nie tylko duplikuje grafikę.");

            Challenge("Projekt F — eksperyment użytkownika",
                "Wybierz jedną modyfikację i przygotuj A/B: wersja bazowa vs ulepszona. Zdefiniuj obiektywną metrykę (czas, błąd, liczba pomyłek) oraz subiektywną ocenę.");

            Heading("Minimalne kryterium dobrej rozbudowy");
            Step("1. Hipoteza", "Jedno zdanie: co ma się poprawić lub zmienić.");
            Step("2. Zmienna", "Jedna kontrolowana modyfikacja, nie pięć naraz.");
            Step("3. Metryka", "Co najmniej jedna liczba możliwa do powtórzenia.");
            Step("4. Warunki", "Ta sama scena, urządzenie, build i procedura A/B.");
            Step("5. Wniosek", "Oddziel obserwację od interpretacji. Jeżeli efekt nie wystąpił, to także jest wynik.");

            Note("Prefab WiRR jest punktem odniesienia. Najbardziej wartościowa praca zaczyna się wtedy, gdy student potrafi wymienić dostawcę danych, rozszerzyć zachowanie i zmierzyć konsekwencje swojej decyzji projektowej.");
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

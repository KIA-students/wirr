using System;
using System.Collections.Generic;

namespace KIA.WiRR.Editor
{
    internal sealed class WiRRLabTask
    {
        public string Checkpoint { get; }
        public string Title { get; }
        public string Goal { get; }
        public IReadOnlyList<string> Steps { get; }
        public string Evidence { get; }

        public WiRRLabTask(string checkpoint, string title, string goal, string[] steps, string evidence)
        {
            Checkpoint = checkpoint;
            Title = title;
            Goal = goal;
            Steps = steps ?? Array.Empty<string>();
            Evidence = evidence;
        }
    }

    internal static class WiRRLabTaskCatalog
    {
        private static readonly WiRRLabTask[][] Tasks =
        {
            new[]
            {
                new WiRRLabTask("3.0", "Pomiar bazowy PC", "Wyznacz punkt odniesienia dla dalszych eksperymentów wydajnościowych.",
                    new[]
                    {
                        "Uruchom przygotowaną scenę bez dodatkowego obciążenia i sprawdź, czy sonda metryk raportuje stabilne FPS oraz czas klatki.",
                        "Wykonaj trzy próby bazowe i zapisz FPS oraz ms/klatkę; do porównań używaj mediany, a nie pojedynczego odczytu.",
                        "Wykonaj wariant eksperymentalny v1 z formularza raportu, zmieniając tylko wskazany czynnik i pozostawiając resztę konfiguracji bez zmian.",
                        "Porównaj warunek kontrolny i eksperymentalny oraz zapisz krótki wniosek o tym, czy obserwowana różnica jest większa od naturalnego rozrzutu pomiarów."
                    },
                    "Dowód: trzy pomiary baseline, seria kontrolna v1 i wniosek zapisany w sekcji 3.0 raportu."),
                new WiRRLabTask("3.5", "Obciążenie CPU i GC", "Rozdziel wpływ pracy CPU od kosztu alokacji pamięci.",
                    new[]
                    {
                        "Dodaj generator obciążenia CPU i wykonaj serię CPU-A dla kolejnych wartości iterationsPerFrame: 0, 10000, 25000, 50000, 100000, 200000 i 400000.",
                        "Wykonaj serię CPU-B dla blocksPerFrame: 1, 2, 4, 8, 16 i 32; dla każdego punktu wykonaj trzy próby.",
                        "Wykonaj serię CPU-C dla allocationBytesPerFrame: 0, 1024, 4096, 16384, 65536 i 262144; obserwuj GC Alloc oraz wystąpienie kolekcji pamięci.",
                        "Dla reprezentatywnego wariantu zapisz medianę FPS, czas Main Thread i alokację GC, a następnie wskaż, który mechanizm ogranicza wydajność."
                    },
                    "Dowód: uzupełnione serie CPU-A/B/C oraz wniosek w sekcji 3.5 raportu."),
                new WiRRLabTask("4.0", "Koszt renderowania", "Sprawdź, które parametry sceny zwiększają koszt CPU/GPU renderowania.",
                    new[]
                    {
                        "Zwiększaj liczbę widocznych obiektów zgodnie z serią GPU-A: 0, 100, 250, 500, 1000, 2000 i 4000; zapisuj FPS, Batches, SetPass i Triangles.",
                        "Zmień liczbę materiałów w serii GPU-B: 1, 2, 4, 8, 16 i 32, nie zmieniając geometrii bardziej niż jest to konieczne.",
                        "Wykonaj serię Render Scale: 0.60, 0.70, 0.80, 0.90, 1.00 i 1.20; dla każdego ustawienia wykonaj trzy pomiary.",
                        "Porównaj zmiany czasu klatki z liczbą partii renderowania, zmian materiału i liczbą trójkątów; wskaż dominujące wąskie gardło."
                    },
                    "Dowód: tabele GPU-A/B/D, zrzut Profilera i wniosek w sekcji 4.0 raportu."),
                new WiRRLabTask("4.5", "Koszt symulacji fizyki", "Zmierz wpływ liczby ciał i częstotliwości kroku fizyki na czas klatki.",
                    new[]
                    {
                        "W serii PHY-A zwiększaj liczbę dynamicznych Rigidbody: 0, 25, 50, 100, 200, 400 i 800; po każdej zmianie ustabilizuj scenę i wykonaj trzy próby.",
                        "Zapisuj medianę FPS, czas klatki i Physics.Processing; nie zmieniaj równocześnie ustawień renderowania.",
                        "W serii PHY-C ustaw Fixed Timestep kolejno na 0.0333, 0.0250, 0.0200, 0.0167, 0.0133 i 0.0111 s i zanotuj liczbę kroków fizyki na sekundę.",
                        "Wyjaśnij kompromis między częstotliwością symulacji, stabilnością ruchu i kosztem obliczeniowym."
                    },
                    "Dowód: serie PHY-A/C i wniosek w sekcji 4.5 raportu."),
                new WiRRLabTask("5.0", "Meta Quest 3 i transfer wyniku", "Sprawdź, czy wnioski z PC pozostają prawdziwe na urządzeniu standalone.",
                    new[]
                    {
                        "Zweryfikuj na Meta Horizon Link obrót i translację głowy, oba kontrolery oraz akcję Select; traktuj Link wyłącznie jako test funkcjonalny.",
                        "Zbuduj i uruchom aplikację jako standalone na Quest 3, bez aktywnego Link.",
                        "Sprawdź działanie 6DoF, kontrolerów i sceny, a następnie wykonaj trzy pomiary standalone: FPS i ms/klatkę.",
                        "W tych samych możliwie zbliżonych warunkach wykonaj trzy pomiary na PC; zapisz obie serie w tabeli PC/Quest, policz mediany i porównaj kierunek oraz skalę efektu."
                    },
                    "Dowód: test funkcjonalny Quest, pomiary standalone i porównanie PC z Quest w sekcji 5.0.")
            },
            new[]
            {
                new WiRRLabTask("3.0", "Chwyt, socket i ray", "Zbuduj trzy podstawowe mechanizmy interakcji i zmierz ich skuteczność.",
                    new[]
                    {
                        "Skonfiguruj XR Origin, kontrolery oraz Input Actions, a następnie utwórz co najmniej jeden obiekt chwytalny z XR Grab Interactable.",
                        "Dodaj gniazdo XR Socket Interactor i ustaw warstwy interakcji tak, aby akceptowało wyłącznie właściwy obiekt.",
                        "Skonfiguruj XR Ray Interactor do wskazywania celu i sprawdź, czy bezpośredni chwyt, osadzenie w gnieździe i interakcja promieniem działają niezależnie.",
                        "Wykonaj po pięć prób w warunkach A, B i C z raportu; zapisuj czas wykonania oraz liczbę błędów."
                    },
                    "Dowód: trzy działające formy interakcji oraz tabela czasu i błędów dla warunków A/B/C."),
                new WiRRLabTask("3.5", "UI i informacja zwrotna", "Sprawdź, czy użytkownik jednoznacznie rozpoznaje stan interakcji.",
                    new[]
                    {
                        "Utwórz panel World Space i skonfiguruj obsługę UI dla XR tak, aby elementy można było aktywować promieniem.",
                        "Dodaj co najmniej dwa rodzaje informacji zwrotnej: wizualną, dźwiękową lub haptyczną; haptyka ma być wyzwalana przez jawne żądanie.",
                        "Sprawdź stan spoczynkowy, hover, select i zakończenie zadania; usuń sytuacje, w których jedno działanie powoduje wielokrotną aktywację.",
                        "Wykonaj po pięć prób A/B/C i zanotuj czas oraz liczbę błędnych lub powtórzonych aktywacji."
                    },
                    "Dowód: działające UI/feedback, informacja o haptyce i pomiary w sekcji 3.5."),
                new WiRRLabTask("4.0", "Lokomocja i komfort", "Skonfiguruj lokomocję tak, aby była przewidywalna i ograniczała błędne teleportacje.",
                    new[]
                    {
                        "Dodaj teleportację do dozwolonych powierzchni i osobno oznacz powierzchnie, na które teleportacja ma być odrzucana.",
                        "Sprawdź poprawność reticle, orientacji po teleportacji i kolizji użytkownika z otoczeniem.",
                        "Wykonaj po pięć prób warunków A/B/C; zapisuj czas, korekty ruchu i nieudane próby teleportacji.",
                        "Na podstawie obserwacji wskaż, które elementy konfiguracji poprawiają przewidywalność ruchu i komfort."
                    },
                    "Dowód: poprawne ograniczenie teleportacji, tabela A/B/C i wniosek w sekcji 4.0."),
                new WiRRLabTask("4.5", "Scenariusz integracyjny i diagnostyka", "Połącz interakcję, UI i lokomocję w jeden mierzalny scenariusz.",
                    new[]
                    {
                        "Zdefiniuj scenariusz obejmujący przemieszczenie, wskazanie, chwyt i osadzenie obiektu oraz jedno działanie na UI.",
                        "Wykonaj pięć prób scenariusza bazowego i zapisuj czas, upuszczenia, błędne aktywacje, nieudane teleportacje oraz błędy resetu/osadzenia.",
                        "Wprowadź kontrolowany problem zgodny z wariantem v4, zapisz dwie konkurencyjne hipotezy H1 i H2 oraz wybierz test rozstrzygający.",
                        "Wprowadź minimalną poprawkę, powtórz scenariusz i sprawdź, czy usunięto objaw bez tworzenia nowej regresji."
                    },
                    "Dowód: seria bazowa, H1/H2, poprawka i wynik po poprawce w sekcji 4.5."),
                new WiRRLabTask("5.0", "Quest 3 i SSQ", "Zweryfikuj interakcję na sprzęcie oraz opisz komfort wyłącznie na podstawie tej sesji.",
                    new[]
                    {
                        "Przed ekspozycją na Quest 3 wypełnij SSQ PRE dla obu osób zgodnie z formularzem kursu.",
                        "Uruchom scenariusz na Quest 3 i zweryfikuj śledzenie, kontrolery, chwyt, ray, UI i teleportację na urządzeniu.",
                        "Wykonaj eksperyment A/B/C na wskazanej osobie; dla każdego warunku wykonaj trzy próby, zapisz medianę czasu, błędy/korekty i komfort 0-10.",
                        "Po ekspozycji wypełnij SSQ POST i porównaj PRE z POST, formułując wyłącznie ostrożny wniosek dotyczący tej konkretnej sesji."
                    },
                    "Dowód: SSQ PRE/POST, wyniki eksperymentu Quest i wniosek w sekcji 5.0.")
            },
            new[]
            {
                new WiRRLabTask("3.0", "Tracking i płaszczyzny", "Sprawdź, jak szybko i w jakich warunkach ARCore uzyskuje użyteczny tracking.",
                    new[]
                    {
                        "Skonfiguruj AR Session, XR Origin dla AR oraz wykrywanie płaszczyzn i uruchom aplikację na obsługiwanym urządzeniu.",
                        "Zmierz osobno czas od startu do SessionTracking oraz czas do wykrycia pierwszej użytecznej płaszczyzny.",
                        "Wykonaj trzy powtórzenia dla każdego warunku A/B/C; zapisuj oba czasy dla każdej próby oraz notTrackingReason, jeżeli tracking nie jest dostępny.",
                        "Opisz warunki otoczenia i najważniejsze ograniczenie pomiaru, tak aby wynik można było odtworzyć."
                    },
                    "Dowód: czasy trackingu i płaszczyzny dla A/B/C oraz opis notTrackingReason."),
                new WiRRLabTask("3.5", "Raycast i placement 1:1", "Umieszczaj obiekt na rzeczywistej powierzchni bez zmiany jego skali.",
                    new[]
                    {
                        "Przygotuj model o znanych wymiarach i ustaw jego skalę w Unity tak, aby odpowiadała rzeczywistym jednostkom metrycznym 1:1.",
                        "Wykonuj raycast z pozycji wskazania ekranu do wykrytej płaszczyzny i umieszczaj model wyłącznie po poprawnym trafieniu.",
                        "Dla warunków A/B/C wykonaj po 10 prób i policz liczbę poprawnych trafień oraz medianę błędu pozycjonowania.",
                        "Sprawdź, czy zmiana skali modelu nie jest używana do maskowania błędu rejestracji."
                    },
                    "Dowód: 10 prób na warunek, błąd pozycjonowania i potwierdzenie skali 1:1."),
                new WiRRLabTask("4.0", "Kotwica i dryf", "Zmierz stabilność obiektu zakotwiczonego po ruchu użytkownika.",
                    new[]
                    {
                        "Utwórz kotwicę w punkcie referencyjnym i zapisz jej początkowe położenie względem fizycznego znacznika.",
                        "Przejdź z urządzeniem zadaną ścieżkę, wróć do punktu startowego i ponownie zmierz odchylenie obiektu od znacznika.",
                        "Wykonaj trzy powtórzenia bazowe oraz serię A/B/C; zapisuj błąd końcowy, stan trackingu i notTrackingReason.",
                        "Wyznacz medianę dryfu i wskaż, czy błąd jest systematyczny, losowy czy związany z utratą trackingu."
                    },
                    "Dowód: trzy powroty, eksperyment A/B/C i mediana dryfu w sekcji 4.0."),
                new WiRRLabTask("4.5", "Rejestracja dwupunktowa", "Oceń błąd dopasowania wirtualnego układu do dwóch punktów rzeczywistych.",
                    new[]
                    {
                        "Wyznacz fizyczne punkty O i X oraz zmierz odległość OX niezależną metodą referencyjną.",
                        "Zarejestruj układ wirtualny na podstawie O i X, a następnie zmierz błędy e1, e2 i e3 w punktach kontrolnych.",
                        "Wykonaj trzy pełne rejestracje od początku; dla każdej policz e_med i e_max.",
                        "Wskaż dominujące źródło niepewności i zaproponuj jedną zmianę procedury, która mogłaby je ograniczyć."
                    },
                    "Dowód: OX, trzy rejestracje, e_med/e_max i opis niepewności."),
                new WiRRLabTask("5.0", "Kontrolowany błąd rejestracji", "Rozpoznaj charakterystyczną sygnaturę błędu i potwierdź diagnozę pomiarem.",
                    new[]
                    {
                        "Wykonaj trzy pomiary baseline i zachowaj ich wartości jako punkt odniesienia.",
                        "Wprowadź kontrolowany błąd z wariantu v5, nie zmieniając innych elementów konfiguracji.",
                        "Sformułuj hipotezy H1 i H2, wykonaj test rozstrzygający i trzy pomiary w stanie z błędem.",
                        "Usuń przyczynę minimalną poprawką, wykonaj trzy pomiary po naprawie i sprawdź, czy e_med wrócił w pobliże baseline."
                    },
                    "Dowód: baseline, fault, repaired po trzy próby oraz diagnoza H1/H2.")
            },
            new[]
            {
                new WiRRLabTask("3.0", "Depth API i diagnostyka", "Sprawdź, czy urządzenie dostarcza użyteczne dane głębi i w jakim trybie.",
                    new[]
                    {
                        "Skonfiguruj AR Occlusion Manager i odczytaj dostępność danych Depth, bieżący tryb oraz rozdzielczość ostatniej klatki głębi.",
                        "Przygotuj jednoznaczny test sukcesu, np. procent poprawnych próbek lub trafień w zdefiniowanym obszarze.",
                        "Wykonaj eksperyment v1 w warunkach A/B/C po trzy powtórzenia i zapisz successRate oraz parametry strumienia.",
                        "Jeżeli Depth jest niedostępny, zapisz ten stan jako wynik zamiast zastępować go sztucznym zerem."
                    },
                    "Dowód: stan subsystemu, trzy serie A/B/C i ograniczenie pomiaru w sekcji 3.0."),
                new WiRRLabTask("3.5", "Okluzja środowiskowa", "Porównaj jakość zasłaniania obiektów wirtualnych przez geometrię rzeczywistą.",
                    new[]
                    {
                        "Umieść obiekt wirtualny częściowo za rzeczywistą przeszkodą i wykonaj test bazowy dla trybów Off, Raw i Smoothed.",
                        "Dla każdego trybu wykonaj 10 obserwacji i oznacz błędy: przeciek modelu, nadmierne wycięcie, migotanie krawędzi, opóźnienie lub inny objaw.",
                        "Wykonaj wariant v2 w warunkach A/B/C; zapisz requested depth, current depth, liczbę błędów i dominującą sygnaturę.",
                        "Porównaj Raw i Smoothed, wskazując kompromis między stabilnością krawędzi a opóźnieniem."
                    },
                    "Dowód: tabela Off/Raw/Smoothed oraz eksperyment v2 z błędami /10."),
                new WiRRLabTask("4.0", "Depth-raycast", "Zmierz błąd odległości tylko dla poprawnych trafień w danych głębi.",
                    new[]
                    {
                        "Ustaw powierzchnię referencyjną kolejno w odległości 0.50 m, 1.00 m i 1.50 m od urządzenia.",
                        "Dla każdej odległości wykonaj pięć prób i oznacz każdą jako HIT albo MISS.",
                        "Dla HIT policz e_d jako różnicę między odległością referencyjną a wynikiem AR; MISS pozostaw jako brak wyniku, a nie błąd 0 m.",
                        "Wykonaj wariant v3 A/B/C i porównaj liczbę HIT/MISS oraz medianę błędu dla poprawnych trafień."
                    },
                    "Dowód: 15 prób bazowych, wyniki v3 oraz mediana błędu wyłącznie dla HIT."),
                new WiRRLabTask("4.5", "Estymacja oświetlenia", "Sprawdź, które parametry oświetlenia są dostępne i jak reagują na zmianę sceny.",
                    new[]
                    {
                        "Odczytaj bieżący tryb estymacji oraz dostępność brightness, color temperature i main light direction.",
                        "Wykonaj pomiary w pięciu warunkach: jasne rozproszone, słabsze rozproszone, źródło boczne, światło od przodu i oświetlenie mieszane.",
                        "Dla każdego warunku zanotuj medianę brightness, temperaturę barwową lub NA, kierunek światła lub NA oraz ocenę zgodności wizualnej 1-5.",
                        "Nie zastępuj niedostępnej wartości API zerem; opisz brak danych jako ograniczenie platformy lub trybu."
                    },
                    "Dowód: pięć warunków oświetlenia i opis dostępności danych API."),
                new WiRRLabTask("5.0", "Kontrolowany błąd MR", "Rozróżnij co najmniej dwie możliwe przyczyny błędu na podstawie pomiaru.",
                    new[]
                    {
                        "Wykonaj serię baseline i zapisz mierzalny wynik używany później do porównania.",
                        "Wprowadź kontrolowany błąd z wariantu v5 i sformułuj dwie konkurencyjne hipotezy H1 i H2.",
                        "Wybierz test, którego wynik rozróżnia H1 od H2, wykonaj trzy powtórzenia i zapisz obserwacje.",
                        "Zastosuj minimalną poprawkę, powtórz trzy pomiary i potwierdź, że usunięto objaw bez zmiany niezależnych parametrów eksperymentu."
                    },
                    "Dowód: baseline, fault, repaired, H1/H2, test rozstrzygający i diagnoza.")
            },
            new[]
            {
                new WiRRLabTask("3.0", "Import i audyt modelu CAD", "Ustal, co zmienia się między modelem źródłowym a zasobem używanym w Unity.",
                    new[]
                    {
                        "Sprawdź obowiązkowy model Assets/WiRR/Lab05/Models/Source/makerbeam_bracket_90degree.stp, plik ATTRIBUTION.md i zachowaj STEP bez zmian.",
                        "Wykonaj tessellację STEP do FBX lub OBJ, zapisz użyte narzędzie, tolerancję, jednostki i osie; do Unity importuj powstały mesh, a nie traktuj standardowego Model Importera jako importera STEP.",
                        "Wykonaj audyt topologii: vertices, triangles, submeshes, material slots, boundary, non-manifold, degenerate oraz wskaźniki jakości q05/qMedian.",
                        "Porównaj FBX, OBJ i kontrolowany wariant STL pochodzące z tego samego źródła; wskaż różnice w rozmiarze, hierarchii, UV, materiałach oraz wymiarach."
                    },
                    "Dowód: audyt baseline i porównanie formatów/eksportu w sekcji 3.0."),
                new WiRRLabTask("3.5", "Triangulacja i kandydaci LOD", "Zredukuj złożoność bez utraty cech istotnych funkcjonalnie.",
                    new[]
                    {
                        "Przygotuj trzy warianty triangulacji lub redukcji A/B/C, zmieniając jeden główny parametr procesu.",
                        "Dla każdego wariantu zmierz vertices, triangles, boundary, non-manifold, degenerate, q05 i qMedian.",
                        "Oceń wizualnie, czy zachowano krawędzie, otwory i powierzchnie istotne dla rozpoznania lub interakcji.",
                        "Na podstawie wyników wybierz geometrie przeznaczone na LOD0, LOD1 i LOD2 oraz zapisz ich liczbę trójkątów."
                    },
                    "Dowód: porównanie A/B/C i uzasadniony zestaw LOD0/1/2."),
                new WiRRLabTask("4.0", "LOD i benchmark", "Zmierz, czy redukcja geometrii rzeczywiście poprawia koszt renderowania zasobu.",
                    new[]
                    {
                        "Skonfiguruj LOD Group dla LOD0, LOD1 i LOD2; sprawdź przejścia w odległości oraz brak znikania kluczowych elementów modelu.",
                        "Zmierz czas importu wybranego wariantu zasobu, a następnie wykonaj trzy próby benchmarku dla wariantów A, B i C w tym samym miejscu pomiaru: Editor albo Build.",
                        "Zapisuj czas importu, mean/median/p95 czasu klatki, FPS, triangles, renderers, material slots, CPU ms, GPU ms, batches i pamięć.",
                        "Porównaj redukcję geometrii z rzeczywistą zmianą kosztu klatki i wskaż, gdzie dalsze upraszczanie przestaje dawać proporcjonalny zysk."
                    },
                    "Dowód: geometrie LOD, konfiguracja LOD Group i trzyserie benchmarku."),
                new WiRRLabTask("4.5", "Materiały, draw calls, kolizje i geometria", "Usuń kosztowne lub błędne elementy zasobu, zachowując jego funkcję.",
                    new[]
                    {
                        "Porównaj warianty A/B/C pod względem liczby materiałów, draw calls/batches, kosztu CPU/GPU, pamięci oraz jakości wizualnej.",
                        "Sprawdź kolizje w co najmniej 20 próbach funkcjonalnych i zanotuj błędy kontaktu lub przenikania.",
                        "Zidentyfikuj jeden problem geometrii lub importu, określ mierzalną miarę błędu przed naprawą i zastosuj minimalną poprawkę.",
                        "Powtórz pomiar po naprawie i potwierdź, że poprawa nie zwiększyła niepotrzebnie kosztu renderowania."
                    },
                    "Dowód: porównanie materiałów/draw calls, testy kolizji oraz miara błędu przed i po naprawie."),
                new WiRRLabTask("5.0", "Potok finalny i kontrolowany błąd", "Wybierz powtarzalny pipeline przygotowania modelu do XR i uzasadnij jego ryzyka.",
                    new[]
                    {
                        "Wprowadź kontrolowany błąd v5, wykonaj pomiar baseline, fault i repaired, zapisując geometrię, hierarchię, czas klatki i błąd funkcjonalny.",
                        "Porównaj role STEP, FBX/OBJ/STL, USD oraz opisów robota URDF/MJCF; wskaż, gdzie znajduje się geometria, a gdzie hierarchia lub semantyka potrzebna do symulacji.",
                        "Wskaż najważniejsze ryzyko wybranego pipeline, np. skalę, triangulację, materiały, hierarchię, UV albo utratę semantyki CAD.",
                        "Wybierz finalny model XR i podsumuj kompromis między liczbą trójkątów, materiałami, wydajnością, pamięcią i jakością."
                    },
                    "Dowód: baseline/fault/repaired, wybór pipeline i tabela finalnego modelu XR.")
            },
            new[]
            {
                new WiRRLabTask("3.0", "Źródło stanu i przepływ danych", "Uruchom wiarygodne źródło JointState i potwierdź przepływ danych do Unity.",
                    new[]
                    {
                        "Wybierz topologię LOCAL, LAN albo WEBSIM i zapisz wersje komponentów oraz adres/port transportu używanego przez Unity.",
                        "Dla LOCAL/LAN uruchom robota w Gazebo i ROS 2; dla WEBSIM uruchom backend, ustaw kod sesji oraz utwórz model z panelu WiRR.",
                        "Wyślij co najmniej trzy polecenia ruchu i sprawdź, czy wartości joint_states zmieniają się w źródle oraz w modelu Unity.",
                        "Zmierz częstotliwość wiadomości JointState i opisz faktyczny przepływ: źródło symulacji → ROS 2/rosbridge → Unity."
                    },
                    "Dowód: konfiguracja środowiska, trzy próby ruchu, częstotliwość JointState i opis przepływu."),
                new WiRRLabTask("3.5", "Mapowanie JointState na model Unity", "Odwzoruj stan robota po nazwach przegubów, a nie po kolejności w tablicy.",
                    new[]
                    {
                        "Dla każdego przegubu zapisz obiekt Unity, typ ruchu, oś lokalną, sign, offset i jednostkę.",
                        "Mapuj po polu name wiadomości JointState i sprawdź odporność na inną kolejność elementów w tablicy position.",
                        "Ustaw trzy znane pozycje robota i dla każdej zapisz kąty joint1, joint2 i joint3 zarówno w źródle, jak i w Unity.",
                        "Dla każdej z trzech pozycji policz błąd w stopniach dla każdego przegubu, a następnie wyjaśnij ewentualny znak, offset lub różnicę konwencji osi."
                    },
                    "Dowód: tabela mapowania i błędy przegubów w sekcji 3.5."),
                new WiRRLabTask("4.0", "Transport, jitter i bufor interpolacji", "Zmierz regularność danych i wpływ bufora na płynność oraz opóźnienie.",
                    new[]
                    {
                        "Zarejestruj pierwsze 30 odstępów inter-arrival między kolejnymi wiadomościami JointState i policz ich zmienność.",
                        "Wykonaj warianty bufora A/B/C, zapisując częstotliwość źródła, delay, parametry bufora, średni odstęp i jego odchylenie oraz subiektywną płynność.",
                        "Wykonaj co najmniej 10 pomiarów RTT Unity ↔ backend i wyznacz medianę opóźnienia.",
                        "Wybierz ustawienie bufora, które ogranicza jitter bez wprowadzania nieuzasadnionego opóźnienia, i uzasadnij wybór danymi."
                    },
                    "Dowód: 30 inter-arrival, warianty bufora, 10 próbek RTT i wniosek transportowy."),
                new WiRRLabTask("4.5", "LIVE, STALE i odzyskanie danych", "Sprawdź, czy system jawnie sygnalizuje utratę aktualnego stanu robota.",
                    new[]
                    {
                        "Zapisz próg STALE i częstotliwość joint_states podczas normalnej pracy.",
                        "Zatrzymaj źródło danych na kontrolowany czas i zmierz opóźnienie od ostatniej wiadomości do wykrycia STALE.",
                        "Wznow źródło danych i zmierz czas powrotu do LIVE; powtórz test dla wariantu sieciowego v4 lub dwóch warunków A/B.",
                        "Sformułuj H1 i H2 dla dominującego źródła opóźnienia lub utraty danych i wykonaj test, który pozwala je rozróżnić."
                    },
                    "Dowód: czasy STALE/LIVE, porównanie RTT i diagnoza H1/H2."),
                new WiRRLabTask("5.0", "Kontrolowany błąd bliźniaka", "Potwierdź, że system wykrywa błąd synchronizacji i wraca do wiarygodnego stanu.",
                    new[]
                    {
                        "Zapisz baseline: stan robota, wartości przegubów, częstotliwość danych i RTT.",
                        "Wprowadź kontrolowany błąd v5, np. błędne mapowanie, zatrzymanie danych albo zmianę parametru transportu, zgodnie z wariantem.",
                        "Zapisz objaw, H1, H2, test diagnostyczny i minimalną poprawkę; po naprawie wykonaj ponowny pomiar.",
                        "Wyznacz maksymalny błąd synchronizacji i potwierdź, że przejście STALE → LIVE działa poprawnie po przywróceniu danych."
                    },
                    "Dowód: baseline/fault/repaired, błąd synchronizacji i poprawny powrót do LIVE.")
            },
            new[]
            {
                new WiRRLabTask("3.0", "Kryteria akceptacji i smoke test", "Zdefiniuj warunki zaliczenia systemu przed rozpoczęciem testów.",
                    new[]
                    {
                        "Przed uruchomieniem pomiarów zapisz kryteria AC-01…AC-06 dla uruchomienia, funkcjonalności, wydajności, stabilności, użyteczności i bezpieczeństwa oraz oznacz kryteria blokujące.",
                        "Wykonaj pięć testów dymnych SM-01…SM-05; dla każdego zapisz wynik oczekiwany, rzeczywisty, status PASS/FAIL/NV i dowód.",
                        "Przejdź po funkcjach reprezentujących Lab 1–6 i uzupełnij macierz: procedura, wynik oczekiwany, wynik rzeczywisty, status i dowód.",
                        "Nie oznaczaj niewykonanego testu jako PASS; użyj NV i zapisz przyczynę."
                    },
                    "Dowód: kryteria zapisane przed testami, smoke test i macierz funkcjonalna Lab 1–6."),
                new WiRRLabTask("3.5", "Wydajność i stabilność", "Sprawdź zachowanie systemu w bezczynności, pracy nominalnej i pod obciążeniem.",
                    new[]
                    {
                        "Zdefiniuj trzy stany: bezczynność, nominalny i obciążeniowy, z jednoznacznym opisem tego, co dzieje się w scenie.",
                        "W każdym stanie wykonaj porównywalny pomiar i zapisz medianę FPS, ms/klatkę, CPU ms, GPU ms oraz pamięć.",
                        "Jeżeli system korzysta z transmisji sieciowej, zanotuj warunki transportu i sprawdź, czy nie są źródłem obserwowanych skoków czasu klatki.",
                        "Porównaj wynik z wcześniej zdefiniowanymi kryteriami akceptacji i oznacz każde niespełnione kryterium."
                    },
                    "Dowód: trzy warunki wydajnościowe i wniosek powiązany z kryteriami akceptacji."),
                new WiRRLabTask("4.0", "Użyteczność i dostępność", "Zmierz, czy użytkownik może poprawnie wykonać kluczowe zadania bez nadmiernej pomocy.",
                    new[]
                    {
                        "Zdefiniuj trzy zadania użytkownika U1, U2 i U3 z jednoznacznym kryterium sukcesu.",
                        "Dla każdego zadania zapisz sukces, czas lub liczbę prób, błędne aktywacje i liczbę podpowiedzi.",
                        "Zidentyfikuj co najmniej jeden problem dostępności albo ograniczenie interfejsu i opisz jego wpływ na wykonanie zadania.",
                        "Policz skuteczność i porównaj ją z kryterium akceptacji bez zastępowania brakujących danych wynikiem pozytywnym."
                    },
                    "Dowód: U1-U3, skuteczność, błędne aktywacje, pomoc i problem dostępności."),
                new WiRRLabTask("4.5", "Awaria kontrolowana i regresja", "Udowodnij, że potrafisz znaleźć przyczynę usterki i sprawdzić skutki poprawki.",
                    new[]
                    {
                        "Wprowadź kontrolowaną awarię v4 i zmierz stan przed poprawką, w tym czas potrzebny do przywrócenia działania.",
                        "Zapisz H1 i H2, wybierz test rozstrzygający i na jego podstawie wskaż minimalną poprawkę.",
                        "Po naprawie uruchom test REG-01 bezpośrednio sprawdzający usuniętą usterkę oraz REG-02 sprawdzający funkcję, która mogła zostać naruszona ubocznie.",
                        "Dla obu testów zapisz stan przed i po poprawce oraz końcowy status; brak dowodu oznacz jako NV."
                    },
                    "Dowód: kontrolowany FAIL, diagnoza H1/H2, minimalna poprawka i dwa testy regresyjne."),
                new WiRRLabTask("5.0", "Automatyzacja, ryzyko i decyzja", "Zakończ walidację powtarzalnym testem i decyzją wynikającą z jawnych danych.",
                    new[]
                    {
                        "Zautomatyzuj co najmniej jeden test przy użyciu Unity Test Framework i pokaż zarówno poprawny PASS, jak i kontrolowany FAIL.",
                        "Utwórz trzy pozycje ryzyka R-01…R-03; dla każdej określ prawdopodobieństwo P, skutek S, policz R=P×S i zapisz działanie ograniczające.",
                        "Po działaniu ograniczającym opisz ryzyko resztkowe oraz wskaż najwyższy pozostały priorytet ryzyka.",
                        "Wybierz ACCEPT, ACCEPT WITH CONDITIONS albo REJECT wyłącznie na podstawie wcześniej zapisanych kryteriów blokujących, wyników testów i ryzyka; uzasadnij decyzję dowodami."
                    },
                    "Dowód: test automatyczny z PASS/FAIL, macierz ryzyka i uzasadniona decyzja akceptacyjna.")
            }
        };

        public static IReadOnlyList<WiRRLabTask> Get(int labNumber)
        {
            if (labNumber < 1 || labNumber > Tasks.Length)
                throw new ArgumentOutOfRangeException(nameof(labNumber));

            return Tasks[labNumber - 1];
        }
    }
}

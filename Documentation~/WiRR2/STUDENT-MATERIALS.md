# Materiały Unity do nowych instrukcji

Wersja `student-kit/0.1.7`, 2026-10-16. **Zestaw pilotażowy do odbioru w Unity**.
Nie jest jeszcze pełnym wdrożeniem wszystkich 40 checkpointów ani zatwierdzonym
wydaniem na stanowiska studentów. Sceny i źródła są częścią istniejącego pakietu,
bez drugiej wtyczki i bez dodatkowych zależności runtime.

## Przygotowanie stanowiska przez prowadzącego

1. Otwórz wzorcowy projekt URP z pakietem WiRR i Input System.
2. Wybierz `WiRR → Nowe instrukcje → Przygotuj osiem scen (prowadzący)`.
   Narzędzie kopiuje brakujące sceny do `Assets/WiRR/WiRR2` i zachowuje istniejące
   pliki. Nie modyfikuje wcześniejszych scen laboratoriów.
3. Otwórz scenę przez `WiRR → Nowe instrukcje → Materiały studenta` i uruchom Play.
   Ten panel jest też dostępny z istniejącego okna czterech bloków.
4. Przeprowadź testy poniżej, następnie przygotuj projekt na Ubuntu i Windows.
   Dopiero po odbiorze udostępnij go studentom. Student otrzymuje przygotowany
   projekt, nie polecenie instalowania pakietów lub generowania scen.

W Samples Lab01–Lab08 są również gotowe pliki `.unity` w podkatalogu `WiRR2`.
Zawierają komponent startowy, który tworzy lekką geometrię i interfejs w Play.
Brakujące zasoby lub shader są błędem, nie powodem do wpisywania fikcyjnych pomiarów.
Nie wykonano tutaj buildów Player/APK ani przygotowania lockfile nowego wydania.

## Zawartość i ograniczenia względem PDF

| Lab / scena | Dostarczone materiały i mechanika | Pozostały zakres odbioru/rozbudowy |
|---|---|---|
| 01 `Lab01_Station` | Referencje 0,1/1/10 m, 10/100/300 brył, cienie, fizyka, stała trasa, 2 s rozgrzewki i 8 s próby, mediana/p95 rzeczywistych czasów klatki | Odbiór budżetu stanowiska, kontrolowane profile CPU/GPU, pilotaż |
| 02 `Lab02_Interaction` | Obiekt i gniazdo, Direct PC klawiaturą i Ray PC myszą, serie pięciu cykli z czasami i błędami, trening oddzielony od danych, podgląd teleportacji PC poza pomiarem | Pełne XRI Direct/Ray/Socket i teleportacja XRI/HMD; obecny PC to własny symulator dydaktyczny |
| 03 `Lab03_AnchorGallery` | Referencje co 0,30 m, umieszczenie 1:1, dwa profile SIM_AR, wspólna trasa trzech punktów, wektor błędu, trzy odczyty przed i trzy po relokalizacji, wariant braku odzyskania | Odbiór UI/PlayMode na Ubuntu i Windows oraz opcjonalny adapter REAL_AR |
| 04 `Lab04_DepthRoom` | Trzy obiekty i przeszkoda, niezależna referencja geometrii, błędna/poprawna okluzja, regulowana tolerancja głębi, jasne/ciemne światło, mylące etykiety, błędny komunikat promienia, dwa widoki i osobne fałszywe alarmy/pominięcia | Odbiór PlayMode/renderowania na stanowiskach, opcjonalne dane z providerów urządzeniowych |
| 05 `Lab05_CADWorkshop` | Proceduralne siatki High/Medium z rzeczywistym otworem i Low bez otworu; wariant stały lub automatyczny według odległości, dwa regulowane progi w metrach, wspólna radialna trasa benchmarku, 1/4 identycznych materiałów, widok kontrolny, statystyki geometrii z całego pomiaru | Collidery, osobne gotowe modele konwersji CAD i odbiór renderowania; obecny pierścień jest modelem dydaktycznym |
| 06 `Lab06_DigitalTwin` | Model dwóch przegubów, nazwane kolumny JointState w radianach, syntetyczny replay 30 s, przerwy 1,5/3 s, watchdog 0,25/0,5/1 s, wiek odbiornika, odstępy wiadomości, osobne zdarzenia STALE/powrotu, zamrożenie albo ukrycie modelu | Bufor, jitter transportu, opcjonalny ROS_LIVE; brak pomiaru RTT |
| 07 `Lab07_Expedition` | Wybór celów PC, przejścia punktowe PC, jednokrotna klasyfikacja świeżości; rozmiar celu, podświetlenie następnego kroku, kontrolowany drugi dystans, błędy/czas każdego kroku i limit 90 s | Rzeczywiste chwytanie, swobodna nawigacja XR i testy użyteczności z uczestnikami |
| 08 `Lab08_ExperienceStudio` | Trenażer sekwencji, inspekcja trzech celów, test stanu SIM; moduły podświetlenia i rozmiaru celu, osoba/gość symulowany, ręczne zakończenie pokazu 60–90 s | Bogatsze demonstratory AR/REPLAY i scenariusze ograniczeń, testy z użytkownikami |

Lab08 rozwija sprawdzony w repo wzorzec istniejącego `WiRRLab08Experiment`:
lokalne działania, raycast desktop, czas i błędy. Starego demonstratora i jego
eksportu nie nadpisano. Nowy kontroler współdzieli logikę zapisu między ośmioma
laboratoriami zamiast powielać osiem odrębnych eksporterów.

## Dane i raport

- Polecenia, pytania, warunki i pięć opisów progów pochodzą z tych samych YAML
  i `07-PROGI-OCEN.md` co instrukcje `2.0.0-draft.2`. Są dostępne offline.
  Przełącznik progu służy przeczytaniu celu, nie potwierdza zaliczenia.
- Profil obejmuje 1–2 indeksy i G1/G2. Jedna sesja ma dwa niezależne wyniki;
  otwarcie drugiego laboratorium w bloku nie wymaga ukończenia pierwszego.
  Po restarcie wpisz ten sam profil i otwórz ten sam blok, aby wznowić zapis.
- Sesje są izolowane kluczem indeksów/grupy/bloku w
  `Application.persistentDataPath/WiRR/sessions`. Lokalny plik jest zapisywany
  przez plik tymczasowy i zamianę z kopią `.bak`. Uszkodzony plik nie jest
  automatycznie nadpisywany. Ochrona danych między kontami OS pozostaje zadaniem
  konfiguracji stanowiska; skrót nazwy pliku nie jest szyfrowaniem.
- Zakończona albo przerwana próba pozostaje w historii. Trening jest jawnie
  oddzielony od porównania. Utrata fokusu/pauza przerywa próbę jako nieudaną.
  Rozpoczęcie próby jest zapisywane przed uruchomieniem pomiaru. Po awarii
  procesu wznowienie zachowuje jej identyfikator, konfigurację i czas startu;
  wynik ma status FAILED, a niezmierzony czas zakończenia pozostaje `null`.
  Odzyskanie nie powiela próby i nie odtwarza fikcyjnych pomiarów cząstkowych.
- Eksport tworzy dwa `WiRR-labNN.json` i indeks `block.json` w
  `Application.persistentDataPath/WiRR/exports/<sessionId>`.
  Wynik ma schemat `wirr-report/2.0-draft.1` opisany w
  [kontrakcie integralności](EVIDENCE-CONTRACT.md), z jawnym źródłem,
  jednostkami, warunkami, wersjami, brakami `null` i czasem UTC.
- `GUEST_HUMAN` w UI jest mapowane na `participant_mode=HUMAN` kontraktu draft.1;
  `GUEST_SIM` pozostaje odrębne. Scenariusz GUEST_SIM wykonuje trzy działania
  co sekundę. To jawny test funkcjonalny, nie model czasu reakcji człowieka.
- `checkpointEvidence` jest na tym etapie puste. Nie ma automatycznej propozycji
  oceny: `proposed_grade=null`, `achieved_checkpoints=[]`, `PROVISIONAL`.
  Syntetyczna próba kotwicy lub replay nie staje się pomiarem sprzętu.
- Eksport ma `deliveryStatus=LOCAL_ONLY`. Domyślna wysyłka wymaga połączenia
  z istniejącym systemem odbioru i rzeczywistego potwierdzenia. Obecny moduł
  nigdy nie deklaruje `SENT`. Nie podłącza się sam do sieci.

## Weryfikacja techniczna

W tym środowisku wykonano: 8 kontroli pakowania/danych i składni C# tree-sitter,
14 regresji istniejącego gradera oraz Ruff narzędzi Python. Zweryfikowano też
zgodność dziewięciu źródeł instrukcji/rubryki z przypiętą wersją źródłową.
**Nie wykonano kompilacji C#, importu Unity, renderowania Game View, testów
EditMode/PlayMode ani odbioru Windows/Ubuntu**. Analiza składni nie zastępuje tych testów.

Dołączone testy Unity: 63 przypadki EditMode (m.in. JSON, zapis z kopią,
rozdzielenie wyników, percentyle, nazwane mapowanie replay, geometria siatek
i import wszystkich scen, granice progów LOD i odzyskanie próby po serializacji)
oraz osiem testów PlayMode: start ośmiu doświadczeń, zmiana siatki LOD z powrotem do High,
teleportacja PC, kotwica oraz geometria głębi z dwóch widoków, niezależna od światła i etykiet, oraz niezmienność geometrii przy zmianie materiałów CAD i reakcja modelu REPLAY na brak danych/STALE/powrót oraz stałość celów w kontrpróbie Lab07.
W projekcie testowym dodaj `pl.prz.kia.wirr` do `testables` w `Packages/manifest.json`.
Testy uruchamiaj lokalnie albo w zatwierdzonym workflow na `main`, zgodnie
z polityką repozytorium. Nie dodano automatycznych runów na PR.

```sh
python -m unittest discover -s tools/wirr2 -p 'test_student_materials.py' -v
python -m unittest discover -s scripts~ -p 'test_wirr*.py' -v
python -m ruff check tools/wirr2
```

Kontrole hosta wymagają `PyYAML`, `tree-sitter`, `tree-sitter-c-sharp` i `ruff`.
Test C# emitujący rzeczywisty JSON zapisuje `student-kit-report.json` w katalogu
wyników testów NUnit. Ten plik sprawdź potem:

```sh
python scripts~/wirr_grade_report.py student-kit-report.json --output grading-result.json
```

Odbiór stanowiskowy obejmuje: start każdej sceny, wszystkie opisane przełączniki,
porównanie A/B, poprawny zapis i eksport, wznowienie profilu, drugi lab bloku,
brak sieci i HMD oraz zgodność wyświetlanych dowodów z PDF. Kolejną bramką
jest implementacja specyficznych reguł 40 checkpointów i pilotaż 90 minut.

## Odtworzenie zasobów

Narzędzie prowadzącego `tools/wirr2/build_student_materials.py --course-root <kurs>`
odtwarza JSON, sceny i deterministyczny syntetyczny replay. Suma wejść jest
w `tools/wirr2/instruction-inputs.json`. Dane replay pochodzą z funkcji
sinus/cosinus zapisanych w generatorze; nie przedstawiają rzeczywistego robota.
Edytowalne źródła siatek są w `WiRRStudentWorld.RingMesh`.

Źródła API: [LODGroup.ForceLOD](https://docs.unity.com/en-us/engine/6000.5/script-reference/unityengine/lodgroup/forcelod),
[font domyślny uGUI](https://github.com/Unity-Technologies/uGUI/blob/main/com.unity.ugui/Runtime/UGUI/UI/Core/Text.cs).
Sprawdzenie dokumentacji API nie potwierdza kompatybilności całego zestawu.

## Automatyczny LOD w tej wersji

Sterowanie używa jawnej polityki odległości od kamery do środka części:
poniżej pierwszego progu High, od pierwszego do drugiego Medium, od drugiego
Low. Wybrany poziom jest przekazywany do `LODGroup.ForceLOD`. To demonstrator
progów w **metrach**, a nie odtwarzanie wewnętrznego algorytmu Unity opartego na
względnej wysokości obrazu. Nie należy utożsamiać tych dwóch polityk.

Stały i automatyczny wariant mają tę samą trasę radialną 3–15 m i powrót,
z opcjonalnym przesunięciem dystansu o 4 m w kontrpróbie. Wynik obejmuje min/max
poziomu, liczbę przełączeń i zachowanie otworu we wszystkich próbkach okna
pomiarowego, a także liczbę trójkątów ostatnio wybranej siatki. Utrata otworu
choćby w części porównania nie jest ukrywana przez powrót do High na końcu.

## Pięć cykli Lab02

Rozpoczęcie próby uruchamia serię pięciu cykli wybranej techniki. Czas cyklu
będzie liczony od startu serii albo resetu po poprzednim cyklu. Błędny chwyt
lub upuszczenie zwiększa licznik pomyłek; poprawne osadzenie zamyka cykl.
Cykl można również zakończyć jawnie jako nieudany; limit 30 s automatycznie
kończy cykl niepowodzeniem. Każdy kolejny cykl ma ten sam stan startowy.

Raport zachowuje czas, liczbę błędów i wynik każdego cyklu. Mediana udanych
cykli jest pokazywana osobno od liczby niepowodzeń; brak sukcesu daje brak
mediany, a nie zerowy czas. Przerwanie serii nie tworzy pięciu sztucznych prób.
Zakończone cykle są zapisywane na bieżąco w sesji i pozostają po restarcie;
niezakończony cykl ma nieznane metryki. Cała odzyskana seria pozostaje FAILED.
Trening ma takie samo sterowanie i pozostaje poza zapisanym porównaniem.

Dwa punkty teleportacji PC i powrót do widoku bazowego są dostępne poza
aktywną serią. Rozpoczęcie serii odtwarza scenę, aby wcześniejsza eksploracja
nie zmieniała dystansu A/B. To zmiana pozycji kamery w symulatorze, nie
potwierdzenie działania TeleportationProvider ani interaktorów XRI.

## Lab03: galeria kotwic SIM_AR

1. Wybierz profil A (wiele cech) i rozpocznij próbę. Umieść model przy referencji.
   Scena wykonuje trzy odczyty na tej samej trasie trzech punktów obserwacji.
2. Wybierz profil B (mało cech), pozostaw pozostałe warunki i powtórz. Porównaj
   średnią długość błędu, normę średniego przesunięcia i rozrzut RMS wokół średniej.
   Mały rozrzut nie oznacza braku stałego przesunięcia.
3. Wybierz lepszy warunek i zaznacz kontrpróbę relokalizacji. Po trzech odczytach
   źródło traci śledzenie; model i wektor różnicy znikają. Po powrocie scena
   wykonuje trzy nowe odczyty na tej samej trasie. Zapisuje rzeczywiście
   zaobserwowany czas powrotu w zegarze aplikacji, nie opóźnienie urządzenia AR.
4. Scenariusz graniczny „brak odzyskania” kończy się FAILED po 5 s obserwacji
   utraty. Czas relokalizacji i odczyty po powrocie pozostają `null`.

Model `anchor-model/1` jest jawnie syntetyczny. W pliku `anchor-tracking.json`
są stałe przesunięcia, trzy składowe zmienności i zmiana po relokalizacji.
Seed 2718 wyznacza kolejność odczytów, taką samą dla A/B. Dane nie pochodzą
z telefonu ani gogli. Model używa jawnej referencji; nie wykonuje pomiaru
bezwzględnej dokładności rzeczywistego systemu AR.

Raport zachowuje oddzielne fazy `before` i `after`: liczbę odczytów, składowe
wektora [cm], czasy obserwacji [s], średnią długość błędu, normę średniego
wektora oraz RMS rozrzutu. Dla jednego odczytu nie wyznacza rozrzutu. Podczas
utraty nie tworzy poprawnych odczytów; po opóźnionej klatce nie dopisuje kilku
próbek wstecz. Przekroczenie limitu powrotu nie staje się sukcesem.

Każdy odczyt i zmiana fazy są zapisywane w rozpoczętej próbie. Restart zachowuje
dotychczasowe metryki, oznacza przerwanie i nie uzupełnia brakujących odczytów.
Nadal nie ma automatycznego zaliczania pięciu progów ani potwierdzenia testów
rzeczywistego AR. Instrukcje sterowania i uruchomienia pozostają takie same dla
solo i pary.

## Lab04: pomieszczenie głębi SIM_AR

1. W widoku bazowym zacznij od wariantu A. Rozpoznawaj obiekty po trwałej
   tożsamości: 1 czerwony, 2 zielony, 3 turkusowy. Napisy 2/3 i komunikat
   promienia mogą być celowo błędne. Odpowiedz dla wszystkich trzech obiektów,
   czy ich **środek** powinien być widoczny z tego punktu według geometrii.
2. Powtórz z wariantem B, pozostawiając widok i pozostałe ustawienia. Zmieniaj
   osobno tolerancję głębi albo oświetlenie. Tolerancja [m] dopuszcza widoczność,
   gdy inna powierzchnia jest bliższa od obiektu o nie więcej niż ustawiony próg.
   Duża tolerancja może zatem błędnie pokazać obiekt przez przeszkodę.
3. Sprawdź ustawienie z drugiego punktu. Przygotowany układ ma zasłaniać środek
   czerwonego obiektu z widoku bazowego, a odsłaniać go z bocznego. To wymaga
   potwierdzenia w dołączonym teście Unity. Nie wybieraj poprawki wyłącznie na
   podstawie jaśniejszego obrazu.

Referencja używa `Physics.RaycastAll` do środka obiektu. Brak trafienia w sam
obiekt oznacza brak referencji; brak innej powierzchni na tym promieniu jest
innym stanem i nie jest zastępowany odległością zero. To syntetyczna geometria,
nie ARCore Depth ani pomiar kamery. Kryterium dotyczy środka, nie widoczności
wszystkich pikseli i krawędzi częściowo przesłoniętej bryły.

Stan referencyjny, model renderowania, komunikat promienia i odpowiedź studenta
są odrębne. Wariant A wymusza widoczność czerwonego obiektu mimo bliższej
przeszkody; B i tolerancja korzystają z testu odległości. Napisy i komunikaty
nie zmieniają referencji. Oświetlenie zmienia wygląd, nie wynik testu geometrii.
Model widoczności w raporcie jest decyzją algorytmu, nie pomiarem pikseli GPU.

Po trzech odpowiedziach wynik zawiera poprawne wskazania, fałszywe alarmy
(„widoczny” przy referencji „zasłonięty”), pominięcia widocznych obiektów,
liczbę błędów modelu okluzji i błędnych komunikatów promienia. Nie oceniamy
liczby kliknięć. Odpowiedzi są zapisywane pojedynczo; restart zachowuje je jako
przerwaną próbę bez dopisywania pozostałych odpowiedzi. Zmiana geometrii lub
brak trafienia podczas próby kończy ją jako FAILED z jawnym brakiem referencji.


## Lab05: materiały, widok kontrolny i pełne okno pomiaru

1. Poza pomiarem włącz widok kontrolny i porównaj High/Medium/Low w tej samej
   skali. Kamera patrzy wzdłuż osi otworu z odległości 2 m. To inspekcja wizualna,
   nie sprawdzian przejścia trzpienia ani pomiar collidera.
2. Porównaj warianty przy tej samej trasie i odległości. Pomiar zawsze uruchamia
   trasę radialną po rozgrzewce, niezależnie od widoku kontrolnego. Ustawienie
   drugiego dystansu nadal przesuwa całą trasę o 4 m i należy je zachować w A/B.
3. Zmień wyłącznie 1/4 materiałów lub jeden próg LOD. W wariancie 4 materiałów
   trójkąty tej samej siatki są dzielone na cztery submeshe z osobnymi materiałami
   o identycznym kolorze/shaderze. Skala, pozycje wierzchołków i trójkąty pozostają
   takie same. Sloty materiałów nie są licznikiem draw calls; batching i renderer
   mogą zmienić rzeczywistą liczbę wywołań. `gpu_draw_calls` pozostaje null.

Raport zawiera min/max/medianę liczby trójkątów, min/max slotów, liczbę próbek,
medianę/p95 klatki, czas obserwowany bez otworu i jego udział w sumie czasów
próbek. Udział jest ważony czasem klatek, a nie liczbą próbek. Stan otworu wynika
z jawnej definicji wariantu proceduralnego; nie jest pomiarem percepcji ani testem
kolizji. Te koszty opisują cały obserwowany fragment trasy, nie tylko ostatni LOD.
Brak próbek daje null. Anulowanie, utrata fokusu lub zamknięcie sceny zapisują
częściowe statystyki jako FAILED. W mierzonym oknie nie wykonuje się zapisów na dysk,
aby ich opóźnienia nie zanieczyszczały porównania. Twarda awaria odzyskuje początek
próby jako FAILED bez statystyk z pamięci; nie odtwarza nieutrwalonych klatek.
Liczba draw calls i odbiór renderowania nadal wymagają Unity oraz narzędzi profilera.


## Lab06: aktualność, przerwa i odzyskanie

Źródło `jointstate-replay.csv` jest syntetycznym plikiem z nazwanymi kolumnami
`joint1_rad` i `joint2_rad`. Ich kolejność w pliku jest celowo odwrócona;
mapowanie odbywa się po nazwach. To nie ROS_LIVE ani zapis fizycznego robota.
Wersja relacji zegarów: `paused-source/1`. Zegar odbiornika jest monotonicznym
czasem Unity bez skalowania, a wirtualny czas pliku zatrzymuje się podczas
kontrolowanej przerwy. Odtwarzanie nie nadrabia przerwy kolejką historycznych póz.

1. Rozpocznij próbę z tym samym plikiem i progiem 0,25/0,5/1 s. Obserwuj numer
   wiadomości, wiek od ostatniego odbioru oraz niezależny wskaźnik odświeżania obrazu.
   Przed pierwszą wiadomością stan jest nieznany, a model ukryty.
2. Wywołaj jedną przerwę na próbę. Profil 1,5/3 s określa żądany czas pauzy.
   STALE zachodzi przy wieku większym od progu; wykrycie jest próbkowane w Update.
   Po upływie pauzy źródło może być wznowione przed nadejściem następnej wiadomości.
   Dopiero jej rzeczywisty odbiór przywraca stan świeży i widoczność modelu.
3. Porównaj zamrożenie z ukryciem przy STALE albo zmień watchdog. W kontrpróbie
   wybierz drugi czas przerwy, utrzymując pozostałe warunki. Zakończ obserwację
   przyciskiem zapisu; nie trzeba czekać na koniec 30-sekundowego pliku.

Raport rozróżnia `simulated_gap_duration` (początek do wznowienia źródła),
`resume_to_first_message` (wznowienie do pierwszego odbioru),
`last_message_to_stale` (ostatni odbiór do pierwszego wykrycia) i
`first_stale_duration` (pierwsze wykrycie do odbioru przywracającego świeżość).
`stale_detection_overshoot` opisuje przekroczenie progu w chwili wykrycia,
a nie opóźnienie sieci. Pierwsze zdarzenia nie są nadpisywane przez kolejne;
liczniki obejmują wszystkie zaobserwowane epizody. Odstępy wiadomości obejmują
przerwy oraz granicę pętli pliku. Osobno zapisywane są czasy klatek obrazu.
Wiek dotyczy odbiornika, nie wieku fizycznego pomiaru w zdalnym źródle.
RTT i opóźnienie źródło–odbiornik pozostają null z powodem braku pomiaru.

Zapis zdarzeń pierwszej wiadomości, STALE, końca pauzy i odzyskania zachowuje
częściowe metryki do odzyskania po awarii jako FAILED. Nie odtwarza brakujących
zdarzeń. Lokalne opóźnienia Update, renderowania i zapisu mogą wpływać na
obserwowane odstępy; nie są przypisywane transportowi ROS. Brak wiadomości
uniemożliwia ważną próbę. Niezweryfikowane pozostają kompilacja, PlayMode,
wygląd wskaźników, odbiór na Windows/Ubuntu, bufor i adapter ROS_LIVE.


## Lab07: trzy kroki i kontrola transferu

Wersja zadania `desktop-three-steps/1` ma trzy strefy desktopowe: wybór celów
1–2–3, przejścia kamery między punktami po wyborze celu oraz trzy klasyfikacje
świeżości na podstawie jawnych wieków danych 0,1/1,2/0,1 s przy progu 0,5 s.
Wybór celów nie jest chwytaniem, a przejścia punktowe nie są lokomocją XRI.
Klasyfikacja korzysta z fixture, nie z pomiaru transportu.

1. Wybierz strefę i wykonaj trzy kroki. Błędny wybór lub kliknięcie poza celem
   zwiększa liczbę błędów bez przechodzenia dalej. W klasyfikacji błędna odpowiedź
   kończy dany krok i zostaje zapisana; nie można poprawiać jej aż do trafienia.
2. Zmień tylko rozmiar celu albo podświetlenie następnego kroku. Powtórz zadanie
   przy tym samym dystansie i wejściu. Zachowaj pierwszą próbę. Raport zapisuje
   liczbę prób, błędów i czas ukończenia każdego kroku od początku misji oraz
   powodzenie całego celu, zamiast uznawać trzy odpowiedzi za trzy sukcesy.
3. W kontrpróbie wybierz drugi dystans: start kamery jest odsunięty o 4 m,
   pozycje celów i ich skala nie zmieniają się. W przejściach punktowych ten sam
   offset 4 m dotyczy kolejnych stanowisk. Zachowaj tę samą zmianę interfejsu.

Limit wynosi 90 s. Przerwanie albo limit zapisują FAILED i dostępne kroki;
nieukończone czasy i odpowiedzi pozostają null. Stan częściowy jest utrwalany
po każdej odpowiedzi, także błędnej. Zakończona próba klasyfikacji może być VALID
jako pomiar, mimo nieosiągnięcia celu; `mission_goal_achieved` pozostaje wtedy 0.
To nie automatyczne zaliczenie. `trial_ordinal` zapisuje kolejność pomiarów;
trening nie tworzy próby porównawczej, a efekt uczenia nadal wymaga interpretacji.

GUEST_SIM jest skryptem funkcjonalnym. Czasy kroków są w nim null z wyjaśnieniem,
a czas całej próby opisuje tylko wykonanie skryptu. SOLO/HUMAN nie są mieszane
z GUEST_SIM przy interpretacji. Testy z rzeczywistymi uczestnikami, dostępność
wskaźników i odbiór Unity na stanowiskach pozostają do wykonania.

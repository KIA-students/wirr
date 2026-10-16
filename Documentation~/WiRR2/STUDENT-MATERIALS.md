# Materiały Unity do nowych instrukcji

Wersja `student-kit/0.1.2`, 2026-10-16. **Zestaw pilotażowy do odbioru w Unity**.
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
| 03 `Lab03_AnchorGallery` | Referencje co 0,30 m, umieszczenie, dwa profile syntetycznego błędu, trzy odczyty, średni błąd i rozrzut RMS, jawna przerwa śledzenia SIM | Tor obserwacji, pełny scenariusz relokalizacji i opcjonalny adapter REAL_AR |
| 04 `Lab04_DepthRoom` | Trzy obiekty, przeszkoda, shader błędnej okluzji, wariant zgodny z głębią, odpowiedzi widoczny/niewidoczny sprawdzane raycastem, drugi widok | Próg głębi, warianty oświetlenia, osobne anomalie etykiety i błędnego raycastu, pomiary urządzeniowe |
| 05 `Lab05_CADWorkshop` | Proceduralne siatki High/Medium z rzeczywistym otworem i Low bez otworu; wariant stały lub automatyczny według odległości, dwa regulowane progi w metrach, wspólna radialna trasa benchmarku | Materiały/collidery, osobne gotowe modele konwersji CAD i odbiór renderowania; obecny pierścień jest modelem dydaktycznym |
| 06 `Lab06_DigitalTwin` | Model dwóch przegubów, nazwane kolumny JointState w radianach, syntetyczny replay 30 s, przerwy 1,5/3 s, watchdog 0,25/0,5/1 s, wiek i odstępy wiadomości | Bufor, kolejne strategie reakcji, opcjonalny ROS_LIVE; brak pomiaru RTT |
| 07 `Lab07_Expedition` | Sekwencja celów, nawigacja punktowa, klasyfikacja świeżości danych testowych; podświetlenie, kontekst, czas/błędy/sukcesy | Pełne mikromisje, kontrola transferu i testy użyteczności z uczestnikami |
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

W tym środowisku wykonano: 7 kontroli pakowania/danych i składni C# tree-sitter,
14 regresji istniejącego gradera oraz Ruff narzędzi Python. Zweryfikowano też
zgodność dziewięciu źródeł instrukcji/rubryki z przypiętą wersją źródłową.
**Nie wykonano kompilacji C#, importu Unity, renderowania Game View, testów
EditMode/PlayMode ani odbioru Windows/Ubuntu**. Analiza składni nie zastępuje tych testów.

Dołączone testy Unity: 24 przypadki EditMode (m.in. JSON, zapis z kopią,
rozdzielenie wyników, percentyle, nazwane mapowanie replay, geometria siatek
i import wszystkich scen, granice progów LOD i odzyskanie próby po serializacji)
oraz trzy testy PlayMode: start ośmiu doświadczeń, zmiana siatki LOD z powrotem do High
i teleportacja PC bez przesuwania obiektu i gniazda.
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

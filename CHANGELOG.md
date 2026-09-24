# Changelog

## Unreleased

## 0.9.0 (2026-09-24)

- dodano opisy proponowanych zastosowań przy dodatkowych materiałach dydaktycznych, prefabach Grid, demonstratorach materiałów, ruchu, fizyki i dźwięku oraz modelu Unitree G1 EDU;
- przeniesiono dziewięć środowisk HDRI z projektu deweloperskiego do instalowalnej części pakietu `Textures/HDRI`, dzięki czemu są dostępne po instalacji UPM;
- dodano `WiRRHdriTools` i sekcję „HDRI i skybox”: wybór panoramy, opis zastosowania oraz automatyczne utworzenie materiału `Skybox/Panoramic` i przypisanie go do aktywnej sceny;
- dodano ręczną instrukcję zmiany skyboxa, opis różnicy między plikami HDR EXR i podglądami tonemapped JPG oraz zalecenia eksperymentalne dotyczące Exposure i Rotation;
- dodano okno `WiRR → Pomoc → Budowanie i instalacja` z osobnymi procedurami dla Windows PC, smartfona z Androidem i Meta Quest 3;
- instrukcja buildów obejmuje Build Profiles, Scene List, Android Build Support, SDK/NDK/OpenJDK, ADB, `adb devices`, `adb install -r`, OpenXR/Meta Quest, ARM64 oraz rozróżnienie Meta Horizon Link od buildu standalone;
- dodano dokumenty `EXTRA_ASSETS.md`, `BUILD_AND_DEPLOY.md` i `Textures/HDRI/README.md`;
- workflow wydania sprawdza obecność nowych narzędzi, dokumentacji i komplet dziewięciu panoram HDRI przed publikacją.


## 0.8.2 (2026-09-24)

- po pierwszej instalacji danej wersji w konkretnym projekcie `WiRR Course Toolkit` otwiera się automatycznie jeden raz i od razu wskazuje kolejny krok;
- każdy punkt kontrolny w `WiRR Course Toolkit` pokazuje teraz bezpośrednio obowiązkowe pola wyniku oraz wszystkie tabele pomiarowe z formularza raportu;
- uzupełniono surowe pomiary PC–Quest w Lab 01, osobne serie czasu do `SessionTracking` i pierwszej płaszczyzny w Lab 03 oraz trzy pozycje kontrolne JointState→Unity w Lab 06;
- Lab 05 jawnie wymaga pomiaru czasu importu zasobu, zgodnie z obowiązkowym polem formularza;
- dodano narzędzie importu `RoboAnimation.unitypackage` z modeliem 3D humanoidalnego robota Unitree G1 EDU i przykładowymi animacjami;
- model Unitree G1 EDU jest dostępny jako dodatkowy zasób szczególnie dla Lab 05, Lab 06 i Lab 07; przykładowe animacje nie są traktowane jako źródło JointState ani referencja pomiarowa bliźniaka cyfrowego;
- workflow wydania sprawdza obecność pakietu modelu i narzędzia importującego przed publikacją.


## 0.8.1 (2026-09-23)

- dodano katalog zadań `WiRRLabTaskCatalog` dla laboratoriów 01–07, zgodny z punktami kontrolnymi 3.0–5.0 i polami raportu;
- główny panel `WiRR Course Toolkit` zawiera nową sekcję „Zadania laboratoryjne” z instrukcjami krok po kroku, paskiem postępu i lokalnymi checkboxami;
- każdy punkt kontrolny kończy się opisem dowodu, który student powinien zapisać w raporcie;
- uporządkowano przebieg panelu: walidacja konfiguracji jest wykonywana przed listą eksperymentów, a raport po wykonaniu zadań;
- rozbudowano `Samples~/Lab01`–`Lab07/README.md` o samodzielne instrukcje wykonania ćwiczeń bez konieczności odgadywania kolejności z samego szablonu raportu;
- checkboxy pozostają wyłącznie lokalną pomocą organizacyjną w `EditorPrefs` i nie są dołączane do raportu ani wysyłane do repozytorium.


## 0.8.0 (2026-09-22)

- dodano `WiRRLoopMotion` z trybami PingPong, Bob, Orbit i Rotate; komponent może sterować bezpośrednio Transform albo kinematycznym Rigidbody;
- dodano `WiRRPhysicsImpulsePad` do demonstracji triggerów, ForceMode, impulsu liniowego i momentu obrotowego;
- dodano `WiRRProceduralAudio`, który generuje bez zewnętrznych plików profile Hum, Beacon, Engine i Wind oraz może modulować pitch/głośność stanem obiektu;
- dodano `WiRRVehicleController`: prosty pojazd Rigidbody sterowany WASD/strzałkami lub gamepadem; API `SetExternalInput()` umożliwia późniejsze podłączenie sterowania XR;
- Runtime assembly jawnie odwołuje się do `Unity.InputSystem`, który jest już zależnością pakietu;
- dodano pięć opcjonalnych prefabów dynamicznych: unoszącą platformę teleportacyjną, platformę wahadłową, sterowalny wózek, wyrzutnię fizyczną i kinetyczny beacon audio;
- ruchome platformy zawierają wyłącznie obiekt `TeleportAreaPlaceholder`; student sam konfiguruje XRI Teleportation Area;
- rozbudowano Course Toolkit o sekcję „Ruch, fizyka i dźwięk” z generowaniem pojedynczych demonstratorów albo całego zestawu;
- prefab pojazdu korzysta z materiałów PBR, procedurally generated engine audio i fizyki Rigidbody, ale nie wymaga gotowych Input Actions ani XRI;
- demonstratory są opcjonalne i nie zmieniają kryteriów zaliczenia laboratoriów.


## 0.7.0 (2026-09-22)

- rozszerzono bibliotekę tekstur pakietu o 12 rodzin materiałów PBR 1K: AcousticFoam003, Concrete032, DiamondPlate005D, Fabric023, Fabric066, Metal004, Metal044A, PaintedWood007A, Sign002, Grass001, WoodFloor034 i WoodFloor040;
- dodano `WiRRSurfaceTextureLibrary`, która tworzy robocze kopie map Color, NormalGL, Roughness, Ambient Occlusion, Metalness, Displacement i Opacity oraz generuje edytowalne materiały URP;
- dla pełnego materiału PBR generator automatycznie pakuje metalness do kanału R i smoothness wyliczone z roughness do kanału A mapy Metallic/Smoothness;
- dodano pięć opcjonalnych prefabów dydaktycznych: galerię 12 materiałów, ścianę rozdzielczości 128/256/512/1024 px, porównanie albedo → albedo+normal → pełny PBR, galerię kanałów PBR oraz stanowisko tilingu i mipmap;
- rozszerzono WiRR Course Toolkit o osobną sekcję „Opcjonalne laboratorium materiałów” z możliwością dodawania pojedynczych demonstratorów lub całego zestawu;
- demonstratory są niezależne od obowiązkowych ćwiczeń i mogą być wykorzystane szczególnie w Laboratorium 01 oraz Laboratorium 05;
- do map normalnych używany jest wariant Y+ / OpenGL (`NormalGL`) zgodny z konwencją oczekiwaną przez Unity; `DiamondPlate` zachowuje również `NormalDX` do przyszłych ćwiczeń porównawczych.


## 0.6.0 (2026-09-22)

- dodano `WiRRPrefabTextureLibrary`, która korzysta z zestawów tekstur `grid-1_*`, `grid_2_*` oraz `grid-4_*`; trzeci zestaw jest w kodzie logicznie oznaczany jako `Grid3`, ponieważ w repozytorium nie ma obecnie plików `grid_3_*`;
- dodano package-visible katalog `Textures/Grid`; generator kopiuje mapy do `Assets/WiRR/Common/Textures/Generated`, ustawia import normal map/AO/specular i tworzy edytowalne materiały URP;
- dodano `WiRRPrefabTools` z czternastoma dodatkowymi, teksturowanymi prefabami dla laboratoriów 01–07;
- dodano m.in. arenę metryczną, galerię materiałów, stanowisko montażowe, planszę rejestracji AR, korytarz okluzji, stanowisko LOD/collider, celę bliźniaka cyfrowego, konsolę sieciową, arenę QA i macierz ryzyka;
- rozbudowano WiRR Course Toolkit o generowanie, odświeżanie materiałów i usuwanie wyłącznie instancji prefabów Grid;
- zachowano zasadę dydaktyczną: prefaby dostarczają kontekst wizualny i stanowisko pomiarowe, ale nie konfigurują za studenta XRI, AR Foundation ani mapowania ROS.


## 0.5.1 (2026-09-22)

- przywrócono nazwę produktu `WiRR Course Toolkit` w Unity, Package Manager, README i stronie kursu;
- usunięto pauzy typu em dash z aktywnych etykiet interfejsu oraz zastąpiono je dwukropkami, nawiasami lub prostszymi sformułowaniami;
- zachowano kolorowy, etapowy układ Course Toolkit wprowadzony wcześniej;
- zaktualizowano nazwy próbek laboratoriów i walidację wydania tak, aby używały separatora dwukropka.


## 0.5.0 — 2026-09-22

- dodano generator lekkich prefabów dydaktycznych dla Lab 01–07 bez dodatkowych zależności od XRI/AR/ROS;
- dodano wspólne środowisko `WiRR_LabRoom` oraz lekki `WiRR_ARReferenceKit` dla laboratoriów AR;
- dodano zestawy eksperymentalne: ścianę obciążenia geometrii i Physics Dropper, moduł zasilania i gniazdo, artefakt AR i Ghost Reference, Occlusion Bot i Depth Probe, warianty chwytaka do LOD, wizualne ramię robota z osiami oraz stanowisko QA z panelem kontrolowanych usterek;
- wygenerowane prefaby trafiają do `Assets/WiRR/LabXX/Prefabs/Generated`, a własna praca studenta pozostaje poza folderem odtwarzalnym;
- rozbudowano `WiRR → Narzędzia kursu` o tworzenie środowiska, zestawu eksperymentalnego i bezpieczne usuwanie obiektów dydaktycznych wyłącznie z aktywnej sceny;
- utrzymano zasadę, że materiały startowe nie konfigurują za studenta komponentów XRI, AR ani mapowania ROS stanowiących cel ćwiczenia;
- uporządkowano strukturę publikacji tak, aby publiczne repozytorium kursu zawierało wyłącznie materiały potrzebne studentom i użytkownikom pakietu.


## 0.4.5 — 2026-09-21

- przebudowano okno `WiRR → Narzędzia kursu` pod kątem ergonomii poznawczej: kolorowe nagłówki etapów, pasek postępu, czytelne stany GOTOWE/BRAK, komunikat „następny krok” oraz kolorystyczne rozróżnienie walidacji;
- zachowano redundancję informacji: kolor zawsze występuje razem z tekstem, dzięki czemu stan interfejsu nie zależy wyłącznie od rozpoznawania barw;
- ustawiono publiczną stronę kursu `https://kia-students.github.io/wirr/` jako kanoniczny adres dokumentacji WiRR;
- wszystkie odwołania do poprzedniego adresu strony zastąpiono adresem `https://kia-students.github.io/wirr/`;
- instalacja pakietu, klonowanie kodu i odwołania do źródeł wskazują publiczne repozytorium `KIA-students/wirr`;
- zaktualizowano stronę GitHub Pages do polskich nazw poleceń aktualnego interfejsu Unity;
- workflow wydania generuje notatki i instrukcję instalacji z docelowego repozytorium `KIA-students/wirr`.


## 0.4.4 — 2026-09-21

- ujednolicono polski język interfejsu w oknach WiRR, menu Unity, komunikatach walidatora, WebSim, nakładce metryk i inspektorach komponentów;
- przebudowano formularz raportu pod kątem studentów: etapy są zwijane, etap 3.0 jest jednoznacznie oznaczony jako wymagany do wysłania, pokazano postęp pól oraz objaśnienia celu każdego pola;
- dodano automatyczne obliczenia wartości pochodnych z danych surowych, m.in. median, czasu klatki z FPS, zmiany FPS, liczby kroków fizyki na sekundę, e_med/e_max, sum błędów, RTT, zmienności odstępów między wiadomościami i ryzyka R=P×S;
- pola i komórki obliczane automatycznie są tylko do odczytu i zawierają opis zastosowanego wzoru lub źródła danych;
- dla pól o zamkniętym zbiorze odpowiedzi dodano kontrolowane wybory zamiast swobodnego tekstu, m.in. PASS/FAIL/NV, HIT/MISS, tak/nie, skale 1–5 i 0–10;
- zachowano dotychczasowe stabilne klucze JSON tabel mimo polonizacji etykiet wyświetlanych użytkownikowi;
- walidacja raportu wskazuje teraz konkretne nieuzupełnione wiersze tabel oraz wyjaśnia, jakich danych brakuje do obliczenia pól automatycznych;
- rozszerzono obliczenia automatyczne m.in. o błąd odległości w raycaście głębi i przeliczenie FPS ↔ czas klatki tam, gdzie zależność jest jednoznaczna;
- ujednolicono polskie nazwy próbek w Unity Package Manager oraz naprawiono niespójne nazwy próbek laboratoriów 04 i 07, które mogły uniemożliwiać ich automatyczne odnalezienie.
- dodano automatyczny proces wydania: walidację pakietu, testy WebSim i walidatora raportów, archiwa ZIP/TGZ, sumy SHA-256 oraz publikację oznaczonego wydania GitHub.


## 0.4.3 — 2026-09-21

- import próbki WiRR — zarówno z Course Toolkit, jak i bezpośrednio z Unity Package Manager — automatycznie tworzy kompletny workspace `Assets/WiRR/LabXX` z folderami `Scenes`, `Scripts`, `Materials`, `Models`, `Prefabs`, `Textures`, `Data`, `Evidence` i `Documentation`;
- przy pierwszym przygotowaniu workspace automatycznie powstaje scena `Scenes/LabXX.unity` z pojedynczym `WiRRSceneMarker`, kamerą, światłem i gruntem;
- przebudowano okno `WiRR Course Toolkit`: dodano status laboratorium, przewijany układ kroków, akcje otwierania folderu/sceny i naprawy workspace oraz czytelniejszy workflow;
- raport można wysłać po ukończeniu checkpointu `3.0`; checkpointy `3.5–5.0` są opcjonalne i ich puste lub nieukończone pola nie blokują wysyłki;
- walidacja typów pól przy wysyłce obejmuje tylko najwyższy kompletny, sekwencyjny checkpoint raportu.


- uporządkowano menu Unity: narzędzia sceny przeniesiono do `WiRR → Lab scene`, a awaryjny szablon Markdown do `WiRR → Reports`; usunięto duplikat otwierania formularza raportu;
- `Create / repair base scene` wymusza teraz dokładnie jeden `WiRRSceneMarker` w aktywnej scenie, a walidator zgłasza duplikaty lub marker przypisany do innego laboratorium;
- grunt `WiRR_Ground` używa prostego `BoxCollider` zamiast `MeshCollider`, eliminując ostrzeżenie Unity 6000.6 o brakujących pre-baked triangle collision data.

- dodano `WebSim~/`: gotowy backend Docker Compose dla Lab 06 z ROS 2 Jazzy, rosbridge i deterministycznym generatorem `JointState` per sesja;
- backend obsługuje `RRBot 2R`, `WiRR Arm 3R`, komendy `Motion A/B/C`, `Home`, `Reset`, status i `/clock` zgodnie z klientem Unity;
- dodano instrukcję uruchomienia na tym samym komputerze oraz przez LAN.
- dodano workflow CI budujący kontener i sprawdzający utworzenie sesji ROS 2 oraz publikację `JointState`.
- poprawiono inicjalizację środowiska ROS 2 w kontenerze dla powłok z włączonym `nounset`.
- usunięto kolizję nazwy wewnętrznego zegara `rclpy` z publisherem `/clock`.
- dodano statyczną stronę WebSim i workflow publikacji przez GitHub Pages.
- rozbudowano stronę WebSim o instrukcję krok po kroku: przygotowanie Dockera, weryfikację topików, konfigurację Unity, diagnostykę i zakończenie pracy.

## 0.4.2 — 2026-09-17

- rozdzielono instalowalny pakiet UPM od referencyjnego projektu Unity: projekt deweloperski znajduje się teraz w `Project~/` i nie jest importowany do projektu studenta;
- usunięto źródło konfliktów GUID powodowanych przez duplikowanie `Assets`, `Packages` i `ProjectSettings` wewnątrz `Packages/pl.prz.kia.wirr`;
- dodano brakujący `Editor/WiRRReportEvaluator.cs.meta`, dzięki czemu evaluator jest poprawnie importowany i dostępny dla `WiRRReportWindow`;
- dodano pliki `.meta` do zasobów widocznych w głównym katalogu pakietu;
- dokumentację referencyjną przeniesiono do `Documentation~/`, a walidator CI do `scripts~/`, aby Unity nie importowało plików technicznych jako zasobów pakietu;
- minimalną wersję pakietu ustawiono na Unity `6000.6`;
- zaktualizowano workflow walidacji raportów do nowej ścieżki `scripts~/wirr_grade_report.py`.

## 0.4.1 — 2026-09-17

- dodano dedykowaną ikonę pakietu dla Unity Package Manager na podstawie logo WiRR;
- rozbudowano główny opis pakietu o zakres funkcjonalny i workflow `Raport → Sprawdź → Wyślij`;
- rozbudowano opisy próbek Lab 01–07 o cele oraz kluczowe etapy odpowiadające checkpointom `3.0 → 3.5 → 4.0 → 4.5 → 5.0`;
- doprecyzowano pełne nazwy laboratoriów w widoku Samples.

## 0.4.0 — 2026-09-17

- uproszczono raportowanie do jednego formularza: `wypełnij → sprawdź → wyślij`;
- lokalny `WiRRReportEvaluator` sprawdza checkpointy sekwencyjnie `3.0 → 3.5 → 4.0 → 4.5 → 5.0`;
- kontrola lokalna wymaga pól oznaczonych jako wymagane oraz obecności danych pomiarowych dla danego checkpointu;
- uproszczono dokument raportu `wirr-report/1.0` do danych potrzebnych do sprawozdania; usunięto telemetrykę przebiegu pracy z raportu;
- wysyłka korzysta ze stałego repozytorium kursu `KIA-students/wirr` i ścieżki `students/reports`;
- repozytoryjny skrypt walidacyjny wykonuje podstawową kontrolę integralności raportu w CI; ocena merytoryczna pozostaje po stronie prowadzącego;
- Lab 04 uporządkowano zgodnie z aktualnym przebiegiem ćwiczenia: Depth API → okluzja → depth-raycast → estymacja oświetlenia → kontrolowany błąd i diagnoza;
- WebSim pozostawiono wyłącznie dla Lab 06 jako opcjonalne źródło stanu bliźniaka cyfrowego;
- uproszczono dokumentację i interfejs Course Toolkit zgodnie z zasadą KISS;
- poprawiono obsługę uruchamiania połączenia WebSim w Play Mode.

## 0.3.0 — 2026-09-16

- `WiRR Reports` — okienkowe formularze raportów Lab 01–07 bez konieczności ręcznej edycji Markdown;
- stabilny format `wirr-report/1.0` do automatycznego parsowania;
- wspólne dane zespołu i automatyczne wyliczanie wariantów dla zespołów 2- i 3-osobowych;
- autosave szkiców w `Library/WiRRReports`;
- eksperymentalna telemetryka projektu i metadane środowiska;
- wysyłka raportu na osobną gałąź Git i automatyczne tworzenie PR przez `gh`, jeśli jest dostępne;
- dotychczasowy `report-template.md` pozostaje trybem awaryjnym.

## 0.2.0 — 2026-09-16

- WiRR WebSim jako trzeci tryb Lab 6 obok lokalnego i sieciowego ROS 2/Gazebo;
- `WebSimStateSource` — klient rosbridge WebSocket bez dodatkowej biblioteki Unity;
- wspólny kontrakt `IRobotStateSource` / `RobotState`;
- `WiRRRobotRig` do odwzorowania stanu przegubów;
- wybór RRBot 2R lub WiRR Arm 3R;
- heartbeat i wykrywanie STALE;
- wspólne definicje robotów z backendem WebSim.

## 0.1.0 — 2026-09-16

- pierwsza wersja pakietu UPM dla kursu WiRR;
- panel `WiRR → Course Toolkit`;
- instalacja zależności per laboratorium;
- import próbek WiRR i oficjalnych próbek Unity;
- przygotowanie sceny bazowej;
- walidacja projektu i sceny;
- komponent pomiarowy FPS / frame time / pamięć;
- generowanie i otwieranie kopii pustego szablonu sprawozdania;
- osobne `Samples~` dla laboratoriów 1–7.

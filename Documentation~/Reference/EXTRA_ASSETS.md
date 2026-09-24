# Dodatkowe elementy pakietu WiRR — proponowane zastosowania

Pakiet zawiera elementy opcjonalne. Nie są one wymagane w każdym laboratorium i nie zastępują zadań ocenianych. Ich rolą jest skrócenie czasu przygotowania sceny, ułatwienie eksperymentów i dostarczenie powtarzalnych obiektów testowych.

## Materiały dydaktyczne

### Dodaj środowisko
Tworzy rozpoznawalne, powtarzalne otoczenie zamiast pustej sceny.

**Proponowane zastosowanie:**
- kontrola skali 1:1 i orientacji w scenie;
- wspólne tło dla porównywania wyników między zespołami;
- szybkie przygotowanie stanowiska do testów interakcji, widoczności i nawigacji.

### Dodaj zestaw eksperymentalny
Tworzy zestaw obiektów właściwy dla wybranego laboratorium.

**Proponowane zastosowanie:**
- szybkie rozpoczęcie kontrolowanego eksperymentu;
- powtarzalne warunki A/B/C;
- ograniczenie różnic wynikających z ręcznego modelowania przez różne zespoły.

## Prefaby Grid

Teksturowane prefaby korzystają z rodzin `grid-1`, `grid_2` i `grid-4`.

**Proponowane zastosowanie:**
- ocena skali i orientacji obiektów;
- powierzchnie referencyjne do testów materiałów i oświetlenia;
- obserwacja jakości tekstur przy zmianie odległości i kąta widzenia;
- punkty odniesienia w eksperymentach AR/XR.

## Galeria materiałów PBR

Pakiet zawiera biblioteki materiałów m.in. metalu, drewna, tkaniny, betonu, trawy i powierzchni oznaczeniowych.

**Proponowane zastosowanie:**
- porównanie wpływu albedo, normal map, roughness, metalness i AO;
- nauka różnicy między materiałem dielektrycznym i metalicznym;
- test kosztu wielu materiałów i slotów materiałowych;
- kontrola zachowania materiałów pod różnymi HDRI.

## Demonstratory materiałów

### Galeria materiałów
Porównanie wielu typów powierzchni w identycznym oświetleniu.

### Rozdzielczość 128–1024
Analiza wpływu rozdzielczości tekstur na ostrość, pamięć i koszt renderowania.

### Normal map: porównanie
Porównanie geometrii bez normal mapy i z normal mapą.

### Kanały PBR
Badanie wpływu poszczególnych map materiału na wygląd końcowy.

### Tiling i mipmapy
Obserwacja powtarzania tekstury, aliasingu oraz przełączania poziomów mip.

**Najlepsze zastosowanie:** Lab 01 i Lab 05, a dodatkowo każde ćwiczenie dotyczące percepcji, wydajności i jakości wizualnej.

## Ruch, fizyka i dźwięk

### Unosząca platforma
Do testów ruchomego punktu teleportacji, komfortu ruchu i zachowania użytkownika względem ruchomego podłoża.

### Platforma wahadłowa
Do testów ruchu okresowego, układów odniesienia, Rigidbody i komfortu XR.

### Sterowalny pojazd
Do testów Input System, fizyki Rigidbody, sterowania i późniejszego podłączenia wejścia XR.

### Wyrzutnia fizyczna
Do eksperymentów z triggerami, impulsami, kolizjami i powtarzalnością symulacji.

### Kinetyczny beacon audio
Do testów audio przestrzennego, sygnałów ostrzegawczych, multimodalnej informacji zwrotnej i powiązania dźwięku z ruchem.

## Efekty charakterystyczne dla XR

Pakiet zawiera także pięć gotowych demonstratorów XR, które mają przypisaną własną logikę Runtime i działają od razu w Play Mode:

- **XR_ParallaxPortal** — wielowarstwowy portal sterowany translacją głowy; skrypt `WiRRHeadParallax` pokazuje head-coupled parallax i znaczenie 6DoF;
- **XR_GazeBloom** — holograficzny obiekt reagujący na kierunek patrzenia; `WiRRGazeBloom` pokazuje implicit interaction i może przyjąć zewnętrzny sygnał z eye trackingu/XRI;
- **XR_TelekinesisOrb** — demonstracja distant interaction/force grab; `WiRRTelekinesisOrb` ma gaze dwell, płynne przyciąganie do użytkownika, wiązkę i powrót do pozycji bazowej;
- **XR_DiegeticHUD** — panel przestrzenny z miękkim podążaniem za głową; `WiRRHeadFollower` pozwala porównać UI podążające i world-locked;
- **XR_WorldScaleTotem** — obiekt przechodzący od miniatury do room-scale w funkcji odległości; `WiRRProximityScale` pokazuje embodied scale i wpływ skali 1 unit = 1 m.

Każdy komponent ma publiczne metody przeznaczone do późniejszego podłączenia XRI, kontrolera lub hand trackingu. Oznacza to, że demonstracja działa bez dodatkowego SDK, ale student może wykorzystać ją jako gotowy obiekt wykonawczy dla własnej interakcji.

Szczegółowy opis działania, API oraz propozycje eksperymentów znajdują się w `Documentation~/Reference/XR_SHOWCASE.md`.

## Mixed Reality: realne otoczenie jako część sceny

Drugi zestaw pięciu demonstratorów pokazuje nie tylko wirtualne efekty XR, ale bezpośrednie łączenie danych z rzeczywistego świata z grafiką Unity:

- **MR_CameraWindow** — obraz z kamery lub zewnętrznej tekstury z wirtualnym reticle; `WiRRCameraFeedMixer` może korzystać z `WebCamTexture` albo `SetExternalTexture(Texture)`;
- **MR_HandAura** — wirtualne markery nadgarstka i opuszków nakładane na śledzoną dłoń; `WiRRHandAura` wylicza również `Pinch01`;
- **MR_PeopleAwareness** — anonimowe markery i strefy wokół wykrytych osób; `WiRRPeopleAwareness` przyjmuje wyłącznie pozycje w świecie;
- **MR_SpatialSurfaceScanner** — wizualizacja punktów z depth/spatial mesh, z raycastowym fallbackiem do testów w Editorze;
- **MR_WallPortal** — portal i wirtualna głębia kotwione do wykrytej ściany przez `WiRRWallAnchor` i `WiRRHeadParallax`.

Każdy prefab ma gotową logikę Runtime i jednocześnie jawny punkt integracji z konkretnym providerem. Szczegółowy opis znajduje się w `Documentation~/Reference/MIXED_REALITY_SHOWCASE.md`. Jeżeli chcesz podłączyć realny sensor lub SDK i rozbudować prefab, użyj `MIXED_REALITY_IMPLEMENTATION.md` albo okna `WiRR → Pomoc → Mixed Reality: implementacja i rozbudowa`; można tam również wygenerować pięć starterów adapterów do własnego folderu `Scripts`.

## Mobile AR: rozszerzona rzeczywistość na telefonie

Pakiet zawiera również pięć demonstratorów przeznaczonych specjalnie do AR na smartfonie:

- **AR_TapPlacement** — reticle i umieszczanie obiektu przez tap na powierzchni plane/depth;
- **AR_ImageMarkerPortal** — zawartość zakotwiona do rozpoznanego obrazu z Reference Image Library;
- **AR_WorldRuler** — pomiar odległości w świecie przez dwa tapy;
- **AR_LightMatchObject** — dopasowanie wirtualnego światła i materiału do estymacji oświetlenia kamery;
- **AR_SurfacePainter** — rysowanie palcem po realnych powierzchniach z użyciem raycastu.

Każdy element ma fallback do testów w Editorze i publiczne API dla AR Foundation. Przycisk `Utwórz startery AR Foundation` generuje kod studenta w `Assets/WiRR/LabXX/Scripts/MobileARAdapters` bez modyfikowania provider-neutralnego Runtime WiRR.

Szczegóły znajdują się w `Documentation~/Reference/MOBILE_AR_SHOWCASE.md` oraz w `WiRR → Pomoc → Mobile AR: telefon`.

## Unitree G1 EDU

`RoboAnimation.unitypackage` zawiera model 3D humanoidalnego robota Unitree G1 EDU oraz przykładowe animacje.

**Proponowane zastosowanie:**
- Lab 05: złożony obiekt do audytu geometrii, materiałów, pamięci i wydajności;
- Lab 06: reprezentacja wizualna robota w scenie bliźniaka cyfrowego;
- Lab 07: realistyczne obciążenie sceny w testach wydajności, regresji i stabilności;
- demonstracje animacji humanoida i hierarchii kości.

Animacje są materiałem demonstracyjnym. Nie zastępują danych `JointState` i nie są referencją poprawności mapowania ROS 2 → Unity.

## HDRI

Pakiet zawiera panoramy HDR w `Textures/HDRI`. Mogą pełnić jednocześnie rolę tła i źródła światła środowiskowego.

- **Amsterdam** — środowisko miejskie; szkło, metal, złożone tło.
- **Clean Horizon** — neutralny horyzont; benchmarki i porównania materiałów.
- **Day Sky** — jasne warunki dzienne; ekspozycja i kontrast.
- **Evening Environment** — cieplejsze, ciemniejsze światło; czytelność UI i percepcja.
- **Forrest** — naturalne, bogate wizualnie otoczenie.
- **Indoor Environment** — światło wnętrza; odbicia i materiały.
- **Near Lake** — jasne niebo i ciemniejszy teren; odbicia.
- **Night Sky** — testy emisji, oświetlenia i UI w ciemności.
- **Tower** — otwarty krajobraz; sylwetka, skala i daleki horyzont.

Do faktycznego skyboxa używaj plików `*_HDR.exr`. Pliki `*_TONEMAPPED.jpg` są przede wszystkim wygodnym podglądem LDR.

Szczegółowa procedura znajduje się w `Textures/HDRI/README.md`. W `WiRR → Narzędzia kursu` można też wybrać HDRI i użyć przycisku **Ustaw wybrane HDRI jako Skybox**.

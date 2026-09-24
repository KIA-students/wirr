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

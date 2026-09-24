# Mixed Reality — instrukcja implementacji i dalszej rozbudowy

Ten dokument pokazuje **jak przejść od gotowego demonstratora WiRR do własnej implementacji studenta**. Celem nie jest kopiowanie kodu pakietu, lecz nauczenie się rozdzielania warstwy sprzętowej od logiki aplikacji.

## Architektura, której należy przestrzegać

Stosuj układ:

`sensor / SDK / model CV → adapter w Assets/WiRR/LabXX/Scripts → komponent WiRR Runtime → prefab / wizualizacja`

Przykłady:

- `AR Foundation camera texture → CameraFeedAdapterStarter → WiRRCameraFeedMixer → MR_CameraWindow`;
- `XR Hands joints → HandTrackingAdapterStarter → WiRRHandAura → MR_HandAura`;
- `people detector / body tracker → PeopleDetectorAdapterStarter → WiRRPeopleAwareness → MR_PeopleAwareness`;
- `Depth API / spatial mesh → SpatialDepthAdapterStarter → WiRRSpatialSurfaceScanner → MR_SpatialSurfaceScanner`;
- `Scene Understanding planes → WallPlaneAdapterStarter → WiRRWallAnchor → MR_WallPortal`.

Adapter powinien być mały. Jego zadaniem jest przetłumaczenie danych konkretnego SDK do prostych typów Unity (`Texture`, `Vector3`, `Quaternion`, `Vector2`). Dzięki temu prefab może działać z innym providerem bez zmian w warstwie Runtime.

## Przygotowanie workspace

1. Otwórz `WiRR → Narzędzia kursu`.
2. Wybierz laboratorium i przygotuj jego workspace.
3. W sekcji `Mixed Reality: świat rzeczywisty + wirtualny` dodaj wybrany prefab.
4. Kliknij `Jak to zaimplementować i rozbudować?`.
5. Ustaw numer laboratorium i kliknij `Utwórz 5 starterów adapterów`.
6. Pliki pojawią się w `Assets/WiRR/LabXX/Scripts/MixedRealityAdapters`.
7. Otwórz tylko starter dotyczący wybranego elementu i rozwijaj go w projekcie studenta.

Generator nie nadpisuje istniejących plików. Kod utworzony w `Assets` należy do studenta i może zawierać zależności Meta, AR Foundation, XR Hands albo własnego modelu CV. Pakiet WiRR pozostaje od nich niezależny.

---

## MR_CameraWindow — implementacja krok po kroku

### Etap 1: sprawdzenie wersji bazowej

1. Dodaj `Camera Window`.
2. Uruchom Play Mode.
3. Zezwól na kamerę, jeśli system o to poprosi.
4. Sprawdź, czy `WiRRCameraFeedMixer` znajduje się na korzeniu prefabu.
5. Sprawdź, czy renderer `RealCameraFeed` pokazuje obraz lub fallback.

### Etap 2: wybór providera

Możliwe źródła:

- `WebCamTexture` — PC / klasyczny Android;
- kamera z AR Foundation;
- tekstura dostarczana przez własny moduł CV;
- RenderTexture generowana przez inny subsystem;
- platformowy passthrough Meta — zwykle jako warstwa compositora, nie jako zwykła tekstura.

### Etap 3: adapter

Otwórz `CameraFeedAdapterStarter.cs` i:

1. dodaj referencję do wybranego SDK;
2. zasubskrybuj zdarzenie dostarczenia klatki lub tekstury;
3. gdy provider ma nową `Texture`, wywołaj `PushFrame(texture)`;
4. przy zatrzymaniu sesji wywołaj `ClearFrame()`;
5. przy zmianie orientacji sprawdź `SetMirror(horizontal, vertical)`.

### Etap 4: walidacja

Sprawdź:

- aspect ratio;
- rotację obrazu;
- mirror front camera;
- opóźnienie kamera → wyświetlenie;
- zachowanie po utracie i odzyskaniu zgody;
- zachowanie po `OnApplicationPause` / wznowieniu.

### Proponowane dalsze modyfikacje

**Łatwe:**
- przełączanie front/back camera;
- suwak opacity warstwy realnej i wirtualnej;
- overlay FPS i timestamp.

**Średnie:**
- pomiar camera-to-display latency;
- bounding boxes z modelu detekcji obiektów;
- reticle przyklejony do punktu wykrytego w obrazie;
- korekcja intrinsics i ray z piksela do świata.

**Zaawansowane:**
- segmentacja ludzi/obiektów i selektywne nakładanie grafiki;
- rekonstrukcja głębi i poprawna okluzja;
- foveated processing region sterowany gaze;
- porównanie dwóch providerów kamery pod kątem latency i jakości.

**Przykładowa metryka:** mediana i p95 opóźnienia kamera → obraz oraz błąd reprojekcji punktu 2D → 3D.

---

## MR_HandAura — implementacja krok po kroku

### Etap 1: wersja bazowa

1. Dodaj `Hand Aura`.
2. Uruchom Play Mode.
3. Fallback wygeneruje proceduralną dłoń przed kamerą.
4. Obserwuj `PinchCore` i wartość `Pinch01`.

### Etap 2: dane minimalne

Do wersji podstawowej potrzebujesz:

- wrist position;
- wrist rotation;
- thumb tip;
- index tip;
- middle tip;
- ring tip;
- little tip;
- flagi tracking valid.

### Etap 3: adapter

W `HandTrackingAdapterStarter.cs`:

1. odczytaj jointy z providera;
2. przelicz je do world space Unity;
3. upewnij się, że jednostką są metry;
4. wywołaj `PushPose(...)`;
5. przy utracie trackingu wywołaj `LostTracking()`.

### Etap 4: kontrola jakości

Zmierz:

- jitter końcówki index finger przy nieruchomej dłoni;
- latency podczas szybkiego ruchu;
- false positive / false negative pinch;
- czas odzyskania trackingu po zasłonięciu dłoni.

### Proponowane dalsze modyfikacje

**Łatwe:**
- osobne kolory dla lewej i prawej dłoni;
- zmiana wielkości markerów w zależności od confidence;
- wizualny stan `TRACKED / LOST`.

**Średnie:**
- pełny skeleton 21/26 jointów;
- mesh dłoni;
- One Euro Filter z regulowanym cutoff;
- rozpoznawanie `grab`, `point`, `open hand`, `thumbs up`.

**Zaawansowane:**
- dwuręczne skalowanie i obrót;
- hand-object occlusion;
- near-touch przyciski z deformacją;
- porównanie predykcji vs brak predykcji przy szybkich gestach.

**Przykładowa metryka:** RMS jitter jointu, średni czas detekcji pinch oraz opóźnienie ruch rzeczywisty → marker wirtualny.

---

## MR_PeopleAwareness — implementacja krok po kroku

### Etap 1: wersja bazowa

1. Dodaj `People Awareness`.
2. Fallback symuluje dwie poruszające się osoby.
3. Obserwuj zmianę halo przy zmniejszaniu odległości.

### Etap 2: wybór punktu reprezentującego osobę

Preferowane:

- pelvis / root joint body trackera;
- torso;
- środek 3D bounding box;
- punkt na podłodze wyliczony z sylwetki i depth.

Nie używaj środka twarzy jako jedynej pozycji do proxemics, jeśli provider oferuje stabilniejszy punkt ciała.

### Etap 3: adapter

W `PeopleDetectorAdapterStarter.cs`:

1. pobierz listę wykrytych osób;
2. dla każdej oblicz `Vector3` w world space;
3. wywołaj `PushPeople(worldPositions)`;
4. przy pustej scenie wywołaj `ClearPeople()`;
5. nie przechowuj danych identyfikacyjnych, jeśli nie są potrzebne.

### Etap 4: walidacja

Ustaw osobę w znanych odległościach 1 m, 2 m, 3 m i porównaj pozycję markera. Powtórz dla ruchu poprzecznego i zasłonięcia.

### Proponowane dalsze modyfikacje

**Łatwe:**
- różne stany kolorystyczne wg dystansu;
- wskaźnik kierunku poza polem widzenia;
- licznik osób bez identyfikacji.

**Średnie:**
- estymacja velocity;
- predykowany tor na 0,5–1 s;
- eliptyczna strefa proxemics;
- chwilowy anonimowy track-id do ciągłości trajektorii.

**Zaawansowane:**
- dynamiczne strefy człowiek–robot;
- estymacja orientacji ciała;
- multimodalne ostrzeżenie audio + wizualizacja;
- fusion vision + LiDAR/depth;
- social referencing bez identyfikowania osoby.

**Przykładowa metryka:** błąd lokalizacji osoby [m], czas detekcji wejścia do strefy i liczba fałszywych alarmów.

---

## MR_SpatialSurfaceScanner — implementacja krok po kroku

### Etap 1: wersja bazowa

1. Dodaj `Spatial Surface Scanner`.
2. Upewnij się, że scena ma collidery.
3. W Play Mode fallback wysyła raycasty z kamery i oznacza trafione powierzchnie.

### Etap 2: provider depth/spatial

Źródłem może być:

- raycast depth;
- environment depth;
- spatial mesh;
- scene mesh;
- depth image po unprojection.

### Etap 3: adapter

W `SpatialDepthAdapterStarter.cs`:

1. pobierz `worldPoint`;
2. pobierz lub policz `worldNormal`;
3. jeżeli dostępne jest confidence, znormalizuj je do 0–1;
4. wywołaj `PushSample(point, normal, confidence)`;
5. dla batcha użyj `PushBatch(points, normals)`;
6. ogranicz częstotliwość i liczbę punktów.

### Etap 4: walidacja

Na płaskiej ścianie policz:

- średni błąd od płaszczyzny;
- odchylenie standardowe;
- stabilność normalnych;
- liczbę próbek/s;
- koszt CPU.

### Proponowane dalsze modyfikacje

**Łatwe:**
- kolor wg confidence;
- marker wg typu powierzchni;
- kontrolowana długość życia próbek.

**Średnie:**
- lokalny mesh z punktów;
- plane fitting RANSAC;
- klasyfikacja floor/wall/table;
- depth-only occlusion material.

**Zaawansowane:**
- incremental mesh reconstruction;
- real-world physics collisions;
- navigable surface extraction;
- fusion kilku klatek depth;
- porównanie latency i dokładności dwóch źródeł depth.

**Przykładowa metryka:** RMSE punktów do płaszczyzny, błąd normalnej [°], CPU ms i liczba próbek/s.

---

## MR_WallPortal — implementacja krok po kroku

### Etap 1: wersja bazowa

1. Dodaj `Wall Portal / Anchor`.
2. Dodaj lub wykorzystaj collider ściany.
3. W Play Mode możesz wywołać `TryAnchorFromViewerRay()`.
4. Sprawdź, czy portal ustawia się na powierzchni i czy paralaksa reaguje na ruch głowy.

### Etap 2: provider płaszczyzn

Potrzebujesz:

- center ściany;
- normalnej;
- width/height w metrach;
- opcjonalnie semantic label i polygon boundary.

### Etap 3: adapter

W `WallPlaneAdapterStarter.cs`:

1. odfiltruj floor/ceiling;
2. wybierz ścianę rayem lub UI;
3. sprawdź kierunek normalnej;
4. wywołaj `PushWall(center, normal, sizeMeters)`;
5. przy utracie kotwicy wywołaj `ClearWall()`.

### Etap 4: stabilność

Przejdź wokół portalu, odwróć głowę, zasłoń fragment ściany, wróć po kilku sekundach. Zapisz dryf i skok po relokalizacji.

### Proponowane dalsze modyfikacje

**Łatwe:**
- ręczny wybór jednej z wykrytych ścian;
- margines od krawędzi;
- obrót/skalowanie zawartości gestem.

**Średnie:**
- persistent anchor;
- obsługa polygon boundary zamiast samego extent;
- automatyczne rozmieszczanie obrazów/paneli;
- feathering krawędzi portalu.

**Zaawansowane:**
- portal do wirtualnego pomieszczenia z poprawną okluzją;
- przechodzenie wirtualnych obiektów przez granicę realnej ściany;
- rekonstrukcja framugi/otworu drzwiowego;
- synchronizacja tego samego wall anchor między kilkoma użytkownikami.

**Przykładowa metryka:** translacyjny dryf kotwicy [cm], błąd kąta normalnej [°] i czas relokalizacji [s].

---

## Jak zaprojektować własną rozbudowę

Nie zaczynaj od pytania „jaki efekt dodać?”. Zacznij od pytania „jaki problem MR chcę zbadać?”.

### Schemat pracy

1. **Hipoteza** — np. „filtr One Euro zmniejszy jitter dłoni bez wzrostu opóźnienia > 20 ms”.
2. **Baseline** — zmierz gotowy prefab.
3. **Jedna modyfikacja** — wprowadź tylko jeden nowy mechanizm.
4. **Powtórzony pomiar** — te same warunki i urządzenie.
5. **Porównanie A/B** — liczby, nie tylko zrzuty ekranu.
6. **Wniosek** — oddziel obserwację od interpretacji.

### Proponowane tematy samodzielnej rozbudowy

- uncertainty-aware MR;
- predykcja ruchu dłoni lub człowieka;
- multimodalny feedback: grafika + audio + haptics;
- persistent spatial anchors;
- semantic scene understanding;
- real-world occlusion;
- real-world physics;
- cross-provider benchmarking;
- dynamic safety zones;
- privacy-preserving perception;
- shared anchors dla wielu użytkowników;
- adaptive quality zależna od obciążenia GPU;
- pomiar motion-to-photon / sensor-to-photon latency.

## Co warto pokazać w sprawozdaniu z własnej rozbudowy

- diagram `provider → adapter → WiRR component → visual`;
- nazwę i wersję SDK;
- transformacje układów współrzędnych;
- sposób obsługi utraty trackingu;
- parametry filtracji;
- metrykę i procedurę A/B;
- trzy lub więcej powtórzeń, gdy mierzone są czasy/błędy;
- jeden zrzut pokazujący efekt;
- wniosek opisujący także ograniczenia.

Gotowy prefab WiRR powinien być traktowany jak **kontrolowany baseline**. Najbardziej wartościowym etapem jest moment, w którym student zastępuje fallback prawdziwym providerem, rozbudowuje zachowanie i potrafi zmierzyć wpływ własnej decyzji implementacyjnej.

<p align="center">
  <img src="icon.png" alt="WiRR" width="128">
</p>

# WiRR Course Toolkit

Pakiet Unity Package Manager (UPM) dla przedmiotu **Wirtualna i Rozszerzona Rzeczywistość**. WiRR przygotowuje środowisko laboratoriów 1–7, pomaga sprawdzić konfigurację, udostępnia potrzebne narzędzia pomiarowe i prowadzi studenta do złożenia raportu.

## Repozytorium kursu

Publiczne repozytorium kursu znajduje się pod adresem [KIA-students/wirr](https://github.com/KIA-students/wirr). To z niego studenci instalują pakiet przez Unity Package Manager i do niego odnoszą się materiały kursowe.

Strona kursu jest dostępna pod adresem [kia-students.github.io/wirr](https://kia-students.github.io/wirr/). Zawiera instrukcję instalacji pakietu, opis WebSim oraz interaktywny kwestionariusz SSQ.

## Zasada działania

WiRR automatyzuje infrastrukturę, a nie wykonanie ćwiczenia. Student nadal sam implementuje rozwiązanie, wykonuje pomiary i interpretuje wyniki zgodnie z instrukcją laboratoryjną.

Pakiet zawiera:

- **WiRR Course Toolkit**: główny panel prowadzący przez przygotowanie laboratorium;
- **Dependency Installer**: instalację wymaganych pakietów Unity;
- **Samples Manager**: import materiałów dla wybranego laboratorium;
- **Scene Tools / Validator**: przygotowanie i kontrolę sceny;
- **Runtime Utilities**: lekkie komponenty pomiarowe;
- **Checklisty zadań Lab 01–07**: krok po kroku prowadzą przez punkty kontrolne 3.0–5.0 i wskazują wymagany dowód pomiarowy;
- **WebSim**: połączenie Unity ze środowiskiem ROS 2/Gazebo przez rosbridge;
- **Raporty WiRR**: formularz, automatyczny zapis, tabele pomiarowe i raport JSON;
- **Wysyłanie przez Git**: wysłanie raportu do repozytorium;
- **Walidacja raportu w CI**: techniczną kontrolę integralności raportu;
- **Unitree G1 EDU**: dodatkowy model humanoidalnego robota z przykładowymi animacjami, importowany z `RoboAnimation.unitypackage`.

## Instalacja

W Unity wybierz:

`Window → Package Manager → + → Install package from git URL...`

Repozytorium studenckie:

```text
https://github.com/KIA-students/wirr.git
```

Pakiet znajduje się w katalogu głównym repozytorium. Po pierwszej instalacji danej wersji w projekcie panel **WiRR Course Toolkit** otworzy się automatycznie jeden raz i wskaże kolejny krok. Później można go otworzyć ręcznie przez **WiRR → Narzędzia kursu**. Wymagana wersja to Unity **6000.6.x** lub nowsza zgodna wersja 6000.6.

## Struktura repozytorium

Katalog główny repozytorium jest czystym pakietem UPM. Kod pakietu znajduje się w `Editor/` i `Runtime/`, a materiały importowane przez Package Manager w `Samples~/`.

Pełny projekt Unity używany do rozwoju i testowania pakietu znajduje się w `Project~/`. Sufiks `~` powoduje, że Unity Package Manager nie importuje tego katalogu do projektu studenta, dzięki czemu `Assets`, `Packages` i `ProjectSettings` projektu testowego nie powodują konfliktów GUID z projektem użytkownika. Osoby rozwijające pakiet powinny otwierać w Unity katalog `Project~`, a nie katalog główny repozytorium.

Dodatkowa dokumentacja techniczna znajduje się w `Documentation~/`, a skrypt walidatora CI w `scripts~/`; katalogi te również są celowo pomijane przez Asset Database.

## Laboratoria i zależności

| Lab | Dodatkowe pakiety |
|---|---|
| 1 | XR Management, OpenXR, XRI, XR Hands |
| 2 | XR Management, OpenXR, XRI |
| 3 | XR Management, XRI, AR Foundation, ARCore |
| 4 | XR Management, XRI, AR Foundation, ARCore |
| 5 | brak dodatkowych |
| 6 | ROS-TCP-Connector v0.7.1 z Git |
| 7 | Unity Test Framework |

Materiały startowe znajdują się w `Samples~/Lab01`–`Samples~/Lab07`. Po imporcie próbki WiRR: z panelu WiRR albo bezpośrednio z Unity Package Manager: pakiet automatycznie przygotowuje folder roboczy `Assets/WiRR/LabXX` z folderami `Scenes`, `Scripts`, `Materials`, `Models`, `Prefabs`, `Textures`, `Data`, `Evidence` i `Documentation`. Przy pierwszym imporcie tworzona jest również scena `Scenes/LabXX.unity`.

## Prefaby dydaktyczne

W sekcji **Scena i pomiary → Materiały dydaktyczne** można opcjonalnie wygenerować:

- wspólne środowisko laboratorium albo lekki zestaw odniesienia AR;
- zestaw rozpoznawalnych obiektów eksperymentalnych właściwy dla Lab 01–07;
- zwykłe prefaby Unity zapisane w `Assets/WiRR/LabXX/Prefabs/Generated`.

Generator tworzy stylizowane, lekkie obiekty low-poly: stanowisko pomiarowe, moduł zasilania i gniazdo, artefakt AR i ramę kalibracyjną, robota do okluzji, warianty chwytaka do LOD, ramię robota z osiami przegubów oraz stanowisko QA z panelem usterek.

Materiały **nie rozwiązują ćwiczenia za studenta**. Nie konfigurują automatycznie komponentów XRI, kotwic AR, raycastów, mapowania ROS ani procedury testowej, jeżeli właśnie te elementy są celem laboratorium. Folder `Prefabs/Generated` jest odtwarzalny; własne rozwiązania studenta powinny znajdować się poza nim.

### Teksturowane prefaby Grid

Drugi generator, `WiRRPrefabTools`, tworzy dodatkowe obiekty o bardziej charakterystycznej warstwie wizualnej. Korzysta z map PBR dostarczonych w projekcie deweloperskim w `Assets/Textures`:

- `grid_1_*` → `Grid1`; bieżące pliki `grid-1_*` są obsługiwane jako fallback;
- `grid_2_*` → `Grid2`;
- `grid_3_*` → `Grid3`; bieżące pliki `grid-4_*` są obsługiwane jako fallback.

Przy instalacji UPM te same mapy są dostępne w `Textures/Grid`. Narzędzie kopiuje je do `Assets/WiRR/Common/Textures/Generated`, ustawia właściwości importera oraz buduje materiały URP w `Assets/WiRR/Common/Materials/Generated/Grid`. Prefaby trafiają do `Assets/WiRR/LabXX/Prefabs/GridGenerated`.

Dla każdego laboratorium generowane są dwa dodatkowe stanowiska kontekstowe. Są one przeznaczone do obserwacji, pomiarów i własnej rozbudowy, a nie jako gotowe rozwiązania zadań.

## Opcjonalne laboratorium materiałów

WiRR 0.7.0 zawiera dodatkową bibliotekę powierzchni PBR i zestaw demonstratorów przeznaczonych przede wszystkim do Laboratorium 01 oraz Laboratorium 05. Są dostępne w każdej scenie z sekcji **Scena i pomiary → Opcjonalne laboratorium materiałów**.

Biblioteka obejmuje 12 rodzin: piankę akustyczną, beton, blachę ryflowaną, dwie tkaniny, dwa metale, malowane drewno, materiał znaku, trawę i dwie odmiany drewnianej podłogi. Generator korzysta z wielu map: Color, NormalGL, Roughness, Ambient Occlusion, Metalness, Displacement oraz Opacity, jeśli są dostępne.

Dostępne demonstratory:

- **Galeria materiałów** — dwanaście powierzchni na identycznych próbkach geometrycznych;
- **Rozdzielczość 128–1024** — cztery wersje tej samej tekstury przy tej samej geometrii i UV;
- **Normal map: porównanie** — albedo bez normal mapy, albedo z normal mapą oraz pełny PBR; lokalne źródło światła można przesuwać;
- **Kanały PBR** — osobne panele Color, Normal, Roughness, AO, Metalness i Displacement tego samego materiału;
- **Tiling i mipmapy** — ta sama drewniana powierzchnia z różną gęstością UV, przeznaczona do obserwacji powtarzalności wzoru i zachowania mipmap pod małym kątem.

Źródłowe mapy są kopiowane do `Assets/WiRR/Common/Textures/Generated/Surfaces`. Materiały powstają w `Assets/WiRR/Common/Materials/Generated/Surfaces`, natomiast robocze warianty porównawcze w katalogach `Generated/Quality`. Pełny materiał PBR generuje mapę Metallic/Smoothness: metalness trafia do kanału R, a smoothness jest wyznaczane jako `1 - roughness` i trafia do kanału A.

Do standardowych materiałów używany jest `NormalGL`, ponieważ Unity oczekuje map normalnych w konwencji Y+ (OpenGL). Materiały źródłowe pochodzą z biblioteki ambientCG i są udostępniane przez ambientCG na licencji CC0.

## Ruch, fizyka i dźwięk

WiRR 0.8.0 dodaje opcjonalne demonstratory działające w **Play Mode**. Są dostępne z sekcji **Scena i pomiary → Ruch, fizyka i dźwięk**.

Komponenty runtime:

- `WiRRLoopMotion` — ruch PingPong, Bob, Orbit i Rotate; może używać zwykłego Transform albo `Rigidbody.MovePosition/MoveRotation` dla obiektu kinematycznego;
- `WiRRPhysicsImpulsePad` — trigger nadający dynamicznym Rigidbody impuls i opcjonalny moment obrotowy;
- `WiRRProceduralAudio` — generuje profile Hum, Beacon, Engine i Wind bez plików WAV/OGG;
- `WiRRVehicleController` — pojazd Rigidbody z przyspieszaniem, jazdą wstecz, skrętem, przyczepnością boczną, hamowaniem i limitem prędkości.

Gotowe prefaby:

- **DYN_FloatingTeleportPlatform** — platforma okresowo unosząca się w pionie, z przestrzennym niskim humem i `TeleportAreaPlaceholder`;
- **DYN_ShuttlePlatform** — platforma odjeżdżająca między dwoma położeniami, z dźwiękiem ruchu i miejscem na XRI Teleportation Area;
- **DYN_DriveableCart** — fizyczny wózek sterowany klawiaturą lub gamepadem; `WASD`/strzałki sterują, spacja hamuje;
- **DYN_PhysicsImpulseLauncher** — pole triggera wyrzucające kule i kostki o różnych masach;
- **DYN_KineticAudioBeacon** — obracająca się konstrukcja z orbitującą sondą i proceduralnym beaconem audio.

Pojazd udostępnia metodę `SetExternalInput(Vector2 steeringAndThrottle, float brake)`, dlatego student może zastąpić wejście z klawiatury własnym panelem UI, kontrolerem XR albo innym źródłem sygnału bez modyfikacji fizyki pojazdu.

Ruchome platformy **nie mają automatycznie skonfigurowanego XRI Teleportation Area**. W prefabie znajduje się jedynie `TeleportAreaPlaceholder`, aby student sam wykonał część ćwiczenia dotyczącą teleportacji i warstw interakcji.

## Dodatkowy model Unitree G1 EDU

W katalogu głównym pakietu znajduje się `RoboAnimation.unitypackage`. Zawiera model 3D humanoidalnego robota **Unitree G1 EDU** oraz przykładowe klipy animacji. W panelu **Scena i pomiary → Dodatkowe modele 3D** można otworzyć standardowy import Unity przyciskiem **Importuj Unitree G1 EDU + animacje**. Import odbywa się przez standardowe okno Unity, dzięki czemu przed zatwierdzeniem student widzi listę dodawanych zasobów. Po imporcie model i animacje stają się zwykłymi zasobami projektu studenta i mogą być używane w scenach oraz pomiarach tak jak pozostałe modele.

Model jest opcjonalnym zasobem uzupełniającym:
- w **Lab 05** może służyć jako złożony, wieloczęściowy model porównawczy do audytu kosztu renderowania, pamięci, materiałów i LOD;
- w **Lab 06** może służyć jako dodatkowa reprezentacja wizualna robota podczas pracy z architekturą bliźniaka cyfrowego;
- w **Lab 07** może być użyty jako realistyczne obciążenie sceny w testach wydajności, stabilności i regresji.

Przykładowe animacje z pakietu służą do demonstracji ruchu. Nie są źródłem `JointState`, nie zastępują pomiarów ROS 2/WebSim i nie stanowią referencji poprawności mapowania przegubów. Import modelu jest opcjonalny i nie zmienia kryteriów zaliczenia żadnego laboratorium.

## Strona kursu (GitHub Pages)

[Strona WiRR](https://kia-students.github.io/wirr/) zawiera trzy zakładki:

- [Import do Unity](https://kia-students.github.io/wirr/#unity): instalacja UPM, zależności, próbki i walidacja sceny.
- [WebSim / Gazebo](https://kia-students.github.io/wirr/#websim): istniejąca instrukcja backendu ROS 2, lokalnie lub w LAN; WebSim jest alternatywą dla symulacji dynamiki w Gazebo.
- [SSQ](https://kia-students.github.io/wirr/#ssq): 16 objawów, pomiar przed/po, N/O/D/TS, różnica i eksport JSON. Braki odpowiedzi nie są zerami. Polskie tłumaczenie robocze nie jest zwalidowaną adaptacją. Dane nie opuszczają przeglądarki i nie są utrwalane po odświeżeniu.

Źródła strony: `WebSim~/site/`. Testy punktacji: `node --test WebSim~/tests/*.test.mjs`.
Workflow Pages uruchamia testy przed publikacją. Bieżący zakres strony jest kompletny;
pozostało 0 dodatkowych PR dla trzech zakładek. Walidacja polskiej adaptacji SSQ
pozostaje osobnym zadaniem badawczym, poza zakresem wersji dydaktycznej.

## WebSim

WebSim jest prostym klientem rosbridge używanym w ćwiczeniach robotycznych. Student podaje adres backendu, identyfikator sesji i model robota, tworzy model w scenie, uruchamia Play Mode i łączy się z backendem. Panel pokazuje jedynie stan potrzebny do wykonania ćwiczenia, m.in. `LIVE` lub `STALE`, oraz podstawowe polecenia ruchu, Home i Reset.

Gotowy backend Docker znajduje się w `WebSim~/`. Uruchamia lokalny generator ROS 2/rosbridge bez instalacji ROS 2 po stronie Unity. Dla tego samego komputera użyj `ws://127.0.0.1:9090`; dla serwera w LAN: `ws://<IP-serwera>:9090`. Szczegółowa instrukcja: [`WebSim~/README.md`](WebSim~/README.md).

Kontrakt komunikacyjny:

```text
/wirr/control
/wirr/<SESSION>/command
/wirr/<SESSION>/joint_states
/wirr/<SESSION>/tf
/wirr/<SESSION>/status
/clock
```

W Lab 6 możliwy jest również klasyczny wariant ROS 2/Gazebo z ROS-TCP-Endpoint.

## Raportowanie

Podstawową ścieżką jest `WiRR → Raporty → Formularz raportu laboratoryjnego`. Każde laboratorium ma formularz odpowiadający etapom:

`3.0 → 3.5 → 4.0 → 4.5 → 5.0`.

Formularz zawiera pola opisowe oraz tabele wyników wymagane przez dane ćwiczenie. Szkic jest automatycznie zapisywany lokalnie w `Library/WiRRReports`.

Student podaje identyfikator zespołu i 2–3 numery indeksów. Warianty zadania są wyliczane automatycznie. Do wysłania raportu wymagany jest kompletny etap 3.0. Etapy 3.5–5.0 są opcjonalne; student może zakończyć raport na dowolnym kompletnym etapie, a wyższy etap wymaga ukończenia poprzednich. Wartości jednoznacznie wynikające z danych surowych: m.in. mediany, wybrane sumy, R=P×S i statystyki RTT: formularz oblicza automatycznie i pokazuje jako pola tylko do odczytu.

Finalny raport ma schemat `wirr-report/1.0`. Zawiera wyłącznie dane raportu: identyfikację zgłoszenia, laboratorium, zespół, numery indeksów, daty oraz odpowiedzi i wyniki. Pakiet nie dołącza telemetryki pracy studenta, danych o systemie, GPU, historii plików ani innych dodatkowych metadanych środowiska.

Markdown `report-template.md` w materiałach laboratoryjnych pozostaje formatem referencyjnym i awaryjnym.

Standardowa ścieżka:

`Raport → Sprawdź → Wyślij → GitHub → walidacja CI → prowadzący`.

## Wysyłanie raportu

Przycisk **Wyślij raport** zapisuje finalny JSON w strukturze:

```text
students/reports/<team>/lab-XX/<submissionId>.json
```

i przygotowuje dedykowaną gałąź raportową. Pakiet korzysta z lokalnej konfiguracji `git`; nie przechowuje tokenu GitHub. Jeśli dostępny jest zalogowany GitHub CLI (`gh`), może utworzyć Pull Request automatycznie. W przeciwnym razie student tworzy PR z wypchniętej gałęzi. Wysyłka wymaga konta GitHub z prawem zapisu do repozytorium kursu.

## Walidacja CI

Workflow `.github/workflows/wirr-report-grade.yml` wykonuje wyłącznie kontrolę techniczną raportu. Sprawdza m.in. wersję schematu, numer laboratorium, identyfikator raportu, identyfikator zespołu oraz 2–3 poprawne i unikalne numery indeksów.

CI nie wystawia oceny merytorycznej i nie analizuje sposobu pracy studenta. Ocenę raportu wykonuje prowadzący.

Walidator uruchamiany w Pull Request jest pobierany z zaufanej gałęzi bazowej, natomiast raport pochodzi z gałęzi studenta.

## Zalecany przebieg pracy studenta

1. Utwórz projekt **Universal 3D (URP)** w wersji Unity 6000.6.x wskazanej przez prowadzącego.
2. Zainstaluj WiRR z `KIA-students/wirr`.
3. Otwórz `WiRR → Narzędzia kursu` i wybierz laboratorium.
4. Zainstaluj wymagane zależności i zaimportuj próbkę. WiRR automatycznie utworzy folder roboczy `Assets/WiRR/LabXX` i scenę bazową.
5. Otwórz przygotowaną scenę, w razie potrzeby użyj funkcji naprawy sceny i uruchom walidację.
6. W sekcji **Zadania laboratoryjne** wykonuj checklistę punkt po punkcie; po każdym punkcie kontrolnym zapisz wymagany dowód w formularzu raportu.
7. Uzupełnij formularz raportu do osiągniętego etapu; pola pochodne zostaną obliczone automatycznie.
8. Wybierz **Sprawdź raport**, popraw wskazane braki i użyj **Wyślij raport**.

## Identyfikacja wizualna

`icon.png` jest podstawowym logo pakietu i jest używany w dokumentacji oraz w oknach Unity **WiRR Course Toolkit** i **WiRR Raport**. `.icon.png` jest ikoną wyświetlaną przez Unity Package Manager. `icon.ico` pozostaje zasobem ikony aplikacyjnej dla środowisk wymagających formatu ICO. Zestaw faviconów jest przechowywany jako `favicon_io.zip`.

## Prywatność

WiRR Reports służy do przekazania sprawozdania, a nie do monitorowania aktywności studenta. Do repozytorium trafia raport, nie historia pracy w Unity.

## Licencja

Szczegóły znajdują się w pliku `LICENSE`.


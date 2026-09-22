# Architektura WiRR Course Toolkit

## Cel

WiRR Course Toolkit jest publicznym pakietem dydaktycznym dla przedmiotu Wirtualna i Rozszerzona Rzeczywistość. Repozytorium kursu zawiera kod pakietu, materiały startowe, narzędzia pomocnicze i dokumentację potrzebną studentom oraz osobom korzystającym z pakietu.

Pakiet ogranicza czas poświęcany na ręczne pobieranie zależności, powtarzalną konfigurację projektu i tworzenie pomocniczych elementów sceny. Nie ukrywa jednak architektury systemu i nie wykonuje za studenta zadań będących celem ćwiczenia.

Publiczne repozytorium kursu:
https://github.com/KIA-students/wirr

Strona kursu:
https://kia-students.github.io/wirr/

## Warstwy

- **Runtime**: niezależne od XRI, AR i ROS komponenty wspólne dla laboratoriów.
- **Editor**: WiRR Course Toolkit, instalator UPM, import próbek, przygotowanie sceny, generator prefabów dydaktycznych, walidator i obsługa raportu.
- **Samples~**: siedem niezależnych zestawów laboratoryjnych importowanych do projektu studenta.
- **Project~**: projekt testowy używany do rozwoju i weryfikacji pakietu; nie jest importowany przez UPM.
- **WebSim~**: backend i strona pomocnicza dla Laboratorium 06.
- **Documentation~**: dokumentacja techniczna pakietu.

## Prefaby dydaktyczne

`WiRRTeachingAssetTools` generuje zwykłe prefaby Unity bezpośrednio w projekcie studenta. Dzięki temu repozytorium pakietu pozostaje lekkie, a wygenerowane obiekty można oglądać i modyfikować w Inspectorze.

Ścieżki:

```text
Assets/WiRR/Common/
  Materials/Generated/
  Prefabs/Generated/

Assets/WiRR/LabXX/
  Prefabs/Generated/
```

Dla laboratoriów 01, 02, 05, 06 i 07 środowiskiem odniesienia jest `WiRR_LabRoom`. Dla laboratoriów 03 i 04 używany jest lekki `WiRR_ARReferenceKit`, aby wirtualne wnętrze nie zakłócało ćwiczeń opartych na rzeczywistym otoczeniu.

Generator może zostać uruchomiony ponownie. Nadpisuje wyłącznie zasoby w folderach `Generated` i obiekty pod `WiRR_TeachingAssets`. Nie usuwa obiektów studenta z pozostałej części sceny.

Prefaby są pomocami dydaktycznymi, a nie kompletnymi rozwiązaniami. Przykładowo moduł Laboratorium 02 ma Rigidbody, Collider i punkt mocowania, ale student sam konfiguruje XRI. Artefakt Laboratorium 03 nie otrzymuje automatycznie kotwicy AR. Wizualne ramię Laboratorium 06 zachowuje nazwy `joint1`, `joint2` i `joint3`, lecz student nadal wykonuje mapowanie stanu.

### Warstwa Grid

`WiRRPrefabTextureLibrary` oddziela źródłowe mapy PBR od materiałów projektu studenta. W projekcie deweloperskim źródła znajdują się w `Project~/Assets/Textures`, natomiast instalowalny pakiet udostępnia te same mapy pod `Textures/Grid`.

Przy pierwszym użyciu generator:

1. kopiuje tekstury do `Assets/WiRR/Common/Textures/Generated`;
2. ustawia normal map, sRGB, mipmapy, wrap mode i parametry importu;
3. tworzy warianty materiałów URP dla ról `Floor`, `Wall`, `Panel` i `Accent`;
4. zapisuje prefaby w `Assets/WiRR/LabXX/Prefabs/GridGenerated`;
5. umieszcza instancje w scenie pod `WiRR_TeachingAssets/GridPrefabs`.

API używa kanonicznych rodzin `grid_1`, `grid_2` i `grid_3`. Dla zgodności z bieżącą zawartością repozytorium resolver akceptuje również `grid-1_*` jako źródło `Grid1` oraz `grid-4_*` jako źródło `Grid3`.


## Biblioteka powierzchni PBR i demonstratory opcjonalne

`Textures/Surfaces` jest kuratorowanym zestawem materiałów 1K dostępnym z pakietu UPM. `WiRRSurfaceTextureLibrary` nigdy nie modyfikuje plików pakietu. Kopiuje potrzebne mapy do projektu studenta i dopiero tam ustawia importer oraz tworzy materiały.

Dla standardowych materiałów używane są:
- Color jako sRGB;
- NormalGL jako `TextureImporterType.NormalMap`;
- AO, Roughness, Metalness i Displacement jako dane liniowe;
- automatycznie pakowana mapa Metallic/Smoothness dla URP/Lit.

Struktura zasobów roboczych:

```text
Assets/WiRR/Common/
  Textures/Generated/Surfaces/
  Textures/Generated/Quality/
  Materials/Generated/Surfaces/
  Materials/Generated/Quality/
  Prefabs/Optional/
```

`WiRROptionalPrefabTools` generuje pięć niezależnych stanowisk. Są umieszczane pod `WiRR_TeachingAssets/OptionalMaterialDemos`, dlatego ich usunięcie nie narusza podstawowych prefabów laboratorium ani pracy studenta.

Stanowiska jakości tekstur powstają z kopii źródeł. Warianty 128, 256, 512 i 1024 px różnią się tylko `maxTextureSize`, dzięki czemu porównanie nie miesza rozdzielczości z inną geometrią lub UV.

## Dynamiczne komponenty dydaktyczne

Warstwa Runtime zawiera cztery niezależne komponenty demonstracyjne:

```text
Runtime/
  WiRRLoopMotion.cs
  WiRRPhysicsImpulsePad.cs
  WiRRProceduralAudio.cs
  WiRRVehicleController.cs
```

`WiRRLoopMotion` może pracować na Transform lub na kinematycznym Rigidbody. Dzięki temu ten sam komponent nadaje się zarówno do prostych obiektów dekoracyjnych, jak i ruchomych platform mających kolizję.

`WiRRVehicleController` korzysta z `Unity.InputSystem`. Domyślnie czyta klawiaturę i gamepad bez wymagania gotowego assetu Input Actions. Publiczne `SetExternalInput()` / `ClearExternalInput()` tworzą punkt integracji dla XRI, UI, ROS lub własnego kontrolera.

`WiRRProceduralAudio` generuje AudioClip w pamięci przy uruchomieniu. Nie zwiększa paczki o pliki audio i pozwala omawiać częstotliwość, harmoniczne, modulację, spatial blend, pitch oraz zależność dźwięku od prędkości pojazdu.

Dynamiczne prefaby są generowane do:

```text
Assets/WiRR/Common/Prefabs/Dynamic/
```

Ich instancje są umieszczane pod `WiRR_TeachingAssets/DynamicDemos`. Usunięcie dynamicznych demonstratorów nie ingeruje w pozostałe obiekty dydaktyczne ani pracę studenta.

Platformy zawierają obiekt `TeleportAreaPlaceholder`, lecz nie zależą od XRI. Konfiguracja Teleportation Area i warstw interakcji pozostaje zadaniem studenta.

## Zależności

Pakiet bazowy wymusza tylko URP i Input System. Pozostałe pakiety są instalowane dla konkretnego laboratorium:

| Laboratorium | Dodatkowe pakiety |
|---|---|
| 01 | XR Management, OpenXR, XRI, XR Hands |
| 02 | XR Management, OpenXR, XRI |
| 03 | XR Management, XRI, AR Foundation, ARCore |
| 04 | XR Management, XRI, AR Foundation, ARCore |
| 05 | brak dodatkowych |
| 06 | ROS-TCP-Connector v0.7.1 z Git |
| 07 | Unity Test Framework |

## Laboratorium 06

ROS 2 i Gazebo nie są zależnościami Unity. Mogą działać lokalnie lub na osobnym komputerze zgodnie z instrukcją laboratorium. WebSim pozostaje alternatywnym źródłem `JointState` przez rosbridge.

## Zasada Samples~

Każde laboratorium ma własny katalog `Samples~/LabXX`. Unity Package Manager pokazuje je jako osobne próbki, dlatego student importuje wyłącznie materiały potrzebne w aktualnym laboratorium.

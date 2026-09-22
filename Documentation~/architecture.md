# Architektura WiRR — narzędzia kursu

## Repozytoria

Źródłem prawdy dla rozwoju pakietu jest **MatPomGit/wirr**. Po przetestowaniu i wydaniu ta sama wersja jest synchronizowana do **KIA-students/wirr**, które jest repozytorium produkcyjnym instalowanym przez studentów.

Repozytorium **MatPomGit/prz** przechowuje wykłady, instrukcje laboratoryjne i zasoby potrzebne do ich budowania. Nie powinno zawierać kopii kodu pakietu, WebSim ani jego workflowów CI.

Przepływ:

```text
MatPomGit/wirr
  development + tests
        |
        v
KIA-students/wirr
  production / students

MatPomGit/prz
  lectures + laboratory instructions
  -> link to MatPomGit/wirr
```

## Cel

Pakiet redukuje czas tracony na ręczne pobieranie zależności, powtarzalną konfigurację projektu i modelowanie pomocniczych elementów sceny, ale nie ukrywa przed studentami architektury systemu ani nie wykonuje za nich zadań będących celem ćwiczenia.

## Warstwy

- **Runtime** — niezależne od XRI/AR/ROS komponenty wspólne dla laboratoriów.
- **Editor** — panel kursu, instalator UPM, import próbek, przygotowanie sceny, generator prefabów dydaktycznych, walidator i obsługa raportu.
- **Samples~** — siedem niezależnych zestawów laboratoryjnych importowanych do projektu studenta.
- **Project~** — projekt deweloperski używany do testów pakietu; nie jest importowany przez UPM.
- **WebSim~** — backend i strona pomocnicza dla Lab 06.

## Prefaby dydaktyczne

`WiRRTeachingAssetTools` generuje zwykłe prefaby Unity już w projekcie studenta. Dzięki temu repozytorium pakietu pozostaje tekstowe i stabilne, a wygenerowane obiekty można normalnie oglądać i modyfikować w Inspectorze.

Ścieżki:

```text
Assets/WiRR/Common/
  Materials/Generated/
  Prefabs/Generated/

Assets/WiRR/LabXX/
  Prefabs/Generated/
```

Dla Lab 01, 02, 05, 06 i 07 środowiskiem odniesienia jest `WiRR_LabRoom`. Dla Lab 03 i 04 używany jest lekki `WiRR_ARReferenceKit`, aby wirtualne wnętrze nie zakłócało ćwiczeń opartych na rzeczywistym otoczeniu.

Generator może zostać uruchomiony ponownie. Nadpisuje wyłącznie zasoby w folderach `Generated` i obiekty pod `WiRR_TeachingAssets`; nie usuwa obiektów studenta z pozostałej części sceny.

Prefaby są pomocami dydaktycznymi, nie kompletnymi rozwiązaniami. Przykładowo moduł Lab 02 ma Rigidbody, Collider i punkt mocowania, ale student sam konfiguruje XRI; artefakt Lab 03 nie otrzymuje automatycznie kotwicy AR; wizualne ramię Lab 06 zachowuje nazwy `joint1`–`joint3`, lecz student nadal wykonuje mapowanie stanu.

## Zależności

Pakiet bazowy wymusza tylko URP i Input System. Pozostałe pakiety są instalowane dla konkretnego laboratorium:

| Lab | Dodatkowe pakiety |
|---|---|
| 1 | XR Management, OpenXR, XRI, XR Hands |
| 2 | XR Management, OpenXR, XRI |
| 3 | XR Management, XRI, AR Foundation, ARCore |
| 4 | XR Management, XRI, AR Foundation, ARCore |
| 5 | brak dodatkowych |
| 6 | ROS-TCP-Connector v0.7.1 z Git |
| 7 | Unity Test Framework |

## Lab 6

ROS 2 i Gazebo nie są zależnościami Unity. Mogą działać lokalnie lub na osobnym komputerze zgodnie z instrukcją laboratorium. WebSim pozostaje alternatywnym źródłem `JointState` przez rosbridge.

## Zasada Samples~

Każde laboratorium ma własny katalog `Samples~/LabXX`. Unity Package Manager pokazuje je jako osobne próbki, dlatego student importuje wyłącznie materiały potrzebne w aktualnym laboratorium.

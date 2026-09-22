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

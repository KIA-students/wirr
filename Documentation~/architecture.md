# Architektura WiRR Course Toolkit

## Cel

Pakiet redukuje czas tracony na ręczne pobieranie zależności i powtarzalną konfigurację projektu, ale nie ukrywa przed studentami architektury systemu ani wyników walidacji.

## Warstwy

- **Runtime** — niezależne od XRI/AR/ROS komponenty wspólne dla laboratoriów.
- **Editor** — panel kursu, instalator UPM, import próbek, przygotowanie sceny, walidator i obsługa raportu.
- **Samples~** — siedem niezależnych zestawów laboratoryjnych importowanych do `Assets/Samples/...`.

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

ROS 2 i Gazebo **nie są zależnościami Unity**. Są instalowane na Ubuntu zgodnie z instrukcją laboratorium. Unity może działać na osobnym komputerze i komunikować się z `ROS-TCP-Endpoint` po TCP/10000.

## Zasada Samples~

Każde laboratorium ma własny katalog `Samples~/LabXX`. Unity Package Manager pokazuje je jako osobne próbki. Panel WiRR korzysta z tego samego API, dlatego student może importować wyłącznie materiały potrzebne w aktualnym laboratorium.

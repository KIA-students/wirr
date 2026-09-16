# WiRR Course Toolkit

Pakiet UPM dla przedmiotu **Wirtualna i Rozszerzona Rzeczywistość**.

## Instalacja

W projekcie utworzonym z szablonu **Universal 3D (URP)** wybierz:

`Window → Package Manager → + → Add package from git URL...`

W repozytorium roboczym `przkia` użyj:

```text
https://github.com/MatPomGit/przkia.git?path=/courses/wirtualna-i-rozszerzona-rzeczywistosc/unity-package
```

Po przeniesieniu folderu `unity-package` do repozytorium `KIA-students/wirr` docelowy adres będzie:

```text
https://github.com/KIA-students/wirr.git?path=/unity-package
```

Po instalacji w głównym menu Unity pojawi się **WiRR**.

## Zalecany workflow studenta

1. Utwórz nowy projekt **Universal 3D (URP)** w wersji Unity wskazanej przez prowadzącego.
2. Zainstaluj `WiRR Course Toolkit` z Git URL.
3. Otwórz `WiRR → Course Toolkit`.
4. Wybierz numer laboratorium.
5. Kliknij **Install / repair lab dependencies**.
6. Zaimportuj wymagane Samples.
7. Kliknij **Create / repair base scene**.
8. Dla Lab 4 lub Lab 6 skonfiguruj **WiRR WebSim**, jeżeli używasz środowiska webowego.
9. Kliknij **Validate scene** i usuń wszystkie błędy.
10. Otwórz **Open WiRR Reports form** i wpisuj wyniki bezpośrednio w Unity.
11. Przed oddaniem użyj **Sprawdź kompletność**, a następnie **Wyślij raport i utwórz PR**.

## WiRR Reports

Od wersji `0.3.0` podstawowym sposobem przygotowania sprawozdania jest moduł **WiRR Reports**. Dotychczasowy `report-template.md` pozostaje wyłącznie formatem awaryjnym i materiałem referencyjnym.

Formularz jest inny dla każdego z laboratoriów 1–7. Pola mają stabilne identyfikatory, typy i checkpointy `3.0`, `3.5`, `4.0`, `4.5`, `5.0`, dzięki czemu wynikowy JSON może być bezpiecznie parsowany przez CI. Numery wariantów są wyliczane automatycznie z numerów indeksów 2- lub 3-osobowego zespołu.

Szkic jest zapisywany lokalnie w `Library/WiRRReports`, więc nie zaśmieca katalogu `Assets`. Finalny plik ma schemat `wirr-report/1.0` i zawiera:

- odpowiedzi i wyniki pomiarów;
- identyfikator zespołu i numery indeksów, bez nazwisk;
- wersję Unity, system operacyjny, GPU, render pipeline i hash manifestu pakietów;
- bieżący commit/branch projektu, jeżeli projekt jest repozytorium Git;
- telemetryczną oś czasu pracy;
- snapshoty stanu projektu i sceny;
- zdarzenia tworzenia, modyfikacji, przenoszenia i usuwania plików w `Assets`;
- aktywny czas Unity, czas Play Mode, kompilacje oraz czas przypisany do checkpointów.

### Prywatność telemetryki

Telemetryka jest jawna i widoczna w formularzu. Nie zapisuje treści plików, nazwy użytkownika systemu, katalogu domowego, zdjęć, danych lokalizacyjnych ani identyfikatorów sprzętowych. Dla plików projektu zapisywane są względna ścieżka, rozmiar, czas zdarzenia i SHA-256. Snapshot całego projektu wykonywany jest domyślnie co 10 minut.

### Przesyłanie raportu

Pakiet nie zawiera tokenu GitHub. Przycisk wysłania korzysta z lokalnej konfiguracji `git` (Git Credential Manager / SSH). Tworzy osobną gałąź w postaci:

```text
report/<team>/labXX-YYYYMMDD-HHMMSS
```

i umieszcza raport w:

```text
students/reports/<team>/lab-XX/<submissionId>.json
```

Jeżeli dostępny jest zalogowany GitHub CLI (`gh`), pakiet automatycznie tworzy Pull Request. Bez `gh` raport jest nadal wypychany na osobną gałąź i pozostaje gotowy do otwarcia PR.

Repozytorium kursu zawiera workflow `wirr-report-grade.yml` oraz skrypt `scripts/wirr_grade_report.py`. Workflow przygotowuje wyłącznie **propozycję oceny** na podstawie kompletności kolejnych checkpointów i metadanych. Zawsze ustawia `requiresInstructorApproval=true`; prowadzący zatwierdza lub zmienia wynik.

## WiRR WebSim

Pakiet obsługuje trzy źródła środowiska robotycznego dla Lab 6:

- lokalne ROS 2 + Gazebo na tym samym komputerze;
- ROS 2 + Gazebo na drugim komputerze w LAN;
- **WiRR WebSim** — Gazebo + ROS 2 po stronie backendu, a Unity łączy się przez `rosbridge` WebSocket/WSS.

W panelu Lab 4 i Lab 6 podaj adres backendu `wss://...`, kod zespołu / sesji oraz model `RRBot 2R` albo `WiRR Arm 3R`. Ten sam `WiRRRobotRig` jest używany w obu laboratoriach: w Lab 4 jako cyfrowy cień, w Lab 6 jako reprezentacja bliźniaka cyfrowego.

Kontrakt WebSim:

```text
/wirr/control
/wirr/<SESSION>/command
/wirr/<SESSION>/joint_states
/wirr/<SESSION>/tf
/wirr/<SESSION>/status
/clock
```

## Założenia

- Pakiet nie ukrywa błędów konfiguracji i nie zastępuje świadomego wykonania ćwiczenia.
- Zależności laboratoryjne są instalowane sekwencyjnie przez Unity Package Manager.
- Materiały laboratoriów są rozdzielone jako `Samples~`.
- Kod Runtime nie zależy od XRI, AR Foundation ani ROS-TCP-Connector.
- Lab 6 instaluje ROS-TCP-Connector automatycznie dla trybu klasycznego; WebSim nie wymaga lokalnego ROS 2 ani Gazebo na komputerze z Unity.
- Automatyczna propozycja oceny nie jest oceną końcową i nie zastępuje recenzji prowadzącego.

## Struktura

```text
unity-package/
├── package.json
├── Runtime/
│   ├── WiRRReportData.cs
│   ├── IRobotStateSource.cs
│   ├── RobotState.cs
│   ├── WebSimStateSource.cs
│   └── WiRRRobotRig.cs
├── Editor/
│   ├── WiRRReportWindow.cs
│   ├── WiRRReportSchemaCatalog.cs
│   ├── WiRRTelemetryRecorder.cs
│   ├── WiRRGitSubmission.cs
│   └── WiRRWebSimTools.cs
├── Samples~/
│   ├── Lab01/
│   ├── ...
│   └── Lab07/
└── Documentation~/
```

## Wersjonowanie

Aktualna wersja pakietu to `0.3.0`. Po przeniesieniu do repozytorium studenckiego zalecane jest tagowanie wydań i instalowanie przez URL przypięty do konkretnego tagu.

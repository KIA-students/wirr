<p align="center">
  <img src="icon.png" alt="WiRR" width="128">
</p>

# WiRR Course Toolkit

Pakiet Unity Package Manager (UPM) dla przedmiotu **Wirtualna i Rozszerzona Rzeczywistość**. WiRR przygotowuje środowisko laboratoriów 1–7, pomaga sprawdzić konfigurację, udostępnia potrzebne narzędzia pomiarowe i prowadzi studenta do złożenia raportu.

`MatPomGit/wirr` jest repozytorium roboczym. Po zakończeniu weryfikacji jego zawartość jest kopiowana 1:1 do publicznego `KIA-students/wirr`, z którego korzystają studenci.

## Zasada działania

WiRR automatyzuje infrastrukturę, a nie wykonanie ćwiczenia. Student nadal sam implementuje rozwiązanie, wykonuje pomiary i interpretuje wyniki zgodnie z instrukcją laboratoryjną.

Pakiet zawiera:

- **Course Toolkit** — główny panel laboratorium;
- **Dependency Installer** — instalację wymaganych pakietów Unity;
- **Samples Manager** — import materiałów dla wybranego laboratorium;
- **Scene Tools / Validator** — przygotowanie i kontrolę sceny;
- **Runtime Utilities** — lekkie komponenty pomiarowe;
- **WebSim** — połączenie Unity ze środowiskiem ROS 2/Gazebo przez rosbridge;
- **WiRR Reports** — formularz, autosave, tabele pomiarowe i raport JSON;
- **Git Submission** — wysłanie raportu do repozytorium;
- **Report Validation CI** — techniczną kontrolę integralności raportu.

## Instalacja

W Unity wybierz:

`Window → Package Manager → + → Install package from git URL...`

Repozytorium studenckie:

```text
https://github.com/KIA-students/wirr.git
```

Pakiet znajduje się w katalogu głównym repozytorium. Po instalacji w menu Unity pojawi się **WiRR**. Wymagana wersja to Unity **6000.6.x** lub nowsza zgodna wersja 6000.6.

## Struktura repozytorium

Katalog główny repozytorium jest czystym pakietem UPM. Kod pakietu znajduje się w `Editor/` i `Runtime/`, a materiały importowane przez Package Manager w `Samples~/`.

Pełny projekt Unity używany do rozwoju i testowania pakietu znajduje się w `Project~/`. Sufiks `~` powoduje, że Unity Package Manager nie importuje tego katalogu do projektu studenta, dzięki czemu `Assets`, `Packages` i `ProjectSettings` projektu deweloperskiego nie powodują konfliktów GUID z projektem użytkownika. Po sklonowaniu `MatPomGit/wirr` do pracy nad pakietem otwórz w Unity katalog `Project~`, nie katalog główny repozytorium.

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

Materiały startowe znajdują się w `Samples~/Lab01`–`Samples~/Lab07`.

## WebSim

WebSim jest prostym klientem rosbridge używanym w ćwiczeniach robotycznych. Student podaje adres backendu, identyfikator sesji i model robota, tworzy model w scenie, uruchamia Play Mode i łączy się z backendem. Panel pokazuje jedynie stan potrzebny do wykonania ćwiczenia, m.in. `LIVE` lub `STALE`, oraz podstawowe polecenia ruchu, Home i Reset.

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

Podstawową ścieżką jest `WiRR → Reports → Laboratory report form`. Każde laboratorium ma formularz odpowiadający checkpointom:

`3.0 → 3.5 → 4.0 → 4.5 → 5.0`.

Formularz zawiera pola opisowe oraz tabele wyników wymagane przez dane ćwiczenie. Szkic jest automatycznie zapisywany lokalnie w `Library/WiRRReports`.

Student podaje identyfikator zespołu i 2–3 numery indeksów. Warianty zadania są wyliczane automatycznie. Student może zakończyć raport na dowolnym kompletnym checkpointcie; wyższy checkpoint wymaga ukończenia poprzednich.

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

## Zalecany workflow studenta

1. Utwórz projekt **Universal 3D (URP)** w wersji Unity 6000.6.x wskazanej przez prowadzącego.
2. Zainstaluj WiRR z `KIA-students/wirr`.
3. Otwórz `WiRR → Course Toolkit` i wybierz laboratorium.
4. Zainstaluj wymagane zależności i zaimportuj Sample.
5. Utwórz lub napraw scenę bazową i uruchom walidację.
6. Wykonaj zadania i pomiary zgodnie z instrukcją.
7. Uzupełnij formularz WiRR Reports do osiągniętego checkpointu.
8. Wybierz **Sprawdź raport**, popraw wskazane braki i użyj **Wyślij raport**.

## Identyfikacja wizualna

`icon.png` jest podstawowym logo pakietu i jest używany w dokumentacji oraz w oknach Unity **WiRR Toolkit** i **WiRR Report**. `.icon.png` jest ikoną wyświetlaną przez Unity Package Manager. `icon.ico` pozostaje zasobem ikony aplikacyjnej dla środowisk wymagających formatu ICO. Zestaw faviconów jest przechowywany jako `favicon_io.zip`.

## Prywatność

WiRR Reports służy do przekazania sprawozdania, a nie do monitorowania aktywności studenta. Do repozytorium trafia raport, nie historia pracy w Unity.

## Licencja

Szczegóły znajdują się w pliku `LICENSE`.

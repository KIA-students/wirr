# Changelog

## Unreleased

- dodano `WebSim~/`: gotowy backend Docker Compose dla Lab 06 z ROS 2 Jazzy, rosbridge i deterministycznym generatorem `JointState` per sesja;
- backend obsługuje `RRBot 2R`, `WiRR Arm 3R`, komendy `Motion A/B/C`, `Home`, `Reset`, status i `/clock` zgodnie z klientem Unity;
- dodano instrukcję uruchomienia na tym samym komputerze oraz przez LAN.
- dodano workflow CI budujący kontener i sprawdzający utworzenie sesji ROS 2 oraz publikację `JointState`.
- poprawiono inicjalizację środowiska ROS 2 w kontenerze dla powłok z włączonym `nounset`.
- usunięto kolizję nazwy wewnętrznego zegara `rclpy` z publisherem `/clock`.
- dodano statyczną stronę WebSim i workflow publikacji przez GitHub Pages.
- rozbudowano stronę WebSim o instrukcję krok po kroku: przygotowanie Dockera, weryfikację topików, konfigurację Unity, diagnostykę i zakończenie pracy.

## 0.4.2 — 2026-09-17

- rozdzielono instalowalny pakiet UPM od referencyjnego projektu Unity: projekt deweloperski znajduje się teraz w `Project~/` i nie jest importowany do projektu studenta;
- usunięto źródło konfliktów GUID powodowanych przez duplikowanie `Assets`, `Packages` i `ProjectSettings` wewnątrz `Packages/pl.prz.kia.wirr`;
- dodano brakujący `Editor/WiRRReportEvaluator.cs.meta`, dzięki czemu evaluator jest poprawnie importowany i dostępny dla `WiRRReportWindow`;
- dodano pliki `.meta` do zasobów widocznych w głównym katalogu pakietu;
- dokumentację referencyjną przeniesiono do `Documentation~/`, a walidator CI do `scripts~/`, aby Unity nie importowało plików technicznych jako zasobów pakietu;
- minimalną wersję pakietu ustawiono na Unity `6000.6`;
- zaktualizowano workflow walidacji raportów do nowej ścieżki `scripts~/wirr_grade_report.py`.

## 0.4.1 — 2026-09-17

- dodano dedykowaną ikonę pakietu dla Unity Package Manager na podstawie logo WiRR;
- rozbudowano główny opis pakietu o zakres funkcjonalny i workflow `Raport → Sprawdź → Wyślij`;
- rozbudowano opisy próbek Lab 01–07 o cele oraz kluczowe etapy odpowiadające checkpointom `3.0 → 3.5 → 4.0 → 4.5 → 5.0`;
- doprecyzowano pełne nazwy laboratoriów w widoku Samples.

## 0.4.0 — 2026-09-17

- uproszczono raportowanie do jednego formularza: `wypełnij → sprawdź → wyślij`;
- lokalny `WiRRReportEvaluator` sprawdza checkpointy sekwencyjnie `3.0 → 3.5 → 4.0 → 4.5 → 5.0`;
- kontrola lokalna wymaga pól oznaczonych jako wymagane oraz obecności danych pomiarowych dla danego checkpointu;
- uproszczono dokument raportu `wirr-report/1.0` do danych potrzebnych do sprawozdania; usunięto telemetrykę przebiegu pracy z raportu;
- wysyłka korzysta ze stałego repozytorium kursu `KIA-students/wirr` i ścieżki `students/reports`;
- repozytoryjny skrypt walidacyjny wykonuje podstawową kontrolę integralności raportu w CI; ocena merytoryczna pozostaje po stronie prowadzącego;
- Lab 04 uporządkowano zgodnie z aktualnym przebiegiem ćwiczenia: Depth API → okluzja → depth-raycast → estymacja oświetlenia → kontrolowany błąd i diagnoza;
- WebSim pozostawiono wyłącznie dla Lab 06 jako opcjonalne źródło stanu bliźniaka cyfrowego;
- uproszczono dokumentację i interfejs Course Toolkit zgodnie z zasadą KISS;
- poprawiono obsługę uruchamiania połączenia WebSim w Play Mode.

## 0.3.0 — 2026-09-16

- `WiRR Reports` — okienkowe formularze raportów Lab 01–07 bez konieczności ręcznej edycji Markdown;
- stabilny format `wirr-report/1.0` do automatycznego parsowania;
- wspólne dane zespołu i automatyczne wyliczanie wariantów dla zespołów 2- i 3-osobowych;
- autosave szkiców w `Library/WiRRReports`;
- eksperymentalna telemetryka projektu i metadane środowiska;
- wysyłka raportu na osobną gałąź Git i automatyczne tworzenie PR przez `gh`, jeśli jest dostępne;
- dotychczasowy `report-template.md` pozostaje trybem awaryjnym.

## 0.2.0 — 2026-09-16

- WiRR WebSim jako trzeci tryb Lab 6 obok lokalnego i sieciowego ROS 2/Gazebo;
- `WebSimStateSource` — klient rosbridge WebSocket bez dodatkowej biblioteki Unity;
- wspólny kontrakt `IRobotStateSource` / `RobotState`;
- `WiRRRobotRig` do odwzorowania stanu przegubów;
- wybór RRBot 2R lub WiRR Arm 3R;
- heartbeat i wykrywanie STALE;
- wspólne definicje robotów z backendem WebSim.

## 0.1.0 — 2026-09-16

- pierwsza wersja pakietu UPM dla kursu WiRR;
- panel `WiRR → Course Toolkit`;
- instalacja zależności per laboratorium;
- import próbek WiRR i oficjalnych próbek Unity;
- przygotowanie sceny bazowej;
- walidacja projektu i sceny;
- komponent pomiarowy FPS / frame time / pamięć;
- generowanie i otwieranie kopii pustego szablonu sprawozdania;
- osobne `Samples~` dla laboratoriów 1–7.

# Changelog

## 0.3.0 — 2026-09-16

- `WiRR Reports` — okienkowe formularze raportów Lab 01–07 bez konieczności ręcznej edycji Markdown;
- stabilny format `wirr-report/1.0` do automatycznego parsowania;
- wspólne dane zespołu i automatyczne wyliczanie wariantów dla zespołów 2- i 3-osobowych;
- autosave szkiców w `Library/WiRRReports`;
- jawna telemetryka projektu: snapshot co 10 min, stan sceny, Play Mode, kompilacje i zdarzenia plikowe;
- SHA-256 plików projektu bez przesyłania ich treści;
- agregacja aktywnego czasu pracy per checkpoint `COMMON`, `3.0`, `3.5`, `4.0`, `4.5`, `5.0`;
- eksport finalnego JSON z metadanymi Unity, render pipeline, manifestu pakietów i Git;
- wysyłka raportu na osobną gałąź Git i automatyczne tworzenie PR przez `gh`, jeśli jest dostępne;
- repozytoryjny workflow `wirr-report-grade.yml` i konserwatywna propozycja oceny wymagająca zatwierdzenia prowadzącego;
- dotychczasowy `report-template.md` pozostaje trybem awaryjnym.

## 0.2.0 — 2026-09-16

- WiRR WebSim jako trzeci tryb Lab 6 obok lokalnego i sieciowego ROS 2/Gazebo;
- `WebSimStateSource` — klient rosbridge WebSocket bez dodatkowej biblioteki Unity;
- wspólny kontrakt `IRobotStateSource` / `RobotState`;
- `WiRRRobotRig` do odwzorowania stanu przegubów;
- panel WebSim w `WiRR → Course Toolkit` dla Lab 4 i Lab 6;
- tworzenie cyfrowego cienia w Lab 4 i bliźniaka WebSim w Lab 6;
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

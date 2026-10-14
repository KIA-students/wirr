# Wymagania punktów kontrolnych WiRR

Wersja 1.0.0

> **Status tabeli 3.0–5.0:** poniższa macierz jest materiałem historycznym / rozszerzającym. Nie określa obecnie obowiązkowej ścieżki zaliczenia Lab 01–02. Wymagania bieżące i pojedynczy rezultat do oddania znajdują się na początku instrukcji [Lab 01](../../Samples~/Lab01/README.md) i [Lab 02](../../Samples~/Lab02/README.md). Pozostałe laboratoria będą ujednolicone w kolejnych etapach aktualizacji. Nie należy uzależniać zaliczenia Lab 01–02 od sprzętu Quest lub wariantów v1–v5.

## Spis treści
- [Środowisko i raport](#środowisko-i-raport)
- [Etapy laboratoriów](#etapy-laboratoriów)
- [Dowody i ograniczenia](#dowody-i-ograniczenia)

## Środowisko i raport
Praca jest samodzielna. Warianty v1–v5 wynikają z jednego numeru indeksu. Unity: 6000.6.5f1 lub nowsza zgodna poprawka 6000.6.x. Rzeczywiste wersje pakietów i tryb sprzętu zapisz w raporcie.

## Etapy laboratoriów
Każdy wyższy etap obejmuje wcześniejsze wymagania. Wiersze poniżej odpowiadają instrukcjom, panelowi zadań i formularzowi raportu.

| Lab | 3.0 | 3.5 | 4.0 | 4.5 | 5.0 |
|---|---|---|---|---|---|
| 01 | Baseline PC i v1 | CPU i GC | Renderowanie | Fizyka | Quest standalone i transfer PC–Quest |
| 02 | Chwyt, socket i ray | UI i feedback | Lokomocja | Scenariusz i diagnostyka | Quest i SSQ |
| 03 | Śledzenie i płaszczyzny | Raycast i skala 1:1 | Kotwica i dryf | Rejestracja dwupunktowa | Kontrolowany błąd i diagnoza |
| 04 | Głębia i diagnostyka | Okluzja | Pomiar odległości z głębi | Estymacja oświetlenia | Kontrolowany błąd i diagnoza |
| 05 | Import i topologia | Triangulacja A/B/C | LOD i benchmark | Materiały, tekstury i kolizje | Błąd, naprawa i model finalny |
| 06 | LOCAL/LAN/WEBSIM | Mapowanie JointState | Bufor i interpolacja | RTT, ≥50 próbek na warunek | STALE, awaria i odzyskanie |
| 07 | Kryteria i smoke test | Wydajność | Użyteczność | Awaria i regresja | Automatyzacja, ryzyko i akceptacja |
| 08 | Problem i działająca scena | Anulowanie i test negatywny | Sześć prób A/B | Poprawka i regresja | Integracja i odtworzenie |

## Dowody i ograniczenia
Wymagane serie i interpretacje opisują [próbki laboratoriów](INDEX.md). Szablon `Samples~/LabNN/report-template.md` odpowiada szablonowi osadzonemu w PDF. Brak pomiaru oznacz NV; niewspierana funkcja nie jest wynikiem zerowym. WebSim nie dostarcza gotowego próbnika RTT ani bufora interpolacji: dla tych etapów przygotuj własne komponenty lub przejdź na LOCAL/LAN. Dla RRBot dwa rzeczywiste przeguby wystarczają, joint3 oznacz N/A.

Użyj **WiRR → Raporty → Formularz raportu laboratoryjnego → Sprawdź kompletność raportu → Eksportuj raport lokalnie**. JSON zapisywany jest lokalnie. Przekaż go kanałem wskazanym przez prowadzącego. Eksport nie oznacza oddania ani zaliczenia.

Opcjonalna [synchronizacja postępu](STUDENT_GITHUB_REPORTING.md) zgłasza techniczny stan pracy. Nie przesyła pełnego raportu i nie zastępuje jego oddania.

W Lab 02 etap 3.0 wymaga 5 prób na każdy warunek własnego wariantu. Etapy 3.5 i 4.0 również obejmują po 5 prób na warunek; etap 5.0 obejmuje 3 próby w A i 3 w B. Quest 3 i SSQ PRE/POST dotyczą etapu 5.0. Każdy student bada własną ekspozycję.

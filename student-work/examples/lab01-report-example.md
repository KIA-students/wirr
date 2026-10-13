# Lab 01. Przykładowy raport (fikcyjne dane)

> **WYŁĄCZNIE PRZYKŁAD.** Dane zostały wymyślone w celu pokazania sposobu zapisu pomiarów. Nie są wynikami wykonanych eksperymentów ani pracą studenta. Przedstawiono tylko punkt kontrolny **3.0**.

## Konfiguracja

- Identyfikator: **DEMO**, bez rzeczywistego numeru indeksu ani grupy.
- Środowisko: Unity 6.6, Universal 3D (URP), WiRR Course Toolkit.
- Tryb: Unity Editor na PC, bez gogli; `sampleFrames=180`.
- Kontrola: scena otwiera się, brak błędów kompilacji, Profiler rejestruje FPS i czas klatki.
- Pytanie badawcze: czy wzrost liczby renderowanych sześcianów pogarsza czas klatki?
- Hipoteza: więcej obiektów zwiększy medianę czasu klatki.
- Stałe: jedna rozdzielczość, te same ustawienia graficzne i sprzęt, jeden czynnik zmieniany jednocześnie.

## Przykładowe pomiary

Po odrzuceniu odczytu stabilizacyjnego wykonano trzy odczyty dla każdego warunku.

| Warunek | Obiekty | FPS: próba 1 | FPS: próba 2 | FPS: próba 3 | Mediana FPS | Czas klatki 1 [ms] | 2 [ms] | 3 [ms] | Mediana [ms] |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| A | 20 | 90 | 89 | 91 | **90** | 11,11 | 11,24 | 10,99 | **11,11** |
| B | 200 | 74 | 72 | 73 | **73** | 13,51 | 13,89 | 13,70 | **13,70** |

Różnica mediany czasu klatki: `13,70 − 11,11 = +2,59 ms`. Pomocnicza zmiana mediany FPS: `(73 − 90) / 90 × 100% = −18,9%`.

**Przykładowy wniosek:** w przyjętych fikcyjnych warunkach wzrost liczby obiektów zwiększył czas klatki. Same FPS nie pozwalają stwierdzić, czy przyczyną jest CPU, GPU czy inne obciążenie. Do identyfikacji wąskiego gardła potrzebna jest analiza Profiler.

## Status

- Punkt kontrolny 3.0: **zilustrowany**.
- Punkty 3.5, 4.0, 4.5: **niewykonane, brak danych**.
- Punkt 5.0 (Quest 3): **niewykonany**, nie wpisano fikcyjnych wyników.
- Rzeczywista praca powinna zostać zapisana w formularzu WiRR Reports i wyeksportowana lokalnie.

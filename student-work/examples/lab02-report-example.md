# Lab 02. Przykładowy raport (fikcyjne dane)

> **WYŁĄCZNIE PRZYKŁAD.** Liczby i przebieg badania są demonstracyjne. Nie są pracą studenta ani rzeczywistymi pomiarami SSQ. Przedstawiono tylko punkt kontrolny **3.0**.

## Konfiguracja i test interakcji

- Identyfikator: **DEMO**, bez numeru indeksu i grupy.
- Scena: `Assets/WiRR/Lab02/Scenes/Lab02_Interaction.unity`.
- Środowisko: Unity 6.6, Input System (New), XR Interaction Toolkit, Starter Assets i XR Interaction Simulator.
- Tryb: Unity Editor, bez HMD.
- Kontrola komponentów: jedna Main Camera, XR Origin i XR Interaction Manager. Direct Grab, Socket i Ray działają prawidłowo. Socket odrzuca obiekt niewłaściwej warstwy.

## Eksperyment 3.0

**Pytanie badawcze:** czy większa odległość początkowa chwytanego modułu wydłuża czas osadzenia w gnieździe?

**Hipoteza przed pomiarem:** mediana czasu w wariancie B będzie większa.

- A: moduł bliżej operatora.
- B: moduł dalej od operatora.
- Zmienia się wyłącznie odległość początkowa. Jedna próba zapoznawcza nie jest wliczana.
- Dla A i B wykonano po pięć prób. Czas liczony od Start do poprawnego osadzenia.

| Warunek | Próba 1 [s] | 2 | 3 | 4 | 5 | Mediana [s] | Liczba błędów |
|---|---:|---:|---:|---:|---:|---:|---:|
| A | 6,2 | 6,0 | 6,4 | 6,1 | 6,3 | **6,2** | 0 |
| B | 8,4 | 8,1 | 8,7 | 8,3 | 8,5 | **8,4** | 1 |

Różnica median: `8,4 − 6,2 = +2,2 s`.

**Przykładowy wniosek:** w fikcyjnych danych warunek B wymagał więcej czasu i zawierał jedną błędną próbę. Danych nie można uogólniać na użytkowników rzeczywistych gogli.

## Status i raport

- Punkt 3.0: **zilustrowany**.
- Punkty 3.5 (UI), 4.0 (lokomocja), 4.5 (diagnostyka): **niewykonane**.
- Punkt 5.0: **niewykonany**. Nie badano Quest 3, nie wpisano fikcyjnych SSQ PRE/POST.

**W prawdziwym raporcie:** otwórz `WiRR → Raporty → Formularz raportu laboratoryjnego`, wybierz Lab 02, wpisz warunki, wszystkie próby, błędy, mediany i wnioski. Uruchom **Sprawdź kompletność raportu**, potem **Eksportuj raport lokalnie**. Eksport do `Library/WiRRReports/exports` nie jest równoznaczny z oddaniem pracy.

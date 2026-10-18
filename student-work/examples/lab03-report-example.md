# Lab 03. Przykładowe sprawozdanie: Rejestracja i kotwice AR

> **PRZYKŁAD DYDAKTYCZNY, DANE FIKCYJNE.** Żadna liczba nie pochodzi z pomiaru rzeczywistego studenta lub sprzętu. Pokazano jedną demonstracyjną sekcję raportu na poziomie 3.0, nie całe zaliczone laboratorium.

## Identyfikacja i środowisko

- Student: `DEMO`, bez numeru indeksu i przydziału do grupy.
- Laboratorium: 03; poziom demonstracji: **3.0**.
- Temat: Śledzenie AR i detekcja płaszczyzn.
- Cel: Czas do pierwszego stanu SessionTracking i pierwszej użytecznej płaszczyzny na urządzeniu z ARCore.
- Warunki: Oświetlenie: A rozproszone, B słabsze, C mocniejsze.

## Wyniki demonstracyjne

| Warunek | Próba 1 tracking / plane [s] | Próba 2 [s] | Próba 3 [s] | Mediana tracking [s] | Mediana plane [s] |
|---|---:|---:|---:|---:|---:|
| A | 2,1 / 4,0 | 2,2 / 4,1 | 2,0 / 3,9 | 2,1 | 4,0 |
| B | 3,0 / 6,1 | 3,2 / 6,4 | 3,1 / 6,2 | 3,1 | 6,2 |
| C | 1,9 / 3,6 | 1,8 / 3,5 | 2,0 / 3,7 | 1,9 | 3,6 |

notTrackingReason: brak w A/B/C; urządzenie, tryb śledzenia i warunki oświetlenia muszą być opisane w prawdziwym raporcie.

## Wniosek z przykładu

W demonstracyjnych danych słabsze oświetlenie wydłuża obydwa czasy; nie dowodzi to ogólnej zależności bez prawdziwych powtórzeń i kontroli otoczenia.

## Co należy uzupełnić w pełnym sprawozdaniu?

Do rzeczywistego raportu wpisz datę, numer indeksu i grupę **tylko do lokalnego formularza**, wersje narzędzi, wariant przypisany studentowi, warunki, surowe pomiary, obliczenia, ograniczenia i wniosek. Wyższe poziomy wymagają dodatkowych doświadczeń: 3.5: raycast i skala 1:1; 4.0: kotwica i dryf; 4.5: rejestracja dwupunktowa; 5.0: błąd kontrolowany.

**Status:** 3.0 pokazano wyłącznie jako przykład; wszystkie faktyczne pomiary i poziomy 3.5–5.0 są **niewykonane**. Nie deklaruj zrealizowanych testów sprzętu, których nie było.

## Zapis i wysyłanie

W Unity: **WiRR → Raporty → Formularz raportu laboratoryjnego** → wybierz Lab 03 → **Sprawdź kompletność raportu** → **Zapisz raport lokalnie (bez logowania)**. Zapis na komputerze nie oznacza przesłania raportu. Wysłanie go do zewnętrznego systemu jest osobnym procesem.

[Spis przykładowych raportów](../README.md)

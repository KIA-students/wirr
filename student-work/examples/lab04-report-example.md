# Lab 04. Przykładowe sprawozdanie: Rozumienie sceny i mieszanie rzeczywistości

> **PRZYKŁAD DYDAKTYCZNY, DANE FIKCYJNE.** Żadna liczba nie pochodzi z pomiaru rzeczywistego studenta lub sprzętu. Pokazano jedną demonstracyjną sekcję raportu na poziomie 3.0, nie całe zaliczone laboratorium.

## Identyfikacja i środowisko

- Student: `DEMO`, bez numeru indeksu i przydziału do grupy.
- Laboratorium: 04; poziom demonstracji: **3.0**.
- Temat: Dostępność Depth API.
- Cel: Udział prób, w których aplikacja zwróciła poprawną klatkę głębi.
- Warunki: A stabilne światło, B słabsze światło, C powierzchnia o niskiej teksturze.

## Wyniki demonstracyjne

| Warunek | Próba 1 (1/0) | 2 | 3 | Sukcesy | successRate |
|---|---:|---:|---:|---:|---:|
| A | 1 | 1 | 1 | 3/3 | 100% |
| B | 1 | 0 | 1 | 2/3 | 66,7% |
| C | 0 | 1 | 0 | 1/3 | 33,3% |

1 oznacza dostępność klatki spełniającej z góry ustalone kryterium; 0 to niepowodzenie pomiaru. Gdy urządzenie w ogóle nie wspiera Depth API, wpisz **NA**, nie zero.

## Wniosek z przykładu

W fikcyjnej serii dane głębi były mniej dostępne w warunku C; nie wolno utożsamiać tych wyników z jakością rzeczywistego urządzenia.

## Co należy uzupełnić w pełnym sprawozdaniu?

Do rzeczywistego raportu wpisz datę, numer indeksu i grupę **tylko do lokalnego formularza**, wersje narzędzi, wariant przypisany studentowi, warunki, surowe pomiary, obliczenia, ograniczenia i wniosek. Wyższe poziomy wymagają dodatkowych doświadczeń: 3.5: okluzja Off/Raw/Smoothed; 4.0: hit/miss i błąd głębi; 4.5: estymacja oświetlenia; 5.0: błąd kontrolowany.

**Status:** 3.0 pokazano wyłącznie jako przykład; wszystkie faktyczne pomiary i poziomy 3.5–5.0 są **niewykonane**. Nie deklaruj zrealizowanych testów sprzętu, których nie było.

## Zapis i wysyłanie

W Unity: **WiRR → Raporty → Formularz raportu laboratoryjnego** → wybierz Lab 04 → **Sprawdź kompletność raportu** → **Zapisz raport lokalnie (bez logowania)**. Zapis na komputerze nie oznacza przesłania raportu. Wysłanie go do zewnętrznego systemu jest osobnym procesem.

[Spis przykładowych raportów](../README.md)

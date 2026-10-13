# Lab 05. Przykładowe sprawozdanie: Optymalizacja modeli CAD

> **PRZYKŁAD DYDAKTYCZNY, DANE FIKCYJNE.** Żadna liczba nie pochodzi z pomiaru rzeczywistego studenta lub sprzętu. Pokazano jedną demonstracyjną sekcję raportu na poziomie 3.0, nie całe zaliczone laboratorium.

## Identyfikacja i środowisko

- Student: `DEMO`, bez numeru indeksu i przydziału do grupy.
- Laboratorium: 05; poziom demonstracji: **3.0**.
- Temat: Import i audyt modelu CAD.
- Cel: Porównanie geometrii po przekształceniu modelu STEP do formatów obsługiwanych przez Unity.
- Warunki: Źródło: makerbeam_bracket_90degree.stp; warianty po konwersji A/B/C są demonstracyjne.

## Wyniki demonstracyjne

| Eksport | Wierzchołki | Trójkąty | Submeshes | Wymiar odniesienia [mm] | Wymiar po imporcie [mm] |
|---|---:|---:|---:|---:|---:|
| A: OBJ | 1200 | 2200 | 1 | 40,0 | 40,0 |
| B: FBX | 1200 | 2200 | 1 | 40,0 | 40,0 |
| C: STL | 1200 | 2200 | 1 | 40,0 | 40,0 |

W raporcie rzeczywistym dodaj rozmiar pliku, topologię (boundary, non-manifold, degenerate), q05/qMedian, materiał, UV, licencję źródła i łańcuch konwersji. Przykładowe liczby nie są charakterystyką modelu MakerBeam.

## Wniosek z przykładu

W fikcyjnym zestawieniu trzy reprezentacje zachowują wymiar referencyjny; sama zgodność liczby trójkątów nie dowodzi zachowania materiałów, UV ani hierarchii.

## Co należy uzupełnić w pełnym sprawozdaniu?

Do rzeczywistego raportu wpisz datę, numer indeksu i grupę **tylko do lokalnego formularza**, wersje narzędzi, wariant przypisany studentowi, warunki, surowe pomiary, obliczenia, ograniczenia i wniosek. Wyższe poziomy wymagają dodatkowych doświadczeń: 3.5: jakość triangulacji; 4.0: LOD i FPS/ms; 4.5: materiały, draw calls i kolizje; 5.0: pipeline i błąd kontrolowany.

**Status:** 3.0 pokazano wyłącznie jako przykład; wszystkie faktyczne pomiary i poziomy 3.5–5.0 są **niewykonane**. Nie deklaruj zrealizowanych testów sprzętu, których nie było.

## Zapis i wysyłanie

W Unity: **WiRR → Raporty → Formularz raportu laboratoryjnego** → wybierz Lab 05 → **Sprawdź kompletność raportu** → **Zapisz raport lokalnie (bez logowania)**. Zapis na komputerze nie oznacza przesłania raportu. Wysłanie go do zewnętrznego systemu jest osobnym procesem.

[Spis przykładowych raportów](../README.md)

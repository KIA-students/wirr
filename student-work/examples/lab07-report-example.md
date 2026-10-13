# Lab 07. Przykładowe sprawozdanie: Walidacja i testy akceptacyjne XR

> **PRZYKŁAD DYDAKTYCZNY, DANE FIKCYJNE.** Żadna liczba nie pochodzi z pomiaru rzeczywistego studenta lub sprzętu. Pokazano jedną demonstracyjną sekcję raportu na poziomie 3.0, nie całe zaliczone laboratorium.

## Identyfikacja i środowisko

- Student: `DEMO`, bez numeru indeksu i przydziału do grupy.
- Laboratorium: 07; poziom demonstracji: **3.0**.
- Temat: Kryteria akceptacji i smoke test.
- Cel: Ocena pięciu testów typu smoke na podstawie wcześniej ustalonych oczekiwań.
- Warunki: System demonstracyjny: scena PC z Lab 02, bez fizycznego HMD.

## Wyniki demonstracyjne

| Test | Oczekiwane | Zaobserwowane | Wynik |
|---|---:|---:|---:|---:|---:|
| SM-01 | Scena otwiera się | Scena otworzyła się | PASS |
| SM-02 | Chwyt aktywuje obiekt | Chwyt aktywował obiekt | PASS |
| SM-03 | Socket odrzuca złą warstwę | Socket odrzucił obiekt | PASS |
| SM-04 | Reset czyści stan | Stan nie został wyczyszczony | FAIL |
| SM-05 | Zamknięcie próby zapisuje wynik | Nie uruchomiono testu | NV |

Przed uruchomieniem testów zapisuje się AC-01…AC-06, kryteria blokujące, dowód i wersję systemu. FAIL powinien prowadzić do zgłoszenia defektu, nie do zmiany kryterium po fakcie.

## Wniosek z przykładu

W tym fikcyjnym zestawie test SM-04 wykazał niezgodność, a SM-05 pozostał niezweryfikowany. Nie wolno zamieniać NV na PASS.

## Co należy uzupełnić w pełnym sprawozdaniu?

Do rzeczywistego raportu wpisz datę, numer indeksu i grupę **tylko do lokalnego formularza**, wersje narzędzi, wariant przypisany studentowi, warunki, surowe pomiary, obliczenia, ograniczenia i wniosek. Wyższe poziomy wymagają dodatkowych doświadczeń: 3.5: wydajność; 4.0: użyteczność U1–U3; 4.5: błąd i regresja REG-01/02; 5.0: automatyzacja, R=P×S i decyzja.

**Status:** 3.0 pokazano wyłącznie jako przykład; wszystkie faktyczne pomiary i poziomy 3.5–5.0 są **niewykonane**. Nie deklaruj zrealizowanych testów sprzętu, których nie było.

## Zapis i wysyłanie

W Unity: **WiRR → Raporty → Formularz raportu laboratoryjnego** → wybierz Lab 07 → **Sprawdź kompletność raportu** → **Zapisz raport lokalnie (bez logowania)**. Zapis na komputerze nie oznacza przesłania raportu. Wysłanie go do zewnętrznego systemu jest osobnym procesem.

[Spis przykładowych raportów](../README.md)

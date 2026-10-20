# Lab 06. Przykładowe sprawozdanie: Bliźniak cyfrowy ROS 2 + Gazebo

> **PRZYKŁAD DYDAKTYCZNY, DANE FIKCYJNE.** Żadna liczba nie pochodzi z pomiaru rzeczywistego studenta lub sprzętu. Pokazano jedną demonstracyjną sekcję raportu na poziomie 3.0, nie całe zaliczone laboratorium.

## Identyfikacja i środowisko

- Student: `DEMO`, bez numeru indeksu i przydziału do grupy.
- Laboratorium: 06; poziom demonstracji: **3.0**.
- Temat: Źródło stanu i transmisja JointState.
- Cel: Sprawdzenie, czy polecenia ruchu ze źródła stanów powodują poprawną aktualizację przegubu w Unity.
- Warunki: Topologia demonstracyjna: REPLAY na PC; nie jest to pomiar żywego stanowiska ROS 2.

## Wyniki demonstracyjne

| Polecenie | Wartość źródła | Unity | Ocena |
|---|---:|---:|---:|---:|---:|
| 1 | 0° | 0° | zgodny |
| 2 | 30° | 30° | zgodny |
| 3 | −20° | −20° | zgodny |

Właściwy raport dokumentuje topologię LOCAL/LAN/WEBSIM lub REPLAY, adres, port, nazwę topicu, jednostki, częstotliwość JointState i rzeczywiste źródło danych. Odróżniaj REPLAY od LIVE.

## Wniosek z przykładu

W fikcyjnym replay wszystkie trzy stany odpowiadają stanom referencyjnym. Nie wnioskuj stąd o działaniu Gazebo, sieci LAN czy opóźnieniach rzeczywistego połączenia.

## Co należy uzupełnić w pełnym sprawozdaniu?

Do rzeczywistego raportu wpisz datę, numer indeksu i grupę **tylko do lokalnego formularza**, wersje narzędzi, wariant przypisany studentowi, warunki, surowe pomiary, obliczenia, ograniczenia i wniosek. Wyższe poziomy wymagają dodatkowych doświadczeń: 3.5: mapowanie przegubów; 4.0: inter-arrival/jitter i bufor; 4.5: RTT (≥50 próbek na warunek); 5.0: STALE→LIVE i diagnoza.

**Status:** 3.0 pokazano wyłącznie jako przykład; wszystkie faktyczne pomiary i poziomy 3.5–5.0 są **niewykonane**. Nie deklaruj zrealizowanych testów sprzętu, których nie było.

## Zapis i wysyłanie

W Unity: **WiRR → Raporty → Formularz raportu laboratoryjnego** → wybierz Lab 06 → **Sprawdź kompletność raportu** → **Zapisz raport lokalnie (bez logowania)**. Zapis na komputerze nie oznacza przesłania raportu. Wysłanie go do zewnętrznego systemu jest osobnym procesem.

[Spis przykładowych raportów](../README.md)

# WiRR. Szybki start kampanii 4 × 2 laboratoria

**Wersja robocza 1.1.0-preview.1.** Dokładnie 8 laboratoriów, po 2 w każdym z 4 bloków. Zacznij od pierwszego doświadczenia, nie od instalowania wszystkich SDK.

## Spis treści
- [Którą ścieżkę wybierasz?](#którą-ścieżkę-wybierasz)
- [Spotkanie 1: Lab 01 + Lab 02](#spotkanie-1-lab-01--lab-02)
- [Spotkanie 2: Lab 03 + Lab 04](#spotkanie-2-lab-03--lab-04)
- [Spotkanie 3: Lab 05 + Lab 06](#spotkanie-3-lab-05--lab-06)
- [Spotkanie 4: Lab 07 + Lab 08](#spotkanie-4-lab-07--lab-08)
- [Co zapisujesz po każdym spotkaniu?](#co-zapisujesz-po-każdym-spotkaniu)
- [Co robisz, jeśli nie działa sprzęt?](#co-robisz-jeśli-nie-działa-sprzęt)

## Którą ścieżkę wybierasz?

- **Nie masz Unity:** [instrukcja instalacji](OS_SETUP.md#unity-nie-jest-zainstalowane).
- **Masz Unity:** [pierwsza scena w kilka minut](../../Samples~/Lab01/README.md#pierwsze-10-minut-eksperyment-zamiast-instalatora).
- **Nie masz licencji/konta:** [Unity Education Grant, Personal i Student](UNITY_ACCESS_AND_LICENSES.md#nawigacja-warunkowa).
- **Windows:** [kroki dla Windows](OS_SETUP.md#windows), **Ubuntu:** [dla Ubuntu](OS_SETUP.md#ubuntu).
- **Brak gogli i telefonu AR:** doświadczenia podstawowe rozpoczniesz na PC; tryb zaznacz jako **UNITY_EDITOR**, nie jako Quest lub ARCore.
- **Masz gotowy projekt z poprzedniego bloku:** nie zakładaj nowego. Otwórz **WiRR → Misje i odkrycia** i odpowiednie Lab XX.

## Spotkanie 1: Lab 01 + Lab 02

[Przewodnik bloku I: wtorek 13 października 2026](../Campaign/BLOK-01.md). W Lab 01 odkryjesz zależności skala–fizyka–FPS; w Lab 02 interakcję, chwyt, UI i komfort VR.

## Spotkanie 2: Lab 03 + Lab 04

[Przewodnik bloku II](../Campaign/BLOK-02.md): AR, kotwice, powierzchnie, Depth API i okluzja. Zależności telefonu dodasz dopiero, kiedy wybierzesz wariant Android.

## Spotkanie 3: Lab 05 + Lab 06

[Przewodnik bloku III](../Campaign/BLOK-03.md): CAD, geometria i LOD, a następnie bliźniak robota ROS 2 z aktualnością danych LIVE/STALE/REPLAY. Brak ROS nie blokuje całego bloku.

## Spotkanie 4: Lab 07 + Lab 08

[Przewodnik bloku IV](../Campaign/BLOK-04.md): najpierw diagnostyka i walidacja, potem **pełnoprawne Lab 08**, czyli własny demonstrator poza grami VR. Gotowy [przykład Lab 08](../../Samples~/Lab08/README.md) ma trzy obiekty inspekcji, automatyczny pomiar prób i lokalny JSON.

## Co zapisujesz po każdym spotkaniu?

Jedna lokalna karta sesji w **WiRR → Misje i odkrycia**: dwa laboratoria, dane i tryby urządzeń z posiadanych pomiarów oraz trzy krótkie wypowiedzi studenta. Liczby pochodzą tylko z realnych zapisów, nie są generowane znikąd. W Lab 08 możesz zaimportować JSON bez ręcznego przepisywania czasów.

Nie wymagamy tworzenia osobnego Pull Requestu ani uruchamiania GitHub Actions po każdym eksperymencie. [Pełny indeks materiałów](INDEX.md).

## Co robisz, jeśli nie działa sprzęt?

Diagnozuj po kolei: zasilanie, przewód, uprawnienia USB, SDK, runtime, scena. Po 10–15 minutach bez rozwiązania przejdź do przygotowanej alternatywy, oznacz brak pomiaru fizycznego i kontynuuj naukę. Pomoc: [Windows i Ubuntu](OS_SETUP.md#najczęstsze-błędy).

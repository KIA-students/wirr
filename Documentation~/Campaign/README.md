# WiRR: kampania czterech wypraw, osiem laboratoriów

**Wersja robocza kampanii: 1.1.0-preview.1, 11 października 2026.** Cykl obejmuje dokładnie **4 bloki × 2 laboratoria = 8 laboratoriów**. Nie kończy się na Lab 07. Laboratorium 08 to własny prototyp integracyjny z osobnym przewodnikiem, próbką Unity, programem do rejestracji pomiarów i szablonem raportu.

## Spis treści
- [Osiem laboratoriów, cztery spotkania](#osiem-laboratoriów-cztery-spotkania)
- [Pierwsze zajęcia: wtorek 13 października 2026](#pierwsze-zajęcia-wtorek-13-października-2026)
- [Jak zacząć bez instalowania wszystkiego](#jak-zacząć-bez-instalowania-wszystkiego)
- [Co sprawdzamy po każdym bloku](#co-sprawdzamy-po-każdym-bloku)
- [Dwie grupy i Liga XR](#dwie-grupy-i-liga-xr)
- [Jeżeli sprzęt nie działa](#jeżeli-sprzęt-nie-działa)

## Osiem laboratoriów, cztery spotkania

| Wyprawa | Laboratorium A | Laboratorium B | Efekt eksperymentu |
|---|---|---|---|
| **I: Świat reaguje** | [01: scena, skala, wydajność](../../Samples~/Lab01/README.md) | [02: chwyt, interakcje, komfort](../../Samples~/Lab02/README.md) | Zrozumienie zależności fizyka–obraz–interakcja |
| **II: Świat się miesza** | [03: śledzenie, raycast i kotwice AR](../../Samples~/Lab03/README.md) | [04: głębia, okluzja i światło](../../Samples~/Lab04/README.md) | Wirtualny obiekt wpisany w rzeczywistą scenę |
| **III: Bliźniak rzeczywistości** | [05: modele CAD, geometria, LOD](../../Samples~/Lab05/README.md) | [06: ROS 2, stan robota, LIVE/STALE](../../Samples~/Lab06/README.md) | Wizualizacja inżynierska z wiarygodnym statusem danych |
| **IV: Wynalazca XR** | [07: walidacja, błędy i użyteczność](../../Samples~/Lab07/README.md) | [08: demonstrator integracyjny](../../Samples~/Lab08/README.md) | Własny pomysł na zastosowanie XR poza grami |

Przewodniki bloków: **[Blok I](BLOK-01.md) · [Blok II](BLOK-02.md) · [Blok III](BLOK-03.md) · [Blok IV](BLOK-04.md)**. Każdy blok ma jeden cel i kończy się jedną lokalną kartą sesji obejmującą obydwa laboratoria.

## Pierwsze zajęcia: wtorek 13 października 2026

Na wtorek przygotowane jest laboratorium **01 + 02**, z działającą alternatywą na ekranie PC. Właśnie ten blok ma pierwszeństwo przy przygotowaniu stanowisk. Jeśli przedmiot jest już w dalszej części semestru, użyj odpowiedniego bloku z powyższej tabeli; kolejność nie wymaga resetowania projektu.

**Przygotowanie organizacyjne do poniedziałku 12 października:** sprawdź projekt Unity 6000.6.x na stanowisku uczelnianym, aktywowaną licencję, pierwszy Play, jeden model fizyczny, przewód USB-C z transmisją danych i jedna sprawną parę Quest 3. Prawdziwy test Quest jest cenny, ale **nie jest warunkiem wykonania Lab 01 na PC**. [Lista preflight prowadzącego](../Reference/INSTRUCTOR_GUIDE.md).

## Jak zacząć bez instalowania wszystkiego

- **Nie mam Unity:** [instalacja Windows/Ubuntu](../Reference/OS_SETUP.md#unity-nie-jest-zainstalowane).
- **Unity działa:** przejdź do [Lab 01: pierwsze 10 minut](../../Samples~/Lab01/README.md#pierwsze-10-minut-eksperyment-zamiast-instalatora).
- **Nie mam konta/licencji:** [Unity Education Grant / Personal / Student](../Reference/UNITY_ACCESS_AND_LICENSES.md#nawigacja-warunkowa).
- **Brak gogli:** cały eksperyment bazowy wykonasz na zwykłym monitorze. Wynik nie jest jednak pomiarem fizycznego HMD.
- **Problemy ze sprzętem:** [instrukcja diagnostyki](../Reference/OS_SETUP.md#najczęstsze-błędy). Po krótkiej diagnozie przejdź do działającej alternatywy.

Nie tworzymy nowego projektu na każdą sesję: zachowujemy działający projekt i oddzielne sceny Lab01–Lab08. Żaden student nie musi kupować Unity Student Plan ani udostępniać hasła.

## Co sprawdzamy po każdym bloku

Celem jest zrozumienie zjawiska, a nie wypełnianie formularzy. W każdym laboratorium: **jedno pytanie → jedna zmiana → trzy obserwacje → jeden wniosek**. Formularz WiRR lub lokalny generator przejmuje metadane i dostępne zapisane liczby. Student dopisuje **odkrycie, diagnozę i pomysł**. Nie przypisujemy zer za brak danych i nie udajemy eksperymentów na sprzęcie.

Sugerowany czas czterech godzin dydaktycznych (180 min): wprowadzenie 10 min, laboratorium A 65 min, przerwa 10 min, laboratorium B 65 min, misja ciekawostkowa 15 min i wspólny raport 15 min. Dostosuj czas do faktycznego harmonogramu, bez zmieniania wymiaru sylabusa.

## Dwie grupy i Liga XR

Drużyny G1 i G2 mogą zaproponować nazwy i wykonywać dobrowolne misje poza zajęciami. Jest to **zabawa w tle**, nie indywidualna ocena. Punkty za wspólne odkrycia przyznaje prowadzący, a dane źródłowe mają trafić docelowo do [KIA-students/wirr](https://github.com/KIA-students/wirr) dopiero po zatwierdzeniu wydania. [Zasady ligi](../../competition/OPTIONAL_MISSIONS.md).

## Jeżeli sprzęt nie działa

Rozpoznaj pierwsze niespełnione ogniwo: zasilanie → przewód → system i uprawnienia → SDK/runtime → scena i skrypt. Zamiast wielokrotnej reinstalacji skorzystaj z działającego projektu kontrolnego lub logu REPLAY, wyraźnie oznaczając, czego **nie przetestowano fizycznie**.

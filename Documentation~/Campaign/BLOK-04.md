# Blok IV: Wynalazca XR. Lab 07 + Lab 08

**Spotkanie 4 z 4.** Sprawdź, czy wcześniejsze demonstratory są wiarygodne, a następnie zaprojektuj własny przypadek użycia XR poza grami.

## Spis treści
- [Wybierz wariant](#wybierz-wariant)
- [Lab 07: testuj, zanim zaufasz](#lab-07-testuj-zanim-zaufasz)
- [Lab 08: własny demonstrator](#lab-08-własny-demonstrator)
- [Finał bez zbędnego raportowania](#finał-bez-zbędnego-raportowania)

## Wybierz wariant

- **Masz dowolną działającą scenę z Lab 01–06:** przejdź do [testów Lab 07](../../Samples~/Lab07/README.md#30-kryteria-i-smoke-test).
- **Nie masz urządzenia XR:** wykonaj [Lab 08 na PC](../../Samples~/Lab08/README.md#pierwsze-pięć-minut-gotowa-scena), z rzeczywistymi wynikami w Unity Editor.
- **Masz Quest / AR / VITURE:** użyj jednej z [dróg Lab 08](../../Samples~/Lab08/README.md#wybierz-ścieżkę-bez-powtórnej-instalacji), nie wszystkich naraz.
- **Nie działa import laboratorium:** wróć do [systemu operacyjnego i diagnostyki](../Reference/OS_SETUP.md#najczęstsze-błędy).

## Lab 07: testuj, zanim zaufasz

1. Zdefiniuj prostą listę: scena uruchamia się, interakcja działa, wartości są aktualne, użytkownik widzi awarię.
2. Wykonaj krótki smoke test i oznacz każde sprawdzenie jako PASS/FAIL/NV. Brak testu to **NV**, nie PASS.
3. Zmień jeden element na wadliwy. Czy umiesz wskazać przyczynę i wykonać test regresyjny?
4. Rozważ wydajność na słabszym sprzęcie, dostępność UI oraz możliwość przerwania działania w VR.
5. Zapisz jeden zdiagnozowany błąd. To może być ważniejsze niż dodatkowa grafika.

[Pełne Lab 07](../../Samples~/Lab07/README.md#zadania-krok-po-kroku).

## Lab 08: własny demonstrator

1. Otwórz **Start: Lab 08** oraz **WiRR → Lab 08 → Dodaj demonstrator inspekcji**.
2. Wybierz jeden cel użytkownika i trzy próby bazowe (lub rozwiń własną scenę AR/VR/ROS/VITURE).
3. Zmień jeden parametr interfejsu, bezpieczeństwa lub geometrii.
4. Wykonaj trzy porównywalne próby po poprawce, zapisane automatycznie do JSON.
5. Zaimportuj rzeczywisty JSON do **WiRR Reports (Lab 08)**. Dodaj krótkie uzasadnienie zastosowania.
6. Zaprezentuj 60–90 sekund działania bez osobnej prezentacji PowerPoint.

[Pełne Lab 08](../../Samples~/Lab08/README.md#zadania-krok-po-kroku) oraz [gotowy szablon raportu](../../Samples~/Lab08/report-template.md).

## Finał bez zbędnego raportowania

Jedna karta **blok IV** + scena Lab 07 + prototyp Lab 08 i realne lokalne wyniki. **Ciekawostka:** najbardziej wpływowy wynalazek XR może być prostym wizualnym ostrzeżeniem serwisanta, które ratuje przed kosztownym błędem, zamiast efektownym światem wirtualnym.

Drużynowa Liga XR pozostaje dobrowolna i niezależna od ocen. Pomysły można rozwijać po zajęciach, bez obowiązkowej rejestracji aplikacji w sklepie czy kompilowania ciężkich workflow GitHub Actions.

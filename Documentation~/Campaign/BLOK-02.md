# Blok II: Świat się miesza. Lab 03 + Lab 04

**Spotkanie 2 z 4.** Pokaż, że telefon potrafi nie tylko wyświetlić wirtualny obiekt, ale powiązać go z prawdziwą powierzchnią i innymi przedmiotami.

## Spis treści
- [Wybierz wariant](#wybierz-wariant)
- [Lab 03: powierzchnie i kotwice](#lab-03-powierzchnie-i-kotwice)
- [Lab 04: głębia i okluzja](#lab-04-głębia-i-okluzja)
- [Raport i samodzielny pomysł](#raport-i-samodzielny-pomysł)

## Wybierz wariant
- **Telefon zgodny z ARCore:** [przygotuj Android i ADB](../Reference/OS_SETUP.md#urzadzenia-xr).
- **Android/ARCore już działa:** przejdź od razu do [Lab 03](../../Samples~/Lab03/README.md#30-śledzenie-i-płaszczyzny).
- **Ubuntu:** sprawdź udev i ADB w [instrukcji Ubuntu](../Reference/OS_SETUP.md#ubuntu).
- **Windows:** [wersja Windows](../Reference/OS_SETUP.md#windows).
- **Telefon bez Depth API:** wykonaj Lab 03 i część Lab 04 bez obsługi głębi; opisz brak API, nie fabrykuj wyniku.
- **Brak telefonu:** zbuduj scenę referencyjną i przeanalizuj obraz/log testowy przygotowany przez prowadzącego.

## Lab 03: powierzchnie i kotwice

1. Zamiast instalować wszystko od nowa otwórz **Start: Lab 03** i powiąż scenę z AR Foundation/ARCore tylko w wariancie Android.
2. W realnym otoczeniu porównaj **powierzchnię bogatą w detale** i jednolitą, gładką. Czy tracking i wykrywanie płaszczyzn działają równie dobrze?
3. Umieść obiekt wirtualny na stole i sprawdź, czy ma skalę zbliżoną do realnej. Wykonaj trzy próby.
4. Dodaj kotwicę i powróć do tego samego położenia, rejestrując zauważony dryf lub brak danych.
5. W raporcie oddziel **pomiar telefonu** od **symulowanego pozycjonowania na PC**.

[Pełny opis eksperymentu Lab 03](../../Samples~/Lab03/README.md#zadania-krok-po-kroku).

## Lab 04: głębia i okluzja

1. Otwórz istniejącą scenę Lab 03, a potem **Start: Lab 04**. Nie powtarzaj instalacji, jeśli wszystko działa.
2. Przy zgodnym urządzeniu porównaj zasłanianie wirtualnej bryły przez realny kubek. Testuj w trzech pozycjach i zaznacz błędne zasłonięcia.
3. Zmień światło w rzeczywistym pokoju. Sprawdź, czy dane light estimation są dostępne i jak wpływają na wiarygodność sceny.
4. Porównaj wynik z trybem bez okluzji; jeśli Depth API jest niedostępne, opisz to jako ograniczenie kompatybilności.
5. Zaproponuj AR do naprawy urządzenia, wystawy muzealnej lub edukacji przyrodniczej.

[Pełny opis Lab 04](../../Samples~/Lab04/README.md#zadania-krok-po-kroku).

## Raport i samodzielny pomysł

Zapisz obie sceny i lokalną kartę **blok II**. Zmierz tylko dane faktycznie udostępniane przez dane urządzenie. **Ciekawostka:** aby hologram „wyglądał na prawdziwy”, często ważniejsze jest, czy prawdziwy przedmiot go zasłania, niż sama liczba trójkątów.

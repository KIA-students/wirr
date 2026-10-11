# Bezpieczny operator VR: automatyczna detekcja i 10 prób

## Spis treści
- [Cel](#cel)
- [Jakie sygnały można wykryć?](#jakie-sygnały-można-wykryć)
- [Próba techniczna](#próba-techniczna)
- [Bezpieczeństwo i ograniczenia](#bezpieczeństwo-i-ograniczenia)
- [Punkty dla Ligi XR](#punkty-dla-ligi-xr)

## Cel

Dobrowolne wyzwanie polega na zaprojektowaniu i sprawdzeniu **automatycznego zatrzymania ekspozycji**, a nie na porównywaniu odporności ludzi na nudności. Limit wynosi **10 prób technicznych na grupę na jednym stanowisku**. Wystarczy zwykły PC: wszystkie zdarzenia można symulować bez noszenia gogli.

## Jakie sygnały można wykryć?

| Zdarzenie | Mechanizm | Co naprawdę oznacza? |
|---|---|---|
| Utrata obecności w goglach | UnityEngine.XR.CommonUsages.userPresence, gdy wspierane przez urządzenie | Zdjęcie gogli lub brak obecności, **nie diagnoza dyskomfortu** |
| Brak wiarygodnego śledzenia | CommonUsages.isTracked | Utrata trackingu, nie omdlenie |
| Nagła zmiana położenia | Różnica próbek devicePosition / czas | Możliwy skok danych lub szybki ruch, **nie dowód utraty równowagi** |
| Utrata fokusu aplikacji | OnApplicationFocus(false) | Aplikacja straciła fokus |
| Ręczny STOP | Przycisk uczestnika albo obserwatora | Natychmiastowe zatrzymanie na żądanie |
| Funkcja niedostępna | TryGetFeatureValue == false | UNSUPPORTED, nie zdarzenie zdjęcia gogli |

Unity dokumentuje [CommonUsages.userPresence](https://docs.unity.com/en-us/engine/6000.3/script-reference/unityengine/xr/commonusages/userpresence). Dostępność i znaczenie tego sygnału zależą od konkretnego modelu gogli.

## Próba techniczna

1. W Unity uruchom **WiRR → Komfort XR → Utwórz 2 prefaby i stanowiska obserwacyjne**. W nowej wersji powstaje również komponent **WiRR_Comfort_SafetyWatchdog**.
2. W Play wybierz grupę G1/G2 i kliknij **Rozpocznij próbę**. Licznik jest zwiększany przy starcie, do maksymalnie 10 prób lokalnych.
3. Sprawdź przyciski **Symuluj zdjęcie gogli**, **Symuluj brak trackingu** i **Symuluj skok ruchu**. Oprogramowanie powinno zatrzymać próbę i zatrzymać pozostałe demonstratory.
4. Sprawdź działanie **Ręczny STOP** i automatycznego limitu czasu. Na sprzęcie Quest dodatkowo sprawdź, czy dostępny jest czujnik userPresence. Bez takiego sensora wpisz UNSUPPORTED.
5. Wyeksportuj lokalne podsumowanie techniczne w JSON: grupa, liczba prób i wykrytych zdarzeń, bez nazwisk, numerów indeksów, objawów lub czasu „wytrzymywania” dolegliwości.
6. Po dziesiątej próbie nie wykonuj więcej prób konkursowych na tym stanowisku. Techniczny limit lokalny nie jest antycheatem między komputerami.

## Bezpieczeństwo i ograniczenia

Nie wolno prowokować nagłych ruchów głowy ani zaburzeń równowagi. **Zdarzenie gwałtownego ruchu symuluj przyciskiem**, nie ruchem ciała. W rzeczywistym HMD zadanie jest dobrowolne, w pozycji siedzącej, z obserwatorem i natychmiastowym STOP.

Czujnik obecności może nie być obsługiwany; nie traktuj braku sygnału jako dowodu zdjęcia gogli. Wykrycie skoku pozycji nie oznacza upadku. Moduł ma charakter **dydaktyczny, a nie medyczny i nie zastępuje nadzoru człowieka**.

## Punkty dla Ligi XR

W ramach misji **xr-optics** można zdobyć do **2 punktów na grupę** za poprawną detekcję, bezpieczny STOP, obsługę niedostępnego czujnika i rzetelny raport techniczny. Liczba podejść ani czas trwania ekspozycji nie zwiększają wyniku. Nie przyznaje się punktów za wystąpienie objawów.

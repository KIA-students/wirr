# WiRR w Unity: jedna ścieżka pracy studenta

> Kanoniczna ścieżka dydaktyczna. Materiały studenckie i adresy instalacji pochodzą wyłącznie z [KIA-students/wirr](https://github.com/KIA-students/wirr).

## Cel

Każde z ośmiu laboratoriów prowadzi od **pytania badawczego**, przez **własną scenę Unity**, **eksperyment i obserwacje**, do **sprawdzonego raportu**. Pakiet przygotowuje stanowisko, ale nie wykonuje ćwiczeń i nie generuje fikcyjnych wyników. Cztery bloki po dwa laboratoria są jedną kampanią, a nie czterema oddzielnymi instalacjami.

## Pierwsze uruchomienie w Unity

1. Uruchom Unity Hub i otwórz **własny projekt Universal 3D (URP)** w wersji Unity 6000.6.x zatwierdzonej dla grupy. Na komputerze uczelnianym korzystaj z licencji skonfigurowanej przez administratora; na prywatnym z własnego konta i odpowiedniej licencji.
2. Sprawdź Git: `git --version`. W Unity otwórz **Window → Package Manager → + → Install package from git URL** i zainstaluj:

   ```text
   https://github.com/KIA-students/wirr.git#upm
   ```

3. Otwórz **WiRR → Narzędzia kursu**. Wykonaj kontrolę konfiguracji. Doinstaluj tylko zależności wymagane przez aktualne laboratorium, a nie wszystkie SDK XR/AR/ROS naraz.
4. Otwórz **WiRR → Misje i odkrycia (4 bloki)**, wybierz numer laboratorium i przygotuj scenę startową. Jeżeli dany przycisk nie jest widoczny, użyj importu odpowiedniej próbki w Package Manager i narzędzia przygotowania sceny.
5. Zapisz scenę we własnym `Assets/WiRR/LabXX`; nie edytuj `Library/PackageCache`. Wykonuj zadania w swoim projekcie, nie w repozytorium pakietu.

### Opcjonalne zasoby wizualne

Bazowe laboratoria **nie wymagają** pełnych bibliotek HDRI/PBR. Jeżeli ćwiczenie rzeczywiście korzysta z panoram lub materiałów PBR, dodaj drugi pakiet:

```text
https://github.com/KIA-students/wirr.git#assets
```

Następnie w Package Manager otwórz **WiRR Optional HDRI and PBR Assets → Samples → HDRI PBR → Import**. Samo dodanie zależności pobiera pakiet; import próbki kopiuje wybrane materiały do projektu. Nie instaluj tej biblioteki na wszystkich stanowiskach profilaktycznie. Lekkie prefaby Grid i podstawowe narzędzia pozostają w pakiecie `upm`.

## Powtarzalny cykl każdego laboratorium

| Krok | Student robi w Unity | Wynik, który powinien zostać zachowany |
|---|---|---|
| 1. Pytanie | Czyta cel, ograniczenia sprzętowe i punkt kontrolny wybranego Lab XX | Hipoteza albo pytanie pomiarowe |
| 2. Przygotowanie | Wczytuje sample i przygotowuje własną scenę; sprawdza Console i zależności | Scena, wersja Unity, wersja pakietu |
| 3. Realizacja | Samodzielnie modyfikuje obiekty, skrypty, parametry, interakcje lub integracje | Własny kod i scena, niezmienione materiały bazowe |
| 4. Pomiar | Uruchamia próbę kontrolną, rejestruje wartości i warunki; powtarza pomiar zgodnie z instrukcją | Rzeczywiste pomiary, wariant, liczba prób |
| 5. Interpretacja | Porównuje warianty, identyfikuje niepewność, wyjaśnia przyczynę wyniku | Wniosek i ograniczenia obserwacji |
| 6. Raport | Otwiera **WiRR → Raporty**, importuje istniejące dane, sprawdza kompletność i zapisuje/wysyła | Indywidualny raport z dowodami |
| 7. Domknięcie bloku | Po obu laboratoriach otwiera **WiRR → Misje i odkrycia** i zapisuje kartę sesji | Jedno odkrycie, jedna diagnoza, jeden pomysł |

**Ważne:** zapis karty sesji nie zastępuje dwóch raportów laboratoryjnych. Brak pomiaru oznacza brak danych, a nie zero. Ćwiczenia wykonywane bez urządzenia należy oznaczyć jako wariant PC/symulacyjny, bez podszywania się pod wynik sprzętowy.

## Cztery bloki, osiem doświadczeń

| Blok | Laboratoria | Oś badawcza | Rezultat |
|---|---|---|---|
| I. Świat reaguje | 01: środowisko XR; 02: interakcja | Jak konfiguracja renderowania, fizyki i interakcji zmienia zachowanie sceny? | Scena referencyjna, próby interakcji, pomiary |
| II. Świat się miesza | 03: rejestracja AR; 04: rozumienie sceny | Skąd bierze się błąd pozycjonowania i okluzji? | Test śledzenia, kotwic i danych głębi |
| III. Bliźniak rzeczywistości | 05: modele CAD; 06: ROS 2/Gazebo | Jak geometria, poziom szczegółowości i opóźnienie wpływają na cyfrowego bliźniaka? | Warianty modelu i diagnostyka transmisji |
| IV. Wynalazca XR | 07: testy; 08: integracja | Czy rozwiązanie działa powtarzalnie oraz czy można wykazać jego użyteczność? | Test regresyjny i demonstrator z próbami A/B |

W Lab 08 wyniki demonstratora są importowane z rzeczywiście zapisanego JSON. Wariant bez sprzętu XR jest pełnoprawną ścieżką PC, ale musi być tak opisany w raporcie.

## Liga XR jako warstwa dobrowolna

Liga **nie jest etapem obowiązkowym** i nie zastępuje ocen indywidualnych ani kontroli raportów. Dwie grupy G1/G2 mogą ustalić nazwy (generator na stronie kursu lub własna propozycja), zatwierdzane przez prowadzącego. Prowadzący przyznaje punkty za cztery bloki: eksperyment 0–4, interpretacja 0–3, pomysł 0–2, współpraca 0–1. Dodatkowe misje mają wspólny limit **8 punktów na grupę**. Czas w Play Mode, liczba commitów, liczba urządzeń i liczebność grupy nie dają punktów.

Wyniki są odczytywane na stronie [Liga XR](https://kia-students.github.io/wirr/#liga) z zatwierdzonego `competition/scoreboard.json`; `null` oznacza brak zatwierdzenia, a nie 0 punktów. Ranking nie może nadpisywać historii punktacji podczas synchronizacji. Wartości zatwierdza wyłącznie prowadzący. Żadnych danych osobowych, indeksów, zdjęć uczestników ani surowych odpowiedzi SSQ w rankingu.

Dobrowolne zadania poza zajęciami: watchdog bezpiecznego XR, sygnalizacja LIVE/STALE robota, dostępny HUD oraz pomiar błędu AR. Szczegóły: [OPTIONAL_MISSIONS.md](../../competition/OPTIONAL_MISSIONS.md).

## Rozdzielenie narzędzi i odpowiedzialności

- **Unity**: główne środowisko wykonania scen, interakcji, pomiarów i zapisu projektu.
- **WiRR Course Toolkit**: konfiguracja, wybór laboratorium, import próbek, walidacja.
- **WiRR Reports**: kontrola i złożenie indywidualnego raportu.
- **Misje i odkrycia**: podsumowanie pary laboratoriów w jednej lokalnej karcie.
- **WebSim**: pomocnicza ścieżka sieciowa w Lab 06; nie zastępuje rzeczywistych pomiarów robota.
- **SSQ**: oddzielna procedura samoopisu związana z komfortem XR, uruchamiana tylko zgodnie z instrukcją i zasadami bezpieczeństwa.
- **Liga XR**: opcjonalne publiczne zestawienie punktów grupowych po ręcznym zatwierdzeniu.

## Diagnostyka, aby nie blokować ćwiczeń

Gdy UPM lub SDK zawiedzie, zapisz **pierwszy** błąd z Console, wersję Unity oraz `Packages/manifest.json` i `packages-lock.json`. Nie usuwaj własnego projektu. W Lab 01 rozpocznij pomiar na czystej scenie URP, a zaawansowane SDK dodawaj dopiero do odpowiednich laboratoriów. Przy problemach z HDRI najpierw sprawdź, czy drugi pakiet został zainstalowany **i czy sample zostało zaimportowane**.

## Granice aktualnego wdrożenia

Powyższa procedura porządkuje istniejące komponenty, ale nie jest dowodem zakończenia testów w Unity na Windows i Ubuntu. Przed wydaniem produkcyjnym należy wykonać czysty import `#upm`, import opcjonalnego `#assets`, sprawdzić brak błędów kompilacji, menu WiRR, import Lab 01–08, zapis raportu i przynajmniej jedną scenę na każdej wspieranej platformie.

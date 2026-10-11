# Laboratorium 08: Wynalazca XR, demonstrator integracyjny

**Kampania WiRR, blok IV, laboratorium 8/8.** Po Lab 07 wybierz jeden problem spoza gier VR i pokaż rozwiązanie, które łączy co najmniej dwie umiejętności z kursu. Pracujesz w istniejącym projekcie Unity; nie instaluj niczego, czego nie wymaga wybrany demonstrator.

## Spis treści

- [Wybierz ścieżkę bez powtórnej instalacji](#wybierz-ścieżkę-bez-powtórnej-instalacji)
- [Cel i wynik po 65 minutach](#cel-i-wynik-po-65-minutach)
- [Pierwsze pięć minut: gotowa scena](#pierwsze-pięć-minut-gotowa-scena)
- [Ścieżka A: instruktor serwisowy XR](#ścieżka-a-instruktor-serwisowy-xr)
- [Ścieżka B: stanowisko AR i okluzja](#ścieżka-b-stanowisko-ar-i-okluzja)
- [Ścieżka C: bliźniak robota LIVE i STALE](#ścieżka-c-bliźniak-robota-live-i-stale)
- [Ścieżka D: VITURE stereo i czytelność](#ścieżka-d-viture-stereo-i-czytelność)
- [Eksperyment i porównanie przed i po](#eksperyment-i-porównanie-przed-i-po)
- [Jedna karta raportu, bez przepisywania pomiarów](#jedna-karta-raportu-bez-przepisywania-pomiarów)
- [Punkty kontrolne](#punkty-kontrolne)
- [Zaliczenie i pomysł wynalazczy](#zaliczenie-i-pomysł-wynalazczy)
- [Jeżeli coś nie działa](#jeżeli-coś-nie-działa)
- [Dla ambitnych](#dla-ambitnych)

## Wybierz ścieżkę bez powtórnej instalacji

- **Nie masz projektu ani Unity:** wróć do [instalacji Windows/Ubuntu](../../Documentation~/Reference/OS_SETUP.md#wybierz-aktualną-sytuację), a następnie [Lab 01](../Lab01/README.md#pierwsze-10-minut-eksperyment-zamiast-instalatora).
- **Masz Unity, ale nie scenę Lab 08:** wybierz w Unity **WiRR → Misje i odkrycia (4 bloki) → IV. Wynalazca XR → Start: Lab 08**. Jeżeli masz starszy pakiet, otwórz gotową scenę Lab 07 i zapisz kopię w nowym folderze.
- **Nie masz gogli ani telefonu:** wybierz [ścieżkę A](#ścieżka-a-instruktor-serwisowy-xr), wykonaj prawdziwy eksperyment na ekranie PC, a w raporcie oznacz **UNITY_EDITOR**.
- **Masz Quest z działającymi kontrolerami:** wybierz [ścieżkę A](#ścieżka-a-instruktor-serwisowy-xr) i rozszerz ją o interakcję z [Lab 02](../Lab02/README.md#30-chwyt-socket-i-ray).
- **Masz telefon AR:** wybierz [ścieżkę B](#ścieżka-b-stanowisko-ar-i-okluzja). Gdy ARCore nie działa, skorzystaj ze wspólnej diagnostyki [Lab 03](../Lab03/README.md#przygotowanie).
- **Masz ROS 2 albo log JointState:** wybierz [ścieżkę C](#ścieżka-c-bliźniak-robota-live-i-stale), z odwołaniem do [Lab 06](../Lab06/README.md#30-źródło-stanu).
- **Masz VITURE:** wybierz [ścieżkę D](#ścieżka-d-viture-stereo-i-czytelność) i [instrukcję SBS](../VITURE%20XR/README.md#szybka-konfiguracja).
- **Nie chcesz dodatkowej konfiguracji:** całość minimum wykonasz w [gotowym demonstratorze PC](#pierwsze-pięć-minut-gotowa-scena).

## Cel i wynik po 65 minutach

Twoim wynikiem ma być **jeden mały, działający prototyp**. Wystarczy krótki pokaz, w którym można wskazać: problem, działanie, wynik, ograniczenie oraz jedną poprawkę. Nie jest potrzebna prezentacja slajdowa ani repozytorium nowych pakietów.

**Pytanie przewodnie:** *W jakiej sytuacji XR naprawdę ułatwia zadanie, którego nie rozwiązuje tak samo zwykła aplikacja 2D?*

## Pierwsze pięć minut: gotowa scena

1. W **WiRR → Misje i odkrycia → Start: Lab 08** otwórz nową scenę. Scena PC działa bez dodatkowego runtime XR.
2. Wybierz **WiRR → Lab 08 → Dodaj demonstrator inspekcji**. Generator umieszcza trzy oznaczone wirtualne elementy i panel prób. Jeśli korzystasz z wcześniejszej paczki, zrób to ręcznie: trzy sześciany, widoczna instrukcja, prosty licznik sukcesu.
3. Uruchom **Play**. Kliknij **Rozpocznij próbę**, a następnie aktywuj kontrolę trzech elementów. Po trzecim elemencie czas próby i sukces zapisują się w pamięci programu.
4. Wykonaj **trzy krótkie próby**. Kliknięcie przycisku jest wejściem symulowanym, nie pomiarem kontrolera Quest.
5. Zapisz scenę. **Nie nadpisuj sceny Lab 07**.

Ciekawostka: wirtualna kontrola inspekcyjna może być mało widowiskowa, ale znacznie bardziej użyteczna dla serwisanta niż fantastyczna gra VR.

## Ścieżka A: instruktor serwisowy XR

Zacznij od gotowego demonstratora i zmień **jeden parametr użyteczności**, np. rozmiar etykiet, kolor statusu, sposób aktywacji lub kolejność kroków. Jeśli jest dostępny Quest 3, podłącz istniejące interakcje XRI z [Lab 02](../Lab02/README.md#30-chwyt-socket-i-ray). Zadbaj o czytelność instrukcji i przycisk STOP.

**Doświadczenie:** sprawdź w trzech próbach, czy uczestnik rozumie kolejny krok bez dodatkowej ustnej podpowiedzi. Nie oceniaj jego „odporności na VR”.

## Ścieżka B: stanowisko AR i okluzja

Kontynuuj działającą [scenę AR z Lab 03](../Lab03/README.md#30-śledzenie-i-płaszczyzny). Dodaj jeden wirtualny element, który objaśnia prawdziwy przedmiot. W [Lab 04](../Lab04/README.md#35-okluzja) znajdziesz sposób sprawdzenia zasłaniania obiektu.

**Doświadczenie:** porównaj poprawne i błędne osadzenie elementu w trzech próbach. Gdy telefon nie obsługuje głębi, opisz to jako ograniczenie urządzenia i pokaż wersję bez okluzji.

## Ścieżka C: bliźniak robota LIVE i STALE

Użyj sceny i logu [Lab 06](../Lab06/README.md#30-źródło-stanu). Pokazuj pozycję przegubu oraz bardzo wyraźnie status **LIVE, STALE lub REPLAY**. Nie zastępuj trybu replay deklaracją transmisji na żywo.

**Doświadczenie:** na zarejestrowanych danych wprowadź przerwę w aktualizacji i zmierz, czy użytkownik zauważył informację o utracie aktualności stanu.

## Ścieżka D: VITURE stereo i czytelność

Zacznij od działającego obrazu 2D, a dopiero potem włącz Full SBS odpowiedni do urządzenia. Użyj [gotowej próbki VITURE](../VITURE%20XR/README.md#szybka-konfiguracja). Nie próbuj celowo wymuszać dwojenia obrazu ani przeciążać wzroku.

**Doświadczenie:** porównaj czytelność niewielkiej instrukcji technicznej w 2D i stereo przy komfortowych ustawieniach. Wskaż, kiedy stereo pomaga, a kiedy wystarczy zwykły wyświetlacz.

## Eksperyment i porównanie przed i po

1. Zapisz jedną **hipotezę**, na przykład „większy napis ograniczy liczbę pomyłek”.
2. Ustal jeden warunek bazowy, wykonaj **trzy próby** i zachowaj wyniki z automatycznego rejestratora lub logów narzędzia.
3. Zmień **dokładnie jeden** parametr; wykonaj trzy kolejne próby w porównywalnych warunkach.
4. Wyznacz zmianę liczby sukcesów/błędów lub mediany czasu. Nie wstawiaj zer za brakujące dane.
5. Powiedz, czy obserwowane różnice pozwalają potwierdzić hipotezę. Trzy próby są demonstracją dydaktyczną, nie mocnym badaniem populacyjnym.

## Jedna karta raportu, bez przepisywania pomiarów

- W gotowym demonstratorze naciśnij **Eksportuj wyniki JSON**. Program zapisze czasy prób, liczbę sukcesów i wariant **UNITY_EDITOR**; nie udaje pomiarów z gogli.
- W Unity wybierz **WiRR → Lab 08 → Importuj wyniki demonstratora do raportu**. Wybierz wyeksportowany JSON. Formularz uzupełni metryki z rzeczywistego testu, a nie wartości przykładowe.
- W **WiRR → Raporty → Formularz raportu laboratoryjnego** wybierz **Lab 08**. Uzupełnij problem, kryterium akceptacji, test negatywny, hipotezę, interpretację A/B, regresję i uzasadnienie integracji.
- Całą sesję Lab 07+08 możesz ująć w jednej [karcie bloku IV](../../Documentation~/Reference/FOUR_BLOCK_MISSIONS.md). Nie zbieraj danych medycznych ani wrażliwych.

## Punkty kontrolne

| Etap | Wymagany dowód |
|---|---|
| 3.0 | Problem użytkownika, mierzalne kryterium, działająca scena i tryb platformy |
| 3.5 | Sukces, anulowanie, powtórne wskazanie i test negatywny |
| 4.0 | A1–A3 i B1–B3, dane surowe, obliczenia i hipoteza |
| 4.5 | Jedna poprawka, interpretacja A/B i test regresyjny |
| 5.0 | Integracja dwóch umiejętności, ryzyka, akceptacja i odtworzenie |

## Zaliczenie i pomysł wynalazczy

Zademonstruj działającą scenę, trzy próby A i trzy próby B, jedną poprawkę oraz uzasadnienie wartości rozwiązania dla użytkownika. **Ocena indywidualna nie zależy od Ligi XR**. Wyróżnione pomysły grup mogą otrzymać punkty dobrowolne, zatwierdzane przez prowadzącego. Brak prywatnego urządzenia nie obniża oceny.

## Jeżeli coś nie działa

Jeśli Unity nie kompiluje: [Windows/Ubuntu](../../Documentation~/Reference/OS_SETUP.md#najczęstsze-błędy). Jeśli nie działa chwyt: [Lab 02](../Lab02/README.md#30-chwyt-socket-i-ray). Jeśli AR nie widzi powierzchni: [Lab 03](../Lab03/README.md#30-śledzenie-i-płaszczyzny). Jeśli ROS nie przesyła stanu: [Lab 06](../Lab06/README.md#30-źródło-stanu). Po 10–15 minutach trudności wykonaj wariant na monitorze, zanotuj przyczynę i kontynuuj eksperyment.

## Dla ambitnych

- [XR Interaction Toolkit Samples](https://github.com/Unity-Technologies/XR-Interaction-Toolkit-Examples): nowy gest i interakcja.
- [ARCore Geospatial](https://developers.google.com/ar/develop/geospatial): lokalizacja obiektów poza biurkiem.
- [ROS 2 Quality of Service](https://docs.ros.org/en/humble/Concepts/Intermediate/About-Quality-of-Service-Settings.html): utrata aktualności wiadomości.
- [Meta Passthrough](https://developers.meta.com/vr/essentials/horizon-os-passthrough/): projektuj MR, które nie zasłania realnych przeszkód.

Zaproponuj jedną własną funkcję, którą warto wdrożyć poza laboratorium. Nie jest wymagane kupowanie zasobów Unity Asset Store.


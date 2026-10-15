# Lab 01: Rzeczywistość ma jednostki, a GPU ograniczenia

**Blok 01, część 1 z 2** · 90 minut · DESKTOP_SIM · 2.0.0-draft.3

**Pytanie:** Czy większa liczba obiektów zawsze oznacza mniej płynny obraz?

## Co będziesz badać

Skala określa rozmiar obiektu w świecie, a nie liczbę pikseli na ekranie. Obrót kamery zmienia kierunek patrzenia (3DoF); translacja dodaje zmianę położenia (6DoF) i paralaksę. Czas klatki podajemy w ms. Większy czas oznacza wolniejsze odświeżanie. Mediana opisuje typową klatkę, a p95 pozwala zauważyć wolniejsze klatki. Sam spadek FPS nie rozstrzyga, czy ograniczeniem jest CPU, GPU czy synchronizacja.

## Jedno doświadczenie, trzy działania

Zacznij od 3.0. Kolejne poziomy rozwijają to samo doświadczenie. Możesz zakończyć na osiągniętym etapie i przejść do drugiego laboratorium bloku. Pracuj solo lub w parze; w parze zamieniajcie role operatora i obserwatora.

### 1. Uruchom scenę i pomiar (3.0)

Otwórz przygotowaną Stację XR. Obejrzyj obiekty 0,1 m, 1 m i 10 m. Obróć kamerę bez przesuwania, a następnie przesuń ją w bok. Ustaw 10 obiektów, uruchom pomiar i poczekaj na koniec rozgrzewki oraz zapisu. Sprawdź, czy wynik zawiera medianę i p95 czasu klatki w ms. Na tym etapie możesz zakończyć pracę na 3.0.

### 2. Porównaj i wyjaśnij (3.5–4.0)

Zmień liczbę obiektów z 10 na 100 i uruchom drugi pomiar. Zachowaj tę samą trasę kamery, rozdzielczość, cienie i długość pomiaru. Porównaj medianę i p95 obu zapisów (3.5). W jednym polu wniosku napisz, jak zmienił się czas klatki w twoich danych (4.0). Brak zauważalnej różnicy także jest wynikiem.

### 3. Ulepsz i sprawdź ograniczenie (4.5–5.0)

Dobierz liczbę obiektów do budżetu klatki wyświetlonego na stanowisku. Zmieniaj najpierw samą liczbę obiektów, a ustawienie cieni sprawdź w osobnej próbie. Zachowaj wynik przed i po zmianie (4.5). W wybranej konfiguracji włącz przygotowane obciążenie fizyki, ponów pomiar i dopisz do tego samego wniosku, kiedy konfiguracja przestaje spełniać budżet (5.0).

## Co zrobić na ocenę

| Poziom | Dowód z tego samego doświadczenia |
|---|---|
| 3.0 | Włącz pomiar sceny bazowej i obserwuj 3 skale |
| 3.5 | Zmierz 10 i 100 obiektów |
| 4.0 | Wyjaśnij związek liczby obiektów z frame time w swoich danych |
| 4.5 | Dobierz obciążenie i cienie, aby utrzymać budżet klatki |
| 5.0 | Sprawdź dodatkowy warunek fizyki i wskaż ograniczenie porównania |

Interpretację i ocenę końcową zatwierdza prowadzący.

## Co sprawdzić w zapisanym wyniku

| Wielkość | Jednostka | Kontrola |
|---|---|---|
| Czas klatki: mediana, p95 | ms | Porównuj tę samą trasę i tryb Editor/build. |
| Liczba aktywnych obiektów | liczba | Sprawdź rzeczywisty stan A/B. |
| Warunki próby | opis | Cienie, fizyka, rozdzielczość i czas zapisu. |

**Warunki porównania:** Ta sama kamera, trasa, rozdzielczość, oświetlenie, czas rozgrzewki i okno pomiaru. VSync i limit FPS muszą pozostać takie same.

**Nieważna próba:** Próba jest nieważna, jeśli zmieniła się trasa lub okno pomiaru, scena nie zakończyła rozgrzewki albo zapis nie zawiera poprawnych próbek.

## Jeden wniosek

Przy zmianie ___ czas klatki zmienił się ___; porównanie ogranicza ___.

## Oddanie całego bloku

1. Blok 01: Lab 01 i Lab 02. Zachowaj dwa osobne wyniki: numer labu, indeksy, grupę G1/G2, tryb danych, próby i osiągnięty etap. Od 4.0 dopisz jeden krótki wniosek na laboratorium.
2. Po każdym laboratorium otwórz WiRR > Raporty > Formularz raportu laboratoryjnego. Sprawdź numer labu i kompletność. Kliknij „Zapisz raport lokalnie (bez logowania)”. Unity zapisze JSON i rozpocznie wysyłkę. System automatycznie przekaże zgłoszenie do KIA-students/wirr. Nie zakładaj forka, nie używaj Git ani nie wysyłaj e-maila przy poprawnej wysyłce.
3. Na końcu bloku sprawdź potwierdzenie przyjęcia przez serwer dla obu laboratoriów. Sam zapis lokalny nie oznacza oddania. Przyjęcie przez serwer oznacza oczekiwanie na automatyczne przekazanie do repozytorium, a nie zatwierdzoną ocenę. W parze wystarczą dwa wyniki ze wspólnego stanowiska z oboma indeksami.
4. Brak potwierdzenia: zachowaj pliki i użyj „Wyślij ponownie zapisany raport”. Bez Internetu ponów po odzyskaniu połączenia. Jeśli nadal się nie udaje, zgłoś błąd prowadzącemu i awaryjnie wyślij lab01.json i lab02.json w ZIP WiRR-blok-01-G1-INDEKS.zip na m.pomianek@prz.edu.pl; temat: WiRR blok 01, grupa, indeksy. Wstaw własną grupę i indeksy; w parze wysyła jedna osoba.
5. Tylko gdy nie działa także eksport JSON, uzupełnij WiRR-blok-01-raport-awaryjny.md dla obu laboratoriów i wyślij ten jeden plik zamiast ZIP na ten sam adres. Zachowaj kopię. Nie dołączaj projektu Unity, slajdów ani dodatkowego sprawozdania.

## Gdy stanowisko nie działa

Po 5 minutach bez możliwości rozpoczęcia zgłoś prowadzącemu pierwszy błąd. Przejdź z jego pomocą do przygotowanego stanowiska desktop. Nie instaluj SDK, nie buduj APK ani nie pisz brakującego modułu. Zapisz rzeczywisty tryb i wykonane etapy; brakującej liczby nie zastępuj zerem.

Telefon i gogle nie są wymagane do żadnego progu. Nie zmieniaj komputera ani konfiguracji w trakcie porównania.

## Jak czytać schemat

**Obrót i translacja:** Zmiana pozycji kamery daje inne przesunięcia obiektów bliskich i dalekich. Nie myl tego z powiększeniem modelu.

**Budżet jest warunkiem:** Dobierz częstotliwość docelową dla stanowiska. Obliczony czas na klatkę nie jest zmierzonym czasem renderowania.

**Kontrola porównania:** Porównuj te same fragmenty trasy, tę samą rozdzielczość i okno pomiaru; nie rozstrzygaj CPU/GPU na podstawie samego FPS.

[Szablon tylko przy awarii eksportu JSON](../../Documentation~/WiRR2/WiRR-blok-01-raport-awaryjny.md).

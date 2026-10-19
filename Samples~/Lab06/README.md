# Lab 06: Bliźniak cyfrowy powinien wiedzieć, kiedy nie wie

## Spis treści

- [Co będziesz badać](#co-będziesz-badać)
- [Jedno doświadczenie, trzy działania](#jedno-doświadczenie-trzy-działania)
- [Co zrobić na ocenę](#co-zrobić-na-ocenę)
- [Co sprawdzić w zapisanym wyniku](#co-sprawdzić-w-zapisanym-wyniku)
- [Jeden wniosek](#jeden-wniosek)
- [Oddanie całego bloku](#oddanie-całego-bloku)
- [Gdy stanowisko nie działa](#gdy-stanowisko-nie-działa)
- [Jak czytać schemat](#jak-czytać-schemat)


**Blok 03, część 2 z 2** · 90 minut · ROS_REPLAY · 2.0.0-draft.3

**Pytanie:** Czy płynny ruch robota oznacza, że jego stan jest aktualny?

## Co będziesz badać

JointState przypisuje położenia przegubom przez ich nazwy, nie przez przypadkową kolejność w tablicy. REPLAY odtwarza zapis; ROS_LIVE wymaga rzeczywistego źródła. STALE oznacza przekroczenie przyjętego wieku danych. Odstęp między wiadomościami i opóźnienie wykrycia STALE nie są RTT. RTT wymaga pomiaru drogi żądanie–odpowiedź.

## Jedno doświadczenie, trzy działania

Zacznij od 3.0. Kolejne poziomy rozwijają to samo doświadczenie. Możesz zakończyć na osiągniętym etapie i przejść do drugiego laboratorium bloku. Pracuj solo lub w parze; w parze zamieniajcie role operatora i obserwatora.

### 1. Uruchom odtwarzanie danych (3.0)

Otwórz przygotowaną stację ROS_REPLAY. Sprawdź nazwę zapisu, nazwy przegubów i jednostki. Uruchom odtwarzanie. Obserwuj ruch jednego przegubu, numer kolejnej wiadomości i wiek danych. Przez całe laboratorium źródłem jest zapis; nie uruchamiaj ROS_LIVE ani serwera ROS 2.

### 2. Zatrzymaj dane i wyjaśnij (3.5–4.0)

Wstrzymaj dostarczanie nowych wiadomości przygotowanym przełącznikiem. Obserwuj obraz przez 5 s, następnie wznów dane. Odczytaj czas od ostatniej wiadomości do STALE i czas odzyskania (3.5). W jednym wniosku wyjaśnij, dlaczego płynne odświeżanie obrazu nie dowodzi aktualności danych (4.0). STALE jest stanem aktualności, nie innym źródłem danych.

### 3. Dobierz watchdog i sprawdź profil (4.5–5.0)

Zmień wyłącznie próg watchdoga. Dobierz wartość, która wykrywa przygotowaną przerwę bez ciągłych fałszywych alertów. Powtórz pauzę i wznowienie, zachowując ten sam zapis (4.5). Włącz drugi przygotowany profil przerw, bez zmiany progu. Dopisz do tego samego wniosku, kiedy twoje ustawienie zawodzi (5.0). Nie opisuj odstępów wiadomości ani czasu alertu jako RTT.

## Co zrobić na ocenę

| Poziom | Dowód z tego samego doświadczenia |
|---|---|
| 3.0 | Uruchom przygotowany REPLAY i odczytaj stan |
| 3.5 | Wstrzymaj strumień i zmierz przejście STALE |
| 4.0 | Wyjaśnij różnicę płynności obrazu i aktualności danych |
| 4.5 | Dobierz watchdog/reakcję na STALE i sprawdź przywrócenie |
| 5.0 | Sprawdź drugi profil opóźnienia i uzasadnij bezpieczną strategię; wyraźnie rozróżnij LIVE/REPLAY |

Interpretację i ocenę końcową zatwierdza prowadzący.

## Co sprawdzić w zapisanym wyniku

| Wielkość | Jednostka | Kontrola |
|---|---|---|
| Odstęp wiadomości i próg STALE | ms | To inne wielkości niż RTT. |
| Czas od ostatniej wiadomości do alertu | ms | Porównaj z rzeczywistym progiem watchdoga. |
| Czas odzyskania i źródło | s / opis | Powrót do REPLAY nie oznacza przejścia do ROS_LIVE. |

**Warunki porównania:** Ten sam zapis, mapowanie przegubów i profil przerw w porównaniu przed/po. Zmianę progu odróżnij od zmiany bufora.

**Nieważna próba:** Zegar źródła i odbiornika muszą mieć określoną relację. Oznacz nieznane opóźnienia jako brak danych. Nie odtwarzaj ruchu jako aktualnego po utracie źródła.

## Jeden wniosek

Mimo płynnego obrazu dane były ___; strategia ___ pomaga, lecz ___.

## Oddanie całego bloku

1. Blok 03: Lab 05 i Lab 06. Zachowaj dwa osobne wyniki: numer labu, indeksy, grupę G1/G2, tryb danych, próby i osiągnięty etap. Od 4.0 dopisz jeden krótki wniosek na laboratorium.
2. Po każdym laboratorium otwórz WiRR > Raporty > Formularz raportu laboratoryjnego. Sprawdź numer labu i kompletność. Kliknij „Zapisz raport lokalnie (bez logowania)”. Unity zapisze JSON i rozpocznie wysyłkę. System automatycznie przekaże zgłoszenie do KIA-students/wirr. Nie zakładaj forka, nie używaj Git ani nie wysyłaj e-maila przy poprawnej wysyłce.
3. Na końcu bloku sprawdź potwierdzenie przyjęcia przez serwer dla obu laboratoriów. Sam zapis lokalny nie oznacza oddania. Przyjęcie przez serwer oznacza oczekiwanie na automatyczne przekazanie do repozytorium, a nie zatwierdzoną ocenę. W parze wystarczą dwa wyniki ze wspólnego stanowiska z oboma indeksami.
4. Brak potwierdzenia: zachowaj pliki i użyj „Wyślij ponownie zapisany raport”. Bez Internetu ponów po odzyskaniu połączenia. Jeśli nadal się nie udaje, zgłoś błąd prowadzącemu i awaryjnie wyślij lab05.json i lab06.json w ZIP WiRR-blok-03-G1-INDEKS.zip na m.pomianek@prz.edu.pl; temat: WiRR blok 03, grupa, indeksy. Wstaw własną grupę i indeksy; w parze wysyła jedna osoba.
5. Tylko gdy nie działa także eksport JSON, uzupełnij WiRR-blok-03-raport-awaryjny.md dla obu laboratoriów i wyślij ten jeden plik zamiast ZIP na ten sam adres. Zachowaj kopię. Nie dołączaj projektu Unity, slajdów ani dodatkowego sprawozdania.

## Gdy stanowisko nie działa

Po 5 minutach bez możliwości rozpoczęcia zgłoś prowadzącemu pierwszy błąd. Przejdź z jego pomocą do przygotowanego stanowiska desktop. Nie instaluj SDK, nie buduj APK ani nie pisz brakującego modułu. Zapisz rzeczywisty tryb i wykonane etapy; brakującej liczby nie zastępuj zerem.

Telefon i gogle nie są wymagane do żadnego progu. Nie zmieniaj komputera ani konfiguracji w trakcie porównania.

## Jak czytać schemat

**Mapuj po nazwie:** Kolejność elementów wiadomości może się zmieniać. Przypisz nazwę do właściwej osi, zachowując jednostki i znak.

**Dwa zegary jakości:** Częstotliwość renderowania i wiek danych są niezależne. Płynny obraz może przez dłuższy czas przedstawiać nieaktualny stan.

**Właściwa metryka:** Odstęp wiadomości i czas wykrycia STALE nie są RTT. Po wznowieniu replay źródłem nadal pozostaje zapis.

[Szablon tylko przy awarii eksportu JSON](../../Documentation~/WiRR2/WiRR-blok-03-raport-awaryjny.md).


# Lab 02: Chwycić, nie kliknąć: ergonomia manipulacji

## Spis treści

- [Co będziesz badać](#co-będziesz-badać)
- [Jedno doświadczenie, trzy działania](#jedno-doświadczenie-trzy-działania)
- [Co zrobić na ocenę](#co-zrobić-na-ocenę)
- [Co sprawdzić w zapisanym wyniku](#co-sprawdzić-w-zapisanym-wyniku)
- [Jeden wniosek](#jeden-wniosek)
- [Oddanie całego bloku](#oddanie-całego-bloku)
- [Gdy stanowisko nie działa](#gdy-stanowisko-nie-działa)
- [Jak czytać schemat](#jak-czytać-schemat)


**Blok 01, część 2 z 2** · 90 minut · DESKTOP_SIM · 2.0.0-draft.3

**Pytanie:** Kiedy chwyt bezpośredni ustępuje wskazywaniu promieniem?

## Co będziesz badać

Direct oznacza interakcję w bezpośrednim zasięgu interaktora, a Ray wskazanie promieniem. Socket jest gniazdem potwierdzającym poprawne umieszczenie obiektu. Porównujemy czas ukończenia razem z błędami: szybsza, lecz zawodna technika nie musi być lepsza. Na PC ruch myszy lub symulatora opisuje tę metodę wejścia, a nie naturalny chwyt dłoni w goglach.

## Jedno doświadczenie, trzy działania

Zacznij od 3.0. Kolejne poziomy rozwijają to samo doświadczenie. Możesz zakończyć na osiągniętym etapie i przejść do drugiego laboratorium bloku. Pracuj solo lub w parze; w parze zamieniajcie role operatora i obserwatora.

### 1. Wykonaj chwyt bezpośredni (3.0)

Otwórz przygotowaną scenę manipulacji. Wykonaj jedną próbę treningową: chwyć obiekt metodą Direct i umieść go w oznaczonym gnieździe. Następnie wykonaj pięć rejestrowanych prób Direct. Po każdej próbie użyj przygotowanego resetu położenia. Sprawdź zapis czasu, powodzeń i błędów. Nie usuwaj nieudanych prób.

### 2. Porównaj promień i wyjaśnij (3.5–4.0)

Przełącz technikę na Ray. Wykonaj pięć prób z tym samym obiektem, gniazdem i dystansem. Porównaj czas oraz błędy Direct/Ray (3.5). W jednym polu wniosku wskaż technikę lepszą w tym zadaniu i uzasadnij wybór wynikami (4.0). Po pomiarze wypróbuj przygotowaną teleportację; jej wyniku nie dopisuj do serii Direct/Ray.

### 3. Ulepsz i sprawdź drugi dystans (4.5–5.0)

Zmień wyłącznie podświetlenie gniazda: wybierz ustawienie, które według ciebie ułatwi rozpoznanie celu. Powtórz próbę tą samą techniką i porównaj wynik przed/po (4.5). Następnie przełącz cel na drugi przygotowany dystans, pozostawiając podświetlenie i technikę bez zmian. Dopisz do tego samego wniosku, czy poprawka nadal pomaga (5.0).

## Co zrobić na ocenę

| Poziom | Dowód z tego samego doświadczenia |
|---|---|
| 3.0 | Umieść obiekt w gnieździe metodą Direct |
| 3.5 | Powtórz tę samą czynność metodą Ray |
| 4.0 | Wyjaśnij różnicę czasu i błędów |
| 4.5 | Wybierz ustawienie interakcji lub HUD, poprawiające skuteczność |
| 5.0 | Przetestuj przy innym dystansie celu; wskaż ergonomiczny kompromis |

Interpretację i ocenę końcową zatwierdza prowadzący.

## Co sprawdzić w zapisanym wyniku

| Wielkość | Jednostka | Kontrola |
|---|---|---|
| Czas cyklu | s | Od rozpoczęcia działania do poprawnego osadzenia. |
| Udane osadzenia i pomyłki | liczba | Błąd pozostaje zapisany po resecie. |
| Technika, dystans, ustawienie | opis / m | Rozdziel wpływ techniki i geometrii zadania. |

**Warunki porównania:** Ten sam obiekt, rozmiar i pozycja celu, dystans oraz sposób rozpoczęcia próby. Trening oddziel od pomiaru.

**Nieważna próba:** Nie porównuj serii, jeśli zmienił się cel, pomiar obejmuje pauzę albo licznik resetuje błędy. Oznacz taką serię i powtórz.

## Jeden wniosek

Technika ___ pomaga przy ___; koszt lub ograniczenie tej decyzji to ___.

## Oddanie całego bloku

1. Blok 01: Lab 01 i Lab 02. Zachowaj dwa osobne wyniki: numer labu, indeksy, grupę G1/G2, tryb danych, próby i osiągnięty etap. Od 4.0 dopisz jeden krótki wniosek na laboratorium.
2. Po każdym laboratorium zakończ Play i wybierz `WiRR → Nowe instrukcje → Przenieś wyniki do formularza raportu`. Wybierz zapisaną sesję właściwego bloku i bieżące laboratorium, a następnie otwórz `WiRR → Raporty → Formularz raportu laboratoryjnego`. Sprawdź numer labu i kompletność. Kliknij „Zapisz raport lokalnie (bez logowania)”. Unity zapisze JSON i rozpocznie wysyłkę. System automatycznie przekaże zgłoszenie do KIA-students/wirr. Nie zakładaj forka, nie używaj Git ani nie wysyłaj e-maila przy poprawnej wysyłce.
3. Na końcu bloku sprawdź potwierdzenie przyjęcia przez serwer dla obu laboratoriów. Sam zapis lokalny nie oznacza oddania. Przyjęcie przez serwer oznacza oczekiwanie na automatyczne przekazanie do repozytorium, a nie zatwierdzoną ocenę. W parze wystarczą dwa wyniki ze wspólnego stanowiska z oboma indeksami.
4. Brak potwierdzenia: zachowaj pliki i użyj „Wyślij ponownie zapisany raport”. Bez Internetu ponów po odzyskaniu połączenia. Jeśli nadal się nie udaje, zgłoś błąd prowadzącemu i awaryjnie wyślij lab01.json i lab02.json w ZIP WiRR-blok-01-G1-INDEKS.zip na m.pomianek@prz.edu.pl; temat: WiRR blok 01, grupa, indeksy. Wstaw własną grupę i indeksy; w parze wysyła jedna osoba.
5. Tylko gdy nie działa także eksport JSON, uzupełnij WiRR-blok-01-raport-awaryjny.md dla obu laboratoriów i wyślij ten jeden plik zamiast ZIP na ten sam adres. Zachowaj kopię. Nie dołączaj projektu Unity, slajdów ani dodatkowego sprawozdania.

## Gdy stanowisko nie działa

Po 5 minutach bez możliwości rozpoczęcia zgłoś prowadzącemu pierwszy błąd. Przejdź z jego pomocą do przygotowanego stanowiska desktop. Nie instaluj SDK, nie buduj APK ani nie pisz brakującego modułu. Zapisz rzeczywisty tryb i wykonane etapy; brakującej liczby nie zastępuj zerem.

Telefon i gogle nie są wymagane do żadnego progu. Nie zmieniaj komputera ani konfiguracji w trakcie porównania.

## Jak czytać schemat

**To samo zadanie:** Obiekt, gniazdo i dystans muszą być takie same w serii Direct/Ray. Zmiana techniki nie może zmieniać trudności celu.

**Osobno trening:** Zachowaj kolejność serii i oddziel rozgrzewkę. Poprawa z czasem może wynikać także z uczenia się.

**Bez uogólnienia na HMD:** Sterowanie PC ilustruje technikę wejścia. Nie mierzy naturalnego chwytu ręki ani komfortu w goglach.

[Szablon tylko przy awarii eksportu JSON](../../Documentation~/WiRR2/WiRR-blok-01-raport-awaryjny.md).


---

<div style="break-before: page; page-break-before: always;"></div>

## Załączniki do druku

[Załącznik 1. Karta pomiarowa (02), Markdown do wydruku](https://github.com/MatPomGit/prz/blob/main/courses/wirtualna-i-rozszerzona-rzeczywistosc/laboratories/lab-02/karta-pomiarowa.md)

[Załącznik 2. Kwestionariusz SSQ, 16 objawów, pomiar przed/po](https://github.com/MatPomGit/prz/blob/main/courses/wirtualna-i-rozszerzona-rzeczywistosc/laboratories/lab-02/kwestionariusz-ssq.md)

Każdy załącznik jest **osobnym plikiem Markdown** z podziałem strony na początku i logotypami PRz oraz KIA. Otwórz i wydrukuj go oddzielnie lub zachowaj podział stron w dokumencie zbiorczym. Karta pomiarowa służy do odręcznego notowania, nie jest potwierdzeniem przyjęcia raportu przez Supabase.

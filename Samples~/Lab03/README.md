# Lab 03: Kotwica to nie gwarancja stałości

**Blok 02, część 1 z 2** · 90 minut · SIM_AR · 2.0.0-draft.3

**Pytanie:** Czy stabilny hologram znajduje się w poprawnym miejscu?

## Co będziesz badać

Wykrycie płaszczyzny, trafienie promienia i utworzenie kotwicy to różne operacje. Błąd położenia oznacza różnicę względem znanej referencji; stabilność dotyczy zmian położenia w czasie. Stabilny model może mieć stałe przesunięcie. W SIM_AR referencja i błędy pochodzą z modelu. Rzeczywiste AR bez niezależnego odniesienia nie pozwala automatycznie zmierzyć bezwzględnej dokładności.

## Jedno doświadczenie, trzy działania

Zacznij od 3.0. Kolejne poziomy rozwijają to samo doświadczenie. Możesz zakończyć na osiągniętym etapie i przejść do drugiego laboratorium bloku. Pracuj solo lub w parze; w parze zamieniajcie role operatora i obserwatora.

### 1. Umieść model w symulacji AR (3.0)

Uruchom przygotowaną Galerię kotwic w trybie SIM_AR. Znajdź oznaczony punkt odniesienia i odcinek 0,30 m. Umieść przy nim model w skali 1:1. Sprawdź stan śledzenia i odczytaj błąd położenia w cm. Korzystasz z symulacji na komputerze; nie instaluj aplikacji na telefonie.

### 2. Porównaj warunki i wyjaśnij (3.5–4.0)

Uruchom kolejno przygotowane warunki: powierzchnia z wieloma cechami i powierzchnia z małą liczbą cech. W obu zachowaj ten sam punkt odniesienia, tor kamery i model. Zbierz po trzy odczyty błędu w cm i porównaj ich rozrzut (3.5). W jednym wniosku wyjaśnij różnicę między stałym przesunięciem a niestabilnością modelu (4.0).

### 3. Dobierz warunek i sprawdź powrót (4.5–5.0)

Wybierz warunek śledzenia, który lepiej utrzymuje model przy referencji, i potwierdź wybór ponowną próbą (4.5). Uruchom przygotowaną utratę śledzenia i wznowienie. Sprawdź położenie po powrocie oraz czas odzyskania. Dopisz do tego samego wniosku, czego symulacja nie mówi o dokładności rzeczywistego telefonu (5.0).

## Co zrobić na ocenę

| Poziom | Dowód z tego samego doświadczenia |
|---|---|
| 3.0 | Umieść model przy referencji w REAL/SIM |
| 3.5 | Porównaj dwa warunki obserwacji/trackingu |
| 4.0 | Wyjaśnij różnicę błędu pozycji |
| 4.5 | Dobierz położenie lub warunki śledzenia zmniejszające błąd |
| 5.0 | Ponów próbę po utracie/relocalizacji i wskaż niepewność; nie mieszaj SIM z REAL |

Interpretację i ocenę końcową zatwierdza prowadzący.

## Co sprawdzić w zapisanym wyniku

| Wielkość | Jednostka | Kontrola |
|---|---|---|
| Błąd translacji względem referencji | cm | Tylko przy znanym punkcie odniesienia. |
| Rozrzut odczytów i utraty śledzenia | cm / liczba | Rozdziel błąd systematyczny i zmienność. |
| Czas ponownej lokalizacji | s | Brak odzyskania to nie zero sekund. |

**Warunki porównania:** Ta sama referencja, model, skala, tor obserwacji i liczba odczytów. W symulacji zachowaj profil i seed.

**Nieważna próba:** Oznacz próbę jako nieważną przy braku referencji lub śledzenia. Nie mieszaj SIM_AR z REAL_AR w jednej serii.

## Jeden wniosek

Warunek ___ zmienił ___; po utracie śledzenia ___, a wynik nie dowodzi ___.

## Oddanie całego bloku

1. Blok 02: Lab 03 i Lab 04. Zachowaj dwa osobne wyniki: numer labu, indeksy, grupę G1/G2, tryb danych, próby i osiągnięty etap. Od 4.0 dopisz jeden krótki wniosek na laboratorium.
2. Po każdym laboratorium otwórz WiRR > Raporty > Formularz raportu laboratoryjnego. Sprawdź numer labu i kompletność. Kliknij „Zapisz raport lokalnie (bez logowania)”. Unity zapisze JSON i rozpocznie wysyłkę. System automatycznie przekaże zgłoszenie do KIA-students/wirr. Nie zakładaj forka, nie używaj Git ani nie wysyłaj e-maila przy poprawnej wysyłce.
3. Na końcu bloku sprawdź potwierdzenie przyjęcia przez serwer dla obu laboratoriów. Sam zapis lokalny nie oznacza oddania. Przyjęcie przez serwer oznacza oczekiwanie na automatyczne przekazanie do repozytorium, a nie zatwierdzoną ocenę. W parze wystarczą dwa wyniki ze wspólnego stanowiska z oboma indeksami.
4. Brak potwierdzenia: zachowaj pliki i użyj „Wyślij ponownie zapisany raport”. Bez Internetu ponów po odzyskaniu połączenia. Jeśli nadal się nie udaje, zgłoś błąd prowadzącemu i awaryjnie wyślij lab03.json i lab04.json w ZIP WiRR-blok-02-G1-INDEKS.zip na m.pomianek@prz.edu.pl; temat: WiRR blok 02, grupa, indeksy. Wstaw własną grupę i indeksy; w parze wysyła jedna osoba.
5. Tylko gdy nie działa także eksport JSON, uzupełnij WiRR-blok-02-raport-awaryjny.md dla obu laboratoriów i wyślij ten jeden plik zamiast ZIP na ten sam adres. Zachowaj kopię. Nie dołączaj projektu Unity, slajdów ani dodatkowego sprawozdania.

## Gdy stanowisko nie działa

Po 5 minutach bez możliwości rozpoczęcia zgłoś prowadzącemu pierwszy błąd. Przejdź z jego pomocą do przygotowanego stanowiska desktop. Nie instaluj SDK, nie buduj APK ani nie pisz brakującego modułu. Zapisz rzeczywisty tryb i wykonane etapy; brakującej liczby nie zastępuj zerem.

Telefon i gogle nie są wymagane do żadnego progu. Nie zmieniaj komputera ani konfiguracji w trakcie porównania.

## Jak czytać schemat

**Błąd i stabilność:** Mały rozrzut oznacza powtarzalność odczytów; nie wyklucza stałego przesunięcia względem referencji.

**Znane odniesienie:** W symulacji pozycja referencyjna jest znana. W realnym AR bez niezależnej referencji oceniaj stabilność, nie deklaruj bezwzględnej dokładności.

**Utrata śledzenia:** Po ponownej lokalizacji sprawdź zarówno zmianę położenia, jak i rozrzut. Brak odczytu nie jest zerowym błędem.

[Szablon tylko przy awarii eksportu JSON](../../Documentation~/WiRR2/WiRR-blok-02-raport-awaryjny.md).

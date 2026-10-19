# Lab 04: Za ścianą rzeczywistość też obowiązuje

## Spis treści

- [Co będziesz badać](#co-będziesz-badać)
- [Przygotowanie stanowiska i otwarcie sceny](#przygotowanie-stanowiska-i-otwarcie-sceny)
- [Jedno doświadczenie, trzy działania](#jedno-doświadczenie-trzy-działania)
- [Co zrobić na ocenę](#co-zrobić-na-ocenę)
- [Co sprawdzić w zapisanym wyniku](#co-sprawdzić-w-zapisanym-wyniku)
- [Jeden wniosek](#jeden-wniosek)
- [Oddanie całego bloku](#oddanie-całego-bloku)
- [Gdy stanowisko nie działa](#gdy-stanowisko-nie-działa)
- [Jak czytać schemat](#jak-czytać-schemat)


**Blok 02, część 2 z 2** · 90 minut · SIM_AR · 2.0.0-draft.3

Dlaczego poprawnie narysowany obiekt może być widoczny w złym miejscu?

## Co będziesz badać

Okluzja oznacza zasłanianie obiektu przez bliższą powierzchnię. Test głębi porównuje odległości, a raycast szuka przecięcia promienia z geometrią. Poprawne trafienie promienia nie gwarantuje poprawnej okluzji obrazu. Oświetlenie wpływa na wygląd, lecz nie naprawia błędnej kolejności zasłaniania. W tym doświadczeniu PC korzysta z geometrii i głębi syntetycznej.

## Przygotowanie stanowiska i otwarcie sceny

Użyj Unity 6000.6.5f1 i przygotowanego projektu URP. Otwórz WiRR → Nowe instrukcje → Materiały studenta i wybierz scenę laboratorium 04. Następnie naciśnij Play i w zakładce Game rozpocznij sesję z własnym indeksem i grupą. Nie używaj generatora starszej sceny bazowej do tego doświadczenia.

[Pełna instrukcja krok po kroku: instalacja, scena, przyciski i pomiary](../../Documentation~/WiRR2/LAB04-STUDENT-GUIDE.md). Przygotowanie brakujących scen wykonuje prowadzący; istniejące sceny studenta są zachowywane.

Po doświadczeniu wyłącz Play i [przenieś sesję do istniejącego formularza raportu](../../Documentation~/WiRR2/SUPABASE-REPORT-BRIDGE.md). Eksport z panelu sceny jest kopią lokalną i sam nie oddaje pracy.

## Jedno doświadczenie, trzy działania

Zacznij od 3.0. Kolejne poziomy rozwijają to samo doświadczenie. Możesz zakończyć na osiągniętym etapie i przejść do drugiego laboratorium bloku. Pracuj solo lub w parze; w parze zamieniajcie role operatora i obserwatora.

### 1. Znajdź błąd zasłaniania (3.0)

Uruchom przygotowaną scenę SIM_AR z przeszkodą i modelem. Z pierwszego oznaczonego miejsca wskaż obiekt widoczny przez przeszkodę. Następnie obejrzyj przygotowaną etykietę i odpowiedź promienia; wskaż, które z nich są niezgodne z geometrią. Zapisz własne odpowiedzi przed wyświetleniem rozwiązania.

### 2. Porównaj okluzję i wyjaśnij (3.5–4.0)

W tym samym widoku przełącz przygotowaną okluzję z niepoprawnej na zgodną z geometrią odniesienia. Powtórz wskazania i porównaj poprawne odpowiedzi, błędy widoczności i fałszywe alarmy (3.5). W jednym wniosku wyjaśnij, dlaczego trafienie promienia nie gwarantuje poprawnego zasłaniania obrazu (4.0).

### 3. Popraw ustawienie i sprawdź widok (4.5–5.0)

Zmień wyłącznie parametr głębi udostępniony w scenie tak, aby ograniczyć wykryty błąd; oświetlenie pozostaw bez zmian. Porównaj wynik przed/po (4.5). Przenieś kamerę do drugiego oznaczonego miejsca. Dopisz do tego samego wniosku, czy poprawka nadal działa i jaki ma limit (5.0).

## Co zrobić na ocenę

| Poziom | Dowód z tego samego doświadczenia |
|---|---|
| 3.0 | Znajdź błędnie zasłaniany obiekt |
| 3.5 | Porównaj okluzję błędną/poprawną |
| 4.0 | Wyjaśnij anomalię używając pojęcia głębi/raycast |
| 4.5 | Dobierz parametr głębi poprawiający zgodność sceny; zachowaj stałe oświetlenie |
| 5.0 | Sprawdź z innego punktu widzenia i wskaż ograniczenie rozwiązania |

Interpretację i ocenę końcową zatwierdza prowadzący.

## Co sprawdzić w zapisanym wyniku

| Wielkość | Jednostka | Kontrola |
|---|---|---|
| Poprawne wskazania i fałszywe alarmy | liczba | Wynik odnosi się do znanego zestawu obiektów. |
| Błędy widoczności | liczba | Oddziel je od liczby kliknięć. |
| Źródło głębi, widok i ustawienie | opis | Zachowaj rozdzielenie danych syntetycznych i urządzeniowych. |

Ta sama geometria, obiekty, punkt obserwacji i zadanie wskazywania. Zmieniaj tylko jeden parametr naraz.

Nie przypisuj pomiaru głębi urządzeniu, jeśli provider go nie udostępnia. Brak trafienia lub danych nie jest zerowym błędem.

## Jeden wniosek

Anomalia wynikała z ___; poprawka ___ działała dla ___, ale ___.

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

**Ten sam promień:** Porównanie odległości dotyczy tego samego promienia kamery. Inna pozycja obserwacji może zmienić kolejność przesłaniania.

**Dwa mechanizmy:** Raycast pyta o przecięcie promienia z geometrią; okluzja określa widoczność fragmentów obrazu. Jedna poprawna odpowiedź nie zastępuje drugiej.

**Drugi widok:** Korekta sprawdzona z jednej kamery może zawieść po zmianie widoku. Nie naprawiaj błędu głębi samą zmianą oświetlenia.

[Szablon tylko przy awarii eksportu JSON](../../Documentation~/WiRR2/WiRR-blok-02-raport-awaryjny.md).

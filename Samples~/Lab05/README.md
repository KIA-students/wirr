# Lab 05: Geometria kosztuje, ale otwór montażowy musi zostać

## Spis treści

- [Co będziesz badać](#co-będziesz-badać)
- [Jedno doświadczenie, trzy działania](#jedno-doświadczenie-trzy-działania)
- [Co zrobić na ocenę](#co-zrobić-na-ocenę)
- [Co sprawdzić w zapisanym wyniku](#co-sprawdzić-w-zapisanym-wyniku)
- [Jeden wniosek](#jeden-wniosek)
- [Oddanie całego bloku](#oddanie-całego-bloku)
- [Gdy stanowisko nie działa](#gdy-stanowisko-nie-działa)
- [Jak czytać schemat](#jak-czytać-schemat)


**Blok 03, część 1 z 2** · 90 minut · DESKTOP_SIM · 2.0.0-draft.3

**Pytanie:** Który najlżejszy model nadal spełnia funkcję części?

## Co będziesz badać

Model CAD opisuje geometrię konstrukcyjną; siatka renderingu przybliża jej powierzchnię trójkątami. Redukcja siatki może usunąć ważną cechę, np. otwór. LOD wybiera szczegółowość zależnie od widoku. Collider opisuje kolizję i nie musi odpowiadać widocznej siatce. Liczba trójkątów jest jedną z przyczyn kosztu, a nie pełną miarą wydajności.

## Jedno doświadczenie, trzy działania

Zacznij od 3.0. Kolejne poziomy rozwijają to samo doświadczenie. Możesz zakończyć na osiągniętym etapie i przejść do drugiego laboratorium bloku. Pracuj solo lub w parze; w parze zamieniajcie role operatora i obserwatora.

### 1. Sprawdź geometrię modelu (3.0)

Otwórz przygotowany Warsztat CAD. Obejrzyj High, Medium i Low tej samej części, bez zmiany skali. Znajdź wskazany otwór montażowy i krawędź dopasowania. Dla każdej siatki sprawdź, czy te cechy pozostały. Korzystaj z gotowych modeli; nie konwertuj plików STEP.

### 2. Porównaj koszt i wyjaśnij (3.5–4.0)

Uruchom przygotowany benchmark kolejno dla High, Medium i Low, przy tej samej trasie kamery i rozgrzewce. Porównaj trójkąty, materiały i czas klatki w ms (3.5). Zbliż i oddal kamerę, obserwując zmianę LOD. W jednym wniosku wskaż najlżejszy wariant zachowujący wymaganą cechę (4.0).

### 3. Dobierz LOD i sprawdź granicę (4.5–5.0)

Zmień tylko próg przełączania LOD. Dobierz go tak, aby wymagany otwór pozostał rozpoznawalny przy mniejszym koszcie; sprawdź wynik przed/po (4.5). Powtórz kontrolę przy drugim przygotowanym dystansie kamery. Dopisz do tego samego wniosku, kiedy uproszczenie przestaje nadawać się do instruktażu tej części (5.0).

## Co zrobić na ocenę

| Poziom | Dowód z tego samego doświadczenia |
|---|---|
| 3.0 | Obejrzyj przygotowane High/Low i ich cechy |
| 3.5 | Porównaj geometrię i koszt w benchmarku |
| 4.0 | Wyjaśnij kompromis redukcji siatki |
| 4.5 | Dobierz LOD zachowujący wymagane otwory przy mniejszym koszcie |
| 5.0 | Sprawdź przy innej odległości lub liczbie materiałów, wskaż granicę uproszczenia |

Interpretację i ocenę końcową zatwierdza prowadzący.

## Co sprawdzić w zapisanym wyniku

| Wielkość | Jednostka | Kontrola |
|---|---|---|
| Trójkąty i materiały | liczba | Sprawdź aktywny wariant, nie tylko nazwę pliku. |
| Czas klatki | ms | Ta sama trasa, rozgrzewka i okno pomiaru. |
| Zachowanie cechy i dystans LOD | tak/nie / m | Więcej FPS nie kompensuje brakującego otworu. |

**Warunki porównania:** Ta sama część, skala, orientacja, oświetlenie, trasa kamery i stanowisko. Oddziel zmianę siatki od zmiany materiałów.

**Nieważna próba:** Brak siatki referencyjnej, błędna skala albo nieznana cecha wymiarowa uniemożliwia ocenę zachowania geometrii. Nie zastępuj jej oceną wizualnej atrakcyjności.

## Jeden wniosek

Wariant ___ zachowuje ___ przy koszcie ___; jego granicą jest ___.

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

**Cecha przed kosztem:** Najmniejsza siatka nie musi zachować funkcji części. Sprawdź otwór, krawędź lub wymiar wskazany w scenie.

**LOD i collider:** Zmiana widocznej siatki nie dowodzi zmiany geometrii kolizyjnej. Sprawdzaj te reprezentacje niezależnie.

**Przypadek użycia:** Wymagana dokładność zależy od zadania i dystansu. Próg LOD odczytaj z przygotowanej konfiguracji, nie z tej ilustracji.

[Szablon tylko przy awarii eksportu JSON](../../Documentation~/WiRR2/WiRR-blok-03-raport-awaryjny.md).

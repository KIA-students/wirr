# WiRR: rejestr postępu, identyfikator i ocena

## Ścieżka studenta

Po zainstalowaniu pakietu Unity 6.6 student wpisuje numer indeksu i wybiera grupę G1/G2. Pakiet zapisuje numer lokalnie do wyliczania wariantów ćwiczeń. Każdemu indeksowi przypisuje losowy, stały na danym stanowisku identyfikator techniczny UUID, niezawierający indeksu, daty ani wersji Unity. Zmiana wersji Unity nie zmienia identyfikatora.

Stan techniczny ośmiu laboratoriów jest automatycznie wykrywany i zapisywany co około 60 sekund w `Library/WiRRProgress/progress.json` i `Library/WiRRProgress/outbox/progress.json`. Pola: identyfikator techniczny, grupa, wersja Unity, czas oraz flagi `scenePrepared` i `reportExported` dla Lab 01–08. `gradingStatus=not_assessed` i `submissionStatus=local_only` zapobiegają myleniu aktywności z zatwierdzoną oceną.

## Docelowa automatyczna synchronizacja bez GitHub loginu

**Jeszcze nieaktywna**: wyłącznie serwer pośredniczący, nie Unity, otrzymuje token do tworzenia PR w publicznym `KIA-students/wirr`. Wymagane są: indywidualny, jednorazowy kod aktywacyjny wydany przez prowadzącego (nie login GitHub); potwierdzenie indeksu, grupy i zgody na zakres publikacji; niezależne uwierzytelnienie kolejnych zgłoszeń; limity żądań i walidacja schematu; mapowanie indeksu na UUID w niepublicznej bazie.

Publiczne PR mają zawierać tylko zatwierdzone i pozbawione danych identyfikacyjnych zestawienia. Surowe raporty mogą zawierać numer indeksu, identyfikatory kont, dowolne teksty i dane pomiarowe, dlatego nie wolno bez kontroli ich automatycznie commitować do repozytorium publicznego. Serwer musi usuwać te dane przed publikacją, a prowadzący zatwierdza ocenę. Skrypt lokalnego sprawdzania poprawności raportu nie nadaje automatycznie oceny końcowej.

## Dlaczego system jest etapowy

Publiczny GitHub przechowuje historię, forki i kopie zmian. Nawet niewyróżniony jako taki numer indeksu może identyfikować studenta. Brak logowania w ogóle pozwala podszyć się pod dowolny indeks. Token repozytorium nie może być zapisany w dystrybuowanym pakiecie Unity. Obecnie repozytorium zawiera automatyczny **lokalny** zapis, lecz nie skonfigurowano bezpiecznych kodów aktywacyjnych ani produkcyjnego odbiornika.

## Kryterium włączenia publikacji

1. Uwierzytelniony backend z blokadą grupy po stronie serwera i prywatnym mapowaniem UUID–indeks.
2. Walidator minimalizujący dane przed PR oraz lista dozwolonych pól.
3. Oddzielenie wskaźnika postępu od oceny potwierdzonej przez prowadzącego.
4. Retencja danych, sposób korekty i usunięcia, ochrona przed powieleniem wpisów.
5. Testy pełnego scenariusza w Unity 6.6 i repozytorium testowym.

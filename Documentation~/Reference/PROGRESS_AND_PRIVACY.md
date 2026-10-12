# Automatyczny postęp i przypisanie grupy

## Obecnie wdrożone

Po pierwszym uruchomieniu Unity student wpisuje numer indeksu i wybiera grupę z oficjalnego harmonogramu. Przypisanie grupy jest blokowane **lokalnie na danym stanowisku** dla tego indeksu, również po zmianie projektu Unity. Nazwę drużyny może zmieniać niezależnie od grupy.

Własny projekt zapisuje co około 60 sekund w `Library/WiRRProgress/progress.json` informację o:
- dostępności zapisanej sceny dla każdego Lab 01–08,
- obecności eksportu raportu,
- oznaczeniu grupy oraz losowym technicznym identyfikatorze,
- wersji Unity i czasie utworzenia snapshotu.

Snapshot nie zawiera numeru indeksu, pomiarów ani treści raportów. To **wskaźniki techniczne**, nie oceny ani potwierdzone ukończenie ćwiczenia. Szkice raportów przechowywane lokalnie mogą zawierać indeks zgodnie z wymaganiami wariantów ćwiczeń. Nie należy commitować katalogu `Library`.

## Jeszcze niewdrożone: repozytorium zdalne

Nie wysyłamy informacji identyfikujących studentów do **publicznego** `KIA-students/wirr`. Przegląd repozytorium nie wykazał zatwierdzonego, uwierzytelnionego serwera odbierającego postępy studentów. Umieszczenie tokenu GitHub w pakiecie Unity, automatyczne tworzenie publicznych commitów z indeksami lub wykorzystanie osobistego konta prowadzącego byłoby niewłaściwe.

Do automatycznego raportowania na serwer uczelniany należy przygotować osobny **prywatny** system zapisów: logowanie przez konto studenta, mapowanie indeks–grupa kontrolowane przez prowadzącego, API przyjmujące tylko minimalne metadane, kontrola dostępu, retencja danych i usuwanie, a dopiero po stronie serwera ewentualny commit do prywatnego repozytorium lub zapis bazy. Grupa powinna zostać zablokowana **po stronie serwera**, bo zapis `EditorPrefs` nie zabezpiecza przed zmianami na innym komputerze.

Do czasu zatwierdzenia tego mechanizmu lokalny zapis postępu pozostaje automatyczny, ale **nie ma automatycznej wysyłki do GitHub**.

## Kryteria akceptacji przed włączeniem raportowania sieciowego

- Brak poświadczeń GitHub, PAT i sekretów uczelni w aplikacji, `Assets`, `EditorPrefs` i raportach.
- Uwierzytelnienie studenta i powiązanie numeru indeksu z jego kontem.
- Jeden zatwierdzony przydział grupy na cały cykl i kontrolowana korekta przez prowadzącego.
- Brak danych osobowych w logach CI i publicznych gałęziach `main`, `upm`, `assets`.
- Ponawianie transmisji i idempotentne aktualizacje bez duplikowania wpisów.
- Jawny opis zakresu danych, celu, okresu przechowywania i uprawnionych odbiorców.

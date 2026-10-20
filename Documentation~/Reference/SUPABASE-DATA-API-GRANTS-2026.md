# WiRR: jawne uprawnienia Data API od 30.10.2026

Źródło: powiadomienie Supabase z 20.10.2026, [dyskusja Supabase](https://github.com/orgs/supabase/discussions/45329).

## Zakres audytu `KIA-students/wirr`

Na `main` nie ma plików `.sql` ani `supabase/migrations`. Migracje źródłowe znajdują się poza tym repozytorium i **nie zostały zweryfikowane**. Referencyjne SQL w `tools/supabase/wirr_data_api_grants.sql` **nie jest migracją automatyczną** i nie powinno być uruchamiane na produkcji bez przeglądu polityk RLS oraz własności obiektów.

| Obiekt | Wywołujący | Wymagane GRANT | Dowód |
|---|---|---|---|
| `public.courses` | student / prowadzący | `authenticated: SELECT` | `Editor/WiRRKiaReports.cs`, `tools/grading/kia-dashboard.html` |
| `public.enrollments` | student | `authenticated: SELECT` | `Editor/WiRRKiaReports.cs` |
| `public.wirr_submissions` | student | `authenticated: SELECT, INSERT` | `Editor/WiRRKiaReports.cs` |
| `public.wirr_grades` | prowadzący | `authenticated: SELECT, INSERT, UPDATE` | `tools/grading/kia-dashboard.html` (upsert z return=representation) |
| `public.wirr_instructor_overview` | prowadzący | `authenticated: SELECT` | `tools/grading/kia-dashboard.html` |

Granty dotyczą tylko ról na poziomie obiektu. Właściwe polityki RLS muszą ograniczać zapisy studentów do ich własnych raportów, dostęp do ocen do właściwych studentów/prowadzących, a prowadzącym udostępniać dane tylko własnego kursu. Widok `wirr_instructor_overview` trzeba sprawdzić pod kątem `security_invoker=true` (PostgreSQL 15+) i uprawnień do tabel bazowych; zwykłe widoki mogą omijać RLS przez uprawnienia właściciela. `GRANT` dla widoku nie zastępuje tej kontroli.

Funkcja `/functions/v1/wirr-public-intake` nie podlega bezpośrednio zmianie przyznawania uprawnień PostgREST. Należy jednak sprawdzić, czy funkcja używa `anon`, `authenticated` czy klucza serwisowego do dostępu do tabel. Nie przyznawać `anon` praw do raportów na podstawie samego adresu funkcji.

## Procedura integracji

1. W repozytorium będącym właścicielem migracji znajdź wszystkie `CREATE TABLE`, `CREATE VIEW`, `DROP ... CREATE` oraz przywracanie schematu `public`.
2. Dopisz do tej samej migracji jawne uprawnienia **wyłącznie** dla ról faktycznie korzystających z tabel. Weryfikuj polityki RLS, w tym `WITH CHECK` dla `INSERT` i `UPDATE`.
3. Przejrzyj konfigurację exposed schemas Data API; `GRANT` nie wystarcza, gdy schemat nie jest wystawiony.
4. W izolowanym PostgreSQL 17 + lokalnym stosie Supabase odtwórz bazę od zera, uruchom migracje i sprawdź `has_table_privilege('authenticated', ...)` oraz negatywne przypadki dostępu `anon`.
5. Przetestuj role i JWT kont studenta oraz prowadzącego przez PostgREST, łącznie z upsert i próbami dostępu do cudzych wierszy. Nie testuj na danych produkcyjnych.
6. Dopiero po tych kontrolach przenieś odpowiednie GRANT do migracji źródłowych. Nie nadawaj automatycznie `ALL` ani `PUBLIC`.

Test statyczny: `python -m unittest discover -s tests/supabase -v`. Test kontroluje dokładną macierz przywilejów, transakcję, niedozwolone instrukcje i mutacje symulujące regresje. Nie wymaga połączenia z bazą. Brak modyfikacji infrastruktury i danych produkcyjnych.

**Granice testu:** nie wykonuje on SQL ani nie dowodzi poprawności RLS, ekspozycji Data API, polityk widoków czy zachowania PostgREST. W szczególności przyznanie `authenticated` uprawnień do tabeli ocen jest bezpieczne tylko wtedy, gdy odrębne polityki RLS zabraniają studentom modyfikowania ocen. Przed przeniesieniem SQL do migracji wymagana jest izolowana próba odtworzenia bazy oraz test pozytywny i negatywny z kontami rzeczywistych ról.

**Uwaga o migracjach:** przy odtwarzaniu obiektów `GRANT` należy umieścić *po* odpowiednim `CREATE TABLE`/`CREATE VIEW` w migracjach należących do właściciela schematu, zamiast uruchamiać ten plik na istniejącej produkcji. `service_role` jest świadomie pominięty: sposób dostępu funkcji Edge do bazy nie został jeszcze zweryfikowany.

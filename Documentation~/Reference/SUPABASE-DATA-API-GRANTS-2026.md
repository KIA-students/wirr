# WiRR: jawne uprawnienia Data API od 30.10.2026

Źródło: powiadomienie Supabase z 20.10.2026, [dyskusja Supabase](https://github.com/orgs/supabase/discussions/45329).

## Zakres audytu `KIA-students/wirr`

Na `main` nie ma plików `.sql` ani `supabase/migrations`. Historia migracji została odczytana przez Supabase MCP 20.10.2026 z projektu KIA_tests, bez zmian w bazie. Plików źródłowych migracji nadal nie ma w tym repozytorium. Referencyjne SQL w `tools/supabase/wirr_data_api_grants.sql` **nie jest migracją automatyczną** i nie powinno być uruchamiane na produkcji bez przeglądu polityk RLS oraz własności obiektów.

| Obiekt | Wywołujący | Wymagane GRANT | Dowód |
|---|---|---|---|
| `public.courses` | student / prowadzący | `authenticated: SELECT` | `Editor/WiRRKiaReports.cs`, `tools/grading/kia-dashboard.html` |
| `public.enrollments` | student | `authenticated: SELECT` | `Editor/WiRRKiaReports.cs` |
| `public.wirr_submissions` | student | `authenticated: SELECT, INSERT` | `Editor/WiRRKiaReports.cs` |
| `public.wirr_grades` | prowadzący | `authenticated: SELECT, INSERT; UPDATE (grade, graded_by, graded_at)` | `tools/grading/kia-dashboard.html` (upsert z return=representation) |
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

**Uwaga o migracjach:** przy odtwarzaniu obiektów `GRANT` należy umieścić *po* odpowiednim `CREATE TABLE`/`CREATE VIEW` w migracjach należących do właściciela schematu, zamiast uruchamiać ten plik na istniejącej produkcji. `service_role` otrzymuje SELECT i INSERT wyłącznie do kolejki `wirr_public_intake`, zgodnie z odczytanym kodem funkcji Edge v5. Uprawnienia eksportera wymagają osobnego sprawdzenia.

## Wynik odczytu Supabase, 20.10.2026

- `20261012212457 wirr_course_submissions_and_grades_v1` zawiera już jawne SELECT/INSERT dla raportów oraz granty ocen i polityki RLS.
- `20261012212519 wirr_grades_limit_update_columns` celowo cofa tabelowy UPDATE i dopuszcza tylko `grade`, `graded_by`, `graded_at`. Nie wolno przywracać pełnego UPDATE w poprawce grantów.
- `20261012212917 wirr_instructor_overview_v1` zawiera SELECT dla authenticated i `security_invoker=true`. Bieżący widok zachowuje to ustawienie.
- `20261013122419 create_wirr_public_intake_queue` włącza RLS, odbiera dostęp anon/authenticated, ale nie nadaje jawnych praw service_role. To potwierdzona zależność od dawnych domyślnych grantów przy odtwarzaniu bazy.
- Funkcja Edge `wirr-public-intake` v5 używa klucza serwisowego: SELECT do licznika i potwierdzenia, INSERT z `ignoreDuplicates:true`. Ten wariant upsert nie aktualizuje istniejącego wiersza. Minimalny grant to SELECT, INSERT; nie wymaga sekwencji, identyfikator jest UUID.
- Wszystkie pięć tabel ma RLS. Kolejka nie ma polityk dla klientów. Raporty ograniczają INSERT do aktywnego zapisu i własnego student_id, oceny do właściciela kursu. Nie wykonano testów JWT ani prób zapisu.
- `courses` i `enrollments` mają obecnie szerokie stare granty także dla anon. Nie cofamy ich w poprawce WiRR, ponieważ są współdzielone z KIA. Referencyjna macierz jest minimum klienta WiRR, a nie kompletną polityką uprawnień KIA.

### Miejsce integracji

W migracji tworzącej kolejkę dopisać po CREATE TABLE:

```sql
GRANT USAGE ON SCHEMA public TO service_role;
GRANT SELECT, INSERT ON TABLE public.wirr_public_intake TO service_role;
```

Właściciel migracji wspólnych powinien dodać jawny SELECT na `courses` i `enrollments` dla authenticated do ścieżki odtworzenia. Pozostałych grantów WiRR nie trzeba rozszerzać. SQL referencyjny nie cofa zastanych praw i nie jest skryptem utwardzającym istniejącą bazę.

### Ograniczenie panelu ocen

Panel wysyła upsert ocen wraz z kolumnami klucza `course_id`, `student_id`, `lab_no`. Ścieżka konfliktu może wymagać UPDATE także tych kolumn i kolidować z celowym ograniczeniem kolumnowym. Nie rozszerzać grantu dla obejścia błędu. Osobna poprawka powinna rozdzielić INSERT od PATCH aktualizującego wyłącznie trzy dozwolone kolumny, z obsługą konfliktu równoległych zapisów. Wymaga testu przez lokalny PostgREST.

Weryfikacja offline: 8/8 testów statycznych (w tym regresja granic transakcji SQL). Nie jest to test odtworzenia schematu ani integracji PostgREST. Nie wykonano migracji, wdrożenia funkcji ani zapisów do produkcji.

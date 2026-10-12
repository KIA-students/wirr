# WiRR: ewidencja ocen i raportowanie

Wersja 1.0.1

## Zasada

Student konfiguruje indeks i grupę jednokrotnie. Wypełnia pomiary w Unity; nie ma obowiązku zakładania forka, instalacji GitHub CLI ani tworzenia PR. Do czasu wdrożenia zweryfikowanego transportu raport pozostaje eksportowany lokalnie, a przekazanie prowadzącemu wymaga potwierdzenia odbioru. Nie nazywać eksportu oddaniem.

## Ewidencja prowadzącego

Listę oczekiwanych studentów importuje się z prywatnego rosteru, **nie z napływających zgłoszeń**. Dla każdej pary student–laboratorium przechowuje się stan `missing`, `received`, `needs_review` lub `approved`; ocena 3.0–5.0 występuje tylko w stanie `approved`. Prowadzący musi móc znaleźć brakujące zgłoszenia, oczekujące weryfikacje i wyeksportować CSV. Brak rekordu nie oznacza oceny niedostatecznej. Ponowny import nie może dublować wyników. Sprzeczne rekordy wymagają rozstrzygnięcia, a nie automatycznego nadpisania.

Lokalny audyt: `python tools/grading/grade_audit.py --roster roster.csv --records records.csv --output grades.csv`. Nagłówki rosteru: `student_id,group`. Nagłówki ewidencji: `student_id,lab,status,grade`; `lab01` do `lab08`. CSV zawiera wiersz dla każdego studenta, nawet bez żadnego zgłoszenia. Pliki rosteru, surowe raporty i wyniki **pozostają poza publicznym repozytorium**. Uruchom testy: `python -m unittest discover -s tools/grading -p 'test_*.py'`.

## Docelowy transport

Projekt `KIA_tests` (`ndebunrsdtulflzpemwd`) zawiera wydzielone tabele `public.wirr_submissions` i `public.wirr_grades`. Korzystają one z istniejących kont, kursów i zapisów na zajęcia. Uwierzytelnienie odbywa się przez Supabase Auth; student musi być aktywnie zapisany do kursu i może dodawać oraz czytać tylko własne zgłoszenia. Prowadzący kursu może odczytywać zgłoszenia i zatwierdzać oceny. Zmiany schematu wdrożono; integracja klienta Unity i panelu ewidencji jest nadal do wykonania. Publiczny klient Unity nigdy nie przechowuje klucza `service_role`. Sam numer indeksu nie może stanowić uwierzytelnienia ani uprawniać do odczytu cudzych rekordów. Przesyłanie powinno być idempotentne (identyfikator zgłoszenia), lokalnie kolejkowane offline oraz kończyć się serwerowym potwierdzeniem. Ocena i zatwierdzenie po stronie prowadzącego. Bez testów end-to-end nie wolno deklarować uruchomienia automatycznej wysyłki.

## Implementacja w istniejącej bazie

Zgłoszenie do `wirr_submissions` zawiera UUID klienta `client_submission_id`, `course_id`, `student_id` równe ID uwierzytelnionego użytkownika, `lab_no` 1–8 i obiekt JSON `report`. Unikalne `client_submission_id` zapobiega cichym duplikatom. Tabela `wirr_grades` zawiera pojedynczą ocenę zatwierdzoną dla pary kurs–student–laboratorium; nie ma otwartych praw zapisu oceny przez studenta. Listę braków wyznaczają `enrollments` i brak zgłoszenia/oceny. Klucze prywatne nigdy nie trafiają do Unity.

Nie publikuj numerów indeksów, raportów i eksportów ocen w repozytorium. Warstwa Unity ma używać tokenu uwierzytelnionego użytkownika z KIA, a nie anonimowego żądania. Dotychczasowy eksport JSON pozostaje ścieżką awaryjną do chwili pozytywnego testu całości.

## Kryteria odbioru

1. Student wykonuje co najwyżej jednorazowe skonfigurowanie profilu i jedno wysłanie raportu, bez Git/gh/PR.
2. Przy awarii sieci raport nie ginie, a status wysyłki jest widoczny.
3. Prowadzący widzi wszystkich z rosteru, również tych bez raportu.
4. Żaden student nie może zatwierdzić sobie oceny ani odczytać cudzych wyników.
5. Import duplikatu jest bezskutkowy; konflikt nie jest automatycznie rozstrzygany.
6. Dostępny jest eksport CSV i ślad czasu oraz autora zatwierdzenia.

# WiRR: synchronizacja raportów i ocen z KIA

Wersja 1.0.1

W projekcie Supabase KIA_tests uruchomiono kurs WIRR, tabele `wirr_submissions`, `wirr_grades` oraz widok `wirr_instructor_overview`. Dostęp kontrolują reguły RLS oraz konta i zapisy z KIA.

Student: w Unity wybierz **WiRR → Student → Wyślij raport do KIA**, zaloguj się na dotychczasowe konto KIA i użyj przycisku **Zapisz i przekaż raport do KIA** w standardowym formularzu WiRR; wskazywanie pliku nie jest wymagane. W przypadku braku sieci raport pozostaje w kolejce `UserSettings/WiRRKiaQueue` i można ponowić wysyłkę. Student nie potrzebuje GitHub CLI, forka ani PR. Po zalogowaniu do KIA ponawianie zgłoszeń jest automatyczne podczas otwartej sesji Unity. Po zamknięciu Unity kolejka pozostaje na dysku.

Prowadzący: otwórz lokalnie `tools/grading/kia-dashboard.html`, zaloguj się na konto właściciela kursu i zobacz listę zapisanych studentów, braków, raportów oczekujących na ocenę oraz ocen zatwierdzonych. Ocena jest zapisywana przyciskiem, CSV można pobrać lokalnie. Brak raportu nie jest oceną niedostateczną.

## Warunki włączenia do produkcji

- Import prawdziwej listy studentów do `enrollments`, z przypisaniem kont `students` do kursu WiRR. Bez tego nie da się znaleźć wszystkich studentów z brakującymi ocenami.
- Zbudowanie i uruchomienie kodu na Unity 6000.6.5f1, test logowania konta studenta, zgłoszenia, idempotentnej ponownej wysyłki, widoku prowadzącego i zatwierdzenia oceny.
- Formularz jest połączony z klientem przesyłania; numer laboratorium pochodzi z formularza. Nadal wymagany test end-to-end na kontach KIA.
- Potwierdzenie, że połączenie z KIA działa dla istniejących kont studentów. Token przechowywany jest wyłącznie w pamięci edytora i wygasa zgodnie z polityką Supabase Auth; po wygaśnięciu należy zalogować się ponownie.
- Przed publikacją trzeba sprawdzić, czy lokalna kolejka zawierająca raporty jest odpowiednio chroniona na współdzielonych stanowiskach komputerowych.

Nie uznawać wdrożenia za w pełni autonomiczne przed przejściem powyższych testów. Aktualna implementacja nie modyfikuje istniejącego systemu wejściówek.
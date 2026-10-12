# WiRR: synchronizacja raportów i ocen z KIA

Wersja 1.0.0

W projekcie Supabase KIA_tests uruchomiono kurs WIRR, tabele `wirr_submissions`, `wirr_grades` oraz widok `wirr_instructor_overview`. Dostęp kontrolują reguły RLS oraz konta i zapisy z KIA.

Student: w Unity wybierz **WiRR → Student → Wyślij raport do KIA**, zaloguj się na dotychczasowe konto KIA i wybierz plik JSON raportu nazwany z numerem laboratorium, np. `Lab02-report.json`. W przypadku braku sieci raport pozostaje w kolejce `UserSettings/WiRRKiaQueue` i można ponowić wysyłkę. Student nie potrzebuje GitHub CLI, forka ani PR.

Prowadzący: otwórz lokalnie `tools/grading/kia-dashboard.html`, zaloguj się na konto właściciela kursu i zobacz listę zapisanych studentów, braków, raportów oczekujących na ocenę oraz ocen zatwierdzonych. Ocena jest zapisywana przyciskiem, CSV można pobrać lokalnie. Brak raportu nie jest oceną niedostateczną.

## Warunki włączenia do produkcji

- Import prawdziwej listy studentów do `enrollments`, z przypisaniem kont `students` do kursu WiRR. Bez tego nie da się znaleźć wszystkich studentów z brakującymi ocenami.
- Zbudowanie i uruchomienie kodu na Unity 6000.6.x, test logowania konta studenta, zgłoszenia, idempotentnej ponownej wysyłki, widoku prowadzącego i zatwierdzenia oceny.
- Weryfikacja, czy format JSON aktualnego formularza może zostać automatycznie podłączony do przycisku wysyłki. Na razie student wybiera wyeksportowany plik, a laboratorium rozpoznawane jest po nazwie.
- Potwierdzenie, że połączenie z KIA działa dla istniejących kont studentów. Token przechowywany jest wyłącznie w pamięci edytora i wygasa zgodnie z polityką Supabase Auth; po wygaśnięciu należy zalogować się ponownie.
- Przed publikacją trzeba sprawdzić, czy lokalna kolejka zawierająca raporty jest odpowiednio chroniona na współdzielonych stanowiskach komputerowych.

Nie uznawać wdrożenia za w pełni autonomiczne przed przejściem powyższych testów. Aktualna implementacja nie modyfikuje istniejącego systemu wejściówek.
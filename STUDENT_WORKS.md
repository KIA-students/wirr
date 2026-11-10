# WiRR: wyniki, prywatność i repozytoria

- **Produkcja studencka:** [`KIA-students/wirr`](https://github.com/KIA-students/wirr).
- We wszystkich aktualnych instrukcjach dla studentów używaj linków do **produkcyjnego** `KIA-students/wirr`, nigdy development `MatPomGit/wirr`. Pakiet Unity instaluj z zatwierdzonej gałęzi/tagu produkcyjnego.
- Docelowy katalog zgłoszeń to `student-work/` w `KIA-students/wirr` (pisownia dokładna). Raport z Unity powinien przejść przez walidowaną kolejkę i idempotentny PR do tego katalogu; samo HTTP 202 oznacza przyjęcie do kolejki, nie publikację ani ocenę.
- Prace odebrane pocztą e-mail muszą przejść tę samą walidację i zostać przeniesione do `student-work/` jako kontrolowane zgłoszenie. Nie commituj surowego e-maila, metadanych nadawcy ani załączników z danymi osobowymi. Zachowaj potwierdzenie źródła w prywatnym rejestrze.
- Kopia tabeli ocen w produkcyjnym repozytorium może zawierać **wyłącznie numer indeksu i wynik**. Zabronione są imię, nazwisko, e-mail, identyfikatory prywatne i komentarze identyfikujące.
- Zachowaj `student-work/` jako ścieżkę kanoniczną; sprawdzaj idempotencję i SHA kolejki.
- Walidację Unity wykonuj bez interakcji: Editor `-batchmode -nographics -quit -projectPath ... -runTests -testPlatform EditMode -testResults ...` z właściwą wersją Unity, osobne PlayMode i testy sprzętu gdy dostępne.

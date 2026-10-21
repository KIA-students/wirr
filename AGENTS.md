# AGENTS.md

## GitHub Actions: polityka agentów AI (manual-first, main-only)

- Nie uruchamiaj Actions na gałęziach roboczych ani w odpowiedzi na pull request. Domyślnie korzystaj z `workflow_dispatch` **wyłącznie z `main`**; kosztowne testy WebSim/ROS, release i walidacje raportów są ręczne.
- Wyjątki operacyjne automatyczne ograniczono do `push` na `main`: publikacja WebSim Pages, student UPM, synchronizacja do `KIA-students/wirr`, sumowanie zatwierdzonych punktów. Eksport raportów ze sprawdzonej kolejki działa także z harmonogramu (GitHub `schedule` zawsze wykonuje domyślną gałąź `main`). NIE wyłączaj ich bez analizy procesu oceniania.
- Kontrolę zgłoszeń `students/progress/**` uruchamiaj ręcznie na main z podaniem dwóch SHA (baza i zgłoszenie), bez odpalania Actions na PR. Walidator raportów na main używa trybu manualnego.
- Każdy job musi wymagać `github.ref == 'refs/heads/main'`. Nie dodawaj automatycznych `pull_request_target`, `workflow_run`, `repository_dispatch`, tagów ani jobów na innych gałęziach. Nowe automaty wymagają uzasadnienia, wąskiego `paths` i kontroli zużycia minut.
- Zawsze wybieraj `main` dla Run workflow; starsze workflow na innych gałęziach mogą nie mieć tej blokady. Testy i ocenianie raportuj na podstawie dowodów, bez przypisywania automatycznie ocen przy niezweryfikowanych danych.

## Status produkcyjny WiRR

To repozytorium jest **produkcyjnym, studenckim** punktem dystrybucji WiRR. `MatPomGit/wirr` to repozytorium **rozwojowe** Unity, UPM, Samples, walidatorów i narzędzi. `MatPomGit/prz` przechowuje treści dydaktyczne prowadzącego, rubryki i prywatne dane, a `MatPomGit/research-autopilot` prowadzi orkiestrację, nie jest źródłem pakietów.

- Przyjmuj tylko zatwierdzone wydania z potwierdzonym source SHA, testami i przeglądem zawartości; nie synchronizuj bezpośrednio roboczego HEAD `MatPomGit/wirr`.
- Nie rozwijaj tutaj nowej implementacji Unity. Błędy, regresje i propozycje kodu zgłaszaj w `MatPomGit/wirr`, a wydanie publikuj po weryfikacji.
- Zabronione: prywatne oceny, identyfikujące raporty, SSQ, rubryki prowadzącego, sekrety i niezatwierdzone zadania egzaminacyjne.
- Wynik automatycznego joba bez artefaktu i dowodu testowego nie jest zatwierdzeniem produkcyjnym.

## Prace i wyniki studentów

Stosuj [STUDENT_WORKS.md](STUDENT_WORKS.md): katalog docelowy `student-work/`, tylko indeksy w publikowanej tabeli ocen; pełne imiona i nazwiska wyłącznie w prywatnym `MatPomGit/prz/students/`. Obowiązującym katalogiem jest `student-work/`.

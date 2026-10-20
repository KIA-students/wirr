# AGENTS.md

## GitHub Actions: polityka agentów AI (manual-first, main-only)

- Nie uruchamiaj Actions na gałęziach roboczych ani w odpowiedzi na pull request. Domyślnie korzystaj z `workflow_dispatch` **wyłącznie z `main`**; kosztowne testy WebSim/ROS, release i walidacje raportów są ręczne.
- Wyjątki operacyjne automatyczne ograniczono do `push` na `main`: publikacja WebSim Pages, student UPM, synchronizacja do `KIA-students/wirr`, sumowanie zatwierdzonych punktów. Eksport raportów ze sprawdzonej kolejki działa także z harmonogramu (GitHub `schedule` zawsze wykonuje domyślną gałąź `main`). NIE wyłączaj ich bez analizy procesu oceniania.
- Kontrolę zgłoszeń `students/progress/**` uruchamiaj ręcznie na main z podaniem dwóch SHA (baza i zgłoszenie), bez odpalania Actions na PR. Walidator raportów na main używa trybu manualnego.
- Każdy job musi wymagać `github.ref == 'refs/heads/main'`. Nie dodawaj automatycznych `pull_request_target`, `workflow_run`, `repository_dispatch`, tagów ani jobów na innych gałęziach. Nowe automaty wymagają uzasadnienia, wąskiego `paths` i kontroli zużycia minut.
- Zawsze wybieraj `main` dla Run workflow; starsze workflow na innych gałęziach mogą nie mieć tej blokady. Testy i ocenianie raportuj na podstawie dowodów, bez przypisywania automatycznie ocen przy niezweryfikowanych danych.

## Integracja z Research Autopilot

- Traktuj `MatPomGit/wirr` jako repozytorium źródłowe zarządzane przez `MatPomGit/research-autopilot`.
- Zachowaj issues, PR, testy, skrypty i dokumentację w formatach dostępnych przez GitHub API i narzędzia CLI. Nie uzależniaj procesu wyłącznie od interfejsu Unity.
- Repozytorium `KIA-students/wirr` jest dystrybucją dla studentów; używaj kontrolowanej synchronizacji z repozytorium źródłowego.
- Dane studentów i indywidualne oceny pozostają wyłącznie w prywatnej ewidencji PRz. Nie kopiuj ich do publicznych issue, logów ani rankingu.
- Autopilot może weryfikować schematy, pomiary i CI oraz przygotowywać PR. Zatwierdzanie ocen, punktacji i rozstrzyganie podejrzeń wymaga decyzji prowadzącego.
- Do odbioru scen wymagaj rzeczywistego wyniku Unity 6000.6.5f1. Kontrola statyczna nie jest równoważna uruchomieniu Edytora.
- Kontrakt nadrzędny procesu: `MatPomGit/research-autopilot/docs/integrations/WIRR_AUTOPILOT.md`.

# Walidacja wspólnej wersji WiRR

Wersja referencyjna: Unity **6000.6.5f1**. Harness tworzy osobny projekt UPM poza pakietem, aktywuje jego test assemblies i uruchamia EditMode oraz PlayMode. Nie zmienia projektu używanego przez studentów. Importuje zapisywalną kopię wyłącznie Editor, Runtime, Tests, Samples i lekkich tekstur, bez narzędzi deweloperskich i wcześniejszych artefaktów. Unity nie dopisuje metadanych do checkoutu źródłowego.

## Windows: zalecana ścieżka na Andromedzie

Uruchom polecenia z katalogu aktualnego checkoutu repozytorium. Do parametru `-Editor` podaj istniejący Unity.exe wersji 6000.6.5f1, jeżeli nie jest znaleziony w standardowej lokalizacji Unity Hub ani w `D:\Unity\UnityEditors`.

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\tools\unity-ci\run-windows.ps1 -PreflightOnly
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\tools\unity-ci\run-windows.ps1
```

Preflight nie wykonuje testów. Pełny przebieg tworzy `artifacts/unity-native/summary.json`, XML i logi obu zestawów. Brak XML, testów, wymaganych przypadków, status inny niż Passed albo choć jeden pominięty test oznaczają błąd. Zapisany SHA musi odpowiadać wersji ocenianej w PR. Projekty robocze powstają w `D:\Unity\cli`; błędne przebiegi pozostawiają projekt do diagnostyki, a poprawne usuwają go, chyba że podano `-KeepProject`.

## Linux lub Docker Desktop: ścieżka opcjonalna

Potrzebny jest lokalnie dostępny obraz Linux amd64 z Unity 6000.6.5f1 oraz legalna aktywacja właściwa dla kontenera. Nie zakładamy dostępności konkretnego tagu GameCI i nie pobieramy obrazu automatycznie.

```bash
export UNITY_IMAGE='LOKALNY_OBRAZ_UNITY_6000.6.5f1'
export UNITY_LICENSE_FILE='/sciezka/do/Unity_lic.ulf'
bash tools/unity-ci/run-docker.sh
```

Na Windows z Docker Desktop użyj `run-andromeda.ps1 -Image LOKALNY_OBRAZ -LicenseFile SCIEZKA`. Weryfikacja Docker Desktop bez uruchamiania Unity: `run-andromeda.ps1 -PreflightOnly`. Licencji nie zapisuj w repozytorium ani artefaktach.

Workflow Docker jest ręczny, działa wyłącznie z `main` na self-hosted Linux X64 i wymaga jawnego obrazu oraz ścieżki licencji. Do testowania gałęzi PR użyj lokalnego skryptu. Kontener Linux nie potwierdza zachowania Windows, obrazu GPU, gogli ani telefonu.

## Warunki odbioru

Po poprawnym EditMode i PlayMode wykonaj osobno smoke test czystego i istniejącego projektu na Windows/Ubuntu oraz próbę wysyłki przez istniejący Supabase, ponowienie po braku sieci i sprawdzenie potwierdzenia. Nie traktuj testów parsera lub obecności scen jako dowodu tych operacji. W bieżącej sesji wykonano wyłącznie testy offline, składnię i kontrole strukturalne; nie uruchomiono Unity ani sieciowego E2E.

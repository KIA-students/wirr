# Laboratorium 7 — Walidacja i Testy

## Spis treści

- [Najpierw wybierz ścieżkę](#najpierw-wybierz-ścieżkę)
- [Co powinieneś pobrać](#co-powinieneś-pobrać)
  - [Nowe pakiety (opcjonalnie)](#nowe-pakiety-opcjonalnie)
  - [Pliki](#pliki)
- [Checklist](#checklist)
- [Start Lab 7](#start-lab-7)
- [Checklista Smoke Testsów](#checklista-smoke-testsów)
- [Pomiar wydajności (Profiler)](#pomiar-wydajności-profiler)
- [Raport `evidence/lab07.md`](#raport-evidencelab07md)
- [Identyfikacja pary](#identyfikacja-pary)
- [Smoke Tests](#smoke-tests)
- [Matryca Funkcjonalna](#matryca-funkcjonalna)
- [Wydajność](#wydajność)
- [Błąd Systemowy](#błąd-systemowy)
- [Zaliczenie](#zaliczenie)
- [Dodatkowe, dobrowolne doświadczenie: komfort VR i głębia](#dodatkowe-dobrowolne-doświadczenie-komfort-vr-i-głębia)

## Najpierw wybierz ścieżkę
- **Mam działającą scenę z poprzedniego laboratorium:** [otwórz aktualne Lab 07](../../Samples~/Lab07/README.md#szybki-wybór-ścieżki) i pomiń powtórną instalację.
- **Nie mam Unity / konta:** [system operacyjny](OS_SETUP.md#wybierz-aktualną-sytuację) lub [licencja](UNITY_ACCESS_AND_LICENSES.md#nawigacja-warunkowa).
- **Windows:** [uruchomienie Windows](OS_SETUP.md#windows); **Ubuntu:** [uruchomienie Ubuntu](OS_SETUP.md#ubuntu).
- **Potrzebuję więcej szczegółów:** [pełny przewodnik Lab 07](../../Samples~/Lab07/README.md) zawiera spis treści i ścieżki wariantowe.
- **Nie mam sprzętu XR:** rozpocznij od symulatora lub dostarczonego logu; nie instaluj wszystkiego w ciemno.


## Co powinieneś pobrać

- ✓ Projekt z Lab 1–6 (wszystkie merged na main)
- ✓ Profiler Unity, Memory Profiler

### Nowe pakiety (opcjonalnie)
```
Unity Test Framework          1.3.0+ (dla testów automatycznych)
```

### Pliki
```
Assets/Tests/
├── Lab01_Tests.cs (jednostkowe)
├── Lab02_Tests.cs
└── ...
Assets/Scripts/PerformanceBenchmark.cs (z Lab 1, adapt.)
evidence/lab07.md (template)
```

---

## Checklist

- [ ] Wszystkie Lab 1–6 zaliczone
- [ ] Wszystkie gałęzie zmergowane na main
- [ ] Console: brak RED errors
- [ ] Profiler otwarta
- [ ] Memory Profiler zainstalowany

---

## Start Lab 7

1. Git: `git switch main && git merge --no-ff team-<nr>/lab06-<...>`
2. Git: `git switch -c team-<nr>/lab07-<nazwiska>`
3. Uruchom Play Mode: Smoke Test
4. Profiler: pomiar wydajności
5. Przeprowadź scenariusze użytkownika
6. Wpisz wyniki do raportu `evidence/lab07.md`

---

## Checklista Smoke Testsów

```
[ ] Aplikacja startuje bez crash
[ ] Konsola nie ma RED errors
[ ] XR Origin inicjalizuje
[ ] Kamera główna odpowiada
[ ] Jeśli Quest/AR: sensory reagują
[ ] Jeśli ROS 2: węzeł widoczny
[ ] FPS nie pada poniżej 20
```

---

## Pomiar wydajności (Profiler)

```
Wariant | FPS | CPU [ms] | GPU [ms] | Memory [MB]
--------|-----|---------|---------|-------------
Idle    | ___ | ___     | ___     | ___
Lab 1   | ___ | ___     | ___     | ___
Lab 2   | ___ | ___     | ___     | ___
Lab 3   | ___ | ___     | ___     | ___
Lab 4   | ___ | ___     | ___     | ___
Lab 5   | ___ | ___     | ___     | ___
Lab 6   | ___ | ___     | ___     | ___
Stress  | ___ | ___     | ___     | ___
```

---

## Raport `evidence/lab07.md`

```markdown
# Lab 7 — Raport Finalny

## Identyfikacja pary
- Student 1: ___ (index ___) 
- Student 2: ___ (index ___)

## Smoke Tests
- [x] Start bez errors
- [x] FPS zmierzony
- [ ] ...

## Matryca Funkcjonalna
| Lab | Status | Uwagi |
|-----|--------|-------|
| 1   | PASS   | ... |
| ... | ...    | ... |

## Wydajność
- FPS: ...
- Latency: ...
- Memory: ...

## Błąd Systemowy
- Wariant: A/B
- Objaw: ...
- Przyczyna: ...

## Zaliczenie
- [x] Checkpoint 3.0: PASS
- Ocena finalna: 3.0 / 3.5 / 4.0 / 4.5 / 5.0
```


## Dodatkowe, dobrowolne doświadczenie: komfort VR i głębia

Wybierz **WiRR → Komfort XR → Utwórz 2 prefaby i stanowiska obserwacyjne**. Powstaną prawdziwe prefaby Unity z łagodnym ruchem wskazówek wizualnych (maks. 8 s) i krótkim porównaniem wirtualnej głębi (maks. 10 s). Ich zadaniem nie jest wywoływanie nudności, lecz nauka projektowania komfortowych scen.

Instrukcja pełna: **Documentation~/Reference/COMFORT_STUDIES.md**. Przy pracy w goglach potrzebny jest świadomy ochotnik, osoba obserwująca i możliwość natychmiastowego przerwania. Reszta grupy może wykonać równoważną analizę na monitorze. **Nie przyznaje się punktów za fizjologiczną odporność ani wystąpienie objawów.**

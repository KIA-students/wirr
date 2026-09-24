# Laboratorium 7 — Walidacja i Testy

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

# Laboratorium 1 — XR Origin, Audyt i Wydajność

## Co Powinieneś Pobrać

Przed Lab 1 pobierz i skonfiguruj następujące:

### 1. Wersja Unity
```
Unity 2022 LTS (6000.0.23+) — **MUSI BYĆ TAT SAMA** dla całego kursu
```
Pobierz z: https://unity.com/download

### 2. Projekt startowy (starter template)
```
Assets/
├── Scenes/
│   └── Lab01_Baseline.unity (pusty projekt z XR Origin)
├── Scripts/
│   ├── PerformanceBenchmark.cs (do wklejenia w Lab)
│   └── TODO_Student.cs (placeholder)
└── ProjectSettings/
    └── ProjectVersion.txt
```

### 3. Pakiety wymagane

Zainstaluj w Project → Window → Package Manager:

```
XR Interaction Toolkit        3.1.3+
XR Plugin Management          4.4.1+
XR Hands                       1.4.1+
Input System                   1.8.3+
```

**WAŻNE**: Nie aktualizuj pakietów między Lab 1 a Lab 7!

### 4. Konfiguracja XR Origin (pre-configured)

W scenie `Lab01_Baseline.unity` jest już:
- ✓ XR Origin (position 0,0,0 | scale 1,1,1)
- ✓ XR Camera Manager
- ✓ XR Interaction Manager
- ✓ Input Action Manager (z akcjami)

**Nie modyfikuj XR Origin na poszczególnych lab!**

---

## Checklist — Przed Lab 1

- [ ] Unity 2022 LTS zainstalowany
- [ ] Projekt otwarty, brak RED errory w Console
- [ ] Wszystkie 4 pakiety zainstalowane
- [ ] Scene Lab01_Baseline.unity otwarta
- [ ] Wersja pakietów zapisana w `packages-lock.json`
- [ ] Git: `git clone <ADRES> xr-lab && cd xr-lab`
- [ ] Gałąź: `git switch lab01-start`

---

## Na Początku Lab 1

1. **Utwórz gałąź:**
```bash
git switch -c team-<nr>/lab01-<nazwisko1>-<nazwisko2>
```

2. **Wczytaj kod:**
   - Otwórz `PerformanceBenchmark.cs`
   - Zastąp `int pairIndexSum = 0;` na sumę Twoich numerów indeksów

3. **Uruchom Play Mode** i obserwuj Console — powinien wypisywać FPS co ~300 klatek

4. **Odpowiedź wejściówka** (5 pytań, ~10 min)

---

## Skrypty do pobrania

### PerformanceBenchmark.cs
```csharp
using UnityEngine;

public class PerformanceBenchmark : MonoBehaviour {
    int pairIndexSum = 0; // WPISZ: suma numerów indeksów
    float frameSum = 0;
    int frameCount = 0;
    
    void Update() {
        frameSum += Time.deltaTime;
        frameCount++;
        
        if (frameCount == 300) {
            float fps = frameCount / frameSum;
            float ms = (frameSum / frameCount) * 1000f;
            Debug.Log($"LAB01_BENCHMARK fps={fps:F1} ms={ms:F2} seed={pairIndexSum}");
            frameCount = 0;
            frameSum = 0;
        }
    }
}
```

---

## Dokumentacja

- Unity XR Plugin Management: https://docs.unity3d.com/Manual/xr-plug-in-management.html
- XR Interaction Toolkit: https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@latest/

---

## Timeline Lab 1

- Smoke check: 5 min
- Wejściówka: 10 min
- Benchmark baseline: 15 min
- Audyt urządzeń: 30 min
- Diagnoza: 20 min
- Oddanie: 10 min

**Razem: ~100 minut**

---

**Wersja: 2.0 • 1 listopada 2026 r.**

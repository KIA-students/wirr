# Laboratorium 1 — XR Origin, Audyt i Wydajność

## Spis treści

- [Najpierw wybierz ścieżkę](#najpierw-wybierz-ścieżkę)
- [Konto Unity: nie potrzebujesz wspólnego loginu](#konto-unity-nie-potrzebujesz-wspólnego-loginu)
- [Co Powinieneś Pobrać](#co-powinieneś-pobrać)
  - [1. Wersja Unity](#1-wersja-unity)
  - [2. Projekt startowy (starter template)](#2-projekt-startowy-starter-template)
  - [3. Pakiety wymagane](#3-pakiety-wymagane)
  - [4. Konfiguracja XR Origin (pre-configured)](#4-konfiguracja-xr-origin-pre-configured)
- [Checklist — Przed Lab 1](#checklist-przed-lab-1)
- [Na Początku Lab 1](#na-początku-lab-1)
- [Skrypty do pobrania](#skrypty-do-pobrania)
  - [PerformanceBenchmark.cs](#performancebenchmarkcs)
- [Dokumentacja](#dokumentacja)
- [Timeline Lab 1](#timeline-lab-1)


## Najpierw wybierz ścieżkę
- **Mam działającą scenę z poprzedniego laboratorium:** [otwórz aktualne Lab 01](../../Samples~/Lab01/README.md#szybki-wybór-ścieżki) i pomiń powtórną instalację.
- **Nie mam Unity / konta:** [system operacyjny](OS_SETUP.md#wybierz-aktualną-sytuację) lub [licencja](UNITY_ACCESS_AND_LICENSES.md#nawigacja-warunkowa).
- **Windows:** [uruchomienie Windows](OS_SETUP.md#windows); **Ubuntu:** [uruchomienie Ubuntu](OS_SETUP.md#ubuntu).
- **Potrzebuję więcej szczegółów:** [pełny przewodnik Lab 01](../../Samples~/Lab01/README.md) zawiera spis treści i ścieżki wariantowe.
- **Nie mam sprzętu XR:** rozpocznij od symulatora lub dostarczonego logu; nie instaluj wszystkiego w ciemno.


## Konto Unity: nie potrzebujesz wspólnego loginu

Na komputerze uczelnianym z **aktywną Unity Education Grant License** student może pracować bez indywidualnego logowania. Stanowisko musi wcześniej skonfigurować administrator. **Nie udostępniamy loginu i hasła wspólnego konta Unity.** Na własnym komputerze użyj swojego Unity ID i bezpłatnej licencji Personal; darmowy Student Plan jest rozszerzeniem opcjonalnym.

Przewodnik z krokami i korzyściami dla studentów: [Lab 01: Unity ID i licencje](../../Documentation~/Reference/UNITY_ACCESS_AND_LICENSES.md). Jeżeli pracujesz z próbką zaimportowaną do Assets, skorzystaj z dokumentu w pakiecie WiRR.


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
- Pomiar wariantu bazowego
- Audyt urządzeń: 30 min
- Diagnoza: 20 min
- Oddanie: 10 min

**Razem: ~100 minut**

---

**Wersja: 2.0 • 1 listopada 2026 r.**

# Laboratorium 2 — Interakcja i Lokomocja VR

## Co Powinieneś Pobrać

### Warunki wstępne
- ✓ Zaliczone Lab 1 albo działająca scena z XR Origin
- ✓ Wersja Unity, pakiety, oraz projekt z Lab 1

### Nowe pliki do pobrania

```
Assets/
├── Scenes/
│   └── Lab02_Interaction.unity (kopia Lab01_Baseline)
├── Scripts/
│   ├── Lab02Trial.cs (rejestrator czasów)
│   └── XRISetup.cs (helper)
└── Prefabs/
    └── Module_A.prefab (small object do chwytania)
```

### Pakiety — bez zmian
```
Wszystkie pakiety z Lab 1 (XRI 3.1.3+, Input System 1.8.3+, itp.)
```

---

## Checklist — Przed Lab 2

- [ ] Lab 1 zaliczony
- [ ] Scene Lab02_Interaction.unity otwarta
- [ ] Prefab Module_A widoczny w Project
- [ ] Skrypt Lab02Trial.cs przygotowany
- [ ] Git branch: `git switch -c team-<nr>/lab02-<nazwiska>`

---

## Na Początku Lab 2

1. **Skopiuj scenę:**
```bash
cp Assets/Scenes/Lab01_Baseline.unity Assets/Scenes/Lab02_Interaction.unity
```

2. **Dodaj skrypt do obiektu:**
   - Dodaj Lab02Trial.cs do XR Origin
   - Wpisz sumę numerów indeksów

3. **Utwórz workbench i module:**
   - Blat: 1.4m × 0.7m × 0.9m (Collider)
   - Module_A: 0.12 × 0.06 × 0.18m (Collider + Rigidbody)
   - Socket_A: dla chwytu (Collider trigger)

4. **Uruchom i testuj chwyt** — powinno działać

---

## Skrypty do pobrania

### Lab02Trial.cs — Rejestrator czasów
```csharp
using UnityEngine;

public class Lab02Trial : MonoBehaviour {
    int pairIndexSum = 0; // WPISZ: suma indeksów
    private float startedAt;
    private int errors;
    private bool running;
    
    public void StartTrial() {
        startedAt = Time.unscaledTime;
        errors = 0;
        running = true;
    }
    
    public void RegisterError() {
        if (running) errors++;
    }
    
    public void CompleteTrial() {
        if (!running) return;
        float seconds = Time.unscaledTime - startedAt;
        running = false;
        Debug.Log($"LAB02_RESULT seconds={seconds:F2} errors={errors} seed={pairIndexSum}");
    }
}
```

---

## Dokumentacja

- XR Grab Interactable: https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@latest/
- XR Socket Interactor: https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@latest/
- Teleportation Provider: https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@latest/

---

## Timeline Lab 2

- Wejściówka: 10 min
- Setup repozytorium: 5 min
- Implementacja interakcji: 25 min
- Lokomocja: 15 min
- Pomiary i diagnoza: 30 min
- Oddanie: 10 min

**Razem: ~90 minut**

---

**Wersja: 2.0 • 1 listopada 2026 r.**

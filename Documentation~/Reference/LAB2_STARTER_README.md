# Laboratorium 2 — Interakcja i Lokomocja VR

## Spis treści

- [Najpierw wybierz ścieżkę](#najpierw-wybierz-ścieżkę)
- [Co Powinieneś Pobrać](#co-powinieneś-pobrać)
  - [Warunki wstępne](#warunki-wstępne)
  - [Nowe pliki do pobrania](#nowe-pliki-do-pobrania)
  - [Pakiety — bez zmian](#pakiety--bez-zmian)
- [Checklist — Przed Lab 2](#checklist--przed-lab-2)
- [Na Początku Lab 2](#na-początku-lab-2)
- [Skrypty do pobrania](#skrypty-do-pobrania)
  - [Lab02Trial.cs — Rejestrator czasów](#lab02trialcs--rejestrator-czasów)
- [Dokumentacja](#dokumentacja)
- [Timeline Lab 2](#timeline-lab-2)
- [Dodatkowe, dobrowolne doświadczenie: komfort VR i głębia](#dodatkowe-dobrowolne-doświadczenie-komfort-vr-i-głębia)

## Najpierw wybierz ścieżkę
- **Mam działającą scenę z poprzedniego laboratorium:** [otwórz aktualne Lab 02](../../Samples~/Lab02/README.md#szybki-wybór-ścieżki) i pomiń powtórną instalację.
- **Nie mam Unity / konta:** [system operacyjny](OS_SETUP.md#wybierz-aktualną-sytuację) lub [licencja](UNITY_ACCESS_AND_LICENSES.md#nawigacja-warunkowa).
- **Windows:** [uruchomienie Windows](OS_SETUP.md#windows); **Ubuntu:** [uruchomienie Ubuntu](OS_SETUP.md#ubuntu).
- **Potrzebuję więcej szczegółów:** [pełny przewodnik Lab 02](../../Samples~/Lab02/README.md) zawiera spis treści i ścieżki wariantowe.
- **Nie mam sprzętu XR:** rozpocznij od symulatora lub dostarczonego logu; nie instaluj wszystkiego w ciemno.


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
- Dokumentacja komponentu `Teleportation Provider`: https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@latest/

---

## Timeline Lab 2

- Wejściówka: 10 min
- Konfiguracja repozytorium
- Implementacja interakcji: 25 min
- Lokomocja: 15 min
- Pomiary i diagnoza: 30 min
- Oddanie: 10 min

**Razem: ~90 minut**

---

**Wersja: 2.0 • 1 listopada 2026 r.**


## Dodatkowe, dobrowolne doświadczenie: komfort VR i głębia

Wybierz **WiRR → Komfort XR → Utwórz 2 prefaby i stanowiska obserwacyjne**. Powstaną prawdziwe prefaby Unity z łagodnym ruchem wskazówek wizualnych (maks. 8 s) i krótkim porównaniem wirtualnej głębi (maks. 10 s). Ich zadaniem nie jest wywoływanie nudności, lecz nauka projektowania komfortowych scen.

Instrukcja pełna: **Documentation~/Reference/COMFORT_STUDIES.md**. Przy pracy w goglach potrzebny jest świadomy ochotnik, osoba obserwująca i możliwość natychmiastowego przerwania. Reszta grupy może wykonać równoważną analizę na monitorze. **Nie przyznaje się punktów za fizjologiczną odporność ani wystąpienie objawów.**

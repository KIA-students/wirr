# Laboratorium 3 — Rejestracja AR

## Spis treści

- [Najpierw wybierz ścieżkę](#najpierw-wybierz-ścieżkę)
- [Co Powinieneś Pobrać](#co-powinieneś-pobrać)
  - [Nowe pakiety](#nowe-pakiety)
  - [Pliki](#pliki)
- [Checklist](#checklist)
- [Start Lab 3](#start-lab-3)
- [Kod](#kod)
  - [RegistrationError.cs](#registrationerrorcs)

## Najpierw wybierz ścieżkę
- **Mam działającą scenę z poprzedniego laboratorium:** [otwórz aktualne Lab 03](../../Samples~/Lab03/README.md#szybki-wybór-ścieżki) i pomiń powtórną instalację.
- **Nie mam Unity / konta:** [system operacyjny](OS_SETUP.md#wybierz-aktualną-sytuację) lub [licencja](UNITY_ACCESS_AND_LICENSES.md#nawigacja-warunkowa).
- **Windows:** [uruchomienie Windows](OS_SETUP.md#windows); **Ubuntu:** [uruchomienie Ubuntu](OS_SETUP.md#ubuntu).
- **Potrzebuję więcej szczegółów:** [pełny przewodnik Lab 03](../../Samples~/Lab03/README.md) zawiera spis treści i ścieżki wariantowe.
- **Nie mam sprzętu XR:** rozpocznij od symulatora lub dostarczonego logu; nie instaluj wszystkiego w ciemno.


## Co Powinieneś Pobrać

- ✓ Projekt z Lab 1–2
- ✓ Telefon Android z ARCore (dostępność: https://developers.google.com/ar/devices)

### Nowe pakiety
```
AR Foundation         5.1.0+
ARCore XR Plugin      5.1.0+
XR Hands              1.4.0+
```

### Pliki
```
Assets/Scenes/Lab03_AR.unity
Assets/Scripts/RegistrationError.cs
Assets/Models/ (placeholder)
```

---

## Checklist

- [ ] Android Build Support zainstalowany
- [ ] Telefon z ARCore podłączony
- [ ] USB debugging ON
- [ ] Lab03_AR.unity otwarta
- [ ] ARPlaneManager w scenie

---

## Start Lab 3

1. Git: `git switch -c team-<nr>/lab03-<nazwiska>`
2. Konfiguracja Android: Edit → Project Settings → Player → Android
3. Kompilacja i uruchomienie: `File → Build Profiles` → `Build And Run`
4. Na telefonie: pozwól na dostęp do kamery

---

## Kod

### RegistrationError.cs
```csharp
using UnityEngine;

public class RegistrationError : MonoBehaviour {
    int pairIndexSum = 0; // WPISZ
    
    public void RecordError(float distanceFromOriginMeters, float errorMeters) {
        Debug.Log($"REGISTRATION_ERROR distance={distanceFromOriginMeters:F2}m error={errorMeters:F2}m seed={pairIndexSum}");
    }
}
```

---

**Razem: ~90 minut**

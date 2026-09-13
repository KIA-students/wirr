# Laboratorium 3 — Rejestracja AR

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
3. Kompilacja: File → Build and Run
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

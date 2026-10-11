# Laboratorium 4 — Mieszanie Rzeczywistości

## Spis treści

- [Najpierw wybierz ścieżkę](#najpierw-wybierz-ścieżkę)
- [Co powinieneś pobrać](#co-powinieneś-pobrać)
  - [Nowe komponenty](#nowe-komponenty)
  - [Pliki](#pliki)
- [Checklist](#checklist)
- [Start Lab 4](#start-lab-4)
- [Kod](#kod)
  - [LightingController.cs](#lightingcontrollercs)


## Najpierw wybierz ścieżkę
- **Mam działającą scenę z poprzedniego laboratorium:** [otwórz aktualne Lab 04](../../Samples~/Lab04/README.md#szybki-wybór-ścieżki) i pomiń powtórną instalację.
- **Nie mam Unity / konta:** [system operacyjny](OS_SETUP.md#wybierz-aktualną-sytuację) lub [licencja](UNITY_ACCESS_AND_LICENSES.md#nawigacja-warunkowa).
- **Windows:** [uruchomienie Windows](OS_SETUP.md#windows); **Ubuntu:** [uruchomienie Ubuntu](OS_SETUP.md#ubuntu).
- **Potrzebuję więcej szczegółów:** [pełny przewodnik Lab 04](../../Samples~/Lab04/README.md) zawiera spis treści i ścieżki wariantowe.
- **Nie mam sprzętu XR:** rozpocznij od symulatora lub dostarczonego logu; nie instaluj wszystkiego w ciemno.


## Co powinieneś pobrać

- ✓ Projekt z Lab 3 (Android AR)
- ✓ Telefon z Androidem obsługujący API głębi (Depth API)

### Nowe komponenty
```
Depth API: Edit → Project Settings → ARCore → Enable Depth API
Shader do okluzji (zawarte w Assets/Shaders/)
```

### Pliki
```
Assets/Scenes/Lab04_SceneUnderstanding.unity
Assets/Shaders/OccludedModel.shader
Assets/Scripts/LightingController.cs
```

---

## Checklist

- [ ] Lab 3 zaliczony
- [ ] API głębi (Depth API) dostępne na telefonie
- [ ] Shader zaimportowany
- [ ] LightingController.cs przygotowany

---

## Start Lab 4

1. Git: `git switch -c team-<nr>/lab04-<nazwiska>`
2. Kopiuj scenę: `cp Assets/Scenes/Lab03_AR.unity Assets/Scenes/Lab04_SceneUnderstanding.unity`
3. Dodaj ARDepthManager do sceny
4. Przypisz shader do modelu

---

## Kod

### LightingController.cs
```csharp
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class LightingController : MonoBehaviour {
    int pairIndexSum = 0; // WPISZ
    ARCameraManager cameraManager;
    Light directionalLight;
    
    void Start() {
        cameraManager = GetComponent<ARCameraManager>();
        directionalLight = GetComponentInChildren<Light>();
    }
    
    void Update() {
        if (cameraManager.TryGetLatestFrame(
            ARCameraFrameEventArgs.LightEstimateUpdatedFlag,
            out ARCameraAcquisitionFrame frame)) {
            
            var lightEstimate = frame.lightEstimate;
            if (lightEstimate != null) {
                directionalLight.intensity = lightEstimate.averageBrightness;
                directionalLight.color = lightEstimate.colorCorrection;
                Debug.Log($"LIGHT brightness={lightEstimate.averageBrightness:F2} seed={pairIndexSum}");
            }
        }
    }
}
```

---

# Laboratorium 4 — Mieszanie Rzeczywistości

## Co powinieneś pobrać

- ✓ Projekt z Lab 3 (Android AR)
- ✓ Telefon Android z Depth API

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
- [ ] Depth API dostępna na telefonie
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

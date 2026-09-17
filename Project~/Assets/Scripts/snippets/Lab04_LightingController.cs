// Lab 4 — Light Estimation & Occlusion
// Umieść na kamerze XR (ARCameraManager)

using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class LightingController : MonoBehaviour {
    
    // ❌ ZMIEŃ TO: Wpisz sumę numerów indeksów
    private int pairIndexSum = 0;
    
    private ARCameraManager arCameraManager;
    private Light directionalLight;
    
    void Start() {
        // Pobierz komponenty
        arCameraManager = GetComponent<ARCameraManager>();
        if (arCameraManager == null) {
            Debug.LogError("ARCameraManager not found!");
            return;
        }
        
        // Szukaj światła w scenie (zazwyczaj Directional Light)
        directionalLight = FindObjectOfType<Light>();
        if (directionalLight == null) {
            Debug.LogWarning("No Directional Light found in scene");
        }
    }
    
    void Update() {
        // Pobierz najnowsze dane oświetlenia z AR Foundation
        if (arCameraManager.TryGetLatestFrame(
            ARCameraFrameEventArgs.LightEstimateUpdatedFlag,
            out ARCameraAcquisitionFrame frame)) {
            
            if (frame.lightEstimate != null) {
                // Dostosuj światło sceny do światła rzeczywistości
                float brightness = frame.lightEstimate.averageBrightness;
                Color colorCorrection = frame.lightEstimate.colorCorrection;
                
                if (directionalLight != null) {
                    directionalLight.intensity = brightness;
                    directionalLight.color = colorCorrection;
                }
                
                // Log dla Lab 4
                Debug.Log($"LIGHT brightness={brightness:F2} color_r={colorCorrection.r:F2} seed={pairIndexSum}");
            }
        }
    }
}

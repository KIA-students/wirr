// Lab 5 — LOD Performance Comparison
// Umieść na modelu CAD lub na XR Origin

using UnityEngine;

public class Lab05PerformanceBenchmark : MonoBehaviour {
    
    // ❌ ZMIEŃ TO: Wpisz sumę numerów indeksów
    private int pairIndexSum = 0;
    
    private float frameSum = 0f;
    private int frameCount = 0;
    private LODGroup lodGroup;
    
    void Start() {
        lodGroup = GetComponent<LODGroup>();
        if (lodGroup == null) {
            Debug.LogWarning("LODGroup not found on this object");
        }
    }
    
    void Update() {
        frameSum += Time.deltaTime;
        frameCount++;
        
        // Co 300 klatek
        if (frameCount == 300) {
            float fps = frameCount / frameSum;
            float milliseconds = (frameSum / frameCount) * 1000f;
            
            // Jakie LOD są aktywne?
            string lodActive = "unknown";
            if (lodGroup != null) {
                // Znalezienie aktualnego LOD (przybliżone)
                lodActive = $"LOD@{lodGroup.GetLODCount()}";
            }
            
            Debug.Log($"LAB05_BENCHMARK fps={fps:F1} ms={milliseconds:F2} lod={lodActive} seed={pairIndexSum}");
            
            frameCount = 0;
            frameSum = 0f;
        }
    }
}

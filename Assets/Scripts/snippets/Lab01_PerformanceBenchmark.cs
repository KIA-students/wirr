// Lab 1 — Performance Benchmark
// Umieść ten skrypt na XR Origin w scenie Lab01_Baseline.unity

using UnityEngine;

public class PerformanceBenchmark : MonoBehaviour {
    
    // ❌ ZMIEŃ TO: Wpisz sumę numerów indeksów Twojej pary
    private int pairIndexSum = 0;  // Np. jeśli Twoje numery to 12345 i 67890, to: 12345 + 67890 = 80235
    
    private float frameSum = 0f;
    private int frameCount = 0;
    
    void Update() {
        // Zbieramy czas każdej klatki
        frameSum += Time.deltaTime;
        frameCount++;
        
        // Co 300 klatek (≈5 sekund przy 60 FPS), wypisujemy średnią
        if (frameCount == 300) {
            float fps = frameCount / frameSum;
            float milliseconds = (frameSum / frameCount) * 1000f;
            
            // Wynik trafia do Console — to jest to, co prowadzący przegląda
            Debug.Log($"LAB01_BENCHMARK fps={fps:F1} ms={milliseconds:F2} seed={pairIndexSum}");
            
            // Reset liczników
            frameCount = 0;
            frameSum = 0f;
        }
    }
}

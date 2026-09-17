// Lab 2 — Trial Recorder (pomiary czasów interakcji)
// Umieść na XR Origin w scenie Lab02_Interaction.unity

using UnityEngine;

public class Lab02Trial : MonoBehaviour {
    
    // ❌ ZMIEŃ TO: Wpisz sumę numerów indeksów
    private int pairIndexSum = 0;
    
    private float trialStartTime;
    private int errorCount;
    private bool isRunning = false;
    
    /// <summary>
    /// Wywołaj na początek próby (po kliknięciu "START TRIAL")
    /// </summary>
    public void StartTrial() {
        trialStartTime = Time.realtimeSinceStartup;
        errorCount = 0;
        isRunning = true;
        Debug.Log("Trial started");
    }
    
    /// <summary>
    /// Wywołaj za każdym razem, gdy student popełni błąd
    /// (np. upuści moduł, kliknie błędny przycisk)
    /// </summary>
    public void RegisterError() {
        if (isRunning) {
            errorCount++;
            Debug.Log($"Error registered: {errorCount}");
        }
    }
    
    /// <summary>
    /// Wywołaj na koniec próby (po ukończeniu zadania)
    /// </summary>
    public void CompleteTrial() {
        if (!isRunning) {
            Debug.LogWarning("Trial not running!");
            return;
        }
        
        isRunning = false;
        
        float elapsedSeconds = Time.realtimeSinceStartup - trialStartTime;
        
        // Log w standardowym formacie dla Lab 2
        Debug.Log($"LAB02_RESULT seconds={elapsedSeconds:F2} errors={errorCount} seed={pairIndexSum}");
    }
}

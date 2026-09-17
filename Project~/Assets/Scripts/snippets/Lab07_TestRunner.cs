// Lab 7 — Test Runner (smoke tests, functional tests)
// Umieść w scenie Lab07 jako autostart lub wywołaj ręcznie

using UnityEngine;
using System.Collections.Generic;

public class TestRunner : MonoBehaviour {
    
    // ❌ ZMIEŃ TO: Wpisz sumę numerów indeksów
    private int pairIndexSum = 0;
    
    private List<(string name, bool passed)> results = new();
    
    public void RunSmokeTests() {
        Debug.Log("=== SMOKE TESTS ===");
        
        // Test 1: Aplikacja startuje bez crash
        results.Add(("App startup", !HasFatalErrors()));
        
        // Test 2: XR Origin initialized
        var xrOrigin = FindObjectOfType<UnityEngine.XR.Interaction.Toolkit.XROrigin>();
        results.Add(("XR Origin", xrOrigin != null));
        
        // Test 3: Main camera exists
        var mainCamera = Camera.main;
        results.Add(("Main camera", mainCamera != null));
        
        // Test 4: Input Manager ready
        results.Add(("Input system", UnityEngine.InputSystem.InputSystem.devices.Count > 0));
        
        // Test 5: Memory stable
        long memory = System.GC.GetTotalMemory(false) / (1024 * 1024); // MB
        results.Add(("Memory < 1GB", memory < 1024));
        
        // Pokaż wyniki
        int passed = 0;
        foreach (var (name, result) in results) {
            Debug.Log($"  {(result ? "✓" : "✗")} {name}");
            if (result) passed++;
        }
        
        Debug.Log($"LAB07_SMOKE_TEST passed={passed} total={results.Count} seed={pairIndexSum}");
    }
    
    private bool HasFatalErrors() {
        // Prosty check — jeśli byliśmy w stanie uruchomić ten kod, to ok
        return false;
    }
    
    // Wywoływane np. z UI button:
    // public void OnTestButtonClick() => RunSmokeTests();
}

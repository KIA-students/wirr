// Lab 6 — ROS 2 Manager (inicjalizacja węzła)
// Umieść na obiekcie w scenie Lab06_DigitalTwin.unity

using UnityEngine;

// Wymaga: using ROS2;
// (zainstaluj ROS 2 For Unity z Package Manager)

public class ROS2Manager : MonoBehaviour {
    
    // ❌ ZMIEŃ TO: Wpisz sumę numerów indeksów
    private int pairIndexSum = 0;
    
    void Start() {
        Debug.Log("Initializing ROS 2 node...");
        
        try {
            // Utwórz węzeł
            // var node = ROS2.ROS2.CreateROS2Node("unity_robot_listener");
            // Debug.Log("ROS 2 node created: unity_robot_listener");
            
            // ↑ Linia jest zakomentowana, bo ROS 2 SDK może być niestabilny
            // Odkomentuj gdy jesteś pewny, że jest zainstalowany
            
            Debug.Log($"ROS 2 initialization started (seed={pairIndexSum})");
        } 
        catch (System.Exception ex) {
            Debug.LogError($"Failed to initialize ROS 2: {ex.Message}");
        }
    }
}

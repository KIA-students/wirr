// Lab 3 — Registration Error Measurement
// Umieść na obiekcie, który mierzy błąd rejestracji

using UnityEngine;

public class RegistrationError : MonoBehaviour {
    
    // ❌ ZMIEŃ TO: Wpisz sumę numerów indeksów
    private int pairIndexSum = 0;
    
    /// <summary>
    /// Wywołaj po każdym pomiarze rejestracji
    /// </summary>
    public void RecordMeasurement(
        float distanceFromOriginMeters, 
        float errorMeters) {
        
        Debug.Log($"REGISTRATION_ERROR distance={distanceFromOriginMeters:F2}m error={errorMeters:F2}m seed={pairIndexSum}");
    }
    
    // Przykład użycia (dla studentów):
    // void OnTriggerStay(Collider other) {
    //     if (other.CompareTag("MeasurementPoint")) {
    //         float dist = Vector3.Distance(transform.position, Vector3.zero);
    //         float err = Vector3.Distance(transform.position, expectedPosition);
    //         RecordMeasurement(dist, err);
    //     }
    // }
}

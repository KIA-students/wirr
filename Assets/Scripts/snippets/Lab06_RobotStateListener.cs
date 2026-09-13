// Lab 6 — Robot State Listener (subskrybuje topiki ROS 2)
// Umieść na obiekcie 3D, który reprezentuje robota

using UnityEngine;

public class RobotStateListener : MonoBehaviour {
    
    // ❌ ZMIEŃ TO: Wpisz sumę numerów indeksów
    private int pairIndexSum = 0;
    
    // Tabela pozycji przegubów (dla wizualizacji)
    private Vector3[] jointPositions = new Vector3[6];
    private float lastUpdateTime;
    
    void Start() {
        Debug.Log($"Robot State Listener started (seed={pairIndexSum})");
        lastUpdateTime = Time.realtimeSinceStartup;
        
        // Tutaj powinna się subskrypcja do topiku ROS 2:
        // var node = ROS2.ROS2.CreateROS2Node("robot_listener");
        // var subscription = node.CreateSubscription<sensor_msgs.msg.JointState>(
        //     "/robot/state",
        //     OnJointStateReceived
        // );
    }
    
    /// <summary>
    /// Callback gdy nowy komunikat JointState przybywa
    /// </summary>
    private void OnJointStateReceived(/*sensor_msgs.msg.JointState msg*/) {
        // Przykładowa implementacja:
        // for (int i = 0; i < msg.position.Length && i < jointPositions.Length; i++) {
        //     jointPositions[i].y = (float)msg.position[i];
        // }
        
        float currentTime = Time.realtimeSinceStartup;
        float latency = (currentTime - lastUpdateTime) * 1000f; // ms
        
        // Log latency dla Lab 6
        if (latency > 10) { // loguj tylko znaczące zmiany
            Debug.Log($"LAB06_STATE latency={latency:F1}ms joints={jointPositions.Length} seed={pairIndexSum}");
            lastUpdateTime = currentTime;
        }
    }
}

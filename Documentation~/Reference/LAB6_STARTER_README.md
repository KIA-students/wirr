# Laboratorium 6 — Bliźniak Cyfrowy ROS 2

## Spis treści

- [Najpierw wybierz ścieżkę](#najpierw-wybierz-ścieżkę)
- [Co powinieneś pobrać](#co-powinieneś-pobrać)
  - [Pakiety ROS 2](#pakiety-ros-2)
  - [Pliki](#pliki)
- [Checklist](#checklist)
- [Start Lab 6](#start-lab-6)
- [Kod](#kod)
  - [ROS2Manager.cs](#ros2managercs)
  - [RobotStateListener.cs (adapter)](#robotstatelistenercs-adapter)


## Najpierw wybierz ścieżkę
- **Mam działającą scenę z poprzedniego laboratorium:** [otwórz aktualne Lab 06](../../Samples~/Lab06/README.md#szybki-wybór-ścieżki) i pomiń powtórną instalację.
- **Nie mam Unity / konta:** [system operacyjny](OS_SETUP.md#wybierz-aktualną-sytuację) lub [licencja](UNITY_ACCESS_AND_LICENSES.md#nawigacja-warunkowa).
- **Windows:** [uruchomienie Windows](OS_SETUP.md#windows); **Ubuntu:** [uruchomienie Ubuntu](OS_SETUP.md#ubuntu).
- **Potrzebuję więcej szczegółów:** [pełny przewodnik Lab 06](../../Samples~/Lab06/README.md) zawiera spis treści i ścieżki wariantowe.
- **Nie mam sprzętu XR:** rozpocznij od symulatora lub dostarczonego logu; nie instaluj wszystkiego w ciemno.


## Co powinieneś pobrać

- ✓ Projekt z Lab 5
- ✓ ROS 2 (Humble albo Jazzy) zainstalowany
- ✓ Robot / symulator Gazebo

### Pakiety ROS 2
```
ROS 2 For Unity (ros2-for-unity)
geometry_msgs, std_msgs, control_msgs
```

### Pliki
```
Assets/Scripts/
├── ROS2Manager.cs
├── RobotStateListener.cs
└── LatencyMeasurement.cs
Assets/Scenes/Lab06_DigitalTwin.unity
```

---

## Checklist

- [ ] ROS 2 działający na maszynie
- [ ] Robot/Gazebo startuje
- [ ] `ros2 topic list` wyświetla topiki
- [ ] Topiki: /robot/state, /robot/pose (lub podobne)
- [ ] Unity: ROS 2 For Unity zainstalowany
- [ ] `ros2 node list` pokaże węzeł Unity (na koniec)

---

## Start Lab 6

1. Uruchom robota/Gazebo
2. Terminal: `ros2 topic echo /robot/state` — sprawdź topik
3. Unity: `git switch -c team-<nr>/lab06-<nazwiska>`
4. Dodaj ROS2Manager.cs do sceny
5. Uruchom Play Mode
6. Terminal: `ros2 node list` — powinien być unity_listener

---

## Kod

### ROS2Manager.cs
```csharp
using UnityEngine;
using ROS2;

public class ROS2Manager : MonoBehaviour {
    void Start() {
        var node = ROS2.ROS2.CreateROS2Node("unity_listener");
        Debug.Log("ROS 2 node created: unity_listener");
    }
}
```

### RobotStateListener.cs (adapter)
```csharp
using ROS2;

public class RobotStateListener : MonoBehaviour {
    int pairIndexSum = 0; // WPISZ
    
    void Start() {
        var node = ROS2.ROS2.CreateROS2Node("state_listener");
        var sub = node.CreateSubscription<JointState>(
            "/robot/state",
            OnJointStateReceived
        );
    }
    
    void OnJointStateReceived(JointState msg) {
        // msg.position[] — pozycje przegubów
        Debug.Log($"Received {msg.position.Length} joints, seed={pairIndexSum}");
    }
}
```



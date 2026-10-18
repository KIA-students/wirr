# WiRR WebSim — lokalny backend Lab 06

Ten katalog uruchamia przez Docker pojedynczy, izolowany backend ROS 2 dla Lab 06. Zawiera `rosbridge` (WebSocket) i deterministyczny generator `sensor_msgs/msg/JointState`; nie wymaga instalacji ROS 2 ani Gazebo na komputerach studentów. Stan jest generowany przez prawdziwy węzeł ROS 2, dzięki czemu Unity obserwuje topiki identycznie jak przy zewnętrznym symulatorze.

Publiczna instrukcja krok po kroku jest publikowana przez GitHub Pages z katalogu `site/`. Strona nie uruchamia backendu ani nie przechowuje danych — służy wyłącznie jako punkt startowy dla studentów.

## Uruchomienie

1. Zainstaluj Docker Desktop (Windows/macOS) lub Docker Engine z wtyczką Compose (Linux).
2. W terminalu przejdź do tego katalogu i uruchom:

   ```bash
   docker compose up --build
   ```

3. Poczekaj na komunikat `READY: WiRR WebSim ROS 2 signal generator`.
4. W Unity, w `WiRR → Course Toolkit → Lab 06`, wpisz:

   | Sposób uruchomienia | Adres Backend WebSocket |
   | --- | --- |
   | Ten sam komputer | `ws://127.0.0.1:9090` |
   | Inny komputer w tej samej sieci | `ws://ADRES_IP_SERWERA:9090` |

5. Podaj kod zespołu (3–12 znaków: `A–Z`, cyfry, `_`, `-`), wybierz robot, kliknij **Create / repair WebSim digital twin**, uruchom Play Mode i kliknij **Connect**.

Port `9090/TCP` musi być dostępny przez zaporę na komputerze serwera. Nie wystawiaj go publicznie do Internetu: backend nie ma uwierzytelniania. Dla połączenia spoza LAN należy dodać osobny reverse proxy z TLS/WSS i kontrolą dostępu.

## Co jest symulowane

Po `Connect` klient Unity wysyła `create` do `/wirr/control`; backend tworzy osobne topiki:

```text
/wirr/<SESSION>/joint_states   sensor_msgs/msg/JointState, 30 Hz
/wirr/<SESSION>/command        std_msgs/msg/String
/wirr/<SESSION>/status         std_msgs/msg/String
/wirr/status                   std_msgs/msg/String
/clock                         rosgraph_msgs/msg/Clock
```

Polecenia z panelu `Motion A/B/C`, `Home` i `Reset` zmieniają docelowe położenia przegubów. Generator stosuje powtarzalną dynamikę pierwszego rzędu, dlatego ruch i pomiar opóźnienia są widoczne w Unity. `RRBot 2R` publikuje `joint1`, `joint2`; `WiRR Arm 3R` dodatkowo `joint3`.

## Obsługa

```bash
# logi i stan
docker compose logs -f
docker compose ps

# zatrzymanie
docker compose down
```

W terminalu kontenera można zweryfikować topiki:

```bash
docker compose exec websim bash -lc 'source /opt/ros/jazzy/setup.bash && ros2 topic list'
```

To jest celowo lekki generator sygnału do laboratorium. Klasyczny wariant Local/LAN z Gazebo pozostaje osobną ścieżką ćwiczenia, a jego `JointState` może zastąpić ten generator bez zmiany kodu Unity.

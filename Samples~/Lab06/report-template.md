# Laboratorium 6 — Bliźniak cyfrowy z ROS 2 i Gazebo

## Dane zespołu

- Osoba 1: ........................................................
- Nr indeksu `i1`: ............
- Osoba 2: ........................................................
- Nr indeksu `i2`: ............
- Osoba 3 (opcjonalnie): ..............................................
- Nr indeksu `i3` (opcjonalnie): ............
- `S = suma indeksów wszystkich osób w zespole`: ............
- `v1`: ....  `v2`: ....  `v3`: ....  `v4`: ....  `v5`: ....
- Data: ............................................................
- SHA ostatniego commita: ..........................................

## 1. Konfiguracja środowiska

| Element | Wartość |
|---|---|
| Unity | |
| System operacyjny Unity | |
| ROS 2 / dystrybucja | |
| System operacyjny ROS 2 | |
| Gazebo Sim / wersja | |
| `ros_gz` | |
| `gz_ros2_control` | |
| `ros2_control_demos` / branch lub SHA | |
| Robot symulowany | RRBot / inny: |
| ROS-TCP-Connector | |
| ROS-TCP-Endpoint | |
| Adres IPv4 hosta ROS | |
| Port | |
| Transport Unity ↔ ROS | localhost / LAN / Wi-Fi / inne |

### Topologia zespołu i sieć

| Element | Wartość |
|---|---|
| Komputer A — ROS 2 + Gazebo + Endpoint | osoba / system: |
| IPv4 komputera A | |
| Komputer B — Unity | osoba / system: |
| IPv4 komputera B | |
| Komputer C — diagnostyka (opcjonalny) | osoba / system: |
| Tryb | dwa komputery / jeden komputer |
| `ping` B → A | PASS / FAIL |
| Test TCP B → A:10000 | PASS / FAIL |
| `ss -ltn` na A pokazuje port 10000 | TAK / NIE |
| UFW / inna zapora | stan / zastosowana reguła: |

### Weryfikacja instalacji bazowej

| Kontrola | Wynik / wersja |
|---|---|
| Ubuntu 22.04 / `jammy` | |
| Architektura `amd64` / `arm64` | |
| Locale UTF-8 | |
| `ros2 --help` działa | TAK / NIE |
| Talker → listener | PASS / FAIL |
| `rosdep update` | PASS / FAIL |
| `ros2 doctor --report` | uwagi: |
| `ros2 pkg prefix ros_gz_sim` | |
| `ros2 pkg prefix gz_ros2_control` | |
| `ign gazebo --versions` | |
| `ign gazebo --force-version 6 shapes.sdf` | PASS / FAIL |

### ROS Settings w Unity

| Ustawienie | Wartość |
|---|---|
| Protocol | |
| Connect on Startup | |
| ROS IP Address | |
| ROS Port | |
| KeepAlive time | |
| Network timeout | |
| Listen for TF Messages | |

## 2. Uruchomienie robota w Gazebo

### Kontrolery

Wklej istotny wynik `ros2 control list_controllers`:

```text

```

### Interfejsy sprzętowe symulacji

Wklej istotny wynik `ros2 control list_hardware_interfaces`:

```text

```

### Test ruchu RRBot

| Próba | Komenda `[joint1, joint2]` [rad] | `JointState` po ustaleniu [rad] | Czy Gazebo i Unity poruszyły się zgodnie? |
|---:|---|---|---|
| 1 | | | |
| 2 | | | |
| 3 | | | |

Dodaj zrzut ekranu, na którym jednocześnie widać robota w Gazebo oraz jego bliźniaka w Unity.

## 3. Audyt grafu ROS 2

### Węzły

```text
Wklej wynik lub istotny fragment `ros2 node list`.
```

### Tematy

| Temat | Typ | Publisher(s) | Subscriber(s) | QoS / uwagi | Częstotliwość |
|---|---|---:|---:|---|---:|
| `/joint_states` | `sensor_msgs/msg/JointState` | | | | |
| `/dynamic_joint_states` | | | | | |
| `/tf` | | | | | |
| `/tf_static` | | | | | |
| `/clock` | `rosgraph_msgs/msg/Clock` | | | | |
| `/forward_position_controller/commands` | | | | | |
| `/lab06/ping` | | | | | |
| `/lab06/pong` | | | | | |

### Architektura zaobserwowana na stanowisku

Uzupełnij rzeczywisty przepływ:

```text
Gazebo Sim
  ↓
gz_ros2_control / controller_manager
  ↓
joint_state_broadcaster
  ↓
/joint_states
  ↓
ROS-TCP-Endpoint
  ↓ TCP:10000
ROS-TCP-Connector / Unity
```

### Wniosek z audytu

................................................................................

## 4. Eksperyment 1 — Gazebo → ROS 2 → Unity, wariant `v1 = ...`

**Hipoteza / oczekiwanie:**

................................................................................

**Konfiguracja:**

................................................................................

**Dane / dowód:**

| Parametr / próba | Wynik |
|---|---:|
| | |
| | |
| | |

**Wniosek:**

................................................................................

## 5. Mapowanie JointState → model Unity

| `rosName` | Obiekt Unity | Typ | Oś lokalna | `sign` | `offset` | Jednostka |
|---|---|---|---|---:|---:|---|
| `joint1` | | revolute | | | | rad |
| `joint2` | | revolute | | | | rad |
| | | | | | | |

### Konwencja układów współrzędnych

W RRBot oba przeguby w opisie ROS obracają się wokół osi `(0, 1, 0)`. Opisz, jak ta oś została odwzorowana na lokalną oś modelu Unity i jak zweryfikowano znak obrotu:

................................................................................

### Trzy pozycje kontrolne

Dla każdej pozycji zapisz wartość w źródle, wartość odczytaną/zaobserwowaną w Unity oraz błąd w stopniach. Jeżeli model ma tylko dwa przeguby, pola joint3 oznacz jako N/A.

| Pozycja | joint1 źródło [deg] | joint1 Unity [deg] | błąd joint1 [deg] | joint2 źródło [deg] | joint2 Unity [deg] | błąd joint2 [deg] | joint3 źródło [deg] | joint3 Unity [deg] | błąd joint3 [deg] |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| 1 | | | | | | | | | |
| 2 | | | | | | | | | |
| 3 | | | | | | | | | |

## 6. Eksperyment 2 — wariant `v2 = ...`

**Konfiguracja błędna:**

................................................................................

**Objaw w Gazebo / ROS / Unity:**

................................................................................

**Przyczyna:**

................................................................................

**Poprawka:**

................................................................................

**Wynik po poprawce:**

................................................................................

## 7. Buforowanie i interpolacja

- `interpolationDelayMs`: ............ ms
- `maxBufferedSamples`: ............
- `staleAfterMs`: ............ ms

### Seria inter-arrival odbierana w Unity

Wpisz co najmniej 30 wartości albo dołącz CSV i podaj jego ścieżkę.

| n | inter-arrival [ms] |
|---:|---:|
| 1 | |
| 2 | |
| 3 | |
| ... | ... |

- `N =` ............
- średnia `T̄ =` ............ ms
- odchylenie standardowe `sT =` ............ ms

## 8. Eksperyment 3 — wariant `v3 = ...`

| Konfiguracja | Źródło | Częstotliwość [Hz] | Delay [ms] | Buffer | `T̄` [ms] | `sT` [ms] | Obserwacja płynności |
|---|---|---:|---:|---:|---:|---:|---|
| A | Gazebo | | | | | | |
| B | Gazebo | | | | | | |
| C | generator kontrolowany / Gazebo | | | | | | |

**Wniosek:**

................................................................................

## 9. Pomiar RTT Unity ↔ ROS 2

Opisz, dlaczego RTT nie wymaga synchronizacji zegarów oraz dlaczego nie jest równy jednostronnemu opóźnieniu. Zaznacz, że RTT mierzy tor Unity → ROS-TCP → węzeł echo → ROS-TCP → Unity, a nie czas rozwiązania fizyki Gazebo.

................................................................................

### Wyniki bazowe

| n | RTT [ms] |
|---:|---:|
| 1 | |
| 2 | |
| 3 | |
| ... | ... |

- liczba próbek: ............
- mean: ............ ms
- min: ............ ms
- max: ............ ms

## 10. Eksperyment 4 — wariant `v4 = ...`

| Konfiguracja | N | mean RTT [ms] | min [ms] | max [ms] | Warunki |
|---|---:|---:|---:|---:|---|
| A | | | | | |
| B | | | | | |

**Wniosek:**

................................................................................

## 11. Stan LIVE / STALE przy zatrzymaniu symulacji

| Parametr | Wartość |
|---|---:|
| próg STALE | ms |
| częstotliwość `/joint_states` przed pauzą | Hz |
| czas pauzy Gazebo | s |
| zmierzony czas detekcji STALE | ms |
| zmierzony czas odzyskania LIVE | ms |

Opisz, co działo się równocześnie z `/clock`, `/joint_states` i modelem Unity podczas pauzy Gazebo:

................................................................................

## 12. Eksperyment 5 — wariant `v5 = ...`

### Baseline

................................................................................

### Kontrolowany błąd

................................................................................

### Objaw

................................................................................

### Hipoteza

................................................................................

### Test diagnostyczny

```text
Wpisz użyte polecenia ROS 2 / ustawienia Gazebo / ustawienia Unity i kluczowy wynik.
```

### Poprawka

................................................................................

### Ponowny pomiar

................................................................................

### Wniosek

................................................................................

## 13. Ograniczenia zbudowanego bliźniaka

Wymień co najmniej trzy ograniczenia, np. uproszczona geometria RRBot, brak pełnej rekonstrukcji `tf2`, brak synchronizacji zegarów, opóźnienie wynikające z bufora, brak odwzorowania sił/kontaktów Gazebo, brak sterowania fizycznym robotem.

1. .............................................................................
2. .............................................................................
3. .............................................................................

## 14. Podsumowanie

Odpowiedz krótko:

1. Który element oblicza fizykę robota: Gazebo, ROS-TCP-Endpoint czy Unity?
2. Jak stan przegubów przechodzi z Gazebo do Unity?
3. Dlaczego `joint_state_broadcaster` jest kluczowy dla tej instrukcji?
4. Dlaczego mapowanie po nazwach przegubów jest ważne?
5. Jaki kompromis wprowadza bufor interpolacyjny?
6. Co dokładnie mierzy RTT?
7. Jak system sygnalizuje utratę wiarygodności danych po zatrzymaniu symulacji?

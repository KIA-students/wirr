# Lab 06 — Bliźniak cyfrowy ROS 2 + Gazebo

Ten folder jest próbką pakietu **WiRR — narzędzia kursu** przeznaczoną dla Laboratorium 6.

Laboratorium obsługuje trzy równorzędne topologie techniczne:

1. **Local** — Unity, ROS 2 i Gazebo na jednej maszynie;
2. **LAN** — ROS 2 + Gazebo na komputerze A, Unity na komputerze B;
3. **WebSim** — ROS 2 uruchomione w kontenerowym generatorze sygnału, a Unity łączy się przez `rosbridge` WebSocket. Ten wariant nie wymaga lokalnej instalacji ROS 2 ani Gazebo po stronie Unity.

## Zalecana kolejność

1. Otwórz `WiRR → Narzędzia kursu`.
2. Wybierz Lab 06.
3. Zainstaluj / napraw zależności.
4. Użyj `Utwórz / napraw aktywną scenę`.
5. Wybierz topologię Local, LAN albo WebSim.
6. Dla **WebSim** uruchom `WebSim~/` przez `docker compose up --build`. Wpisz `ws://127.0.0.1:9090` (ten komputer) lub `ws://<IP-serwera>:9090` (LAN), kod sesji zespołu i model robota, a następnie kliknij **Utwórz / napraw model WebSim**.
7. Uruchom Play Mode i kliknij **Połącz**.
8. Sprawdź `Ruch A/B/C`, obserwując konsolę i zmianę przegubów. Stan ma pochodzić z `/wirr/<SESSION>/joint_states`, a nie z lokalnej animacji Unity.
9. Uruchom `Sprawdź konfigurację laboratorium` i usuń błędy konfiguracji.
10. Do sprawozdania użyj `Otwórz awaryjny szablon Markdown`.

W trybie Local/LAN panel nadal może zainstalować ROS-TCP-Connector. W trybie WebSim lokalna instalacja ROS 2 i Gazebo na komputerze Unity nie jest wymagana. WebSim publikuje powtarzalny sygnał `JointState` z węzła ROS 2; klasyczny wariant z Gazebo pozostaje dostępny do rozszerzenia eksperymentu.

Ten sam `WiRRRobotRig` i te same nazwy przegubów są używane w Lab 04 jako cyfrowy cień oraz w Lab 06 jako reprezentacja bliźniaka cyfrowego.

## Materiały dydaktyczne

Po przygotowaniu sceny możesz użyć sekcji **Scena i pomiary → Materiały dydaktyczne**:

1. **Dodaj środowisko** — tworzy wspólne stanowisko WiRR; w Lab 03–04 zamiast wirtualnego pokoju używany jest lekki zestaw odniesienia AR.
2. **Dodaj zestaw eksperymentalny** — tworzy prefaby właściwe dla bieżącego laboratorium w `Assets/WiRR/Lab06/Prefabs/Generated` i umieszcza ich instancje pod `WiRR_TeachingAssets`.
3. **Usuń obiekty dydaktyczne ze sceny** — usuwa wyłącznie gałąź `WiRR_TeachingAssets`; nie usuwa pracy studenta ani wygenerowanych prefabów.

Materiały są opcjonalne i służą jako kontekst eksperymentu. Nie konfigurują za studenta komponentów stanowiących cel ćwiczenia. Folder `Prefabs/Generated` jest odtwarzalny — własne rozwiązania zapisuj poza nim.

`report-template.md` jest synchronizowany z aktualnym szablonem instrukcji laboratorium.

# Lab 06 — Bliźniak cyfrowy ROS 2 + Gazebo

Ten folder jest próbką pakietu **WiRR Course Toolkit** przeznaczoną dla Laboratorium 6.

Laboratorium obsługuje trzy równorzędne topologie techniczne:

1. **Local** — Unity, ROS 2 i Gazebo na jednej maszynie;
2. **LAN** — ROS 2 + Gazebo na komputerze A, Unity na komputerze B;
3. **WebSim** — ROS 2 + Gazebo uruchomione w backendzie kontenerowym, a Unity łączy się przez `rosbridge` WebSocket/WSS.

## Zalecana kolejność

1. Otwórz `WiRR → Course Toolkit`.
2. Wybierz Lab 06.
3. Zainstaluj / napraw zależności.
4. Użyj `Create / repair base scene`.
5. Wybierz topologię Local, LAN albo WebSim.
6. Dla **WebSim** wpisz adres `wss://...`, kod sesji zespołu i model robota, a następnie kliknij **Create / repair WebSim digital twin**.
7. Uruchom Play Mode i kliknij **Connect WebSim**.
8. Sprawdź `Motion A/B/C`, obserwując konsolę i zmianę przegubów. Stan ma pochodzić z `/wirr/<SESSION>/joint_states`, a nie z lokalnej animacji Unity.
9. Uruchom `Validate scene` i usuń błędy konfiguracji.
10. Do sprawozdania użyj `Create / open report template`.

W trybie Local/LAN panel nadal może zainstalować ROS-TCP-Connector. W trybie WebSim lokalna instalacja ROS 2 i Gazebo na komputerze Unity nie jest wymagana — są one uruchamiane po stronie backendu.

Ten sam `WiRRRobotRig` i te same nazwy przegubów są używane w Lab 04 jako cyfrowy cień oraz w Lab 06 jako reprezentacja bliźniaka cyfrowego.

`report-template.md` jest synchronizowany z aktualnym szablonem instrukcji laboratorium.

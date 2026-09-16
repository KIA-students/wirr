# Lab 04 — Rozumienie sceny i mieszanie rzeczywistości

Ten folder jest próbką pakietu **WiRR Course Toolkit** przeznaczoną dla Laboratorium 4.

Po instalacji zależności zaimportuj Starter Assets oraz AR Starter Assets.

## Zalecana kolejność

1. Otwórz `WiRR → Course Toolkit`.
2. Wybierz Lab 04.
3. Zainstaluj / napraw zależności.
4. Zaimportuj oficjalne próbki Unity, jeżeli są wymagane.
5. Użyj `Create / repair base scene`.
6. Jeżeli ćwiczenie wykorzystuje robota jako cyfrowy cień, w sekcji **WiRR WebSim** wpisz adres WSS i kod sesji, wybierz robota, a następnie kliknij **Create / repair digital shadow**.
7. W Play Mode połącz WebSim i sprawdź, czy ruchy A/B/C zmieniają geometrię robota na podstawie zewnętrznego `/joint_states`.
8. Umieść cyfrowy cień na wykrytej płaszczyźnie i oceniaj okluzję, głębię i integrację MR zgodnie z instrukcją laboratorium.
9. Uruchom `Validate scene`.
10. Do sprawozdania użyj `Create / open report template`.

**Ważne:** ruch robota w wariancie WebSim nie jest animacją Unity. `WiRRRobotRig` odwzorowuje stan przegubów przesyłany z zewnętrznego źródła. Dzięki temu ten sam model jest później używany w Lab 06 jako część bliźniaka cyfrowego.

`report-template.md` jest synchronizowany z aktualnym szablonem instrukcji laboratorium.

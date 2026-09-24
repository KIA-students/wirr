# Lab 06 — Bliźniak cyfrowy ROS 2 + Gazebo

Ten folder jest próbką pakietu **WiRR Course Toolkit** przeznaczoną dla Laboratorium 6. Poniższa lista prowadzi przez wykonanie ćwiczenia w tej samej kolejności, którą pokazuje panel `WiRR → Narzędzia kursu`. Instrukcja laboratoryjna rozwija teorię i sposób interpretacji, ale kolejność pracy i wymagane dowody są zebrane tutaj.

## Przygotowanie

1. Otwórz `WiRR → Narzędzia kursu` i wybierz Lab 06.
2. Użyj **Zainstaluj / napraw zależności**.
3. Zaimportuj próbkę WiRR oraz wymagane oficjalne próbki Unity.
4. Użyj **Utwórz / napraw aktywną scenę**.
5. Uruchom **Sprawdź konfigurację laboratorium** i usuń błędy blokujące.
6. Wybierz jedną topologię: LOCAL, LAN albo WEBSIM.
7. Dla WebSim uruchom `docker compose up --build`, ustaw adres WebSocket, kod sesji i model; dla LOCAL/LAN uruchom ROS 2 i Gazebo.
8. Utwórz model w Unity, uruchom Play Mode i uzyskaj stan LIVE przed pomiarami.
9. Opcjonalnie zaimportuj **Unitree G1 EDU + animacje** z sekcji **Scena i pomiary → Dodatkowe modele 3D** jako dodatkową reprezentację wizualną. Klipy animacji nie zastępują `JointState` i nie są źródłem danych pomiarowych.

## Zadania krok po kroku

### 3.0: Źródło stanu

1. Zapisz topologię, wersje komponentów, adres/port i transport.
2. Uruchom źródło joint_states: Gazebo/ROS 2 albo WebSim.
3. Wyślij co najmniej trzy polecenia ruchu i potwierdź zgodność źródła z Unity.
4. Zmierz częstotliwość JointState i narysuj/opisz przepływ źródło→ROS/rosbridge→Unity.

Po zakończeniu tego punktu uzupełnij odpowiadającą mu sekcję formularza **WiRR Reports**. Nie przechodź do kolejnego eksperymentu, dopóki nie zapiszesz warunków pomiaru i wyniku.

### 3.5: Mapowanie przegubów

1. Dla każdego joint zapisz obiekt Unity, oś, sign, offset i jednostkę.
2. Mapuj po nazwie joint, nie po indeksie tablicy position.
3. Ustaw trzy znane pozycje i dla każdej zapisz joint1/joint2/joint3 osobno w źródle oraz w Unity.
4. Dla każdej pozycji policz błąd każdego przegubu w stopniach i wyjaśnij różnice konwencji.

Po zakończeniu tego punktu uzupełnij odpowiadającą mu sekcję formularza **WiRR Reports**. Nie przechodź do kolejnego eksperymentu, dopóki nie zapiszesz warunków pomiaru i wyniku.

### 4.0: Transport i bufor

1. Zarejestruj 30 wartości inter-arrival JointState.
2. Wykonaj warianty bufora A/B/C i zapisz Hz, delay, jitter oraz płynność.
3. Wykonaj co najmniej 10 pomiarów RTT.
4. Wybierz kompromis bufora ograniczający jitter bez nadmiernego opóźnienia.

Po zakończeniu tego punktu uzupełnij odpowiadającą mu sekcję formularza **WiRR Reports**. Nie przechodź do kolejnego eksperymentu, dopóki nie zapiszesz warunków pomiaru i wyniku.

### 4.5: LIVE/STALE

1. Zapisz próg STALE i nominalne joint_states Hz.
2. Zatrzymaj źródło danych i zmierz czas do STALE.
3. Wznow źródło i zmierz czas powrotu do LIVE; porównaj warunki A/B.
4. Zapisz H1/H2 dla źródła opóźnień i wykonaj test rozstrzygający.

Po zakończeniu tego punktu uzupełnij odpowiadającą mu sekcję formularza **WiRR Reports**. Nie przechodź do kolejnego eksperymentu, dopóki nie zapiszesz warunków pomiaru i wyniku.

### 5.0: Kontrolowany błąd bliźniaka

1. Zapisz baseline: jointy, Hz i RTT.
2. Wprowadź kontrolowany błąd v5 i zanotuj objaw.
3. Wykonaj H1/H2, test diagnostyczny i minimalną poprawkę.
4. Po naprawie wyznacz maksymalny błąd synchronizacji i potwierdź STALE→LIVE.

Po zakończeniu tego punktu uzupełnij odpowiadającą mu sekcję formularza **WiRR Reports**. Nie przechodź do kolejnego eksperymentu, dopóki nie zapiszesz warunków pomiaru i wyniku.

## Zakończenie laboratorium

1. Otwórz **WiRR → Raporty → Formularz raportu laboratoryjnego** i wybierz najwyższy kompletny punkt kontrolny.
2. Sprawdź, czy każda tabela zawiera warunki pomiaru, wyniki surowe i krótki wniosek.
3. Użyj **Sprawdź raport** i popraw wskazane braki.
4. Wyślij raport dopiero po przejściu walidacji. Checkboxy z panelu zadań są lokalną pomocą i nie są częścią oceny ani raportu.

## Materiały dydaktyczne
Po przygotowaniu sceny możesz użyć sekcji **Scena i pomiary → Materiały dydaktyczne**:

1. **Dodaj środowisko** — tworzy wspólne stanowisko WiRR; w Lab 03–04 zamiast wirtualnego pokoju używany jest lekki zestaw odniesienia AR.
2. **Dodaj zestaw eksperymentalny** — tworzy prefaby właściwe dla bieżącego laboratorium w `Assets/WiRR/Lab06/Prefabs/Generated` i umieszcza ich instancje pod `WiRR_TeachingAssets`.
3. **Usuń obiekty dydaktyczne ze sceny** — usuwa wyłącznie gałąź `WiRR_TeachingAssets`; nie usuwa pracy studenta ani wygenerowanych prefabów.
4. **Importuj Unitree G1 EDU + animacje** — dodatkowy model humanoidalny do wizualizacji i prób sceny. W obowiązkowych pomiarach mapowania używaj danych ze źródła ROS 2/WebSim; animacja modelu nie jest referencją JointState.

Materiały są opcjonalne i służą jako kontekst eksperymentu. Nie konfigurują za studenta komponentów stanowiących cel ćwiczenia. Folder `Prefabs/Generated` jest odtwarzalny — własne rozwiązania zapisuj poza nim.

`report-template.md` jest synchronizowany z aktualnym szablonem instrukcji laboratorium.

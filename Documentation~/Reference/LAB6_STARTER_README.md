# Lab 06 — Bliźniak cyfrowy ROS 2 + Gazebo

Instrukcja kursowa: WiRR Lab 06 v1.0.2. Unity 6000.6.5f1 lub nowsza zgodna poprawka z linii 6000.6.5f1.

## Spis treści

- [Szybki wybór ścieżki](#szybki-wybór-ścieżki)
- [Przygotowanie](#przygotowanie)
- [Zadania krok po kroku](#zadania-krok-po-kroku)
  - [3.0: Źródło stanu](#30-źródło-stanu)
  - [3.5: Mapowanie przegubów](#35-mapowanie-przegubów)
  - [4.0: Transport i bufor](#40-transport-i-bufor)
  - [4.5: Pomiar RTT](#45-pomiar-rtt)
  - [5.0: Kontrolowany błąd bliźniaka](#50-kontrolowany-błąd-bliźniaka)
- [Zakończenie laboratorium](#zakończenie-laboratorium)
- [Materiały dydaktyczne](#materiały-dydaktyczne)


## Szybki wybór ścieżki
- **Mam działające stanowisko ROS 2 i /joint_states:** przejdź do [źródła stanu](#30-źródło-stanu) i potem [LIVE/STALE](#45-pomiar-rtt).
- **Nie mam ROS 2 / Gazebo:** użyj przygotowanego logu i wariantu replay. Rozpoznaj ograniczenia w [źródle stanu](#30-źródło-stanu).
- **Pracuję na Windows:** połącz się ze stanowiskiem Ubuntu przez sieć. Nie instaluj ROS 2 bez konieczności. [Systemy i urządzenia](../../Documentation~/Reference/OS_SETUP.md#urządzenia-xr).
- **Pracuję na Ubuntu:** [ścieżka Ubuntu](../../Documentation~/Reference/OS_SETUP.md#ubuntu).


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
3. Ustaw trzy znane pozycje i dla każdej zapisz rzeczywiste przeguby osobno; joint3 = N/A dla RRBot w źródle oraz w Unity.
4. Dla każdej pozycji policz błąd każdego przegubu w stopniach i wyjaśnij różnice konwencji.

Po zakończeniu tego punktu uzupełnij odpowiadającą mu sekcję formularza **WiRR Reports**. Nie przechodź do kolejnego eksperymentu, dopóki nie zapiszesz warunków pomiaru i wyniku.

### 4.0: Transport i bufor

1. Zarejestruj 30 wartości inter-arrival JointState.
2. Wykonaj warianty bufora A/B/C i zapisz Hz, opóźnienie, drgania czasowe (jitter) oraz płynność.
3. Zapisz pierwsze 30 odstępów między wiadomościami. WebSim wymaga własnego bufora lub wykonania etapu w LOCAL/LAN.
4. Wybierz kompromis bufora ograniczający drgania czasowe (jitter) bez nadmiernego opóźnienia.

Po zakończeniu tego punktu uzupełnij odpowiadającą mu sekcję formularza **WiRR Reports**. Nie przechodź do kolejnego eksperymentu, dopóki nie zapiszesz warunków pomiaru i wyniku.

### 4.5: Pomiar RTT

1. Uruchom jawny pomiar żądanie/odpowiedź. Dla WebSim przygotuj własny próbnik lub wykonaj etap w LOCAL/LAN.
2. Wykonaj co najmniej 50 próbek na każdy warunek A/B wariantu v4. Zachowaj pełny log; pierwsze 10 próbek bazowych wpisz do formularza.
3. Porównaj N, średnią, minimum i maksimum. Nie utożsamiaj RTT z jednostronnym opóźnieniem.
4. Zapisz H1/H2, warunki sieci i wniosek diagnostyczny.

Po zakończeniu uzupełnij etap 4.5 formularza oraz dołącz log.

### 5.0: Kontrolowany błąd bliźniaka

1. Zapisz wariant bazowy, próg STALE i Hz. Zatrzymaj źródło danych; zmierz czas do STALE i powrotu do LIVE.
2. Wprowadź kontrolowany błąd v5 i zanotuj objaw.
3. Wykonaj H1/H2, test diagnostyczny i minimalną poprawkę.
4. Po naprawie wyznacz maksymalny błąd synchronizacji i potwierdź STALE→LIVE.

Po zakończeniu tego punktu uzupełnij odpowiadającą mu sekcję formularza **WiRR Reports**. Nie przechodź do kolejnego eksperymentu, dopóki nie zapiszesz warunków pomiaru i wyniku.

## Zakończenie laboratorium

1. Otwórz **WiRR → Raporty → Formularz raportu laboratoryjnego** i wybierz najwyższy kompletny punkt kontrolny.
2. Sprawdź, czy każda tabela zawiera warunki pomiaru, wyniki surowe i krótki wniosek.
3. Użyj **Sprawdź kompletność raportu** i popraw wskazane braki.
4. Wyślij raport dopiero po przejściu walidacji. Checkboxy z panelu zadań są lokalną pomocą i nie są częścią oceny ani raportu.

## Materiały dydaktyczne
Po przygotowaniu sceny możesz użyć sekcji **Scena i pomiary → Materiały dydaktyczne**:

1. **Dodaj środowisko** — tworzy wspólne stanowisko WiRR; w Lab 03–04 zamiast wirtualnego pokoju używany jest lekki zestaw odniesienia AR.
2. **Dodaj zestaw eksperymentalny** — tworzy prefaby właściwe dla bieżącego laboratorium w `Assets/WiRR/Lab06/Prefabs/Generated` i umieszcza ich instancje pod `WiRR_TeachingAssets`.
3. **Usuń obiekty dydaktyczne ze sceny** — usuwa wyłącznie gałąź `WiRR_TeachingAssets`; nie usuwa pracy studenta ani wygenerowanych prefabów.
4. **Importuj Unitree G1 EDU + animacje** — dodatkowy model humanoidalny do wizualizacji i prób sceny. W obowiązkowych pomiarach mapowania używaj danych ze źródła ROS 2/WebSim; animacja modelu nie jest referencją JointState.

Materiały są opcjonalne i służą jako kontekst eksperymentu. Nie konfigurują za studenta komponentów stanowiących cel ćwiczenia. Folder `Prefabs/Generated` jest odtwarzalny — własne rozwiązania zapisuj poza nim.

`report-template.md` jest synchronizowany z aktualnym szablonem instrukcji laboratorium.


<a id="checklist"></a>
<a id="co-powinieneś-pobrać"></a>
<a id="kod"></a>
<a id="laboratorium-6--bliźniak-cyfrowy-ros-2"></a>
<a id="najpierw-wybierz-ścieżkę"></a>
<a id="pakiety-ros-2"></a>
<a id="pliki"></a>
<a id="robotstatelistenercs-adapter"></a>
<a id="ros2managercs"></a>
<a id="start-lab-6"></a>

# Lab 01 — Środowisko XR i audyt urządzeń

## Spis treści

- [Szybki wybór ścieżki](#szybki-wybór-ścieżki)
- [Konto Unity: nie potrzebujesz wspólnego loginu](#konto-unity-nie-potrzebujesz-wspólnego-loginu)
- [Pierwsze 10 minut: eksperyment zamiast instalatora](#pierwsze-10-minut-eksperyment-zamiast-instalatora)
- [Przygotowanie dalszych eksperymentów](#przygotowanie-dalszych-eksperymentów)
- [Zadania krok po kroku](#zadania-krok-po-kroku)
  - [3.0: Pomiar bazowy PC](#30-pomiar-bazowy-pc)
  - [3.5: Obciążenie CPU i GC](#35-obciążenie-cpu-i-gc)
  - [4.0: Koszt renderowania](#40-koszt-renderowania)
  - [4.5: Koszt fizyki](#45-koszt-fizyki)
  - [5.0: Meta Quest 3](#50-meta-quest-3)
- [Zakończenie laboratorium](#zakończenie-laboratorium)
- [Materiały dydaktyczne](#materiały-dydaktyczne)


## Szybki wybór ścieżki
- **Nie mam Unity Hub lub edytora:** przejdź do [instalacji Unity](../../Documentation~/Reference/OS_SETUP.md#unity-nie-jest-zainstalowane), a następnie wróć do [pierwszego eksperymentu](#pierwsze-10-minut-eksperyment-zamiast-instalatora).
- **Unity jest już zainstalowane:** od razu wykonaj [pierwsze 10 minut](#pierwsze-10-minut-eksperyment-zamiast-instalatora). Nie reinstaluj.
- **Nie mam konta/licencji Unity:** wybierz [odpowiedni wariant licencji](../../Documentation~/Reference/UNITY_ACCESS_AND_LICENSES.md#wybierz-jedną-z-trzech-ścieżek).
- **Mam działające konto i licencję:** pomiń formalności i przejdź do [pierwszych 10 minut](#pierwsze-10-minut-eksperyment-zamiast-instalatora).
- **Ubuntu:** [instrukcja instalacji i diagnostyka Ubuntu](../../Documentation~/Reference/OS_SETUP.md#ubuntu).
- **Windows:** [instrukcja instalacji i diagnostyka Windows](../../Documentation~/Reference/OS_SETUP.md#windows).
- **Projekt działa, chcę więcej:** [zaawansowane eksperymenty](#zadania-krok-po-kroku) albo [komfort XR](../../Documentation~/Reference/COMFORT_STUDIES.md).


## Konto Unity: nie potrzebujesz wspólnego loginu

Na komputerze uczelnianym z **aktywną Unity Education Grant License** student może pracować bez indywidualnego logowania. Stanowisko musi wcześniej skonfigurować administrator. **Nie udostępniamy loginu i hasła wspólnego konta Unity.** Na własnym komputerze użyj swojego Unity ID i bezpłatnej licencji Personal; darmowy Student Plan jest rozszerzeniem opcjonalnym.

Przewodnik z krokami i korzyściami dla studentów: [Lab 01: Unity ID i licencje](https://github.com/KIA-students/wirr/blob/main/Documentation~/Reference/UNITY_ACCESS_AND_LICENSES.md). Jego aktualizacja jest przygotowywana w pakiecie roboczym i będzie dostępna publicznie po wydaniu.


Ten folder jest próbką pakietu **WiRR Course Toolkit** przeznaczoną dla Laboratorium 1. Poniższa lista prowadzi przez wykonanie ćwiczenia w tej samej kolejności, którą pokazuje panel `WiRR → Narzędzia kursu`. Instrukcja laboratoryjna rozwija teorię i sposób interpretacji, ale kolejność pracy i wymagane dowody są zebrane tutaj.

## Pierwsze 10 minut: eksperyment zamiast instalatora

**Cel pierwszego doświadczenia:** sprawdzić, jak powstaje obraz sceny 3D, co oznacza 1 metr w Unity i jak zmiana jednego parametru wpływa na wynik.

1. Otwórz gotowy projekt URP z pracowni albo samodzielnie utworzony projekt Unity. **Do pierwszego testu nie instaluj AR Foundation, ROS 2, Meta SDK ani Android Studio.**
2. Otwórz **WiRR → Misje i odkrycia (4 bloki) → Start: Lab 01**. Jeżeli nie ma jeszcze tej wersji, otwórz zwykłą scenę 3D.
3. Dodaj sześcian do sceny w odległości około dwóch metrów od kamery i sprawdź w Play, jak wygląda przy skali 0,5 m, 1 m i 2 m.
4. Porównaj to ze swoją intuicją dotyczącą odległości. Zastanów się, dlaczego błędna skala w VR utrudnia pracę operatora urządzenia.
5. Otwórz **Window → Analysis → Profiler**. Wykonaj trzy krótkie odczyty czasu klatki przed i po dodaniu np. 20 identycznych sześcianów. Wymień tylko jeden czynnik.
6. Zapisz scenę. To już jest wartościowy punkt kontrolny. Nie przechodź do instalacji XR przed zakończeniem pierwszego testu.

**Dla zainteresowanych:** kolejne rozdziały zawierają rozbudowane, oryginalne badania CPU, GPU, GC, fizyki i Quest 3. Są rozszerzeniem po opanowaniu podstaw, a nie listą instalacji wymaganych przed pierwszym Play.

## Przygotowanie dalszych eksperymentów

1. Gdy podstawowy eksperyment działa, otwórz panel zaawansowany `WiRR → Narzędzia kursu` i wybierz Lab 01.
2. Użyj **Zainstaluj / napraw zależności** dopiero jeśli wybierasz interakcje XR lub rzeczywiste gogle.
3. Zaimportuj próbkę WiRR oraz wymagane oficjalne próbki Unity.
4. Użyj **Utwórz / napraw aktywną scenę**.
5. Uruchom **Sprawdź konfigurację laboratorium** i usuń błędy blokujące.
6. Zainstaluj zależności oraz zaimportuj Starter Assets i XR Interaction Simulator.
7. Utwórz lub napraw scenę Lab01, dodaj sondę metryk i uruchom walidator.
8. Przed pomiarem wyłącz zbędne aplikacje i nie zmieniaj kilku parametrów eksperymentu jednocześnie.

## Zadania krok po kroku

### 3.0: Pomiar bazowy PC

1. Uruchom scenę bez dodatkowego obciążenia i sprawdź stabilność FPS oraz czasu klatki.
2. Wykonaj trzy próby wariantu bazowego i zapisuj FPS oraz ms/klatkę; do porównań użyj mediany.
3. Wykonaj wariant v1 z formularza, zmieniając tylko wskazany czynnik.
4. Porównaj warunek kontrolny z eksperymentalnym i zapisz wniosek.

Po zakończeniu tego punktu uzupełnij odpowiadającą mu sekcję formularza **WiRR Reports**. Nie przechodź do kolejnego eksperymentu, dopóki nie zapiszesz warunków pomiaru i wyniku.

### 3.5: Obciążenie CPU i GC

1. Wykonaj CPU-A dla iterationsPerFrame: 0, 10000, 25000, 50000, 100000, 200000, 400000.
2. Wykonaj CPU-B dla blocksPerFrame: 1, 2, 4, 8, 16, 32; po trzy próby na punkt.
3. Wykonaj CPU-C dla allocationBytesPerFrame: 0, 1024, 4096, 16384, 65536, 262144; obserwuj GC Alloc.
4. Zapisz reprezentatywny Main Thread ms i wskaż, kiedy ograniczeniem staje się CPU lub GC.

Po zakończeniu tego punktu uzupełnij odpowiadającą mu sekcję formularza **WiRR Reports**. Nie przechodź do kolejnego eksperymentu, dopóki nie zapiszesz warunków pomiaru i wyniku.

### 4.0: Koszt renderowania

1. Wykonaj GPU-A dla 0, 100, 250, 500, 1000, 2000 i 4000 widocznych obiektów.
2. Wykonaj GPU-B dla 1, 2, 4, 8, 16 i 32 materiałów.
3. Wykonaj serię Render Scale: 0.60, 0.70, 0.80, 0.90, 1.00, 1.20.
4. Porównaj FPS z Batches, SetPass i Triangles i wskaż dominujące wąskie gardło.

Po zakończeniu tego punktu uzupełnij odpowiadającą mu sekcję formularza **WiRR Reports**. Nie przechodź do kolejnego eksperymentu, dopóki nie zapiszesz warunków pomiaru i wyniku.

### 4.5: Koszt fizyki

1. Wykonaj PHY-A dla 0, 25, 50, 100, 200, 400 i 800 dynamicznych Rigidbody.
2. Zapisuj FPS, ms/klatkę i Physics.Processing, nie zmieniając ustawień renderowania.
3. Wykonaj PHY-C dla Fixed Timestep: 0.0333, 0.0250, 0.0200, 0.0167, 0.0133, 0.0111 s.
4. Wyjaśnij kompromis między częstotliwością symulacji a kosztem obliczeń.

Po zakończeniu tego punktu uzupełnij odpowiadającą mu sekcję formularza **WiRR Reports**. Nie przechodź do kolejnego eksperymentu, dopóki nie zapiszesz warunków pomiaru i wyniku.

### 5.0: Meta Quest 3

1. Sprawdź przez Meta Horizon Link obrót/translację głowy, kontrolery i Select; Link traktuj tylko funkcjonalnie.
2. Zbuduj samodzielną aplikację na Quest 3 i uruchom ją bez Link.
3. Wykonaj trzy pomiary samodzielnej aplikacji i zapisz FPS oraz ms/klatkę; zweryfikuj 6DoF.
4. W tych samych możliwie zbliżonych warunkach wykonaj trzy pomiary na PC, policz mediany dla obu platform i porównaj kierunek oraz skalę efektu.

Po zakończeniu tego punktu uzupełnij odpowiadającą mu sekcję formularza **WiRR Reports**. Nie przechodź do kolejnego eksperymentu, dopóki nie zapiszesz warunków pomiaru i wyniku.

## Zakończenie laboratorium

1. Otwórz **WiRR → Raporty → Formularz raportu laboratoryjnego** i wybierz najwyższy kompletny punkt kontrolny.
2. Sprawdź, czy każda tabela zawiera warunki pomiaru, wyniki surowe i krótki wniosek.
3. Użyj **Sprawdź kompletność raportu** i popraw wskazane braki.
4. Wyślij raport dopiero po przejściu walidacji. Checkboxy z panelu zadań są lokalną pomocą i nie są częścią oceny ani raportu.

## Materiały dydaktyczne
Po przygotowaniu sceny możesz użyć sekcji **Scena i pomiary → Materiały dydaktyczne**:

1. **Dodaj środowisko** — tworzy wspólne stanowisko WiRR; w Lab 03–04 zamiast wirtualnego pokoju używany jest lekki zestaw odniesienia AR.
2. **Dodaj zestaw eksperymentalny** — tworzy prefaby właściwe dla bieżącego laboratorium w `Assets/WiRR/Lab01/Prefabs/Generated` i umieszcza ich instancje pod `WiRR_TeachingAssets`.
3. **Usuń obiekty dydaktyczne ze sceny** — usuwa wyłącznie gałąź `WiRR_TeachingAssets`; nie usuwa pracy studenta ani wygenerowanych prefabów.

Materiały są opcjonalne i służą jako kontekst eksperymentu. Nie konfigurują za studenta komponentów stanowiących cel ćwiczenia. Folder `Prefabs/Generated` jest odtwarzalny — własne rozwiązania zapisuj poza nim.

`report-template.md` jest synchronizowany z aktualnym szablonem instrukcji laboratorium.


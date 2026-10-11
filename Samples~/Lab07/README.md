# Lab 07 — Walidacja i testy akceptacyjne

## Spis treści

- [Szybki wybór ścieżki](#szybki-wybór-ścieżki)
- [Przygotowanie](#przygotowanie)
- [Zadania krok po kroku](#zadania-krok-po-kroku)
  - [3.0: Kryteria i smoke test](#30-kryteria-i-smoke-test)
  - [3.5: Wydajność i stabilność](#35-wydajność-i-stabilność)
  - [4.0: Użyteczność i dostępność](#40-użyteczność-i-dostępność)
  - [4.5: Awaria i regresja](#45-awaria-i-regresja)
  - [5.0: Automatyzacja i ryzyko](#50-automatyzacja-i-ryzyko)
- [Zakończenie laboratorium](#zakończenie-laboratorium)
- [Materiały dydaktyczne](#materiały-dydaktyczne)
- [Dodatkowe, dobrowolne doświadczenie: komfort VR i głębia](#dodatkowe-dobrowolne-doświadczenie-komfort-vr-i-głębia)


## Szybki wybór ścieżki
- **Mam działający demonstrator:** zacznij od [kryteriów i smoke testu](#30-kryteria-i-smoke-test).
- **Nie mam fizycznego urządzenia:** wykorzystaj działającą symulację i opisz ograniczenia, zamiast instalować niesprawdzone sterowniki.
- **Chcę sprawdzić komfort użytkownika:** [dobrowolne stanowiska komfortu XR](../../Documentation~/Reference/COMFORT_STUDIES.md).
- **Chcę przetestować awarie:** [kontrolowany błąd i regresja](#45-awaria-i-regresja), bez ryzykownej ekspozycji uczestników.


Ten folder jest próbką pakietu **WiRR Course Toolkit** przeznaczoną dla Laboratorium 7. Poniższa lista prowadzi przez wykonanie ćwiczenia w tej samej kolejności, którą pokazuje panel `WiRR → Narzędzia kursu`. Instrukcja laboratoryjna rozwija teorię i sposób interpretacji, ale kolejność pracy i wymagane dowody są zebrane tutaj.

## Przygotowanie

1. Otwórz `WiRR → Narzędzia kursu` i wybierz Lab 07.
2. Doinstaluj narzędzia testowe tylko, jeśli planujesz test automatyczny. Profiler i podstawowy smoke-test możesz uruchomić bez instalacji nowych pakietów.
3. Zaimportuj próbkę WiRR oraz wymagane oficjalne próbki Unity.
4. Użyj **Utwórz / napraw aktywną scenę**.
5. Uruchom **Sprawdź konfigurację laboratorium** i usuń błędy blokujące.
6. Przygotuj wersję systemu przeznaczoną do testów i nie zmieniaj kryteriów po zobaczeniu wyniku.
7. Otwórz Unity Test Framework i formularz raportu Lab07.
8. Ustal sposób przechowywania dowodów: zrzuty, logi, wyniki pomiarów i identyfikatory testów.
9. Opcjonalnie zaimportuj **Unitree G1 EDU + animacje** jako realistyczny dodatkowy obiekt obciążający scenę; jeśli go używasz, zachowaj identyczny stan modelu i animacji we wszystkich porównywanych warunkach.

## Zadania krok po kroku

### 3.0: Kryteria i smoke test

1. Przed testami zapisz AC-01…AC-06 i oznacz kryteria blokujące.
2. Wykonaj SM-01…SM-05 z wynikiem oczekiwanym, rzeczywistym, PASS/FAIL/NV i dowodem.
3. Uzupełnij macierz funkcjonalną dla Lab 1–6.
4. Niewykonany test oznacz NV, nigdy PASS.

Po zakończeniu tego punktu uzupełnij odpowiadającą mu sekcję formularza **WiRR Reports**. Nie przechodź do kolejnego eksperymentu, dopóki nie zapiszesz warunków pomiaru i wyniku.

### 3.5: Wydajność i stabilność

1. Zdefiniuj stany: bezczynność, nominalny i obciążeniowy.
2. W każdym zmierz FPS, ms/klatkę, CPU ms, GPU ms i pamięć.
3. Zanotuj warunki transportu sieciowego, jeśli występuje.
4. Porównaj wyniki z wcześniej zapisanymi kryteriami akceptacji.

Po zakończeniu tego punktu uzupełnij odpowiadającą mu sekcję formularza **WiRR Reports**. Nie przechodź do kolejnego eksperymentu, dopóki nie zapiszesz warunków pomiaru i wyniku.

### 4.0: Użyteczność i dostępność

1. Zdefiniuj U1, U2 i U3 z jawnym kryterium sukcesu.
2. Zapisuj sukces, czas/próby, błędne aktywacje i liczbę podpowiedzi.
3. Zidentyfikuj co najmniej jeden problem dostępności/interfejsu.
4. Policz skuteczność bez zamiany brakujących danych na wynik pozytywny.

Po zakończeniu tego punktu uzupełnij odpowiadającą mu sekcję formularza **WiRR Reports**. Nie przechodź do kolejnego eksperymentu, dopóki nie zapiszesz warunków pomiaru i wyniku.

### 4.5: Awaria i regresja

1. Wprowadź kontrolowaną awarię v4 i zmierz stan przed poprawką.
2. Zapisz H1/H2, test rozstrzygający i minimalną poprawkę.
3. Wykonaj REG-01 dla naprawionej usterki i REG-02 dla potencjalnej regresji.
4. Zapisz statusy przed/po i dowody.

Po zakończeniu tego punktu uzupełnij odpowiadającą mu sekcję formularza **WiRR Reports**. Nie przechodź do kolejnego eksperymentu, dopóki nie zapiszesz warunków pomiaru i wyniku.

### 5.0: Automatyzacja i ryzyko

1. Zautomatyzuj co najmniej jeden test i pokaż PASS oraz kontrolowany FAIL.
2. Utwórz R-01…R-03, określ P i S oraz policz R=P×S.
3. Zapisz działanie ograniczające i ryzyko resztkowe.
4. Wybierz ACCEPT / ACCEPT WITH CONDITIONS / REJECT wyłącznie na podstawie kryteriów i danych.

Po zakończeniu tego punktu uzupełnij odpowiadającą mu sekcję formularza **WiRR Reports**. Nie przechodź do kolejnego eksperymentu, dopóki nie zapiszesz warunków pomiaru i wyniku.

## Zakończenie laboratorium

1. Otwórz **WiRR → Raporty → Formularz raportu laboratoryjnego** i wybierz najwyższy kompletny punkt kontrolny.
2. Sprawdź, czy każda tabela zawiera warunki pomiaru, wyniki surowe i krótki wniosek.
3. Użyj **Sprawdź raport** i popraw wskazane braki.
4. Wyślij raport dopiero po przejściu walidacji. Checkboxy z panelu zadań są lokalną pomocą i nie są częścią oceny ani raportu.

## Materiały dydaktyczne
Po przygotowaniu sceny możesz użyć sekcji **Scena i pomiary → Materiały dydaktyczne**:

1. **Dodaj środowisko** — tworzy wspólne stanowisko WiRR; w Lab 03–04 zamiast wirtualnego pokoju używany jest lekki zestaw odniesienia AR.
2. **Dodaj zestaw eksperymentalny** — tworzy prefaby właściwe dla bieżącego laboratorium w `Assets/WiRR/Lab07/Prefabs/Generated` i umieszcza ich instancje pod `WiRR_TeachingAssets`.
3. **Usuń obiekty dydaktyczne ze sceny** — usuwa wyłącznie gałąź `WiRR_TeachingAssets`; nie usuwa pracy studenta ani wygenerowanych prefabów.
4. **Importuj Unitree G1 EDU + animacje** — pozwala użyć humanoidalnego modelu jako stałego obciążenia sceny w testach wydajności, stabilności i regresji.

Materiały są opcjonalne i służą jako kontekst eksperymentu. Nie konfigurują za studenta komponentów stanowiących cel ćwiczenia. Folder `Prefabs/Generated` jest odtwarzalny — własne rozwiązania zapisuj poza nim.

`report-template.md` jest synchronizowany z aktualnym szablonem instrukcji laboratorium.


## Dodatkowe, dobrowolne doświadczenie: komfort VR i głębia

Wybierz **WiRR → Komfort XR → Utwórz 3 prefaby: obserwacja i watchdog**. Powstaną prawdziwe prefaby Unity z łagodnym ruchem wskazówek wizualnych (maks. 8 s) i krótkim porównaniem wirtualnej głębi (maks. 10 s). Ich zadaniem nie jest wywoływanie nudności, lecz nauka projektowania komfortowych scen.

Instrukcja pełna: **Documentation~/Reference/COMFORT_STUDIES.md**. Przy pracy w goglach potrzebny jest świadomy ochotnik, osoba obserwująca i możliwość natychmiastowego przerwania. Reszta grupy może wykonać równoważną analizę na monitorze. **Nie przyznaje się punktów za fizjologiczną odporność ani wystąpienie objawów.**

## Wyzwanie dodatkowe: operator potrafi bezpiecznie przerwać

Po zrealizowaniu podstawowego doświadczenia sprawdź [automatyczną detekcję zdjęcia gogli, utraty śledzenia i skoku danych](../../Documentation~/Reference/XR_SAFETY_WATCHDOG.md). Możesz wykonać do 10 krótkich prób technicznych, także wyłącznie na PC. **Nie wykonuj nagłych ruchów głową dla punktów.** Punktuje się poprawność prototypu i interpretacji, a nie podatność na objawy.

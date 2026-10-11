# Lab 02 — Interakcja i manipulacja XR

## Spis treści

- [Szybki wybór ścieżki](#szybki-wybór-ścieżki)
- [Przygotowanie](#przygotowanie)
- [Zadania krok po kroku](#zadania-krok-po-kroku)
  - [3.0: Chwyt, socket i ray](#30-chwyt-socket-i-ray)
  - [3.5: UI i feedback](#35-ui-i-feedback)
  - [4.0: Lokomocja](#40-lokomocja)
  - [4.5: Scenariusz i diagnostyka](#45-scenariusz-i-diagnostyka)
  - [5.0: Quest 3 i SSQ](#50-quest-3-i-ssq)
- [Zakończenie laboratorium](#zakończenie-laboratorium)
- [Materiały dydaktyczne](#materiały-dydaktyczne)
- [Dodatkowe, dobrowolne doświadczenie: komfort VR i głębia](#dodatkowe-dobrowolne-doświadczenie-komfort-vr-i-głębia)


## Szybki wybór ścieżki
- **Nie mam gotowej sceny Lab 01:** [wróć do pierwszej sceny](../Lab01/README.md#pierwsze-10-minut-eksperyment-zamiast-instalatora).
- **Mam działający projekt:** [od razu przejdź do interakcji](#zadania-krok-po-kroku).
- **Brakuje XRI:** doinstaluj tylko pakiety używane w tym wariancie, zobacz [warunki startu](../../Documentation~/Reference/OS_SETUP.md#pakiety-xr-tylko-gdy-potrzebne).
- **Chcę przejść do rzeczywistego sprzętu:** najpierw [uruchomienie urządzeń](../../Documentation~/Reference/OS_SETUP.md#urzadzenia-xr), potem [Quest 3 i SSQ](#50-quest-3-i-ssq).
- **Chcę poznać ograniczenia percepcji:** [Komfort XR](../../Documentation~/Reference/COMFORT_STUDIES.md).


Ten folder jest próbką pakietu **WiRR Course Toolkit** przeznaczoną dla Laboratorium 2. Poniższa lista prowadzi przez wykonanie ćwiczenia w tej samej kolejności, którą pokazuje panel `WiRR → Narzędzia kursu`. Instrukcja laboratoryjna rozwija teorię i sposób interpretacji, ale kolejność pracy i wymagane dowody są zebrane tutaj.

## Przygotowanie

1. W **WiRR → Misje i odkrycia** wybierz **Start: Lab 02**, aby natychmiast otworzyć scenę PC. Panel zaawansowany służy dopiero do integracji XR.
2. Zależności XRI instaluj tylko wtedy, kiedy wykonujesz chwyt kontrolerem, teleportację lub pracujesz w symulatorze XR.
3. Zaimportuj próbkę WiRR oraz wymagane oficjalne próbki Unity.
4. Użyj **Utwórz / napraw aktywną scenę**.
5. Uruchom **Sprawdź konfigurację laboratorium** i usuń błędy blokujące.
6. Zainstaluj zależności oraz zaimportuj Starter Assets i XR Interaction Simulator.
7. Utwórz scenę Lab02 i zweryfikuj XR Origin, Input Actions oraz warstwy interakcji.
8. Przygotuj prosty scenariusz z obiektem chwytalnym, gniazdem, ray, UI i obszarem teleportacji.

## Zadania krok po kroku

### 3.0: Chwyt, socket i ray

1. Skonfiguruj XR Grab Interactable na obiekcie chwytalnym.
2. Dodaj XR Socket Interactor i ogranicz go do właściwego obiektu/warstwy.
3. Skonfiguruj XR Ray Interactor i potwierdź niezależne działanie chwytu, socketu i ray.
4. Wykonaj po pięć prób A/B/C; zapisuj czas i liczbę błędów.

Po zakończeniu tego punktu uzupełnij odpowiadającą mu sekcję formularza **WiRR Reports**. Nie przechodź do kolejnego eksperymentu, dopóki nie zapiszesz warunków pomiaru i wyniku.

### 3.5: UI i feedback

1. Utwórz panel World Space i skonfiguruj obsługę UI dla XR.
2. Dodaj co najmniej dwa rodzaje feedbacku: wizualny, dźwiękowy lub haptyczny.
3. Sprawdź stany `Hover`/`Select` oraz zakończenie oraz usuń wielokrotne aktywacje jednego działania.
4. Wykonaj po pięć prób A/B/C i zapisz czas oraz błędne aktywacje.

Po zakończeniu tego punktu uzupełnij odpowiadającą mu sekcję formularza **WiRR Reports**. Nie przechodź do kolejnego eksperymentu, dopóki nie zapiszesz warunków pomiaru i wyniku.

### 4.0: Lokomocja

1. Skonfiguruj teleportację wyłącznie na dozwolone powierzchnie.
2. Sprawdź reticle, orientację po teleportacji i kolizję użytkownika z otoczeniem.
3. Wykonaj po pięć prób A/B/C, zapisując czas oraz błędy/korekty.
4. Zapisz, które elementy konfiguracji najbardziej wpływają na przewidywalność i komfort.

Po zakończeniu tego punktu uzupełnij odpowiadającą mu sekcję formularza **WiRR Reports**. Nie przechodź do kolejnego eksperymentu, dopóki nie zapiszesz warunków pomiaru i wyniku.

### 4.5: Scenariusz i diagnostyka

1. Połącz lokomocję, wskazanie, chwyt, socket i UI w jeden scenariusz.
2. Wykonaj pięć prób i rejestruj czas, upuszczenia, błędne aktywacje i nieudane teleportacje.
3. Wprowadź kontrolowany problem v4, zapisz H1/H2 i wykonaj test rozstrzygający.
4. Zastosuj minimalną poprawkę i powtórz scenariusz, sprawdzając regresje.

Po zakończeniu tego punktu uzupełnij odpowiadającą mu sekcję formularza **WiRR Reports**. Nie przechodź do kolejnego eksperymentu, dopóki nie zapiszesz warunków pomiaru i wyniku.

### 5.0: Quest 3 i SSQ

1. Wypełnij SSQ PRE dla obu osób przed ekspozycją.
2. Uruchom scenariusz samodzielnej aplikacji na Quest 3 i zweryfikuj interakcję oraz lokomocję.
3. Wykonaj A/B/C na wskazanej osobie po trzy próby, zapisując czas, błędy i komfort 0–10.
4. Wypełnij SSQ POST i sformułuj wniosek ograniczony do tej sesji.

Po zakończeniu tego punktu uzupełnij odpowiadającą mu sekcję formularza **WiRR Reports**. Nie przechodź do kolejnego eksperymentu, dopóki nie zapiszesz warunków pomiaru i wyniku.

## Zakończenie laboratorium

1. Otwórz **WiRR → Raporty → Formularz raportu laboratoryjnego** i wybierz najwyższy kompletny punkt kontrolny.
2. Sprawdź, czy każda tabela zawiera warunki pomiaru, wyniki surowe i krótki wniosek.
3. Użyj **Sprawdź raport** i popraw wskazane braki.
4. Wyślij raport dopiero po przejściu walidacji. Checkboxy z panelu zadań są lokalną pomocą i nie są częścią oceny ani raportu.

## Materiały dydaktyczne
Po przygotowaniu sceny możesz użyć sekcji **Scena i pomiary → Materiały dydaktyczne**:

1. **Dodaj środowisko** — tworzy wspólne stanowisko WiRR; w Lab 03–04 zamiast wirtualnego pokoju używany jest lekki zestaw odniesienia AR.
2. **Dodaj zestaw eksperymentalny** — tworzy prefaby właściwe dla bieżącego laboratorium w `Assets/WiRR/Lab02/Prefabs/Generated` i umieszcza ich instancje pod `WiRR_TeachingAssets`.
3. **Usuń obiekty dydaktyczne ze sceny** — usuwa wyłącznie gałąź `WiRR_TeachingAssets`; nie usuwa pracy studenta ani wygenerowanych prefabów.

Materiały są opcjonalne i służą jako kontekst eksperymentu. Nie konfigurują za studenta komponentów stanowiących cel ćwiczenia. Folder `Prefabs/Generated` jest odtwarzalny — własne rozwiązania zapisuj poza nim.

`report-template.md` jest synchronizowany z aktualnym szablonem instrukcji laboratorium.


## Dodatkowe, dobrowolne doświadczenie: komfort VR i głębia

Wybierz **WiRR → Komfort XR → Utwórz 3 prefaby: obserwacja i watchdog**. Powstaną prawdziwe prefaby Unity z łagodnym ruchem wskazówek wizualnych (maks. 8 s) i krótkim porównaniem wirtualnej głębi (maks. 10 s). Ich zadaniem nie jest wywoływanie nudności, lecz nauka projektowania komfortowych scen.

Instrukcja pełna: **Documentation~/Reference/COMFORT_STUDIES.md**. Przy pracy w goglach potrzebny jest świadomy ochotnik, osoba obserwująca i możliwość natychmiastowego przerwania. Reszta grupy może wykonać równoważną analizę na monitorze. **Nie przyznaje się punktów za fizjologiczną odporność ani wystąpienie objawów.**

## Wyzwanie dodatkowe: operator potrafi bezpiecznie przerwać

Po zrealizowaniu podstawowego doświadczenia sprawdź [automatyczną detekcję zdjęcia gogli, utraty śledzenia i skoku danych](../../Documentation~/Reference/XR_SAFETY_WATCHDOG.md). Możesz wykonać do 10 krótkich prób technicznych, także wyłącznie na PC. **Nie wykonuj nagłych ruchów głową dla punktów.** Punktuje się poprawność prototypu i interpretacji, a nie podatność na objawy.

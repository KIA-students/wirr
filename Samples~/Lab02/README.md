# Lab 02 — Interakcja i manipulacja XR

Ten folder jest próbką pakietu **WiRR Course Toolkit** przeznaczoną dla Laboratorium 2. Poniższa lista prowadzi przez wykonanie ćwiczenia w tej samej kolejności, którą pokazuje panel `WiRR → Narzędzia kursu`. Instrukcja laboratoryjna rozwija teorię i sposób interpretacji, ale kolejność pracy i wymagane dowody są zebrane tutaj.

## Przygotowanie

1. Otwórz `WiRR → Narzędzia kursu` i wybierz Lab 02.
2. Użyj **Zainstaluj / napraw zależności**.
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
3. Sprawdź hover/select/zakończenie oraz usuń wielokrotne aktywacje jednego działania.
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
2. Uruchom scenariusz standalone na Quest 3 i zweryfikuj interakcję oraz lokomocję.
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

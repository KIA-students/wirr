# Lab 03 — Rejestracja i kotwice AR

Ten folder jest próbką pakietu **WiRR Course Toolkit** przeznaczoną dla Laboratorium 3. Poniższa lista prowadzi przez wykonanie ćwiczenia w tej samej kolejności, którą pokazuje panel `WiRR → Narzędzia kursu`. Instrukcja laboratoryjna rozwija teorię i sposób interpretacji, ale kolejność pracy i wymagane dowody są zebrane tutaj.

## Przygotowanie

1. Otwórz `WiRR → Narzędzia kursu` i wybierz Lab 03.
2. Użyj **Zainstaluj / napraw zależności**.
3. Zaimportuj próbkę WiRR oraz wymagane oficjalne próbki Unity.
4. Użyj **Utwórz / napraw aktywną scenę**.
5. Uruchom **Sprawdź konfigurację laboratorium** i usuń błędy blokujące.
6. Zainstaluj AR Foundation/ARCore oraz wymagane próbki.
7. Utwórz scenę Lab03 i zweryfikuj AR Session, XR Origin i wykrywanie płaszczyzn.
8. Przygotuj fizyczne punkty odniesienia oraz model o znanych wymiarach.

## Zadania krok po kroku

### 3.0: Tracking i płaszczyzny

1. Uruchom aplikację na urządzeniu i zmierz czas do SessionTracking.
2. Zmierz czas do pierwszej użytecznej płaszczyzny.
3. Powtórz po trzy razy dla warunków A/B/C; zapisuj notTrackingReason.
4. Opisz warunki otoczenia i ograniczenia pomiaru.

Po zakończeniu tego punktu uzupełnij odpowiadającą mu sekcję formularza **WiRR Reports**. Nie przechodź do kolejnego eksperymentu, dopóki nie zapiszesz warunków pomiaru i wyniku.

### 3.5: Raycast i skala 1:1

1. Ustaw model w jednostkach metrycznych i zachowaj skalę 1:1.
2. Umieszczaj model wyłącznie po poprawnym raycast do płaszczyzny.
3. Dla A/B/C wykonaj po 10 prób i policz poprawne trafienia.
4. Zmierz medianę błędu pozycjonowania bez korygowania go skalą modelu.

Po zakończeniu tego punktu uzupełnij odpowiadającą mu sekcję formularza **WiRR Reports**. Nie przechodź do kolejnego eksperymentu, dopóki nie zapiszesz warunków pomiaru i wyniku.

### 4.0: Kotwica i dryf

1. Utwórz kotwicę przy fizycznym punkcie referencyjnym.
2. Przejdź zadaną ścieżkę, wróć i zmierz odchylenie od znacznika.
3. Wykonaj trzy powtórzenia baseline oraz serię A/B/C, zapisując tracking.
4. Policz medianę dryfu i określ charakter błędu.

Po zakończeniu tego punktu uzupełnij odpowiadającą mu sekcję formularza **WiRR Reports**. Nie przechodź do kolejnego eksperymentu, dopóki nie zapiszesz warunków pomiaru i wyniku.

### 4.5: Rejestracja dwupunktowa

1. Wyznacz fizyczne punkty O i X oraz niezależnie zmierz OX.
2. Zarejestruj układ na O i X i zmierz e1, e2, e3.
3. Powtórz pełną rejestrację trzy razy; policz e_med i e_max.
4. Wskaż główne źródło niepewności i sposób jego ograniczenia.

Po zakończeniu tego punktu uzupełnij odpowiadającą mu sekcję formularza **WiRR Reports**. Nie przechodź do kolejnego eksperymentu, dopóki nie zapiszesz warunków pomiaru i wyniku.

### 5.0: Kontrolowany błąd

1. Wykonaj trzy pomiary baseline.
2. Wprowadź błąd v5 i zapisz H1/H2.
3. Wykonaj test rozstrzygający i trzy pomiary ze stanem błędnym.
4. Napraw minimalnie, wykonaj trzy pomiary i sprawdź powrót e_med do baseline.

Po zakończeniu tego punktu uzupełnij odpowiadającą mu sekcję formularza **WiRR Reports**. Nie przechodź do kolejnego eksperymentu, dopóki nie zapiszesz warunków pomiaru i wyniku.

## Zakończenie laboratorium

1. Otwórz **WiRR → Raporty → Formularz raportu laboratoryjnego** i wybierz najwyższy kompletny punkt kontrolny.
2. Sprawdź, czy każda tabela zawiera warunki pomiaru, wyniki surowe i krótki wniosek.
3. Użyj **Sprawdź raport** i popraw wskazane braki.
4. Wyślij raport dopiero po przejściu walidacji. Checkboxy z panelu zadań są lokalną pomocą i nie są częścią oceny ani raportu.

## Materiały dydaktyczne
Po przygotowaniu sceny możesz użyć sekcji **Scena i pomiary → Materiały dydaktyczne**:

1. **Dodaj środowisko** — tworzy wspólne stanowisko WiRR; w Lab 03–04 zamiast wirtualnego pokoju używany jest lekki zestaw odniesienia AR.
2. **Dodaj zestaw eksperymentalny** — tworzy prefaby właściwe dla bieżącego laboratorium w `Assets/WiRR/Lab03/Prefabs/Generated` i umieszcza ich instancje pod `WiRR_TeachingAssets`.
3. **Usuń obiekty dydaktyczne ze sceny** — usuwa wyłącznie gałąź `WiRR_TeachingAssets`; nie usuwa pracy studenta ani wygenerowanych prefabów.

Materiały są opcjonalne i służą jako kontekst eksperymentu. Nie konfigurują za studenta komponentów stanowiących cel ćwiczenia. Folder `Prefabs/Generated` jest odtwarzalny — własne rozwiązania zapisuj poza nim.

`report-template.md` jest synchronizowany z aktualnym szablonem instrukcji laboratorium.

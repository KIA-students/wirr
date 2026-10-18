# Lab 04 — Rozumienie sceny i mieszanie rzeczywistości

Instrukcja kursowa: WiRR Lab 04 v1.0.3. Unity 6000.6.5f1 lub nowsza zgodna poprawka z linii 6000.6.x.

## Spis treści

- [Szybki wybór ścieżki](#szybki-wybór-ścieżki)
- [Przygotowanie](#przygotowanie)
- [Zadania krok po kroku](#zadania-krok-po-kroku)
  - [3.0: API głębi (Depth API)](#30-api-głębi-depth-api)
  - [3.5: Okluzja](#35-okluzja)
  - [4.0: Rzutowanie promienia z użyciem danych głębi](#40-rzutowanie-promienia-z-użyciem-danych-głębi)
  - [4.5: Estymacja oświetlenia](#45-estymacja-oświetlenia)
  - [5.0: Kontrolowany błąd](#50-kontrolowany-błąd)
- [Zakończenie laboratorium](#zakończenie-laboratorium)
- [Materiały dydaktyczne](#materiały-dydaktyczne)


## Szybki wybór ścieżki
- **Mam działającą scenę AR z Lab 03:** przejdź wprost do [Depth API i okluzji](#30-api-głębi-depth-api).
- **Nie mam działającego AR:** [wróć do śledzenia i płaszczyzn w Lab 03](../../Samples~/Lab03/README.md#30-śledzenie-i-płaszczyzny), zamiast reinstalować całość.
- **Telefon nie wspiera Depth API:** użyj wariantu symulacyjnego i [przejdź do eksperymentu okluzji](#35-okluzja), jasno oznaczając brak pomiaru sprzętowego.


Ten folder jest próbką pakietu **WiRR Course Toolkit** przeznaczoną dla Laboratorium 4. Poniższa lista prowadzi przez wykonanie ćwiczenia w tej samej kolejności, którą pokazuje panel `WiRR → Narzędzia kursu`. Instrukcja laboratoryjna rozwija teorię i sposób interpretacji, ale kolejność pracy i wymagane dowody są zebrane tutaj.

## Przygotowanie

1. Otwórz `WiRR → Narzędzia kursu` i wybierz Lab 04.
2. Użyj **Zainstaluj / napraw zależności**.
3. Zaimportuj próbkę WiRR oraz wymagane oficjalne próbki Unity.
4. Użyj **Utwórz / napraw aktywną scenę**.
5. Uruchom **Sprawdź konfigurację laboratorium** i usuń błędy blokujące.
6. Zainstaluj AR Foundation/ARCore oraz AR Starter Assets.
7. Utwórz scenę Lab04 i skonfiguruj AR Occlusion Manager.
8. Przygotuj realne przeszkody, powierzchnię referencyjną i kilka kontrolowanych warunków oświetlenia.

## Zadania krok po kroku

### 3.0: API głębi (Depth API)

1. Sprawdź dostępność danych głębi, bieżący tryb i rozdzielczość ramki.
2. Zdefiniuj mierzalny warunek sukcesu dla danych głębi.
3. Wykonaj v1 w A/B/C po trzy powtórzenia i zapisz successRate.
4. Brak danych głębi oznacz jako niedostępność, a nie jako wartość zero.

Po zakończeniu tego punktu uzupełnij odpowiadającą mu sekcję formularza **WiRR Reports**. Nie przechodź do kolejnego eksperymentu, dopóki nie zapiszesz warunków pomiaru i wyniku.

### 3.5: Okluzja

1. Umieść obiekt wirtualny częściowo za rzeczywistą przeszkodą.
2. Porównaj Off, Raw i Smoothed po 10 obserwacji.
3. Klasyfikuj przeciek, nadmierne wycięcie, migotanie i opóźnienie.
4. Wykonaj v2 A/B/C i porównaj błędy /10 oraz dominującą sygnaturę.

Po zakończeniu tego punktu uzupełnij odpowiadającą mu sekcję formularza **WiRR Reports**. Nie przechodź do kolejnego eksperymentu, dopóki nie zapiszesz warunków pomiaru i wyniku.

### 4.0: Rzutowanie promienia z użyciem danych głębi

1. Ustaw powierzchnię kolejno w 0.50, 1.00 i 1.50 m.
2. Wykonaj po pięć prób na odległość i oznacz HIT/MISS.
3. Dla HIT policz błąd odległości; MISS pozostaw jako brak wyniku.
4. Wykonaj v3 A/B/C i porównaj HIT/MISS oraz medianę błędu.

Po zakończeniu tego punktu uzupełnij odpowiadającą mu sekcję formularza **WiRR Reports**. Nie przechodź do kolejnego eksperymentu, dopóki nie zapiszesz warunków pomiaru i wyniku.

### 4.5: Estymacja oświetlenia

1. Sprawdź dostępność brightness, color temperature i main light direction.
2. Zmierz pięć warunków: jasne rozproszone, słabsze rozproszone, boczne, przednie i mieszane.
3. Zapisz mediany, tryb API i ocenę zgodności wizualnej 1–5.
4. Niedostępne pola zapisz jako NA i wyjaśnij ograniczenie.

Po zakończeniu tego punktu uzupełnij odpowiadającą mu sekcję formularza **WiRR Reports**. Nie przechodź do kolejnego eksperymentu, dopóki nie zapiszesz warunków pomiaru i wyniku.

### 5.0: Kontrolowany błąd

1. Wykonaj pomiar wariantu bazowego dla wybranej mierzalnej cechy.
2. Wprowadź v5 i zapisz H1/H2.
3. Wykonaj test rozstrzygający oraz trzy powtórzenia.
4. Zastosuj minimalną poprawkę i wykonaj trzy pomiary po naprawie.

Po zakończeniu tego punktu uzupełnij odpowiadającą mu sekcję formularza **WiRR Reports**. Nie przechodź do kolejnego eksperymentu, dopóki nie zapiszesz warunków pomiaru i wyniku.

## Zakończenie laboratorium

1. Otwórz **WiRR → Raporty → Formularz raportu laboratoryjnego** i wybierz najwyższy kompletny punkt kontrolny.
2. Sprawdź, czy każda tabela zawiera warunki pomiaru, wyniki surowe i krótki wniosek.
3. Użyj **Sprawdź kompletność raportu** i popraw wskazane braki.
4. Wyślij raport dopiero po przejściu walidacji. Checkboxy z panelu zadań są lokalną pomocą i nie są częścią oceny ani raportu.

## Materiały dydaktyczne
Po przygotowaniu sceny możesz użyć sekcji **Scena i pomiary → Materiały dydaktyczne**:

1. **Dodaj środowisko** — tworzy wspólne stanowisko WiRR; w Lab 03–04 zamiast wirtualnego pokoju używany jest lekki zestaw odniesienia AR.
2. **Dodaj zestaw eksperymentalny** — tworzy prefaby właściwe dla bieżącego laboratorium w `Assets/WiRR/Lab04/Prefabs/Generated` i umieszcza ich instancje pod `WiRR_TeachingAssets`.
3. **Usuń obiekty dydaktyczne ze sceny** — usuwa wyłącznie gałąź `WiRR_TeachingAssets`; nie usuwa pracy studenta ani wygenerowanych prefabów.

Materiały są opcjonalne i służą jako kontekst eksperymentu. Nie konfigurują za studenta komponentów stanowiących cel ćwiczenia. Folder `Prefabs/Generated` jest odtwarzalny — własne rozwiązania zapisuj poza nim.

`report-template.md` pozostaje formatem referencyjnym i awaryjnym.


<a id="laboratorium-04-rozumienie-sceny-ar"></a>
<a id="najpierw-wybierz-ścieżkę"></a>
<a id="procedura"></a>
<a id="wymagania"></a>

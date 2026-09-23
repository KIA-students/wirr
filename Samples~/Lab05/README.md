# Lab 05 — Optymalizacja modeli CAD

Ten folder jest próbką pakietu **WiRR Course Toolkit** przeznaczoną dla Laboratorium 5. Poniższa lista prowadzi przez wykonanie ćwiczenia w tej samej kolejności, którą pokazuje panel `WiRR → Narzędzia kursu`. Instrukcja laboratoryjna rozwija teorię i sposób interpretacji, ale kolejność pracy i wymagane dowody są zebrane tutaj.

## Przygotowanie

1. Otwórz `WiRR → Narzędzia kursu` i wybierz Lab 05.
2. Użyj **Zainstaluj / napraw zależności**.
3. Zaimportuj próbkę WiRR oraz wymagane oficjalne próbki Unity.
4. Użyj **Utwórz / napraw aktywną scenę**.
5. Uruchom **Sprawdź konfigurację laboratorium** i usuń błędy blokujące.
6. Przygotuj model CAD i co najmniej dwa alternatywne formaty/ustawienia eksportu.
7. Utwórz scenę Lab05 i geometrię referencyjną do kontroli skali.
8. Wszystkie benchmarki wykonuj w tych samych warunkach Editor/Build i na tej samej kamerze.

## Zadania krok po kroku

### 3.0: Import i audyt CAD

1. Zapisz format źródłowy, łańcuch konwersji i utracone/zachowane informacje.
2. Sprawdź jednostki, skalę i wymiary po imporcie do Unity.
3. Zmierz vertices, triangles, submeshes, materiały, boundary, non-manifold, degenerate, q05/qMedian.
4. Porównaj eksporty A/B/C pod względem rozmiaru, UV, hierarchii, materiałów i wymiarów.

Po zakończeniu tego punktu uzupełnij odpowiadającą mu sekcję formularza **WiRR Reports**. Nie przechodź do kolejnego eksperymentu, dopóki nie zapiszesz warunków pomiaru i wyniku.

### 3.5: Triangulacja i LOD

1. Przygotuj trzy warianty triangulacji/redukcji A/B/C.
2. Zmierz topologię i jakość każdego wariantu.
3. Sprawdź zachowanie cech geometrycznych ważnych dla interakcji.
4. Wybierz kandydatów LOD0/LOD1/LOD2 i zapisz ich liczbę trójkątów.

Po zakończeniu tego punktu uzupełnij odpowiadającą mu sekcję formularza **WiRR Reports**. Nie przechodź do kolejnego eksperymentu, dopóki nie zapiszesz warunków pomiaru i wyniku.

### 4.0: LOD i benchmark

1. Skonfiguruj LOD Group i sprawdź przejścia między poziomami.
2. Wykonaj po trzy próby benchmarku dla A/B/C.
3. Zapisz mean/median/p95, FPS, triangles, renderers, material slots, CPU/GPU ms, batches i pamięć.
4. Wskaż punkt, w którym dalsza redukcja geometrii nie daje proporcjonalnego zysku.

Po zakończeniu tego punktu uzupełnij odpowiadającą mu sekcję formularza **WiRR Reports**. Nie przechodź do kolejnego eksperymentu, dopóki nie zapiszesz warunków pomiaru i wyniku.

### 4.5: Materiały, draw calls i kolizje

1. Porównaj A/B/C pod względem materiałów, draw calls, CPU/GPU i pamięci.
2. Wykonaj co najmniej 20 prób kolizji i policz błędy funkcjonalne.
3. Zidentyfikuj mierzalny problem geometrii/importu i zapisz wynik przed poprawką.
4. Zastosuj minimalną naprawę i potwierdź wynik po poprawce.

Po zakończeniu tego punktu uzupełnij odpowiadającą mu sekcję formularza **WiRR Reports**. Nie przechodź do kolejnego eksperymentu, dopóki nie zapiszesz warunków pomiaru i wyniku.

### 5.0: Pipeline finalny

1. Wykonaj baseline/fault/repaired dla błędu v5.
2. Porównaj STEP→DCC→FBX/glTF, USD, bezpośrednią siatkę lub inny uzasadniony pipeline.
3. Wskaż najważniejsze ryzyko wybranego pipeline.
4. Wybierz finalny model XR i podsumuj kompromis jakość–wydajność–pamięć.

Po zakończeniu tego punktu uzupełnij odpowiadającą mu sekcję formularza **WiRR Reports**. Nie przechodź do kolejnego eksperymentu, dopóki nie zapiszesz warunków pomiaru i wyniku.

## Zakończenie laboratorium

1. Otwórz **WiRR → Raporty → Formularz raportu laboratoryjnego** i wybierz najwyższy kompletny punkt kontrolny.
2. Sprawdź, czy każda tabela zawiera warunki pomiaru, wyniki surowe i krótki wniosek.
3. Użyj **Sprawdź raport** i popraw wskazane braki.
4. Wyślij raport dopiero po przejściu walidacji. Checkboxy z panelu zadań są lokalną pomocą i nie są częścią oceny ani raportu.

## Materiały dydaktyczne
Po przygotowaniu sceny możesz użyć sekcji **Scena i pomiary → Materiały dydaktyczne**:

1. **Dodaj środowisko** — tworzy wspólne stanowisko WiRR; w Lab 03–04 zamiast wirtualnego pokoju używany jest lekki zestaw odniesienia AR.
2. **Dodaj zestaw eksperymentalny** — tworzy prefaby właściwe dla bieżącego laboratorium w `Assets/WiRR/Lab05/Prefabs/Generated` i umieszcza ich instancje pod `WiRR_TeachingAssets`.
3. **Usuń obiekty dydaktyczne ze sceny** — usuwa wyłącznie gałąź `WiRR_TeachingAssets`; nie usuwa pracy studenta ani wygenerowanych prefabów.

Materiały są opcjonalne i służą jako kontekst eksperymentu. Nie konfigurują za studenta komponentów stanowiących cel ćwiczenia. Folder `Prefabs/Generated` jest odtwarzalny — własne rozwiązania zapisuj poza nim.

`report-template.md` jest synchronizowany z aktualnym szablonem instrukcji laboratorium.

# Macierz oceniania

## Jak używać

Każde laboratorium ma swoją macierz oceniania. Przejdź przez kryteria w kolejności:

1. **Smoke Test** — czy system w ogóle się ładuje?
2. **Punkt kontrolny 3.0** — czy procedury są zrobione?
3. **Pomiary** — czy dane są przybliżone?
4. **Diagnoza** — czy błąd znaleziony + naprawiony?
5. **Raport wykonania** — czy raport wypełniony?

---

## Lab 1 — XR Origin, audyt, wydajność

| Kryterium | Tak (1 pkt) | Nie (0 pkt) | Uwagi |
|-----------|------------|-----------|-------|
| **Smoke Test** | Aplikacja startuje bez RED errors | Aplikacja crashuje lub ma RED errors | Obowiązkowe |
| **XR Origin** | Position (0,0,0), Scale (1,1,1) | Coś innego | Sprawdź Transform |
| **Pomiar wydajności** | FPS zmierzony, co 300 klatek log | Brak logu albo błędy | Powinien być widoczny w konsoli |
| **Audyt** | Poligony, Draw Calls, shader cost | Brak pomiaru | Min. dwa pomiary |
| **Diagnoza** | Błąd znaleziony, naprawa wdrożona | Bez diagnozy | Niezbędna dla 3.0 |
| **Raport wykonania** | Wypeł., wszystkie tabele | Puste lub niekompletne | Markdown format |

**Punkt kontrolny 3.0:** >= 5/6 kryteriów  
**Punkt kontrolny 3.5:** 6/6 + dodatkowe pomiary (np. profil pamięci)  
**Punkt kontrolny 4.0:** 6/6 + analiza wąskiego gardła (CPU vs GPU)  
**Punkt kontrolny 4.5:** 6/6 + porównanie z linią bazową  
**Punkt kontrolny 5.0:** 6/6 + automatyzacja testów + raport produkcyjny

---

## Lab 2 — Interakcja i lokomocja

| Kryterium | Tak (1 pkt) | Nie (0 pkt) | Uwagi |
|-----------|------------|-----------|-------|
| **Chwyt** | Direct Grab + Socket Interactor | Jedno z nich nie działa | Demo: chwycić moduł |
| **Panel UI** | Ray Interactor + UI kanvas | Panel nie reaguje na ray | Demo: kliknąć przycisk |
| **Lokomocja** | Teleportacja + Snap Turn | Jedno z nich nie działa | Demo: teleport + obrót |
| **Pomiary** | 3 próby, czasy zarejestrowane | Brak prób albo czasy != log | Mediana ma sens? |
| **Błędy** | Liczba błędów zarejestrowana | Brak logu | LAB02_RESULT |
| **Raport wykonania** | Wypeł., wnioski | Puste | Min. 2 zdania |

**Punkt kontrolny 3.0:** >= 5/6 kryteriów  
**Punkt kontrolny 4.0:** 6/6 + UI sprzężenie zwrotne (dźwięk/wibracja)

---

## Lab 3 — Rejestracja AR

| Kryterium | Tak (1 pkt) | Nie (0 pkt) | Uwagi |
|-----------|------------|-----------|-------|
| **Skanowanie** | ARPlaneManager aktywny | Nie skanuje płaszczyzn | Android + ARCore |
| **Rzutowanie promienia** | Umieszczanie modelu na płaszczyźnie | Model nie da się umieścić | Demo |
| **Rejestracja** | 2-punktowa wyrównana | Niezalignowana | Punkt O i X |
| **Pomiary** | 5 pomiarów błędu | < 5 pomiarów | Różne dystanse |
| **Trend** | Czy błąd rośnie czy stały? | Nie wiadomo | Wniosek z danych |
| **Raport wykonania** | Wypeł., tabela pomiarów | Puste | REGISTRATION_ERROR |

**Punkt kontrolny 3.0:** >= 5/6 kryteriów  
**Punkt kontrolny 4.0:** 6/6 + segmentacja (osoba vs tło)

---

## Lab 4 — Mieszanie Rzeczywistości

| Kryterium | Tak (1 pkt) | Nie (0 pkt) | Uwagi |
|-----------|------------|-----------|-------|
| **API głębi** | Aktywna, zwraca dane | Null albo błąd | ARCore >= 1.40 |
| **Okluzja** | Model okluzowany rzeczywistością | Zawsze widoczny | Demo: przesłonić ręką |
| **Light Estimation** | Zmienia się z oświetleniem | Stałe | Demo: zmienić oświetlenie |
| **Wiarygodność** | 5 testów oświetleniowych | < 5 testów | Średnia ocena |
| **Pomiary** | Temperatura barwna + błąd | Brak danych | Luksomierz opcjonalnie |
| **Raport wykonania** | Wypeł., wnioski | Puste | logi LIGHT |

**Punkt kontrolny 3.0:** >= 4/6 kryteriów  
**Punkt kontrolny 4.0:** >= 5/6 + analiza błędu

---

## Lab 5 — Optymalizacja CAD

| Kryterium | Tak (1 pkt) | Nie (0 pkt) | Uwagi |
|-----------|------------|-----------|-------|
| **Import** | Model wczytany, skalowanie OK | Błąd skali | Scale Factor = 0.01 |
| **Hierarchia** | LOD Group ustawiona | Brak lub źle | Widoczna w Hierarchy |
| **Pomiar wydajności** | Tabela: wariant bazowy, LOD0, LOD1 | Brakuje danych | Min. 3 poziomy |
| **Poligony** | Zmniejszają się między LOD | Takie same | Redukcja >= 20% |
| **FPS** | Wzrost >= 20% (LOD vs wariant bazowy) | Mniej niż 20% | Lub uzasadnienie |
| **Raport wykonania** | Wypeł., analiza | Puste | LAB05_BENCHMARK |

**Punkt kontrolny 3.0:** >= 4/6 kryteriów  
**Punkt kontrolny 4.0:** >= 5/6 + identyfikacja bottleneck

---

## Lab 6 — Bliźniak Cyfrowy ROS 2

| Kryterium | Tak (1 pkt) | Nie (0 pkt) | Uwagi |
|-----------|------------|-----------|-------|
| **ROS 2** | Węzeł pojawia się w `ros2 node list` | Brak węzła | Terminal: `ros2 node list` |
| **Topiki** | Wszystkie 4 topiki dostępne | < 4 topiki | `ros2 topic list` |
| **Synchronizacja** | Model śledzi robota w RT | Opóźnienie > 500ms | Demo |
| **Latency** | < 100ms mediana | >= 100ms | 3 próby pomiarowe |
| **Buforowanie** | Ring buffer, brak drżeń | Drży albo traci pakiety | Obserwacja |
| **Raport wykonania** | Wypeł., tabela pozycji | Puste | LAB06_STATE |

**Punkt kontrolny 3.0:** >= 4/6 kryteriów  
**Punkt kontrolny 4.0:** >= 5/6 + analiza opóźnienia

---

## Lab 7 — Walidacja i Testy

| Kryterium | Tak (1 pkt) | Nie (0 pkt) | Uwagi |
|-----------|------------|-----------|-------|
| **Smoke Tests** | Brak RED errors | Są RED errors | Console check |
| **Matryca** | >= 70% Lab 1–6 PASS | < 70% | 18 testów razem |
| **Wydajność** | FPS >= 60 (VR) / >= 30 (AR) | Mniej | Tabela pomiarów |
| **Stress Test** | Stabilny przez 60s | Crash lub drastyczny spadek | Memory stable? |
| **Scenariusze UX** | 3 przeprowadzone, czasy | < 3 albo bez czasów | Błędy zarejestrowane? |
| **Diagnoza Błędu** | Znaleziony, naprawiony | Brak diagnozy | Niezbędna dla 3.0 |
| **Raport wykonania** | Wypeł., wnioski, zaliczenie | Puste | Raport finalny |

**Punkt kontrolny 3.0:** >= 5/7 kryteriów  
**Punkt kontrolny 4.0:** >= 6/7 + przypadki brzegowe  
**Punkt kontrolny 5.0:** 7/7 + testy automatyczne + raport produkcyjny

---

## Jak szybko oceniać

**Dla każdego Laboratorium (max 90 sekund):**

1. **Smoke (10 sekund):** Uruchomić grę — czy startuje?
2. **Demo (30 sekund):** Student pokazuje główną funkcjonalność
3. **Evidence (20 sekund):** Przejrzeć tabelę pomiarów
4. **Diagnoza (20 sekund):** Zrozumieć, co znalezli
5. **Werdykt (10 sekund):** Wpisać ocenę

---

## Jak sprawdzić wkład pary?

```bash
# Sprawdź commity dla każdej osoby
git log --author="Imię" lab0X...lab0X-start --oneline

# Jeśli:
- >= 2 merytoryczne commity na osobę → wkład ok (50/50)
- 1 commit albo mniej → może być problem

```

---

## Oceny finalne

| Punkt kontrolny | Wymagania | Ćwiczenia |
|-----------|-----------|-----------|
| **3.0** | >= 5/6 (Lab 1–6) lub >= 5/7 (Lab 7) | Smoke + Demo |
| **3.5** | 6/6 (Lab 1–6) lub >= 6/7 (Lab 7) | + komponenty |
| **4.0** | 6/6 + analiza | + pomiary + porównania |
| **4.5** | 6/6 + analiza + eksperymenty | + przypadki brzegowe |
| **5.0** | 6/6 + wszystko + automatyzacja | + raport produkcyjny |

---

**Wersja:** 2.0 • 13 września 2026 r.


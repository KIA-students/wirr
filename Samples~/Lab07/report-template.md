# Laboratorium 7 — walidacja i testy akceptacyjne systemu XR

## 1. Zespół i środowisko

- Osoba 1: 
- Osoba 2: 
- Indeksy: `i1 = `, `i2 = `
- `S = i1 + i2 = `
- Warianty: `v1 = `, `v2 = `, `v3 = `, `v4 = `, `v5 = `
- Data: 
- Commit bazowy: 
- Commit końcowy: 
- Unity: 
- Platforma testowa: 
- HMD / urządzenie AR: 
- ROS 2: 
- Gazebo: 
- Uwagi o stanowisku: 

## 2. Kryteria akceptacji przed testami

Wpisz kryteria **przed** wykonaniem pomiarów.

| ID | Obszar | Kryterium akceptacji | Źródło / uzasadnienie | Blokujące? |
|---|---|---|---|---|
| AC-01 | uruchomienie |  |  | tak/nie |
| AC-02 | funkcjonalność |  |  | tak/nie |
| AC-03 | wydajność |  |  | tak/nie |
| AC-04 | stabilność |  |  | tak/nie |
| AC-05 | użyteczność |  |  | tak/nie |
| AC-06 | bezpieczeństwo / fail-safe |  |  | tak/nie |

## 3. Test dymny

| ID | Warunek | Oczekiwany wynik | Wynik rzeczywisty | Status | Dowód |
|---|---|---|---|---|---|
| SM-01 | projekt otwiera się bez błędu krytycznego |  |  | PASS/FAIL/NV |  |
| SM-02 | właściwy XR Origin i kamera |  |  | PASS/FAIL/NV |  |
| SM-03 | wejście użytkownika działa |  |  | PASS/FAIL/NV |  |
| SM-04 | wymagane podsystemy XR/AR startują |  |  | PASS/FAIL/NV |  |
| SM-05 | Gazebo/ROS 2/Unity — jeżeli używane |  |  | PASS/FAIL/NV |  |

`NV` = niezweryfikowano. Nie zastępuj braku testu wynikiem PASS.

## 4. Macierz funkcjonalna laboratoriów 1–6

| Lab | Funkcja / kontrakt | Procedura | Oczekiwany wynik | Wynik rzeczywisty | Status | Dowód |
|---:|---|---|---|---|---|---|
| 1 | XR Origin, skala, osie, baseline |  |  |  |  |  |
| 2 | chwyt, promień, UI, lokomocja |  |  |  |  |  |
| 3 | rejestracja AR i błąd rejestracji |  |  |  |  |  |
| 4 | mieszanie rzeczywistości / okluzja |  |  |  |  |  |
| 5 | model CAD i LOD |  |  |  |  |  |
| 6 | Gazebo → ROS 2 → Unity, `joint1`, `joint2`, LIVE/STALE |  |  |  |  |  |

### Eksperyment 1 — wariant `v1`

- Wariant: 
- Badany przepływ: 
- Warunki początkowe: 
- Jedna zmienna kontrolowana: 
- Wynik: 
- Dowód: 
- Wniosek: 

## 5. Wydajność i stabilność

### Konfiguracja pomiaru

- urządzenie / target: 
- scena: 
- rozdzielczość / odświeżanie: 
- czas rozgrzewki: 
- długość próby: 
- liczba powtórzeń: 

| Scenariusz | FPS mediana | ms/kl. mediana | CPU [ms] | GPU [ms] | pamięć [MB] | uwagi |
|---|---:|---:|---:|---:|---:|---|
| bezczynność |  |  |  |  |  |  |
| scenariusz nominalny |  |  |  |  |  |  |
| scenariusz obciążeniowy |  |  |  |  |  |  |

### Pamięć

- pamięć po stabilizacji: 
- pamięć po scenariuszu: 
- różnica: 
- trend po powtórzeniu scenariusza: 
- dominująca kategoria zasobów: 

### Eksperyment 2 — wariant `v2`

- Wariant: 
- Zmienna niezależna: 
- Poziomy zmiennej: 
- Wyniki: 
- Wąskie gardło: CPU / GPU / pamięć / komunikacja / brak jednoznacznego
- Wniosek: 

## 6. Użyteczność i dostępność

| Scenariusz | Cel użytkownika | Sukces? | Czas / próby | Błędne aktywacje | Potrzebna pomoc | Problem dostępności |
|---|---|---|---|---:|---|---|
| U1 |  |  |  |  |  |  |
| U2 |  |  |  |  |  |  |
| U3 |  |  |  |  |  |  |

### Eksperyment 3 — wariant `v3`

- Wariant: 
- Zadanie: 
- Miara behawioralna: 
- Wynik: 
- Obserwacja jakościowa: 
- Rekomendacja: 

## 7. Awaria kontrolowana i regresja

### Eksperyment 4 — wariant `v4`

- Wariant awarii: 
- Stan nominalny przed awarią: 
- Sposób wywołania awarii: 
- Mierzalny objaw: 
- Stan bezpieczny / oczekiwane zachowanie: 
- Czy system odzyskał działanie po przywróceniu komponentu?: 
- Czas odzyskania (jeżeli mierzony): 

### Diagnostyka

1. Hipoteza H1: 
2. Hipoteza H2: 
3. Test rozstrzygający: 
4. Wynik testu: 
5. Minimalna poprawka: 

### Test regresji

| ID | Test | Przed poprawką | Po poprawce | Status końcowy | Dowód |
|---|---|---|---|---|---|
| REG-01 | test naprawianej funkcji |  |  |  |  |
| REG-02 | wcześniej działający test powiązany |  |  |  |  |

## 8. Automatyzacja

### Eksperyment 5 — wariant `v5`

- Wariant: 
- Co zautomatyzowano: 
- Narzędzie: Unity Test Framework / skrypt / CI / inne
- Sposób uruchomienia: 
- Wynik pozytywny: 
- Wynik negatywny kontrolowany: 
- Artefakt / log: 

## 9. Macierz ryzyka

Skala przykładowa: prawdopodobieństwo `P = 1..5`, skutek `S = 1..5`, priorytet `R = P × S`.

| ID | Ryzyko | P | S | R | Dowód | Działanie ograniczające | Ryzyko resztkowe |
|---|---|---:|---:|---:|---|---|---|
| R-01 |  |  |  |  |  |  |  |
| R-02 |  |  |  |  |  |  |  |
| R-03 |  |  |  |  |  |  |  |

## 10. Decyzja akceptacyjna

Status końcowy wybierz na podstawie wcześniej zapisanych kryteriów:

- [ ] **ACCEPT** — wszystkie kryteria blokujące spełnione.
- [ ] **ACCEPT WITH CONDITIONS** — kryteria blokujące spełnione, istnieją jawne ograniczenia lub ryzyka resztkowe.
- [ ] **REJECT** — co najmniej jedno kryterium blokujące niespełnione.

### Uzasadnienie

1. Najważniejszy dowód za decyzją: 
2. Najważniejsze ograniczenie: 
3. Najważniejsze ryzyko: 
4. Działanie wymagane przed wdrożeniem: 

## 11. Checklist końcowy

- [ ] Kryteria akceptacji zapisano przed pomiarami.
- [ ] Każdy test ma oczekiwany i rzeczywisty wynik.
- [ ] Każdy FAIL ma opis skutku.
- [ ] Brak testu oznaczono jako NV, nie PASS.
- [ ] Wyniki wydajności opisują platformę i warunki pomiaru.
- [ ] Awaria kontrolowana została odtworzona i udokumentowana.
- [ ] Po poprawce wykonano test regresji.
- [ ] Decyzja ACCEPT / ACCEPT WITH CONDITIONS / REJECT wynika z danych.

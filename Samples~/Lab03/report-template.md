# Laboratorium 3 — raport: rejestracja przestrzenna, kotwice i pomiar błędu AR

> Uzupełniaj raport na bieżąco. Nie zapisuj zdjęć osób, danych lokalizacyjnych ani innych danych identyfikujących.

## Identyfikacja i warianty

- Osoba A — numer indeksu: __________
- Osoba B — numer indeksu: __________
- `S = i1 + i2`: __________
- `v1` (3.0): ___
- `v2` (3.5): ___
- `v3` (4.0): ___
- `v4` (4.5): ___
- `v5` (5.0): ___
- Data: __________
- Stanowisko: __________
- Model telefonu: __________
- Wersja Androida: __________

Wzór: `v_k = 1 + ((S + 2(k - 1)) mod 5)`.

---

## Środowisko

- Unity: __________
- AR Foundation: __________
- ARCore XR Plugin: __________
- XR Plug-in Management: __________
- Input System: __________
- URP: __________
- Graphics API: OpenGLES3 / inne: __________
- ARCore Requirement: Required / Optional / inne: __________
- Minimum API Level: __________
- `adb devices`: __________
- Scena: `Assets/Scenes/Lab03_AR.unity`

## Kontrola konfiguracji

| Element | Wynik |
|---|---|
| Android Build Support | OK / problem |
| ARCore włączony dla Androida | tak / nie |
| OpenXR wyłączony dla gałęzi Lab 3 | tak / nie |
| AR Session — jedna instancja | tak / nie |
| XR Origin — jedna instancja | tak / nie |
| XR Origin scale `(1,1,1)` | tak / nie |
| AR Camera Manager | działa / nie działa |
| AR Camera Background | działa / nie działa |
| AR Background Renderer Feature | dodany / brak |
| ARPlaneManager | działa / nie działa |
| ARRaycastManager | działa / nie działa |
| ARAnchorManager | działa / nie działa |

---

# Punkt kontrolny 3.0 — ARCore, tracking i płaszczyzny

## Tracking

- czas do `SessionTracking` [s]: __________
- pierwszy `notTrackingReason`: __________
- najczęstszy `notTrackingReason`: __________
- czas do pierwszej użytecznej płaszczyzny [s]: __________

## Eksperyment kontrolny 3.0

- Wariant `v1`: ___
- Pytanie badawcze: __________
- Przewidywanie przed pomiarem: __________
- Moderator: __________
- Warunki: __________

| Warunek | Powtórzenie 1 [s] | Powtórzenie 2 [s] | Powtórzenie 3 [s] | Mediana [s] | notTrackingReason / uwagi |
|---|---:|---:|---:|---:|---|
| A | | | | | |
| B | | | | | |
| C (jeśli dotyczy) | | | | | |

**Odpowiedź na pytanie badawcze:** __________

**Ograniczenia pomiaru:** __________

---

# Punkt kontrolny 3.5 — raycast i placement 1:1

## Model

- prefab: __________
- oczekiwany wymiar kontrolny [m]: __________
- zmierzony wymiar względem odniesienia [m]: __________
- XR Origin scale: __________
- `Create Session Anchor`: false
- `Place Only Once`: true

## Test raycastu

| Test | Wynik |
|---|---|
| tapnięcie wewnątrz płaszczyzny | sukces / brak |
| tapnięcie poza płaszczyzną | brak placementu / błędny placement |
| model pojawia się tylko raz | tak / nie |
| skala 1:1 | tak / nie |
| brak ręcznego skalowania XR Origin | tak / nie |

## Eksperyment kontrolny 3.5

- Wariant `v2`: ___
- Pytanie badawcze: __________
- Przewidywanie: __________

| Warunek | Poprawne trafienia / 10 | Mediana błędu placementu [mm] | Uwagi |
|---|---:|---:|---|
| A | | | |
| B | | | |
| C (jeśli dotyczy) | | | |

**Wniosek:** __________

---

# Punkt kontrolny 4.0 — kotwica i dryf

## Konfiguracja bazowa

- `Create Session Anchor`: true
- dystans pozycji startowej od `O` [m]: __________
- długość standaryzowanej ścieżki [m]: __________
- czas ścieżki [s]: __________

## Pomiar po powrocie

| Powtórzenie | Błąd końcowy [mm] | Tracking podczas ruchu | `notTrackingReason` | Uwagi |
|---:|---:|---|---|---|
| 1 | | | | |
| 2 | | | | |
| 3 | | | | |
| **Mediana** | | | | |

## Eksperyment kontrolny 4.0

- Wariant `v3`: ___
- Pytanie badawcze: __________
- Przewidywanie: __________

| Warunek | Powtórzenie 1 [mm] | 2 [mm] | 3 [mm] | Mediana [mm] | Uwagi |
|---|---:|---:|---:|---:|---|
| A | | | | | |
| B | | | | | |
| C (jeśli dotyczy) | | | | | |

**Wniosek:** __________

---

# Punkt kontrolny 4.5 — rejestracja dwupunktowa i błąd

## Geometria stanowiska

- `O`: __________
- `X`: __________
- oczekiwany `OX` [m]: __________
- `P1`: `(0, 0, 0.20)` m / inne: __________
- `P2`: `(0.15, 0, 0.20)` m / inne: __________
- `P3`: `(0.30, 0, 0.20)` m / inne: __________
- `Correct Uniform Scale`: false

## Bazowe rejestracje

| Rejestracja | Zmierzony OX [m] | e1 [mm] | e2 [mm] | e3 [mm] | e_med [mm] | e_max [mm] |
|---:|---:|---:|---:|---:|---:|---:|
| 1 | | | | | | |
| 2 | | | | | | |
| 3 | | | | | | |

- mediana `e_med` z trzech rejestracji [mm]: __________
- maksimum z całej serii [mm]: __________

## Eksperyment kontrolny 4.5

- Wariant `v4`: ___
- Pytanie badawcze: __________
- Przewidywanie: __________

| Warunek | Rejestracja | e1 [mm] | e2 [mm] | e3 [mm] | e_med [mm] | e_max [mm] |
|---|---:|---:|---:|---:|---:|---:|
| A | 1 | | | | | |
| A | 2 | | | | | |
| A | 3 | | | | | |
| B | 1 | | | | | |
| B | 2 | | | | | |
| B | 3 | | | | | |
| C (jeśli dotyczy) | 1 | | | | | |
| C | 2 | | | | | |
| C | 3 | | | | | |

**Wniosek:** __________

## Ograniczenie metody

Wynik obejmuje łącznie błąd rejestracji, raycastu, wskazania dotykowego i śledzenia.

- Najważniejsze źródło niepewności w tej sesji: __________
- Jak można byłoby poprawić metodę pomiarową: __________

---

# Punkt kontrolny 5.0 — diagnostyka kontrolowanego błędu

## Wariant

- `v5`: ___
- kontrolowany błąd: __________
- przewidywana sygnatura: __________

## H1 / H2

- Objaw: __________
- H1: __________
- H2: __________
- Przewidywanie rozróżniające H1 i H2: __________
- Test rozstrzygający: __________

## Baseline → błąd → naprawa

| Stan | Powtórzenie | e1 [mm] | e2 [mm] | e3 [mm] | e_med [mm] | e_max [mm] |
|---|---:|---:|---:|---:|---:|---:|
| baseline | 1 | | | | | |
| baseline | 2 | | | | | |
| baseline | 3 | | | | | |
| kontrolowany błąd | 1 | | | | | |
| kontrolowany błąd | 2 | | | | | |
| kontrolowany błąd | 3 | | | | | |
| po naprawie | 1 | | | | | |
| po naprawie | 2 | | | | | |
| po naprawie | 3 | | | | | |

- Czy sygnatura była zgodna z przewidywaniem? __________
- Przyczyna uznana za najbardziej prawdopodobną: __________
- Minimalna poprawka: __________
- Czy wynik po naprawie wrócił w pobliże baseline? __________

---

# Wnioski

1. **Tracking i środowisko:** __________
2. **Raycast i placement:** __________
3. **Kotwica / dryf:** __________
4. **Rejestracja O–X:** __________
5. **Najważniejsza sygnatura błędu:** __________
6. **Najważniejsze ograniczenie metody:** __________

# Git

Ostatni commit:

```text
lab03: complete AR registration experiments
```

- SHA: __________
- Najwyższy osiągnięty punkt kontrolny: 3.0 / 3.5 / 4.0 / 4.5 / 5.0

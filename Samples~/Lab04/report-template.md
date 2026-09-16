# Laboratorium 4 — raport: rozumienie sceny, głębia, okluzja i estymacja oświetlenia

> Uzupełniaj raport na bieżąco. Nie zapisuj obrazu kamery zawierającego twarze, dokumenty, ekrany ani inne dane identyfikujące.

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
- Google ARCore XR Plugin: __________
- XR Plug-in Management: __________
- Input System: __________
- URP: __________
- Graphics API: __________
- Minimum API Level: __________
- ARCore: Required / Optional: __________
- Depth: Required / Optional: __________
- Scena: `Assets/Scenes/Lab04_SceneUnderstanding.unity`
- Gałąź: `lab04-work`

## Kontrola konfiguracji

| Element | Wynik |
|---|---|
| Android aktywny | tak / nie |
| ARCore aktywny dla Androida | tak / nie |
| OpenXR wyłączony dla mobilnej gałęzi AR | tak / nie |
| AR Session — jedna instancja | tak / nie |
| XR Origin — jedna instancja | tak / nie |
| XR Origin scale `(1,1,1)` | tak / nie |
| AR Camera Manager | działa / problem |
| AR Camera Background | działa / problem |
| AR Background Renderer Feature | jest / brak |
| AR Occlusion Manager na kamerze | jest / brak |
| ARRaycastManager | jest / brak |
| Directional Light | jest / brak |
| Lab04Systems | jest / brak |

---

# Punkt kontrolny 3.0 — Depth API i diagnostyka

## Stan subsystemu

- `environmentDepthImageSupported`: __________
- `environmentDepthTemporalSmoothingSupported`: __________
- `requestedEnvironmentDepthMode`: __________
- `currentEnvironmentDepthMode`: __________
- `environmentDepthTemporalSmoothingEnabled`: __________
- rozdzielczość ostatniej klatki depth: __________

## Eksperyment v1

- Wariant `v1`: ___
- Zmienna niezależna: __________
- Hipoteza przed pomiarem: __________
- Warunki stałe: __________

| Warunek | Powt. 1 successRate [%] | Powt. 2 [%] | Powt. 3 [%] | Mediana [%] | Rozdzielczość | Uwagi |
|---|---:|---:|---:|---:|---|---|
| A | | | | | | |
| B | | | | | | |
| C (jeśli dotyczy) | | | | | | |

**Wniosek:** __________

**Najważniejsze ograniczenie pomiaru:** __________

---

# Punkt kontrolny 3.5 — okluzja środowiskowa

## Test bazowy

| Tryb | Model zasłaniany przez przeszkodę? | Błędy / 10 prób | Dominujący błąd |
|---|---|---:|---|
| Off | | | |
| Raw | | | |
| Smoothed | | | |

## Eksperyment v2

- Wariant `v2`: ___
- Zmienna: __________
- Hipoteza: __________

| Warunek | Tryb / jakość | Błędy okluzji / 10 | Requested depth | Current depth | Uwagi |
|---|---|---:|---|---|---|
| A | | | | | |
| B | | | | | |
| C (jeśli dotyczy) | | | | | |

Dominująca sygnatura:
- [ ] przeciek modelu przez przeszkodę
- [ ] nadmierne wycięcie modelu
- [ ] migotanie krawędzi
- [ ] opóźnienie po ruchu
- [ ] inna: __________

**Wniosek o smoothingu / jakości depth:** __________

---

# Punkt kontrolny 4.0 — depth-raycast i błąd odległości

## Konfiguracja

- `Use Depth First`: true
- `Fallback To Plane`: false
- marker prefab: __________
- sposób wyznaczenia dystansu referencyjnego: __________

## Seria bazowa

| d_ref [m] | Próba | HIT/MISS | d_AR [m] | e_d = abs(d_AR-d_ref) [m] |
|---:|---:|---|---:|---:|
| 0.50 | 1 | | | |
| 0.50 | 2 | | | |
| 0.50 | 3 | | | |
| 0.50 | 4 | | | |
| 0.50 | 5 | | | |
| 1.00 | 1 | | | |
| 1.00 | 2 | | | |
| 1.00 | 3 | | | |
| 1.00 | 4 | | | |
| 1.00 | 5 | | | |
| 1.50 | 1 | | | |
| 1.50 | 2 | | | |
| 1.50 | 3 | | | |
| 1.50 | 4 | | | |
| 1.50 | 5 | | | |

- mediana błędu dla 0.50 m: __________
- mediana błędu dla 1.00 m: __________
- mediana błędu dla 1.50 m: __________

## Eksperyment v3

- Wariant `v3`: ___
- Zmienna: __________
- Hipoteza: __________

| Warunek | HIT / 5 | MISS / 5 | Mediana błędu dla HIT [m] | Uwagi |
|---|---:|---:|---:|---|
| A | | | | |
| B | | | | |
| C (jeśli dotyczy) | | | | |

**Wniosek:** __________

**Dlaczego MISS nie jest 0 m:** __________

---

# Punkt kontrolny 4.5 — estymacja oświetlenia

## Dostępność danych

### AmbientIntensity

- requested: __________
- current: __________
- `averageBrightness`: dostępne / NA
- `averageColorTemperature`: dostępne / NA
- `colorCorrection`: dostępne / NA

### EnvironmentalHdr

- requested: __________
- current: __________
- `mainLightDirection`: dostępne / NA
- `mainLightColor`: dostępne / NA
- `averageMainLightBrightness`: dostępne / NA
- `ambientSphericalHarmonics`: dostępne / NA

## Pięć warunków bazowych

| Warunek | Mediana brightness | Color temp K / NA | Main direction / NA | Current mode | Ocena wizualna 1–5 | Uwagi |
|---|---:|---:|---|---|---:|---|
| Jasne rozproszone | | | | | | |
| Słabsze rozproszone | | | | | | |
| Źródło boczne | | | | | | |
| Światło od przodu | | | | | | |
| Oświetlenie mieszane | | | | | | |

## Eksperyment v4

- Wariant `v4`: ___
- Zmienna: __________
- Hipoteza: __________

| Warunek | Próbek API | Mediana brightness / NA | Requested | Current | Ocena 1–5 | Uwagi |
|---|---:|---:|---|---|---:|---|
| A | 5 | | | | | |
| B | 5 | | | | | |

**Wniosek:** __________

**Które pole było niedostępne i dlaczego nie wpisano 0:** __________

---

# Punkt kontrolny 5.0 — kontrolowany błąd i diagnoza

## Wariant

- `v5`: ___
- kontrolowany błąd: __________
- oczekiwana sygnatura: __________
- miara używana do porównania: __________

## H1 / H2

- Objaw: __________
- H1: __________
- H2: __________
- Przewidywanie dla H1: __________
- Przewidywanie dla H2: __________
- Test rozstrzygający: __________
- Wynik testu: __________
- Najbardziej prawdopodobna przyczyna: __________

## Baseline → błąd → naprawa

| Stan | Powt. 1 | Powt. 2 | Powt. 3 | Mediana / wynik zbiorczy | Uwagi |
|---|---:|---:|---:|---:|---|
| baseline | | | | | |
| kontrolowany błąd | | | | | |
| po naprawie | | | | | |

- Czy wynik po naprawie wrócił w pobliże baseline? __________
- Czy sygnatura błędu była zgodna z przewidywaniem? __________
- Minimalna poprawka: __________

---

# Wnioski

1. **Depth API:** __________
2. **Okluzja:** __________
3. **Depth-raycast:** __________
4. **Light estimation:** __________
5. **Najbardziej wrażliwy warunek środowiskowy:** __________
6. **Najważniejsze ograniczenie metody:** __________

# Git

Ostatni commit:

```text
lab04: complete scene understanding experiments
```

- SHA: __________
- Najwyższy osiągnięty punkt kontrolny: 3.0 / 3.5 / 4.0 / 4.5 / 5.0

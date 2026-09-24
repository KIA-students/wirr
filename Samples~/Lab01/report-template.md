# Laboratorium 1 — raport pomiarowy

> Uzupełniaj ten plik na bieżąco. Instrukcja wskazuje sekcję raportu po każdym pomiarze.

## Identyfikacja i warianty

- Student 1 — numer indeksu: __________
- Student 2 — numer indeksu: __________
- Suma `S = i1 + i2`: __________
- `v1` (3.0): ___
- `v2` (3.5): ___
- `v3` (4.0): ___
- `v4` (4.5): ___
- `v5` (5.0): ___
- Data: __________
- Stanowisko / PC: __________
- GPU: __________
- Unity: __________
- Gałąź robocza: `lab01-work`

Wzór: `v_k = 1 + ((S + 2(k - 1)) mod 5)`.

---

# Środowisko i scena

## Środowisko

- Unity: __________
- XR Plug-in Management: __________
- OpenXR Plugin: __________
- XR Interaction Toolkit: __________
- Input System: __________
- Active Input Handling: __________
- PC RP Asset: __________
- Quest RP Asset: __________

## Kontrola sceny

| Test | Wynik |
|---|---|
| dokładnie jedna Main Camera | działa / nie działa |
| jeden XR Origin | działa / nie działa |
| XR Interaction Simulator | działa / nie działa |
| obrót głowy | działa / nie działa |
| translacja głowy | działa / nie działa |
| kontroler / Select | działa / nie działa |
| `Cube_1m` ma wymiar 1 m | tak / nie |

## Skrypty utworzone podczas laboratorium

- [ ] `PerformanceBenchmark.cs`
- [ ] `CpuLoadGenerator.cs`
- [ ] `RenderLoadGenerator.cs`
- [ ] `PhysicsLoadGenerator.cs`

---

# Punkt kontrolny 3.0

## Wariant bazowy PC

- tryb: Edytor / samodzielna aplikacja PC (`standalone`)
- Game View / rozdzielczość: __________
- VSync: __________
- `sampleFrames`: 180

| Próba | FPS | ms/klatka |
|---:|---:|---:|
| 1 | | |
| 2 | | |
| 3 | | |
| **Mediana** | | |

## Eksperyment kontrolny 3.0

- Wariant `v1`: ___
- Warunek A: __________
- Warunek B: __________

| Warunek | FPS 1 | FPS 2 | FPS 3 | Mediana FPS | ms 1 | ms 2 | ms 3 | Mediana ms |
|---|---:|---:|---:|---:|---:|---:|---:|---:|
| A | | | | | | | | |
| B | | | | | | | | |

**Różnica / zmiana względna:** __________  
**Odpowiedź na pytanie badawcze:** __________

---

# CPU — punkt kontrolny 3.5

## CPU-A — `iterationsPerFrame`

Stałe: `blocksPerFrame = 1`, `allocationBytesPerFrame = 0`.

| Iteracje/klatkę | FPS 1 | FPS 2 | FPS 3 | Mediana FPS | Mediana ms | Main Thread ms | ΔFPS [%] |
|---:|---:|---:|---:|---:|---:|---:|---:|
| 0 | | | | | | | |
| 10000 | | | | | | | |
| 25000 | | | | | | | |
| 50000 | | | | | | | |
| 100000 | | | | | | | |
| 200000 | | | | | | | |
| 400000 | | | | | | | |

**Wykres:** `CPU-A.png`  
**Wniosek:** __________

## CPU-B — `blocksPerFrame`

Stałe: `iterationsPerFrame = 320000`, `allocationBytesPerFrame = 0`.

| Bloki/klatkę | FPS 1 | FPS 2 | FPS 3 | Mediana FPS | Mediana ms | ΔFPS [%] |
|---:|---:|---:|---:|---:|---:|---:|
| 1 | | | | | | |
| 2 | | | | | | |
| 4 | | | | | | |
| 8 | | | | | | |
| 16 | | | | | | |
| 32 | | | | | | |

**Wykres:** `CPU-B.png`  
**Wniosek:** __________

## CPU-C — `allocationBytesPerFrame`

Stałe: `iterationsPerFrame = 100000`, `blocksPerFrame = 1`.

| B/klatkę | FPS 1 | FPS 2 | FPS 3 | Mediana FPS | Mediana ms | GC Alloc | GC zaobserwowane? |
|---:|---:|---:|---:|---:|---:|---:|---|
| 0 | | | | | | | |
| 1024 | | | | | | | |
| 4096 | | | | | | | |
| 16384 | | | | | | | |
| 65536 | | | | | | | |
| 262144 | | | | | | | |

**Wykres:** `CPU-C.png`  
**Wniosek:** __________

## Eksperyment kontrolny 3.5

- Wariant `v2`: ___
- Warunek A: __________
- Warunek B: __________

| Warunek | FPS 1 | FPS 2 | FPS 3 | Mediana FPS | Mediana ms |
|---|---:|---:|---:|---:|---:|
| A | | | | | |
| B | | | | | |

**Odpowiedź:** __________

---

# Renderowanie — punkt kontrolny 4.0

## GPU-A — liczba widocznych obiektów

Stałe: `materialCount = 1`, `primitiveType = Cube`.

| Obiekty | FPS 1 | FPS 2 | FPS 3 | Mediana FPS | Mediana ms | Batches | SetPass | Triangles | ΔFPS [%] |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| 0 | | | | | | | | | |
| 100 | | | | | | | | | |
| 250 | | | | | | | | | |
| 500 | | | | | | | | | |
| 1000 | | | | | | | | | |
| 2000 | | | | | | | | | |
| 4000 | | | | | | | | | |

**Wykres:** `GPU-A.png`  
**Wniosek:** __________

## GPU-B — liczba materiałów

Stała liczba obiektów: 1000.

| Materiały | FPS 1 | FPS 2 | FPS 3 | Mediana FPS | Mediana ms | Batches | SetPass |
|---:|---:|---:|---:|---:|---:|---:|---:|
| 1 | | | | | | | |
| 2 | | | | | | | |
| 4 | | | | | | | |
| 8 | | | | | | | |
| 16 | | | | | | | |
| 32 | | | | | | | |

**Wykres:** `GPU-B.png`  
**Wniosek:** __________

## GPU-C — liczba trójkątów

Wpisz sześć punktów utworzonych przez zmianę `primitiveType` i/lub `objectCount`.
Oś X wykresu ma przedstawiać rzeczywistą wartość `Triangles` z Game View.

| Punkt | Primitive | Obiekty | Triangles | FPS 1 | FPS 2 | FPS 3 | Mediana FPS | Mediana ms |
|---:|---|---:|---:|---:|---:|---:|---:|---:|
| 1 | | | | | | | | |
| 2 | | | | | | | | |
| 3 | | | | | | | | |
| 4 | | | | | | | | |
| 5 | | | | | | | | |
| 6 | | | | | | | | |

**Wykres:** `GPU-C.png`  
**Wniosek:** __________

## GPU-D — Render Scale

| Render Scale | FPS 1 | FPS 2 | FPS 3 | Mediana FPS | Mediana ms | ΔFPS [%] |
|---:|---:|---:|---:|---:|---:|---:|
| 0.60 | | | | | | |
| 0.70 | | | | | | |
| 0.80 | | | | | | |
| 0.90 | | | | | | |
| 1.00 | | | | | | |
| 1.20 | | | | | | |

**Wykres:** `GPU-D.png`  
**Wniosek:** __________

## GPU-E — post-processing

Kolejność: Bloom, Vignette, Chromatic Aberration, Film Grain, Color Adjustments.

| Aktywne efekty | FPS 1 | FPS 2 | FPS 3 | Mediana FPS | Mediana ms | ΔFPS [%] |
|---:|---:|---:|---:|---:|---:|---:|
| 0 | | | | | | |
| 1 | | | | | | |
| 2 | | | | | | |
| 3 | | | | | | |
| 4 | | | | | | |
| 5 | | | | | | |

**Wykres:** `GPU-E.png`  
**Wniosek:** __________

## Eksperyment kontrolny 4.0

- Wariant `v3`: ___
- Warunek A: __________
- Warunek B: __________

| Warunek | FPS 1 | FPS 2 | FPS 3 | Mediana FPS | Mediana ms | Batches | SetPass | Triangles |
|---|---:|---:|---:|---:|---:|---:|---:|---:|
| A | | | | | | | | |
| B | | | | | | | | |

**Odpowiedź:** __________

---

# Fizyka — punkt kontrolny 4.5

## PHY-A — liczba dynamicznych Rigidbody

| Rigidbody | FPS 1 | FPS 2 | FPS 3 | Mediana FPS | Mediana ms | Physics.Processing ms | ΔFPS [%] |
|---:|---:|---:|---:|---:|---:|---:|---:|
| 0 | | | | | | | |
| 25 | | | | | | | |
| 50 | | | | | | | |
| 100 | | | | | | | |
| 200 | | | | | | | |
| 400 | | | | | | | |
| 800 | | | | | | | |

**Wykres:** `PHY-A.png`  
**Wniosek:** __________

## PHY-B — `spacing`

Stała liczba obiektów: 400.

| spacing | FPS 1 | FPS 2 | FPS 3 | Mediana FPS | Mediana ms | Physics.Processing ms |
|---:|---:|---:|---:|---:|---:|---:|
| 2.0 | | | | | | |
| 1.5 | | | | | | |
| 1.2 | | | | | | |
| 1.0 | | | | | | |
| 0.8 | | | | | | |
| 0.6 | | | | | | |

**Wykres:** `PHY-B.png`  
**Wniosek:** __________

## PHY-C — Fixed Timestep

| Fixed Timestep [s] | kroki/s | FPS 1 | FPS 2 | FPS 3 | Mediana FPS | Mediana ms | Physics.Processing ms |
|---:|---:|---:|---:|---:|---:|---:|---:|
| 0.0333 | 30 | | | | | | |
| 0.0250 | 40 | | | | | | |
| 0.0200 | 50 | | | | | | |
| 0.0167 | 60 | | | | | | |
| 0.0133 | 75 | | | | | | |
| 0.0111 | 90 | | | | | | |

**Wykres:** `PHY-C.png`  
**Wniosek:** __________

## PHY-D — Default Solver Iterations

| Iteracje solvera | FPS 1 | FPS 2 | FPS 3 | Mediana FPS | Mediana ms | Physics.Processing ms |
|---:|---:|---:|---:|---:|---:|---:|
| 1 | | | | | | |
| 2 | | | | | | |
| 4 | | | | | | |
| 6 | | | | | | |
| 8 | | | | | | |
| 12 | | | | | | |

**Wykres:** `PHY-D.png`  
**Wniosek:** __________

## Eksperyment kontrolny 4.5

- Wariant `v4`: ___
- Warunek A: __________
- Warunek B: __________

| Warunek | FPS 1 | FPS 2 | FPS 3 | Mediana FPS | Mediana ms | Physics.Processing ms |
|---|---:|---:|---:|---:|---:|---:|
| A | | | | | | |
| B | | | | | | |

**Odpowiedź:** __________

---

# Meta Quest 3 — punkt kontrolny 5.0

## Quest 3 — połączenie

- Developer Mode: włączony / wyłączony / niezweryfikowano
- Kabel / port USB: __________
- USB debugging zaakceptowane: tak / nie
- `adb devices`: __________
- Status ADB: `device` / `unauthorized` / brak
- Uwagi: __________

## Quest 3 — Meta Horizon Link

- Meta Horizon Link wykrywa HMD: tak / nie
- Platforma Unity: Windows / PC
- OpenXR dla Windows: aktywny / nieaktywny
- `PC_RPAsset`: aktywny / nieaktywny
- XR Interaction Simulator podczas Link: wyłączony / włączony

| Test | Wynik |
|---|---|
| scena uruchamia się z Unity Play w HMD | działa / nie działa |
| obrót głowy | działa / nie działa |
| translacja głowy | działa / nie działa |
| lewy kontroler | działa / nie działa |
| prawy kontroler | działa / nie działa |
| Select | działa / nie działa |

**Uwaga:** Meta Horizon Link nie jest pomiarem wydajności samodzielnej aplikacji na Quest 3.

## Quest 3 — aplikacja samodzielna

- Profil: Android / Meta Quest
- Architektura: ARM64
- API graficzne: __________
- OpenXR: aktywny / nieaktywny
- `Mobile_RPAsset`: aktywny / nieaktywny
- `Render Scale` wariantu bazowego: __________
- wynik polecenia `Build And Run`: sukces / błąd
- Aplikacja działa bez aktywnego Meta Horizon Link: tak / nie
- Uwagi / błąd buildu: __________

## Quest 3 — kontrola funkcjonalna aplikacji samodzielnej

| Test | działa | nie działa | niezweryfikowano |
|---|:---:|:---:|:---:|
| poprawna skala sceny | ☐ | ☐ | ☐ |
| obrót głowy | ☐ | ☐ | ☐ |
| translacja głowy | ☐ | ☐ | ☐ |
| pozycja lewego kontrolera | ☐ | ☐ | ☐ |
| pozycja prawego kontrolera | ☐ | ☐ | ☐ |
| Select | ☐ | ☐ | ☐ |
| stabilność przez 60 s | ☐ | ☐ | ☐ |

## Quest 3 — wariant bazowy aplikacji samodzielnej

- tryb: samodzielna aplikacja (`standalone`)
- Render Scale: __________
- częstotliwość odświeżania: __________ Hz

| Próba | FPS | ms/klatka |
|---:|---:|---:|
| 1 | | |
| 2 | | |
| 3 | | |
| **Mediana** | | |

## Eksperyment kontrolny 5.0 — przeniesienie PC ↔ samodzielna aplikacja Quest

- Wariant `v5`: ___
- Parametr: __________
- Warunek A: __________
- Warunek B: __________

### PC

| Warunek | FPS 1 | FPS 2 | FPS 3 | Mediana FPS | Mediana ms |
|---|---:|---:|---:|---:|---:|
| A | | | | | |
| B | | | | | |

### Samodzielna aplikacja na Quest 3

| Warunek | FPS 1 | FPS 2 | FPS 3 | Mediana FPS | Mediana ms |
|---|---:|---:|---:|---:|---:|
| A | | | | | |
| B | | | | | |

- Zmiana względna czasu klatki — PC: __________ %
- Zmiana względna czasu klatki — Quest 3: __________ %

**Odpowiedź na pytanie badawcze (maks. 2 zdania):**  
__________

---

# Wnioski

1. **CPU:** __________
2. **Renderowanie:** __________
3. **Fizyka:** __________
4. **PC vs Quest 3** (dla 5.0): __________

# Załączniki

- [ ] wykresy dla wykonanych serii
- [ ] co najmniej jeden zrzut Profilera
- [ ] zrzut sceny bazowej
- [ ] opcjonalnie log `LAB01_BENCHMARK` z Quest 3

# Git

- SHA ostatniego commita: __________
- Najwyższy osiągnięty punkt kontrolny: 3.0 / 3.5 / 4.0 / 4.5 / 5.0

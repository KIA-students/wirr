# Laboratorium 5 — raport: od modelu CAD do siatki czasu rzeczywistego

> Uzupełniaj raport na bieżąco. Nie nadpisuj modelu źródłowego. Wszystkie porównania wykonuj przy jednej zmiennej niezależnej.

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

Wzór: `v_k = 1 + ((S + 2(k - 1)) mod 5)`.

---

## Środowisko

- Unity: __________
- URP: __________
- Blender / inne DCC: __________
- narzędzie CAD / konwerter STEP: __________
- system operacyjny: __________
- GPU: __________
- rozdzielczość / tryb renderowania: __________
- scena: `Assets/Scenes/Lab05_CADOptimization.unity`
- gałąź: `lab05-work`

## Model źródłowy

- nazwa: __________
- format źródłowy: STEP / STP / FBX / OBJ / STL / USD / USDZ / inny: __________
- czy użyto importera USD / Pixyz / innego pluginu Unity: __________
- program źródłowy: __________
- jednostka źródłowa: __________
- rozmiar pliku: __________
- liczba części / obiektów: __________
- czy zachowano oryginalny plik bez zmian: tak / nie

## Łańcuch konwersji

```text
SOURCE -> __________ -> __________ -> __________ -> Unity
```

### Informacja zachowana / utracona

| Etap | Format wejściowy | Format wyjściowy | Zachowane informacje | Utracone / zmienione informacje |
|---|---|---|---|---|
| 1 | | | | |
| 2 | | | | |
| 3 | | | | |

---

## Ustawienia importu Unity

- Scale Factor / Convert Units: __________
- Bake Axis Conversion: __________
- Mesh Compression: __________
- Read/Write: __________
- Optimize Mesh: __________
- Index Format: __________
- Normals / Tangents: __________
- Preserve Hierarchy: __________
- Generate Colliders: __________
- materiały — strategia importu/wyszukiwania: __________
- LOD: ręczny / automatycznie rozpoznany z `_LOD#` / wygenerowany narzędziem zewnętrznym: __________

## Skala i geometria referencyjna

- root `localScale`: __________
- `Scale Factor`: __________
- `Convert Units`: __________
- wymiar referencyjny 1 — opis: __________
- wymiar oczekiwany 1 [m]: __________
- wymiar zmierzony 1 [m]: __________
- błąd 1 [mm]: __________
- wymiar referencyjny 2 — opis: __________
- wymiar oczekiwany 2 [m]: __________
- wymiar zmierzony 2 [m]: __________
- błąd 2 [mm]: __________

---

# Punkt kontrolny 3.0 — format, import i audyt topologii

## Baseline topology audit

| Pole | Wynik |
|---|---:|
| MeshFilter count | |
| Vertices | |
| Triangles | |
| Submeshes | |
| Material slots | |
| Boundary edges | |
| Non-manifold edges | |
| Degenerate triangles | |
| q05 | |
| qMedian | |
| World size X/Y/Z [m] | |

## Porównanie formatów / eksportu

| Wariant | Format | Rozmiar pliku | Obiekty | Vertices | Triangles | Submesh | Materiały | UV | Hierarchia | Wymiary OK? |
|---|---|---:|---:|---:|---:|---:|---:|---|---|---|
| A | | | | | | | | | | |
| B | | | | | | | | | | |
| C (jeśli dotyczy) | | | | | | | | | | |

## Eksperyment v1

- `v1`: ___
- zmienna niezależna: __________
- hipoteza przed pomiarem: __________
- warunki stałe: __________

| Warunek | Co zmieniono | Triangles | Material slots | q05 | qMedian | Wymiar kontrolny [m] | Wynik jakościowy |
|---|---|---:|---:|---:|---:|---:|---|
| A | | | | | | | |
| B | | | | | | | |
| C (jeśli dotyczy) | | | | | | | |

**Wniosek:** __________

**Która informacja została utracona podczas konwersji i dlaczego:** __________

---

# Punkt kontrolny 3.5 — triangulacja i jakość siatki

## Wyjaśnienie teoretyczne własnymi słowami

- geometria vs topologia: __________
- warunek pustego okręgu Delaunaya: __________
- Bowyer–Watson — czym jest cavity: __________
- triangulacja polygonu vs tessellacja CAD: __________

## Eksperyment v2

- `v2`: ___
- zmienna: __________
- hipoteza: __________

| Warunek | Metoda / parametr | Vertices | Triangles | Boundary | Non-manifold | Degenerate | q05 | qMedian | Cechy funkcjonalne zachowane? |
|---|---|---:|---:|---:|---:|---:|---:|---:|---|
| A | | | | | | | | | |
| B | | | | | | | | | |
| C (jeśli dotyczy) | | | | | | | | | |

### Ocena wizualna i funkcjonalna

- zmiana sylwetki: brak / mała / duża
- utracone otwory / szczeliny / krawędzie funkcjonalne: __________
- artefakty normalnych: __________
- artefakty UV: __________

**Czy mniejsza liczba trójkątów oznaczała lepszy model? Uzasadnij:** __________

---

# Punkt kontrolny 4.0 — LOD i benchmark

## Finalne geometrie LOD

| Model | Rozmiar pliku | Vertices | Triangles | Submesh | Materiały | q05 | qMedian | Uwagi |
|---|---:|---:|---:|---:|---:|---:|---:|---|
| Baseline | | | | | | | | |
| LOD0 | | | | | | | | |
| LOD1 | | | | | | | | |
| LOD2 | | | | | | | | |

## Konfiguracja LOD Group

- LOD0 threshold: __________
- LOD1 threshold: __________
- LOD2 threshold: __________
- próg culling: __________
- czy zmienia się skala/pozycja przy przejściu: tak / nie
- widoczne popping / artefakty: __________

## Eksperyment v3

- `v3`: ___
- zmienna: __________
- warunki stałe: __________
- miejsce pomiaru: Editor / Build: __________

| Warunek | Próba | mean ms | median ms | p95 ms | FPS z mediany | Triangles | Renderers | Material slots | CPU [ms] | GPU [ms] | Draw calls / batches | Pamięć |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| A | 1 | | | | | | | | | | | |
| A | 2 | | | | | | | | | | | |
| A | 3 | | | | | | | | | | | |
| B | 1 | | | | | | | | | | | |
| B | 2 | | | | | | | | | | | |
| B | 3 | | | | | | | | | | | |
| C (jeśli dotyczy) | 1 | | | | | | | | | | | |
| C | 2 | | | | | | | | | | | |
| C | 3 | | | | | | | | | | | |

**Dominujący koszt:** geometria / CPU / GPU / materiały / pamięć / inne: __________

**Wniosek:** __________

---

# Punkt kontrolny 4.5 — materiały, draw calls, tekstury i kolizje

## Baseline render / physics

- renderers: __________
- submeshes: __________
- unikalne materiały: __________
- material slots: __________
- draw calls / batches: __________
- pamięć tekstur: __________
- collidery — typ/liczba: __________

## Eksperyment v4

- `v4`: ___
- zmienna: __________
- hipoteza: __________

| Warunek | Triangles | Materiały | Draw calls / batches | CPU [ms] | GPU [ms] | Pamięć | Błędy funkcjonalne / 20 | Ocena jakości 1–5 |
|---|---:|---:|---:|---:|---:|---:|---:|---:|
| A | | | | | | | | |
| B | | | | | | | | |
| C (jeśli dotyczy) | | | | | | | | |

**Co zmieniło koszt mimo niezmienionej / podobnej geometrii:** __________

**Czy wariant B zostałby wdrożony? Dlaczego:** __________

---

# Punkt kontrolny 5.0 — kontrolowany błąd i diagnostyka

## Eksperyment v5

- `v5`: ___
- kontrolowany błąd: __________
- przewidywana sygnatura: __________

### Hipotezy

- H1: __________
- przewidywanie H1: __________
- H2: __________
- przewidywanie H2: __________
- test rozstrzygający: __________

## Baseline → fault → repaired

| Stan | Wymiar [m] | Vertices | Triangles | Boundary | q05 | Materiały | Hierarchia OK? | median ms | p95 ms | Błąd funkcjonalny | Uwagi |
|---|---:|---:|---:|---:|---:|---:|---|---:|---:|---|---|
| baseline | | | | | | | | | | | |
| fault | | | | | | | | | | | |
| repaired | | | | | | | | | | | |

- obserwacja rozstrzygająca H1/H2: __________
- przyczyna: __________
- minimalna poprawka: __________
- czy stan repaired wrócił do baseline: tak / częściowo / nie
- dlaczego: __________

---

# Tabela końcowa

| Wariant | Vertices | Triangles | Submesh | Materiały | median ms | p95 ms | Draw calls / batches | Pamięć | Najważniejszy kompromis |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---|
| Baseline | | | | | | | | | |
| LOD0 | | | | | | | | | |
| LOD1 | | | | | | | | | |
| LOD2 | | | | | | | | | |
| Final XR | | | | | | | | | |

## Wybór finalnego modelu

- wybrany wariant: __________
- dlaczego nie wariant o najmniejszej liczbie trójkątów: __________
- kluczowy dowód wydajnościowy: __________
- kluczowy dowód zachowania funkcji: __________

---

# Wnioski

1. **Największa strata informacji w pipeline:** __________
2. **Największy mierzalny zysk optymalizacyjny:** __________
3. **Najważniejsza obserwacja dotycząca topologii:** __________
4. **Najważniejsza obserwacja dotycząca formatów:** __________
5. **Najważniejsza obserwacja dotycząca draw calls / materiałów:** __________
6. **Najważniejsze ograniczenie metody pomiarowej:** __________

# Git

- commit z baseline: __________
- commit z LOD: __________
- commit finalny: __________
- najwyższy osiągnięty punkt kontrolny: 3.0 / 3.5 / 4.0 / 4.5 / 5.0

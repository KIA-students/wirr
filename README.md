# WiRR — Wirtualna i Rozszerzona Rzeczywistość

### Repozytorium laboratoryjne (Lab 1-7)

Katedra Informatyki i Automatyki, Politechnika Rzeszowska

## Struktura

```
Assets/
  Scenes/           # Lab01_Baseline.unity ... Lab07_Validation.unity
  Scripts/
    Lab01/ ... Lab07/   # kod wlasciwy dla kazdego laboratorium
    Shared/             # kod wspoldzielony (jesli potrzebny)
  Prefabs/
  Shaders/
  Models/           # modele CAD (Lab 5), duze pliki przez Git LFS
  Materials/
evidence/           # raporty studenckie (evidence/lab0X.md)
docs/               # dokumentacja kursu
```

## Gałęzie

| Gałąź | Zawartość |
|---|---|
| `main` | Stan zmergowany, oceniony przez prowadzącego |
| `lab0X-start` | Punkt startowy dla laboratorium X |
| `team-<NR>/lab0X-<nazwiska>` | Praca konkretnej pary nad laboratorium X |

## Workflow studenta

```bash
git clone <URL> xr-lab
cd xr-lab
git switch lab01-start
git switch -c team-<NR>/lab01-<nazwisko1>-<nazwisko2>
# praca, commity
git push -u origin team-<NR>/lab01-<nazwisko1>-<nazwisko2>
```

Pełna instrukcja: `docs/REPOSITORY_SETUP.md`

## Wymagania

- Unity 2022 LTS (patrz `ProjectSettings/ProjectVersion.txt`)
- Git LFS (`git lfs install`) dla modeli FBX i duzych tekstur
- Pakiety: patrz `docs/LAB1_STARTER_README.md`

## Ocenianie

System checkpointow 3.0-5.0, opisany w `docs/GRADING_MATRIX.md`.
Wariant diagnostyczny pary: `(numer_indeksu_1 + numer_indeksu_2) mod N`, patrz instrukcja danego Lab.

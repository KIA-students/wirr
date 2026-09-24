# Konfiguracja repozytorium — dla studentów

## Klonowanie i Konfiguracja

### 1. Klonuj repozytorium
```bash
git clone <URL_REPOZYTORIUM> xr-lab
cd xr-lab
```

### 2. Sprawdzenie wersji
```bash
# Powinno być Lab01-Lab07
git branch -a

# Pokaż aktualną wersję pakietów
cat Packages/packages-lock.json | grep "version"
```

### 3. Przygotowanie do Lab X

Dla każdego laboratorium:

```bash
# Przejdź na gałąź startową
git switch labXX-start

# Utwórz gałąź pracy
git switch -c team-<NUMER_GRUPY>/labXX-<NAZWISKO1>-<NAZWISKO2>

# Np. dla Lab 1, grupa 15:
# git switch -c team-15/lab01-kowalski-nowak
```

### 4. Pracuj i zacommituj

Podczas pracy:

```bash
# Po każdym logicznym kroku
git add Assets/Scripts/MyScript.cs
git commit -m "feat(lab01): initial benchmark implementation"

# Na koniec Lab
git push origin team-15/lab01-kowalski-nowak
```

### 5. Oddanie

Przygotuj raport wykonania:

```bash
# Skopiuj template
cp evidence_template_labXX.md evidence/labXX.md

# Wypeł raport
# (edytuj w edytorze tekstu)

# Zacommituj
git add evidence/labXX.md
git commit -m "docs(lab01): finalize evidence report"
git push
```

---

## Gałąź Main — Nie Rób Tego!

```bash
# ❌ NIE RÓB
git push origin team-15/lab01-kowalski-nowak:main

# ✓ RÓB (prowadzący merge'uje)
git push origin team-15/lab01-kowalski-nowak
```

Prowadzący będzie merge'ować każdą gałąź ręcznie (Quality Gate).

---

## Struktura Repozytorium

```
xr-lab/
├── .git/                      # Historia Git (automatyczne)
├── Assets/
│   ├── Scenes/
│   │   ├── Lab01_Baseline.unity
│   │   ├── Lab02_Interaction.unity
│   │   ├── Lab03_AR.unity
│   │   └── ... (aż do Lab07)
│   ├── Scripts/
│   │   ├── Lab01/
│   │   ├── Lab02/
│   │   └── ... (katalogi na każdy lab)
│   ├── Prefabs/
│   ├── Shaders/
│   └── Models/ (CAD dla Lab 5)
├── Packages/
│   ├── packages-lock.json    # NIE ZMIENIAJ!
│   └── manifest.json
├── ProjectSettings/
│   ├── ProjectVersion.txt    # Zawsze Unity 2022 LTS
│   └── ...
├── evidence/                  # Twoje raporty
│   ├── lab01.md
│   ├── lab02.md
│   └── ... (aż do lab07.md)
├── README.md                 # Instrukcje kursu
└── .gitignore               # Ignoruj: Library, Temp, Build, itd.
```

---

## Co NIE Wgrywaj do Git

Plik `.gitignore` powinien zawierać:

```
Library/
Temp/
Build/
*.log
*.csproj
*.sln
.DS_Store
obj/
bin/
*.asset (duże)
```

Jeśli przypadkowo wgrasz:

```bash
# Usuń z historii (OSTATNIA SZANSA!)
git rm --cached Library/ Temp/ Build/
git commit --amend
git push --force  # Tylko na własnej gałęzi!
```

---

## Tygodniowy przepływ pracy

```
Przed zajęciami:
  1. git fetch origin
  2. git switch lab0X-start
  3. git pull origin lab0X-start

Podczas zajęć:
  1. git switch -c team-NR/lab0X-...
  2. Pracuj, commituj, pushuj

Po zajęciach:
  1. Wypeł evidence/lab0X.md
  2. git add, git commit, git push
  3. Poczekaj na recenzję prowadzącego

Przejście do Lab X+1:
  1. Prowadzący merge'uje: lab0X-start → main
  2. Nowa gałąź startowa: lab0(X+1)-start
  3. Powtórz proces
```

---

## Jeśli Coś Pójdzie Nie Tak

### Merge Conflict?
```bash
# Nie wchodź w panikę!
git status  # Pokaż, które pliki w konflikcie

# Edytuj pliki, usuń markery <<<<<<, ======, >>>>>>
# Rozwiąż logicznie (np. weź wersję z naszej gałęzi)

git add <plik>
git commit -m "resolve: merge conflict in Assets/Scenes/Lab01.unity"
git push
```

### Wgrałeś Coś Dużego (Library/)?
```bash
# Natychmiast (zanim ktoś pushuje):
git reset --soft HEAD~1      # Cofnij commit, ale zachowaj pliki
git reset HEAD Library/      # Wyłącz z staging
git checkout Library/        # Przywróć z dysku
git commit -m "remove: library (oops)"
git push --force-with-lease
```

### Jesteś w Innej Gałęzi Niż Chciałeś?
```bash
git status                   # Pokaż bieżącą gałąź
git switch -c team-15/lab01-...  # Utwórz nową z tym samym kodem
git switch lab01-start       # Cofnij czasowe zmiany
```

---

## Wsparcie

- **Prowadzący:** Czat/Email na zajęciach
- **Peer review:** Każdy commit przeglądany przez parę
- **Dokumentacja:** W tym pliku + linki w `README.md`

---



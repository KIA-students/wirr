# Konfiguracja repozytorium dla studentów

Polecenia wykonuj w katalogu **własnego projektu Unity** z `Assets`, `Packages` i `ProjectSettings`. Instalacja pakietu WiRR przez UPM nie wymaga klonowania repozytorium kursu ani dostępu do zapisu.

## Spis treści

- [Przygotowanie projektu](#przygotowanie-projektu)
- [Pierwszy commit](#pierwszy-commit)
- [Praca na laboratorium](#praca-na-laboratorium)
- [Oddanie raportu](#oddanie-raportu)
- [Naprawa problemów](#naprawa-problemów)

## Przygotowanie projektu

1. Otwórz działający projekt z poprzednich zajęć. Jeżeli go nie masz, wykonaj [konfigurację środowiska](ENVIRONMENT_SETUP.md).
2. Użyj kursowej wersji Unity 6000.6.5f1 lub zgodnej z linii 6000.6.5f1. Rzeczywistą wersję zapisz w raporcie.
3. Zachowaj `Packages/manifest.json`, `Packages/packages-lock.json`, `ProjectSettings` oraz pliki `.meta` zasobów. Zmiany zależności zatwierdzaj razem ze zmianami projektu.
4. Nie wykonuj poleceń Git w `Library/PackageCache` ani w katalogu pobranego pakietu.

## Pierwszy commit

W katalogu projektu utwórz `.gitignore`:

```gitignore
[Ll]ibrary/
[Tt]emp/
[Oo]bj/
[Ll]ogs/
[Uu]ser[Ss]ettings/
[Bb]uild/
[Bb]uilds/
```

Jeżeli projekt nie ma jeszcze repozytorium:

```bash
git init
git add .gitignore Assets Packages ProjectSettings
git commit -m "Initialize Unity project with WiRR"
```

Nie ignoruj wszystkich plików `.asset`: mogą zawierać materiały i konfigurację potrzebne do odtworzenia sceny. Jeżeli masz własne zdalne repozytorium projektu, sprawdź jego adres przez `git remote -v` przed wysłaniem zmian.

## Praca na laboratorium

Z działającego stanu projektu utwórz gałąź, np. dla Lab 01:

```bash
git switch -c lab01-work
```

Jeżeli gałąź już istnieje, użyj `git switch lab01-work`. Zapisuj sceny w Unity, a następnie zatwierdzaj wykonane etapy:

```bash
git status
git add Assets Packages ProjectSettings
git commit -m "Complete Lab 01 baseline experiment"
```

Dla następnego laboratorium utwórz `lab02-work` z działającego stanu poprzedniego ćwiczenia. Kurs obejmuje osiem laboratoriów. Nie zakładaj istnienia gałęzi `labXX-start` w swoim projekcie.

Jeżeli własne repozytorium zdalne jest skonfigurowane:

```bash
git push --set-upstream origin lab01-work
```

Nie wpisuj nazwisk do nazw publicznych gałęzi. Raport i warianty są indywidualne: `S = i`, gdzie `i` to własny numer indeksu.

## Oddanie raportu

1. Otwórz formularz **WiRR → Raporty → Formularz raportu laboratoryjnego**.
2. Uzupełnij własny identyfikator, indeks, warianty i rzeczywiste dowody.
3. Wybierz **Sprawdź kompletność raportu**, popraw braki, a następnie **Wyślij raport**. Kompletność formularza nie oznacza przyznania oceny.
4. Wysyłka tworzy gałąź raportu i PR do `main` repozytorium kursu. Wymaga skonfigurowanego Git i odpowiedniego dostępu.
5. Gdy wysyłka automatyczna nie jest dostępna, użyj awaryjnego szablonu Markdown z próbki danego laboratorium, zapisz `evidence/labNN.md` i przekaż go kanałem wskazanym przez prowadzącego.

Szczegóły: [oddawanie raportów](STUDENT_SUBMISSIONS.md). Gałąź `upm` służy instalacji pakietu, a nie oddawaniu raportów.

## Naprawa problemów

- **Library trafiło do Git:** wykonaj `git rm -r --cached -- Library`, uzupełnij `.gitignore`, zatwierdź poprawkę i wyślij zwykłym push. Pliki lokalne pozostaną na dysku.
- **Konflikt scalania:** sprawdź `git status`, rozwiąż konflikt, zachowując potrzebne zmiany, a następnie zatwierdź wynik. Nie usuwaj całego projektu.
- **Push odrzucony:** sprawdź adres `origin`, dostęp i nazwę gałęzi. Nie używaj wymuszonego push jako pierwszej metody naprawy.
- **Unity nie kompiluje:** sprawdź pierwszy błąd Console oraz [FAQ](FAQ.md). Zachowaj kopię manifestu, lockfile i własnych zasobów.

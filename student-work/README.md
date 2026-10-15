# Prace laboratoryjne WiRR

Katalog zawiera dwie grupy: G1 i G2. Numer laboratorium jest w nazwie raportu; nie tworzy się podkatalogów lab01–lab08. Repozytorium jest publiczne.

## Grupy i laboratoria

| Grupa | Miejsce na raporty |\n|---|---|\n| [G1](G1/README.md) | `student-work/G1/labNN-<identyfikator>.json` |\n| [G2](G2/README.md) | `student-work/G2/labNN-<identyfikator>.json` |\n\n## Przykładowe raporty

- [Lab 01](examples/lab01-report-example.md): fikcyjne FPS, mediany i wniosek.
- [Lab 02](examples/lab02-report-example.md): fikcyjne próby interakcji, błędy i mediana.
- [Lab 03](examples/lab03-report-example.md): śledzenie AR i wykrywanie płaszczyzn.
- [Lab 04](examples/lab04-report-example.md): dostępność Depth API i successRate.
- [Lab 05](examples/lab05-report-example.md): audyt konwersji CAD i siatki.
- [Lab 06](examples/lab06-report-example.md): źródło JointState w wariancie replay.
- [Lab 07](examples/lab07-report-example.md): kryteria akceptacji, PASS/FAIL/NV.
- [Lab 08](examples/lab08-report-example.md): demonstrator integracyjny, próby A/B.

## Przygotowanie raportu

1. W Unity otwórz **WiRR → Student → Profil i grupa** i uzupełnij profil.
2. Przejdź do **WiRR → Raporty → Formularz raportu laboratoryjnego**, wybierz właściwe laboratorium.
3. Wpisz rzeczywiste warunki eksperymentu, surowe pomiary, obliczenia i wnioski.
4. Użyj **Sprawdź kompletność raportu**, następnie **Eksportuj raport lokalnie**.
5. Plik JSON w `Library/WiRRReports/exports` przekaż kanałem wyznaczonym przez prowadzącego.

**Nie umieszczaj** w tym publicznym repozytorium numerów indeksów, nazwisk, indywidualnych wyników SSQ, ocen ani identyfikujących raportów. Same foldery G1/G2 służą orientacji i ewentualnej publikacji materiałów zbiorczych, anonimowych, zatwierdzonych przez prowadzącego. Eksport JSON nie oznacza automatycznego oddania pracy.

## Wspólny klucz kursu

Pakiet Unity zawiera publiczny identyfikator kursu. Szczegóły: [PUBLIC_COURSE_KEY.md](../Documentation~/Reference/PUBLIC_COURSE_KEY.md). Nie zastępuje on logowania w obecnej usłudze KIA; endpoint bez logowania wymaga osobnego wdrożenia.

## Ręczne dodawanie przez GitHub

Zobacz [instrukcję wgrywania anonimowych materiałów przez stronę GitHub](HOW_TO_UPLOAD_GITHUB.md). Publiczny PR nie zastępuje wysłania rzeczywistego raportu do kolejki KIA.

# Ręczne dodanie raportu WiRR przez GitHub

**Uwaga:** repozytorium [KIA-students/wirr](https://github.com/KIA-students/wirr) jest publiczne. Jeżeli raport zawiera numer indeksu, imię i nazwisko, indywidualne dane SSQ albo inne dane identyfikujące, **nie publikuj go na GitHubie**. Użyj wysyłki z Unity do kolejki KIA albo kanału wskazanego przez prowadzącego.

## Najpierw przygotuj raport w Unity

1. Wybierz **WiRR → Raporty → Formularz raportu laboratoryjnego** i numer laboratorium.
2. Wpisz jeden numer indeksu (samodzielnie) albo zaznacz **Pracujemy w parze** i uzupełnij drugi numer. Sprawdź warianty wyliczone z sumy indeksów.
3. Wypełnij pola, sprawdź kompletność, a następnie wybierz **Zapisz raport lokalnie (bez logowania)**.
4. Plik znajdziesz w katalogu `<projekt Unity>/Library/WiRRReports/exports`. Przycisk **Otwórz folder wyeksportowanych raportów** otwiera tę lokalizację. W celu poprawienia istniejącego raportu wybierz **Otwórz zapisany raport JSON do edycji** i zapisz ponownie.
5. **Przed ewentualnym upublicznieniem utwórz osobną kopię bez danych identyfikujących.** Nie zmieniaj oryginalnego raportu przeznaczonego do oceny.

## Dodanie anonimowego materiału przez przeglądarkę

1. Zaloguj się na [GitHub](https://github.com) do swojego konta (tryb ręczny wymaga konta GitHub; automatyczna wysyłka z Unity nie wymaga logowania).
2. Otwórz folder grupy i laboratorium: [G1](../G1/README.md) lub [G2](../G2/README.md), następnie `lab01` ... `lab08`.
3. Wybierz **Add file → Upload files**, przeciągnij **anonimową kopię** dokumentu i wpisz zwięzły opis zmiany.
4. Jeżeli masz prawo zapisu, wybierz utworzenie oddzielnej gałęzi i **Propose changes**, a następnie utwórz Pull Request. Jeśli nie masz prawa zapisu, GitHub może zaproponować fork repozytorium; jeśli nie, poproś prowadzącego o kanał dostarczenia pliku.
5. Po utworzeniu PR poczekaj na weryfikację prowadzącego. **Utworzenie PR nie oznacza zaliczenia** ani automatycznego opublikowania raportu.

Przykłady zawartości (bez danych studentów): [Lab 01](../examples/lab01-report-example.md), [Lab 02](../examples/lab02-report-example.md), [Lab 03](../examples/lab03-report-example.md) itd.

**Ważne:** Git i historia PR mogą trwale zachowywać dane również po usunięciu pliku. Nie przesyłaj na publiczny GitHub raportów identyfikujących studentów ani wrażliwych wyników.

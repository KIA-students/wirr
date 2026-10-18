# Standard nawigacji w instrukcjach WiRR

## Spis treści

- [Zasada](#zasada)
- [Pierwszy rozdział: ścieżka warunkowa](#pierwszy-rozdział-ścieżka-warunkowa)
- [Spis treści i kotwice](#spis-treści-i-kotwice)
- [Odsyłacze do poprzednich laboratoriów](#odsyłacze-do-poprzednich-laboratoriów)
- [Kontrola jakości](#kontrola-jakości)

## Zasada

Kampania obejmuje **dokładnie cztery bloki i osiem laboratoriów (01–08)**. Każda instrukcja studentów ma **spis treści w pierwszym ekranie** oraz pytania warunkowe, które pozwalają pominąć czynności już wykonane. Nie opisuj od nowa instalacji Git, Unity Hub, Android SDK, ARCore ani ROS 2, jeśli istnieje aktualna, wspólna instrukcja.

## Pierwszy rozdział: ścieżka warunkowa

Pisz konkretnie, na przykład:
- Jeśli masz **zainstalowane Unity z aktywną licencją**, przejdź do sekcji „Pierwsze doświadczenie”.
- Jeśli **nie masz Unity**, skorzystaj z [instalacji](OS_SETUP.md#unity-nie-jest-zainstalowane).
- Jeśli **nie masz konta**, przeczytaj [instrukcję licencji](UNITY_ACCESS_AND_LICENSES.md#nawigacja-warunkowa).
- Jeśli pracujesz na **Windows**, przejdź do [Windows](OS_SETUP.md#windows); jeśli **Ubuntu**, przejdź do [Ubuntu](OS_SETUP.md#ubuntu).
- Jeśli brak sprzętu XR, wykorzystaj równoważną demonstrację na monitorze, nie przypisując jej pomiarów z HMD.

## Spis treści i kotwice

Używaj czytelnych nagłówków Markdown i lokalnych fragmentów URL, np. [Lab 02: chwyt](../../Samples~/Lab02/README.md#30-chwyt-socket-i-ray). Unikaj powtarzających się nagłówków o tej samej nazwie, ponieważ GitHub nada im kolejne numery w identyfikatorach. Stosuj polskie znaki, a fragmenty testuj programowo.

## Odsyłacze do poprzednich laboratoriów

- AR Foundation w Lab 04: [przygotowanie AR z Lab 03](../../Samples~/Lab03/README.md#przygotowanie).
- Wydajność w Lab 07: [pierwszy pomiar z Lab 01](../../Samples~/Lab01/README.md#30-pomiar-bazowy-pc).
- Komfort XR: [wyjaśnienie wergencji i akomodacji](COMFORT_STUDIES.md).
- Safe XR Watchdog: [10 prób technicznych](XR_SAFETY_WATCHDOG.md).

## Kontrola jakości

Uruchom **lokalnie**, po zmianie struktury instrukcji, w tym Lab 08 i pięciu plików Documentation~/Campaign/:

    python3 scripts~/verify_instruction_navigation.py

Program sprawdza spisy treści, istnienie względnych odnośników i kotwice nagłówków. Nie wymaga Unity, konta GitHub, dostępu do sieci ani workflow GitHub Actions. CI nie powinien uruchamiać kompilacji PDF/Unity przy każdej korekcie tekstu.

**Uwaga:** istniejące stare README/plik LaTeX są zachowane jako legacy. Aktualizacje nawigacji i przekierowania kierują do nowych, zatwierdzonych instrukcji.

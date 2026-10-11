# Misje WiRR: cztery bloki i lokalna karta sesji (wersja robocza)

## Spis treści

- [Przed wydaniem produkcyjnym](#przed-wydaniem-produkcyjnym)
- [Konto Unity, opcjonalne misje i komfort XR](#konto-unity-opcjonalne-misje-i-komfort-xr)
- [Ścieżki warunkowe i doświadczenia komfortu](#ścieżki-warunkowe-i-doświadczenia-komfortu)

## Przed wydaniem produkcyjnym
- uruchomić okno w czystym Unity 6000.6.x na Windows i Ubuntu;
- przetestować zapis Markdown i odczyt przykładowych JSON;
- sprawdzić, że pobieranie danych nie wymaga Internetu ani tokenów;
- potwierdzić, że nie ma błędów kompilacji C# w Editor;
- opublikować razem z nowymi instrukcjami WiRR z repozytorium repozytorium instrukcji.

Trzeci prefab **WiRR_Comfort_SafetyWatchdog** umożliwia symulację zdjęcia gogli, utraty trackingu lub dużego skoku danych, z lokalnym limitem 10 prób i raportem wyłącznie technicznym. Szczegóły: [XR_SAFETY_WATCHDOG.md](XR_SAFETY_WATCHDOG.md).

Do czasu tego testu jest to funkcja **gałęzi rozwojowej**, nie potwierdzona funkcja wydania produkcyjnego.


## Konto Unity, opcjonalne misje i komfort XR
- **Nie udostępniaj współdzielonego hasła Unity**. Na stanowiskach uczelni wybierz aktywowaną przez administratora Education Grant License, a prywatny komputer może korzystać z indywidualnego Unity Personal/Student. Instrukcja: **UNITY_ACCESS_AND_LICENSES.md**.
- Generator zabawnych nazw w oknie **Misje i odkrycia** jest lokalny; propozycje zatwierdza prowadzący. Także strona WiRR ma generator i cztery dodatkowe, dobrowolne wyzwania z limitem **+8 punktów na grupę**, niezależne od ocen indywidualnych.
- **WiRR → Komfort XR** generuje prefab bardzo łagodnego ruchu odniesienia i prefab obserwacji wirtualnej głębi; oba są uruchamiane wyłącznie dobrowolnie i samoczynnie kończą próbę. Szczegóły: **COMFORT_STUDIES.md**. Nie należy wywoływać ani porównywać objawów zdrowotnych studentów.
- Materiały i kod pozostają w gałęzi roboczej aż do testu Unity. Zmiany w rankingach mogą być synchronizowane po zatwierdzeniu; stan wyników repozytorium studentów nie może być zastąpiony kopią roboczą.

## Ścieżki warunkowe i doświadczenia komfortu

Konto/licencję sprawdź przez [instrukcję Unity](UNITY_ACCESS_AND_LICENSES.md#nawigacja-warunkowa); instalację rozdzielają [Windows i Ubuntu](OS_SETUP.md#wybierz-aktualną-sytuację). WiRR generuje [trzy prefaby komfortu i bezpieczeństwa](COMFORT_STUDIES.md), w tym [dziesięciopróbowy watchdog](XR_SAFETY_WATCHDOG.md). Żadne z tych zadań nie wymaga wywoływania objawów u uczestnika.

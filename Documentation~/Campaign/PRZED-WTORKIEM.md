# Lista gotowości na poniedziałek 12 października 2026

**Tylko dla prowadzącego.** Najbliższe zajęcia: **wtorek 13 października**, blok I (Lab 01 + Lab 02). Ta lista pozwala w krótkim czasie sprawdzić, czy studenci będą mogli od razu wykonywać eksperymenty, nawet gdy na poszczególnych stanowiskach nie ma sprzętu XR.

## Spis treści
- [Przed uruchomieniem](#przed-uruchomieniem)
- [Próba kontrolna Unity](#próba-kontrolna-unity)
- [Jedna para gogli i kontrolery](#jedna-para-gogli-i-kontrolery)
- [Wariant awaryjny bez urządzeń](#wariant-awaryjny-bez-urządzeń)
- [Materiały udostępniane studentom](#materiały-udostępniane-studentom)
- [Brak zielonego testu](#brak-zielonego-testu)

## Przed uruchomieniem

- [ ] Komputer pracowni z Windows i, jeśli planowane, Ubuntu ma wskazaną wersję **Unity 6000.6.x**, najlepiej dokładnie tę samą co projekt.
- [ ] Zatwierdzona licencja Unity aktywowana przez administratora. Nie udostępniaj studentom loginu i hasła konta funkcyjnego. W przypadku nieaktywnej licencji przygotuj alternatywne, zgodnie z zasadami licencyjnymi, stanowisko.
- [ ] Gotowy projekt URP jest uruchamiany jednym kliknięciem z Unity Hub. Nie potrzeba rozszerzeń XR do pierwszej części Lab 01.
- [ ] Treść pierwszego bloku jest dostępna: [BLOK-01.md](BLOK-01.md), [Lab 01](../../Samples~/Lab01/README.md), [Lab 02](../../Samples~/Lab02/README.md).
- [ ] Prowadzący zna [ścieżkę instalacji Windows/Ubuntu](../Reference/OS_SETUP.md) i udostępnia ją studentom zamiast odtwarzać czynności z pamięci.

## Próba kontrolna Unity

1. Uruchom **WiRR → Misje i odkrycia → Start: Lab 01**. Sprawdź: pojawia się kamera, podłoga i światło.
2. Dodaj sześcian, Rigidbody i sprawdź, czy opada. Otwórz **Window → Analysis → Profiler** i odczytaj FPS, czas klatki.
3. Uruchom **Start: Lab 02**. Jeśli XRI jest już zainstalowane, uruchom w symulatorze prosty chwyt. Jeżeli nie, przygotuj demonstrator bazowy i krótkie objaśnienie logiki chwytu, bez instalowania XRI podczas zajęć.
4. Wybierz w panelu misji **blok I**, wykonaj **Zapisz lokalną kartę sesji**, zweryfikuj ścieżkę i polskie znaki. Nie udawaj pomiarów z Quest.
5. Sprawdź Console: **brak błędów kompilacji**. Jeśli nowe C# nie przeszły kompilacji, przygotuj czysty projekt URP jako wariant awaryjny, a nie wymagaj jego naprawiania od studentów.

## Jedna para gogli i kontrolery

- [ ] Załadowany Quest 3, wyczyszczone powierzchnie kontaktowe, wolna przestrzeń, osoba obserwująca.
- [ ] Przewód USB-C obsługuje dane, nie tylko ładowanie.
- [ ] W wariancie APK komputer rozpoznaje urządzenie przez **adb devices -l**, a w goglach zaakceptowano autoryzację debugowania.
- [ ] W XRI jasno rozróżnia się trigger, grip, joystick i przycisk systemowy. Nie zakładaj identycznego przypisania we wszystkich runtime.
- [ ] Osoby niechętne ekspozycji w VR mają pełnowartościowe zadanie obserwatora lub operatora symulatora.

## Wariant awaryjny bez urządzeń

Gdy brakuje gogli lub SDK, **Lab 01** wykonaj na czystym Unity URP z fizyką i Profilerem. **Lab 02** wykonaj na zbudowanej scenie interakcji z wizualizacją chwytu i ograniczeń myszy. Oznacz tryb **UNITY_EDITOR / XR_SIM**, bez wprowadzania fikcyjnych wyników Quest 3. Nie odwołuj zajęć z powodu niedostępnej usługi sieciowej.

## Materiały udostępniane studentom

- [Kampania: 4 × 2](README.md), [blok I](BLOK-01.md), [Lab 01](../../Samples~/Lab01/README.md), [Lab 02](../../Samples~/Lab02/README.md).
- [Konto Unity](../Reference/UNITY_ACCESS_AND_LICENSES.md) i [Windows/Ubuntu](../Reference/OS_SETUP.md).
- [Komfort XR](../Reference/COMFORT_STUDIES.md) jako **dobrowolne** rozszerzenie, bez wywoływania objawów.
- [Liga XR](../../competition/OPTIONAL_MISSIONS.md) w tle eksperymentów, bez wpływu na oceny indywidualne.

Nie przekazuj studentom gałęzi z nieprzetestowanym narzędziem jako stabilnego release. Instrukcje Markdown mogą być udostępnione wcześniej; demonstrator Unity musi być najpierw zweryfikowany w edytorze.

## Brak zielonego testu

Jeżeli na poniedziałkowej próbie Unity nie przejdzie kompilacji nowego pakietu, realizuj blok I na **istniejącym działającym Unity URP**. Instrukcje zawierają ćwiczenia niewymagające WiRR. Zapisz pierwszy komunikat błędu i wersję edytora do naprawy, bez przerywania dydaktyki.

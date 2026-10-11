# VITURE XR: przykładowe sceny WiRR

## Spis treści

- [Szybki wybór ścieżki](#szybki-wybór-ścieżki)
- [Sceny](#sceny)
  - [VITURE_01_StereoDepthLayers.unity](#viture_01_stereodepthlayersunity)
  - [VITURE_02_StereoComfort.unity](#viture_02_stereocomfortunity)
  - [VITURE_03_Immersive2D_Source.unity](#viture_03_immersive2d_sourceunity)
- [Alternatywa: wygenerowanie scen w projekcie](#alternatywa-wygenerowanie-scen-w-projekcie)
- [Szybka konfiguracja](#szybka-konfiguracja)
- [Dodatkowe, dobrowolne doświadczenie: komfort VR i głębia](#dodatkowe-dobrowolne-doświadczenie-komfort-vr-i-głębia)


## Szybki wybór ścieżki
- **Mam kompatybilne okulary i wyjście USB-C z wideo:** przejdź do [szybkiej konfiguracji](#szybka-konfiguracja), a następnie [StereoDepthLayers](#viture_01_stereodepthlayersunity).
- **Nie mam okularów:** najpierw zobacz sceny w oknie Game lub na monitorze, porównaj wynik 2D i parę SBS. Nie twierdź, że testowałeś fizyczne okulary.
- **Obraz jest czarny:** sprawdź wideo USB-C / DP Alt Mode i [system operacyjny](../../Documentation~/Reference/OS_SETUP.md).
- **Chcę sprawdzić konflikty wskazówek głębi:** [komfort stereo i akomodacja](../../Documentation~/Reference/COMFORT_STUDIES.md#eksperyment-b-wirtualna-głębia-i-stała-odległość-optyczna).


Sample zawiera trzy gotowe sceny Unity do prezentacji działania okularów VITURE XR.

## Sceny

### VITURE_01_StereoDepthLayers.unity

Demonstracja natywnej stereoskopii Full SBS.

Po wejściu w Play Mode scena tworzy:
- dwie kamery Full SBS;
- obiekty na kilku odległościach od obserwatora;
- znaczniki głębi rozmieszczone wzdłuż osi Z.

Użycie:
1. Otwórz scenę.
2. Uruchom Play Mode.
3. Ustaw Game View lub build na 3840×1080.
4. Podłącz okulary VITURE.
5. Włącz tryb 3D.
6. Porównaj dysparację obiektów bliskich i dalekich.

### VITURE_02_StereoComfort.unity

Scena służy do demonstracji wpływu dysparacji na komfort widzenia.

Zawiera:
- płaszczyznę odniesienia;
- zestaw punktów na kilku głębokościach;
- dalszy obiekt referencyjny.

Najpierw użyj geometrii stereo zatwierdzonej dla urządzenia. **Nie manipuluj agresywnie parametrem ipdMeters ani dysparycją w celu wywołania bólu oczu lub dwojenia.** Celem jest wyjaśnienie, że optyczna odległość ogniskowania w konwencjonalnym wyświetlaczu jest stała, podczas gdy wskazówki wergencji wynikają z geometrii treści. Zwykła scena Unity nie mierzy fizjologicznej akomodacji.

### VITURE_03_Immersive2D_Source.unity

To celowo zwykła scena 2D z silnymi wskazówkami perspektywicznymi:
- obiektami na różnych odległościach;
- zmianą skali;
- przesłanianiem;
- perspektywą liniową.

Użycie:
1. Otwórz scenę i uruchom ją w zwykłym trybie 2D.
2. Zaobserwuj obraz bez natywnego stereo.
3. Uruchom VITURE Immersive 3D / SpaceWalker.
4. Włącz 2D-to-3D.
5. Porównaj efekt z natywnym Full SBS z pierwszej sceny.

## Alternatywa: wygenerowanie scen w projekcie

Jeśli chcesz otrzymać świeże sceny zapisane bezpośrednio przez aktualnie używaną wersję Unity, wybierz:

`WiRR → VITURE XR → Wygeneruj 3 sceny demonstracyjne`

WiRR zapisze je do:

`Assets/WiRR/VITURE/Demos`

Ta metoda jest zalecanym fallbackiem, jeśli przyszła wersja Unity zmieni format serializacji scen.

## Szybka konfiguracja

Dla natywnego Full SBS:
- docelowy framebuffer: 3840×1080;
- lewa połowa: lewe oko;
- prawa połowa: prawe oko;
- domyślny rozstaw kamer: 0,064 m;
- osie kamer: równoległe.

Dla Immersive 3D:
- pozostaw scenę źródłową jako zwykłe 2D;
- konwersję wykonuje oprogramowanie VITURE, a nie komponent Unity.

Dokumentacja: `Documentation~/Reference/VITURE_XR.md`.


## Dodatkowe, dobrowolne doświadczenie: komfort VR i głębia

Wybierz **WiRR → Komfort XR → Utwórz 3 prefaby: obserwacja i watchdog**. Powstaną prawdziwe prefaby Unity z łagodnym ruchem wskazówek wizualnych (maks. 8 s) i krótkim porównaniem wirtualnej głębi (maks. 10 s). Ich zadaniem nie jest wywoływanie nudności, lecz nauka projektowania komfortowych scen.

Instrukcja pełna: **Documentation~/Reference/COMFORT_STUDIES.md**. Przy pracy w goglach potrzebny jest świadomy ochotnik, osoba obserwująca i możliwość natychmiastowego przerwania. Reszta grupy może wykonać równoważną analizę na monitorze. **Nie przyznaje się punktów za fizjologiczną odporność ani wystąpienie objawów.**

## Wyzwanie dodatkowe: operator potrafi bezpiecznie przerwać

Po zrealizowaniu podstawowego doświadczenia sprawdź [automatyczną detekcję zdjęcia gogli, utraty śledzenia i skoku danych](../../Documentation~/Reference/XR_SAFETY_WATCHDOG.md). Możesz wykonać do 10 krótkich prób technicznych, także wyłącznie na PC. **Nie wykonuj nagłych ruchów głową dla punktów.** Punktuje się poprawność prototypu i interpretacji, a nie podatność na objawy.

# VITURE XR: przykładowe sceny WiRR

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

Można zmieniać `ipdMeters` w komponencie `WiRRVitureDemoBootstrap` i obserwować zmianę odczuwanej głębi. Nie należy zwiększać bazy stereo agresywnie; celem jest porównanie, a nie maksymalizacja efektu 3D.

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

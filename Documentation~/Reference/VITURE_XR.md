# VITURE XR: stereoskopia SBS i Immersive 3D

> **Ważne dla posiadanych VITURE XR Luma Ultra:** [pełna procedura podłączenia, 2D, Full SBS, Windows/Ubuntu i SpaceWalker](DEVICE_DEPLOYMENT_QUEST_ANDROID_VITURE.md#3-viture-xr-luma-ultra-fizyczne-podłączenie-2d-full-sbs-i-spacewalker). Dotychczasowy przykład generatora WiRR jest **3840×1080**, podczas gdy Luma Ultra ma **1920×1200 na oko** (natywna para **3840×1200**). Nie ogłaszaj działania 1200p, dopóki nie zweryfikujesz obsługi tej rozdzielczości w generatorze i fizycznym torze wideo.

## Spis treści

- [Trzy różne tryby](#trzy-różne-tryby)
  - [1. Zwykłe 2D](#1-zwykłe-2d)
  - [2. Natywne 3D Full SBS](#2-natywne-3d-full-sbs)
  - [3. Immersive 3D: 2D-to-3D](#3-immersive-3d-2d-to-3d)
- [Przełączanie 2D / 3D](#przełączanie-2d--3d)
  - [VITURE One i VITURE Pro](#viture-one-i-viture-pro)
  - [VITURE Luma Series](#viture-luma-series)
- [Odtwarzanie pliku 3D SBS](#odtwarzanie-pliku-3d-sbs)
- [Przykład Unity](#przykład-unity)
- [Ćwiczenie porównawcze](#ćwiczenie-porównawcze)
- [Oficjalne materiały](#oficjalne-materiały)
- [Opcjonalny eksperyment percepcji głębi](#opcjonalny-eksperyment-percepcji-głębi)

## Trzy różne tryby

### 1. Zwykłe 2D

Okulary działają jak zewnętrzny ekran. Oba oczy otrzymują ten sam obraz.

### 2. Natywne 3D Full SBS

Okulary VITURE wyświetlają Full SBS: dwa obrazy obok siebie. W wielu starszych trybach jest to **1920×1080 na oko**, razem **3840×1080**. **VITURE Luma Ultra natywnie oferuje 1920×1200 na oko**, czyli **3840×1200 łącznie**, o ile zgodny tor wideo i aplikacja obsługują ten format. Parametry i źródło: [specyfikacja Luma Ultra](https://www.viture.com/academy/product/viture-luma-ultra-dock-pack).

W Unity użyj:

`WiRR → VITURE XR → Utwórz przykład Full SBS 3840x1080`

**Uwaga:** to obecnie generator demonstracyjny **1080p**, nie potwierdzony profil natywny Luma Ultra 1200p. Nie zmieniaj wymiarów bez kontroli render texture, viewportów i fizycznego testu. Generator tworzy:
- `WiRR_VITURE_FullSBS`;
- `VITURE_LeftEye`;
- `VITURE_RightEye`;
- komponent `WiRRVitureSbsRig`.

Kamery używają równoległych osi. Domyślna baza stereo wynosi 0,064 m i może być zmieniona w Inspectorze.

### 3. Immersive 3D: 2D-to-3D

Immersive 3D jest funkcją programową VITURE. Analizuje kolejne klatki zwykłej treści 2D i tworzy z nich parę stereoskopową. Nie jest to pomiar głębi i nie należy traktować wygenerowanej geometrii jako danych referencyjnych.

Producent udostępnia Immersive 3D:
- jako rozwiązanie desktopowe na Windows i macOS;
- w SpaceWalker na obsługiwanych platformach mobilnych;
- w wybranych urządzeniach VITURE, np. Pro Neckband, zgodnie z bieżącą dokumentacją producenta.

Dostępne są poziomy głębokości Soft, Standard i Enhanced oraz tryby zoptymalizowane pod film i grę.

## Przełączanie 2D / 3D

### VITURE One i VITURE Pro

Naciśnij i przytrzymaj krótki przycisk Mode na lewym zauszniku.

### VITURE Luma Series

Naciśnij i przytrzymaj R1.

Funkcje przycisków mogą zmieniać się wraz z firmware. Przed laboratorium zaktualizuj okulary i w razie rozbieżności sprawdź bieżącą instrukcję VITURE Academy dla konkretnego modelu.

## Odtwarzanie pliku 3D SBS

1. Podłącz okulary VITURE do urządzenia obsługującego obraz przez USB-C/DisplayPort lub odpowiedni adapter.
2. Otwórz materiał Full SBS.
3. Dla starszego przykładu WiRR użyj **3840×1080** (1920×1080 na oko); dla **Luma Ultra** zweryfikuj tryb **3840×1200** (1920×1200 na oko). Użyta wartość musi odpowiadać rzeczywistemu wyjściu i trybowi urządzenia; nie deklaruj pełnej jakości natywnej na podstawie przykładu 1080p.
4. Włącz tryb 3D w okularach.
5. Jeżeli nadal widzisz dwa obrazy obok siebie, sprawdź tryb okularów, proporcje playera i format źródła.
6. Na PC można użyć np. Kodi. Na Androidzie producent wskazuje VLC jako praktyczny odtwarzacz.

Nie utożsamiaj Full SBS z Half-SBS. VITURE natywnie oczekuje Full SBS; Half-SBS może wymagać przetworzenia przez player lub urządzenie pośredniczące.

## Przykład Unity

Komponent `WiRRVitureSbsRig`:
- dzieli ekran na dwie równe pionowe połowy;
- ustawia lewą kamerę na `x=-IPD/2`;
- ustawia prawą kamerę na `x=+IPD/2`;
- zachowuje równoległe osie kamer;
- nie korzysta z Unity XR stereo target, ponieważ wynik ma być pojedynczym framebufferem SBS.

Dla **istniejącego** generatora WiRR ustaw 3840×1080 i zweryfikuj wynik. Dla natywnego Luma Ultra docelowy format to **3840×1200**, ale wymaga dostosowania/testu render targetów, obsługi rozdzielczości wyjścia i fizycznego urządzenia. Nie traktuj obu formatów jako równoważnych.

## Ćwiczenie porównawcze

Porównaj trzy warunki:
1. 2D;
2. natywne Full SBS z dwóch kamer Unity;
3. Immersive 3D wygenerowane z obrazu 2D.

Możliwe pomiary:
- komfort;
- postrzegana głębia;
- czytelność tekstu;
- opóźnienie;
- FPS;
- subiektywna naturalność obrazu;
- błędy percepcji głębi.

Nie używaj wyniku Immersive 3D do pomiarów metrologicznych geometrii.

## Oficjalne materiały

- Luma Series, przyciski (R1 dłużej = 2D/3D): https://www.viture.com/en-MY/academy/xr-glasses/luma-series
- Luma Ultra, parametry 1200p/6DoF: https://www.viture.com/academy/product/viture-luma-ultra-dock-pack
- SpaceWalker Desktop Windows/macOS, połączenie USB z danymi i wideo: https://www.viture.com/academy/spacewalker/desktop
- Procedura stanowiska WiRR: [Quest/Android/VITURE](DEVICE_DEPLOYMENT_QUEST_ANDROID_VITURE.md)

**Bezpieczna konfiguracja:** do odbioru wymagana jest demonstracja 2D, potem Full SBS tylko gdy działa. Samo podłączenie jako monitor nie udostępnia w Unity trackingu 6DoF ani dostępu do kamer Luma Ultra. Na Windows nie wyłączaj Secure Boot/BitLocker ani nie włączaj test-signing na urządzeniach uczelni bez zgody administratora; jeśli sterownik SpaceWalker tego wymaga, użyj demonstracji 2D/Full SBS bez SpaceWalker.

- VITURE Academy: https://www.viture.com/academy
- VITURE Download Center: https://www.viture.com/academy/download
- Immersive 3D: https://www.viture.com/blog/a-worlds-first-turn-2d-into-magical-3d-in-real-time

## Opcjonalny eksperyment percepcji głębi

Porównanie nie wymaga zwiększania wergencji lub dysparycji do wartości powodujących dyskomfort. Jeżeli chcesz zobaczyć zależność między sygnałami stereoskopowymi a stałą odległością optyczną, przejdź do [bezpiecznej demonstracji](COMFORT_STUDIES.md#eksperyment-b-wirtualna-głębia-i-stała-odległość-optyczna). Przy braku okularów skorzystaj z monitora i porównania dwóch renderów.

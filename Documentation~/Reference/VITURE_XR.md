# VITURE XR: stereoskopia SBS i Immersive 3D

WiRR obsługuje okulary VITURE XR jako zewnętrzny stereoskopowy wyświetlacz Full SBS oraz jako platformę do demonstracji różnicy między natywnym renderowaniem stereo a programową konwersją 2D-to-3D.

## Trzy różne tryby

### 1. Zwykłe 2D

Okulary działają jak zewnętrzny ekran. Oba oczy otrzymują ten sam obraz.

### 2. Natywne 3D Full SBS

VITURE XR obsługuje Full SBS. Dwa obrazy są umieszczone poziomo obok siebie. Dla pełnej rozdzielczości każde oko otrzymuje 1920×1080, dlatego cały framebuffer ma 3840×1080.

W Unity użyj:

`WiRR → VITURE XR → Utwórz przykład Full SBS 3840x1080`

Generator tworzy:
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
3. Dla pełnej jakości użyj 3840×1080, czyli 1920×1080 na oko.
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

Dla buildu PC ustaw docelową rozdzielczość okna lub pełnego ekranu na 3840×1080.

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

- VITURE Academy: https://www.viture.com/academy
- VITURE Download Center: https://www.viture.com/academy/download
- Immersive 3D: https://www.viture.com/blog/a-worlds-first-turn-2d-into-magical-3d-in-real-time

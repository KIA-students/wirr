# Mobile AR Showcase — rozszerzona rzeczywistość na smartfonie

Zestaw **Mobile AR Showcase** zawiera pięć gotowych demonstratorów zaprojektowanych pod klasyczny scenariusz AR na telefonie: kamera tylna, ekran dotykowy, AR Foundation/ARCore i obserwowanie świata przez ekran urządzenia.

Każdy prefab ma przypisany skrypt Runtime oraz fallback do testów w Unity Editor. Wersja urządzeniowa pozostaje provider-neutralna: kod studenta może podać wyniki AR Foundation do prostego API WiRR bez dodawania zależności AR Foundation do Runtime pakietu.

Fallbacki są celowo aktywne **tylko w Unity Editor**. W buildzie Android brak danych od providera AR oznacza brak hitu/trackingu/estymacji zamiast sztucznego wyniku; dzięki temu nie da się pomylić symulacji z realnym pomiarem na telefonie.

Prefaby powstają w `Assets/WiRR/Common/Prefabs/MobileAR`, a ich instancje w scenie w `WiRR_TeachingAssets/MobileARShowcase`.

## 1. AR_TapPlacement

**Efekt:** reticle śledzi rzeczywistą powierzchnię pod palcem, a dotknięcie ekranu umieszcza holograficzny obiekt w świecie.

**Skrypt:** `WiRRMobileARTapPlacement`.

**API:**
- `SetSurfaceHit(point, normal, valid, confidence)` — aktualny wynik raycastu;
- `SetSurfacePose(pose, valid, confidence)` — alternatywnie gotowa poza;
- `ConfirmPlacement()` — umieszcza obiekt;
- `ClearPlacement()` — usuwa bieżące umieszczenie.

Na telefonie adapter zwykle wykorzystuje `ARRaycastManager`. W Editorze skrypt używa `Physics.Raycast` do colliderów sceny.

**Co warto badać:** czas pierwszej detekcji powierzchni, błąd skali 1:1, stabilność reticle, zachowanie na krawędzi plane i różnicę plane raycast vs depth raycast.

## 2. AR_ImageMarkerPortal

**Efekt:** rzeczywisty obraz/marker staje się kotwicą miniaturowego portalu i wirtualnej informacji.

**Skrypt:** `WiRRMobileARImageAnchor`.

**API:**
- `SetTrackedImagePose(name, position, rotation, physicalSize, tracked, confidence)`;
- `LostTracking()`;
- właściwości `TrackedImageName`, `IsTracked`, `Confidence`.

Adapter AR Foundation korzysta z `ARTrackedImageManager` i Reference Image Library. Fizyczny rozmiar obrazu jest wykorzystywany do skalowania zawartości.

**Co warto badać:** dystans i kąt rozpoznania, częściowe zasłonięcie, czas odzyskania trackingu, wpływ jakości tekstury markera oraz błędu zadeklarowanego rozmiaru fizycznego.

## 3. AR_WorldRuler

**Efekt:** pierwszy tap ustala punkt A, drugi B, a wirtualna linia i podziałka pokazują odcinek w realnym świecie.

**Skrypt:** `WiRRMobileARRuler`.

**API:**
- `SetCandidatePoint(point, normal, valid)`;
- `ConfirmCandidate()`;
- `SetPoint(index, point)`;
- `ResetMeasurement()`;
- `DistanceMeters`.

Komponent celowo nie wymaga TextMeshPro. Student może wyświetlić `DistanceMeters` w dowolnym własnym UI.

**Co warto badać:** błąd pomiaru względem wzorca, zależność błędu od odległości, kąta obserwacji, rodzaju raycastu i jakości trackingu.

## 4. AR_LightMatchObject

**Efekt:** wirtualny obiekt reaguje na realne warunki oświetleniowe: jasność, barwę i kierunek głównego światła.

**Skrypt:** `WiRRMobileARLightMatch`.

**API:**
- `SetLightEstimate(normalizedIntensity, lightColor, mainLightDirection, confidence)`;
- właściwości `Intensity01`, `Confidence`.

Adapter AR Foundation zwykle korzysta z `ARCameraManager.frameReceived`. Należy pamiętać, że konkretne urządzenie może udostępniać tylko część pól Light Estimation.

**Co warto badać:** dopasowanie obiektu w jasnym/ciemnym środowisku, różnicę z/bez light estimation, dostępność parametrów na różnych telefonach oraz opóźnienie reakcji.

## 5. AR_SurfacePainter

**Efekt:** przeciąganie palcem po ekranie pozostawia wirtualny ślad na realnej podłodze, stole lub ścianie.

**Skrypt:** `WiRRMobileARSurfacePainter`.

**API:**
- `SetSurfaceHit(point, normal, valid)`;
- `PaintAtCurrentHit()`;
- `BeginStroke()` / `EndStroke()`;
- `ClearPaint()`;
- `PaintedCount`.

Prefab używa puli 120 markerów i nie tworzy nowych GameObjectów podczas rysowania. W urządzeniu adapter aktualizuje hit dla aktualnej pozycji palca.

**Co warto badać:** ciągłość śladu, jitter, różnicę plane/depth raycast, koszt dużej liczby punktów i zachowanie przy granicy kilku wykrytych płaszczyzn.

## Startery AR Foundation

Przycisk `Utwórz 5 starterów AR Foundation` tworzy w:

`Assets/WiRR/LabXX/Scripts/MobileARAdapters`

następujące pliki:

- `TapPlacementARFoundationAdapterStarter.cs`;
- `ImageTrackingARFoundationAdapterStarter.cs`;
- `RulerARFoundationAdapterStarter.cs`;
- `LightEstimationARFoundationAdapterStarter.cs`;
- `SurfacePainterARFoundationAdapterStarter.cs`.

Startery nie są nadpisywane przy ponownym użyciu generatora. Są kodem studenta i zawierają komentarze wskazujące, gdzie podłączyć `ARRaycastManager`, `ARTrackedImageManager` oraz `ARCameraManager`.

## Minimalna procedura testowa na telefonie

1. Zbuduj aplikację Android na urządzenie wspierające wymagane funkcje ARCore.
2. Uruchom scenę w dobrze oświetlonym miejscu.
3. Poczekaj na stabilizację trackingu i detekcję powierzchni.
4. Sprawdź działanie elementu bez szybkich ruchów telefonu.
5. Powtórz przy innym kącie i odległości.
6. Wykonaj co najmniej trzy pomiary, jeśli element zwraca błąd/czas/dystans.
7. Zapisz model telefonu, wersję Androida, warunki i aktywny provider.

## Proponowane dalsze rozszerzenia

- pinch-to-scale i obrót dwoma palcami dla Tap Placement;
- persistent anchors i odtwarzanie po restarcie;
- semantic placement tylko na podłodze/stole/ścianie;
- image-to-world handoff po utracie markera;
- pomiar powierzchni i objętości w AR Ruler;
- confidence-aware ruler pokazujący niepewność;
- occlusion z environment depth;
- mesh/stroke zamiast punktowego Surface Painter;
- zapisywanie i współdzielenie rysunków między urządzeniami;
- porównanie dwóch telefonów pod kątem plane detection, dryfu i błędu pomiaru.

Najlepszą rozbudową jest taka, dla której można zdefiniować baseline, jedną kontrolowaną zmianę i konkretną metrykę: czas [s], błąd [cm], dryf [cm/min], FPS, latency [ms] albo skuteczność rozpoznania [%].

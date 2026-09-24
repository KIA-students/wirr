# Mixed Reality Showcase — łączenie świata rzeczywistego i wirtualnego

Zestaw **Mixed Reality Showcase** zawiera pięć prefabów przeznaczonych do ćwiczeń, w których dane z rzeczywistego otoczenia sterują warstwą wirtualną. Każdy prefab ma przypisany własny komponent Runtime, działa także bez konkretnego SDK dzięki trybowi demonstracyjnemu i udostępnia publiczne API do podłączenia rzeczywistego dostawcy danych.

Prefaby są generowane do `Assets/WiRR/Common/Prefabs/MixedReality` i umieszczane w scenie pod `WiRR_TeachingAssets/MixedRealityShowcase`.

> **Chcesz zbudować własną wersję?** Otwórz `WiRR → Pomoc → Mixed Reality: implementacja i rozbudowa` albo dokument `MIXED_REALITY_IMPLEMENTATION.md`. Znajdziesz tam procedurę dostawca danych → adapter → WiRR, generator starterów kodu, walidację i propozycje dalszych modyfikacji.

## 1. MR_CameraWindow

**Cel:** wprowadzić obraz z rzeczywistej kamery do obiektu wirtualnego i nałożyć na niego wirtualne elementy, np. celownik lub informacje.

**Skrypt:** `WiRRCameraFeedMixer`.

**Działanie:**
- na PC i smartfonie komponent może uruchomić `WebCamTexture` po uzyskaniu zgody systemowej;
- wirtualna ramka i reticle pozostają obiektami Unity;
- zewnętrzny dostawca danych może podać dowolną `Texture` przez `SetExternalTexture(Texture)`;
- `SetMirror(bool horizontal, bool vertical)` pozwala dostosować orientację obrazu;
- `StopWebCamera()` zatrzymuje strumień, a `ClearExternalTexture()` wraca do źródła lokalnego/trybu zastępczego.

**Quest 3:** tryb passthrough jest zwykle realizowany przez warstwę kompozytora platformy, a nie jako zwykły `WebCamTexture`. Ten prefab może wtedy pozostać wirtualną nakładką nad passthrough albo otrzymać teksturę od dostawcy danych, jeśli użyte API ją udostępnia.

**Pomysły dydaktyczne:** HUD na realnym obrazie, celownik AR, porównanie opóźnienia kamera → obraz, analiza kadrowania i mirrorowania.

## 2. MR_HandAura

**Cel:** nałożyć holograficzną reprezentację na realną śledzoną dłoń.

**Skrypt:** `WiRRHandAura`.

**Działanie:** dostawca danych wysyła pozycję nadgarstka oraz pięciu opuszków przez `SetHandPose(...)`. Prefab rysuje markery, linie palców i wirtualny wskaźnik gestu szczypnięcia (pinch) pomiędzy kciukiem i palcem wskazującym.

**API:**
- `SetHandPose(...)` — aktualizacja pozycji dłoni;
- `SetTracked(bool)` — jawna informacja o utracie/odzyskaniu śledzenia;
- `Pinch01` — znormalizowana wartość 0–1 wyliczana z odległości kciuk–wskazujący.

Jeżeli nie ma dostawcy danych, prefab symuluje dłoń przed kamerą i okresowo demonstruje gest szczypnięcia (pinch). Dzięki temu student widzi zachowanie komponentu jeszcze przed integracją z XR Hands/Meta Hand Tracking.

**Pomysły dydaktyczne:** informacja zwrotna o śledzeniu dłoni, wizualizacja niepewności, gest szczypnięcia (pinch), problem drgań położenia (jitteru) i filtrowania.

## 3. MR_PeopleAwareness

**Cel:** wizualizować obecność innych ludzi w rzeczywistym otoczeniu bez identyfikowania ich.

**Skrypt:** `WiRRPeopleAwareness`.

Dostawca danych przekazuje wyłącznie pozycje w układzie świata Unity przez `SetPeople(Vector3[])` albo `SetPerson(slot, position)`. Wirtualne sylwetki/halo są nanoszone w tych miejscach, a ich pulsowanie rośnie przy małej odległości od użytkownika.

**Założenie prywatności:** komponent nie wymaga obrazu twarzy, nazwisk, embeddingów biometrycznych ani trwałego identyfikatora osoby. Do demonstracji wystarczają anonimowe pozycje.

Bez zewnętrznego detektora prefab symuluje dwie poruszające się osoby.

**Pomysły dydaktyczne:** awareness w shared space, dynamiczne strefy bezpieczeństwa HRI, social proxemics, sygnalizacja kolizji i prywatność.

## 4. MR_SpatialSurfaceScanner

**Cel:** pokazać, jak rzeczywista geometria otoczenia może stać się częścią sceny wirtualnej.

**Skrypt:** `WiRRSpatialSurfaceScanner`.

**Działanie:**
- źródło danych głębi lub siatki przestrzennej wysyła punkty i normalne przez `SubmitSurfaceSample(...)` lub `SubmitSurfaceSamples(...)`;
- prefab rozmieszcza na realnych powierzchniach krótkotrwałe wirtualne markery;
- markery wygasają, dzięki czemu widać aktualnie obserwowaną geometrię;
- bez API danych głębi skrypt wykonuje rozproszone rzuty promieni z `Camera.main` do colliderów sceny, co pozwala testować cały mechanizm w Editorze.

**Pomysły dydaktyczne:** czujniki głębi, mapowanie przestrzenne i normalne powierzchni, okluzja, rzutowanie promienia (raycast) względem realnej geometrii, opóźnienie i gęstość próbkowania.

## 5. MR_WallPortal

**Cel:** przyklejać zawartość wirtualną do rzeczywistej ściany.

**Skrypty:** `WiRRWallAnchor` + `WiRRHeadParallax`.

Moduł Scene Understanding przekazuje środek ściany, normalną i jej rozmiar przez `SetWallPlane(center, normal, size)`. Portal jest ustawiany kilka milimetrów przed powierzchnią i skaluje się do dostępnej ściany. Trzy warstwy wirtualne znajdują się optycznie za płaszczyzną i reagują paralaksą na ruch głowy.

Do testów bez Scene Understanding służy `TryAnchorFromViewerRay()`, który używa zwykłego `Physics.Raycast` do collidera wskazywanego przez kamerę.

**Pomysły dydaktyczne:** semantic anchors, scene understanding, poprawna orientacja normalnej ściany, dopasowanie rozmiaru, stabilność kotwicy i head-coupled parallax.

## Integracja z konkretnymi źródłami danych

Warstwa WiRR jest celowo niezależna od dostawcy danych. Typowy adapter ma tylko odczytać dane z właściwego SDK i wywołać metodę komponentu:

| Źródło | Komponent WiRR | Punkt integracji |
|---|---|---|
| kamera / źródło tekstury | `WiRRCameraFeedMixer` | `SetExternalTexture(Texture)` |
| śledzenie dłoni | `WiRRHandAura` | `SetHandPose(...)` |
| detekcja ludzi / śledzenie ciała | `WiRRPeopleAwareness` | `SetPeople(...)` |
| dane głębi / siatki przestrzennej | `WiRRSpatialSurfaceScanner` | `SubmitSurfaceSample(s)` |
| Scene Understanding / wall plane | `WiRRWallAnchor` | `SetWallPlane(...)` |

Takie rozdzielenie pozwala studentowi porównać np. AR Foundation i Meta SDK bez przepisywania logiki wizualizacji.

## Bezpieczeństwo i prywatność

- Nie zapisuj obrazu z kamery, jeśli eksperyment tego nie wymaga.
- Nie wykonuj identyfikacji twarzy w demonstratorze People Awareness — do celu ćwiczenia wystarcza pozycja.
- Przy pracy z osobami postronnymi stosuj zgodę i ograniczaj dane do minimum potrzebnego w eksperymencie.
- Wizualizacja strefy bezpieczeństwa nie jest certyfikowanym systemem bezpieczeństwa funkcjonalnego.
- Dane z siatki przestrzennej lub mapy głębi mogą ujawniać geometrię pomieszczenia; nie eksportuj ich bez potrzeby.

## Zalecana kolejność

1. Camera Window — najprostsze mieszanie obrazu i grafiki.
2. Hand Aura — połączenie realnego ciała z wirtualnym feedbackiem.
3. Spatial Surface Scanner — przejście z obrazu do geometrii środowiska.
4. Wall Portal — semantyczne wykorzystanie konkretnej powierzchni.
5. People Awareness — dynamiczne elementy realnego otoczenia i interakcja społeczna.


## Następny krok studenta

Nie kończ pracy na uruchomieniu trybu zastępczego. Wybierz co najmniej jeden demonstrator, podłącz rzeczywistego dostawcę danych i wprowadź jedną własną modyfikację, której efekt można zmierzyć. Gotowy prefab jest kontrolowanym wariantem bazowym; wartość dydaktyczna zaczyna się przy porównaniu `wariant bazowy → własna wersja`.

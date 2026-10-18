# XR Showcase — gotowe demonstratory efektów przestrzennych

Zestaw **XR Showcase** zawiera pięć prefabów, które mają pokazywać zjawiska szczególnie czytelne w goglach 6DoF. Każdy prefab ma już przypisany skrypt Runtime i działa po wejściu w Play Mode. XRI nie jest wymagane do działania wersji demonstracyjnej; publiczne metody komponentów pozwalają jednak podłączyć później `Select`, `Hover`, kontrolery lub śledzenie dłoni.

Prefaby są generowane do `Assets/WiRR/Common/Prefabs/XRShowcase` i umieszczane w aktywnej scenie pod `WiRR_TeachingAssets/XRShowcase`.

## 1. XR_ParallaxPortal

**Efekt:** wielowarstwowe okno/portal, w którym warstwy przesuwają się względem siebie podczas translacji głowy.

**Dlaczego jest charakterystyczny dla XR:** efekt opiera się na translacyjnym 6DoF i head-coupled perspective. W zwykłym monitorze student może go zobaczyć, ale dopiero ruch głowy w headsetcie daje silne wrażenie zaglądania w głąb obiektu.

**Skrypt:** `WiRRHeadParallax`.

**Logika:** po uruchomieniu zapamiętywana jest początkowa pozycja głowy. Kolejne przesunięcia HMD w osiach X/Y sterują różnym przesunięciem warstwy bliskiej, środkowej i dalekiej. Dzięki różnym współczynnikom głębokości powstaje paralaksa ruchowa.

**API do rozbudowy:** `Recenter()` ustawia nową pozycję referencyjną głowy.

**Co student może zbadać:**
- różnicę między 3DoF i 6DoF;
- wpływ amplitudy paralaksy na wiarygodność głębi;
- konflikt między stereoskopią, paralaksą i błędną skalą sceny;
- wpływ opóźnienia śledzenie głowy → renderowanie na komfort.

## 2. XR_GazeBloom

**Efekt:** przestrzenny „kwiat/hologram” otwiera się, pulsuje i obraca, gdy użytkownik na niego patrzy.

**Dlaczego jest charakterystyczny dla XR:** pokazuje interakcję niejawną (implicit interaction) — obiekt reaguje bez kliknięcia. W zestawie podstawowym źródłem jest kierunek głowy/kamery, a tę samą logikę można później sterować rzeczywistym śledzeniem wzroku lub stanem wskazania (hover) kontrolera.

**Skrypt:** `WiRRGazeBloom`.

**Logika:** obliczany jest kąt między osią patrzenia a kierunkiem do obiektu oraz odległość. Wartość aktywacji 0–1 steruje skalą rdzenia, rozwarciem satelitów i ruchem.

**API do XRI / śledzenia wzroku:**
- `SetExternalActivation(float)` — wymusza aktywację 0–1;
- `ClearExternalActivation()` — wraca do reakcji na spojrzenie głową.

**Co student może zbadać:** czas utrzymania spojrzenia (dwell), problem Midasa, wielkość celu, informację zwrotną przed selekcją oraz różnicę między śledzeniem wzroku i kierunkiem głowy (head-gaze).

## 3. XR_TelekinesisOrb

**Efekt:** po utrzymaniu spojrzenia na kuli przez około 0,85 s obiekt leci do punktu przed użytkownikiem. Pojawia się wiązka z piedestału. Po odwróceniu wzroku kula wraca.

**Dlaczego jest charakterystyczny dla XR:** jest to demonstracja interakcji na odległość (distant interaction) i „zdalnego przyciągania” (force grab) — techniki często stosowanej, gdy bezpośrednie sięganie dłonią byłoby niewygodne.

**Skrypt:** `WiRRTelekinesisOrb`.

**Logika:** utrzymanie spojrzenia przez określony czas (gaze dwell) uruchamia selekcję; po aktywacji obiekt płynnie śledzi pozycję przed kamerą. Utrata spojrzenia przez określony czas zwalnia obiekt i uruchamia powrót do pozycji bazowej.

**API do XRI / śledzenia dłoni:**
- `BeginHold()`;
- `Release()`;
- `ToggleHold()`.

Te metody można przypisać np. do `Select Entered`/`Select Exited` bez przepisywania logiki ruchu.

## 4. XR_DiegeticHUD

**Efekt:** panel informacyjny miękko podąża za użytkownikiem, ale nie jest sztywno przyklejony do HMD.

**Dlaczego jest charakterystyczny dla XR:** pokazuje różnicę między interfejsem przywiązanym do głowy (head-locked), odnoszonym do ciała (body-referenced) i zakotwiczonym w świecie. Sztywne UI przyklejone do głowy może być męczące; opóźnione podążanie i możliwość przypięcia do świata pozwala porównać strategie.

**Skrypt:** `WiRRHeadFollower`.

**API:**
- `Pin()` — pozostawia panel w świecie;
- `Unpin()` — ponownie włącza miękkie podążanie;
- `TogglePin()`.

**Co student może zbadać:** stabilność UI, czytelność podczas ruchu, komfort, opóźnienie follow i odległość panelu.

## 5. XR_WorldScaleTotem

**Efekt:** miniaturowa postać/totem rośnie do rozmiaru zbliżonego do skali pomieszczenia, gdy użytkownik się zbliża.

**Dlaczego jest charakterystyczny dla XR:** skala w XR jest odczuwana ciałem. Ten sam model może być odbierany jako figurka albo obiekt większy od użytkownika bez zmiany kamery.

**Skrypt:** `WiRRProximityScale`.

**Logika:** odległość głowy od obiektu jest mapowana na współczynnik 0–1, a następnie na skalę miniaturową → skala pomieszczenia (room-scale).

**API:**
- `SetExternalFactor(float)` — sterowanie skalą suwakiem, kontrolerem albo gestem szczypnięcia (pinch);
- `UseProximity()` — powrót do automatyki odległościowej.

**Co student może zbadać:** world scale, propriocepcję, perceived size, near-field comfort oraz błędy wynikające z niepoprawnej skali 1 Unity unit = 1 m.

## Zalecana kolejność demonstracji

1. Portal — najpierw czysta obserwacja 6DoF.
2. Gaze Bloom — reakcja systemu na uwagę użytkownika.
3. Telekinesis — przejście od obserwacji do interakcji na odległość (distant interaction).
4. Diegetic HUD — projektowanie informacji przestrzennej.
5. World Scale Totem — manipulacja skalą i embodied perception.

## Zasada dydaktyczna

Demonstratory mają gotową logikę, ponieważ ich celem jest pokazanie efektu i umożliwienie pomiaru. Nie zastępują ćwiczeń XRI. Gdy laboratorium wymaga samodzielnego skonfigurowania `XR Grab Interactable`, interaktora promieniowego (ray interactor), teleportacji lub śledzenia dłoni, student nadal wykonuje tę część samodzielnie. Publiczne API demonstratorów jest punktem integracji, a nie gotowym rozwiązaniem ocenianego zadania.

# WiRR: przygotowanie urządzeń i instalacja aplikacji (Quest 3, Android, VITURE Luma Ultra)

**Wersja przewodnika:** 2026-10-14. **Adresat:** prowadzący i administrator stanowiska.  
**Zakres:** fizyczne podłączenie, konfiguracja narzędzi, budowanie, przekazanie aplikacji na urządzenie, pierwsze uruchomienie i diagnostyka.  
**Ważne:** opisuje **procedury**, nie potwierdza, że wszystkie sceny WiRR 2.0 ani gotowe pliki APK zostały już zbudowane. Ich dostępność trzeba sprawdzić w wydaniu. Dokładna wersja Unity 6000.6.x pozostaje do zatwierdzenia testami na Ubuntu i Windows.

## Wybierz jedną z trzech ścieżek

| Urządzenie | Co jest uruchamiane | Jak przenosi się aplikację lub obraz | Co robi student |
|---|---|---|---|
| **Meta Quest 3 (VR/MR)** | Samodzielna aplikacja Android z obsługą OpenXR/Meta Quest | Unity **Build And Run** albo gotowy APK przez **ADB** | Zakłada przygotowane gogle, uruchamia wcześniej zainstalowaną aplikację; opcjonalnie doświadcza VR |
| **Telefon Android (AR)** | Aplikacja APK z **AR Foundation + ARCore**, na obsługiwanym urządzeniu | APK przez zatwierdzony kanał dystrybucji; ADB tylko do pracy deweloperskiej | Dobrowolnie otwiera gotową aplikację i udziela wymaganej zgody na kamerę |
| **VITURE XR Luma Ultra (okulary XR/AR)** | **Obraz z PC/telefonu**: 2D albo stereoskopowy Full SBS; opcjonalnie SpaceWalker | **Nie wgrywa się zwykłego APK bezpośrednio do okularów**: kabel przenosi obraz. Aplikacja działa na źródłowym PC/telefonie lub osobnym kompatybilnym urządzeniu obliczeniowym | Ogląda przygotowany pokaz stereoskopowy; nie konfiguruje sterowników |

**Wszystkie obowiązkowe laboratoria i progi 3.0–5.0 WiRR 2.0 muszą działać na przygotowanym PC Ubuntu/Windows bez tych urządzeń.** Trzy ścieżki sprzętowe są nieobowiązkowe i nie wymagają pożyczania prywatnego telefonu. Przygotowanie urządzenia nie jest ocenianym zadaniem studenta.

## 0. Wspólna kontrola stanowiska (przed przyjściem studentów)

1. Na Ubuntu i Windows ustal **jedną przetestowaną wersję Unity 6000.6.x**. Sprawdź, czy projekt i pakiety `pl.prz.kia.wirr`, URP, Input System, XRI oraz profile Android/Meta Quest otwierają się bez błędów. Nie opieraj wersji na samym README.
2. Jeżeli **budujesz** APK, w Unity Hub → `Installs → [edytor] → Add modules` włącz **Android Build Support**, **Android SDK & NDK Tools** i **OpenJDK**. W `Edit → Preferences → External Tools` sprawdź ścieżki dostarczane przez Unity. **Android Studio nie jest niezbędne** do zbudowania przez Unity.
3. Zapewnij kabel **USB-C z transmisją danych** dla Quest/ADB; dla VITURE kabel/port muszą obsługiwać **wyjście wideo USB-C DisplayPort Alt Mode**. Kabel służący tylko do ładowania nie zastępuje żadnego z tych połączeń.
4. Przygotuj **oddzielne** konfiguracje/profil buildu **Quest** i **Android AR**. Różnią się providery XR, uprawnienia i zachowania wejścia; zwykły APK z telefonu nie staje się automatycznie aplikacją VR.
5. Wydaj i zweryfikuj **dwa oddzielne artefakty**, np. `WiRR-Quest-<wersja>.apk` i `WiRR-AndroidAR-<wersja>.apk`, ze znaną nazwą pakietu, wersją, docelowym urządzeniem oraz sumą SHA-256. Nazwy są **wzorcem**, nie informacją o istniejącym pliku. Bezpieczne klucze podpisywania pozostają poza repozytorium i poza pakietem studenckim.
6. Wszystkie urządzenia podłączaj na stanowisku przygotowawczym, a nie w środku 90-minutowego doświadczenia. Utrzymaj działającą wersję desktop/offline na wypadek błędu.

**Kontrola ADB na komputerze z Android SDK:**

```bash
adb devices -l
```

Poprawne urządzenie ma status `device`. `unauthorized` znaczy, że należy zaakceptować autoryzację na ekranie telefonu/w goglach; `no permissions` na Ubuntu wymaga diagnozy reguł udev/uprawnień. Nie uruchamiaj Unity ani ADB jako root w celu obejścia problemu. Jeżeli istnieje kilka instalacji ADB, uruchom tę z zatwierdzonego Android SDK; na Windows sprawdź `where.exe adb`, na Ubuntu `which -a adb`. Nie instaluj przypadkowych sterowników.

## 1. Meta Quest 3: od pierwszego podłączenia do działającego APK

### 1.1. Przygotowanie Meta Quest przez prowadzącego (jednorazowo)

1. Skonfiguruj Quest 3 zgodnie z instrukcją Meta, zaloguj urządzenie do **uprawnionego konta Meta** prowadzącego/instytucji i sparuj je z aplikacją **Meta Horizon** na telefonie administratora.
2. Konto używane do developmentu musi należeć do **zweryfikowanego zespołu deweloperskiego Meta**. Tego etapu nie da się zastąpić samym podłączeniem kabla.
3. W aplikacji **Meta Horizon** wybierz sparowane gogle → **Headset Settings → Developer Mode → ON**. Nazwy ekranów mogą się zmieniać. Studentom nie przekazuje się konta ani hasła prowadzącego.
4. Podłącz Quest 3 do komputera przewodem **USB-C DATA**. W goglach zaakceptuj **Allow USB debugging**, najlepiej dla autoryzowanego komputera prowadzącego. W bieżącej dokumentacji Meta dodatkowe ustawienia widoczności komunikatów USB/MTP znajdują się w sekcji **Quick Control → Settings → Developer**.
5. Sprawdź `adb devices -l`; na **Windows**, jeśli urządzenie nie jest rozpoznawane, sprawdź **oficjalny Oculus ADB Driver**. Na Ubuntu użyj SDK/ADB i zweryfikuj reguły udev. **Meta Horizon Link nie jest potrzebny do standalone APK**.

### 1.2. Budowanie samodzielnej aplikacji Quest w Unity

1. Otwórz **gotowy projekt VR**, a nie scenę AR przeznaczoną dla telefonu.
2. `File → Build Profiles`: wybierz **Meta Quest** w Unity 6.1+ (aktywuj profil/platformę i `Switch Platform`). Gdy profil nie istnieje w starszej konfiguracji, użyj celu **Android** i skonfiguruj odpowiedni OpenXR.
3. Upewnij się, że `XR Plug-in Management` używa dla tego celu **OpenXR** wraz z funkcjami **Meta Quest Support** i właściwym profilem kontrolerów (np. Oculus Touch). Nie uruchamiaj równocześnie providera **ARCore** na tym samym androidowym celu. Wersje pakietów muszą pochodzić z zatwierdzonego manifestu.
4. W `Player Settings` sprawdź `Scripting Backend: IL2CPP`, `Target Architecture: ARM64`, identyfikator aplikacji, wymagany minimalny Android/API według aktualnego profilu i XR validation. Nie wymuszaj ustawień nieobsługiwanych przez używaną wersję SDK.
5. W `Scene List` uwzględnij poprawną scenę startową Quest. Podłączone gogle pojawią się w polu `Run Device` po wykryciu ADB.
6. Kliknij **Build And Run** i zapisz wynikowy **APK** w katalogu artefaktów; Unity powinno wykonać instalację i uruchomienie. Wariant `Development Build` nadaje się do diagnozy, ale zmienia profil wydajności; do pomiarów przygotuj odpowiednią wersję bez debugowych narzutów.

### 1.3. Wgranie gotowego pliku APK bez ponownej kompilacji

Gdy prowadzący ma już **kompatybilny, zweryfikowany plik Quest APK**, nie trzeba ponownie otwierać Unity:

```bash
adb devices -l
adb install -r "WiRR-Quest-<wersja>.apk"
```

Zastąp nazwę pliku rzeczywistą ścieżką do pliku. `-r` zachowuje dane aplikacji przy zgodnym podpisie i identyfikatorze pakietu. Jeśli otrzymasz błąd podpisu, sprawdź, czy nie instalujesz artefaktu podpisanego innym kluczem; **nie usuwaj istniejących danych sesji bez ich zabezpieczenia**.

Na Quest otwórz **Library → Unknown Sources** (w zależności od wersji systemu menu może mieć inną nazwę), wybierz aplikację i wykonaj krótki smoke test kontrolerów, skali, tekstu, przycisku STOP i powrotu do aplikacji. Aplikacja instalowana z ADB nie musi pojawiać się w standardowej siatce sklepowej.

### 1.4. Co oznacza „podłączyć gogle do Unity”

- **Standalone / Quest APK:** komputer jest potrzebny do buildu i wgrania; podczas pracy aplikacja uruchamia się **na goglach**. Tak mierzy się jej wydajność mobilną.
- **PC VR przez Meta Horizon Link:** na zgodnym komputerze **Windows** aplikacja może być wykonywana na PC, a obraz trafia do gogli przez Link/Air Link. To **inna** ścieżka i inny profil opóźnień/wydajności; nie podstawiaj jej wyników za standalone. **Ubuntu nie wymaga instalacji Meta Horizon Link.**
- **Play w Unity Editor bez fizycznych gogli:** symulator wejścia XRI może służyć do weryfikacji sceny, ale nie dowodzi obsługi trackingu, komfortu ani mobilnej wydajności Quest.

### 1.5. Najczęstsze błędy

| Objaw | Sprawdź |
|---|---|
| `adb devices` nic nie pokazuje | Kabel DATA, port USB, Developer Mode, sterownik Meta (Windows), odpowiedni ADB |
| `unauthorized` | Ekran gogli i zgodę USB debugging, konto/opcje programistyczne |
| Quest pojawia się w ADB, ale nie w Unity | `Run Device → Refresh`, aktualny profil Meta Quest/Android i Android Build Support |
| Kompilacja przechodzi, ale obraz jest czarny | Aktywna scena w `Scene List`, OpenXR/Meta feature, URP, konfiguracja kamer/XR Origin, pierwszy wyjątek z logcat |
| `INSTALL_FAILED_UPDATE_INCOMPATIBLE` | Różny podpis APK od zainstalowanej wersji; wykonaj kopię danych, dopiero potem rozważ czystą reinstalację |
| Aplikacja działa przez Link, a nie na goglach | Sprawdź, czy zbudowałeś **Android/Meta Quest APK**, nie Windows .exe |

Do logowania na komputerze prowadzącego: `adb logcat`. Nie udostępniaj publicznie zrzutu logów zawierającego identyfikatory czy dane studentów.

## 2. Telefon Android: opcjonalne rzeczywiste AR

### 2.1. Warunki kompatybilności

1. Sprawdź model na oficjalnej **liście urządzeń ARCore**: https://developers.google.com/ar/devices. Sam Android, złącze USB-C i aparat fotograficzny **nie wystarczają**.
2. Rozróżnij osobno: (a) wsparcie ARCore, (b) obecność i aktualność **Google Play Services for AR**, (c) uprawnienie kamery, (d) wsparcie **Depth API**. Brak Depth API nie musi uniemożliwiać placement i plane detection w Lab03.
3. W aplikacji obsłuż wynik `ARSession.state`, brak/złą wersję usług AR i uprawnienia. Dla projektu z fallbackiem przemyśl wariant **AR Optional**; w trybie `AR Required` niekompatybilne urządzenia nie mogą prawidłowo realizować funkcji AR. **Tryb AR Optional nie oznacza, że symulowany desktop staje się realnym AR.**
4. W scenie z okluzją sprawdź w runtime możliwości subsystemu occlusion/depth. Jeśli nie obsługuje, wyświetl `DEPTH_UNSUPPORTED` i przejdź do odpowiedniego zadania w symulatorze. Nie wstawiaj fikcyjnych pomiarów głębi.

### 2.2. Zbudowanie APK na komputerze prowadzącego

1. Zainstaluj Android Build Support + SDK/NDK + OpenJDK w Unity Hub.
2. `File → Build Profiles → Android → Switch Platform`. **Nie używaj profilu Meta Quest dla zwykłego ARCore na smartfonie.**
3. Skonfiguruj zgodne wersje **AR Foundation**, **Google ARCore XR Plug-in**, `XR Plug-in Management` dla Android z providerem ARCore, `AR Session`, `XR Origin`, `AR Camera`, plane/raycast i occlusion tylko w scenariuszach, które ich używają. Wyłącz OpenXR jako konkurencyjny provider tego samego celu Android.
4. Zastosuj wymagania `Minimum API Level`, `Target API Level`, IL2CPP/ARM64 i Graphics API ustalone na podstawie **zatwierdzonej wersji Unity/ARCore**, a nie kopiowanej na ślepo konfiguracji Quest. Zapewnij uprawnienie `CAMERA`; nie żądaj mikrofonu/geolokalizacji, jeśli nie są potrzebne.
5. Dodaj sceny Lab03/04 w `Scene List` z właściwą sceną startową, ustaw stałą nazwę pakietu Android i wersję. Wygeneruj oraz podpisz zatwierdzony APK. Przy tworzeniu aplikacji dystrybuowanej poza sklepem zweryfikuj poprawność podpisu i pochodzenia paczki.

### 2.3. Dwie drogi instalacji na telefonie

**Ścieżka podstawowa dla uczestnika BYOD:** prowadzący udostępnia **sprawdzony, podpisany plik APK** z oficjalnej strony kursu / kontrolowanego wydania, razem z wersją i sumą kontrolną. Student **dobrowolnie** pobiera i otwiera APK, potwierdza instalację przez systemowy mechanizm Androida i przy pierwszym uruchomieniu przyznaje dostęp do kamery, tylko jeśli chce uruchomić AR. Opcja instalowania aplikacji z danego źródła bywa dostępna w ustawieniach Androida; wyłącznie za świadomą zgodą właściciela. **Nie polecać wyłączania Play Protect, roota ani omijania zabezpieczeń urządzenia.** Gdy organizacja lub student nie akceptuje instalacji spoza sklepu, użyj gotowej ścieżki desktop.

**Ścieżka techniczna na telefonie testowym prowadzącego:** włącz `Developer options` (zwykle wielokrotne dotknięcie „Build number”, nazwy menu różnią się według producenta), aktywuj `USB debugging`, podłącz przewód DATA, zaakceptuj klucz RSA i wykonaj:

```bash
adb devices -l
adb install -r "WiRR-AndroidAR-<wersja>.apk"
```

Alternatywnie w Unity wybierz telefon w `Run Device` i **Build And Run**. ADB i debugowanie USB nie są obowiązkowe dla studenckiego telefonu.

### 2.4. Pierwszy test funkcji AR

1. Uruchom aplikację i potwierdź **rzeczywisty** status `REAL_AR`, wersję programu i tryb kamery.
2. Zgódź się na uprawnienie kamery tylko na własnym urządzeniu; skanuj bezpieczną, teksturowaną powierzchnię przy dobrym oświetleniu i umieść przygotowany model w skali 1:1.
3. Sprawdź, czy model utrzymuje położenie po zmianie punktu widzenia; dla Lab04 sprawdź **osobny** status Depth API i okluzję. Pomiar bez zewnętrznego ground truth nie jest dowodem bezwzględnej dokładności pozycji.
4. Jeśli usługa AR, kamera lub depth nie działa, zapisz status `AR_UNSUPPORTED` lub `DEPTH_UNSUPPORTED` i zakończ opcjonalne doświadczenie. Kontynuuj **pełne zadanie na komputerze Ubuntu/Windows**.

**Prywatność:** nie zapisywać obrazu z kamery, nagrań, geolokalizacji ani identyfikatora telefonu. Wynik przesyłany do raportu powinien zawierać źródło `REAL_AR`, wersję aplikacji, parametry i faktycznie zmierzone wartości; automatyczny transfer może nie istnieć, wtedy obowiązuje zatwierdzony eksport offline.

## 3. VITURE XR Luma Ultra: fizyczne podłączenie, 2D, Full SBS i SpaceWalker

### 3.1. Zasada działania i kompatybilność

VITURE Luma Ultra **nie jest samodzielnym urządzeniem Android/Meta Quest**. W zwykłym trybie okulary otrzymują obraz i zasilanie z kompatybilnego komputera/telefonu przez dołączony przewód z **magnetycznym złączem przy okularach i USB-C przy źródle**. Bezpośredni USB-C źródła musi przenosić **DisplayPort Alt Mode**; zwykły kabel ładowania albo USB-C z samymi danymi nie wystarczy.

Luma Ultra ma według specyfikacji producenta **1920×1200 na oko**, czyli **3840×1200 łącznie** dla obrazu Full SBS; do 120 Hz w trybach wspieranych. Istniejący generator pakietu WiRR tworzy **3840×1080**, odpowiednie dla wielu starszych trybów Full SBS, ale **nie jest to natywna wysokość 1200p Luma Ultra**. Przed deklaracją poprawnej obsługi 3840×1200 trzeba sprawdzić, czy generator i urządzenie negocjują oraz wyświetlają właściwą rozdzielczość; sama zmiana Game View nie potwierdza pracy w trybie natywnym.

Okulary Luma Ultra posiadają sensory i sprzęt do funkcji 6DoF, jednak działanie **wyświetlania jako monitora 2D** ani **materiału SBS** nie oznacza, że Unity ma już dostęp do śledzenia głowy, dłoni, RGB/Depth lub osadzania hologramów. Te funkcje wymagają zgodnego oprogramowania/SDK, transportu danych i testu. **Nie opisywać zwykłego SBS jako pełnego AR ani nie korzystać z ARCore telefonu jako domyślnej obsługi kamer okularów**.

### 3.2. Szybki pokaz na Windows/Ubuntu bez dodatkowego oprogramowania

1. Na komputerze sprawdź oznaczenie i parametry portu USB-C (**DP Alt Mode / Thunderbolt / USB4**). Jeżeli komputer ma tylko HDMI/DisplayPort, potrzebny jest **właściwy adapter aktywny**, który zapewnia sygnał wideo i zasilanie, a w niektórych scenariuszach także transmisję USB do sensora. Nie zakładaj, że dowolna przejściówka USB-C↔HDMI jest dwukierunkowa.
2. Podłącz **magnetyczne złącze do okularów**, USB-C do komputera. W Windows otwórz **Ustawienia → System → Ekran**; w Ubuntu **Ustawienia → Ekrany/Displays**. Sprawdź, czy wykryto dodatkowy wyświetlacz i czy widać pulpit w **2D**. Ustaw duplikowanie lub rozszerzenie według scenariusza.
3. Załóż okulary, dopasuj noski i położenie względem oczu, ustaw jasność i korekcję dioptryczną w zakresie urządzenia. Rozpocznij od obrazu 2D; nie wymuszaj dużej dysparycji.
4. Uruchom **gotowy demonstrator SBS** na źródłowym PC, wyświetl go na ekranie VITURE w rzeczywistym, obsługiwanym trybie wyjściowym. **Full SBS**: lewa połowa = lewe oko, prawa = prawe; nie odwracać kolejności.
5. W **serii Luma** przytrzymaj przycisk **R1**, aby przełączyć 2D ↔ 3D. Jeśli widzisz dwa obrazy obok siebie, sprawdź, czy w okularach i systemie uruchomiono ten sam tryb oraz rozdzielczość i proporcje. Nie zmieniaj agresywnie baz stereoskopowych, by doprowadzić do „złączenia obrazu”.
6. Po zakończeniu wróć do 2D i odłącz urządzenie po zatrzymaniu pokazu. Jeśli ekran w systemie nie został wykryty, to najpierw problem portu/kabla/adaptera, a nie konfiguracji kamery w Unity.

**Ubuntu:** gwarantowany w tej instrukcji jest test **zewnętrznego wyświetlacza**, a nie integracja 6DoF ze SpaceWalker. Nie ma podstaw do obiecywania programu SpaceWalker Desktop dla Ubuntu na podstawie dokumentacji Windows/macOS producenta.

### 3.3. SpaceWalker Windows: tylko na przygotowanym, zgodnym komputerze

Opcjonalne **SpaceWalker for Windows** dostarcza układy wirtualnych ekranów, recenter, wybór częstotliwości oraz według producenta **6DoF mocowania ekranu dla Luma Ultra**. Wymagane jest połączenie zapewniające **jednocześnie zasilanie, obraz DP Alt Mode i transmisję danych USB**. Sam obraz na okularach nie dowodzi, że SpaceWalker potrafi pobierać dane z czujników.

1. Na komputerze prowadzącego sprawdź wymagania Windows, GPU, RAM, USB-C DP Alt Mode i transmisji danych. Zainstaluj aktualny SpaceWalker **z oficjalnego centrum VITURE**, a potem sprawdź wykrycie konkretnej pary okularów. Nie podłączaj niezweryfikowanych adapterów, które przenoszą wyłącznie obraz.
2. Po uruchomieniu wybierz **Single Display**, a następnie **Mirror** lub **Extend**, po czym dopasuj rozmiar, jasność, częstotliwość i **Recenter**. Dopiero potem przetestuj ewentualne 6DoF poprzez przesunięcie głowy w bezpiecznym zakresie. Jeśli konfiguracja nie wspiera danych ruchowych, pozostań w trybie zwykłego ekranu.
3. **Ważne ograniczenie bezpieczeństwa:** część aktualnych procedur sterownika SpaceWalker Windows producenta opisuje wyłączanie Secure Boot/BitLocker lub włączanie Windows Test Mode. **Nie wykonuj tego na komputerach uczelnianych ani prywatnych urządzeniach studentów bez osobnej zgody administratora i oceny ryzyka**. Jeśli tego wymaga dana instalacja, **zrezygnuj z SpaceWalker w laboratorium** i pokaż przygotowane 2D/Full SBS.
4. Nie zakładaj, że `SpaceWalker Windows 6DoF` oznacza gotowy dostęp do **head pose w Unity przez OpenXR**. Wymagane jest osobno zweryfikowane API/SDK. W szczególności nie aktywuj w raportach pola `REAL_AR` na podstawie samego działania wirtualnego pulpitu.

### 3.4. Android jako źródło obrazu dla VITURE

Telefon Android może wyświetlać obraz na Luma Ultra **tylko jeśli** obsługuje odpowiedni wyjściowy sygnał wideo przez USB-C (DisplayPort Alt Mode) albo zweryfikowany zgodny tor pośredni. To **osobna cecha** od obsługi ARCore przez telefon. Podłącz kabel i sprawdź zwykłe 2D; SpaceWalker na Androidzie stosuj jedynie przy **zgodnym urządzeniu i trybie**, bez gwarantowania 6DoF na Androidzie. Jeśli telefon nie obsługuje wyjścia wideo, nie da się naprawić tego ustawieniem ARCore ani instalacją aplikacji Unity.

### 3.5. Diagnostyka VITURE

| Objaw | Najpierw sprawdź |
|---|---|
| Brak obrazu i brak wykrycia ekranu | DP Alt Mode, kabel magnetyczny, zasilanie, zgodność adaptera, rzeczywisty ekran systemowy |
| Widać obraz 2D, ale SpaceWalker nie widzi okularów | Tor **USB data** oprócz obrazu; zgodność modelu, adapter, wersja aplikacji |
| Dwa obrazy obok siebie zamiast 3D | Przełącznik **R1 przytrzymanie**, format **Full SBS**, tryb wyjścia i proporcje |
| 3D rozciągnięte lub ucięte | Natywny profil Luma Ultra **3840×1200**, ograniczenie starego generatora **3840×1080**, weryfikacja EDID/rozdzielczości |
| Brak śledzenia głowy w Unity | To nie jest automatyczna funkcja monitora; wymaga zgodnego SDK i transportu danych |
| Obraz powoduje dyskomfort | Natychmiast przerwij, wróć do neutralnego 2D, skontroluj dopasowanie i bezpieczną geometrię stereo |

## 4. Krótka karta gotowości przed każdym blokiem

| Kontrola | Quest 3 | Telefon Android | VITURE Luma Ultra |
|---|---|---|---|
| Sprawdzono model, kabel i zasilanie | USB-C DATA | opcjonalnie USB-C DATA | USB-C **z DP Alt Mode** |
| Przygotowano właściwy artefakt | Meta Quest APK | Android AR APK | PC build/wideo 2D/SBS; **bez APK na okulary** |
| Zweryfikowano łączność | `adb devices -l` | ADB tylko na sprzęcie testowym | wykryty monitor 2D |
| Zweryfikowano funkcję | działa XR i kontrolery | działa ARCore i kamera, depth jeśli jest | 2D + opcjonalnie poprawne Full SBS |
| Przygotowano fallback | desktop | desktop `SIM_AR` | desktop 2D |
| Odnotowano dowód testu | model, OS, wersja, wynik | model, funkcje, wynik | port, tryb, rozdzielczość, wynik |

**Bezpieczeństwo na zajęciach:** pojedyncze gogle Quest 3 i pojedyncze Luma Ultra są współdzielone i używane rotacyjnie. Student nie jest oceniany za skorzystanie z nich. Nie wolno wymagać dostępu do cudzych kont Meta/Google, haseł, debugowania prywatnego telefonu, wyłączania mechanizmów ochronnych, publikowania obrazu z kamery czy rejestrowania danych o zdrowiu. Rozpocznij od komfortowego trybu obrazu, zadbaj o higienę i możliwość natychmiastowego przerwania.

## 5. Źródła i zakres weryfikacji (14.10.2026)

- [Meta: Test your app on your device / Developer Mode i ADB](https://developers.meta.com/vr/documentation/android-apps/enable-developer-mode/)
- [Meta: Device Setup, including Oculus ADB Drivers](https://developers.meta.com/vr/documentation/native/android/mobile-device-setup/)
- [Meta: Build Configuration Overview, Unity Build And Run](https://developers.meta.com/vr/documentation/unity/unity-build/)
- [Unity: Meta Quest build profiles](https://docs.unity.com/en-us/engine/6000.3/manual/xr/configuring-project-for/meta-quest-develop/build-profile)
- [Unity: Meta OpenXR, konfiguracja providerów Android](https://docs.unity.cn/Packages/com.unity.xr.meta-openxr%402.0/manual/project-setup.html)
- [Google: ARCore supported devices](https://developers.google.com/ar/devices)
- [Google: AR Foundation/ARCore, ustawienia i runtime checks](https://developers.google.com/ar/develop/unity-arf/enable-arcore)
- [Google: ARCore Depth API](https://developers.google.com/ar/develop/depth)
- [VITURE: Luma Series, przyciski i tryby](https://www.viture.com/en-MY/academy/xr-glasses/luma-series)
- [VITURE: Luma Ultra, rozdzielczość i funkcje 6DoF](https://www.viture.com/academy/product/viture-luma-ultra-dock-pack)
- [VITURE: SpaceWalker Desktop, połączenie i wymagania](https://www.viture.com/academy/spacewalker/desktop)
- [VITURE Academy](https://www.viture.com/academy)

**Sprawdzone:** publiczna instrukcja producentów, istniejące pliki dokumentacji w repozytorium, obecność lokalnych procedur `BUILD_AND_DEPLOY.md`, `OS_SETUP.md`, `VITURE_XR.md`. **Nie sprawdzone fizycznie w tej zmianie:** test konkretnego Quest 3, telefonu, portu/adaptera Luma Ultra, podpisu APK, wyniku buildu Unity, dostępności 3840×1200 w aktualnym rig WiRR oraz transmisji sensorów Luma Ultra do Unity. Wynik testu sprzętowego należy dopisać po jego faktycznym wykonaniu.

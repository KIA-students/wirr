# Budowanie i instalacja aplikacji WiRR

Ta instrukcja dotyczy Unity 6000.6.x i projektu URP używanego na kursie WiRR. W Unity dostępne jest także okno `WiRR → Pomoc → Budowanie i instalacja`, które prowadzi przez te same kroki.

## Najważniejsza zasada

PC, smartfon z Androidem i Meta Quest 3 są osobnymi platformami docelowymi kompilacji. Ten sam projekt może obsługiwać wszystkie trzy, ale nie oznacza to identycznych ustawień. Zmiana platformy może wymagać innego modułu obsługi XR, interfejsu graficznego (`Graphics API`), architektury procesora, uprawnień, systemu wejścia (`Input System`) i profilu renderowania.

## PC — Windows

1. Otwórz `File → Build Profiles`.
2. Dodaj właściwą scenę do listy scen (`Scene List`).
3. Wybierz profil Windows/Standalone i wykonaj `Switch Platform`.
4. Dla zwykłej aplikacji desktopowej XR nie jest wymagane. Dla PC VR skonfiguruj OpenXR w grupie Standalone.
5. Ustaw rozdzielczość, Graphics API i pozostałe ustawienia eksperymentu.
6. Użyj `Build` albo `Build And Run`.
7. Wynikiem jest zwykle plik `.exe` i katalog `*_Data`; należy przenosić je razem.

## Android — smartfon

### Przygotowanie Unity
W Unity Hub dla używanej wersji edytora doinstaluj:
- Android Build Support;
- Android SDK & NDK Tools;
- OpenJDK.

### Przygotowanie telefonu
1. Włącz Opcje programistyczne.
2. Włącz Debugowanie USB.
3. Podłącz telefon przewodem obsługującym transmisję danych.
4. Zaakceptuj klucz RSA / zgodę na debugowanie.
5. Sprawdź: `adb devices`.

Status powinien być `device`, nie `unauthorized`.

### Budowanie
1. `File → Build Profiles`.
2. Wybierz Android i `Switch Platform`.
3. Ustaw unikalną nazwę pakietu (`Package Name`).
4. Dla ćwiczeń AR skonfiguruj AR Foundation/ARCore zgodnie z instrukcją laboratorium.
5. Dodaj sceny do listy scen (`Scene List`).
6. W polu `Run Device` wybierz telefon.
7. Użyj `Build And Run`.

Unity zbuduje APK, zainstaluje go przez ADB i uruchomi.

Gotowy APK można również zainstalować ręcznie: `adb install -r nazwa.apk`.

## Meta Quest 3 — aplikacja samodzielna

Quest 3 jest urządzeniem Android/Meta Horizon OS, ale wymaga konfiguracji XR właściwej dla gogli.

1. W Unity Hub zainstaluj Android Build Support, SDK/NDK i OpenJDK.
2. Włącz tryb programisty (`Developer Mode`) dla Quest 3.
3. Podłącz gogle przewodem USB-C z transmisją danych.
4. W goglach zaakceptuj zgodę na debugowanie USB (`USB debugging`).
5. Sprawdź: `adb devices`.
6. Otwórz `File → Build Profiles`.
7. Jeżeli Unity oferuje profil Meta Quest, włącz go i użyj `Switch Platform`. W przeciwnym razie użyj Android.
8. W `Project Settings → XR Plug-in Management` włącz OpenXR dla Android/Meta Quest oraz wymagane funkcje Meta Quest Support.
9. Używaj ARM64.
10. W polu `Run Device` wybierz Quest 3.
11. Kliknij `Build And Run` i wskaż miejsce zapisu APK.

Ręczna instalacja: `adb install -r nazwa.apk`.

Do diagnostyki: `adb logcat`.

Meta Horizon Link uruchamia aplikację PC VR strumieniowaną do gogli. Nie jest równoważny samodzielnej wersji aplikacji i nie powinien zastępować pomiaru wydajności APK uruchomionego bez Link.

## Smartfon a Quest 3

Oba urządzenia mogą otrzymać APK przez ADB, ale ten sam plik APK nie musi być poprawny dla obu konfiguracji.

Smartfon AR może wymagać:
- ARCore;
- dostępu do kamery;
- interfejsu dotykowego;
- innej orientacji ekranu.

Quest 3 wymaga:
- konfiguracji OpenXR/Meta Quest;
- stereoskopowego renderowania;
- kontrolerów lub śledzenia dłoni;
- mobilnego budżetu GPU odpowiedniego dla dwóch widoków oka.

Przed każdym pomiarem porównawczym zapisz platformę, urządzenie, profil kompilacji i istotne ustawienia renderowania.

Jeżeli zmieniasz platformę między PC i Androidem, po `Switch Platform` ponownie sprawdź XR Plug-in Management oraz aktywny profil renderowania przed wykonaniem pomiarów.

## Dokumentacja producentów

- Unity — profile i konfiguracja budowania (`Build Profiles` / `Build Configuration`): https://docs.unity3d.com/
- Meta — konfiguracja projektu Unity dla Meta Quest: https://developers.meta.com/horizon/documentation/unity/unity-project-setup/
- Meta — przygotowanie gogli do programowania i ADB: https://developers.meta.com/horizon/documentation/unity/unity-env-device-setup/
- Meta — przegląd konfiguracji budowania aplikacji: https://developers.meta.com/horizon/documentation/unity/unity-build/

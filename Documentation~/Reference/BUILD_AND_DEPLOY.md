# Budowanie i instalacja aplikacji WiRR

Ta instrukcja dotyczy Unity 6000.6.x i projektu URP używanego na kursie WiRR. W Unity dostępne jest także okno `WiRR → Pomoc → Budowanie i instalacja`, które prowadzi przez te same kroki.

## Najważniejsza zasada

PC, smartfon z Androidem i Meta Quest 3 są osobnymi targetami kompilacji. Ten sam projekt może obsługiwać wszystkie trzy, ale nie oznacza to identycznych ustawień. Zmiana platformy może zmienić XR provider, Graphics API, architekturę procesora, uprawnienia, Input System i profil renderowania.

## PC — Windows

1. Otwórz `File → Build Profiles`.
2. Dodaj właściwą scenę do Scene List.
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

### Build
1. `File → Build Profiles`.
2. Wybierz Android i `Switch Platform`.
3. Ustaw unikalny Package Name.
4. Dla ćwiczeń AR skonfiguruj AR Foundation/ARCore zgodnie z instrukcją laboratorium.
5. Dodaj sceny do Scene List.
6. W Run Device wybierz telefon.
7. Użyj `Build And Run`.

Unity zbuduje APK, zainstaluje go przez ADB i uruchomi.

Gotowy APK można również zainstalować ręcznie: `adb install -r nazwa.apk`.

## Meta Quest 3 — standalone

Quest 3 jest urządzeniem Android/Meta Horizon OS, ale wymaga konfiguracji XR właściwej dla headsetu.

1. W Unity Hub zainstaluj Android Build Support, SDK/NDK i OpenJDK.
2. Włącz Developer Mode dla Quest 3.
3. Podłącz headset przewodem USB-C z transmisją danych.
4. W headsecie zaakceptuj USB debugging.
5. Sprawdź: `adb devices`.
6. Otwórz `File → Build Profiles`.
7. Jeżeli Unity oferuje profil Meta Quest, włącz go i użyj `Switch Platform`. W przeciwnym razie użyj Android.
8. W `Project Settings → XR Plug-in Management` włącz OpenXR dla Android/Meta Quest oraz wymagane funkcje Meta Quest Support.
9. Używaj ARM64.
10. W Run Device wybierz Quest 3.
11. Kliknij `Build And Run` i wskaż miejsce zapisu APK.

Ręczna instalacja: `adb install -r nazwa.apk`.

Do diagnostyki: `adb logcat`.

Meta Horizon Link uruchamia aplikację PC VR strumieniowaną do headsetu. Nie jest równoważny buildowi standalone i nie powinien zastępować pomiaru wydajności APK uruchomionego bez Link.

## Smartfon a Quest 3

Oba urządzenia mogą otrzymać APK przez ADB, ale build nie jest automatycznie wymienny.

Smartfon AR może wymagać:
- ARCore;
- dostępu do kamery;
- interfejsu dotykowego;
- innej orientacji ekranu.

Quest 3 wymaga:
- konfiguracji OpenXR/Meta Quest;
- stereoskopowego renderowania;
- kontrolerów lub hand trackingu;
- mobilnego budżetu GPU odpowiedniego dla dwóch widoków oka.

Przed każdym benchmarkiem zapisz platformę, urządzenie, profil buildu i istotne ustawienia renderowania.

Jeżeli zmieniasz platformę między PC i Androidem, po `Switch Platform` ponownie sprawdź XR Plug-in Management oraz aktywny profil renderowania przed wykonaniem pomiarów.

## Dokumentacja producentów

- Unity — Build Profiles / Build Configuration: https://docs.unity3d.com/
- Meta — Unity project setup for Meta Quest: https://developers.meta.com/horizon/documentation/unity/unity-project-setup/
- Meta — headset development setup and ADB: https://developers.meta.com/horizon/documentation/unity/unity-env-device-setup/
- Meta — build configuration overview: https://developers.meta.com/horizon/documentation/unity/unity-build/

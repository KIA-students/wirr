using UnityEditor;
using UnityEngine;

namespace KIA.WiRR.Editor
{
    public sealed class WiRRBuildGuideWindow : EditorWindow
    {
        private Vector2 scroll;
        private int tab;

        [MenuItem("WiRR/Pomoc/Budowanie i instalacja", priority = 50)]
        public static void Open()
        {
            var window = GetWindow<WiRRBuildGuideWindow>();
            window.titleContent = new GUIContent("WiRR · Build");
            window.minSize = new Vector2(650, 620);
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Budowanie i instalacja aplikacji", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "PC i Android powstają z tego samego projektu Unity, ale są osobnymi targetami kompilacji. Zmiana platformy może zmienić aktywne ustawienia renderowania, XR, architekturę, dostępne API i wymagania wydajnościowe. Przed pomiarami zawsze zapisz w raporcie aktywną platformę i profil buildu.",
                MessageType.Info);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Otwórz Build Profiles", GUILayout.Height(28)))
                    OpenBuildProfiles();
                if (GUILayout.Button("Otwórz Player Settings", GUILayout.Height(28)))
                    SettingsService.OpenProjectSettings("Project/Player");
                if (GUILayout.Button("Otwórz XR Plug-in Management", GUILayout.Height(28)))
                    SettingsService.OpenProjectSettings("Project/XR Plug-in Management");
            }

            tab = GUILayout.Toolbar(tab, new[] { "PC", "Android · smartfon", "Meta Quest 3", "Różnice" });
            scroll = EditorGUILayout.BeginScrollView(scroll);
            EditorGUILayout.Space(8);

            switch (tab)
            {
                case 0: DrawPc(); break;
                case 1: DrawPhone(); break;
                case 2: DrawQuest(); break;
                default: DrawDifferences(); break;
            }

            EditorGUILayout.EndScrollView();
        }

        private static void DrawPc()
        {
            Heading("Build na PC — Windows");
            Step("1. Sceny", "Otwórz File → Build Profiles i upewnij się, że właściwa scena znajduje się na Scene List. Dodaj otwartą scenę, jeśli jej brakuje.");
            Step("2. Platforma", "Wybierz profil Windows/Standalone i użyj Switch Platform. Dla zwykłej aplikacji desktopowej XR nie jest wymagane. Dla PC VR/OpenXR skonfiguruj OpenXR także dla grupy Standalone.");
            Step("3. Ustawienia", "Sprawdź rozdzielczość, tryb pełnoekranowy, Graphics API i ewentualne ustawienia OpenXR. Development Build włączaj do diagnostyki; do pomiarów końcowych używaj konfiguracji zapisanej w raporcie.");
            Step("4. Build", "Kliknij Build. Wskaż pusty katalog docelowy. Unity utworzy plik .exe oraz katalog danych aplikacji — przenoś je razem.");
            Step("5. Build And Run", "Jeżeli chcesz od razu uruchomić program na tym samym PC, użyj Build And Run.");
            Note("Typowy wynik: plik .exe + katalog *_Data. Nie jest to APK i nie można zainstalować go na urządzeniu z Androidem.");
        }

        private static void DrawPhone()
        {
            Heading("Build na smartfon z Androidem");
            Step("1. Moduły Unity", "W Unity Hub dla używanej wersji edytora zainstaluj Android Build Support, Android SDK & NDK Tools oraz OpenJDK.");
            Step("2. Telefon", "W Androidzie włącz Opcje programistyczne oraz Debugowanie USB. Podłącz telefon przewodem danych i zaakceptuj komunikat o zaufaniu/kluczu RSA.");
            Step("3. Sprawdzenie ADB", "W terminalu uruchom adb devices. Urządzenie powinno mieć status device, a nie unauthorized.");
            Step("4. Platforma", "W File → Build Profiles wybierz Android i Switch Platform. Ustaw unikalny Package Name w Player Settings. Dla Lab 03–04 skonfiguruj wymagany provider AR (np. ARCore); nie kopiuj bezmyślnie ustawień Quest.");
            Step("5. Sceny i orientacja", "Dodaj sceny do Scene List. Sprawdź orientację ekranu, wymagane uprawnienia kamery oraz minimalny/target API Level zgodny z urządzeniem i użytymi pakietami.");
            Step("6. Build And Run", "Wybierz telefon w Run Device i użyj Build And Run. Unity zbuduje APK, zainstaluje go przez ADB i uruchomi.");
            Step("7. Instalacja istniejącego APK", "Jeżeli APK jest już zbudowany, można użyć adb install -r nazwa.apk. Opcja -r aktualizuje istniejącą instalację bez ręcznego odinstalowania.");
            Command("adb devices");
            Command("adb install -r nazwa.apk");
            Note("Smartfon i Quest 3 używają formatu APK, ale konfiguracja XR/AR, wejścia, manifestu i renderowania jest inna. Nie zakładaj, że jeden build jest poprawny dla obu urządzeń.");
        }

        private static void DrawQuest()
        {
            Heading("Build standalone na Meta Quest 3");
            Step("1. Moduły Unity", "Zainstaluj Android Build Support, Android SDK & NDK Tools oraz OpenJDK. Quest działa jako urządzenie Android/Meta Horizon OS.");
            Step("2. Tryb deweloperski", "Włącz Developer Mode dla headsetu. Podłącz Quest 3 przewodem USB-C z transmisją danych, załóż headset i zaakceptuj USB debugging; warto zaznaczyć stałe zaufanie dla używanego komputera.");
            Step("3. ADB", "Uruchom adb devices. Quest powinien pojawić się ze statusem device. Jeżeli widzisz unauthorized, ponownie zaakceptuj komunikat w headsecie.");
            Step("4. Profil buildu", "W Unity 6 użyj File → Build Profiles. Jeżeli dostępny jest profil Meta Quest, włącz go i przełącz platformę; w przeciwnym razie użyj Android. Dla Quest wymagane są ustawienia Android/Meta Quest, a nie sam profil Windows.");
            Step("5. OpenXR", "W Project Settings → XR Plug-in Management włącz OpenXR dla Android/Meta Quest. Włącz wymagane funkcje Meta Quest Support. Nie myl tej konfiguracji z OpenXR dla Standalone/PC.");
            Step("6. Architektura", "Używaj ARM64. Sprawdź Player Settings, Graphics API i ustawienia renderowania przeznaczone dla urządzenia mobilnego. Quest ma znacznie mniejszy budżet CPU/GPU niż PC.");
            Step("7. Build And Run", "W Build Profiles wybierz Quest w Run Device i kliknij Build And Run. Zapisz APK. Unity zainstaluje aplikację przez ADB i uruchomi ją w headsecie.");
            Step("8. Instalacja APK ręcznie", "Gotowy APK można zainstalować poleceniem adb install -r nazwa.apk. Do diagnostyki przydatne są także adb logcat oraz Meta Quest Developer Hub.");
            Command("adb devices");
            Command("adb install -r nazwa.apk");
            Note("Meta Horizon Link służy do szybkiego uruchamiania wersji PC VR z edytora. Nie zastępuje pomiaru aplikacji standalone zbudowanej jako Android/Meta Quest.");
        }

        private static void DrawDifferences()
        {
            Heading("PC a Android — co faktycznie się różni?");
            Difference("Format", "PC: .exe + katalog danych.", "Android/Quest: .apk instalowany na urządzeniu.");
            Difference("Procesor", "Typowo x86-64 na komputerze.", "Typowo ARM64 na telefonie i Quest 3.");
            Difference("Grafika", "Większy budżet GPU, inne API i sterowniki.", "Mobilny GPU, większa presja na liczbę draw calls, fill-rate, pamięć i temperaturę.");
            Difference("XR/AR", "OpenXR dla Standalone dotyczy PC VR.", "OpenXR/Meta Quest albo ARCore dotyczą osobnej grupy Android/Meta Quest.");
            Difference("Wejście", "Klawiatura, mysz, gamepad lub PC VR.", "Dotyk, sensory telefonu albo kontrolery/hand tracking Quest.");
            Difference("Pliki i uprawnienia", "Klasyczny system plików desktopowych.", "Sandbox aplikacji Android i jawne uprawnienia, np. kamera.");
            Difference("Testowanie", "Build może działać na komputerze deweloperskim.", "Build trzeba zainstalować na urządzeniu; adb devices jest podstawowym testem połączenia.");
            Difference("Wydajność", "Nie przenoś wyników FPS z PC na urządzenie mobilne.", "Mierz standalone na docelowym urządzeniu i przy tej samej konfiguracji eksperymentu.");
        }

        private static void Heading(string text)
        {
            EditorGUILayout.LabelField(text, EditorStyles.boldLabel);
            EditorGUILayout.Space(4);
        }

        private static void Step(string title, string body)
        {
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            EditorGUILayout.LabelField(body, EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space(6);
        }

        private static void Difference(string name, string pc, string android)
        {
            EditorGUILayout.LabelField(name, EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"PC: {pc}", EditorStyles.wordWrappedMiniLabel);
            EditorGUILayout.LabelField($"Android: {android}", EditorStyles.wordWrappedMiniLabel);
            EditorGUILayout.Space(5);
        }

        private static void Command(string command)
        {
            EditorGUILayout.LabelField(command, EditorStyles.textField, GUILayout.Height(21));
        }

        private static void Note(string text)
        {
            EditorGUILayout.HelpBox(text, MessageType.Warning);
            EditorGUILayout.Space(6);
        }

        private static void OpenBuildProfiles()
        {
            if (!EditorApplication.ExecuteMenuItem("File/Build Profiles"))
                EditorApplication.ExecuteMenuItem("File/Build Settings...");
        }
    }
}

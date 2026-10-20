# Lab 01. Dostęp do Unity bez wspólnego hasła

## Spis treści

- [Nawigacja warunkowa](#nawigacja-warunkowa)
- [Wybierz jedną z trzech ścieżek](#wybierz-jedną-z-trzech-ścieżek)
- [Wariant A: komputer uczelniany, licencja Grant](#wariant-a-komputer-uczelniany-licencja-grant)
- [Wariant B: własny komputer, bezpłatne Unity Personal](#wariant-b-własny-komputer-bezpłatne-unity-personal)
- [Wariant C: darmowy Unity Student Plan (opcjonalnie)](#wariant-c-darmowy-unity-student-plan-opcjonalnie)
- [Ścieżka awaryjna bez konta w trakcie zajęć](#ścieżka-awaryjna-bez-konta-w-trakcie-zajęć)
- [Pytanie do samodzielnego rozważenia](#pytanie-do-samodzielnego-rozważenia)
  - [Oficjalne źródła](#oficjalne-źródła)


## Nawigacja warunkowa
- **Jestem na uczelnianym PC i licencja już działa:** nie loguj się na wspólne konto; wróć do [pierwszego doświadczenia](../../Samples~/Lab01/README.md#1-uruchom-scenę-i-pomiar-30).
- **Komputer uczelniany nie ma jeszcze aktywnej licencji:** [Wariant A](#wariant-a-komputer-uczelniany-licencja-grant); zgłoś to administratorowi.
- **Mam Unity ID i Personal:** pomiń rejestrację i przejdź do [pracy w Unity](../../Samples~/Lab01/README.md#1-uruchom-scenę-i-pomiar-30).
- **Nie mam Unity ID, pracuję na własnym komputerze:** [Wariant B](#wariant-b-własny-komputer-bezpłatne-unity-personal).
- **Chcę skorzystać z darmowego planu studenckiego:** [Wariant C](#wariant-c-darmowy-unity-student-plan-opcjonalnie).
- **Nie mam Unity Hub:** [instalacja zależna od systemu](OS_SETUP.md#unity-nie-jest-zainstalowane).


**Cel:** rozpocząć pracę w Unity na uczelnianym lub własnym komputerze, nie tracąc zajęć na zakładanie kont. Konto Unity i uprawnienie do uruchomienia edytora to dwie różne rzeczy.

## Wybierz jedną z trzech ścieżek

| Stanowisko | Rozwiązanie | Co robi student? |
|---|---|---|
| Komputer pracowni PRz | **Unity Education Grant License**, gdy uczelnia ją uzyska i administrator aktywuje na stanowiskach | Uruchamia przygotowany projekt Unity. **Nie zakłada i nie udostępnia żadnego konta.** |
| Własny komputer | Indywidualne **Unity ID + Unity Personal** zgodnie z warunkami licencji | Zakłada swoje bezpłatne konto Unity, loguje się do Hub i aktywuje licencję Personal |
| Własny komputer, student chce więcej narzędzi | **Unity Student Plan**, bezpłatny po weryfikacji statusu studenta | Używa swojego Unity ID i opcjonalnie aktywuje plan Student |

**Ważne:** korzystanie ze wspólnego loginu i hasła do Unity ID na obu grupach NIE jest ścieżką kursową. Unity zabrania udostępniania danych logowania osobom trzecim. Adres funkcyjny w domenie prz.edu.pl może zostać użyty do administrowania wnioskiem uczelni i otrzymywania korespondencji, jeśli upoważnia do tego administrator, ale **hasło pozostaje wyłącznie u administratora**.

## Wariant A: komputer uczelniany, licencja Grant

1. Administrator PRz sprawdza [Unity Education Grant License](https://unity.com/products/unity-education-grant-license), uprawnienie uczelni i liczbę wymaganych stanowisk; **nie przypisuje indywidualnych licencji Student do współdzielonych komputerów**.
2. Wykonuje weryfikację instytucji i formalny wniosek, konfiguruje licencję wielostanowiskową zgodnie z otrzymaną instrukcją Unity. Samo założenie Unity ID na skrzynkę funkcyjną nie stanowi jeszcze przyznanej licencji.
3. Na jednym komputerze testowym z kontem studenckim systemu Windows/Ubuntu uruchamia Unity Hub i zatwierdzoną wersję Unity. Sprawdza, że student nie musi się logować do prywatnego Unity ID. Następnie przygotowuje pozostałe stanowiska.
4. Do zajęć udostępnia gotowy projekt URP z WiRR, wersją edytora i materiałami. Uczeń/student uruchamia go bez dodatkowej rejestracji.
5. **Gdy licencja Grant nie została jeszcze przyznana**, nie udawaj, że bezlogowanie jest już dostępne; użyj zatwierdzonego przez uczelnię rozwiązania przejściowego. Studenci na własnych komputerach mogą wybrać bezpłatne konto Personal.

**Informacja dla prowadzącego:** Unity opisuje Education Grant jako bezpłatną licencję wielostanowiskową dla kwalifikujących się instytucji i wskazuje, że może ona działać na stanowiskach laboratoryjnych bez logowania studenta. Wniosek i instalację powinien wykonać administrator, zgodnie z aktualnymi warunkami oferty. Unity może stosować warunki korzystania z projektów/zasobów przenoszonych między typami licencji; przed pracą domową z projektem uczelnianym sprawdź zgodność.

## Wariant B: własny komputer, bezpłatne Unity Personal

1. Na [Unity ID](https://id.unity.com/) załóż prywatne konto lub zaloguj się na dotychczasowe. Użyj własnej skrzynki; **nie proś prowadzącego o wspólne hasło**.
2. W Unity Hub zaloguj się i otwórz **Preferences / Settings → Licenses → Add / Get a free personal license** (nazwy przycisków mogą różnić się między wydaniami).
3. Potwierdź uprawnienie do Unity Personal według aktualnych warunków Unity. Sam fakt, że ktoś jest studentem, nie zwalnia z warunków określających bezpłatną licencję.
4. Utwórz projekt **Universal 3D (URP)** zatwierdzoną wersją Unity i przejdź do Lab 01. Nie musisz kupować Asset Store ani Unity Pro.

## Wariant C: darmowy Unity Student Plan (opcjonalnie)

Student aktywnie studiujący w akredytowanej uczelni może zgłosić się do [Unity Student Plan](https://unity.com/products/unity-student). Po weryfikacji otrzymuje obecnie między innymi:

- **Unity Pro Editor** do własnego uczenia się i projektów w ramach warunków planu;
- edukacyjne licencje **Odin Inspector i Validator**;
- pakiet materiałów **Synty** oraz korzyści z Asset Store, w tym reklamowaną przez Unity **zniżkę 20%**;
- dostęp do podstawowego ekosystemu Unity Cloud i narzędzi współpracy w zakresie planu;
- roczną licencję z możliwością odnowienia po ponownym potwierdzeniu statusu.

To **oferta indywidualna**; nie zastępuje licencji wielostanowiskowej na komputery pracowni. Dodatki, szczegóły usług chmurowych i regulaminy mogą się zmieniać; sprawdź aktualną ofertę przed aktywacją. Nie musisz podawać własnego hasła ani robić płatnych zakupów, żeby uzyskać zaliczenie.

## Ścieżka awaryjna bez konta w trakcie zajęć

Jeżeli Unity Hub na danym komputerze wymaga logowania lub licencji, **nie próbuj obchodzić zabezpieczeń ani kopiować plików licencyjnych między komputerami**. Zgłoś brak aktywowanej licencji prowadzącemu, przejdź na przygotowane przez niego stanowisko lub wykonaj część projektową na dostarczonym materiale. Żaden student nie traci punktów za brak prywatnego Unity ID lub niedostępną licencję pracowni.

## Pytanie do samodzielnego rozważenia

**Co jest potrzebne do pracy w Unity: konto użytkownika, licencja edytora, projekt czy dodatkowe pakiety XR?** Rozdziel te cztery elementy. Konto Unity nie jest kontem GitHub, a zaimportowany projekt nie musi być związany z numerem Twojego indeksu.

### Oficjalne źródła
- [Unity Education Grant License](https://unity.com/products/unity-education-grant-license)
- [Unity Education](https://unity.com/solutions/education)
- [Unity Student](https://unity.com/products/unity-student)
- [Unity Personal](https://unity.com/products/unity-personal)
- [Unity Terms of Service](https://unity.com/legal/terms-of-service)

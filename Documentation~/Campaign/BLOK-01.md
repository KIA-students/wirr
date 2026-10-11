# Blok I: Świat reaguje. Lab 01 + Lab 02

**Spotkanie 1 z 4.** Pierwszy planowany termin: **wtorek 13 października 2026**. Łącznie cztery godziny dydaktyczne, dwie części. Student odkrywa, co odróżnia scenę komputerową od rzeczywistości wirtualnej, a następnie projektuje interakcję.

## Spis treści
- [Decyzja w 30 sekund](#decyzja-w-30-sekund)
- [Cel wyprawy](#cel-wyprawy)
- [Lab 01: skala, FPS i fizyka](#lab-01-skala-fps-i-fizyka)
- [Lab 02: chwyt, teleportacja i percepcja](#lab-02-chwyt-teleportacja-i-percepcja)
- [Prawdziwy sprzęt, bez walki z instalacją](#prawdziwy-sprzęt-bez-walki-z-instalacją)
- [Jedna karta bloku](#jedna-karta-bloku)
- [Ciekawostka i misja](#ciekawostka-i-misja)

## Decyzja w 30 sekund
- **Masz działający Unity Editor:** [zacznij od Lab 01](../../Samples~/Lab01/README.md#pierwsze-10-minut-eksperyment-zamiast-instalatora). Nie reinstaluj.
- **Nie masz Unity:** [instrukcja instalacji](../Reference/OS_SETUP.md#unity-nie-jest-zainstalowane).
- **Nie masz aktywnego konta/licencji:** [wybór licencji](../Reference/UNITY_ACCESS_AND_LICENSES.md#nawigacja-warunkowa). Na stanowisku uczelnianym może działać licencja Education Grant.
- **Masz Ubuntu:** [Ubuntu](../Reference/OS_SETUP.md#ubuntu), a **Windows:** [Windows](../Reference/OS_SETUP.md#windows).
- **Masz Quest 3:** po wykonaniu pierwszego testu PC [podłącz urządzenie](../Reference/OS_SETUP.md#urzadzenia-xr).
- **Nie masz gogli:** wykonaj całość podstawową w Unity Editor. XR Interaction Simulator jest wariantem rozszerzającym po instalacji XRI.

## Cel wyprawy

**Pytanie:** dlaczego 60 FPS nie zawsze wystarczy, żeby VR było komfortowe, a obiekt o nieprawidłowej skali może wprowadzać użytkownika w błąd? Celem jest porównanie sceny 3D, fizyki i zachowania interakcji.

## Lab 01: skala, FPS i fizyka

1. Otwórz **WiRR → Misje i odkrycia (4 bloki) → Start: Lab 01**. Pojawi się podłoga, światło i kamera. Nie potrzeba dodatkowych SDK.
2. Dodaj sześcian: **GameObject → 3D Object → Cube**, umieść go przed kamerą i zmieniaj rozmiar 0,5 → 1,0 → 2,0 metra. **Co się zmienia w projekcji i odczuciu wielkości?**
3. Dodaj **Rigidbody** i włącz Play. Sprawdź, dlaczego obiekt spada i co decyduje o kolizji z podłogą. Następnie zmień masę, ale **nie zmieniaj równocześnie kilku parametrów**.
4. Otwórz **Window → Analysis → Profiler**. Porównaj trzy krótkie odczyty liczby klatek w scenie bazowej i z dodatkowym obciążeniem (np. kilkadziesiąt sześcianów). Zapisz zmianę.
5. Wskaż, czy pomiar dotyczył CPU/GPU/Physics i co jeszcze należałoby sprawdzić, by stwierdzić przyczynę.
6. Zapisz scenę Lab 01. Pełny przewodnik: [Lab 01](../../Samples~/Lab01/README.md#zadania-krok-po-kroku).

**Rezultat minimalny:** działająca scena, trzy porównywalne obserwacje, jeden wniosek.

## Lab 02: chwyt, teleportacja i percepcja

1. Otwórz **Start: Lab 02**. Zanim włączysz XRI, zaproponuj, jak użytkownik miałby przenieść obiekt do wyznaczonego miejsca bez myszy.
2. Jeśli na stanowisku jest przygotowany XRI, użyj próbki Starter Assets, sprawdź chwyt, promień, socket i teleportację. Jeśli nie, rozpocznij od prostego demonstratora PC i zaprojektuj logikę interakcji.
3. Wykonaj trzy przeniesienia obiektu. Zmierz czas lub liczbę pomyłek, a następnie popraw **jeden** element UI, np. kolor statusu lub wskazówkę docelowego miejsca.
4. Na Quest 3, pod opieką prowadzącego, spróbuj tej samej interakcji: spust, grip i lokalizacja kontrolera mają znaczenie. Nie zakładaj, że laptop Ubuntu uruchomi Windowsowy Quest Link.
5. Dla chętnych: [komfor­t XR i wergencja–akomodacja](../Reference/COMFORT_STUDIES.md) oraz [watchdog techniczny](../Reference/XR_SAFETY_WATCHDOG.md). Ćwiczenia nie wymagają wywoływania objawów.
6. Zapisz Lab 02. Pełny przewodnik: [Lab 02](../../Samples~/Lab02/README.md#zadania-krok-po-kroku).

**Rezultat minimalny:** prosta interakcja lub logiczny prototyp, trzy próby, jeden zdiagnozowany problem.

## Prawdziwy sprzęt, bez walki z instalacją

Rotuj: jeden zespół sprawdza Quest 3, drugi pracuje na scenie desktopowej, potem zamiana. Przy problemie przekraczającym 10–15 minut przejdź do wariantu komputerowego i zapisz pierwszą konkretną przyczynę. Sygnał „gogle się ładują” nie dowodzi połączenia danych przez USB-C; sprawdź ADB tylko gdy jest wymagane.

## Jedna karta bloku

W **WiRR → Misje i odkrycia** wybierz **blok I**. Zapisz lokalną kartę sesji po wykonaniu Lab 01 i Lab 02. Panel odczyta istniejące raporty JSON i dane sprzętu; nie generuje wymyślonych pomiarów. Student dopisuje **jedno odkrycie, jedną diagnozę i jedno zastosowanie**, bez osobnych prezentacji i bez GitHub Actions.

## Ciekawostka i misja

**Ciekawostka:** w rzeczywistym szkoleniu serwisowym milimetrowa zgodność wirtualnego i prawdziwego obiektu bywa ważniejsza niż efektowna grafika.

**Misja wynalazcza:** zaproponuj, jak XR mógłby nauczyć bezpiecznego serwisowania maszyny albo obsługi narzędzia. Drużyny mogą za zgodą prowadzącego prezentować pomysł w tle Ligi XR, bez wpływu na ocenę.

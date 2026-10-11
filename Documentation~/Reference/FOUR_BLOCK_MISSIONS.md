# WiRR: cztery bloki, osiem laboratoriów i jedna karta na sesję

## Spis treści
- [Dokładnie cztery wyprawy](#dokładnie-cztery-wyprawy)
- [Co działa w pakiecie 1.1.0-preview.1](#co-działa-w-pakiecie-110-preview1)
- [Jedna karta sesji](#jedna-karta-sesji)
- [Rzeczywiste urządzenia i warianty](#rzeczywiste-urządzenia-i-warianty)
- [Stan wydania](#stan-wydania)

## Dokładnie cztery wyprawy

| Blok | Dwa laboratoria | Przewodnik |
|---|---|---|
| I | Lab 01 i Lab 02 | [Świat reaguje](../Campaign/BLOK-01.md) |
| II | Lab 03 i Lab 04 | [Świat się miesza](../Campaign/BLOK-02.md) |
| III | Lab 05 i Lab 06 | [Bliźniak rzeczywistości](../Campaign/BLOK-03.md) |
| IV | Lab 07 i **Lab 08** | [Wynalazca XR](../Campaign/BLOK-04.md) |

[Cała kampania](../Campaign/README.md) ma osiem oddzielnych instrukcji i osiem pozycji na liście laboratoriów Unity. Lab 08 to samodzielna próbka z formularzem JSON, nie opcjonalny dodatek do siedmiu starych laboratoriów.

## Co działa w pakiecie 1.1.0-preview.1

Panel **WiRR → Misje i odkrycia (4 bloki)** udostępnia przyciski **Start: Lab XX** dla wszystkich ośmiu laboratoriów. Generuje bazowe sceny bez wymuszania ARCore, ROS 2 i Meta SDK. Klasyczny panel kursu i WiRR Reports również obsługują Lab 08.

W Lab 08 polecenie **WiRR → Lab 08 → Dodaj demonstrator inspekcji** przygotowuje prefab i trzy obiekty testowe, a **Importuj wyniki demonstratora do raportu** wczytuje rzeczywisty JSON z ukończonych prób. W pozostałych laboratoriach formularze pozostają zgodne z dotychczasowym formatem.

## Jedna karta sesji

Po zakończeniu pary laboratoriów otwórz **WiRR → Misje i odkrycia**, wybierz blok i **Zapisz lokalną kartę sesji**. Karta zawiera dostępne sceny, metadane Unity, tryb zadeklarowany przez studenta i istniejące wyniki JSON.

Student wpisuje tylko: **jedno odkrycie, jedną diagnozę i jeden pomysł nowego zastosowania**. Brak wpisu pomiarowego nie jest zastępowany zerem. Nie wystawiamy ocen za liczbę uruchomień ani za czas Play Mode.

## Rzeczywiste urządzenia i warianty

- [Unity, licencja i indywidualne konto](UNITY_ACCESS_AND_LICENSES.md#nawigacja-warunkowa).
- [Windows i Ubuntu](OS_SETUP.md#wybierz-aktualną-sytuację).
- [Gogle i komfort XR](COMFORT_STUDIES.md).
- [Wyzwanie techniczne watchdog, max. 10 prób](XR_SAFETY_WATCHDOG.md).
- [Dobrowolna Liga XR, dwie grupy](../../competition/OPTIONAL_MISSIONS.md).

Nie udostępnia się studentom wspólnego hasła Unity, nie wymaga prywatnego sprzętu i nie udaje się fizycznych pomiarów przy symulacji.

## Stan wydania

**Zmiany są na gałęzi roboczej MatPomGit/wirr.** Kod nowych prefabów i integrację Lab 08 trzeba przetestować w Unity 6000.6.x Windows i Ubuntu przed scaleniem z wydaniem produkcyjnym. Komplet dokumentów Markdown można czytać na GitHubie bez instalowania Unity. Po weryfikacji aktualizacja może zostać przeniesiona do KIA-students/wirr z zachowaniem dotychczasowych wyników ligi.

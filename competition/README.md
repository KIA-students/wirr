# Liga XR: dwie grupy, cztery rundy

## Spis treści

- [Punktacja za blok](#punktacja-za-blok)
- [Aktualizacja wyniku](#aktualizacja-wyniku)
- [Własne nazwy i cztery dodatkowe misje](#własne-nazwy-i-cztery-dodatkowe-misje)
- [Dodatkowa misja VR z watchdogiem](#dodatkowa-misja-vr-z-watchdogiem)


Jedynym źródłem prawdy o punktach po publikacji jest competition/scoreboard.json w KIA-students/wirr. Wartość null oznacza brak oceny, nie zero punktów. W momencie utworzenia ligi żadna grupa nie prowadzi.

## Punktacja za blok

| Kategoria | Maks. | Co nagradzamy? |
|---|---:|---|
| Eksperyment i pomiary | 4 | Poprawną próbę, dane lub analizę awarii |
| Interpretacja i diagnostyka | 3 | Wyjaśnienie mechanizmu i ograniczeń |
| Pomysł i prototyp | 2 | Nowe zastosowanie technologii XR |
| Współpraca | 1 | Pomoc i przekazywanie wiedzy |

Maksymalnie 10 punktów na blok i 40 przez cały cykl. Wynik grupowy NIE wpływa na oceny indywidualne i nie zależy od liczebności grup, liczby urządzeń, szybkości ani czasu przed ekranem. Gdy sprzęt jest niedostępny, równoważny eksperyment symulacyjny z interpretacją ograniczeń może zdobyć pełne punkty.

## Aktualizacja wyniku

Prowadzący po zakończeniu bloku i zaakceptowaniu dowodów może ręcznie edytować plik albo lokalnie uruchomić skrypt competition/update_score.py z opcjami --round, --group, --experiment, --reasoning, --innovation, --cooperation, --reason. Wszystkie wartości muszą odpowiadać rzeczywiście zatwierdzonym rezultatom.

Przykład składni (NIE uruchamiać z fikcyjnymi punktami):

    python competition/update_score.py --round 1 --group G1 --experiment X --reasoning Y --innovation Z --cooperation W --reason "Uzasadnienie wspólnego wyniku"

X, Y, Z, W zastąp rzeczywistymi wartościami całkowitymi. Skrypt waliduje wynik i zapisuje historię, ale nie łączy się z siecią i nie wystawia ocen automatycznie. Raz na blok zatwierdź jeden commit lub PR z wynikami obu grup. Obecny workflow publikacji strony może odświeżyć ranking po tym commicie.

Nie publikuj danych osobowych, indeksów, prywatnych obrazów kamer, danych lokalizacyjnych ani tokenów. Uzasadnienie jest wspólne dla całej grupy. Aktualizacje repozytorium roboczego muszą zachować punkty przechowywane w KIA-students/wirr, nawet jeśli lokalna kopia jest starsza.

Zakładka na stronie kursowej: https://kia-students.github.io/wirr/#liga

## Własne nazwy i cztery dodatkowe misje

Nazwę drużyny ustalają studenci, przy użyciu generatora na stronie kursu lub własnego pomysłu. Prowadzący zatwierdza nazwę wywołaniem `python competition/set_team_name.py --group G1 --name "Kwantowe Kapibary"`. Żaden login nie jest potrzebny do losowania propozycji.

Zasady dobrowolnych misji z limitem **8 punktów na grupę poza zajęciami** zawiera [OPTIONAL_MISSIONS.md](OPTIONAL_MISSIONS.md). Jeden zatwierdzony rezultat z kodem `xr-optics`, `robot-failsafe`, `accessible-hud` lub `ar-measurement` daje 1–2 punkty na grupę, nie na osobę. Obsługuje je lokalny `competition/add_sidequest.py`. Wynik grupowy nie wpływa na indywidualną ocenę.

## Dodatkowa misja VR z watchdogiem

Kod bonusowy **xr-optics** obejmuje [zadanie Safe XR Watchdog](../Documentation~/Reference/XR_SAFETY_WATCHDOG.md), z lokalnym limitem 10 prób. Publiczny ranking przyznaje punkty dopiero po zatwierdzeniu jakości rozwiązania przez prowadzącego. Nie publikuje raportów o objawach ani nie punktuje narażania uczestników na dyskomfort.

## Nagroda dla zwycięskiej grupy

Po ukończeniu czterech rund i zatwierdzeniu wyników przez prowadzącego, każdy student zwycięskiej grupy otrzymuje podwyższenie swojej indywidualnej oceny końcowej z przedmiotu o pełny stopień oceny (1,0) (np. 3,5 → 4,5; maksimum 5,0). Obowiązuje to po spełnieniu podstawowych warunków zaliczenia i pod warunkiem zgodności z zatwierdzonym sylabusem i zasadami oceniania. W razie remisu konieczne jest rozstrzygnięcie przez prowadzącego. Punkty grupowe Ligi XR są przyznawane za osobne zadania i misje w pakiecie Unity WiRR, nie za sumę indywidualnych ocen studentów. Indywidualne punkty z laboratoriów i grupowe punkty Ligi XR są liczone osobno.

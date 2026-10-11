# Liga XR: dwie grupy, cztery rundy

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

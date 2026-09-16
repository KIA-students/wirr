# Laboratorium 2 — raport: interakcja, lokomocja i komfort użytkownika w VR

> Uzupełniaj raport na bieżąco. Nie wpisuj nazwisk ani nie dołączaj skanów wypełnionego SSQ.

## Identyfikacja i warianty

- Osoba A — numer indeksu: __________
- Osoba B — numer indeksu: __________
- Suma `S = i1 + i2`: __________
- `v1` (3.0): ___
- `v2` (3.5): ___
- `v3` (4.0): ___
- `v4` (4.5): ___
- `v5` (5.0): ___
- Data: __________
- Stanowisko: __________

Wzór: `v_k = 1 + ((S + 2(k - 1)) mod 5)`.

---

## Środowisko

- Unity: __________
- XR Interaction Toolkit: __________
- OpenXR: __________
- Input System: __________
- Tryb PC: XR Interaction Simulator / inne: __________
- Scena: `Assets/Scenes/Lab02_Interaction.unity`
- Quest 3 standalone build: tak / nie / niezweryfikowano

---

## Mapowanie wejścia

| Akcja | Action Map | Binding rzeczywisty | Przycisk / oś | Zweryfikowano |
|---|---|---|---|---|
| Select Left | XRI Left Interaction | | grip | tak / nie |
| Select Right | XRI Right Interaction | | grip | tak / nie |
| Activate Left | XRI Left Interaction | | trigger | tak / nie |
| Activate Right | XRI Right Interaction | | trigger | tak / nie |
| UI Press | XRI Left/Right Interaction | | trigger | tak / nie |
| Turn | XRI Right Locomotion | | prawy thumbstick | tak / nie |
| Teleport Mode Activate | XRI Left Locomotion | | X | tak / nie |
| Teleport Mode Cancel | XRI Left Locomotion | | Y | tak / nie |
| StartTrial | Lab02 | | A | tak / nie |
| ResetTrial | Lab02 | | lewy thumbstick click | tak / nie |
| ConfirmAlarm | Lab02 | | B | tak / nie |

- `Input Action Manager` korzysta z `Lab02_XR.inputactions`: tak / nie
- Czy występują równolegle dwie aktywne kopie tych samych map: tak / nie

---

# Punkt kontrolny 3.0 — chwyt, socket, ray

## Kontrola podstawowa

| Test | Wynik |
|---|---|
| jedna Main Camera | działa / nie działa |
| jeden XR Origin | działa / nie działa |
| jeden XR Interaction Manager | działa / nie działa |
| Direct Grab `Module_A` | działa / nie działa |
| Attach Transform naturalny | działa / nie działa |
| `Socket_A` przyjmuje właściwy moduł | działa / nie działa |
| `Socket_A` odrzuca niewłaściwy obiekt | działa / nie działa |
| Ray nie wybiera środowiska | działa / nie działa |

## Eksperyment kontrolny 3.0

- Wariant `v1`: ___
- Pytanie badawcze: __________
- Przewidywanie przed pomiarem: __________
- Warunki / poziomy moderatora: __________

| Warunek | Próba 1 [s] | 2 | 3 | 4 | 5 | Mediana [s] | Błędy łącznie |
|---|---:|---:|---:|---:|---:|---:|---:|
| A | | | | | | | |
| B | | | | | | | |
| C (jeśli dotyczy) | | | | | | | |

**Odpowiedź na pytanie badawcze:** __________

---

# Punkt kontrolny 3.5 — UI i feedback

## Panel World Space

| Element | Wynik |
|---|---|
| Start | działa / nie działa |
| Reset | działa / nie działa |
| Potwierdź alarm | działa / nie działa |
| stan: gotowy → montaż → alarm → ukończono | działa / nie działa |
| Hover różni się od Select | tak / nie |
| osobny dźwięk sukcesu i błędu | tak / nie |
| żądanie haptyki jest generowane | tak / nie |

## Eksperyment kontrolny 3.5

- Wariant `v2`: ___
- Pytanie badawcze: __________
- Przewidywanie: __________

| Warunek | Próba 1 [s] | 2 | 3 | 4 | 5 | Mediana [s] | Błędne / powtórzone aktywacje |
|---|---:|---:|---:|---:|---:|---:|---:|
| A | | | | | | | |
| B | | | | | | | |
| C (jeśli dotyczy) | | | | | | | |

**Wniosek:** __________

---

# Punkt kontrolny 4.0 — lokomocja

## Konfiguracja

- Teleportation Area: __________
- Teleportation Anchor: __________
- Snap Turn bazowy: 30° / inne: __________
- Smooth Turn: wyłączony / włączony
- Smooth Locomotion: wyłączony / włączony
- Niedozwolone powierzchnie są odrzucane: tak / nie

## Audyt komfortu

- automatyczny ruch kamery niezależny od głowy: brak / występuje
- sztuczny ruch pionowy: brak / występuje
- framerate stabilny: tak / nie
- cel teleportacji czytelny: tak / nie
- możliwość natychmiastowego przerwania: tak / nie

## Eksperyment kontrolny 4.0

- Wariant `v3`: ___
- Pytanie badawcze: __________

| Warunek | Próba 1 [s] | 2 | 3 | 4 | 5 | Mediana [s] | Błędy / dodatkowe korekty |
|---|---:|---:|---:|---:|---:|---:|---:|
| A | | | | | | | |
| B | | | | | | | |
| C (jeśli dotyczy) | | | | | | | |

**Wniosek:** __________

---

# Punkt kontrolny 4.5 — scenariusz i diagnostyka

## Scenariusz bazowy

| Próba | Czas [s] | Upuszczenia | Błędne aktywacje | Nieudane teleportacje | Błędy osadzenia / reset | Uwagi |
|---:|---:|---:|---:|---:|---:|---|
| 1 | | | | | | |
| 2 | | | | | | |
| 3 | | | | | | |
| 4 | | | | | | |
| 5 | | | | | | |
| **Mediana / suma** | | | | | | |

## Eksperyment diagnostyczny 4.5

- Wariant `v4`: ___
- Kontrolowany problem: __________
- Objaw mierzalny: __________
- Hipoteza H1: __________
- Hipoteza H2: __________
- Przewidywanie rozróżniające H1/H2: __________
- Test rozstrzygający: __________
- Wynik przed poprawką: __________
- Minimalna poprawka: __________
- Wynik po poprawce: __________
- Commit diagnostyczny: __________

---

# Meta Quest 3 i SSQ — obowiązkowe

## Kontrola Quest 3

| Test | Wynik |
|---|---|
| build standalone uruchamia się | działa / nie działa |
| tracking 6DoF | działa / nie działa |
| Direct Grab | działa / nie działa |
| Ray/UI | działa / nie działa |
| teleportacja | działa / nie działa |
| Snap Turn | działa / nie działa |
| fizyczna haptyka | działa / nie działa / niezweryfikowano |

## SSQ — Osoba A

> Do raportu wpisuj wyłącznie wyniki liczbowe. Formularz źródłowy znajduje się w `resources/SSQ-Lab.pdf` / Załącznik nr 1 instrukcji.

| Pomiar | N | O | D | TS |
|---|---:|---:|---:|---:|
| PRE | | | | |
| POST | | | | |
| **POST − PRE** | | | | |

- ekspozycja zakończona zgodnie z planem: tak / nie
- jeśli przerwano: `przerwanie bezpieczeństwa` / nie dotyczy
- czas ekspozycji [s]: __________

## SSQ — Osoba B

| Pomiar | N | O | D | TS |
|---|---:|---:|---:|---:|
| PRE | | | | |
| POST | | | | |
| **POST − PRE** | | | | |

- ekspozycja zakończona zgodnie z planem: tak / nie
- jeśli przerwano: `przerwanie bezpieczeństwa` / nie dotyczy
- czas ekspozycji [s]: __________

## Interpretacja SSQ

- Która podskala zmieniła się najbardziej u Osoby A? __________
- Która podskala zmieniła się najbardziej u Osoby B? __________
- Czy kierunek zmian był podobny? __________
- Wniosek ograniczony do danych z tej sesji: __________

Nie używaj SSQ jako diagnozy medycznej i nie wnioskuj o trwałej podatności danej osoby.

---

# Punkt kontrolny 5.0 — eksperyment na Quest 3

- Wariant `v5`: ___
- Osoba wykonująca eksperyment: A / B
- Pytanie badawcze: __________

| Warunek | Próba 1 [s] | 2 | 3 | Mediana [s] | Błędy / korekty | Komfort 0–10 |
|---|---:|---:|---:|---:|---:|---:|
| A | | | | | | |
| B | | | | | | |
| C (jeśli dotyczy) | | | | | | |

**Odpowiedź na pytanie badawcze:** __________

---

# Wnioski

1. **Interakcja:** __________
2. **Lokomocja i komfort:** __________
3. **Quest 3 / różnica względem symulatora:** __________
4. **SSQ — wyłącznie wniosek z tej sesji:** __________

# Git

Ostatni commit:
```text
lab02: complete interaction and comfort study
```

- SHA: __________
- Najwyższy osiągnięty punkt kontrolny: 3.0 / 3.5 / 4.0 / 4.5 / 5.0
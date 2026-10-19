# Liga XR: stan dowodów do oceny bloku I (19.10.2026)

> Status: **ocena wstępna, bez zatwierdzonej punktacji**. Ten dokument nie zastępuje `scoreboard.json` i nie przyznaje punktów. Pierwsza runda obejmuje **oba laboratoria 01 i 02**. Zasady ligi wymagają zatwierdzenia wyników przez prowadzącego po bloku. Danych identyfikujących studentów nie publikujemy w rankingu.

## G1

- Lab 01: pięć odrębnych zestawów indeksów i sześć plików zgłoszeniowych. Dwie kopie odnoszą się do jednego tego samego indeksu. Za aktualniejszą wersję uznano `lab01-e0f762da-f9a4-4c70-9c2d-8d4211a1aa59.json` (`updatedAtUtc = 2026-10-19T17:06:58.1371309Z`), zamiast `lab01-ef308297-af89-441f-b7dc-656f8d9a6c90.json` (`2026-10-19T16:54:21.0543263Z`). Nowszy raport nie staje się przez to automatycznie kompletny.
- Najpełniejszy zapis Lab 01 zawiera rozbudowane pomiary CPU i GPU wraz z interpretacją. W części pozostałych zgłoszeń eksperymenty GPU lub dalsze etapy pozostają puste.
- Lab 02: jeden raport z pomiarami trzech warunków chwytu i wnioskiem; dalsze punkty kontrolne są niekompletne.

## G2

- Lab 02: jeden **wspólny raport pary** (dwa numery indeksów zapisane w `studentIndices`), obejmujący pięć prób w każdym z trzech warunków chwytu, błędy i krótki wniosek. **Nie liczyć jako jednego studenta i nie obniżać wyniku za pracę w parze**.
- W bieżącym drzewie `student-work/` brak raportu Lab 01 grupy G2, ale w prywatnej ewidencji PRz odnaleziono dodatkowe cztery archiwa ZIP z wcześniejszego kanału oddawania prac: trzy obejmują Lab 01 i Lab 02, jedno tylko Lab 01. Tych prac nie wolno pomijać przy ocenie rundy. Archiwa i dane identyfikujące autorów pozostają wyłącznie w repozytorium prywatnym.

## Zasady porównania

- W pierwszej rundzie porównywać **jakość udokumentowanych eksperymentów, trafność interpretacji, innowację i współpracę**, nie liczbę plików, indeksów, sesji PlayMode ani posiadane urządzenia.
- Powtórne zgłoszenie tego samego laboratorium i indeksu nie nalicza osobnych punktów; podstawą jest aktualna wersja według `updatedAtUtc`, po kontroli treści.
- Praca w parze jest dopuszczalna. Dwa indeksy w raporcie oznaczają dwóch współautorów, ale sam fakt pary nie dowodzi jeszcze szczególnego osiągnięcia w kategorii współpracy.
- Zgłoszenia `schemaVersion: wirr-report/1.0` z pustym `submittedAtUtc` wymagają kontroli statusu przyjęcia przed ostatecznym zatwierdzeniem.
- **Bieżąca runda: oczekuje na zatwierdzenie**, obie punktacje w `scoreboard.json` pozostają `null`. `null` oznacza brak zatwierdzonej oceny, a nie zero.
- Nie ustalać zwycięzcy ani bonusu do ocen przed ukończeniem czterech rund i zatwierdzeniem punktów.

## Weryfikacja tożsamości

Numery indeksów znajdują się wewnątrz raportów, ale nie są publikowane w tym zestawieniu. W repozytorium `MatPomGit/prz` odszukano specyfikację oceniania WiRR (`config/06-KARTY-STUDENTA.md`, `config/16-ZADANIA-I-OCENIANIE.md`), **nie potwierdzono jednak imiennego wykazu studentów**. Powiązanie indeksów z osobami oraz uprawnieniami do zaliczenia należy sprawdzić w autorytatywnej ewidencji PRz.


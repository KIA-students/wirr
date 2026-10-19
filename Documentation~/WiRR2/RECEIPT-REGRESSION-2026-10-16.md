# WiRR: potwierdzenie przyjęcia raportu, 16.10.2026

## Błąd i poprawka

`Editor/WiRRPublicReportSubmission.cs` zawierał parser `IsAcceptedResponse`,
ale `SubmitFile` zamiast niego szukał dokładnego fragmentu `"accepted":true`.
Odpowiedź `{ "accepted": true }` była odrzucana. Odpowiedź z niezgodnym
`submission_id` mogła zostać uznana za potwierdzenie bieżącej wysyłki.

Ścieżka HTTP wywołuje teraz istniejący parser. Nadal wymaga sukcesu HTTP,
a potwierdzenie jest zapisywane dopiero po zaakceptowaniu odpowiedzi.
Zachowano zgodność z odpowiedzią bez `submission_id`; jeśli serwer podaje
identyfikator, musi on odpowiadać wysyłanemu zgłoszeniu.

## Walidacja

- Uruchomiono `python -m unittest discover -s scripts~ -p 'test_wirr*.py' -v`:
  **14 testów PASS**, w tym 40 podprzypadków lab/próg. To testy walidatora
  raportów Python, nie parsera Unity ani transportu HTTP.
- Dodano **12 przypadków EditMode** w `Tests/Editor/WiRRSubmissionReceiptTests.cs`:
  JSON zwarty i sformatowany, kolejność pól, wielkość liter identyfikatora,
  brak potwierdzenia, odmowa, zagnieżdżone pole, obcy identyfikator i błędny JSON.
- Testy korzystają z rzeczywistego `JsonUtility` i parsera produkcyjnego.
  **Nie uruchomiono ich tutaj: środowisko nie zawiera Unity.**
- Projekt `Project~` deklaruje pakiet w `testables`, aby Test Runner
  mógł znaleźć jego testy. Zależność Test Framework już była w projekcie.

## Następny odbiór

W przygotowanym projekcie Unity uruchomić EditMode dla
`KIA.WiRR.Editor.Tests.WiRRSubmissionReceiptTests`. Następnie wykonać próbę
dwóch raportów z jednego bloku: zapis, przyjęcie przez serwer, ponowienie po
awarii i potwierdzony PR/commit w KIA-students/wirr. Sam plik `.receipt.json`
nie jest dowodem przekazania do GitHub ani zatwierdzenia oceny.

Nie zamykać odbioru WiRR 2.0 na podstawie tej poprawki. Nie wykonano wysyłki
testowej na serwer ani publikacji raportów studentów.

## Kontynuacja: wersje plików i ponawianie po awarii

`WiRRReportStore.ExportFinal` nadpisywał plik o stałej nazwie, pozostawiając
stary `.submission-id` i `.receipt.json`. Poprawiony raport mógł odziedziczyć
identyfikator transportowy oraz potwierdzenie wcześniejszej treści. Eksport
zapisuje teraz osobny plik z nowym identyfikatorem w nazwie, używając
`FileMode.CreateNew`. Identyfikator dokumentu w JSON pozostaje identyfikatorem
pracy; identyfikator transportowy jest przypisany do konkretnego eksportu.
Ponowienie tego samego pliku nadal korzysta z jego `.submission-id`.

Eksporter GitHub nie wykonuje ponownie zapisu pliku, gdy istniejąca treść jest
identyczna. Tożsamość treści sprawdza przez Git blob SHA, odczytując plik dla
konkretnego SHA gałęzi. Po utracie odpowiedzi utworzenia PR lub potwierdzenia
statusu kolejki znajduje istniejący PR. Przed potwierdzeniem statusu wymaga
zgodności SHA z PR. Inna treść pod tym samym identyfikatorem jest błędem,
a nie zgodą na nadpisanie pracy. Nie zmieniano schematu bazy ani uprawnień.

Aktualna walidacja lokalna:

- **24 testy Python PASS**, w tym 10 nowych testów eksportera, z kontrolowanymi
  awariami PR i potwierdzenia kolejki. Wszystkie wywołania HTTP zastępuje lokalny
  model usług. Nie korzystają z rzeczywistej kolejki ani danych studentów.
- Trzy testy regresji uruchomione na poprzedniej wersji eksportera zakończyły
  się oczekiwanym FAIL: ponowny zapis pliku, nadpisanie zmienionej treści i
  potwierdzenie niezgodnego SHA. Te same testy przechodzą po poprawce.
- Dodano 3 przypadki EditMode dotyczące zachowania poprzedniego pliku,
  rozdzielenia potwierdzeń i UTF-8. Łącznie **15 przypadków Unity pozostaje
  nieuruchomionych** z powodu braku Unity w tym środowisku.
- Istniejący ręczny workflow walidacji uruchamia teraz wszystkie `test_wirr*.py`.
  Nadal obowiązuje tylko `workflow_dispatch` na `main`; workflow nie był uruchamiany.

Odbiór produkcyjny pozostaje otwarty: Unity EditMode i test pełnego przepływu.
Ręczna zmiana wcześniej wysłanego pliku nie jest wspieranym sposobem korekty;
należy użyć formularza i nowego eksportu. Testy tego etapu nie obejmowały
jeszcze ręcznego zamknięcia lub scalenia PR; uzupełnienie opisano poniżej.

Dokumentacja formatu blobów: https://git-scm.com/book/en/v2/Git-Internals-Git-Objects
oraz API treści GitHub: https://docs.github.com/en/rest/repos/contents.

## Kontynuacja: zamknięte i scalone PR

Eksporter odczytuje historię PR (`state=all`, docelowa gałąź `main`) przed
utworzeniem gałęzi. Zamknięty PR bez scalenia wymaga interwencji operatora;
ponowienie nie tworzy kolejnego PR i nie potwierdza przyjęcia. Więcej niż jeden
PR dla identyfikatora również zatrzymuje automatyczne rozstrzygnięcie.

Po scaleniu eksporter odczytuje plik dla `merge_commit_sha` i porównuje Git blob
SHA z treścią kolejki. Przy zgodności zapisuje potwierdzenie z SHA scalenia oraz
adresem istniejącego PR, bez zapisów do GitHub. Działa także po usunięciu gałęzi.
Brak pliku, inna treść lub brak SHA nie potwierdzają dostarczenia. Status
`reviewed` zachowuje dotychczasowe znaczenie transportowe; nie nadaje oceny.
Ponowne sprawdzenie stanu PR po zapisie wykrywa zamknięcie w trakcie próby;
nie stanowi transakcji między GitHub i kolejką.

Walidacja: **31 testów Python PASS**, w tym 7 nowych metod testowych. Wszystkie
7 wykrywają regresje na eksporterze z commita `68abe57` (9 niepowodzeń wraz
z podprzypadkami, bez błędów wykonania); po poprawce przechodzą. Kontrolowane
awarie obejmują także utratę potwierdzenia kolejki po scaleniu, niezgodność
scalonej treści, wiele PR i zamknięcie w trakcie próby. Model HTTP uwzględnia
filtrowanie stanu PR oraz odrębny plik w commicie scalenia.

Nie zmieniano API, schematu ani uprawnień Supabase. Nie wykonywano połączeń
z kolejką, zapisów danych studentów ani uruchomień Actions. Nadal wymagane:
15 przypadków Unity EditMode i rzeczywisty E2E dwóch raportów przed wydaniem.

Źródło parametrów API: https://docs.github.com/en/rest/pulls/pulls#list-pull-requests.

## Kontynuacja: niezależne raporty i kolejne strony kolejki

Poprzedni eksporter przerywał po pierwszym błędnym raporcie, brakującym polu
lub awarii publikacji. Pobierał też wyłącznie pierwsze 30 rekordów pending.
W efekcie problem jednej pracy mógł zatrzymać wysyłkę kolejnych studentów.

Walidację i transport pojedynczej pracy wydzielono do `export_report`.
Błąd pozostawia tę pracę w pending, jest logowany i nie zatrzymuje niezależnych
raportów. Po zakończeniu dowolny błąd daje kod wyjścia 1, więc częściowy sukces
nie jest przedstawiany jako pełny sukces. Brak kluczy daje nadal kod 2.
Błędy autoryzacji/limitu HTTP 401, 403 i 429 oraz niedostępność odczytu kolejki
przerywają przebieg, aby nie zużywać żądań dla kolejnych zgłoszeń.

Kolejkę odczytuje się stronami po 30 rekordów, rosnąco po `submission_id`,
z filtrem `submission_id=gt.<ostatni UUID>`. Nie stosuje się offsetu, którego
znaczenie zmieniałoby się po potwierdzeniu wcześniejszych prac. Pełna strona
błędnych rekordów nie blokuje następnej strony. Brak postępu kursora zatrzymuje
przebieg. Kolejność UUID zastępuje kolejność czasu wpłynięcia i nie jest FIFO.
Nowy raport z UUID przed aktualnym kursorem będzie obsłużony w kolejnym przebiegu.

Walidacja lokalna: **40 testów Python PASS**, w tym 9 nowych metod testowych
obsługi wielu raportów. Model usług utrzymuje osobny stan każdego zgłoszenia,
filtruje aktualny status i UUID przy każdym pobraniu strony. Sprawdzono błędne
pola, brakujące pole, awarię tworzenia PR, utratę potwierdzenia kolejki,
31 poprawnych raportów, 30 błędnych przed poprawnym, globalne HTTP 401/403/429,
awarię odczytu kolejki oraz nieruchomy kursor. Na kodzie `d4308fb` nowe testy
wykazały 8 niepowodzeń asercji wraz z podprzypadkami i 2 nieobsłużone wyjątki
(brak pola oraz HTTP 503 podczas odczytu kolejki). Po poprawce wszystkie przechodzą.

Wykonano wyłącznie odczyt metadanych `information_schema.columns` projektu KIA:
`public.wirr_public_intake.submission_id` ma typ `uuid`, a `status` typ `text`.
Nie odczytywano raportów studentów, nie zapisywano do kolejki i nie zmieniano
schematu ani uprawnień. Nowe zapytanie REST nadal wymaga potwierdzenia w E2E;
wyniki modelu HTTP nie stanowią walidacji usługi produkcyjnej. Nie uruchamiano
Actions. Nadal wymagane: 15 przypadków Unity EditMode i pełny E2E dwóch raportów.

Dokumentacja filtra `gt` i porządku zapytań:
https://docs.postgrest.org/en/stable/references/api/tables_views.html.

## Kontynuacja: projekt odbiorowy i uruchamianie Unity

Wykryto błąd konfiguracji `Project~/Packages/manifest.json`: zależność
`pl.prz.kia.wirr` miała wartość `file:..`. Unity rozwiązuje ją względem katalogu
`Packages`, zatem wskazywała `Project~`, a nie katalog pakietu z `package.json`.
Poprawiono na `file:../..`. Zależności i pliku packages-lock.json nie regenerowano
ręcznie; ich faktyczne rozwiązanie musi wykonać Unity. Projekt wskazuje obecnie
edytor **6000.6.5f1**, odczytany z `ProjectSettings/ProjectVersion.txt`.

Dodano `scripts~/run_wirr_editmode.py`. Narzędzie wymaga Python 3.10+ oraz
zainstalowanego i działającego lokalnego edytora z dotychczasową licencją.
Nie instaluje edytora, nie aktywuje licencji i nie uruchamia Actions.
Przed testem zamknij ten projekt w edytorze, aby uniknąć blokady projektu.
Z katalogu repozytorium uruchom w PowerShell:

```powershell
py ".\scripts~\run_wirr_editmode.py"
```

Launcher szuka wersji z ProjectVersion.txt w standardowym katalogu Unity Hub.
Jeśli edytor jest zainstalowany w innym miejscu, podaj jego ścieżkę:

```powershell
py ".\scripts~\run_wirr_editmode.py" --unity "D:\Unity\6000.6.5f1\Editor\Unity.exe"
```

Linux/macOS: `python3 scripts~/run_wirr_editmode.py`, opcjonalnie `--unity`.
`--check-only` sprawdza konfigurację bez uruchamiania Unity i jawnie informuje,
że testów nie wykonano. Nie jest dowodem odbioru EditMode.

Rzeczywisty przebieg uruchamia tylko assembly `KIA.WiRR.Editor.Tests` w EditMode,
bez `-quit`, którego Test Framework nie obsługuje podczas wykonywania testów.
Wyniki trafiają do nowego katalogu `Project~/Logs/wirr-editmode-*`:
`editor.log`, `results.xml` oraz `summary.json`. Podsumowanie zapisuje polecenie,
czas UTC, SHA repozytorium i informację o lokalnych zmianach (jeśli Git jest
dostępny), kod procesu oraz wynik walidacji. Katalog Logs jest już ignorowany
przez Git; po rzeczywistym odbiorze dowody należy dołączyć do PR lub issue.

Kod 0 wymaga jednocześnie powodzenia procesu oraz wszystkich 15 wymaganych
przypadków w XML: 4 akceptacji, 8 odrzucenia i 3 wersji eksportu. Pusty XML,
niekompletne zestawy, pominięcia, powtórzone nazwy, błąd procesu i timeout
nie zaliczają odbioru. Każdy przebieg otrzymuje osobny katalog, więc stary XML
nie może potwierdzić nowego testu. Narzędzie nie wykonuje testu wysyłki do serwera.

Walidacja wykonana tutaj: **48 testów Python PASS**, w tym 8 testów launchera
oraz parsera dowodów NUnit. Kontrola `--check-only` przechodzi z poprawionym
manifestem; test z poprzednią ścieżką `file:..` odrzuca konfigurację. Testy
launchera wykorzystują syntetyczny XML i zastępują proces Unity; nie są testami
C#. Nadal nie wykonano 15 przypadków Unity ani rzeczywistego E2E dwóch raportów.
Brak Unity pozostaje ograniczeniem środowiska; PR pozostaje draft.

Źródła:
- https://docs.unity.com/en-us/engine/6000.7/manual/packages-list/dependencies-lp/upm-localpath
- https://docs.unity.com/en-us/engine/6000.6/manual/programming-environment/test-framework-introduction/reference-command-line

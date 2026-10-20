# Automatyczne przekazywanie raportów WiRR

Domyślnie raport laboratoryjny zapisany w formularzu Unity jest wysyłany do serwera, a następnie automatycznie przekazywany jako zgłoszenie do **KIA-students/wirr**. Student nie instaluje Git ani GitHub CLI i nie tworzy własnego forka. [Kroki dla studenta i awaria](QUICK_START.md).

Przycisk **Zapisz raport lokalnie (bez logowania)** uruchamia zapis JSON i transport. Potwierdzenie serwera oznacza przyjęcie do kolejki. Dopiero URL zgłoszenia i SHA potwierdzają przekazanie do repozytorium. Publikacja i zatwierdzenie oceny są osobnymi etapami. Po każdym z dwóch laboratoriów zapisuje się osobny raport; na końcu bloku sprawdza się oba potwierdzenia. E-mail jest tylko awaryjną alternatywą po nieudanej próbie ponowienia wysyłki.

## Ponowienie i poprawiona wersja

Każdy eksport z formularza zapisuje nowy plik JSON. Wcześniejsze pliki i ich
potwierdzenia pozostają zachowane. Gdy wysyłka nie doszła do skutku, użyj
**Wyślij ponownie zapisany raport** i wybierz ten sam plik. Gdy poprawiasz treść,
otwórz raport w formularzu i wyeksportuj go ponownie; nowy plik otrzyma osobne
zgłoszenie. Nie edytuj ręcznie pliku, dla którego zapisano już potwierdzenie.

Eksporter po awarii korzysta z istniejącego pliku i PR, jeśli treść jest identyczna.
Inna treść pod tym samym identyfikatorem zatrzymuje przetwarzanie; nie nadpisuje
wcześniejszej pracy. Dla otwartego PR status kolejki jest potwierdzany dopiero po zgodności SHA
commita z nagłówkiem PR. Po scaleniu sprawdzana jest treść pliku w commicie
scalenia, także po usunięciu gałęzi. Zamknięcie bez scalenia wymaga wyjaśnienia
przez prowadzącego; automat nie tworzy ponownie zamkniętego zgłoszenia. To potwierdzenie przekazania, nie ocena pracy.

## Techniczny postęp a raport

Synchronizacja `students/progress` służy do śledzenia technicznego postępu i nie zastępuje raportu laboratoryjnego. Nie jest dodatkowym zadaniem do oddania. Oceny zatwierdza prowadzący.

## Warunek wydania WiRR 2.0

Pełny przepływ wymaga sprawdzenia na stanowisku: zapis dwóch raportów, przyjęcie przez serwer, utworzenie zgłoszeń w repozytorium i ponowienie po awarii bez duplikacji. Sam opis funkcji i poprawny zapis lokalny nie potwierdzają działania całej usługi.


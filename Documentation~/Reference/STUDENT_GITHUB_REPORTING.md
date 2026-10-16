# Automatyczne przekazywanie raportów WiRR

Domyślnie raport laboratoryjny zapisany w formularzu Unity jest wysyłany do serwera, a następnie automatycznie przekazywany jako zgłoszenie do **KIA-students/wirr**. Student nie instaluje Git ani GitHub CLI i nie tworzy własnego forka. [Kroki dla studenta i awaria](QUICK_START.md).

Przycisk **Zapisz raport lokalnie (bez logowania)** uruchamia zapis JSON i transport. Potwierdzenie serwera oznacza przyjęcie do kolejki. Dopiero URL zgłoszenia i SHA potwierdzają przekazanie do repozytorium. Publikacja i zatwierdzenie oceny są osobnymi etapami. Po każdym z dwóch laboratoriów zapisuje się osobny raport; na końcu bloku sprawdza się oba potwierdzenia. E-mail jest tylko awaryjną alternatywą po nieudanej próbie ponowienia wysyłki.

## Techniczny postęp a raport

Synchronizacja `students/progress` służy do śledzenia technicznego postępu i nie zastępuje raportu laboratoryjnego. Nie jest dodatkowym zadaniem do oddania. Oceny zatwierdza prowadzący.

## Warunek wydania WiRR 2.0

Pełny przepływ wymaga sprawdzenia na stanowisku: zapis dwóch raportów, przyjęcie przez serwer, utworzenie zgłoszeń w repozytorium i ponowienie po awarii bez duplikacji. Sam opis funkcji i poprawny zapis lokalny nie potwierdzają działania całej usługi.

# WiRR: przewodnik prowadzącego

Wersja 1.0.0

## Zakres i dokumenty

Osiem laboratoriów, realizowanych samodzielnie. Kolejne bloki obejmują 01+02, 03+04, 05+06 i 07+08. Terminy i czas zajęć wynikają z planu uczelnianego.

[Wymagania etapów](GRADING_MATRIX.md) · [Instalacja](QUICK_START.md) · [Szablon Lab 02](../../Samples~/Lab02/report-template.md)

## Sprawdzenie pracy studenta

1. Otwórz scenę i potwierdź działanie elementów wymienionych w wybranym punkcie kontrolnym
2. Sprawdź wariant wynikający z indeksu, rzeczywiste pomiary, jednostki i liczbę prób
3. Poproś o interpretację wyniku i wyjaśnienie zastosowanej konfiguracji
4. Otwórz lokalny JSON raportu i sprawdź numer laboratorium oraz grupę
5. Zatwierdź ocenę w ewidencji prowadzącego

Eksport raportu, zapis sceny i techniczny PR postępu nie oznaczają zaliczenia. Nie oceniaj pracy na podstawie liczby commitów ani czasu Play Mode.

## Lab 02

| Etap | Dowód |
|---|---|
| 3.0 | Chwyt, gniazdo i promień; 5 prób na warunek własnego wariantu |
| 3.5 | UI i informacja zwrotna; 5 prób na warunek |
| 4.0 | Lokomocja; 5 prób na warunek |
| 4.5 | Scenariusz bazowy: 5 prób; diagnoza problemu i wynik po poprawce |
| 5.0 | Quest 3, SSQ PRE/POST autora raportu; 3 próby w A i 3 w B |

Warunek C występuje wyłącznie w wariantach wskazanych w [tabeli wariantów](LAB02_CONSISTENCY_AUDIT.md). SSQ i rzeczywisty test gogli dotyczą etapu 5.0.

## Instalacja i oddanie

Pakiet: `https://github.com/KIA-students/wirr.git#upm`. Student podaje indeks i grupę w **WiRR → Student → Profil i grupa**. Formularz otwiera przyciskiem właściwego laboratorium.

**Eksportuj raport lokalnie** zapisuje JSON bez transportu Git. Student przekazuje plik kanałem ustalonym na zajęciach. Opcjonalny [PR postępu](STUDENT_GITHUB_REPORTING.md) pozostaje osobną funkcją.

## Diagnostyka

Dla problemu z importem sprawdź wersję Unity 6000.6.5f1, adres `#upm` i pierwszy błąd kompilacji. Dla chwytu lub UI sprawdź kolejno akcję wejścia, interaktor, warstwy interakcji i obiekt. Brak pomiaru pozostaje brakiem danych.

Test interfejsu w Unity oraz urządzenia Quest 3 wykonuje się na rzeczywistym stanowisku. Testy Markdown i agregacji punktów nie potwierdzają działania sprzętu.

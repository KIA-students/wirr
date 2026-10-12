# Lab 02: zgodność ścieżki studenta

Stan odniesienia: `KIA-students/wirr:main` 3c27e0f67ea4182ec93a90ea334b2715dc829eff i `upm` f963abb1497d0cbcfbf6f42ed067fe9c8a7ec898 (12.10.2026). Kluczowe pliki edytora i próbka Lab 02 są w tych dwóch gałęziach identyczne.

## Wykryte problemy i poprawki

| Problem | Poprawka |
|---|---|
| Przycisk raportu bloku I otwiera zawsze Lab 01 | Oddzielne przyciski raportów Lab 01 i Lab 02, analogicznie w kolejnych blokach |
| Profil trzeba odnaleźć ręcznie po imporcie | Jednorazowe otwarcie profilu po zakończeniu kompilacji; bez uruchamiania w batch mode |
| Grupa widoczna w oknie, ale nieobecna w JSON raportu | Dodatkowe pole `group`; zapis indeksu i grupy z profilu przy zapisie i odczycie szkicu |
| Formularz może działać bez profilu, więc lokalny postęp się nie zapisuje | Raport kieruje do rejestracji profilu przed wypełnianiem |
| Formularz wysyła pełny JSON, podczas gdy szybki start opisuje lokalny zapis | Przycisk lokalnego eksportu bez wywołania transportu Git; eksport nie oznacza oddania |
| Postęp odświeża się dopiero przy kolejnym tyknięciu minutowym | Natychmiastowe odświeżenie po rejestracji i eksporcie, plus istniejący zapis okresowy |
| Warunek C wymagany mimo dwuwarunkowego wariantu | Tabele formularza, obliczenia i walidacja używają liczby warunków właściwej dla wariantu |
| SSQ wymagane od dwóch osób mimo pracy samodzielnej | Tylko PRE i POST osoby A, czyli autora raportu; zachowane identyfikatory komórek |
| Instrukcja importuje XRI dwukrotnie i sugeruje jego opcjonalność dla obowiązkowego chwytu | Jedna sekwencja importu; rozróżnienie demonstracji PC i konfiguracji XRI |

## Liczba warunków w Lab 02

Zgodnie z tabelami wariantów pełnej instrukcji Lab 02:

| Etap | Warianty z A/B/C | Warianty z A/B | Próby na warunek |
|---|---|---|---|
| 3.0 | 2, 3, 4 | 1, 5 | 5 |
| 3.5 | 1, 2, 4 | 3, 5 | 5 |
| 4.0 | 1, 5 | 2, 3, 4 | 5 |
| 5.0 | brak | 1–5 | 3 |

Wariant: `v_k = 1 + ((i + 2(k - 1)) mod 5)`. Wiersz C nie jest wyświetlany ani wymagany, jeśli nie dotyczy wariantu. Tabele ogólne w panelu konfiguracji mogą pokazywać pełny zestaw warunków; formularz konkretnego studenta dobiera je z indeksu.

## Ścieżka sprawdzenia w Unity 6.6

1. W nowym projekcie URP zaimportuj pakiet. Po kompilacji powinien pojawić się profil. Wybierz G2, zapisz poprawny indeks, zamknij i ponownie otwórz Unity. Profil i grupa powinny pozostać zapisane. Nie używaj rzeczywistych danych studenta w testach publicznych.
2. Otwórz blok I. Przyciski raportów Lab 01 i Lab 02 mają otwierać odpowiednie dokumenty także wtedy, gdy inne okno raportu już jest otwarte.
3. Dla indeksu testowego 10000 sprawdź: v1=1 (A/B), v2=3 (A/B), v3=5 (A/B/C), v5=4 (A/B). W nowych, osobnych projektach sprawdź pozostałe reszty indeksu modulo 5.
4. Uzupełnij etap 3.0, wyeksportuj JSON i sprawdź `labNumber=2`, dokładnie jeden indeks, `group=G2` i warianty. Raport nie powinien być wysłany przez Git ani otrzymać daty oddania tylko z powodu eksportu. Sprawdź odtworzenie szkicu po ponownym otwarciu Unity.
5. Sprawdź `Library/WiRRProgress/progress.json`: `reportExported=true` dla Lab 02 po eksporcie; `gradingStatus=not_assessed`, `submissionStatus=local_only`. Opcjonalna synchronizacja postępu pozostaje osobną funkcją. Zapisana scena i eksport nie są oceną.

## Materiały kursowe wymagające równoległej synchronizacji

W repozytorium materiałów dydaktycznych należy ujednolicić `upm-setup.tex`, makro `raport` w `wirr_lab02.tex`, szablon Markdown i `sessions/lab-02.md`. Obecnie skrócona sesja opisuje trzy próby, a pełna instrukcja pięć. W dostępnym PDF v1.0.3 opisano wysyłkę pełnego raportu, podczas gdy aktualne źródło v1.0.4 już opisuje lokalny zapis. Wydanie PDF trzeba zbudować z ostatecznego, uzgodnionego źródła.

## Walidacja i ograniczenia

`git diff --check` przechodzi. Istniejące testy agregacji Ligi XR przechodzą (5 testów), podobnie testy WebSim (12 testów); nie zastępują testu interfejsu Unity. Globalny skrypt nawigacji zgłasza zastany brak spisu treści w QUICK_START.md. Instrukcje Lab 02 mają lokalnie rozwiązywalne linki. Nie wykonano kompilacji w Unity ani testu na Quest 3 w tym środowisku. Pełne potwierdzenie gotowości wymaga wykonania powyższej ścieżki w Unity.

# Laboratoria 03 i 04. Przekazanie wyników przez istniejący formularz WiRR

## Spis treści

- [Procedura dla studenta](#procedura-dla-studenta)
- [Założenia i ograniczenia adaptera](#założenia-i-ograniczenia-adaptera)
- [Walidacja przed udostępnieniem studentom](#walidacja-przed-udostępnieniem-studentom)

Nowe sceny SIM_AR zapisują próby w lokalnej sesji `wirr-block-session/2.0-draft.1`. Dotychczasowy formularz laboratoryjny i usługa Supabase wymagają raportu `wirr-report/1.0`. Nie przesyłaj eksportów `wirr-report/2.0-draft.1` jako gotowych raportów w starym formacie.

## Procedura dla studenta

1. Uruchom przygotowany projekt Unity i laboratorium `Lab03_AnchorGallery`. W scenie wprowadź indeks i grupę, wybierz `Rozpocznij / wznów sesję`, wykonaj próby i zapisz wniosek.
2. Otwórz `Lab04_DepthRoom` w tym samym bloku i profilu. Wykonaj próby i zapisz drugi wniosek. Sesja jest zapisywana automatycznie do lokalnego katalogu WiRR.
3. Zakończ tryb **Play**. W edytorze wybierz `WiRR → Nowe instrukcje → Przenieś wyniki do formularza raportu`. Jeżeli nie masz profilu w istniejącym formularzu WiRR, aplikacja poprosi o jego uzupełnienie.
4. W oknie wyboru pliku otwórz zapisaną sesję bloku 02 z katalogu `Application.persistentDataPath/WiRR/sessions`. Wybierz laboratorium 03. Numer indeksu i grupa muszą odpowiadać profilowi WiRR.
5. Otwarty zostanie istniejący formularz raportu 03. Sprawdź opis warunków, wszystkie próby, brakujące wartości, błędy i wniosek. Jeżeli nie napisano wniosku w scenie, uzupełnij go tutaj. Kliknij `Sprawdź kompletność raportu`.
6. Kliknij `Zapisz raport lokalnie (bez logowania)` i poczekaj na odrębne potwierdzenie **WiRR: raport przyjęty**. Formularz używa już istniejącego punktu odbioru Supabase i zapisuje potwierdzenie obok raportu.
7. Powtórz import tej samej sesji, wybierając laboratorium 04. Sprawdź drugi formularz i wyślij drugi raport. Zweryfikuj **dwa odrębne potwierdzenia**.

Przeniesienie danych do formularza samo w sobie **nie wysyła** raportu. Przycisk sceny `Eksportuj dwa wyniki bloku` tworzy jedynie dodatkową lokalną kopię danych pomiarowych. Wysłanie danych wymaga użycia przycisku istniejącego formularza WiRR.

W przypadku braku potwierdzenia skorzystaj z `Wyślij ponownie zapisany raport`. W razie awarii zapisz oryginalne pliki i zgłoś błąd prowadzącemu; dalsze postępowanie opisuje [instrukcja awaryjna](../Reference/QUICK_START.md).

## Założenia i ograniczenia adaptera

- Adapter nie posiada nowego endpointu, klucza sieciowego ani własnego klienta HTTP. Transport realizuje niezmieniony `WiRRPublicReportSubmission`.
- Import akceptuje tylko poprawną sesję bloku 02, bez aktywnej próby, z indeksem i grupą zgodnymi z lokalnym profilem.
- Przenosi identyfikatory prób, warunki, statusy, wartości metryk i jawne braki do pól krótkiego raportu `basic.*`. Źródło danych jest opisane jako syntetyczne.
- Zapisane istniejące obserwacje nie są zastępowane bez dodatkowego potwierdzenia. Sam import nie nadaje oceny ani nie deklaruje pomiaru urządzenia.
- Pozostałe pola istniejącego formularza i jego walidator decydują o możliwości wysyłki. Brak wniosku uniemożliwia poprawne ukończenie krótkiego raportu.
- Mechanizm tworzenia zgłoszenia GitHub z kolejki pozostaje odrębny od potwierdzenia przyjęcia przez Supabase.

## Walidacja przed udostępnieniem studentom

W edytorze Unity uruchom sceny 03 i 04 i zweryfikuj, że import pozwala wybrać każdą z nich, zachowuje pomiary i nie akceptuje obcego indeksu/grupy. Sprawdź, że formularz wymaga wniosku oraz że transport zapisuje potwierdzenie z serwera. Następnie przetestuj ponowienie po błędzie sieci i utworzenie zgłoszenia w systemie KIA. Nie utożsamiaj przeglądu kodu z wykonaniem tych testów.

# Raportowanie WiRR 2.0: wszystkie cztery bloki przez istniejący formularz Supabase

Aktualizacja: 20.10.2026. Adapter Unity importuje wyniki **laboratoriów 01–08** z lokalnych sesji WiRR 2.0 do **istniejącego** formularza `wirr-report/1.0`. Nie wdraża nowego endpointu ani nie wysyła danych automatycznie. Potwierdzenie przyjęcia przez Supabase i późniejsze przekazanie zgłoszenia do GitHub są **osobnymi zdarzeniami**.

## Bloki, sceny i źródła danych

| Blok | Laboratoria | Sceny | Oczekiwane źródła |
|---|---|---|---|
| 01 | 01, 02 | `Lab01_Station`, `Lab02_Interaction` | `DESKTOP_SIM`, `DESKTOP_SIM` |
| 02 | 03, 04 | `Lab03_AnchorGallery`, `Lab04_DepthRoom` | `SIM_AR`, `SIM_AR` |
| 03 | 05, 06 | `Lab05_CADWorkshop`, `Lab06_DigitalTwin` | `DESKTOP_SIM`, `ROS_REPLAY` |
| 04 | 07, 08 | `Lab07_Expedition`, `Lab08_ExperienceStudio` | `DESKTOP_SIM`, `DESKTOP_SIM` |

Zapis `ROS_REPLAY` nie jest dowodem pracy z robotem ani połączenia `ROS_LIVE`. Metryka RTT w scenie 06 pozostaje niedostępna. Próby `FAILED` i brakujące pomiary są zachowywane jawnie.

## Procedura dla studenta

1. Otwórz projekt **Unity 6000.6.5f1**, `WiRR → Nowe instrukcje → Materiały studenta`, wybierz właściwą scenę oraz `Play`. Wpisz indeks lub dwa indeksy, grupę G1/G2 i wybierz `Rozpocznij / wznów sesję`.
2. Wykonaj doświadczenia obu laboratoriów danego bloku. Po każdym zapisz `Twój wniosek (jeden na laboratorium)`. Nie zmieniaj konfiguracji podczas aktywnej próby. Lokalne wyniki znajdują się w `Application.persistentDataPath/WiRR/sessions`.
3. Po zakończeniu próby **wyjdź z Play**. W edytorze sprawdź `WiRR → Student → Profil i grupa`, a następnie wybierz **`WiRR → Nowe instrukcje → Przenieś wyniki do formularza raportu`**.
4. Wskaż zapisany **plik sesji odpowiedniego bloku**, nie samodzielny eksport raportu. Adapter odrzuci sesję z otwartą próbą, błędnym schematem, innym indeksem lub grupą i pomiarami z nieodpowiedniego źródła.
5. Wybierz **pierwszy** z dwóch numerów laboratoriów zaproponowanych przez okno. Adapter importuje obserwacje, warunki i metryki (także niedostępne), otwiera `WiRR → Raporty → Formularz raportu laboratoryjnego` i pozwala uzupełnić własny wniosek. **Import nie oznacza wysłania raportu**. Przed nadpisaniem istniejącego formularza otrzymasz ostrzeżenie.
6. Sprawdź indeksy, G1/G2, numer laboratorium, statusy prób, opis źródła oraz wniosek. Naciśnij **`Sprawdź kompletność raportu`**, popraw błędy i wybierz **`Zapisz raport lokalnie (bez logowania)`**.
7. Poczekaj na odrębne potwierdzenie **`WiRR: raport przyjęty`** z identyfikatorem zgłoszenia. To potwierdzenie odbioru przez istniejący endpoint Supabase, **nie automatyczna ocena** ani potwierdzenie publikacji w GitHub.
8. Ponownie wybierz `Przenieś wyniki do formularza raportu`, wskaż **tę samą sesję**, wybierz **drugie laboratorium** bloku. Wyślij drugi raport. Zachowaj **dwa odrębne potwierdzenia**.

`Eksportuj dwa wyniki bloku` zapisuje pliki `WiRR-labNN.json` i `block.json` w trybie `LOCAL_ONLY`. Eksport jest kopią zapasową danych, nie transportem do Supabase. Nie zamieniaj tego eksportu na ręcznie konstruowany `wirr-report/1.0`; adaptację wykonuje sprawdzany kod w Unity.

## Ścieżka bez sieci i wydruk

Przy problemie z połączeniem zachowaj raport JSON i wybierz `Wyślij ponownie zapisany raport` po odzyskaniu sieci. Jeżeli nie ma potwierdzenia, skorzystaj z [procedury awaryjnej](../Reference/QUICK_START.md) i odpowiedniego szablonu `WiRR-blok-0N-raport-awaryjny.md`.

Kanoniczne instrukcje i załączniki (karty pomiarowe, ankieta i SSQ) są utrzymywane w [repozytorium dydaktycznym prz](https://github.com/MatPomGit/prz/tree/main/courses/wirtualna-i-rozszerzona-rzeczywistosc/laboratories). W tym pakiecie pozostaje implementacja adaptera Unity i opis techniczny transportu. Karta papierowa nie jest pokwitowaniem serwera.

## Bezpieczeństwo i odbiór

- Adapter wykorzystuje wyłącznie istniejący `WiRRReportStore`, `WiRRReportWindow` i `WiRRPublicReportSubmission`. Nie przechowuje nowych sekretów ani kluczy Supabase.
- Dane trafiają do publicznego endpointu dopiero po świadomym kliknięciu przycisku wysyłki w formularzu. Istniejące kontrole schematu i tożsamości pozostają aktywne.
- Należy uruchomić testy NUnit `WiRRStudentReportBridgeTests` oraz rzeczywisty test edytora dla każdego bloku. Sprawdź import dwóch laboratoriów osobno, brak przenikania sesji między grupami, zachowanie `FAILED`, braków i wniosków, odmowę przy aktywnej próbie i niezależne pokwitowania.
- Funkcja Supabase `wirr-public-intake` obsługuje aktualnie numery laboratoriów **1–8**, dlatego do uruchomienia czterech bloków nie trzeba modyfikować produkcyjnego Edge Function ani schematu bazy. Potwierdzenie pełnej integracji wymaga testu end-to-end na stanowisku i potwierdzenia serwera.

# Laboratorium 06. Aktualność stanu bliźniaka cyfrowego w odtwarzanym strumieniu JointState

## Spis treści

- [Pobranie materiałów i uruchomienie](#pobranie-materiałów-i-uruchomienie)
- [Cel ćwiczenia i interpretacja sygnałów](#cel-ćwiczenia-i-interpretacja-sygnałów)
- [Przygotowanie stanowiska, około 10 minut](#przygotowanie-stanowiska-około-10-minut)
- [Ćwiczenie wstępne, około 10 minut](#ćwiczenie-wstępne-około-10-minut)
- [Doświadczenie 1. Świeży i nieaktualny stan, około 15 minut](#doświadczenie-1-świeży-i-nieaktualny-stan-około-15-minut)
- [Doświadczenie 2. Wpływ progu STALE, około 20 minut](#doświadczenie-2-wpływ-progu-stale-około-20-minut)
- [Doświadczenie 3. Kontrpróba i strategia reakcji, około 20 minut](#doświadczenie-3-kontrpróba-i-strategia-reakcji-około-20-minut)
- [Opracowanie wyników, około 15 minut](#opracowanie-wyników-około-15-minut)
- [Raport w formularzu WiRR i potwierdzenie Supabase](#raport-w-formularzu-wirr-i-potwierdzenie-supabase)
- [Diagnostyka stanowiska](#diagnostyka-stanowiska)

WiRR 2.0, blok 03, część 2. Czas: 90 minut. Scena `Lab06_DigitalTwin`, tryb `ROS_REPLAY`, Unity **6000.6.5f1**. Źródłem jest **lokalny, syntetyczny zapis ruchu**, nie działający robot ani sieć ROS 2. Do wykonania ćwiczenia nie trzeba instalować Dockera, ROS 2, Gazebo ani konfigurować połączenia sieciowego.

## Pobranie materiałów i uruchomienie

1. Otwórz [materiały kursu](https://github.com/KIA-students/wirr). Instrukcja znajduje się w `Documentation~/WiRR2/`, a scena w `Samples~/Lab06/WiRR2/Lab06_DigitalTwin.unity`. Na komputerze uczelnianym skorzystaj z **przygotowanego projektu Unity**, zamiast pobierać ponownie pliki.
2. Jeżeli przygotowujesz stanowisko samodzielnie, zainstaluj [Unity Hub](https://unity.com/download) oraz edytor **6000.6.5f1**, zgodnie z [instrukcją Windows/Ubuntu](../Reference/OS_SETUP.md). W Unity Hub wybierz `Projects → New project → Universal 3D (URP)`, jeśli nie masz jeszcze projektu z pakietem WiRR.
3. W Unity wybierz `Window → Package Manager → + → Install package from git URL…`. Wklej `https://github.com/KIA-students/wirr.git#upm` i zatwierdź `Install`. W systemie musi działać Git. Nie instaluj ROS 2 ani WebSocket/rosbridge tylko po to, aby odtworzyć gotowy zapis.
4. Otwórz przez `Unity Hub → Projects → Open/Add` przygotowany projekt, odczekaj na zakończenie kompilacji i sprawdź `Window → General → Console`. Błędy kompilacji zgłoś przed rozpoczęciem próby.
5. W menu Unity wybierz `WiRR → Nowe instrukcje → Materiały studenta`, w panelu `Doświadczenia WiRR` kliknij `Otwórz Lab06_DigitalTwin`. Jeżeli scena nie jest dostępna, prowadzący powinien wybrać `WiRR → Nowe instrukcje → Przygotuj osiem scen (prowadzący)`. Scena po przygotowaniu znajduje się w `Assets/WiRR/WiRR2/Lab06_DigitalTwin.unity`.
6. Kliknij `Play`, przejdź do widoku `Game` i sprawdź panel po lewej. Wybierz te same indeksy i grupę **G1/G2**, co w laboratorium 05, a następnie `Rozpocznij / wznów sesję`. W bloku 03 oba laboratoria są odrębnymi wynikami tej samej sesji.

Ścieżka na gotowym stanowisku: **Unity Hub → projekt → WiRR → Nowe instrukcje → Materiały studenta → Lab06_DigitalTwin → Play → Rozpocznij / wznów sesję**.

## Cel ćwiczenia i interpretacja sygnałów

Celem jest sprawdzenie, czy płynna animacja modelu robota gwarantuje aktualność jego danych źródłowych. Wiadomość `JointState` wiąże wartości kątów z **nazwami przegubów**. Poprawne mapowanie nazw jest ważniejsze niż przypadkowa kolejność kolumn lub pozycji w tablicy. W przygotowanym zapisie scena odtwarza `joint1` i `joint2`, których położenia zapisano w **radianach**; wyświetlana geometria jest dydaktycznym modelem dwóch przegubów.

`ROS_REPLAY` oznacza odtwarzanie gotowego pliku. `ROS_LIVE` oznaczałby bieżący odbiór stanu ze źródła ROS i **nie jest wykorzystywany w tej próbie**. `STALE` jest stanem danych, których wiek przekracza przyjęty próg. Odświeżanie obrazu (klatki/s) i wiek danych to osobne wielkości. Gdy źródło zamilknie, ostatnio pokazana poza może nadal wyglądać wiarygodnie.

Scena udostępnia progi watchdoga **0,25 s**, **0,5 s** i **1 s**, a także przerwy **1,5 s** i **3 s**. W czasie wymuszonej przerwy zegar wirtualnego pliku jest wstrzymywany, natomiast upływa czas odbiornika. Czas do rozpoznania `STALE` i czas odzyskania świeżych danych nie są pomiarami opóźnienia sieci. `RTT` (*round-trip time*) wymaga rzeczywistego żądania i odpowiedzi, dlatego w lokalnym `ROS_REPLAY` jest **niedostępny**, podobnie jak opóźnienie transportowe źródło–odbiornik.

## Przygotowanie stanowiska, około 10 minut

1. Sprawdź nazwę `Lab06_DigitalTwin` oraz etykietę `ROS_REPLAY`. Jeżeli panel pokazuje inne laboratorium, naciśnij `Następny lab` lub wróć do panelu materiałów.
2. Potwierdź profil bloku 03, indeksy, grupę i brak aktywnej próby. Jeśli dane są niezgodne, nie kontynuuj na cudzym profilu.
3. Wybierz `STALE 0,5 s` oraz `Przerwa 1,5 s`. Pozostaw wyłączone `Ukryj model przy STALE (zamiast zamrażać)`.
4. Zlokalizuj w `Game` model robota, licznik wiadomości, wiek odbiornika i etykietę `ŚWIEŻE dane replay`/`STALE`. W panelu są także liczby zdarzeń STALE i powrotów świeżych danych.
5. Wybierz etap `3.0`. Jeżeli nie ma źródła danych, modelu lub dostępnego przycisku `Rozpocznij próbę`, zgłoś problem prowadzącemu. Nie uruchamiaj własnego `rosbridge`.

## Ćwiczenie wstępne, około 10 minut

Włącz `Trening (bez zapisu do porównania)`, a następnie `Rozpocznij próbę`. Obserwuj zmiany kątów dwóch przegubów i licznik odebranych wiadomości. Sprawdź, czy wiek odbiornika po każdej nowej wiadomości maleje, choć liczba wyświetlanych klatek może być znacznie większa niż częstotliwość danych. Kliknij `Wstrzymaj strumień i przywróć` dopiero po pojawieniu się pierwszej wiadomości. Scena automatycznie przywróci dane po zaplanowanej przerwie. Kliknij `Zapisz obserwację`, wyłącz trening i wróć do właściwych pomiarów.

## Doświadczenie 1. Świeży i nieaktualny stan, około 15 minut

1. Wybierz etap `3.0`, próg `STALE 0,5 s` i `Przerwa 1,5 s`. Wyłącz ukrywanie modelu przy STALE; w tym wariancie model zachowuje ostatnią pozę.
2. Naciśnij `Rozpocznij próbę`. Poczekaj na pojawienie się komunikatu `ŚWIEŻE dane replay` oraz co najmniej jednej odebranej wiadomości.
3. Naciśnij `Wstrzymaj strumień i przywróć`. **Nie naciskaj ponownie** tego przycisku w tej samej próbie. Poczekaj do stanu `STALE`, a następnie do powrotu świeżych danych.
4. Obserwuj w trakcie przerwy licznik klatek obrazu, wiek odbiornika, liczbę `Przejścia STALE` i `Powroty świeżych danych`. Po odzyskaniu danych naciśnij `Zapisz obserwację`.
5. Otwórz `Wyniki tego laboratorium`, odszukaj ostatnią próbę i sprawdź jej status. Zapisz metryki `messages_received`, `stale_episodes` i `fresh_recoveries`. Wartość `rtt` ma pozostać niedostępna, **nie wynosi 0 ms**.

## Doświadczenie 2. Wpływ progu STALE, około 20 minut

1. Wybierz etap `3.5`. Przy stałej przerwie `1,5 s` wykonaj pierwszą próbę z `STALE 0,25 s`: rozpocznij, wymuś przerwę, zaczekaj na powrót i kliknij `Zapisz obserwację`.
2. Wykonaj analogiczne próby przy progach `0,5 s` oraz `1 s`. Nie zmieniaj długości przerwy, profilu ani stanowiska w ramach tego porównania.
3. Zestaw rzeczywiste wyniki. Zwróć uwagę na metryki dotyczące **wieku danych przy pierwszym wykryciu STALE** i **odzyskania świeżych danych**, jeśli wystąpiły. Nazwy dostępnych metryk odczytaj z zapisu sesji; brak obserwacji oznacz `BRAK`, nie wpisuj wartości przewidywanej jako pomiar.
4. W etapie `4.0` uzasadnij, dlaczego próg zbyt duży opóźnia reakcję na przerwę, a próg zbyt mały może powodować fałszywe alarmy przy nieregularnych wiadomościach. W tym demonstratorze nie wykonuje się niezależnego pomiaru rzeczywistego jittera sieciowego.

| Próg [s] | Przerwa [s] | Zdarzenia STALE | Powroty świeżych danych | Czas do STALE [s], jeśli dostępny | Status |
|---:|---:|---:|---:|---:|---|
| 0,25 | 1,5 | | | | |
| 0,5 | 1,5 | | | | |
| 1,0 | 1,5 | | | | |

## Doświadczenie 3. Kontrpróba i strategia reakcji, około 20 minut

1. Wybierz `4.5`. Ustal jeden uzasadniony próg (np. z poprzedniego porównania) oraz przerwę `1,5 s`. Włącz `Ukryj model przy STALE (zamiast zamrażać)` i wykonaj pełną próbę: uruchom, wymuś przerwę, zaczekaj na odzyskanie, zapisz obserwację.
2. Powtórz próbę z identycznym progiem i przerwą, ale **wyłącz** ukrywanie modelu. Porównaj komunikat `STALE` oraz zachowanie geometrii: **ukrycie** i **zamrożenie** oznaczają różne decyzje prezentacyjne.
3. Dla etapu `5.0` ustaw `Przerwa 3 s` bez zmiany progu watchdoga. Powtórz próbę i określ, jak dłuższy brak danych wpływa na czas niedostępności aktualnego stanu i decyzję o widoczności modelu.
4. Oceń, czy model robota powinien pozostawać widoczny podczas utraty źródła, i zaproponuj **bezpieczną prezentację niepewności** w interfejsie. Nie przypisuj demonstratorowi funkcji bezpieczeństwa robotycznego, których nie wdrożono i nie zwalidowano.
5. W każdym wariancie zachowaj źródło `ROS_REPLAY`. Zmiana przełącznika ukrywania nie czyni danych bieżącymi ani nie uruchamia `ROS_LIVE`.

## Opracowanie wyników, około 15 minut

W polu `Twój wniosek (jeden na laboratorium)` opisz jedną zależność popartą identyfikatorami prób oraz pomierzonymi wartościami: „Przy progu … s przerwa … s spowodowała …; mimo … kl./s dane były …; do prezentacji nieaktualnego stanu wybrałam/wybrałem …, ponieważ …”. Użyj własnych wyników i wyraźnie oddziel od nich przewidywania.

| Poziom | Oczekiwany dowód |
|---|---|
| 3.0 | Odczyt statusu źródła, nazw przegubów i jednostek |
| 3.5 | Próba z wymuszoną przerwą i zaobserwowanym stanem STALE |
| 4.0 | Wyjaśnienie różnicy między płynnością obrazu a aktualnością danych |
| 4.5 | Porównanie progu i reakcji ukrycie/zamrożenie na tych samych danych |
| 5.0 | Kontrpróba z drugą długością przerwy i uzasadnieniem ograniczenia strategii |

Nie uznawaj `ROS_REPLAY` za dowód działającego ROS 2, Gazebo czy rzeczywistego robota. W tym trybie **brak jest pomiaru RTT i opóźnienia sieciowego**. Poziomy w panelu oznaczają etapy dydaktyczne, a nie automatycznie wystawione oceny.

## Raport w formularzu WiRR i potwierdzenie Supabase

1. Po ostatniej próbie wpisz wniosek. Opcjonalnie użyj `Eksportuj dwa wyniki bloku` i zachowaj lokalne `WiRR-lab05.json`, `WiRR-lab06.json` oraz `block.json`. W manifestach taki eksport ma status `LOCAL_ONLY`, więc **nie oznacza wysłania**.
2. Wyłącz tryb `Play`. W Unity otwórz `WiRR → Student → Profil i grupa` i sprawdź zgodność indeksu i G1/G2 ze sceną. Następnie wybierz `WiRR → Raporty → Formularz raportu laboratoryjnego`.
3. Z listy `Laboratorium` wybierz **06**, zaznacz **krótki raport**, jeśli nie jest wymagany wariant szczegółowy. Na podstawie zapisanej sesji wpisz rzeczywiste warunki, obserwacje, wniosek i ograniczenia. Źródło opisz jako `ROS_REPLAY, lokalny syntetyczny JointState`. Wpisz jawnie `RTT: niedostępne, ponieważ nie mierzono cyklu żądanie–odpowiedź`. Nie wpisuj wymyślonych wartości ani nieudokumentowanego stanu `ROS_LIVE`.
4. Kliknij `Sprawdź kompletność raportu`. Po poprawieniu braków wybierz `Zapisz raport lokalnie (bez logowania)`. Oczekuj odrębnego komunikatu **„WiRR: raport przyjęty”** z identyfikatorem zgłoszenia.
5. Sprawdź, czy dla poprzedniego laboratorium **05** również istnieje niezależne potwierdzenie. Dwa potwierdzenia serwera oznaczają przyjęcie obu raportów bloku 03, lecz nie automatyczną ocenę i nie potwierdzenie końca synchronizacji z GitHub.

**Ograniczenie aktualnej integracji:** menu `WiRR → Nowe instrukcje → Przenieś wyniki do formularza raportu` akceptuje tylko **blok 02 (lab 03–04)**. Nie używaj go dla laboratoriów 05–06; wyniki tego bloku przenoś **ręcznie** do standardowego formularza korzystającego z istniejącego kanału Supabase. Nie wysyłaj surowego `wirr-report/2.0-draft.1` jako zgodnego z formularzem `wirr-report/1.0`. Przy poprawnym potwierdzeniu nie korzystaj z forka, Git CLI ani poczty.

Jeżeli nie ma potwierdzenia, w formularzu kliknij `Wyślij ponownie zapisany raport`; po przerwie sieciowej ponów po odzyskaniu połączenia. Jeśli serwer nadal nie odpowiada, zgłoś prowadzącemu i postępuj zgodnie z [instrukcją awaryjną bloku 03](WiRR-blok-03-raport-awaryjny.md) oraz [zasadami przekazywania wyników](../Reference/QUICK_START.md). Nie przesyłaj całego projektu Unity.

## Diagnostyka stanowiska

Jeżeli licznik wiadomości pozostaje na zerze, sprawdź, czy rzeczywiście uruchomiono scenę `Lab06_DigitalTwin`, czy trwa `Play` i czy nie pojawił się błąd w `Window → General → Console`. Jeżeli nie można kliknąć `Wstrzymaj strumień i przywróć`, poczekaj na pierwszą odebraną wiadomość: przycisk jest aktywny tylko podczas próby i przed pierwszą przerwą. Aby powtórzyć eksperyment, rozpocznij **nową próbę**, nie wymuszaj drugiej przerwy w tej samej. Jeśli próba jest oznaczona `FAILED`, zachowaj jej status oraz powód zamiast usuwać ją z danych.

Pełna wersja laboratorium z rzeczywistym ROS 2, Gazebo, transportem, buforem i pomiarem RTT to [osobna ścieżka rozszerzona](../Reference/LAB6_STARTER_README.md). Wymaga stanowiska z przygotowanym środowiskiem i odrębnych pomiarów. Jej wyniki **nie powstają automatycznie** w scenie `ROS_REPLAY`.

Materiały kursowe: [KIA-students/wirr](https://github.com/KIA-students/wirr). Wersja instrukcji: 20.10.2026.
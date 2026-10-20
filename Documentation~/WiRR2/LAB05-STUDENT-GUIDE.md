# Laboratorium 05. Optymalizacja reprezentacji CAD i ocena zachowania cech geometrycznych

## Spis treści

- [Pobranie materiałów i pierwsze uruchomienie](#pobranie-materiałów-i-pierwsze-uruchomienie)
- [Cel i zasady pomiaru](#cel-i-zasady-pomiaru)
- [Przygotowanie stanowiska, około 10 minut](#przygotowanie-stanowiska-około-10-minut)
- [Ćwiczenie wstępne, około 10 minut](#ćwiczenie-wstępne-około-10-minut)
- [Doświadczenie 1. Porównanie High, Medium i Low, około 15 minut](#doświadczenie-1-porównanie-high-medium-i-low-około-15-minut)
- [Doświadczenie 2. Automatyczny LOD i koszt renderowania, około 20 minut](#doświadczenie-2-automatyczny-lod-i-koszt-renderowania-około-20-minut)
- [Doświadczenie 3. Progi LOD i kontrpróba, około 20 minut](#doświadczenie-3-progi-lod-i-kontrpróba-około-20-minut)
- [Opracowanie wyników, około 15 minut](#opracowanie-wyników-około-15-minut)
- [Przekazanie raportu do formularza WiRR i Supabase](#przekazanie-raportu-do-formularza-wirr-i-supabase)
- [Diagnostyka stanowiska](#diagnostyka-stanowiska)

WiRR 2.0, blok 03, część 1. Czas: 90 minut. Scena: `Lab05_CADWorkshop`, tryb `DESKTOP_SIM`, Unity **6000.6.5f1**. Ćwiczenie można wykonać na komputerze bez zestawu XR, telefonu, konwertera STEP i instalacji ROS 2. Wyniki odnoszą się do dydaktycznej siatki proceduralnej, **nie do pomiaru dokładności przemysłowego modelu CAD**.

## Pobranie materiałów i pierwsze uruchomienie

Na stanowisku uczelnianym przejdź od razu do punktu 4, jeśli projekt Unity jest przygotowany. Nie twórz nowego projektu zamiast istniejącego.

1. Otwórz publiczne [materiały WiRR](https://github.com/KIA-students/wirr). Instrukcje znajdują się w katalogu `Documentation~/WiRR2/`, a przykładowa scena w `Samples~/Lab05/WiRR2/`. Do ćwiczenia na komputerze nie trzeba pobierać całego repozytorium.
2. Jeżeli pracujesz na własnym komputerze, zainstaluj [Unity Hub](https://unity.com/download), zaloguj się na swoje Unity ID i w `Installs → Install Editor` wybierz edytor **6000.6.5f1**. Sprawdź [instrukcję dla Windows i Ubuntu](../Reference/OS_SETUP.md). Nie instaluj Android SDK, OpenXR ani pakietów do gogli na potrzeby tej sceny.
3. Dla nowego stanowiska wybierz w Unity Hub `Projects → New project → Universal 3D (URP) → Create project`. W Unity otwórz `Window → Package Manager`, wybierz `+ → Install package from git URL…`, wklej `https://github.com/KIA-students/wirr.git#upm` i naciśnij `Install`. Komputer musi mieć działający Git; w razie komunikatu o jego braku skorzystaj z [konfiguracji systemu](../Reference/OS_SETUP.md). Jeżeli na stanowisku jest już pełny projekt z pakietem WiRR, pomiń instalację.
4. Otwórz przygotowany projekt przez `Unity Hub → Projects → Open` lub `Add`, wskaż katalog projektu i poczekaj na zakończenie importu. Brak błędów blokujących w `Window → General → Console` jest warunkiem rozpoczęcia pomiarów.
5. W edytorze wybierz `WiRR → Nowe instrukcje → Materiały studenta`. W oknie `Doświadczenia WiRR` naciśnij `Otwórz Lab05_CADWorkshop`. Jeżeli sceny nie ma, zgłoś to prowadzącemu; prowadzący może wybrać `WiRR → Nowe instrukcje → Przygotuj osiem scen (prowadzący)`. Po przygotowaniu scenę znajdziesz w `Assets/WiRR/WiRR2/Lab05_CADWorkshop.unity`.
6. Uruchom scenę przyciskiem `Play` na górnym pasku Unity. Pracuj w zakładce `Game`. Po lewej pojawi się panel eksperymentu; po prawej widok części. Jeżeli panel nie mieści się na ekranie, przewijaj jego zawartość.
7. Wpisz własny numer indeksu, a w przypadku pracy w parze również numer drugiej osoby. Wybierz **G1** albo **G2**, zgodnie z planem zajęć, i kliknij `Rozpocznij / wznów sesję`. Te same dane będą używane w laboratorium 06. Nie wpisuj indeksu do publicznego komentarza ani do repozytorium ręcznie.

Skrócona ścieżka na gotowym stanowisku: **Unity Hub → przygotowany projekt → WiRR → Nowe instrukcje → Materiały studenta → Lab05_CADWorkshop → Play → Rozpocznij / wznów sesję**.

## Cel i zasady pomiaru

Celem jest ocena kompromisu między szczegółowością siatki trójkątów, kosztem renderowania i zachowaniem cechy funkcjonalnej, którą w tym ćwiczeniu jest **otwór montażowy**. Rzeczywisty obiekt CAD ma geometrię konstrukcyjną; siatka renderowania jest jej aproksymacją. Uproszczenie siatki może zmniejszyć koszt, ale pozbawić model cechy wymaganej do instruktażu lub montażu.

Przygotowane warianty `High` i `Medium` zachowują otwór, natomiast `Low` go nie zachowuje. Są to modele dydaktyczne wygenerowane proceduralnie, a nie trzy wyniki konwersji dostarczonego pliku STEP. `LOD` (*level of detail*) określa wybór poziomu szczegółowości zależny m.in. od odległości kamery; osobnym zagadnieniem jest geometria kolizji. Z samej liczby trójkątów nie można wnioskować o czasie GPU, liczbie wywołań renderera ani zgodności collidera.

Scena zapisuje rzeczywiste próbki **czasu klatki w Unity**: 2 s rozgrzewki i 8 s pomiaru podczas jednakowej trasy kamery. `cad_frame_median` oraz `cad_frame_p95` są podane w milisekundach; `triangles_min/max/median` dotyczą aktywnej siatki. `gpu_draw_calls` pozostaje niedostępne, ponieważ liczba materiałów nie mierzy liczby wywołań GPU. Porównania wykonuj na tym samym komputerze, przy tych samych ustawieniach rozdzielczości i jakości.

## Przygotowanie stanowiska, około 10 minut

1. Upewnij się, że na górze panelu widać nazwę laboratorium 05, `DESKTOP_SIM` oraz profil bloku 03 z poprawnym indeksem i grupą.
2. Sprawdź trzy przyciski `High`, `Medium` i `Low`. Wyłącz `Automatyczny LOD według odległości` oraz zaznacz `1 materiał`.
3. Zaznacz `Widok kontrolny otworu (poza benchmarkiem)`. Obejrzyj geometrię i wskaż otwór montażowy. Ten widok służy inspekcji, a nie porównaniu czasu klatki.
4. Wybierz etap `3.0` i `Trening (bez zapisu do porównania)`. Przed rozpoczęciem właściwych prób odznacz trening.
5. Sprawdź, czy przycisk `Rozpocznij próbę` jest aktywny. Jeśli Console zgłasza błędy lub brak obiektu, poinformuj prowadzącego i nie wpisuj fikcyjnych wartości.

## Ćwiczenie wstępne, około 10 minut

Porównaj wizualnie trzy warianty w widoku kontrolnym: `High` i `Medium` zachowują otwór, `Low` go wypełnia. Zapisz we własnych słowach, dlaczego liczba trójkątów nie jest wystarczającym kryterium wyboru modelu przeznaczonego do montażu. Wyłącz widok kontrolny przed uruchomieniem benchmarku. Próba treningowa nie stanowi dowodu do porównania.

## Doświadczenie 1. Porównanie High, Medium i Low, około 15 minut

1. Odznacz `Trening`. Pozostaw wyłączony automatyczny LOD i wybierz `1 materiał`. Ustaw etap `3.0`.
2. Wybierz `High`, kliknij `Rozpocznij próbę` i poczekaj około 10 s. Kamera porusza się automatycznie; nie zmieniaj ustawień w trakcie próby.
3. W części `Wyniki tego laboratorium` odczytaj status (`VALID` albo `FAILED`), `cad_frame_median`, `cad_frame_p95`, liczbę trójkątów i informację o zachowaniu otworu. Zapisz wartości wraz z jednostkami.
4. Wybierz `Medium` i powtórz pomiar. Następnie wybierz `Low` i wykonaj trzecią próbę. Pozostałe parametry powinny pozostać niezmienione.
5. Oblicz różnicę median czasu klatki między wybranymi wariantami, porównaj ich liczbę trójkątów i wskaż, który wariant nie spełnia funkcji części. Nie uznawaj `FAILED` za wynik równy zeru.

| Wariant | Otwór zachowany? | Liczba trójkątów | Mediana [ms] | p95 [ms] | Status |
|---|---|---:|---:|---:|---|
| High | | | | | |
| Medium | | | | | |
| Low | | | | | |

## Doświadczenie 2. Automatyczny LOD i koszt renderowania, około 20 minut

1. Wybierz etap `3.5`, zaznacz `Automatyczny LOD według odległości` i pozostaw `1 materiał`. Ustaw wartości początkowe progów (zwykle ok. **6 m** dla `High → Medium` i **10 m** dla `Medium → Low`); zanotuj rzeczywiste wartości widoczne w panelu.
2. Naciśnij `Rozpocznij próbę`. Kamera wykona kontrolowaną trasę; obserwuj oznaczenie aktualnego `LOD` i stan otworu. Przed rozpoczęciem możesz obejrzeć `Widok kontrolny otworu`, lecz wyłącz go na czas pomiaru.
3. Po zakończeniu sprawdź `lod_transitions`, `lod_level_min`, `lod_level_max` oraz `hole_missing_time_fraction`. Ostatnia wielkość oznacza udział czasu pomiaru, w którym otwór był utracony. Nie myl jej z błędem położenia ani z czasem GPU.
4. Wykonaj drugą próbę przy `4 materiały`, nie zmieniając progów LOD. Porównaj `material_slots_min/max` oraz czasy klatki. Zachowaj ostrożność: sloty materiałów nie stanowią pomiaru `draw calls`.
5. Na etapie `4.0` sformułuj wniosek, czy automatyczna redukcja w tej konfiguracji zapewnia zachowanie funkcji montażowej na całej trasie kamery.

## Doświadczenie 3. Progi LOD i kontrpróba, około 20 minut

1. Wybierz `4.5`, włącz automatyczny LOD i `1 materiał`. Dostępne suwaki ustawiają próg `High → Medium` w zakresie **3–8 m** i `Medium → Low` w zakresie **9–14 m**. Zanotuj wartości bazowe.
2. Wykonaj próbę bazową. Następnie zmień **wyłącznie jeden** próg, zachowując drugi próg, stanowisko i materiały. Powtórz pomiar.
3. Porównaj zmianę `hole_missing_time_fraction`, `lod_transitions` i `cad_frame_median/p95`. Wybór progu uzasadnij jednocześnie kosztem oraz tym, czy ważna cecha pozostaje rozpoznawalna.
4. Dla etapu `5.0` wykonaj kontrpróbę z innym ustawieniem progów lub odległością widoku (`widok`, jeśli jest dostępny w panelu). Zapisz, w jakich warunkach przyjęta strategia przestaje zachowywać otwór.
5. Wyraźnie rozróżnij brak cechy w `Low`, brak pomiaru oraz status `FAILED`. Żadnej z tych sytuacji nie zastępuj zerową wartością błędu.

## Opracowanie wyników, około 15 minut

Zapisz w sekcji `Twój wniosek (jeden na laboratorium)` jedno zwięzłe porównanie odwołujące się do **identyfikatorów własnych prób i rzeczywistych wartości**. Przykładowa struktura: „Wariant … zachował otwór przy … trójkątach oraz medianie … ms. Zmiana progu … zmniejszyła/zwiększyła …, lecz jej ograniczeniem jest …”. Nie wpisuj liczb z demonstracyjnego schematu jako wyników.

| Poziom | Oczekiwany dowód |
|---|---|
| 3.0 | Poprawne rozpoznanie trzech siatek i zachowania otworu |
| 3.5 | Porównanie co najmniej dwóch poprawnych prób czasu klatki i kosztu |
| 4.0 | Interpretacja kompromisu geometrii i zachowania funkcji |
| 4.5 | Dwie próby przed/po zmianie jednego progu LOD |
| 5.0 | Kontrpróba i wskazanie granicy użyteczności przyjętego uproszczenia |

Poziom wybrany w panelu jest etapem pracy, nie oceną wystawianą automatycznie. Ostateczną ocenę zatwierdza prowadzący.

## Przekazanie raportu do formularza WiRR i Supabase

1. Zakończ ostatnią próbę, sprawdź zapisane warunki, wpisz wniosek, a następnie **zatrzymaj Play**. Możesz kliknąć `Eksportuj dwa wyniki bloku`, aby zachować lokalne `WiRR-lab05.json`, `WiRR-lab06.json` i `block.json`. Ten eksport ma oznaczenie `LOCAL_ONLY`: **nie wysyła** raportu.
2. W edytorze Unity wybierz `WiRR → Student → Profil i grupa` i sprawdź numer indeksu oraz G1/G2. Następnie otwórz `WiRR → Raporty → Formularz raportu laboratoryjnego`.
3. W polu `Laboratorium` wybierz **05**. Włącz **krótki raport** (obserwacja, zmiana, wniosek, ograniczenia), jeżeli pełny formularz nie jest wymagany przez prowadzącego. Przenieś **rzeczywiste** warunki, statusy i metryki z panelu sceny albo zapisanego pliku JSON; podaj źródło `DESKTOP_SIM` i fakt wykorzystania dydaktycznych siatek proceduralnych.
4. Sprawdź kompletność przyciskiem `Sprawdź kompletność raportu`. Popraw wskazane braki. Wybierz `Zapisz raport lokalnie (bez logowania)` i zaczekaj na **oddzielne potwierdzenie „WiRR: raport przyjęty”** z identyfikatorem zgłoszenia.
5. Po laboratorium 06 wypełnij i wyślij osobny formularz dla **06**. Dopiero **dwa potwierdzenia serwera** oznaczają przekazanie obu raportów bloku 03; nie są zatwierdzeniem oceny ani dowodem zakończenia późniejszej synchronizacji z GitHub.

**Ważne ograniczenie aktualnej wersji:** opcja `WiRR → Nowe instrukcje → Przenieś wyniki do formularza raportu` automatyzuje obecnie tylko laboratoria **03–04**. Nie używaj jej do sesji bloku 03; dla laboratoriów 05–06 przenieś wyniki ręcznie do istniejącego formularza WiRR. Nie wysyłaj eksportu `wirr-report/2.0-draft.1` zamiast raportu przyjętego przez Supabase. Nie jest wymagane konto GitHub, fork, Git CLI ani osobny e-mail, jeśli potwierdzenie serwera zostało odebrane.

Jeżeli potwierdzenie nie pojawi się, wybierz w formularzu `Wyślij ponownie zapisany raport`. Przy braku łączności zachowaj lokalne pliki i ponów później. W razie trwałej awarii zgłoś problem prowadzącemu; [instrukcja awaryjna bloku 03](WiRR-blok-03-raport-awaryjny.md) oraz [procedura oddawania](../Reference/QUICK_START.md) opisują ostateczną drogę przez ZIP z `lab05.json` i `lab06.json`. Nie przesyłaj całego projektu Unity.

## Diagnostyka stanowiska

Jeżeli nie widać menu WiRR, sprawdź zakończenie importu pakietu w `Window → Package Manager` oraz błędy w `Window → General → Console`. Jeżeli brakuje sceny, nie przerabiaj innego laboratorium; poproś prowadzącego o przygotowanie `Lab05_CADWorkshop`. Jeżeli próba ma status `FAILED` lub zero próbek, sprawdź, czy nie była przerwana, oraz czy zakończył się cały pomiar 2 s + 8 s. Nie zmieniaj rozdzielczości, konfiguracji grafiki ani stanowiska między porównaniami.

Materiał rozszerzony dotyczący importu STEP, tessellacji i produkcyjnego modelu CAD znajduje się w [instrukcji narzędzi CAD](../Reference/LAB5_STARTER_README.md). Są to **dodatkowe ćwiczenia wymagające odpowiedniego stanowiska**; ich wyników nie przypisuj automatycznie scenie `DESKTOP_SIM`.

Materiały kursowe: [KIA-students/wirr](https://github.com/KIA-students/wirr). Wersja instrukcji: 20.10.2026.
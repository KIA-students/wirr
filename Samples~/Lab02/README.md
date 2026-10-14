# Lab 02 — Interakcja i manipulacja XR

## Obowiązkowe laboratorium: jedno doświadczenie (około 35 minut)

**Pytanie:** co sprawia, że użytkownik od razu rozumie, jak przenieść wirtualny przedmiot do celu?

1. Otwórz **WiRR → Misje i odkrycia → Start: Lab 02**. Wykorzystaj scenę i elementy interakcji, które są już przygotowane na stanowisku.
2. Wskaż obiekt do przeniesienia, cel oraz sygnał potwierdzający udaną akcję. Przenieś obiekt **trzy razy**. Zapisz liczbę pomyłek lub czas każdej próby, jeśli pomiar jest dostępny.
3. Zmień **jeden** element informacji zwrotnej, np. kolor celu albo podświetlenie poprawnego miejsca.
4. Powtórz trzy próby. Porównaj, czy po zmianie łatwiej rozpoznać prawidłową akcję.
5. Napisz jeden wniosek o użyteczności i jedną propozycję ulepszenia.

**Co oddać:** jeden raport **Lab 02** z indeksem, grupą, krótkim opisem zmiany, wynikami **3 prób przed i 3 po zmianie**, jeśli można je zaobserwować lub zmierzyć, oraz jednym wnioskiem. Raport eksportuj lokalnie i prześlij kanałem wskazanym przez prowadzącego. Nie wysyłaj całego projektu Unity.

**Zaliczenie podstawowe:** działająca albo przedstawiona w scenie demonstracyjnej interakcja, porównanie przed i po zmianie oraz uzasadniony wniosek. Konfiguracja XRI, Quest 3, teleportacja i SSQ nie są obowiązkowe w tej ścieżce.

**Gdy nie działa XRI lub nie ma gogli:** wykonaj ćwiczenie na demonstratorze PC dostępnym na stanowisku. Jeżeli demonstrator nie pozwala na interakcję, opisz projekt i wykonaj obserwację udostępnionego pokazu, wyraźnie zaznaczając brak własnych pomiarów. Sposób zaliczenia potwierdza prowadzący.

Dalsze sekcje dokumentują rozbudowane warianty techniczne. **Nie są obowiązkową listą zadań laboratoryjnych.**


## Spis treści

- [Szybki wybór ścieżki](#szybki-wybór-ścieżki)
- [Przygotowanie](#przygotowanie)
- [Zadania krok po kroku](#zadania-krok-po-kroku)
  - [3.0: Chwyt, socket i ray](#30-chwyt-socket-i-ray)
  - [3.5: UI i feedback](#35-ui-i-feedback)
  - [4.0: Lokomocja](#40-lokomocja)
  - [4.5: Scenariusz i diagnostyka](#45-scenariusz-i-diagnostyka)
  - [5.0: Quest 3 i SSQ](#50-quest-3-i-ssq)
- [Zakończenie laboratorium](#zakończenie-laboratorium)
- [Materiały dydaktyczne](#materiały-dydaktyczne)
- [Dodatkowe, dobrowolne doświadczenie: komfort VR i głębia](#dodatkowe-dobrowolne-doświadczenie-komfort-vr-i-głębia)
- [Wyzwanie dodatkowe: operator potrafi bezpiecznie przerwać](#wyzwanie-dodatkowe-operator-potrafi-bezpiecznie-przerwać)

## Szybki wybór ścieżki
- **Nie mam gotowej sceny Lab 01:** [wróć do pierwszej sceny](../Lab01/README.md#pierwsze-10-minut-eksperyment-zamiast-instalatora).
- **Mam działający projekt:** [od razu przejdź do interakcji](#zadania-krok-po-kroku).
- **Brakuje XRI:** zainstaluj XR Interaction Toolkit i wymagane próbki, zobacz [warunki startu](../../Documentation~/Reference/OS_SETUP.md#pakiety-xr-tylko-gdy-potrzebne).
- **Chcę przejść do rzeczywistego sprzętu:** najpierw [uruchomienie urządzeń](../../Documentation~/Reference/OS_SETUP.md#urzadzenia-xr), potem [Quest 3 i SSQ](#50-quest-3-i-ssq).
- **Chcę poznać ograniczenia percepcji:** [Komfort XR](../../Documentation~/Reference/COMFORT_STUDIES.md).


Ten folder jest próbką pakietu **WiRR Course Toolkit** przeznaczoną dla Laboratorium 2. Poniższa lista prowadzi przez wykonanie ćwiczenia w tej samej kolejności, którą pokazuje panel `WiRR → Misje i odkrycia`. Instrukcja laboratoryjna rozwija teorię i sposób interpretacji, ale kolejność pracy i wymagane dowody są zebrane tutaj.

## Przygotowanie

1. Po imporcie pakietu poczekaj na zakończenie kompilacji. W **Profilu WiRR** wpisz numer indeksu, wybierz grupę z planu i kliknij **Zapisz profil i przypisz grupę**. Dla grupy 2 wybierz **G2**. Profil można też otworzyć przez **WiRR → Student → Profil i grupa**.
2. W **WiRR → Misje i odkrycia** wybierz blok I i **Start: Lab 02**. Ten przycisk przygotowuje scenę PC; nie oznacza ukończenia konfiguracji XRI.
3. Do obowiązkowego chwytu, gniazda i promienia zainstaluj zależności XRI oraz zaimportuj **Starter Assets**, **XR Interaction Simulator** i próbkę WiRR Lab 02. Użyj panelu zaawansowanego, potem **Sprawdź konfigurację laboratorium**.
4. Zachowaj działającą scenę Lab 01. W Lab 02 zapisz scenę interakcji w `Assets/WiRR/Lab02/Scenes`. Narzędzie tworzy `Lab02.unity`; własną kopię możesz zapisać jako `Lab02_Interaction.unity`. Zweryfikuj jeden XR Origin, jedną kamerę i XR Interaction Manager.
5. Kliknij **Otwórz raport: Lab 02**. Sprawdź numer laboratorium, indeks, grupę i wyliczone warianty v1–v5. Przygotuj stanowisko z obiektem chwytalnym, gniazdem, promieniem, UI i obszarem teleportacji.

## Zadania krok po kroku

### 3.0: Chwyt, socket i ray

1. Skonfiguruj XR Grab Interactable na obiekcie chwytalnym.
2. Dodaj XR Socket Interactor i ogranicz go do właściwego obiektu/warstwy.
3. Skonfiguruj XR Ray Interactor i potwierdź niezależne działanie chwytu, socketu i ray.
4. Wykonaj po pięć prób w każdym warunku własnego wariantu (A/B, a C tylko gdy występuje w tabeli instrukcji); zapisuj czas i liczbę błędów.

Po zakończeniu tego punktu uzupełnij odpowiadającą mu sekcję formularza **WiRR Reports**. Nie przechodź do kolejnego eksperymentu, dopóki nie zapiszesz warunków pomiaru i wyniku.

### 3.5: UI i feedback

1. Utwórz panel World Space i skonfiguruj obsługę UI dla XR.
2. Dodaj co najmniej dwa rodzaje feedbacku: wizualny, dźwiękowy lub haptyczny.
3. Sprawdź stany `Hover`/`Select` oraz zakończenie interakcji; usuń wielokrotne aktywacje jednego działania.
4. Wykonaj po pięć prób w każdym warunku własnego wariantu (A/B, a C tylko gdy występuje w tabeli instrukcji) i zapisz czas oraz błędne aktywacje.

Po zakończeniu tego punktu uzupełnij odpowiadającą mu sekcję formularza **WiRR Reports**. Nie przechodź do kolejnego eksperymentu, dopóki nie zapiszesz warunków pomiaru i wyniku.

### 4.0: Lokomocja

1. Skonfiguruj teleportację wyłącznie na dozwolone powierzchnie.
2. Sprawdź reticle, orientację po teleportacji i kolizję użytkownika z otoczeniem.
3. Wykonaj po pięć prób w każdym warunku własnego wariantu (A/B, a C tylko gdy występuje w tabeli instrukcji), zapisując czas oraz błędy/korekty.
4. Zapisz, które elementy konfiguracji najbardziej wpływają na przewidywalność i komfort.

Po zakończeniu tego punktu uzupełnij odpowiadającą mu sekcję formularza **WiRR Reports**. Nie przechodź do kolejnego eksperymentu, dopóki nie zapiszesz warunków pomiaru i wyniku.

### 4.5: Scenariusz i diagnostyka

1. Połącz lokomocję, wskazanie, chwyt, socket i UI w jeden scenariusz.
2. Wykonaj pięć prób i rejestruj czas, upuszczenia, błędne aktywacje i nieudane teleportacje.
3. Wprowadź kontrolowany problem v4, zapisz H1/H2 i wykonaj test rozstrzygający.
4. Zastosuj minimalną poprawkę i powtórz scenariusz, sprawdzając regresje.

Po zakończeniu tego punktu uzupełnij odpowiadającą mu sekcję formularza **WiRR Reports**. Nie przechodź do kolejnego eksperymentu, dopóki nie zapiszesz warunków pomiaru i wyniku.

### 5.0: Quest 3 i SSQ

1. Wypełnij własny SSQ PRE przed ekspozycją.
2. Uruchom scenariusz samodzielnej aplikacji na Quest 3 i zweryfikuj interakcję oraz lokomocję.
3. Wykonaj dla własnego wariantu v5 po trzy próby w każdym wymaganym warunku, zapisując czas, błędy i komfort 0–10.
4. Wypełnij SSQ POST i sformułuj wniosek ograniczony do tej sesji.

Po zakończeniu tego punktu uzupełnij odpowiadającą mu sekcję formularza **WiRR Reports**. Nie przechodź do kolejnego eksperymentu, dopóki nie zapiszesz warunków pomiaru i wyniku.

## Zakończenie laboratorium

1. Otwórz **WiRR → Raporty → Formularz raportu laboratoryjnego**, wybierz **Lab 02** i sprawdź najwyższy kompletny punkt kontrolny.
2. Sprawdź, czy każda tabela zawiera warunki pomiaru, wyniki surowe i krótki wniosek.
3. Użyj **Sprawdź kompletność raportu** i popraw wskazane braki.
4. Kliknij **Eksportuj raport lokalnie** dopiero po przejściu walidacji. Otworzy się folder z JSON w `Library/WiRRReports/exports`. Przekaż ten plik kanałem wskazanym przez prowadzącego. Eksport nie oznacza oddania pracy. Checkboxy z panelu zadań są lokalną pomocą i nie są częścią oceny ani raportu.

## Materiały dydaktyczne
Po przygotowaniu sceny możesz użyć sekcji **Scena i pomiary → Materiały dydaktyczne**:

1. **Dodaj środowisko** — tworzy wspólne stanowisko WiRR; w Lab 03–04 zamiast wirtualnego pokoju używany jest lekki zestaw odniesienia AR.
2. **Dodaj zestaw eksperymentalny** — tworzy prefaby właściwe dla bieżącego laboratorium w `Assets/WiRR/Lab02/Prefabs/Generated` i umieszcza ich instancje pod `WiRR_TeachingAssets`.
3. **Usuń obiekty dydaktyczne ze sceny** — usuwa wyłącznie gałąź `WiRR_TeachingAssets`; nie usuwa pracy studenta ani wygenerowanych prefabów.

Materiały są opcjonalne i służą jako kontekst eksperymentu. Nie konfigurują za studenta komponentów stanowiących cel ćwiczenia. Folder `Prefabs/Generated` jest odtwarzalny — własne rozwiązania zapisuj poza nim.

`report-template.md` służy do awaryjnego raportu Markdown. Wypełnij formularz Unity albo szablon, zgodnie z kanałem wskazanym przez prowadzącego. Nie przepisuj wyników do obu formatów.

**Postęp:** panel pokazuje zapisaną scenę i lokalny eksport. Stan jest aktualizowany po eksporcie i co około minutę. Opcjonalne menu **WiRR → Student → Włącz automatyczne wysyłanie na GitHub** synchronizuje techniczny postęp, a nie pełny raport ani ocenę.


## Dodatkowe, dobrowolne doświadczenie: komfort VR i głębia

Wybierz **WiRR → Komfort XR → Utwórz 3 prefaby: obserwacja i watchdog**. Powstaną prawdziwe prefaby Unity z łagodnym ruchem wskazówek wizualnych (maks. 8 s) i krótkim porównaniem wirtualnej głębi (maks. 10 s). Ich zadaniem nie jest wywoływanie nudności, lecz nauka projektowania komfortowych scen.

Instrukcja pełna: **Documentation~/Reference/COMFORT_STUDIES.md**. Przy pracy w goglach potrzebny jest świadomy ochotnik, osoba obserwująca i możliwość natychmiastowego przerwania. Reszta grupy może wykonać równoważną analizę na monitorze. **Nie przyznaje się punktów za fizjologiczną odporność ani wystąpienie objawów.**

## Wyzwanie dodatkowe: operator potrafi bezpiecznie przerwać

Po zrealizowaniu podstawowego doświadczenia sprawdź [automatyczną detekcję zdjęcia gogli, utraty śledzenia i skoku danych](../../Documentation~/Reference/XR_SAFETY_WATCHDOG.md). Możesz wykonać do 10 krótkich prób technicznych, także wyłącznie na PC. **Nie wykonuj nagłych ruchów głową dla punktów.** Punktuje się poprawność prototypu i interpretacji, a nie podatność na objawy.


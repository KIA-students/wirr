# WiRR: rozpoczęcie i oddanie pracy

## Uruchomienie krok po kroku

1. Uruchom **Unity Hub**. W zakładce **Installs** sprawdź, czy dostępny jest Unity **6000.6.5f1**. Jeżeli nie, wybierz **Install Editor** i doinstaluj tę wersję.
2. W **Projects** otwórz projekt przygotowany na stanowisku. Na własnym komputerze wybierz **New project → Universal 3D (URP) → Create project**.
3. Jeżeli pakiet WiRR nie jest jeszcze zainstalowany, w edytorze wybierz **Window → Package Manager**, naciśnij **+ → Install package from git URL**, wklej `https://github.com/KIA-students/wirr.git#upm` i wybierz **Install**. Jeśli Package Manager zgłasza błąd Git, sprawdź `git --version` i komunikat w Console.
4. Otwórz **WiRR → Misje i odkrycia (4 bloki) → Start: Lab 01**. Jeśli pakiet nie zadziała, użyj pustej sceny URP, utwórz sześcian przez **GameObject → 3D Object → Cube**, naciśnij **Play**, a następnie zakończ i zapisz scenę. Problem zgłoś prowadzącemu bez wielokrotnego importu pakietu.
5. Wybierz właściwy [blok laboratoryjny](../Campaign/README.md), wykonaj eksperyment i przekaż dwa oddzielne raporty, zgodnie z dalszymi krokami.

Nie instaluj Android SDK, ROS 2, Gazebo ani pakietów XR do zwykłego uruchomienia pierwszej sceny na PC.

## Przekazywanie raportów

1. Otwórz przygotowany projekt Unity 6000.6.5f1 i bieżący blok. Pracuj solo lub w parze. W scenach SIM_AR Lab03–04 po zakończeniu Play [przenieś zapisaną sesję do formularza](../WiRR2/SUPABASE-REPORT-BRIDGE.md). W Lab05–06 (blok 03) adapter automatycznego importu nie jest jeszcze dostępny: [przenieś rzeczywiste wyniki ręcznie do istniejącego formularza](../WiRR2/SUPABASE-REPORT-BRIDGE.md#laboratoria-05-i-06-przekazanie-wyników-bez-adaptera). Lokalny eksport dwóch wyników sam nie oddaje pracy.
2. Wpisz indeksy i grupę G1/G2 raz na blok.
3. Wykonaj pierwsze laboratorium. Otwórz **WiRR > Raporty > Formularz raportu laboratoryjnego**, sprawdź numer labu i kompletność, kliknij **Zapisz raport lokalnie (bez logowania)**. Unity zapisze JSON i rozpocznie wysyłkę.
4. W tym samym projekcie wykonaj drugie laboratorium i zapisz jego raport tym samym przyciskiem.
5. Na koniec bloku sprawdź osobne potwierdzenie przyjęcia przez serwer dla obu laboratoriów. System automatycznie przekazuje zgłoszenia do **KIA-students/wirr**. Przy poprawnej wysyłce niczego nie pakuj ani nie wysyłaj e-mailem. W parze wystarczą dwa raporty z oboma indeksami.

Sam zapis lokalny nie oznacza oddania. Potwierdzenie serwera oznacza przyjęcie do kolejki; automatyczne utworzenie zgłoszenia w repozytorium następuje osobno. Przyjęcie raportu nie oznacza zatwierdzenia oceny. Nie potrzebujesz Git, GitHub CLI ani własnego forka.

**Brak potwierdzenia:** zachowaj pliki i kliknij **Wyślij ponownie zapisany raport**. Bez Internetu ponów po odzyskaniu połączenia. Jeśli nadal się nie udaje, zgłoś problem prowadzącemu i użyj awaryjnego e-maila opisanego w karcie bloku: jeden ZIP z dwoma JSON na **m.pomianek@prz.edu.pl**, temat **WiRR blok NN, grupa, indeksy**. ZIP: `WiRR-blok-NN-G1-INDEKS.zip`, np. `lab01.json` i `lab02.json` dla bloku 01. Wstaw własną grupę i indeksy; w parze wysyła jedna osoba.

**Gdy także eksport JSON nie działa:** uzupełnij podlinkowany w karcie bloku szablon dla obu laboratoriów i wyślij go zamiast ZIP. Zachowaj kopię. Nie dołączaj projektu Unity, slajdów ani dodatkowej karty sesji.

[Wybierz jeden z czterech bloków](../Campaign/README.md)

WiRR 2.0 pozostaje wersją redakcyjną do odbioru scen, pomiarów i pełnego przepływu raportów.

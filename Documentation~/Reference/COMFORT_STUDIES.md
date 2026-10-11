# Komfort XR: choroba symulatorowa, wergencja i akomodacja

## Po co wykonujemy te eksperymenty?
VR angażuje jednocześnie widzenie, zmysł równowagi i kontrolę ruchów oczu. Informacje z tych układów mogą być niespójne. Cybersickness może obejmować nudności, zawroty głowy i dezorientację. **Wergencja** określa zbieżność osi widzenia obu oczu na obiekcie, natomiast **akomodacja** to ustawienie ostrości przez soczewkę oka. W tradycyjnym wyświetlaczu stereoskopowym obraz może pozornie znajdować się na różnych głębokościach, mimo że optyczna odległość jego ogniskowania pozostaje stała.

**Celem jest dostrzeganie wskazówek percepcyjnych i projektowanie komfortowych rozwiązań, a nie wywołanie nudności, bólu głowy ani dwojenia widzenia.**

## Gotowe programy i prefaby

W Unity wybierz **WiRR → Komfort XR → Utwórz 2 prefaby i stanowiska obserwacyjne**. Generator tworzy w folderze Assets/WiRR/Common/Prefabs/ComfortStudy:

- **WiRR_Comfort_OpticFlow.prefab**, z komponentem WiRRVisualMotionStudy. Umożliwia jeden łagodny, ograniczony do 8 s ruch **pasków referencyjnych**, a nie kamery XR.
- **WiRR_Comfort_DepthCues.prefab**, z komponentem WiRRDepthCueStudy. Umożliwia jedno płynne porównanie wirtualnych odległości obiektu, ograniczone do 10 s, **bez zmiany IPD, projekcji kamery i optyki gogli**.

Prefaby zawierają prawdziwe obiekty Unity i programy C#. Generowane są lokalnie jednym poleceniem bez ARCore, ROS 2, kont Unity i pobierania dodatkowych zasobów. Żaden program nie startuje automatycznie ani nie zapętla ekspozycji.

### Uruchomienie i przerwanie
1. Rozpocznij **Play** na monitorze. Zapoznaj się z obiektem nieruchomym i ruchomym.
2. Wybierz dobrowolną krótką demonstrację przyciskiem w **Game View** albo klawiszem **Space**. Klawisz **Esc** natychmiast przerywa; kontroler Gamepad może używać A do startu i B do przerwania.
3. Na goglach kontrolery XR nie muszą być rozpoznawane jako zwykły Gamepad. Operator może użyć przycisku na monitorze albo menu kontekstowego komponentu **Rozpocznij / Przerwij**. Metody publiczne Begin i StopStudy można opcjonalnie powiązać z akcjami XRI w innej scenie.
4. Wyjście z Play Mode też zatrzymuje animację. Po zakończeniu pojedynczej próby nie powtarzaj jej w poszukiwaniu silniejszych objawów.

## Warunki uczestnictwa
- Uczestnik dobrowolnie wyraża zgodę, siedzi w bezpiecznym miejscu i może przerwać w każdej chwili. Druga osoba obserwuje i pomaga bez dodatkowej ekspozycji.
- **Nie wybieramy studenta „najbardziej odpornego” na chorobę symulatorową.** Grupa może wybrać ochotnika prezentującego, operatora Stop i obserwatora. Pozostali wykonują równoważne zadanie przy monitorze.
- Jeśli uczestnik poczuje dyskomfort, próbę należy niezwłocznie przerwać. Nie wolno wydłużać jej po wystąpieniu objawów ani porównywać „wytrzymałości” drużyn.
- Wyników dotyczących objawów i zdrowia nie publikujemy w GitHub ani w Lidze XR. Opcjonalny, prywatny SSQ służy dydaktycznej refleksji, a nie ocenie lub rywalizacji.

## Eksperyment A: ruch obrazu i wskazówki ruchu własnego

1. Najpierw popatrz przez 10–15 s na **nieruchomą** scenę z pionowymi paskami oraz znakiem referencyjnym.
2. Uruchom jedną krótką, wolną translację pasków. Sprawdź, czy nieruchomy znak pozostaje nieruchomy i czy kamera nie jest przesuwana.
3. W trybie na monitorze lub przy dobrowolnym pokazie na goglach odpowiedz: „Czy rozpoznaję ruch pasków?” i „Czy mam wrażenie ruchu całego otoczenia?” (tak/nie/nie wiem).
4. Nie oczekuj nudności. Porównaj mechanizmy **ruchu obiektu**, **ruchu kamery** i **ruchu własnej głowy** na rysunku.
5. Zaproponuj funkcję poprawiającą komfort: stabilny horyzont, teleportację lub wyraźny nieruchomy punkt odniesienia.

**Ograniczenie:** bardzo łagodny ruch pasków nie jest kompletną symulacją konfliktu wzrokowo-przedsionkowego. Wyczuwalne vection i choroba symulatorowa nie są tożsame.

## Eksperyment B: wirtualna głębia i stała odległość optyczna

1. Otwórz scenę z nieruchomym sześcianem i kulą poruszającą się powoli między dwiema bezpiecznymi odległościami wirtualnymi.
2. Porównaj widok na monitorze **2D** i w zgodnym urządzeniu stereo, o ile uczestnik chce je założyć.
3. Zapisz, które wskazówki głębi działają w obu wariantach: perspektywa, wielkość kątowa, zasłanianie. Która z nich wynika z różnicy obrazu pomiędzy lewym i prawym okiem?
4. Wyjaśnij na schemacie, dlaczego para kamer stereo o stałym rozstawie nie zmienia odległości ogniskowania fizycznego wyświetlacza. **Nie twierdź, że Unity mierzy akomodację oczu**.
5. W wariancie VITURE sprawdź najpierw poprawny tryb 2D i Full SBS. Nie zwiększaj samowolnie rozstawu kamer lub ekstremalnej dysparycji w celu wywołania dwojenia obrazu.

## Raport: jedno odkrycie zamiast testu odporności

Zapisz wariant PC/QUEST/VITURE, warunki sceny i wynik testu przycisku Stop. Dodaj **jedną zaobserwowaną różnicę wskazówek percepcyjnych** oraz **jedną propozycję poprawy dostępności/komfortu**. Nie wpisuj niezmierzonej odległości akomodacji, diagnozy medycznej ani wymyślonych wyników SSQ.

Misja dodatkowa Ligi XR **xr-optics** premiuje poprawne wyjaśnienie zjawiska i projekt poprawy, nigdy nasilenie objawów.

### Literatura
- Hoffman i in. (2008), Vergence–accommodation conflicts hinder visual performance and cause visual fatigue: https://pmc.ncbi.nlm.nih.gov/articles/PMC2879326/
- Shibata i in. (2011), The zone of comfort: https://pmc.ncbi.nlm.nih.gov/articles/PMC3369815/
- Kramida (2016), Resolving the Vergence–Accommodation Conflict in HMD: https://pubmed.ncbi.nlm.nih.gov/26336129/
- Kennedy i in. (1993), Simulator Sickness Questionnaire: https://doi.org/10.1207/s15327108ijap0303_3

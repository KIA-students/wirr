# Formaty modeli 3D i zasady konwersji

Ten dokument uzupełnia Laboratorium 05. Najważniejsza zasada: format pliku nie jest tylko rozszerzeniem. Każdy format opisuje inny rodzaj informacji, dlatego konwersja może być stratna nawet wtedy, gdy po konwersji model wygląda podobnie.

## 1. Najpierw rozpoznaj rodzaj danych

| Format | Główna rola | Najważniejsza informacja |
|---|---|---|
| URDF | opis robota w ekosystemie ROS | linki, przeguby, osie, limity, układy współrzędnych, masa, bezwładność, osobne geometrie visual i collision oraz odwołania do plików mesh |
| MJCF | model robota lub układu dynamicznego dla MuJoCo | struktura ciał i przegubów oraz dane symulacyjne, m.in. contact, actuator, tendon, sensor i ustawienia solvera |
| USD, USDA, USDC, USDZ | opis i kompozycja sceny | hierarchia primów, warstwy, referencje, warianty, instancje, geometria, materiały, animacja i opcjonalne schematy fizyki |
| STEP, STP | neutralna wymiana danych CAD | dokładna geometria B-Rep, powierzchnie analityczne/NURBS, bryły, a zależnie od profilu także struktura produktu i metadane |
| STL | prosta siatka trójkątów | geometria powierzchni; brak standardowej hierarchii, UV, materiałów, rigu i animacji |
| OBJ + MTL | statyczna siatka DCC | pozycje, polygon mesh, UV, normalne, grupy oraz podstawowe materiały przez MTL i osobne tekstury |
| FBX | bogata wymiana DCC i silnik czasu rzeczywistego | mesh, hierarchia, transformacje, UV, normalne, materiały, rig, skinning i animacja |

URDF i MJCF nie są odpowiednikami OBJ lub FBX. Plik URDF może wskazywać na wiele plików STL, OBJ albo DAE. Podobnie opis MJCF może odwoływać się do osobnych zasobów mesh. Sam plik opisu robota zawiera semantykę i zależności, a nie musi zawierać całej geometrii.

USD także nie jest po prostu kolejnym formatem mesha. Może złożyć scenę z wielu warstw i referencji. Spłaszczenie USD do pojedynczego FBX lub OBJ może usunąć warianty, referencje, część materiałów, schematów fizyki i metadanych.

STEP stoi po drugiej stronie pipeline'u. Jest właściwym źródłem geometrii inżynierskiej. Przejście STEP do FBX, OBJ lub STL wymaga tessellacji, czyli zamiany dokładnych powierzchni na skończoną siatkę.

## 2. Co Unity importuje bezpośrednio

Standardowy Model Importer Unity 6 obsługuje przede wszystkim FBX, DAE, DXF i OBJ. Unity używa FBX wewnętrznie w łańcuchu importu i rekomenduje FBX jako przewidywalny format wymiany z narzędziami DCC.

STEP i STL nie są na tej liście standardowych formatów Model Importera. Przed użyciem w typowym projekcie Unity należy je przekonwertować albo użyć specjalizowanego importera. USD również wymaga zgodnego pakietu lub importera USD. URDF i MJCF wymagają narzędzia robotycznego albo wcześniejszego przekształcenia struktury robota.

Dokumentacja Unity:
https://docs.unity3d.com/6000.0/Manual/3D-formats.html

## 3. Zalecane ścieżki konwersji

| Źródło | Cel | Zalecana ścieżka | Co trzeba sprawdzić |
|---|---|---|---|
| STEP | FBX lub OBJ | FreeCAD, narzędzie CAD, Pixyz/Asset Transformer albo inny tessellator, następnie ewentualnie Blender | tolerancję tessellacji, jednostki, osie, nazwy części i podział materiałów |
| STEP | STL | tessellacja w CAD | utratę hierarchii, materiałów, UV i semantyki CAD |
| STEP | URDF lub MJCF | najpierw osobne części CAD do meshy, potem jawne zbudowanie linków, jointów, visual/collision i inertial | nie traktować tego jako automatycznej zmiany rozszerzenia |
| STL | OBJ lub FBX | Blender albo konwerter mesh | konwersja nie odzyskuje UV, materiałów, historii CAD ani utraconej hierarchii |
| OBJ | FBX | Blender lub DCC | MTL, tekstury, normalne, UV, skala i pivot |
| FBX | OBJ | Blender lub DCC | rig, skinning, animacja i część semantyki sceny nie przejdą do OBJ |
| URDF | MJCF | URDF-Studio, MuJoCo lub narzędzie robotyczne | osie i limity jointów, ramy, inertial, collision, mimic/transmission oraz elementy bez odpowiednika w MJCF |
| MJCF | URDF | narzędzie robotyczne lub URDF-Studio | aktory, sensory, tendony, kontakty i ustawienia MuJoCo mogą nie mieć odpowiednika w URDF |
| URDF lub MJCF | USD | URDF-Studio, Isaac Sim lub inny importer robotyczny | artykulację, masy, bezwładności, collidery, joint drives i nazwy linków |
| USD | FBX lub OBJ | DCC lub konwerter po świadomym spłaszczeniu sceny | warstwy, referencje, warianty, instancing i schematy fizyki |
| FBX lub OBJ | URDF lub MJCF | użyj mesha jako visual/collision i zbuduj opis robota | sam mesh nie zawiera poprawnej kinematyki ani dynamiki |

### Przykład internetowego konwertera

Aktualna dokumentacja Convert3D wymienia w tabeli typowych konwersji m.in. STEP/STP do GLB, STL, OBJ, DXF i DWG, STL do GLB/OBJ/FBX oraz konwersje pomiędzy OBJ i FBX. Sam fakt, że dany format występuje równocześnie na liście importu i eksportu, nie oznacza, że każda para wejście–wyjście jest równie dobrze obsługiwana. Dla STEP do FBX bezpieczniej przyjąć jawny pipeline STEP → OBJ/GLB → DCC → FBX albo użyć narzędzia CAD/Pixyz, jeśli zależy nam na kontrolowanej tessellacji.

Convert3D jest wygodny do ćwiczeń i szybkiej diagnostyki, ale wynik trzeba zweryfikować w narzędziu docelowym.

Lista formatów:
https://convert3d.org/supported-apps/unity
https://docs.convert3d.org/docs/formats

W przypadku danych firmowych, niepublicznych lub objętych umową zawsze stosuj zasady organizacji dotyczące przetwarzania plików poza zatwierdzonym środowiskiem. Nie wybieraj narzędzia chmurowego wyłącznie dlatego, że technicznie obsługuje dany format.

## 4. Przykład Unitree G1: geometria nie jest opisem robota

Repozytorium Unitree ROS zawiera dla G1 modele URDF, które odwołują się do osobnych plików STL w katalogu `meshes`. W samym URDF znajdują się m.in. linki, jointy, limity, geometrie visual/collision oraz dane inertial:
https://github.com/unitreerobotics/unitree_ros/tree/master/robots/g1_description
https://github.com/unitreerobotics/unitree_ros/blob/master/robots/g1_description/g1_29dof.urdf

To ważny przykład praktyczny: plik STL opisuje tylko geometrię powierzchni, natomiast URDF nadaje tym zasobom strukturę robota i znaczenie kinematyczne/dynamiczne.

W plikach URDF link może zawierać osobno:
- inertial: masa, położenie środka masy i tensor bezwładności;
- visual: geometrię przeznaczoną do wyświetlania;
- collision: geometrię używaną przez kolizje;
- joint: parent, child, origin, axis oraz limity ruchu.

Przykładowy G1 odwołuje się z URDF do plików meshes/*.STL. To dobry dowód, że STL jest tylko zasobem geometrycznym, a URDF niesie strukturę i semantykę robota.

URDF Studio:
https://github.com/OpenLegged/URDF-Studio

Projekt może otwierać i eksportować m.in. URDF, MJCF, USD, SDF i Xacro oraz pozwala analizować topologię, visual/collision i parametry robota. Traktuj go jako narzędzie do jawnego mapowania reprezentacji, nie jako gwarancję bezstratnego round-trip. Przy konwersji zawsze sprawdź ramy odniesienia, osie jointów, limity, masy, bezwładności, collidery i elementy specyficzne dla danego symulatora.

## 5. Zasady, których trzeba przestrzegać

1. Zachowaj źródło prawdy. Nigdy nie nadpisuj oryginalnego STEP, URDF, MJCF ani USD plikiem po konwersji.
2. Zapisuj cały łańcuch. W raporcie podaj program, wersję, format wejściowy, format wyjściowy i kluczowe ustawienia każdego etapu.
3. Ustal jednostki przed konwersją. STEP może jawnie nieść jednostki, STL często wymaga ich założenia, a Unity interpretuje 1 jednostkę świata jako 1 metr.
4. Ustal układy osi. Sprawdź handedness, oś pionową, kierunek forward oraz konwersję lokalnych układów linków.
5. Nie naprawiaj skali tylko Transformem w Unity. Przy modelu docelowym dąż do poprawnego importu i root localScale = (1,1,1).
6. Kontroluj tessellację. Zapisz tolerancję liniową/chordal i kątową. Zbyt luźna tessellacja niszczy kształt, zbyt gęsta zwiększa koszt bez widocznej korzyści.
7. Tesselluj możliwie raz. Wielokrotne przejścia CAD do mesh i kolejne re-eksporty zwiększają ryzyko błędów i utrudniają reprodukcję.
8. Zachowuj nazwy, hierarchię, pivoty i lokalne transformacje. Są często ważniejsze niż sam wygląd.
9. Rozdziel visual i collision. Model kolizji powinien zachowywać funkcję, nie pełną szczegółowość wizualną.
10. W robotyce weryfikuj jointy. Sprawdź parent/child, origin, axis, limity, mimic/transmission, masy, środki masy i tensor bezwładności.
11. Sprawdź atrybuty mesha. Po każdej konwersji skontroluj normalne, tangenty, UV, materiały, tekstury, liczbę submeshy i orientację ścian.
12. Traktuj pliki towarzyszące jako część assetu. OBJ może wymagać MTL i tekstur, glTF może wskazywać BIN i obrazy, URDF może wskazywać wiele osobnych meshy.
13. Nie myl konwersji z rekonstrukcją. STL, OBJ lub FBX do STEP nie odtworzy automatycznie pierwotnych NURBS, więzów, cech parametrycznych ani intencji konstrukcyjnej.
14. Waliduj liczbowo. Porównaj co najmniej dwa wymiary, bounding box, liczbę części, wierzchołków, trójkątów, materiałów i colliderów.
15. Waliduj funkcjonalnie. Dla robota uruchom zakresy jointów i sprawdź położenia zerowe; dla XR sprawdź sylwetkę, skalę, kolizje i LOD.
16. Porównuj w niezależnym viewerze. Jeżeli to możliwe, otwórz wynik poza narzędziem, które wykonało konwersję.
17. Zachowaj pochodzenie i licencję. Konwersja formatu nie zmienia praw autorskich ani licencji modelu.
18. Nie wysyłaj do zewnętrznego konwertera modelu zastrzeżonego bez uprawnienia, nawet jeśli narzędzie deklaruje prywatność lub pracę lokalną w przeglądarce.

## 6. Model referencyjny Lab 05

Obowiązkowy model znajduje się w:
Samples~/Lab05/Models/Source/makerbeam_bracket_90degree.stp

Jest to wspornik MakerBeam zapisany jako STEP AP214 z dokładną reprezentacją B-Rep. Zawiera m.in. płaszczyzny, powierzchnie cylindryczne i krzywe, dlatego nadaje się do obserwacji wpływu parametrów tessellacji.

Po przygotowaniu workspace Lab 05 WiRR kopiuje plik do:
Assets/WiRR/Lab05/Models/Source/makerbeam_bracket_90degree.stp

Model pochodzi z FreeCAD Parts Library. Autor: Benjamin Aigner. Licencja zasobu: CC-BY-3.0. Szczegóły znajdują się w pliku ATTRIBUTION.md obok modelu.

## 7. Minimalna walidacja po każdej konwersji

Przed przejściem dalej odpowiedz:
- czy wymiary są zgodne;
- czy osie i orientacja są zgodne;
- czy liczba części i hierarchia są zgodne z oczekiwaniem;
- czy visual i collision nie zostały przypadkowo zamienione lub złączone;
- czy materiały i tekstury są kompletne;
- czy geometria nie ma nowych dziur, odwróconych normalnych lub degeneracji;
- czy w przypadku robota działają jointy, limity, masa i bezwładność;
- jakie informacje zostały utracone i czy ich utrata jest akceptowalna dla celu XR.

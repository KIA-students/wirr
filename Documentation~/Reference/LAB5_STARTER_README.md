# Laboratorium 5: optymalizacja modeli CAD

## Co otrzymujesz z pakietu

Po przygotowaniu workspace Lab 05 pakiet WiRR kopiuje do projektu:

- **Assets/WiRR/Lab05/Models/Source/makerbeam_bracket_90degree.stp**: obowiązkowy model referencyjny STEP;
- **Assets/WiRR/Lab05/Models/Source/ATTRIBUTION.md**: autor i licencja modelu;
- **Assets/WiRR/Lab05/Documentation/MODEL_FORMATS_AND_CONVERSION.md**: przewodnik po URDF, MJCF, USD, STEP, STL, OBJ i FBX.

Model pochodzi z FreeCAD Parts Library. Autor: Benjamin Aigner. Licencja zasobu: CC-BY-3.0.

## Narzędzia

- Unity 6000.6.x z pakietem WiRR;
- Blender 4.x albo inne DCC do pracy z meshem;
- FreeCAD, inne narzędzie CAD lub konwerter obsługujący STEP do tessellacji;
- opcjonalnie URDF Studio do porównania reprezentacji URDF, MJCF i USD;
- opcjonalnie Convert3D do ćwiczeń z konwersją formatów, jeśli polityka dotycząca danych pozwala użyć takiego narzędzia.

## Zanim rozpoczniesz

1. Wybierz Lab 05 w **WiRR → Narzędzia kursu**.
2. Użyj **Zainstaluj / napraw zależności**.
3. Zaimportuj próbkę WiRR.
4. Użyj **Utwórz / napraw aktywną scenę** albo ponownie przygotuj workspace.
5. Sprawdź, czy istnieje plik **Assets/WiRR/Lab05/Models/Source/makerbeam_bracket_90degree.stp**.
6. Utwórz lokalną gałąź **lab05-work**.
7. Nie nadpisuj modelu STEP.

## Minimalny pipeline

    STEP źródłowy
        -> tessellacja w CAD/konwerterze
        -> FBX lub OBJ
        -> kontrola w Blenderze/DCC
        -> Unity
        -> audyt topologii
        -> LOD0 / LOD1 / LOD2
        -> benchmark i decyzja

STL wykonaj jako wariant świadomie stratny. Powinien pomóc pokazać, że sama geometria trójkątów nie zachowuje całej informacji z CAD.

## Co musisz rozumieć przed konwersją

- STEP przechowuje dokładną geometrię CAD/B-Rep, a FBX, OBJ i STL są reprezentacjami mesh po tessellacji.
- STL przechowuje głównie geometrię trójkątów. Nie odzyskasz z niego automatycznie materiałów, UV, hierarchii ani historii CAD.
- OBJ jest dobry dla statycznego mesha, UV i normalnych, ale nie jest pełnym formatem animowanego robota.
- FBX przenosi bogatszą scenę DCC: hierarchię, transformacje, mesh, materiały, rig i animację.
- URDF opisuje strukturę robota. Może wskazywać na wiele plików mesh i dodatkowo przechowuje linki, jointy, limity oraz dane inertial.
- MJCF opisuje model dla MuJoCo i zawiera semantykę dynamiki specyficzną dla symulatora.
- USD opisuje i komponuje scenę z warstw, referencji i primów. Konwersja USD do pojedynczego FBX może spłaszczyć lub utracić część tej semantyki.

Pełny opis i macierz konwersji znajdują się w **MODEL_FORMATS_AND_CONVERSION.md**.

## Kontrola po każdej konwersji

Sprawdź zawsze:

1. dwa wymiary referencyjne i bounding box;
2. jednostki, oś pionową, kierunek forward i handedness;
3. liczbę części, nazwy, pivoty i hierarchię;
4. normalne, tangenty, UV, materiały i tekstury;
5. liczbę wierzchołków, trójkątów, submeshy i colliderów;
6. czy visual i collision nie zostały przypadkowo złączone;
7. w modelach robotów: joint origin, axis, limits, mass, inertia i center of mass;
8. które informacje zostały utracone i czy ich utrata jest akceptowalna.

## Zasada najważniejsza

Konwersja nie jest rekonstrukcją. Zmiana STL, OBJ albo FBX na STEP nie odtworzy oryginalnych powierzchni NURBS, więzów i intencji konstrukcyjnej. Tak samo konwersja FBX do URDF nie utworzy poprawnej kinematyki robota bez jawnego zdefiniowania linków, jointów i parametrów dynamiki.

Dalszą procedurę, eksperymenty A/B/C i wymagane dowody wykonuj zgodnie z główną instrukcją Laboratorium 05.

# Blok III: Bliźniak rzeczywistości. Lab 05 + Lab 06

**Spotkanie 3 z 4.** Połącz geometrię obiektu przemysłowego z informacją o jego aktualnym stanie. Model z piękną grafiką nie staje się bliźniakiem, dopóki nie wiadomo, skąd i kiedy pochodzą dane.

## Spis treści
- [Wybierz ścieżkę](#wybierz-ścieżkę)
- [Lab 05: CAD i wydajność](#lab-05-cad-i-wydajność)
- [Lab 06: LIVE, STALE i REPLAY](#lab-06-live-stale-i-replay)
- [Raport i misja](#raport-i-misja)

## Wybierz ścieżkę
- **Masz siatkowy FBX/OBJ/STL:** zacznij od [importu Lab 05](../../Samples~/Lab05/README.md#30-import-i-audyt-cad).
- **Masz STEP:** skorzystaj z [konwersji modelu](../../Samples~/Lab05/README.md#szybki-wybór-formatu).
- **Nie masz programu CAD:** użyj dostarczonego modelu referencyjnego z Lab 05.
- **Masz ROS 2 i temat JointState:** [Lab 06](../../Samples~/Lab06/README.md#30-źródło-stanu).
- **Nie masz ROS 2:** wybierz log REPLAY, bez instalacji na każdym laptopie. Zapisz, że oglądasz odtworzone dane.
- **Windows:** łącz się z przygotowanym hostem Ubuntu; **Ubuntu:** możesz korzystać z [lokalnej ścieżki](../Reference/OS_SETUP.md#ubuntu).

## Lab 05: CAD i wydajność

1. Otwórz **Start: Lab 05**. Sprawdź jednostki, hierarchię obiektów oraz liczbę trójkątów w dostarczonym modelu.
2. Porównaj warianty Low/High albo LOD0/LOD1/LOD2. Ustal, jaki detal jest potrzebny do rozpoznania elementu maszyny, a jaki jest zbędny.
3. Wykonaj trzy porównywalne próby Profiler przy identycznej kamerze i scenie.
4. Opisz kompromis między czytelnością modelu, liczbą materiałów i czasem renderowania.
5. Nie zmieniaj jednocześnie skali, siatki i materiałów. Pierwszy realny wynik jest ważniejszy niż imponujący asset.

[Pełny Lab 05](../../Samples~/Lab05/README.md#zadania-krok-po-kroku).

## Lab 06: LIVE, STALE i REPLAY

1. Wybierz źródło **LOCAL, LAN lub REPLAY**; wskaż, czy dane pochodzą z rzeczywistej transmisji.
2. Otwórz **Start: Lab 06**. Użyj modelu robota oraz dostępnego źródła JointState. Sprawdź mapowanie przegubów po nazwach, nie po kolejności.
3. Wprowadź kontrolowaną przerwę w odtwarzaniu albo sieci. Czy wskaźnik STALE uczciwie ostrzega użytkownika?
4. Przywróć dane i zaobserwuj zmianę STALE → LIVE albo ponowne odtwarzanie REPLAY.
5. Opisz zastosowanie bliźniaka do zdalnego monitorowania robota, diagnostyki fabryki lub szkolenia operatora.

[Pełny Lab 06](../../Samples~/Lab06/README.md#zadania-krok-po-kroku).

## Raport i misja

Jedna karta **blok III**, dwie sceny i jedna interpretacja ograniczeń. **Ciekawostka:** zły bliźniak bywa bardziej niebezpieczny niż brak bliźniaka, jeśli pokazuje płynny, ale nieaktualny ruch robota. Dlatego stan STALE jest informacją równie istotną jak sam model.

# Lab 04 — Rozumienie sceny i mieszanie rzeczywistości

Ten folder jest próbką pakietu **WiRR — narzędzia kursu** przeznaczoną dla Laboratorium 4.

Po instalacji zależności zaimportuj wymagane próbki AR Foundation.

## Zalecana kolejność

1. Otwórz `WiRR → Narzędzia kursu`.
2. Wybierz Lab 04.
3. Zainstaluj lub napraw zależności.
4. Zaimportuj wymagane próbki Unity.
5. Użyj `Utwórz / napraw aktywną scenę`.
6. Wykonaj diagnostykę Depth API i eksperyment v1.
7. Zbadaj okluzję środowiskową i eksperyment v2.
8. Wykonaj pomiary depth-raycast i eksperyment v3.
9. Zbadaj estymację oświetlenia i eksperyment v4.
10. Wykonaj kontrolowany błąd, diagnozę H1/H2 i naprawę dla checkpointu 5.0.
11. Uruchom `Sprawdź konfigurację laboratorium`.
12. Uzupełnij `WiRR Reports` do osiągniętego checkpointu.

Zakres raportu odpowiada aktualnej instrukcji: **Depth API → okluzja → depth-raycast → estymacja oświetlenia → kontrolowany błąd i diagnoza**.

## Materiały dydaktyczne

Po przygotowaniu sceny możesz użyć sekcji **Scena i pomiary → Materiały dydaktyczne**:

1. **Dodaj środowisko** — tworzy wspólne stanowisko WiRR; w Lab 03–04 zamiast wirtualnego pokoju używany jest lekki zestaw odniesienia AR.
2. **Dodaj zestaw eksperymentalny** — tworzy prefaby właściwe dla bieżącego laboratorium w `Assets/WiRR/Lab04/Prefabs/Generated` i umieszcza ich instancje pod `WiRR_TeachingAssets`.
3. **Usuń obiekty dydaktyczne ze sceny** — usuwa wyłącznie gałąź `WiRR_TeachingAssets`; nie usuwa pracy studenta ani wygenerowanych prefabów.

Materiały są opcjonalne i służą jako kontekst eksperymentu. Nie konfigurują za studenta komponentów stanowiących cel ćwiczenia. Folder `Prefabs/Generated` jest odtwarzalny — własne rozwiązania zapisuj poza nim.

`report-template.md` pozostaje formatem referencyjnym i awaryjnym.
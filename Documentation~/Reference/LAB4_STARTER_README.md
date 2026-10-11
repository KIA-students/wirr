# Laboratorium 04. Rozumienie sceny AR

Niniejszy dokument jest skrótem dla pakietu Unity. Aktualna, autorytatywna instrukcja laboratoryjna znajduje się w repozytorium `MatPomGit/prz`:

- [Instrukcja PDF, WiRR Lab 04](https://github.com/MatPomGit/prz/blob/main/courses/wirtualna-i-rozszerzona-rzeczywistosc/laboratories/lab-04-rozumienie-sceny-i-mieszanie-rzeczywistosci/wirr_lab04.pdf)
- [Kod LaTeX](https://github.com/MatPomGit/prz/blob/main/courses/wirtualna-i-rozszerzona-rzeczywistosc/laboratories/lab-04-rozumienie-sceny-i-mieszanie-rzeczywistosci/wirr_lab04.tex)

## Wymagania

- Unity **6000.6.5f1 lub nowsze**, pod warunkiem zgodności pakietów użytych w projekcie.
- Działająca scena Android AR z Lab 03.
- AR Foundation i Google ARCore XR Plugin w zgodnej parze wersji.
- Na telefonie: dostępność ARCore Depth należy sprawdzić w runtime, a nie zakładać na podstawie modelu urządzenia.

## Procedura

1. Zapisz kopię sceny Lab 03 pod nazwą `Assets/WiRR/Lab04/Scenes/Lab04_SceneUnderstanding.unity`.
2. Na kamerze AR dodaj `AROcclusionManager` oraz sprawdź działanie `ARCameraManager` i `ARCameraBackground`.
3. Wykonaj diagnostykę Depth API oraz żądanych i aktywnych trybów okluzji.
4. Przeprowadź pomiary okluzji i trafień `ARRaycastManager` w trybie `TrackableType.Depth`.
5. Zbadaj estymację oświetlenia, rozdzielając dostępność Ambient Intensity i Environmental HDR.
6. Uzupełnij raport i zachowaj surowe dane, wyniki `HIT/MISS` i brak danych oznaczony `NA/NV`.

**Uwaga:** historyczne instrukcje odwołujące się do `ARDepthManager`, `ARCameraManager.TryGetLatestFrame` albo `ARCameraAcquisitionFrame` nie opisują aktualnej implementacji AR Foundation 6. Nie kopiuj z nich kodu C#. Kompletne przykłady czterech skryptów znajdują się w aktualnym PDF.

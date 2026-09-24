# HDRI w pakiecie WiRR

Folder zawiera panoramy equirectangular HDR oraz ich podglądy LDR. Do oświetlenia i skyboxa wybieraj pliki `*_HDR.exr`; pliki `*_TONEMAPPED.jpg` są wygodnym podglądem, ale nie zachowują pełnego zakresu luminancji.

## Najprościej: narzędzie WiRR

1. Otwórz `WiRR → Narzędzia kursu`.
2. Przejdź do `Scena i pomiary → HDRI i skybox`.
3. Wybierz środowisko.
4. Przeczytaj proponowane zastosowanie.
5. Kliknij **Ustaw wybrane HDRI jako Skybox**.

WiRR utworzy edytowalny materiał `Skybox/Panoramic` w `Assets/WiRR/Common/Skyboxes`, przypisze do niego wybrany EXR i ustawi go jako globalny skybox aktywnej sceny.

## Ręczna konfiguracja

1. W Project Browser rozwiń `Packages → WiRR Course Toolkit → Textures → HDRI`.
2. Wybierz plik kończący się `_HDR.exr`.
3. Utwórz własny materiał: `Assets → Create → Material`.
4. W Inspectorze ustaw Shader na `Skybox/Panoramic`.
5. Przeciągnij wybrany EXR do pola tekstury panoramy.
6. Dla panoramy equirectangular ustaw mapowanie typu Latitude-Longitude i obraz 360°.
7. Otwórz `Window → Rendering → Lighting`.
8. W zakładce Environment przypisz materiał do `Skybox Material`.
9. Ustaw `Environment Lighting Source` na Skybox, jeśli chcesz używać panoramy także jako źródła światła otoczenia.
10. Dostosuj `Exposure` i `Rotation` materiału skybox, zamiast modyfikować plik EXR.

W URP panoramiczny skybox jest obsługiwany. Jeżeli scena korzysta z własnego komponentu Skybox na kamerze, może on nadpisywać globalny skybox.

## Proponowane zastosowania zestawów

- Amsterdam: miasto, metal/szkło, złożone tło.
- Clean Horizon: benchmarki i neutralne porównania.
- Day Sky: światło dzienne i ekspozycja.
- Evening Environment: zmierzch, kontrast UI.
- Forrest: środowisko naturalne.
- Indoor Environment: wnętrza i odbicia.
- Near Lake: kontrast niebo–teren i materiały refleksyjne.
- Night Sky: emisja, światła sztuczne i czytelność w ciemności.
- Tower: otwarty horyzont, skala i sylwetka obiektu.

## Dobra praktyka eksperymentalna

Jeżeli HDRI jest częścią warunków eksperymentu, nie zmieniaj jednocześnie panoramy, ekspozycji, kierunku światła i ustawień post-processingu. Zmieniaj jedną zmienną naraz i zapisuj nazwę HDRI, Exposure oraz Rotation w raporcie.

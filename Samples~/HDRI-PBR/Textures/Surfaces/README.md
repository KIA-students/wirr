# Surface texture library

Curated PBR textures used by the optional WiRR material-learning prefabs.

Families: AcousticFoam003, Concrete032, DiamondPlate005D, Fabric023, Fabric066, Metal004, PaintedWood007A, Sign002, Grass001, Metal044A, WoodFloor034 and WoodFloor040.

## Proponowane zastosowania

- AcousticFoam / Fabric — materiały niemetaliczne, mapa chropowatości (`roughness`) i mapa normalnych; dobre do porównania subtelnego reliefu.
- Concrete / PaintedWood / WoodFloor — kafelkowanie tekstury (`tiling`), skala UV, mipmapy i obserwacja powierzchni pod małym kątem.
- DiamondPlate / Metal004 / Metal044A — metalness, odbicia środowiskowe i wpływ HDRI.
- Sign — opacity i czytelność powierzchni oznaczeniowych.
- Grass — materiał organiczny i ocena powtarzalności tekstury.

Najbardziej użyteczne są w Lab 01 (koszt renderowania i percepcja jakości), Lab 05 (optymalizacja materiałów/modelu) oraz w testach HDRI, gdzie ten sam materiał można obserwować pod różnymi warunkami środowiskowymi.

The tools prefer the Y+ / OpenGL normal maps (NormalGL). DiamondPlate also keeps NormalDX so students can compare normal-map conventions.

The asset identifiers correspond to ambientCG materials. ambientCG publishes its assets under CC0: https://ambientcg.com/

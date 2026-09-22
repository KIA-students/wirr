# Grid texture sets

These textures are package-visible copies of the course textures kept in the development project at `Project~/Assets/Textures/`.

Logical aliases used by `WiRRPrefabTextureLibrary`:

| Logical set | Source prefix |
|---|---|
| Grid1 | `grid-1_*` |
| Grid2 | `grid_2_*` |
| Grid3 | `grid-4_*` |

The third logical alias intentionally maps to `grid-4_*`, because the repository currently contains no `grid_3_*` files.

At generation time the editor tool copies the selected maps into `Assets/WiRR/Common/Textures/Generated/`, configures import settings there, and creates ordinary editable URP materials in the student's project. The package textures themselves remain read-only source assets.

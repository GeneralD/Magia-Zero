# Magia ZERO — AI Context

- Unity Editor extension (UPM package) that generates randomized VRM avatar
  variations for NFT collections — "generative VRM-NFT generator".
- Stack: Unity 2022.2.10f1, C#, UIElements/UI Toolkit (UXML/USS) for editor UI.
  Depends on UniVRM (`com.vrmc.vrm` 0.108.0) for VRM import/export.
- Distributed as a UPM git package: `https://github.com/GeneralD/Magia-Zero?path=/Assets`.
  Package manifest: `Assets/package.json` (`com.zero.magia`, v0.0.1, MIT in
  manifest but no LICENSE file in repo).
- Status: early-stage personal project, last commit 2023-05; effectively dormant.
- Layout:
  - `Assets/Editor/Zero/` — editor inspectors & property drawers
    (GeneratorInspector, rule drawers for combination/probability/metadata/
    randomization/trait rules) plus UXML/USS in `Resources/`.
  - `Assets/Scripts/Zero/` (Generator, Extensions, Utility) — runtime/core
    generation logic.
  - `ProjectSettings/` — Unity project config; `UIElementsSchema/` — generated.
- No CLI build/test command; open in Unity 2022.2 to work on it.

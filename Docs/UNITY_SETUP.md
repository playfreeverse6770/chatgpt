# Unity Setup

## Recommended target
- Unity 2022.3 LTS or Unity 6
- 2D Core project is sufficient
- No optional package is required by the baseline scripts

## Import
1. Clone this repository.
2. Copy `Unity/Assets/GeometricStrategyGame` into your Unity project's `Assets` folder.
3. Let Unity compile.
4. Open any empty 2D scene.
5. Use `Tools > Geometric Strategy > Build MVP Scene`.
6. Save the generated scene.
7. Enter Play Mode.

## Controls in the MVP
- Left mouse: select Player One unit.
- Right mouse: move selected unit.
- Combat is automatic when an enemy enters attack range.

## Audio
The scene builder creates the audio service. Imported real clips can be assigned in the inspector. If a cue is missing, procedural fallback SFX is generated at runtime for the MVP.

## Suggested folder after import
`Assets/GeometricStrategyGame/`
- Runtime/Core
- Runtime/Rendering
- Runtime/Units
- Runtime/Economy
- Runtime/World
- Runtime/AI
- Runtime/Input
- Runtime/Audio
- Runtime/Bootstrap
- Editor

## Important
The generated values are prototype balance values. Keep gameplay tuning separate from visual ownership rules: unit **color means level**, while the **inner emblem means faction**.

# Geometric Strategy Game

Unity-ready source project for a strategy game built around geometric units, resource symbols, professions, faction emblems, AI raiders, progression, and audio hooks.

## Core visual language

### Units
- Circle = Soldier / Infantry
- Triangle = Archer / Ranged unit
- Square = Cavalry
- Sun = King

### Unit levels
1. Yellow
2. Orange
3. Red
4. Purple
5. Green
6. Light Blue
7. Deep Blue

The fill color communicates level. The emblem inside the unit communicates faction/player ownership, so two players can use the same level colors without becoming visually ambiguous.

### Resources
- Green six-point star = Wood
- Gray trapezoid = Stone
- Brown pentagon = Metal
- Gold pentagon = Gold

### Professions
The codebase reserves geometric profession emblems for Blacksmith, Farmer, Carpenter, Miner, Weaponsmith, and Animal Breeder.

### AI factions
Initial hostile computer factions: Wolf, Bear, Eagle. Their emblems are rendered as geometric line/polygon approximations rather than raster art.

## Unity integration

Copy `Unity/Assets/GeometricStrategyGame` into your Unity project's `Assets` folder. The runtime code avoids optional packages and is intended to remain compatible with Unity 2022.3 LTS and Unity 6.

See `Docs/GAME_DESIGN.md` and `Docs/UNITY_SETUP.md` before wiring the scene.

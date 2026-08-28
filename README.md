# Geometric Strategy Game

Unity-ready source project for a strategy game built around geometric units, resource symbols, professions, faction emblems, AI raiders, progression, crafting, livestock, and audio.

## Current MVP

- Circle = Soldier / Infantry
- Triangle = Archer / Ranged
- Square = Cavalry
- Sun = King
- King elimination is the initial win/loss condition.
- Unit fill color means level; inner emblem means faction ownership.
- Player One and Player Two can be controlled locally; press `Tab` to switch the active player in the MVP.
- Wolf, Bear, and Eagle raider factions automatically seek and attack player factions.
- Units auto-attack enemies in range.
- No external art package is required: shapes and faction/profession emblems are generated procedurally.

## Level colors

1. Yellow
2. Orange
3. Red
4. Purple
5. Green
6. Light Blue
7. Deep Blue

## Resources

- Green six-point star = Wood
- Gray trapezoid = Stone
- Brown pentagon = Metal
- Gold pentagon = Gold
- Gold can be minted into Coin through the crafting system.

Initial recipes include Wall, Bow, Basic Weapon, Catapult, Advanced Weapon, and Coin minting.

## Professions

- Blacksmith
- Farmer
- Carpenter
- Miner
- Weaponsmith
- Animal Breeder

Carpenters harvest Wood. Miners harvest Stone/Metal/Gold. Farmers produce Food. Animal Breeders use Food and can be configured for Horse, Sheep, Cow, or Chicken. Blacksmiths and Weaponsmiths consume stored resources through the shared crafting system.

## Audio

The runtime provides procedural fallback cues for hit, arrow shot, wolf, bear, eagle, victory, defeat, upgrade, build, harvest, and coin events. Real CC0 replacement sources are curated in `Docs/AUDIO_SOURCES.md`.

## Unity integration

Copy `Unity/Assets/GeometricStrategyGame` into your Unity project's `Assets` folder.

Recommended target:
- Unity 2022.3 LTS or Unity 6
- 2D project
- Legacy Input Manager enabled, or Active Input Handling set to Both

Then open an empty scene and run:

`Tools > Geometric Strategy > Build MVP Scene`

The builder creates the camera/audio listener, two player armies, King units, shared resources, profession workers, faction economies, crafting/livestock storage, Wolf/Bear/Eagle raiders, game manager, audio service, and player command controller.

Controls:
- Left click: select current player's military unit
- Right click: move selected unit
- Tab: switch Player One / Player Two

See `Docs/GAME_DESIGN.md`, `Docs/UNITY_SETUP.md`, and `Docs/AUDIO_SOURCES.md`.

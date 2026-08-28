# Geometric Strategy Game — Design Baseline v0.1

## 1. Visual language

The game deliberately uses a strict geometric visual language. A unit's **outer shape** communicates its military class, its **fill color** communicates its level, and its **inner emblem** communicates ownership/faction.

### Military units
| Shape | Role |
|---|---|
| Circle | Soldier / Infantry |
| Triangle | Archer / Ranged |
| Square | Cavalry |
| Sun | King / sovereign objective |

### Level palette
| Level | Color |
|---|---|
| 1 | Yellow |
| 2 | Orange |
| 3 | Red |
| 4 | Purple |
| 5 | Green |
| 6 | Light Blue |
| 7 | Deep Blue |

Level colors never identify ownership. Ownership is always shown by an emblem rendered inside the unit.

## 2. Factions

### Player factions
- Player One
- Player Two

Each player gets a unique inner emblem. The exact emblem can be replaced later without changing unit logic.

### Hostile computer factions
Initial AI raider factions:
- Wolf Clan
- Bear Clan
- Eagle Clan

The project renders their emblems using geometric polylines/polygons rather than raster images.

## 3. King

The King uses a sun shape. The MVP default victory rule is **King elimination**: if a player's King reaches zero health, that faction is defeated. This rule is configurable and can later be expanded to territory, score, diplomacy, or timed objectives.

## 4. Resources

| Symbol | Color | Resource | Main use |
|---|---|---|---|
| Six-point star | Green | Wood | Walls, basic weapons, construction |
| Trapezoid | Gray | Stone | Stone construction, catapults, fortification |
| Pentagon | Brown | Metal | Higher-tier weapons and equipment |
| Pentagon | Gold | Gold | Minting coins, purchases, upgrades |

Gold is stored as a raw resource first. Coin minting is a craft/production step rather than treating raw gold and currency as the same thing.

## 5. Professions

Initial professions:
- Blacksmith
- Farmer
- Carpenter
- Miner
- Weaponsmith
- Animal Breeder

Animal Breeders are intended to support horses plus food animals such as sheep, cows, and chickens. Profession emblems are also geometric and are defined as data, so new professions can be added without replacing the rendering architecture.

## 6. Initial production/crafting chain

- Wood -> Wall
- Wood + Metal -> Bow / basic weapon
- Wood + Stone + Metal -> Catapult
- Metal + Gold -> advanced weapon tier
- Gold -> Coin

Numbers are intentionally balance placeholders. Recipes are centralized so tuning does not require rewriting gameplay systems.

## 7. MVP interaction

- Left click: select a controllable unit.
- Right click: issue a move order.
- Units automatically attack valid enemies in range.
- Archers have longer range and lower durability.
- Cavalry has higher mobility.
- Soldiers are the baseline frontline unit.
- AI Wolf/Bear/Eagle raiders seek enemy units/Kings and attack automatically.
- Resource nodes can be harvested through the resource API; worker automation is the next layer.

## 8. Architecture principles

1. Data-driven enums/configuration for unit class, level, resources, professions, factions, crafting and audio cues.
2. Rendering is procedural; no mandatory sprite package.
3. Gameplay logic does not depend on a specific faction emblem.
4. Audio clips are optional assignments. A procedural fallback keeps the MVP audible before final audio is imported.
5. No optional Unity package is required for the baseline.

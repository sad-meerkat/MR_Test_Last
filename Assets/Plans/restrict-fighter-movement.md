# Project Overview
- Game Title: MR Tabletop Fighting Game
- High-Level Concept: A 1v1 fighting game played on a virtual tabletop in Mixed Reality.
- Players: Multiplayer (Networked)
- Inspiration: Classic fighting games (Street Fighter, Tekken) adapted for MR.
- Target Platform: Android (Quest/Meta)
- Render Pipeline: URP-Performant
- Input System: New Input System

# Game Mechanics
## Core Gameplay Loop
Players select characters, spawn them on a virtual table, and battle until one player's health reaches zero. The game area is restricted to the surface of the virtual table.

## Controls and Input Methods
- Movement: Joystick/Thumbstick (Vector2) converted to XZ movement on the table.
- Attack: Button press.

# UI
- Fighting HUD: Displays health bars, timer, and player names. (Previously fixed scaling issues).

# Key Asset & Context
- `FightingGameManager.cs`: Handles spawning and game state.
- `FighterController.cs`: Handles character movement and attack input.
- `ByakuyaFighter.prefab`, `SasukeFighter.prefab`: Character prefabs.

# Implementation Steps
## 1. Modify FightingGameManager to Parent Characters
To ensure characters move with the table and can be easily restricted using local coordinates, they should be parented to the game manager (which is centered on the table).
- File: `Assets/MRTabletopAssets/Games/FightingGame/Scripts/FightingGameManager.cs`
- Change: Update `Instantiate` call in `SpawnFighters` to include `transform` as parent.

## 2. Modify FighterController to Clamp Movement
Add boundary constraints to prevent characters from walking off the table.
- File: `Assets/MRTabletopAssets/Games/FightingGame/Scripts/FighterController.cs`
- Change:
    - Add `[SerializeField] Vector2 m_MovementBounds = new Vector2(0.4f, 0.4f);` (based on table size of 0.8m).
    - In `Update()`, after updating `localPosition`, clamp `x` and `z` values.
    - `transform.localPosition = new Vector3(Mathf.Clamp(newPos.x, -m_MovementBounds.x, m_MovementBounds.x), newPos.y, Mathf.Clamp(newPos.z, -m_MovementBounds.y, m_MovementBounds.y));`

## 3. Verify and Tune Bounds
- Check the prefabs to ensure the bounds match the actual table size in the scene.
- Default bounds of 0.4m (total 0.8m) match the observed table mesh scale.

# Verification & Testing
- **Manual Check**: Play the game and try to walk the character to the edge of the table. They should stop at the boundary.
- **Table Movement Test**: Move the virtual table using MR interactions. The characters should move with the table.
- **Networking Test**: Ensure the clamped position syncs correctly across clients (since it's done in `Update` by the owner, `NetworkTransform` or similar should handle the sync).

# Project Overview
- Game Title: MR Tabletop Fighting Game
- High-Level Concept: A 1v1 fighting game played on a virtual tabletop in MR.
- Players: Single player (vs Dummy) or local/networked multiplayer.
- Input System: New Input System with XR Hands/Controllers.
- Render Pipeline: URP.

# Game Mechanics
## Core Gameplay Loop
Players control fighters on a tabletop, performing attacks to reduce the opponent's health to zero within a time limit.
## Controls and Input Methods
XR Hand tracking or controllers are used to move and trigger attacks (sword swings).

# UI
- **FightingHUD**: Top-of-screen UI containing health bars for both players and a countdown timer.
- **Fighter Health UI**: A world-space health bar attached to each character.

# Key Asset & Context
- `FighterHealth.cs`: Manages character health and updates UI.
- `FightingHUDManager.cs`: Manages the top-of-screen HUD.
- `ByakuyaFighter.prefab`, `SasukeFighter.prefab`: Character prefabs.
- `FightingHUD`: The HUD object in the scene.

# Implementation Steps
The goal is to remove the character-attached health UI and ensure the top HUD correctly reflects health changes.

## Step 1: Remove Character-Attached UI
- **Description**: Deactivate the `HealthCanvas` on the character prefabs so they no longer display individual health bars.
- **Assigned role**: developer
- **Files**:
    - `Assets/MRTabletopAssets/Games/FightingGame/Prefabs/ByakuyaFighter.prefab`
    - `Assets/MRTabletopAssets/Games/FightingGame/Prefabs/SasukeFighter.prefab`
- **Action**: Deactivate the `HealthCanvas` child object in these prefabs.
- **Dependencies**: None
- **Parallelizable**: Yes

## Step 2: Update FighterHealth.cs Logic
- **Description**: Modify the `UpdateUI` method to improve robustness when updating the top HUD and remove references to the local health bar.
- **Assigned role**: developer
- **Files**:
    - `Assets/MRTabletopAssets/Games/FightingGame/Scripts/FighterHealth.cs`
- **Action**:
    - Comment out or remove the logic updating `m_HealthBar`.
    - Ensure `FightingHUDManager.Instance` is checked and, if null, attempt to find the instance using `Object.FindFirstObjectByType<FightingHUDManager>()`.
    - Add debug logging to trace `playerIndex` and `healthPercent` to help identify why the update might be failing.
- **Dependencies**: None
- **Parallelizable**: Yes

## Step 3: Scene Cleanup (Optional/Verification)
- **Description**: Verify the "two UI" situation. If there is a duplicate HUD or timer, it should be deactivated.
- **Assigned role**: explorer
- **Action**: Check `InGameUI` and `PreGameUI` activation logic in `FightingGameManager`.
- **Dependencies**: Step 2

# Verification & Testing
1. **Manual Check**: Play the game and take damage.
2. **UI Verification**:
    - Confirm the health bar above the character's head is gone.
    - Confirm the health bar at the top of the screen (HUD) decreases when the character is hit.
3. **Log Verification**: Check the console for logs showing `UpdateUI` being called with the correct `playerIndex` (0 or 1).

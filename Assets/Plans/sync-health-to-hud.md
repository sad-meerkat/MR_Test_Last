# Project Overview
- Game Title: MR Tabletop Fighting Game
- High-Level Concept: 1v1 fighting game in MR.
- Players: Multiplayer / Single vs Dummy.
- UI: Top HUD for health/timer.

# Game Mechanics
## Core Gameplay Loop
Players fight and health decreases on the top HUD.

# UI
- **FightingHUD**: Contains P1/P2 health bars and timer.
- **Character UI**: Removed (as per user).

# Key Asset & Context
- `FighterHealth.cs`: Updates the HUD when health changes.
- `FightingHUDManager.cs`: Singleton managing the HUD.
- `FightingGameManager.cs`: Toggles UI states.

# Implementation Steps

## Step 1: Enhance FighterHealth.cs
- **Description**: Improve the HUD update logic to ensure it works even if the singleton instance is initialized late or accessed via search. Add logging to debug player index issues.
- **Assigned role**: developer
- **Files**: `Assets/MRTabletopAssets/Games/FightingGame/Scripts/FighterHealth.cs`
- **Action**:
    - Modify `UpdateUI()` to check for `FightingHUDManager.Instance`.
    - If null, try to find it using `Object.FindFirstObjectByType<FightingHUDManager>()`.
    - Add `Debug.Log` to see what `playerIndex` and `health` values are being sent to the HUD.
- **Dependencies**: None

## Step 2: Verify FightingHUDManager Singleton
- **Description**: Ensure the `FightingHUDManager` singleton is correctly initialized in `Awake`.
- **Assigned role**: developer
- **Files**: `Assets/MRTabletopAssets/Games/FightingGame/Scripts/FightingHUDManager.cs`
- **Action**: Verify `Instance = this;` in `Awake`. (Already exists in current code, but good to keep in mind).
- **Dependencies**: None

## Step 3: Scene Verification
- **Description**: Confirm that the `FightingHUD` object in the scene has the `FightingHUDManager` script and the health bar images are correctly assigned to `p1HealthBar` and `p2HealthBar`.
- **Assigned role**: explorer
- **Action**: Use `RunReadOnlyCommand` to check assignments if not already confirmed.
- **Dependencies**: None

# Verification & Testing
1. **Damage Test**: Trigger an attack that hits a character.
2. **Log Check**: Look for "[FighterHealth] Updating HUD: Player [0/1] - Health [Amount]" in the console.
3. **UI Check**: Confirm the top health bar reflects the health change.

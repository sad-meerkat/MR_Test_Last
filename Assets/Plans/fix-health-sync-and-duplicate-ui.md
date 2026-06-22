# Project Overview
- Game Title: MR Tabletop Fighting Game
- High-Level Concept: 1v1 tabletop fighting game.
- UI: World-space HUD with health bars and timer.

# Game Mechanics
- Players fight, health decreases.
- Game ends when health reaches 0, returning to the "R u ready?" (Start) screen.

# UI
- **FightingHUD**: Top-of-screen health bars (P1=Left, P2=Right) and Timer.
- **PreGameUI**: Contains the "R u ready?" button.

# Key Asset & Context
- `FightingHUDManager.cs`: Manages the top HUD health bars.
- `FighterHealth.cs`: Sends health updates to the HUD.
- Logs show that `FighterHealth` is correctly sending updates for "Player 1" (the Dummy) when it gets hit.

# Implementation Steps

## Step 1: Add Robust Logging to HUD Manager
- **Description**: Add logs to `FightingHUDManager.UpdateHealth` to verify the exact values and component states during a health update. This will help confirm if the `fillAmount` is actually being applied to the Image.
- **Assigned role**: developer
- **Files**: `Assets/MRTabletopAssets/Games/FightingGame/Scripts/FightingHUDManager.cs`
- **Action**:
    - Modify `UpdateHealth(int playerIndex, float healthPercent)` to include:
      `Debug.Log($"[FightingHUDManager] Updating Player {playerIndex} Health Bar to {healthPercent}. FillAmount was: {(playerIndex == 0 ? p1HealthBar.fillAmount : p2HealthBar.fillAmount)}");`
- **Dependencies**: None

## Step 2: Ensure UI Exclusivity and Hide Duplicate Timers
- **Description**: The user mentioned "two UIs with timers". While the code handles toggling `InGameUI` and `PreGameUI`, we will ensure that when the game is in `Fighting` state, any other potential timers or HUDs are explicitly hidden.
- **Assigned role**: developer
- **Files**: `Assets/MRTabletopAssets/Games/FightingGame/Scripts/FightingGameManager.cs`
- **Action**:
    - In `UpdateUIState`, double-check that `m_InGameUI` is the ONLY active UI container during a fight.
    - We will also add a safety check to hide the `PreGameUI`'s `SelectionTimer` if it persists.
- **Dependencies**: None

## Step 3: Verify Practice Mode Indexing
- **Description**: Confirm that the Dummy is always assigned index 1 (Right bar) and the Player is index 0 (Left bar).
- **Assigned role**: developer
- **Files**: `Assets/MRTabletopAssets/Games/FightingGame/Scripts/FightingGameManager.cs`
- **Action**: Add a log in `SpawnFighters` to explicitly show which character is assigned which index.
- **Dependencies**: None

# Verification & Testing
1. **Play Mode**: Start the game in Practice Mode.
2. **Attack Dummy**: Hit the dummy and check the console.
    - You should see: `[FighterHealth] Updating HUD for Player 1: 90/100`
    - And: `[FightingHUDManager] Updating Player 1 Health Bar to 0.9...`
3. **Visual Check**: Confirm the **Right** health bar decreases.
4. **End Game**: When dummy dies, confirm "R u ready?" appears and HUD disappears.

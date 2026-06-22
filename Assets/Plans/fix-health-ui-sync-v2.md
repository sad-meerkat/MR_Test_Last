# Project Overview
- Game Title: MR Tabletop Fighting Game
- High-Level Concept: 1v1 fighting game on a virtual table.
- Issue: Top health bar not reflecting damage, duplicate timers, and UI scale issues.

# Game Mechanics
- Players swing hands to trigger sword attacks.
- Sword hits decrease health and update the top HUD.

# UI
- **FightingHUD**: Top-of-screen health bars and timer.
- **PreGameUI**: Start button and character selection.
- **InGameUI**: Container for FightingHUD.

# Key Assets
- `FighterHealth.cs`: Manages health and HUD updates.
- `FightingGameManager.cs`: Handles game flow and UI transitions.
- `TableCharacter.cs`: Base class for fighters, handles movement and attacks.
- `TableCharacterInput.cs`: Connects hand gestures to character actions.

# Root Cause Analysis
1. **Ownership Conflict**: In Practice Mode, the Server (Host) owns both the Player and the Dummy. `TableCharacterInput` auto-finds the last spawned owned character, which is often the Dummy. Thus, the player's hand swing triggers the **Dummy** to attack the **Player**, causing the **Left** bar (Player 0) to decrease instead of the dummy's bar.
2. **UI Scaling**: `InGameUI` has a local scale of (0,0,0) in the scene, making it invisible or distorted.
3. **UI State Conflict**: `PreGameUI` and `InGameUI` are active simultaneously, causing duplicate timers.
4. **Hit Logic**: The dummy is hit but the player also gets hit because the attack range hits both, or the index assignment is confusing.

# Implementation Steps

## Step 1: Distinguish Dummy and Prevent Dummy Control
- **Description**: Add a `isDummy` flag to `TableCharacter` and ensure `TableCharacterInput` only controls the non-dummy character.
- **Files**:
    - `Assets/MRTabletopAssets/Scripts/Character/TableCharacter.cs` (Add `public bool isDummy;`)
    - `Assets/MRTabletopAssets/Games/FightingGame/Scripts/FightingGameManager.cs` (Set `isDummy = true` for the dummy character)
    - `Assets/MRTabletopAssets/Scripts/Character/TableCharacterInput.cs` (Modify auto-find logic to ignore dummies)

## Step 2: Fix UI Scale and Visibility
- **Description**: Ensure `InGameUI` is scaled to (1,1,1) when activated and only the correct UI state is active.
- **Files**:
    - `Assets/MRTabletopAssets/Games/FightingGame/Scripts/FightingGameManager.cs`
- **Action**:
    - In `UpdateUIState`, set `m_InGameUI.transform.localScale = Vector3.one` if it's not.
    - Explicitly deactivate `PreGameUI` when `Fighting`.

## Step 3: Enhance HUD Sync Robustness
- **Description**: Ensure `UpdateUI` is called correctly when `playerIndex` is set.
- **Files**:
    - `Assets/MRTabletopAssets/Games/FightingGame/Scripts/FighterHealth.cs`
- **Action**: Call `UpdateUI()` when `playerIndex` changes to ensure the bar is positioned correctly at spawn.

# Verification & Testing
1. **Play Mode (Host)**: Start a practice game.
2. **Check Control**: Move the hand; verify your character (Seat 0) swings, not the dummy.
3. **Check Hit**: Hit the dummy; verify the **Right** health bar (Player 1) decreases.
4. **Check UI**: Verify only one timer is visible and the HUD is properly scaled.

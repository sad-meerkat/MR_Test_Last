# Project Overview
- Issue: Sasuke character cannot be selected or controlled properly in character selection.
- Root Cause: 
  1. Input system accidentally links to the Dummy character (also owned by host).
  2. Character selection logic only supports two seats (0 and 1).
  3. UI buttons might pass indices out of bounds of the character list.

# Implementation Steps

## Step 1: Update FightingGameManager.cs Logic
- **Description**: Fix the character linking coroutine and expand seat support to all 4 seats.
- **Assigned role**: developer
- **Files**: `Assets/MRTabletopAssets/Games/FightingGame/Scripts/FightingGameManager.cs`
- **Action**:
    - Modify `LinkLocalCharacterDelayed` to only set characters where `!character.isDummy`.
    - Refactor `TogglePreReadyRpc` and `SelectCharacterRpc` to support all 4 seats (using arrays or loops instead of hardcoded P1/P2 logic).
    - Refactor `SpawnFighters` to use the same logic for all seated players.

## Step 2: Robust Character Index Mapping
- **Description**: Ensure that clicking character slots always picks a valid character or provides a fallback.
- **Assigned role**: developer
- **Files**: `Assets/MRTabletopAssets/Games/FightingGame/Scripts/FightingGameManager.cs`
- **Action**: Add a check in `SpawnFighters` to clamp the choice index or handle invalid indices gracefully.

# Verification & Testing
1. **Play Mode**: Start the game as Host.
2. **Selection**: Sit in Seat 0 and click the Sasuke button (Slot 1).
3. **Verify Control**: Confirm you move Sasuke.
4. **Seat Test**: Sit in Seat 2 (Side seat) and verify character selection still works.

# Project Overview
- Game Title: MR Tabletop Fighting Game
- Issue:
  - `InGameUI` is invisible because its scale is (0,0,0) in the scene and the fix was not applied due to a connection error.
  - Player controls the Dummy in Practice Mode because the ownership/input logic fix was not applied.
  - Health reflection is confusing because the Player is hitting themselves or the Dummy is attacking the Player.

# Implementation Steps

## Step 1: Fix UI Scale and Exclusivity
- **Description**: Ensure `InGameUI` is scaled to (1,1,1) when the fight starts and `PreGameUI` is hidden.
- **Assigned role**: developer
- **Files**: `Assets/MRTabletopAssets/Games/FightingGame/Scripts/FightingGameManager.cs`
- **Action**: Update `UpdateUIState` to set `m_InGameUI.transform.localScale = Vector3.one` when `state == FightingGameState.Fighting`.

## Step 2: Fix Practice Mode Control (Dummy vs Player)
- **Description**: Ensure the Player only controls their own character and not the Dummy.
- **Assigned role**: developer
- **Files**:
  - `Assets/MRTabletopAssets/Games/FightingGame/Scripts/FightingGameManager.cs` (Set `isDummy = true` for practice dummy)
  - `Assets/MRTabletopAssets/Scripts/Character/TableCharacterInput.cs` (Ignore characters with `isDummy = true`)
- **Action**:
  - In `FightingGameManager.SpawnFighters`, set `tableChar.isDummy = true` for the dummy.
  - In `TableCharacterInput.Update`, add `!character.isDummy` to the character search condition.

# Verification & Testing
1. **Play Mode**: Start a practice game.
2. **UI Check**: Verify the top health bars and timer are visible and properly scaled.
3. **Control Check**: Verify you move your character, not the dummy.
4. **Health Check**: Hit the dummy and verify the **Right** health bar (P2) decreases.

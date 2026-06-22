# Project Overview
- Game Title: MR Tabletop Fighting Game
- Issue: Top health bar UI is invisible due to zero scale and duplicate timers are visible.

# Implementation Steps

## Step 1: Fix InGameUI Scale and UI Transitions
- **Description**: Ensure `InGameUI` (where the health bars are) is properly scaled to (1,1,1) when the game starts, and ensure only one UI is active at a time to remove duplicate timers.
- **Assigned role**: developer
- **Files**: `Assets/MRTabletopAssets/Games/FightingGame/Scripts/FightingGameManager.cs`
- **Action**:
    - In `UpdateUIState(FightingGameState state)`:
      - If `state == FightingGameState.Fighting`, set `m_InGameUI.transform.localScale = Vector3.one`.
      - Ensure `m_PreGameUI` is explicitly deactivated during the fight.
- **Dependencies**: None

# Verification & Testing
1. **Play Mode**: Start the game.
2. **Visual Check**: Verify that the top health bars and ONE timer appear correctly in the center-top of the table.

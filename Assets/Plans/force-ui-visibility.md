# Project Overview
- Issue: Health bar UI is invisible when the game starts.
- Cause: `InGameUI` scale is (0,0,0) and the code fix to set it to (1,1,1) might not be executing or is being overridden.
- Goal: Force UI scale and visibility.

# Implementation Steps

## Step 1: Force UI Scale and Visibility in FightingGameManager
- **Description**: Ensure `InGameUI` and `PreGameUI` scales are always (1,1,1) in `Awake` and log state changes to the console.
- **Assigned role**: developer
- **Files**: `Assets/MRTabletopAssets/Games/FightingGame/Scripts/FightingGameManager.cs`
- **Action**:
    - In `Awake()`, set `m_InGameUI.transform.localScale = Vector3.one` and `m_PreGameUI.transform.localScale = Vector3.one`.
    - In `UpdateUIState()`, add `Debug.Log($"[FightingGameManager] State: {state}, InGameUI Active: {m_InGameUI.activeSelf}");`.
- **Dependencies**: None

## Step 2: Ensure HUD Sync on Activation
- **Description**: Call `UpdateUI()` in `FighterHealth` when the game state changes to `Fighting` to ensure the HUD correctly reflects current health values.
- **Assigned role**: developer
- **Files**: `Assets/MRTabletopAssets/Games/FightingGame/Scripts/FighterHealth.cs`
- **Action**: Subscribe to `m_Manager.OnSummoned` or similar event to trigger a UI refresh when the fight starts.
- **Dependencies**: None

# Verification & Testing
1. **Play Mode**: Start the game.
2. **Visual Check**: Confirm the HUD appears when the fight starts. Check the console for `[FightingGameManager]` state logs.

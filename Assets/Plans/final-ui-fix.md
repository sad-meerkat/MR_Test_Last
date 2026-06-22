# Project Overview
- Game Title: MR Tabletop Fighting Game
- High-Level Concept: 1v1 fighting game on a tabletop.
- Issue: Health bar UI is invisible when the game starts, and there are duplicate timers/UIs.
- Goal: Fix UI visibility (scale issues), ensure correct HUD is shown, and maintain player control logic.

# Game Mechanics
## Core Gameplay Loop
Players fight using hand-swing gestures. Health decreases on the top HUD. Game ends when health reaches zero.

# UI
- **PreGameUI**: Shown during Idle and Character Selection. Contains "R u Ready?" and character selection.
- **InGameUI**: Shown during Fighting. Contains the top health bars and the main timer.

# Key Asset & Context
- `FightingGameManager.cs`: Manages state transitions and UI visibility.
- `FighterHealth.cs`: Sends health updates to the HUD.
- `TableCharacterInput.cs`: Handles player input and distinguishes between player and dummy.

# Implementation Steps

## Step 1: Force UI Scale and Ensure Exclusivity
- **Description**: Ensure `InGameUI` has a valid scale (1,1,1) from the start and that it is the only active UI during the fight. Add debug logs to track state changes.
- **Assigned role**: developer
- **Files**: `Assets/MRTabletopAssets/Games/FightingGame/Scripts/FightingGameManager.cs`
- **Action**:
    - In `Awake()` or `Start()`, set `m_InGameUI.transform.localScale = Vector3.one`.
    - In `UpdateUIState()`, add `Debug.Log($"[FightingGameManager] State changed to {state}. InGameUI active: {state == FightingGameState.Fighting}")`.
    - Explicitly set `m_InGameUI.transform.localScale = Vector3.one` inside `UpdateUIState` when the state is `Fighting`.
- **Dependencies**: None

## Step 2: Ensure HUD Sync when HUD becomes Active
- **Description**: If the HUD was inactive when the player spawned, the initial health update might have been missed. We will ensure the HUD is updated as soon as it becomes available.
- **Assigned role**: developer
- **Files**: `Assets/MRTabletopAssets/Games/FightingGame/Scripts/FightingHUDManager.cs`
- **Action**: Add an `OnEnable` method that calls a refresh or ensures the bars are initialized correctly. (Alternatively, have `FighterHealth` retry).
- **Dependencies**: None

## Step 3: Verification of Dummy Control Logic
- **Description**: Double-check that the player is indeed controlling their own character and not the dummy, which was a cause of confusing health updates earlier.
- **Assigned role**: developer
- **Files**: `Assets/MRTabletopAssets/Scripts/Character/TableCharacterInput.cs`
- **Action**: Verify the `!character.isDummy` check is robust.
- **Dependencies**: None

# Verification & Testing
1. **Play Mode**: Start the game and proceed to the Fighting state.
2. **Visual Check**: Verify that the top health bars are visible. If not, check the console for `[FightingGameManager]` logs.
3. **Control Check**: Verify that swinging your hand makes YOUR character attack, not the dummy.
4. **Health Sync Check**: Hit the dummy and verify the right health bar decreases.

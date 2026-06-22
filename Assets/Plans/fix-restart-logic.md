# Project Overview
- Game Title: MR Tabletop (Fighting Game Mode)
- Problem: The "R u ready?" button at the end of a match (`RuReadyButton_Result`) terminates the game session or does nothing, preventing a proper restart.
- Goal: Clicking the final "R u ready?" button should return both players to the initial "Lobby" state, requiring both to press the initial "R u ready?" button to start a new match.

# Root Cause Analysis
1.  **Improper Reset Function**: The button is currently linked to `RequestHideGameModeServerRpc`, which calls `HideGameMode()`.
2.  **UI Shutdown**: `HideGameMode()` explicitly sets `m_PreGameUI.SetActive(false)`, which hides the lobby UI (the initial "R u ready?" button).
3.  **State Reset**: While it sets the state to `Idle`, the immediate deactivation of the UI prevents players from seeing or interacting with the game again.

# Proposed Solution
Implement a new "Restart Match" logic that resets all game variables (round wins, player choices, ready flags) and returns the game state to `Idle` without hiding the lobby UI.

# Key Assets & Context
- `Assets/MRTabletopAssets/Games/FightingGame/Scripts/FightingGameManager.cs`: Main state controller.
- `MatchResultUI`: The panel shown at the end of the game.
- `RuReadyButton_Result`: The button that triggers the restart.

# Implementation Steps

## 1. Modify `FightingGameManager.cs`
- **Description**: Add a new `ServerRpc` for restarting the match.
- **Assigned role**: developer
- **Implementation**:
    - Add `[Rpc(SendTo.Server)] public void RequestRestartMatchServerRpc()`.
    - Inside this RPC:
        - Set `m_GameState.Value = FightingGameState.Idle`.
        - Reset `m_P1PreReady.Value = false` and `m_P2PreReady.Value = false`.
        - Reset `m_P1RoundWins.Value = 0` and `m_P2RoundWins.Value = 0`.
        - Reset `m_P1Ready.Value = false` and `m_P2Ready.Value = false`.
        - Set `m_WinnerIndex.Value = -1`.
        - Deactivate all images in `m_CharacterWinImages`.
        - Deactivate `m_RuReadyButton` (the result screen button).
        - Call `CleanupFighters()` to remove character models.
- **Dependencies**: None
- **Parallelizable**: No

## 2. Update Scene Button Event
- **Description**: Link the final "R u ready?" button to the new RPC.
- **Assigned role**: developer
- **Implementation**:
    - Locate `RuReadyButton_Result` in the hierarchy.
    - Change the `OnClick` event from `RequestHideGameModeServerRpc` to the new `RequestRestartMatchServerRpc`.
- **Dependencies**: Step 1
- **Parallelizable**: No

# Verification & Testing
1.  **Restart Loop Test**:
    - Play a match until it ends.
    - Click the "R u ready?" button in the result screen.
    - **Verify**: The result screen disappears, and the initial "R u ready?" button (Lobby) appears.
    - **Verify**: Both players must click the initial button to proceed to character selection.
2.  **State Sync Test**:
    - Ensure that when the Host clicks the restart button, the Client also sees the lobby UI.
    - Ensure all previous win markers and character models are gone.

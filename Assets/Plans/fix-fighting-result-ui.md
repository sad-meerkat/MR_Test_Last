# Project Overview
- Game Title: MR Tabletop (Fighting Game Mode)
- Problem: 
    - `RuReadyButton_Result` (the button to restart/return after a match) does not work when clicked.
    - Result UI elements (like winner images and the ready button) are only activated on the server, making them invisible or non-functional for clients.
    - The "Ready" button and result images are positioned too low or flipped (from previous feedback).

# Game Mechanics
- When a match ends (someone dies or time runs out), the game enters a result phase.
- Players should see who won, see a character-specific win image, and have a button to return to the lobby or restart.

# Key Assets & Context
- `Assets/MRTabletopAssets/Games/FightingGame/Scripts/FightingGameManager.cs`: Main manager for the game state.
- `RuReadyButton_Result`: Button in the `MatchResultUI`.
- `m_RuReadyButton`: Field in `FightingGameManager` pointing to the button.
- `m_CharacterWinImages`: Array of images for winners.

# Implementation Steps
1.  **Network Synchronization for Result UI**:
    - Add a `NetworkVariable<bool> m_ShowReadyButton` to `FightingGameManager`.
    - Add a `NetworkVariable<int> m_WinningCharacterChoice` to sync which win image to show.
    - In `OnNetworkSpawn`, subscribe to these variables to show/hide `m_RuReadyButton` and `m_CharacterWinImages` on all clients.
2.  **Fix Button Interaction**:
    - Create a `[Rpc(SendTo.Server)] void RequestHideGameModeServerRpc()` method in `FightingGameManager`.
    - This RPC will call the existing `HideGameMode()` logic on the server.
    - Update the `RuReadyButton_Result` button's `onClick` event in the scene to call this new RPC (or a wrapper method that calls it).
3.  **Refactor `EndGameSequence`**:
    - Instead of setting `SetActive(true)` directly (which only works on the server), update the new `NetworkVariable` values.
4.  **UI Alignment & Orientation (Continuing from previous request)**:
    - Set `MatchResultUI` and `FightingHUD` local rotations to `(0, 0, 0)`.
    - Adjust `RuReadyButton_Result` and Win Images' anchored positions (moving them higher as requested).
5.  **Cleanup**:
    - Ensure `HideGameMode` resets the new NetworkVariables so the UI is hidden for the next match.

# Verification & Testing
1.  **Multiplayer Test (Host & Client)**:
    - Run the game with two players.
    - Finish a match.
    - **Verify**: Both Host and Client see the winner text, the correct win image, and the "Ready" button.
    - **Verify**: Clicking the "Ready" button as a Client correctly resets the game for everyone.
    - **Verify**: The UI elements are oriented correctly and positioned at a good height.

# Project Overview
- Game Title: MR Tabletop
- Problem: 
    1. When selecting a game (e.g., VR Fight), the next UI (Start Button, Character Selection) does not appear to the player.
    2. Several games (Chess, Slingshot, etc.) are missing from the game selection list.
    3. Duplicate and broken UI hierarchies exist in the scene.

# Root Cause Discovery
1.  **UI Orientation**: UI panels like `PreGameUI` and `InGameUI` have `SeatBillboard` components but are not correctly hooked up to the `TableSeatSystem.m_OnSeatChanged` event. When a player sits at Seat 2, these panels remain oriented for Seat 1, making them invisible or incorrectly positioned for the player at Seat 2.
2.  **Hidden Buttons**: In the `GamesPanel`, the buttons for `Empty Table`, `Physics Sandbox`, `Slingshot`, and `Chess` have `ActiveSelf` set to `false`, so they don't appear in the selection list.
3.  **Redundant UI**: A duplicate hierarchy `UI (1)` exists with broken/null references, causing potential confusion and raycasting issues.

# Implementation Steps
1.  **Fix UI Seat Following (Manual/Inspector)**:
    - Select the `TableSystem` object (which has `TableSeatSystem`).
    - In the `On Seat Changed (Int32)` event list, add/verify the following objects:
        - `PreGameUI`
        - `InGameUI`
        - `World Space Canvas` (Main Menu)
        - `MatchResultUI`
    - For each, set the function to `SeatBillboard -> RotateBillboard` (using the **Dynamic int** version).
2.  **Restore Missing Games**:
    - Navigate to `UI/World Space Canvas/Table UI/Navigation Menu/Panel Container/Current Table Panel/GameSelection/GamesPanel`.
    - Set `Game Button (1)` through `(4)` to **Active**.
3.  **Cleanup Scene**:
    - Delete the `UI (1)` GameObject hierarchy.
4.  **Fix Fighting Game Restart (Network)**:
    - Update `FightingGameManager.cs` to include a `ServerRpc` for the restart/return button so clients can also trigger the game reset (as discussed in the previous turn).

# Verification & Testing
1.  **Seat 2 Test**:
    - Sit at Seat 2.
    - **Verify**: The UI (Navigation Menu) follows you.
    - **Verify**: Open Game Selection. All 6 games should be visible.
    - **Verify**: Select "VR Fight".
    - **Verify**: The `StartButton` should appear directly in front of you at Seat 2.
2.  **Multiplayer Test**:
    - Ensure both players can see and interact with the game selection and pre-game UI at their respective seats.

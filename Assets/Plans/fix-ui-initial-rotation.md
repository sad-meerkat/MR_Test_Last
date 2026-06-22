# Project Overview
- Game Title: MR Tabletop
- Problem: When sitting at Seat 2 and selecting "VR Fight", the "R u ready?" button is invisible/not appearing.
- Root Cause: 
    - The `SeatBillboard` component on `PreGameUI` only updates its rotation when the `m_OnSeatChanged` event fires.
    - When a player sits down, the event fires, but since the fighting game hasn't started yet, `PreGameUI` is **Inactive**, so it might miss the update or it's later reactivated with its default (Seat 1) rotation.
    - More importantly, if a player is already seated and then selects the game, the UI is activated but nothing tells it to rotate to the current seat immediately. It stays at 0 degrees (facing Seat 1).

# Game Mechanics
- Players sit at the table (fires `OnSeatChanged`).
- Players select a game mode (activates the game's UI).
- UI should face the current player.

# Key Assets & Context
- `Assets/MRTabletopAssets/Scripts/SeatBillboard.cs`: Needs to initialize rotation.
- `Assets/MRTabletopAssets/Scripts/Table/TableTop.cs`: Holds the current seat index in `k_CurrentSeat`.

# Implementation Steps
1.  **Modify `SeatBillboard.cs`**:
    - Add an `OnEnable()` method.
    - Inside `OnEnable()`, call `RotateBillboard(TableTop.k_CurrentSeat)` to ensure the UI immediately faces the correct player as soon as it's turned on.
    - This ensures that when "VR Fight" is selected and the UI becomes active, it doesn't wait for a seat change event to fix its rotation.

# Verification & Testing
1.  **Manual Test**:
    - Sit at Seat 2.
    - Open the game list and select "VR Fight".
    - **Verify**: The "R u ready?" button should appear directly in front of you immediately.
2.  **Edge Case**:
    - Switch seats while the button is visible.
    - **Verify**: The button should rotate to follow you to the new seat.

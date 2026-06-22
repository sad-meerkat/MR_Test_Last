# Project Overview
- Game Title: MR Tabletop
- High-Level Concept: Multi-user tabletop game in MR/VR.
- Players: Multiplayer (Networked)
- Render Pipeline: URP
- Target Platform: Android (Quest)

# Bug Report Analysis
The user reports that when switching from Seat 1 to Seat 2, the UI stays at Seat 1. Only the player's position changes.

## Root Cause Discovery
1.  The UI is contained within a **World Space Canvas** (and other GameObjects) that uses the `SeatBillboard` component.
2.  `SeatBillboard.RotateBillboard(int seatID)` is responsible for rotating the UI to match the current seat.
3.  `TableSeatSystem.TeleportToSeat(int seatNum)` triggers the seat change and invokes the `m_OnSeatChanged` Unity Event.
4.  Investigation reveals that `SeatBillboard.RotateBillboard` is likely not hooked up to `TableSeatSystem.m_OnSeatChanged` in the scene, or the connection is missing for many UI elements.
5.  Since there are multiple `SeatBillboard` instances across different UI systems (Main Menu, Fighting HUD, PreGame UI), they all remain at the default rotation (Seat 1 position).

# Proposed Solution
Modify `SeatBillboard.cs` to automatically subscribe to `TableSeatSystem.m_OnSeatChanged` when it starts. This ensures that any object with a `SeatBillboard` component will correctly sync its rotation to the current seat without requiring manual setup in the Unity Inspector for every instance.

# Key Assets & Context
- `Assets/MRTabletopAssets/Scripts/SeatBillboard.cs`: Component that handles UI rotation based on seat.
- `Assets/MRTabletopAssets/Scripts/Table/TableSeatSystem.cs`: System that handles player teleportation and seat state.
- `Assets/MRTabletopAssets/Scripts/Table/TableTop.cs`: Holds the current seat index.

# Implementation Steps
1.  **Modify `SeatBillboard.cs`**:
    - Add `Start()` method.
    - Find the `TableSeatSystem` instance in the scene.
    - Add a listener to `m_OnSeatChanged` that calls `RotateBillboard`.
    - Call `RotateBillboard` once during `Start` to set the initial orientation based on `TableTop.k_CurrentSeat`.
    - (Optional but recommended) Add `OnDestroy()` to remove the listener.

# Verification & Testing
1.  **Manual Test**:
    - Enter Play Mode.
    - Join the table (Seat 1). Verify UI is visible.
    - Switch to Seat 2.
    - **Verify**: The UI (Navigation Menu, HUD) should now be in front of the player at Seat 2 instead of staying at Seat 1.
2.  **Edge Case**:
    - Check if UI is correct when joining as a spectator then moving to a seat.
    - Check if UI is correct after moving the table (though the UI is at world origin, the relative positioning should still hold if the table is centered).

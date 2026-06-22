# Project Overview
- Game Title: MR Tabletop Fighting Game
- Issue: Table manipulation (grabbing and moving) is broken after adding `OVRCameraRig`. Grabbing works, but the table snaps back to its original position upon release.
- Cause: The system moves the `XR Origin` to simulate table movement while keeping the table's world coordinates fixed for network sync. It does not account for the new `OVRCameraRig`, so the user's view remains unshifted.

# Game Mechanics
## Table Manipulation
- Grabbing the table handle allows "moving" the table.
- Technically, the table resets to its origin, and the player is moved by the inverse delta of the grab.

# Key Asset & Context
- `Assets/MRTabletopAssets/Scripts/Table/TableManipulationSystem.cs`: Contains the `TableManipulator` class which handles the player-shifting logic.

# Implementation Steps
1. **Update TableManipulator Class**:
   - Add fields to track the `OVRCameraRig` and its initial transform during a grab.
2. **Modify `Start`**:
   - Find the `OVRCameraRig` in the scene (if it exists).
3. **Modify `StartSelection`**:
   - Capture the initial `localToWorldMatrix` of the `OVRCameraRig`.
4. **Modify `MovePlayer` (Awaitable)**:
   - Calculate the new world position and rotation for the `OVRCameraRig` using the same inverse transform delta applied to the `XR Origin`.
   - Apply the new transform to the `OVRCameraRig`.

# Verification & Testing
1. **Grab Test**:
   - Grab the table handle and move it to a new location.
   - Release the handle.
2. **Success Condition**:
   - The table should appear to stay at the new location (because both the `XR Origin` and `OVRCameraRig` have been shifted to the correct relative position).
   - If the view snaps back, the `OVRCameraRig` was not moved correctly.

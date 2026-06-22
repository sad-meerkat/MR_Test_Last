# Project Overview
- Game Title: MR Tabletop Fighting Game
- Issue: "Half-and-half" system (simultaneous hands and controllers) stopped working after fixing table movement.
- Diagnosis:
  1. `IndependentHandModalityManager` on `XR Origin (XR Rig)` has **Missing** references for "Hand Interactor".
  2. Manual movement of individual rigs (`XR Origin` and `OVRCameraRig`) in `TableManipulator.cs` is redundant since they share a common parent `MRInteractionSetup`.

# Game Mechanics
## Hand-Controller Modality
- Uses `IndependentHandModalityManager` to switch visibility and interactors between hands and controllers based on activity.
## Table Manipulation
- Moves the player root to simulate table movement.

# Key Asset & Context
- `IndependentHandModalityManager` component on `XR Origin (XR Rig)`.
- `TableManipulator.cs` script.
- `MRInteractionSetup` root object.

# Implementation Steps
1. **Restore Modality Manager References**:
   - Re-link the `NearFarInteractor` component from `Left Hand/Near-Far Interactor` to the `m_LeftTrack.handInteractor` field.
   - Re-link the `NearFarInteractor` component from `Right Hand/Near-Far Interactor` to the `m_RightTrack.handInteractor` field.
2. **Optimize Table Movement Logic**:
   - Modify `TableManipulator.cs` to move the **common parent** (`MRInteractionSetup`) instead of moving `XR Origin` and `OVRCameraRig` separately.
   - This ensures all rigs, input managers, and interaction managers stay perfectly aligned and move as one unit.
3. **Clean Up Logic**:
   - Remove the individual `OVRCameraRig` movement code from `TableManipulator.cs` to prevent double-transformation.

# Verification & Testing
1. **Modality Test**:
   - Pick up a controller: Hand should disappear, controller should appear.
   - Put down controller: Hand should reappear.
2. **Movement Test**:
   - Move the table: The entire player setup (view and hands) should shift correctly without breaking the modality switching.

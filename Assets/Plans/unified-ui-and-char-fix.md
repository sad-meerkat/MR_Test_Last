# Project Overview
- Goal: Fix UI orbit/follow behavior and fix character selection (Sasuke) for all seats.
- Issues:
    1. UI stays at center, doesn't follow player to seats 2, 3, 4.
    2. Host accidentally controls the Dummy instead of their selected character.
    3. Character selection only works for Seats 0 and 1.

# Implementation Steps

## Step 1: Update FightingGameManager.cs Logic
- **Description**: Fix character control linking and expand seat support.
- **Assigned role**: developer
- **Files**: `Assets/MRTabletopAssets/Games/FightingGame/Scripts/FightingGameManager.cs`
- **Action**:
    - Update `LinkLocalCharacterDelayed` to ignore objects with `isDummy = true`.
    - Refactor `SelectCharacterRpc` and `TogglePreReadyRpc` to support seats 0-3.
    - Add bounds check for `m_CharacterPrefabs` index.

## Step 2: Fix UI Orbit Positions (Z-Offsets)
- **Description**: Move UI elements from the center to the player's edge so they orbit properly.
- **Assigned role**: developer
- **Files**: Scene Objects
- **Action**:
    - Set `StartButton` Pos Z to -35.
    - Set `CharacterSelectionUI` Pos Z to -35.
    - Set `FightingHUD` Pos Z to -350.

## Step 3: Wire UI Seat Billboard Events
- **Description**: Connect the UI rotation logic to the seat change event.
- **Assigned role**: developer
- **Files**: `TableSystem` / `TableSeatSystem`
- **Action**:
    - Ensure `PreGameUI` and `FightingHUD` have `SeatBillboard`.
    - Add them to `TableSeatSystem.m_OnSeatChanged` event listeners.

# Verification & Testing
1. **Play Mode**: Sit in Seat 2 (Side seat).
2. **UI Check**: Confirm "Start" button is right in front of you.
3. **Character Selection**: Select Sasuke (Slot 1).
4. **Control Check**: Verify you are controlling Sasuke and Byakuya is the dummy.

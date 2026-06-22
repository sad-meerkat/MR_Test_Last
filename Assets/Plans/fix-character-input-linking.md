# Project Overview
- Game Title: MR Test (XR Multiplayer)
- Issue: Character does not move after recent namespace and logic refactoring. Additionally, the user wants the character to hold and swing a sword when the player does.
- Root Cause:
    1. **Strict Search Logic**: `TableCharacterInput` only links to characters where `IsSpawned && IsOwner` is true. In local testing or non-networked summoning, `IsSpawned` is false, preventing the link.
    2. **Input Lifecycle**: The movement actions in `TableCharacterInput` might not be enabled correctly if the character isn't found immediately.
    3. **Missing Component Cleanup**: There's a missing script on `XR Origin` which might be a remnant of the old `TableCharacterInput`.
    4. **Missing Sword Visual**: The character prefabs do not have a sword visual attached or a mechanism to enable/disable it in sync with the player.

# Implementation Steps

## Step 1: Relax TableCharacterInput Search Logic
- **Description**: Update the search loop to include local (non-spawned) characters during development/local testing.
- **File**: `Assets/MRTabletopAssets/Scripts/Character/TableCharacterInput.cs`
- **Logic**: 
  ```csharp
  if (!character.IsSpawned || character.IsOwner) { SetCharacter(character); break; }
  ```
- **Assigned role**: developer

## Step 2: Ensure Input Actions are Enabled
- **Description**: Explicitly enable actions in `Start()` as well as `OnEnable()` to handle cases where assignment happens after the first enable.
- **File**: `Assets/MRTabletopAssets/Scripts/Character/TableCharacterInput.cs`
- **Assigned role**: developer

## Step 3: Implement Character Sword Logic
- **Description**: Add sword visual management to `TableCharacter` and `TableCharacterInput`.
- **Files**: 
    - `Assets/MRTabletopAssets/Scripts/Character/TableCharacter.cs`: Add `m_SwordVisual` field and `SetSwordActive(bool active)` method.
    - `Assets/MRTabletopAssets/Scripts/Character/TableCharacterInput.cs`: Add `SetCharacterSwordActive(bool active)` method.
- **Assigned role**: developer

## Step 4: Scene and Prefab Setup
- **Description**: 
    - Add a sword model (e.g., `Katana.prefab`) to the right hand of the character prefabs (`ByakuyaFighter`, `SasukeFighter`).
    - Assign the added sword to the `m_SwordVisual` field in the `TableCharacter` component on the prefabs.
    - Update the `FistGesture` object in the scene to trigger `TableCharacterInput.SetCharacterSwordActive` in addition to the existing hand sword activation.
- **Assigned role**: developer

# Verification & Testing
- **Local Test**: Press Play, click "Summon" on the table. Verify the character moves with the joystick.
- **Sword Test**: Perform the Fist gesture. Verify both the player's hand and the character's hand have a sword. Swing the hand and verify the character swings its sword.
- **Multiplayer Test**: Join a session. Verify only the owned character moves and activates its sword.

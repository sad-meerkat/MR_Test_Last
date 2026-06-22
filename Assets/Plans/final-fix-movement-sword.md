# Project Overview
- Goal: Fix character movement (sluggish/non-responsive) and sword visual issues (huge scale/screen covering).
- Approach:
    1. **Code Fix**: Implement auto-scaling for the sword and robust `MovePosition` physics.
    2. **Linkage Fix**: Ensure `TableCharacterInput` always controls the active owned character.
    3. **Prefab Fix**: Standardize Rigidbody drag and speed on all characters.

# Implementation Steps

## Step 1: Code Improvements (TableCharacter.cs)
- **Description**: Add world-space scaling for the sword and switch movement to `MovePosition`.
- **File**: `Assets/MRTabletopAssets/Scripts/Character/TableCharacter.cs`
- **Logic**:
    - `SetSwordActive`: Force `lossyScale.x` to `0.05` on activation.
    - `FixedUpdate`: Use `MovePosition` instead of `linearVelocity` setting for horizontal movement.
- **Assigned role**: developer

## Step 2: Intelligent Linkage (TableCharacterInput.cs)
- **Description**: Automatically search for characters if the current one is null OR inactive.
- **File**: `Assets/MRTabletopAssets/Scripts/Character/TableCharacterInput.cs`
- **Assigned role**: developer

## Step 3: Prefab Physics Calibration
- **Description**: Update Byakuya and Sasuke prefabs with correct drag/speed.
- **Assigned role**: developer
- **Settings**: `Drag = 0.5`, `MoveSpeed = 2.0`.

# Verification & Testing
- **Summon Test**: Summon Byakuya, then Sasuke. Joystick should immediately control Sasuke (the active one).
- **Sword Test**: Perform Fist gesture. The sword in character hand should be small and correctly placed.
- **Movement Test**: Character should move briskly and stop accurately.

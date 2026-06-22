# Project Overview
- Game Title: MR Test (XR Multiplayer)
- Goal: Fix character movement issues and sword visual scaling/positioning.
- Current Status: Input logs confirm data flow, but character doesn't move and sword is huge/misplaced.

# Root Causes
1. **Movement**: `linearVelocity` manipulation is being countered by high drag (10) or conflicting with `NetworkTransform` synchronization. Using `MovePosition` with zero drag is more robust for Host-Client setups.
2. **Sword Scale**: Sasuke's `Armature` scale is 100, making a local scale of 0.01 result in a huge 1-meter sword. Hierarchy-dependent scaling is causing "screen-covering" visuals.
3. **Character Linkage**: Input handler is sometimes stuck on old/inactive characters after new ones are summoned.

# Implementation Steps

## Step 1: Robust Movement & Auto-Scaling Sword
- **File**: `Assets/MRTabletopAssets/Scripts/Character/TableCharacter.cs`
- **Changes**:
    - Update `SetSwordActive` to automatically calculate and apply a correct local scale so the sword is always ~5cm in world space (`lossyScale.x = 0.05`).
    - Change `FixedUpdate` movement logic to use `m_Rigidbody.MovePosition(m_Rigidbody.position + movement)` for smoother network syncing.
    - Set horizontal velocity to zero when moving to prevent unintended physics sliding while allowing gravity.

## Step 2: Intelligent Character Linkage
- **File**: `Assets/MRTabletopAssets/Scripts/Character/TableCharacterInput.cs`
- **Changes**:
    - Update `Update()` to automatically re-link to an active character if the current one is null, inactive, or destroyed.
    - If multiple characters exist, prioritize the one that is `activeInHierarchy` and `IsOwner`.

## Step 3: Prefab Physics Optimization
- **Description**: Standardize physics settings across all fighter prefabs.
- **Files**: `ByakuyaFighter.prefab`, `SasukeFighter.prefab`, `RobotFighter.prefab`, `KnightFighter.prefab`
- **Settings**:
    - `Rigidbody`: `Drag = 0.5`, `Angular Drag = 0.05`, `Use Gravity = True`, `Is Kinematic = False`.
    - `TableCharacter`: `Move Speed = 2.0`.

# Verification & Testing
- **Movement**: Verify the character moves smoothly and stops when the joystick is released.
- **Sword**: Perform the Fist gesture and confirm the sword is small and correctly positioned in the character's hand.
- **Multiplayer**: Confirm Host and Client can both move their respective characters.

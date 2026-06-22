# Project Overview
- Goal: Fix character jump force ignoring inspector values, improve movement speed for tabletop scale, and ensure skills work in both local and networked play.
- Root Cause: 
    1. `FighterController.cs` has a hardcoded `5f` jump force in `JumpServerRpc`.
    2. RPC-based actions fail when the object is not spawned in Netcode.
    3. Movement speed is too high relative to character scale (0.07).

# Implementation Steps

## Step 1: Fix FighterController.cs Logic
- **Description**: Replace hardcoded values and add local execution fallbacks for non-spawned objects.
- **File**: `Assets/MRTabletopAssets/Games/FightingGame/Scripts/FighterController.cs`
- **Changes**:
    - In `JumpServerRpc`, replace `5f` with `m_JumpForce`.
    - In `Jump()`, if `!IsSpawned`, execute jump logic locally (AddForce and Animator trigger).
    - In `PerformSwordSwing()`, if `!IsSpawned`, execute attack logic locally (Animator trigger and damage check).

## Step 2: Fix TableCharacter.cs Defaults
- **Description**: Set more sensible default values for tabletop scale.
- **File**: `Assets/MRTabletopAssets/Scripts/Character/TableCharacter.cs`
- **Changes**: 
    - Set default `m_MoveSpeed` to `0.5f`.
    - Set default `m_JumpForce` to `1.5f`.

## Step 3: Final Sasuke Sword Correction
- **Description**: Correct rotation and scale of the sword in the Sasuke prefab.
- **Assigned role**: developer
- **Sub-tasks**:
    1. Set `Character_Katana` Local Rotation to `(180, 90, 0)` in `SasukeFighter.prefab`.
    2. Set `Character_Katana` Local Scale to `(0.005, 0.005, 0.005)`.

# Verification & Testing
- **Local Test**: Press Play (without NetworkManager). Confirm movement is slower, jump is lower (and adjustable), and sword swing works.
- **Inspector Test**: Change `Jump Force` to `0.1` and verify the character barely hops.
- **Multiplayer Test**: Join session. Confirm movement and skills still sync correctly.

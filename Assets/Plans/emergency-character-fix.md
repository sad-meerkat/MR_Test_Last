# Project Overview
- Goal: Emergency fix for character movement, broken skills, and mis-scaled sword visuals.
- Issue: Movement logic conflict, incomplete skill linkage, and incorrect prefab scaling.

# Implementation Steps

## Step 1: Fix Movement Logic Conflict
- **File**: `Assets/MRTabletopAssets/Scripts/Character/TableCharacter.cs`
- **Change**: Remove `m_Rigidbody.linearVelocity = new Vector3(0, m_Rigidbody.linearVelocity.y, 0);` from inside the movement `if` block. This line was nullifying the `MovePosition` call.
- **Assigned role**: developer

## Step 2: Link Skills to Character Attack System
- **File**: `Assets/MRTabletopAssets/Games/FightingGame/Scripts/FighterController.cs`
- **Change**: Override `CastSkill()` to call `AttackServerRpc()`. This ensures that gestures calling `CastSkill()` actually trigger the damage-dealing attack logic.
- **Assigned role**: developer

## Step 3: Fix Sword Scaling and Positioning
- **Description**: Re-adjust the `Character_Katana` transform in the prefabs to be appropriate sizes.
- **Sub-tasks**:
    1. **ByakuyaFighter**: Set sword scale to `(0.3, 0.3, 0.3)`.
    2. **SasukeFighter**: Set sword scale to `(0.003, 0.003, 0.003)`.
    3. **Player Hand**: Check and set `Katana_Visual` under `R_Wrist` to a safer scale like `(0.03, 0.03, 0.03)`.
- **Assigned role**: developer

# Verification & Testing
- **Movement**: Verify character moves with joystick.
- **Skills**: Verify Fist/Skill gestures trigger the attack animation and damage logs.
- **Visuals**: Verify the sword is small enough to not block the VR view.

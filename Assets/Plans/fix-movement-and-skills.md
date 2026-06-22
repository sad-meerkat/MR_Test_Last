# Project Overview
- Game Title: MR Test (XR Multiplayer)
- Issues:
    1. Character doesn't move via joystick.
    2. Skills are not functioning as expected.
    3. Character sword is mis-scaled or mis-positioned (according to user report).
- Root Causes:
    1. **Movement Conflict**: `TableCharacter.cs` was using `MovePosition` followed immediately by `linearVelocity = 0` on the same frame, which cancelled the movement.
    2. **Skill Logic Mismatch**: `FighterController` uses `AttackServerRpc` for damage, but `TableCharacterInput` calls `CastSkill`, which was only triggering a local animator state and spawning a local effect.
    3. **Sword Visibility**: The user reports the sword is "too big and covers the screen," suggesting the player's hand sword or character sword is mis-scaled in the current setup.

# Implementation Steps

## Step 1: Fix Movement and Stop Conflicting Logic
- **Description**: Simplify movement to use direct velocity manipulation for better responsiveness and consistency with physics.
- **File**: `Assets/MRTabletopAssets/Scripts/Character/TableCharacter.cs`
- **Change**: 
    - Use `m_Rigidbody.linearVelocity` for horizontal movement.
    - Remove the redundant `linearVelocity = 0` call inside the movement block.
    - Preserve gravity (Y velocity).

## Step 2: Fix Skill Functionality
- **Description**: Ensure that when a skill is "cast" via gesture, it triggers the character's attack logic (damage and networking).
- **File**: `Assets/MRTabletopAssets/Games/FightingGame/Scripts/FighterController.cs`
- **Change**: Override `CastSkill()` to call `AttackServerRpc()`.

## Step 3: Refine Sword Scaling and Parenting
- **Description**: Re-verify and adjust the character sword visuals to ensure they are correctly sized and positioned.
- **Assigned role**: developer
- **Sub-tasks**:
    1. Check `Katana_Visual` under `R_Wrist` scale (player sword).
    2. Check `Character_Katana` under character hand bones.
    3. Adjust `SasukeFighter` sword scale to be even smaller if it's still too large.

# Verification & Testing
- **Movement**: Verify the character moves smoothly and stops when input is released.
- **Skills**: Verify that gestures trigger the attack animation AND logic (check console for attack logs).
- **Visuals**: Verify the sword is appropriately sized in both the player's hand and the character's hand.

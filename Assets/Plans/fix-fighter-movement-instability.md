# Project Overview
- Game Title: MR Test (XR Multiplayer)
- Issue: Characters accelerate uncontrollably and move/look in weird directions.
- Cause: "Apply Root Motion" is enabled on characters, causing the animations to move the object at the same time as the movement script. Manual Rigidbody movement is also being called in `Update` instead of `FixedUpdate`.

# Game Mechanics
- Characters are controlled by joystick input via `FighterController.cs`.
- Movement should be constant speed and rotation should face the direction of travel.

# Key Asset & Context
- `ByakuyaFighter.prefab` & `SasukeFighter.prefab`: Have `Apply Root Motion` enabled.
- `FighterController.cs`: Handles movement and rotation logic.

# Implementation Steps

## Step 1: Disable Root Motion on Prefabs
- **Description**: Set `Animator.applyRootMotion = false` on the `ByakuyaFighter` and `SasukeFighter` prefabs. This ensures the animations don't move the character physics-wise.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes

## Step 2: Refactor FighterController.cs
- **Description**: 
    1. Move movement and rotation logic from `Update` to `FixedUpdate` for better physics stability.
    2. Store the input in `Update` and apply it in `FixedUpdate`.
    3. Use `m_Rigidbody.position` as the base for movement calculation.
    4. Implement smooth rotation (Slerp) instead of hard snapping to the direction.
    5. Ensure the character only rotates when there is significant movement input.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes

## Step 3: Add Rigidbody Drag
- **Description**: Increase linear drag on the character Rigidbodies to prevent sliding after stopping input, providing a "snappier" feel.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes

# Verification & Testing
- **Speed Test**: Verify the character moves at a constant speed proportional to the joystick deflection.
- **Rotation Test**: Verify the character turns smoothly to face the direction of movement.
- **Drift Test**: Verify the character stops immediately when the joystick is released and doesn't drift or "accelerate" over time.

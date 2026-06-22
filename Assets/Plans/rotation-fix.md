# Project Overview
- Game Title: MR Tabletop Fighting Game
- Issue: Character rotates continuously (360 degrees) like a music box even without input.
- Diagnosis: 
    1. Feedback loop in `FixedUpdate`: If `Camera.main` is missing, the code uses `transform.forward` as the movement basis. Any horizontal input (including controller drift) causes the character to rotate towards its own "right", which moves the "right" vector, causing further rotation—a classic feedback loop.
    2. Lack of angular velocity reset: When there is no input, linear velocity is stopped but angular velocity is not, allowing residual spinning.

# Game Mechanics
## Core Gameplay Loop
- Unchanged.
## Controls and Input Methods
- Character movement and rotation should be relative to the Camera or a fixed world reference, not the character's own local space.

# Key Asset & Context
- `TableCharacter.cs`: Contains the movement and rotation logic in `FixedUpdate`.

# Implementation Steps
## Step 1: Fix Rotation Feedback Loop and Angular Velocity
- **Description**: Modify `TableCharacter.cs` to provide a stable fallback for movement direction and ensure angular velocity is zeroed out to prevent unintended spinning.
- **Assigned role**: developer
- **Implementation**:
    - Update `FixedUpdate` in `TableCharacter.cs`.
    - Change fallback for `camForward` and `camRight` from `transform.forward` to `Vector3.forward`.
    - Explicitly set `m_Rigidbody.angularVelocity = Vector3.zero` in both the movement block and the idle block to ensure the character doesn't spin due to physics or drift.
- **Dependencies**: None
- **Parallelizable**: Yes

# Verification & Testing
- **Manual Test**: 
    1. Run the game.
    2. Do not touch the controller sticks.
    3. Observe the character; it should remain facing its last direction and not spin.
    4. Move the stick slightly; it should rotate to that direction and stop once the stick is released (or drifts into the deadzone).
- **Edge Case**: Unplug/Disconnect the camera (simulate null `Camera.main`) and verify movement still works in a fixed world direction without spinning.

# Project Overview
- Game Title: MR Test (XR Multiplayer)
- Issue: Characters look in "strange" directions during movement.
- Goal: Fix character orientation to be intuitive (Camera-Relative) and correct any model-specific rotation offsets.

# Game Mechanics
- Movement: Joystick input should move the character relative to the player's current view (Main Camera).
- Orientation: Characters should face their movement direction smoothly.

# Key Asset & Context
- `TableCharacter.cs`: General character movement logic.
- `FighterController.cs`: Fighter-specific network movement logic.
- `Camera.main`: Used as the reference for directional input.

# Implementation Steps

## Step 1: Update TableCharacter.cs
- **Description**: 
    1. Add `m_RotationOffset` field to handle misaligned models.
    2. Update `FixedUpdate` to calculate movement relative to `Camera.main.transform.forward` (projected on XZ plane).
    3. Increase `m_RotationSpeed` for better responsiveness.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes

## Step 2: Update FighterController.cs
- **Description**: 
    1. Add `m_RotationOffset` field.
    2. Update `FixedUpdate` to use camera-relative direction instead of table-relative direction.
    3. Ensure rotation uses the calculated world direction with the offset applied.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes

## Step 3: Configure Prefabs
- **Description**: Check if the characters face the correct way. If they look 90 or 180 degrees away from the movement, adjust the new `m_RotationOffset` parameter on the prefabs.
- **Assigned role**: developer
- **Dependencies**: Steps 1 & 2
- **Parallelizable**: No

# Verification & Testing
- **Visual Test**: Move the joystick in various directions while standing at different sides of the table. The character should always move "away" from the player when pushing UP.
- **Rotation Test**: Verify the character looks forward relative to its movement.
- **Responsiveness Test**: Verify characters turn quickly enough to feel snappy.

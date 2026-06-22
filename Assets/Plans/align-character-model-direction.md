# Project Overview
- Game Title: MR Test (XR Multiplayer)
- Issue: Character models are rotated 90 degrees incorrectly relative to their movement direction.
- Goal: Apply a +90 degree rotation offset to align the character visuals with their movement path.

# Key Asset & Context
- `ByakuyaFighter.prefab`: Currently `m_RotationOffset = 0`.
- `SasukeFighter.prefab`: Currently `m_RotationOffset = 0`.
- `TableCharacter.prefab`: Currently `m_RotationOffset = 0`.
- The user reports that moving UP results in the character looking LEFT.

# Implementation Steps

## Step 1: Update RotationOffset on Prefabs
- **Description**: Set `m_RotationOffset` to `90` on the `FighterController` (for Byakuya/Sasuke) and `TableCharacter` components in their respective prefabs.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes

# Verification & Testing
- **Movement Test**: Move the joystick UP. The character should now move away from the camera and face forward (the direction of movement).
- **Direction Test**: Check all 4 cardinal directions to ensure the 90-degree offset fixed the mismatch everywhere.

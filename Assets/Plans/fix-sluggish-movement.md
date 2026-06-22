# Project Overview
- Issue: Character movement is still not working despite input logs being captured.
- Findings:
    1. **Prefab Mismatch**: `ByakuyaFighter` and `SasukeFighter` prefabs have `m_MoveSpeed = 0.5` and `Rigidbody.drag = 10`. This combination is likely preventing visible movement.
    2. **Input Flow**: `TableCharacterInput` is successfully calling `Move(input)` on the character.
    3. **Physics Logic**: Direct velocity manipulation is used, but high drag might be counteracting it.
    4. **Camera Dependency**: Movement direction depends on `Camera.main`. If `Camera.main` is not the VR camera, movement might be erratic.

# Implementation Steps

## Step 1: Fix Prefab Physics Settings
- **Description**: Update the character prefabs to have sensible speed and drag values.
- **Assigned role**: developer
- **Sub-tasks**:
    1. **ByakuyaFighter**: Set `m_MoveSpeed` to 2.0, `Rigidbody.drag` to 0.
    2. **SasukeFighter**: Set `m_MoveSpeed` to 2.0, `Rigidbody.drag` to 0.
    3. Ensure `Rigidbody.useGravity` is true and `isKinematic` is false.

## Step 2: Improve TableCharacter Movement Logic
- **Description**: Make the movement logic more robust against camera issues and ensure it works correctly on the Host.
- **File**: `Assets/MRTabletopAssets/Scripts/Character/TableCharacter.cs`
- **Changes**:
    - Add a fallback for the camera: if `Camera.main` is null, use the object's own forward.
    - Improve the direction calculation to prevent zero-vector issues.
    - Use `m_Rigidbody.linearVelocity` but preserve gravity properly.
    - Ensure rotation only happens if movement occurs.

## Step 3: Scene Cleanup
- **Description**: Resolve the "2 audio listeners" warning to ensure `Camera.main` behaves predictably.
- **Assigned role**: developer

# Verification & Testing
- **Movement Test**: Press Play, click "Summon". Move joystick.
- **Observation**: Check if the "cyan debug ray" appears and points in the correct direction.
- **Drag Check**: Confirm the character doesn't feel "stuck" or "sluggish".

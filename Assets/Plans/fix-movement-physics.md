# Project Overview
- Issue: Character movement stopped working after recent updates, although input signals are being received and sword logic is functional.
- Investigation Findings:
    1. Input is definitely reaching `TableCharacterInput` and being forwarded to the character (`ByakuyaFighter`).
    2. Character is local (`IsSpawned=False`).
    3. Movement logic uses `Rigidbody.MovePosition`, which can be slow or blocked by friction/collisions if speed is too low.
    4. Current `m_MoveSpeed` is 0.5 - 1.0, which might be insufficient for the physics interaction or model scale.
    5. The velocity clearing logic might be too aggressive.

# Implementation Steps

## Step 1: Improve Movement Physics and Speed
- **Description**: Increase the movement speed and make the physics more robust.
- **File**: `Assets/MRTabletopAssets/Scripts/Character/TableCharacter.cs`
- **Changes**:
    - Increase default `m_MoveSpeed` to 2.0.
    - Change `m_Rigidbody.MovePosition` logic to potentially use `linearVelocity` for more consistent movement on non-kinematic RBs, or ensure `MovePosition` is used correctly with interpolation.
    - Actually, I will use a more direct velocity-based movement for better responsiveness:
      ```csharp
      Vector3 moveVel = direction * m_MoveSpeed;
      m_Rigidbody.linearVelocity = new Vector3(moveVel.x, m_Rigidbody.linearVelocity.y, moveVel.z);
      ```
    - Ensure `m_Rigidbody.interpolation` is set to `Interpolate` for smooth movement.

## Step 2: Auto-Wake Rigidbody
- **Description**: Ensure the Rigidbody doesn't go to sleep.
- **File**: `Assets/MRTabletopAssets/Scripts/Character/TableCharacter.cs`
- **Change**: Call `m_Rigidbody.WakeUp()` when receiving input.

## Step 3: Debug Visualization
- **Description**: Add a visual debug ray to see the movement direction in the editor.
- **File**: `Assets/MRTabletopAssets/Scripts/Character/TableCharacter.cs`

# Verification & Testing
- **Movement Test**: Press Play, summon character, move joystick. Verify the character moves at a reasonable pace.
- **Console Check**: Check `[TableCharacterInput] Sending Input` logs to confirm data flow.

# Project Overview
- Issue: After previous changes, the character no longer moves with the controller. Additionally, the sword is huge ("covers the screen") and doesn't appear at the character's position.
- Root Cause Analysis:
    1. **Sword Issue**: 
        - Scale: Sasuke's armature is 100x. If I set the sword local scale to `0.5`, it becomes 3.5m tall. I set it to `0.01` in the last fix, but maybe it didn't stick or Byakuya's `0.5` is still too big for the tabletop scale.
        - Position: "Not at character position" suggests it might be getting detached or its parent bone is not where we think it is.
        - Visibility: If the sword has a huge collider, it might be pushing the character away or causing physics glitches.
    2. **Movement Issue**:
        - `linearVelocity` vs `velocity`: Although Unity 6 supports `linearVelocity`, there might be interactions with `NetworkTransform` that make it stutter or stop.
        - Camera Fallback: If `Camera.main` points to a secondary camera (like a scene view camera or a leftover Main Camera), movement might be calculated in a direction the player doesn't see.

# Implementation Steps

## Step 1: Robuster Character Sword Setup
- **Description**: Fix sword scale, collision, and parenting once and for all.
- **Assigned role**: developer
- **Sub-tasks**:
    1. **ByakuyaFighter**: Set `Character_Katana` local scale to `0.1` (was 0.5).
    2. **SasukeFighter**: Set `Character_Katana` local scale to `0.001` (was 0.01).
    3. **Collision**: Find the `BoxCollider` or `MeshCollider` on the Katana (under `Character_Katana`) and set `isTrigger = true` or remove it.
    4. **Hierarchy**: Re-verify that `Character_Katana` is a child of the hand bone in the prefab.

## Step 2: Reliable Movement Logic
- **Description**: Simplify movement and add redundant camera checks.
- **File**: `Assets/MRTabletopAssets/Scripts/Character/TableCharacter.cs`
- **Changes**:
    - Use `m_Rigidbody.velocity` (for compatibility) as well as `linearVelocity`.
    - Ensure `Camera.main` is actually the **VR Camera** by checking tags or presence of `CenterEyeAnchor`.
    - Increase `m_MoveSpeed` to 3.0 to ensure it's visible.
    - Add a `Debug.Log` in `FixedUpdate` that triggers every 1.0s to confirm movement is actually happening.

## Step 3: Scene Cleanup (Cameras)
- **Description**: Ensure only the VR camera is tagged as "MainCamera".
- **Assigned role**: developer

# Verification & Testing
- **Summon Test**: Summon character. Verify sword is small and attached to the hand.
- **Movement Test**: Move joystick. Watch for console logs: `[TableCharacter] Moving at: (X, Y, Z)`.
- **Collider Test**: Ensure character doesn't fly away when sword is activated.

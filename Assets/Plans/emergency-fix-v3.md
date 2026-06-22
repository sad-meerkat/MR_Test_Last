# Project Overview
- Goal: Fix broken character movement and restore the "Senbonzakura" (Thumbs Up) skill.
- Issues:
    1. Movement code conflict preventing any motion.
    2. Missing events on `ThumbsUpGesture`.
    3. Mis-scaled "Cherrykatana" effect covering the screen.

# Implementation Steps

## Step 1: Fix Movement Code Conflict
- **File**: `Assets/MRTabletopAssets/Scripts/Character/TableCharacter.cs`
- **Change**: Remove the line `m_Rigidbody.linearVelocity = new Vector3(0, m_Rigidbody.linearVelocity.y, 0);` from the movement block. This was nullifying the movement each frame.

## Step 2: Restore Thumbs Up Skill Logic
- **Action**: In the scene, find `ThumbsUpGesture`.
- **Change**: 
    - Add listener to `gesturePerformed` calling `TableCharacterInput.OnGesturePerformed`.
    - Add listener to `gesturePerformed` calling `Anchor_Cherrykatana.SetActive(true)`.
    - Add listener to `gestureEnded` calling `Anchor_Cherrykatana.SetActive(false)`.

## Step 3: Fix Senbonzakura (Cherrykatana) Scale
- **Action**: Find `MRInteractionSetup/XR Origin (XR Rig)/Camera Offset/Right Hand/RightHandQuestVisual/R_Wrist/Anchor_Cherrykatana`.
- **Change**: Set **Local Scale** to `(0.05, 0.05, 0.05)` to match the hand and prevent it from covering the screen.

## Step 4: Link Skill to Character Combat
- **File**: `Assets/MRTabletopAssets/Games/FightingGame/Scripts/FighterController.cs`
- **Change**: Override `CastSkill()` to call `AttackServerRpc()`. This ensures damage logic is triggered when the gesture is performed.

# Verification & Testing
- **Movement**: Verify character moves correctly.
- **Skill**: Perform Thumbs Up. Verify the "Cherrykatana" effect appears at a reasonable size and the character performs the attack.
- **Combat**: Confirm the attack causes damage (check logs).

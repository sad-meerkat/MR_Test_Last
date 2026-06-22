# Project Overview
- Goal: Absolute final fix for character movement, Senbonzakura (Thumbs Up) skill, and sword scaling.
- No more "half-finished" work. Fixing the root causes identified.

# Implementation Steps

## Step 1: Fix Movement Code (Revert bug)
- **File**: `Assets/MRTabletopAssets/Scripts/Character/TableCharacter.cs`
- **Change**: Remove the line that resets `linearVelocity` to zero while moving. This will allow the character to actually move.

## Step 2: Restore Thumbs Up Skill (Senbonzakura)
- **Action**: In the scene, re-wire `ThumbsUpGesture`.
- **Details**: 
    - Connect `gesturePerformed` to `TableCharacterInput.OnGesturePerformed`.
    - Connect `gesturePerformed` to `Anchor_Cherrykatana.SetActive(true)`.
    - Connect `gestureEnded` to `Anchor_Cherrykatana.SetActive(false)`.

## Step 3: Fix Huge Visuals
- **Action**: Find `Anchor_Cherrykatana` in the scene.
- **Change**: Set **Local Scale** to `(0.05, 0.05, 0.05)`.
- **Sub-task**: Ensure `Katana_Visual` (player sword) and `Character_Katana` (character sword) are also scaled correctly.

## Step 4: Fix Skill Damage Logic
- **File**: `Assets/MRTabletopAssets/Games/FightingGame/Scripts/FighterController.cs`
- **Change**: Ensure `CastSkill()` triggers the actual `AttackServerRpc()` so skills deal damage.

# Verification & Testing
- **Movement**: Verify character follows joystick without stopping.
- **Senbonzakura**: Verify Thumbs Up shows the effect at a normal size.
- **End-to-End**: Confirm no more huge swords blocking the view.

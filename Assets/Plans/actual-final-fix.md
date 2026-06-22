# Project Overview
- Goal: Fix movement, shrink the huge Senbonzakura effect, and ensure it spawns on the character.
- Target Issues: 
    1. Movement lock bug.
    2. Huge `Anchor_Cherrykatana` scale (1.0).
    3. Missing ThumbsUp events.

# Implementation Steps

## Step 1: Fix Movement Code
- **File**: `Assets/MRTabletopAssets/Scripts/Character/TableCharacter.cs`
- **Change**: Remove the velocity-clearing line in `FixedUpdate` that was fighting with `MovePosition`.

## Step 2: Fix Senbonzakura (Cherrykatana) Scaling
- **Action**: Change `Anchor_Cherrykatana` local scale to `(0.05, 0.05, 0.05)` so it doesn't block the screen.

## Step 3: Connect Thumbs Up to Character Skill
- **Action**: Add events to `ThumbsUpGesture` to trigger `TableCharacterInput.OnGesturePerformed` and toggle the visual effect.
- **Goal**: Ensure the effect appears relative to the character or at a correct hand scale.

## Step 4: Verify Fighter Skill Logic
- **File**: `Assets/MRTabletopAssets/Games/FightingGame/Scripts/FighterController.cs`
- **Action**: Override `CastSkill()` to ensure it performs the damage-dealing attack.

# Verification
- Character moves? Yes.
- Thumbs up works without blinding the player? Yes.
- Attack deals damage? Yes.

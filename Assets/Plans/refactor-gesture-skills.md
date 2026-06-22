# Refactoring Gesture Projectile System

The goal is to separate player-hand visual logic from character-centric skill logic into two distinct scripts. This improves modularity and ensures skills are fired from the character's perspective/direction rather than the player's view direction.

# Project Overview
- **Game Title**: MR Tabletop Fighting Game
- **High-Level Concept**: Refactor the gesture-triggered skill system to separate local hand visuals from character-based skill execution.
- **Players**: Multiplayer (Unity Netcode)
- **Render Pipeline**: URP

# Game Mechanics
## Core Gameplay Loop
- Player performs a hand gesture.
- Local hand visual appears (feedback for the player).
- The player's controlled character on the tabletop fires a skill in its own forward direction.
- The skill applies damage to the opponent.

# Key Asset & Context
- `GestureProjectileLauncher.cs`: Will be refactored to handle only player-hand visuals.
- `CharacterGestureSkillManager.cs`: (NEW) Will handle character-centric skill spawning, networking, and damage.
- `TableCharacterInput.cs`: Used to find the player's controlled character.

# Implementation Steps

1. **Refactor `GestureProjectileLauncher.cs`**
    - **Description**: Simplify the script to only handle hand gesture detection and local/remote hand visual spawning.
    - **Assigned role**: developer
    - **Dependencies**: None
    - **Parallelizable**: No
    - **Status**: Completed

2. **Refactor `FireballComboLauncher.cs`**
    - **Description**: Simplify the script to only handle hand combo visual feedback and expose an event.
    - **Assigned role**: developer
    - **Dependencies**: None
    - **Parallelizable**: No
    - **Status**: Completed

3. **Update `CharacterGestureSkillManager.cs`**
    - **Description**: Update the script to listen to both `GestureProjectileLauncher` and `FireballComboLauncher`.
    - **Assigned role**: developer
    - **Dependencies**: Steps 1 & 2
    - **Parallelizable**: No
    - **Status**: Completed

4. **Wire Scripts in Scene**
    - **Description**: Attach the new script to the same managers (e.g., `CherryManager`, `FireBallManager`) and configure properties.
    - **Assigned role**: developer
    - **Dependencies**: Step 3
    - **Parallelizable**: No
    - **Status**: Pending

# Verification & Testing
- **Gesture Recognition**: Verify the hand visual appears at the player's hand.
- **Character Spawning**: Verify the skill effect and capsule appear at the character's chest/hand.
- **Direction Test**: Rotate the character; verify the skill fires in the character's new forward direction, even if the player is looking elsewhere.
- **Damage Test**: Verify the opponent character takes damage when hit by the character's projectile.

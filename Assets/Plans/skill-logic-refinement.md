# Project Overview
- Game Title: Tabletop AR Fighting Game
- High-Level Concept: Miniature characters on a table performing skills triggered by player hand gestures.
- Render Pipeline: URP

# Game Mechanics
## Core Gameplay Loop
- Player performs hand gestures.
- Skill launchers detect gestures and trigger character skills.
- Skills involve a visual effect at the player's hand and a character-scale effect + invisible damage projectile from the character's front.

# Key Asset & Context
- `FireballComboLauncher.cs`: Sasuke's skill launcher.
- `ComboBarrageLauncher.cs`: Byakuya's Bankai launcher.
- `GestureProjectileLauncher.cs`: Byakuya's Shikai launcher (Cherry petals).
- `TableCharacterInput.cs`: Provides reference to the controlled tabletop character.
- `ProjectileDamage.cs`: Handles damage logic on projectiles.

# Implementation Steps
## Step 1: Update FireballComboLauncher.cs
- **Description**: Add fields for character visual effect and invisible damage projectile. Modify `LaunchFireball()` to spawn Hand Visual (Player Scale), Character Visual (Character Scale), and Invisible Projectile (Character Scale + Damage).
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes

## Step 2: Update ComboBarrageLauncher.cs
- **Description**: Add fields for character visual effect and invisible damage projectile. Modify `FireBarrage()` to spawn Hand Visuals, Character Visuals, and Invisible Damage Projectiles for each blade in the barrage.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes

## Step 3: Update GestureProjectileLauncher.cs
- **Description**: Implement dual-spawning logic (Hand vs Character). Add fields for `m_SpawnAtCharacter`, `m_CharacterEffectPrefab`, and `m_InvisibleProjectilePrefab`. Update `SpawnProjectileServerRpc` to handle spawning multiple objects.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes

## Step 4: Verification
- **Description**: Ensure all three launchers correctly spawn the intended objects at both hand and character positions. Verify that character-scale objects are correctly sized and that damage is dealt to opponents.
- **Assigned role**: explorer
- **Dependencies**: Steps 1, 2, 3
- **Parallelizable**: No

# Verification & Testing
- Use Play Mode to trigger each gesture.
- Check scene hierarchy for spawned objects:
    - 1st person visual at hand.
    - Small visual at character.
    - Invisible projectile at character.
- Verify scaling: The objects from the character should be tiny (matching the character's `localScale`).
- Verify damage: Opponent's health should decrease when hit by the invisible projectile.

# Project Overview
- Game Title: Tabletop AR Fighting Game
- High-Level Concept: Perform hand gestures to trigger skills for miniature characters on a tabletop.
- Players: Single player (with Networked characters)
- Inspiration: Bleach (Byakuya), Naruto (Sasuke)
- Target Platform: Android (Quest 3 / AR)
- Screen Orientation: Landscape
- Render Pipeline: URP

# Game Mechanics
## Core Gameplay Loop
- Player performs hand gestures.
- Gestures are recognized by `FireballComboLauncher` (Sasuke) or `ComboBarrageLauncher` (Byakuya).
- Upon successful combo, a projectile is launched towards the target or in the view direction.
- Projectiles deal damage to `FighterHealth`.

## Controls and Input Methods
- XR Hands for gesture detection.
- Tabletop characters controlled via Input System or Gestures.

# UI
- `ComboDisplay` shows step progress and cooldowns.
- `FightingHUDManager` handles health bars.

# Key Asset & Context
- `FireballComboLauncher.cs`: Sasuke's skill launcher.
- `ComboBarrageLauncher.cs`: Byakuya's skill launcher.
- `TableCharacterInput.cs`: Provides reference to the controlled character.
- `ProjectileDamage.cs`: Handles damage on collision.
- `FireBall.prefab`: Current Sasuke projectile (pink/Legacy).
- `CherryKatana.prefab`: Current Byakuya projectile.

# Implementation Steps
## Step 1: Update Launchers for Dual Spawning
- **Description**: Modify `FireballComboLauncher.cs` and `ComboBarrageLauncher.cs` to spawn two objects upon successful combo: one at the player's hand (original scale) and one at the tabletop character (character scale).
- **Assigned role**: developer
- **Dependencies**: None
- **Files**: `Assets/MRTabletopAssets/Scripts/Character/FireballComboLauncher.cs`, `Assets/MRTabletopAssets/Scripts/Character/ComboBarrageLauncher.cs`

## Step 2: Implement "Logic Projectile" (Small Capsule)
- **Description**: Add a field to the launchers for a `m_CharacterProjectilePrefab`. If assigned, this prefab will be used for the character-centric launch. If unassigned, it will fallback to the existing effect prefab (scaled down).
- **Assigned role**: developer
- **Dependencies**: Step 1
- **Files**: Same as Step 1.

## Step 3: Configure Scene and Prefabs
- **Description**: Assign the hand-held effect and the character projectile in the inspector for both launchers. For the character projectile, use a small capsule prefab with `ProjectileDamage`.
- **Assigned role**: developer
- **Dependencies**: Step 2

## Step 2: Implement Scaling and Offset Logic
- **Description**: Add fields to control the spawn offset relative to the character and a scale multiplier to match the tabletop scale.
- **Assigned role**: developer
- **Dependencies**: Step 1
- **Files**: Same as Step 1.

## Step 3: Create a "Small Capsule" Projectile (Optional/Alternative)
- **Description**: Provide a simple capsule-based projectile prefab that uses URP shaders and has `ProjectileDamage` attached. This addresses the "pink" issue and the user's specific "small capsule" request.
- **Assigned role**: developer
- **Dependencies**: None
- **Files**: Create `Assets/MRTabletopAssets/Prefabs/Skills/CapsuleProjectile.prefab`.

## Step 4: Fix/Update Visuals for Existing Projectiles
- **Description**: Convert Legacy shaders to URP shaders for `FireBall` and `CherryKatana` if they are still used.
- **Assigned role**: developer
- **Dependencies**: None

# Verification & Testing
- Perform the gesture sequence (Fist -> Open Palm -> Point At).
- Verify the projectile spawns in front of the character (Sasuke).
- Verify the projectile is scaled down to match the character.
- Verify the projectile deals damage to the opponent.
- Repeat for Byakuya's barrage.

# Project Overview
- Game Title: MR Test (XR Multiplayer)
- High-Level Concept: Tabletop MR character movement and interaction.
- Players: Multiplayer (using Netcode for GameObjects)
- Target Platform: Android (Quest)
- Render Pipeline: URP
- Input System: New Input System + XRI

# Game Mechanics
## Core Gameplay Loop
- Summon a character on a virtual table.
- Move the character using controller joysticks.
- Perform gestures to cast skills.

# Key Asset & Context
- `TableCharacterInput.cs`: Handles input from XRI actions and passes it to the character.
- `TableCharacter.cs`: General character movement.
- `FighterController.cs`: Character movement specifically for Sasuke/Byakuya fighters.
- `TableCharacterSummoner.cs`: Spawns the general character.
- `FightingGameManager.cs`: Spawns the fighters.

# Implementation Steps
## Step 1: Fix FighterController.cs (Input & Animation)
- **Description**: 
    1. Add `OnEnable`/`OnDisable` to enable/disable `m_MoveAction` and `m_AttackAction`.
    2. Add `Animator` reference and update "Speed" float parameter based on movement.
    3. Increase `m_MoveSpeed` to `1.0f`.
    4. Fix `IsOwner` check to allow local-only testing: `if (IsSpawned && !IsOwner) return;`.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes

## Step 2: Refine TableCharacter.cs (Ownership & Logging)
- **Description**: 
    1. Fix `IsOwner` check to allow local-only testing: `if (IsSpawned && !IsOwner) return;`.
    2. Ensure `m_TableTransform` fallback logic is robust.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes

## Step 3: Scene & Prefab Validation
- **Description**: 
    1. Verify `CapsuleCollider` Center Y is set to `1.0` on all character prefabs.
    2. Ensure `Rigidbody` has `FreezeRotationX` and `FreezeRotationZ` checked.
    3. Confirm `FightingGameManager` has prefabs assigned.
- **Assigned role**: explorer
- **Dependencies**: None
- **Parallelizable**: Yes

# Verification & Testing
- **Input Check**: Observe console logs for `[FighterController] Input detected`.
- **Ownership Check**: Confirm `IsSpawned` and `IsOwner` status in logs.
- **Movement Check**: Verify character moves and rotates in Play Mode.
- **Animation Check**: Confirm "Speed" parameter changes in the Animator window.

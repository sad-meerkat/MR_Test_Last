# Project Overview
- Game Title: MR Tabletop Fighting Game
- Goal: Implement damage reduction when hitting with sword swings or skills.
- Players: 2-player local multiplayer (NGO)

# Game Mechanics
## Core Gameplay Loop
- Players attack each other using physical hand motions (swings) or gestures (skills).
- Hits reduce the opponent's health.
- Health reaches 0, the fighter dies, and the game state resets.

## Controls and Input Methods
- **Sword Swing**: Detected via `HandSwordSkill` (velocity) -> calls `PerformSwordSwing`.
- **Skills**: Detected via Hand Gestures -> calls `CastSkill`.

# Key Asset & Context
- `FighterController.cs`: Handles attack logic and RPCs.
- `FighterHealth.cs`: Handles health synchronization and hit reactions.
- `TableCharacter.cs`: Base class for characters.

# Implementation Steps

## Step 1: Fix Layer for Collision Detection [COMPLETED]
- **Description**: Characters need to be on a consistent layer (e.g., Default or a new "Fighter" layer) so `Physics.OverlapSphere` can find them.
- **Assigned role**: developer
- **Implementation**:
    - Ensure `m_FighterLayer` in `FighterController` prefabs is set to include the layer the characters are on.

## Step 2: Implement Hit Reactions in `FighterHealth` [COMPLETED]
- **Description**: Synchronize the "hit" animation and visual feedback across all clients.
- **Assigned role**: developer
- **Implementation**:
    - **Modify `FighterHealth.cs`**:
        - Add `[Rpc(SendTo.Everyone)] void PlayHitReactionClientRpc()`.
        - In `PlayHitReactionClientRpc()`: Set the `hit` trigger on the animator.
        - In `TakeDamage()` (Server): Call `PlayHitReactionClientRpc()`.

## Step 3: Implement Skill Damage and Refine Attacks in `FighterController` [COMPLETED]
- **Description**: Add parameters for skill range/damage and handle hit detection for skills. Refine attack logic for better accuracy.
- **Assigned role**: developer
- **Implementation**:
    - **Modify `FighterController.cs`**:
        - Add `[Header("Attack Settings")]`, `[Header("Skill Settings")]`.
        - Add `m_SkillRange` and `m_SkillDamage` variables.
        - Override `CastSkill()`: call `base.CastSkill()` then `SkillAttackServerRpc()`.
        - Implement `SkillAttackServerRpc()`: Handles damage for skills.
        - Refine `AttackServerRpc()`: Use a more accurate center for the overlap sphere.
        - Add `OnDrawGizmosSelected()` for visual debugging of ranges.

# Verification & Testing [IN PROGRESS]
- **Manual Test (Solo/Practice Mode)**:
    1. Start practice mode.
    2. Swing hand to trigger sword attack; verify the dummy character takes damage and plays the "hit" animation.
    3. Perform gesture to trigger skill; verify dummy takes damage (should be higher and in a larger area).
- **Multiplayer Test**:
    1. Verify both players see the "hit" reaction when one is struck.
    2. Verify health bars update correctly for both players.

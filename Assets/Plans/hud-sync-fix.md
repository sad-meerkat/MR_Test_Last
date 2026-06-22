# Project Overview
- Game Title: MR Tabletop Fighting Game
- Goal: Connect character health to the top HUD UI, remove character-attached health bars, and implement skill damage.

# Game Mechanics
## Core Gameplay Loop
- Players attack each other. Hits are reflected in the top HUD.
- Skills now also deal damage.

# Key Asset & Context
- `FighterHealth.cs`: Manages health and communicates with `FightingHUDManager`.
- `FighterController.cs`: Handles attack and skill logic.
- Fighter Prefabs: `ByakuyaFighter`, `SasukeFighter`.

# Implementation Steps

## Step 1: Update `FighterHealth.cs` for Top HUD Sync
- **Description**: Remove character-attached UI references and ensure `playerIndex` is used correctly to update `FightingHUDManager`.
- **Assigned role**: developer
- **Implementation**:
    - Remove `[SerializeField] Image m_HealthBar`.
    - In `UpdateUI()`, check if `FightingHUDManager.Instance` exists and call `UpdateHealth(playerIndex.Value, health.Value / 100f)`.
    - Add a log to confirm which `playerIndex` this fighter is using.

## Step 2: Implement Skill Damage in `FighterController.cs`
- **Description**: Override `CastSkill` to apply damage on the server.
- **Assigned role**: developer
- **Implementation**:
    - Add `m_SkillRange` (0.3f) and `m_SkillDamage` (20f).
    - Override `CastSkill()`: call `base.CastSkill()` and then `SkillAttackServerRpc()`.
    - Implement `SkillAttackServerRpc()`: Check for hits in a larger sphere and apply damage.

## Step 3: Remove Character-Attached Health Bars
- **Description**: Disable the `HealthCanvas` on the fighter prefabs so only the top HUD is visible.
- **Assigned role**: developer
- **Implementation**:
    - Find and disable/delete the `HealthCanvas` object in `ByakuyaFighter` and `SasukeFighter` prefabs.

# Verification & Testing
- **Manual Test**:
    1. Start a game (solo or multi).
    2. Attack the opponent; verify the top HUD health bar for the correct player (P1 or P2) decreases.
    3. Perform a skill; verify the top HUD health bar decreases more significantly.
    4. Confirm that no health bars are floating above the characters.

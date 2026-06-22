# Separate Scaling for Skill Visuals and Damage

This plan addresses the requirement to independently control the scale of visual effects and damage-dealing projectiles in both `CharacterGestureSkillManager` and `CharacterBarrageManager`.

## Project Overview
- **Objective**: Split the single scale multiplier into two independent multipliers (Effect and Damage) in the inspector.
- **Context**: Users want visual effects to potentially have different sizes than the invisible damage colliders for gameplay balance or better aesthetics.

## Implementation Steps

### 1. Update `CharacterGestureSkillManager.cs`
- **Description**: Replace `m_ProjectileScaleMultiplier` with `m_EffectScaleMultiplier` and `m_DamageScaleMultiplier`. Update the spawning logic to apply these specifically to visuals and damage objects.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes

### 2. Update `CharacterBarrageManager.cs`
- **Description**: Replace `m_ProjectileScaleMultiplier` with `m_EffectScaleMultiplier` and `m_DamageScaleMultiplier`. Update the sequence routines to apply these specifically.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes

## Detailed Code Changes

### CharacterGestureSkillManager.cs
- **Fields**:
  - `float m_EffectScaleMultiplier`
  - `float m_DamageScaleMultiplier`
- **Logic**: 
  - `OnGesturePerformed` will pass the character's base scale to RPCs.
  - `SpawnCharacterSkillServerRpc` applies `m_DamageScaleMultiplier`.
  - `SpawnCharacterVisualClientRpc` applies `m_EffectScaleMultiplier` (combined with `m_VisualEffectScale`).

### CharacterBarrageManager.cs
- **Fields**:
  - `float m_EffectScaleMultiplier`
  - `float m_DamageScaleMultiplier`
- **Logic**:
  - `OnComboPerformed` passes the character's base scale.
  - `ServerDamageRoutine` uses `m_DamageScaleMultiplier`.
  - `ClientVisualRoutine` uses `m_EffectScaleMultiplier`.

## Verification & Testing
- **Inspector Verification**: Ensure both multipliers appear for all skill managers.
- **Gameplay Test**: Set extreme differences (e.g., tiny visual, giant damage collider) and verify the physical impact matches the damage collider while the visual matches the effect setting.
- **Network Sync**: Confirm that the separation works correctly across server and client RPCs.

# Fix Projectile Visuals and Scaling

This plan addresses the issue where projectiles appear as capsules and have uniform sizes regardless of the assigned prefab.

## Project Overview
- **Issue**: Projectiles show unwanted capsule geometry and ignore prefab-specific scales.
- **Root Cause**: 
  - Damage prefabs (`Dg_` prefix) have visible MeshRenderers.
  - Manager scripts overwrite `localScale` absolutely instead of multiplying by the prefab's original scale.

## Implementation Steps

### 1. Modify `CharacterGestureSkillManager.cs`
- **Description**: Update `SetupProjectile` to multiply the existing scale by the calculated scale instead of overwriting it.
- **Assigned role**: developer
- **Dependencies**: None

### 2. Modify `CharacterBarrageManager.cs`
- **Description**: Update both visual and damage instantiation logic to use multiplicative scaling.
- **Assigned role**: developer
- **Dependencies**: None

### 3. Prefab Cleanup (Manual Instructions)
- **Description**: Advise the user to disable `MeshRenderer` on `Dg_Fire`, `Dg_Tornado`, and `Dg_Katana` prefabs to make them truly invisible damage colliders.

## Verification & Testing
1. **Visual Test**: Verify that the capsule geometry is no longer visible when skills are fired.
2. **Scaling Test**: Assign prefabs of different sizes and verify they maintain their relative scale differences in-game.
3. **Multiplayer Sync**: Ensure scale remains consistent across server (damage) and clients (visuals).

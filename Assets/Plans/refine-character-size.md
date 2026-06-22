# Project Overview
- Game Title: MR Tabletop Fighting Game
- High-Level Concept: Reducing the size of the fighters to better fit the tabletop environment.
- Goals: Decrease character visual size and update physics colliders accordingly.

# Game Mechanics
## Character Scaling
- Character models will be scaled down from 0.1 to 0.07.
- Physics colliders (CapsuleCollider) will be updated to match the new visual size (Height 0.14, Radius 0.035).

# UI
- (No changes)

# Key Asset & Context
- `Assets/MRTabletopAssets/Games/FightingGame/Prefabs/ByakuyaFighter.prefab`
- `Assets/MRTabletopAssets/Games/FightingGame/Prefabs/SasukeFighter.prefab`

# Implementation Steps
1. **Scale Character Models**:
   - Open `ByakuyaFighter` and `SasukeFighter` prefabs.
   - Set the `Local Scale` of the "Model" object to `(0.07, 0.07, 0.07)`.
2. **Update Colliders**:
   - Update the `CapsuleCollider` on the root of each prefab.
   - Set `Height` to `0.14`.
   - Set `Radius` to `0.035`.
   - Set `Center` to `(0, 0.07, 0)`.

# Verification & Testing
- Spawn characters in the scene.
- Verify that they look smaller and are properly grounded on the table.
- Check that hit detection still works with the smaller colliders.

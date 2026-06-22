# Project Overview
- Game Title: MR Tabletop Character
- High-Level Concept: Physics-based character movement on a virtual table.
- Goal: Fix characters escaping the table by switching to proper physics movement and configuring Rigidbody settings. This provides a foundation for future skill-based physics (knockbacks, etc.).

# Game Mechanics
## Core Gameplay Loop
Summon and control fighters on a table.
## Controls and Input Methods
- Physics-based movement via `Rigidbody.MovePosition`.
- Synchronized via `NetworkTransform`.

# Key Asset & Context
- `Assets/MRTabletopAssets/Games/FightingGame/Scripts/FighterController.cs`: Main script to update.
- `Assets/MRTabletopAssets/Games/FightingGame/Prefabs/ByakuyaFighter.prefab`
- `Assets/MRTabletopAssets/Games/FightingGame/Prefabs/SasukeFighter.prefab`
- `Assets/Scripts/MR_Character/TableCharacter.cs` (Already uses MovePosition, but needs verification).

# Implementation Steps
1. **Modify `FighterController.cs`**:
    - Add `private Rigidbody m_Rigidbody` and initialize in `Awake`.
    - Change `Update` movement logic:
        - Convert the 2D input into a 3D vector.
        - Transform the vector from local space (table) to world space.
        - Use `m_Rigidbody.MovePosition` to apply movement.
    - Update rotation to use `transform.rotation` instead of `localPosition`.

2. **Update Fighter Prefabs**:
    - Load `ByakuyaFighter` and `SasukeFighter` prefabs.
    - Set `Rigidbody.isKinematic = false`.
    - Set `Rigidbody.useGravity = true`.
    - Set `Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ`.
    - Ensure `CapsuleCollider` is active and correctly sized.

3. **Verify General Character**:
    - Ensure `TableCharacter.prefab` also has `isKinematic = false` and proper constraints.

4. **Validation**:
    - Enter Play Mode.
    - Move fighters to the table edge. They should be stopped by the `TableBoundaries`.

# Verification & Testing
- **Edge Collision**: Character stops at the edge.
- **Table Tilt/Move**: If the table moves, the character should stay relative to it (handled by local-to-world conversion).
- **Network Sync**: Confirm movement is still synced (NetworkTransform handles world position changes).

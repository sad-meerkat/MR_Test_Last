# Project Overview
- Game Title: MR Tabletop Character
- High-Level Concept: Fighting game characters on a virtual table.
- Problem: Characters (Byakuya, Sasuke) pass through the "Invisible Walls" because their movement logic uses `transform.localPosition` directly, bypassing the physics engine.

# Game Mechanics
## Core Gameplay Loop
Players move fighters on the table and attack each other.
## Controls and Input Methods
- `FighterController.cs` handles movement via `transform.localPosition`.

# Key Asset & Context
- `FighterController.cs`: Needs to be changed to use `Rigidbody` for movement.
- `ByakuyaFighter.prefab`, `SasukeFighter.prefab`: Need `Rigidbody` settings adjusted.

# Implementation Steps
1. **Modify `FighterController.cs`**:
    - Add a `Rigidbody` reference.
    - Change the movement logic in `Update` (or `FixedUpdate`) to use `m_Rigidbody.linearVelocity` or `m_Rigidbody.MovePosition`.
    - Since it's a networked game, ensure `MovePosition` is used to play nice with `NetworkTransform`.
    - Convert the local movement vector to world space before applying it to the Rigidbody.

2. **Adjust Prefab Physics**:
    - Set `Rigidbody.isKinematic = false` on `ByakuyaFighter` and `SasukeFighter`.
    - Set `Rigidbody.useGravity = true` to keep them on the table.
    - Set `RigidbodyConstraints` to freeze rotation on X and Z axes.
    - Ensure `CapsuleCollider` is correctly sized.

3. **Validation**:
    - Test if the fighters now collide with and are stopped by the `TableBoundaries` (Invisible Walls).

# Verification & Testing
- **Collision Test**: Move the character against the invisible wall. It should stop and not "jitter" through.
- **Network Sync Test**: Ensure movement still looks smooth for other players (handled by `NetworkTransform`).

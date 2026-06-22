# Project Overview
- Game Title: MR Tabletop Character
- High-Level Concept: An MR experience where characters move on a virtual table.
- Players: Single player (MR)
- Render Pipeline: URP-Performant
- Target Platform: Android (Quest/MR)

# Game Mechanics
## Core Gameplay Loop
The player summons a character on a virtual table and moves it. The character should stay within the table surface.
## Controls and Input Methods
- XR Input via `TableCharacterInput`.
- Physics-based movement via `TableCharacter.cs` and Rigidbody.

# UI
- Virtual Table interface.

# Key Asset & Context
- `Virtual Table/TableSystem/TableTop`: The parent object for the table surface.
- `Virtual Table/TableSystem/TableTop/Plane`: The visual surface and current reference for table size (~0.8m x 0.8m).
- `TableCharacter.prefab`: The character with a CapsuleCollider and Rigidbody.

# Implementation Steps
1. **Create `TableBoundary.cs`**:
    - Implement a script that creates 4 child GameObjects (`North`, `South`, `East`, `West`) under the object it's attached to.
    - Each child will have a `BoxCollider` to act as an invisible wall.
    - The script will calculate the wall positions and sizes based on a target `MeshRenderer` (the `Plane`) to ensure they match the table edges.
    - The walls will be tall enough (e.g., 0.5m) to prevent the character from jumping or sliding over.

2. **Setup in Scene**:
    - Attach the `TableBoundary` script to the `TableTop` GameObject.
    - Assign the `Plane` object to the script's reference field.
    - Configure the colliders to be on a layer that blocks the character (typically Default).

3. **Verify Collision**:
    - Ensure the character's `Rigidbody` in `TableCharacter.cs` is NOT kinematic (it currently isn't) so it reacts to the physics walls.
    - Test movement to confirm the character is blocked by the invisible walls at the table edges.

# Verification & Testing
- **Boundary Test**: Drive the character into all four edges of the table. It should stop exactly at the edge.
- **Visual Check**: Ensure the walls are invisible in the Game view (no MeshRenderer) but visible in the Scene view (collider wireframes).
- **Scale Test**: Change the `Plane`'s scale and verify the boundaries update accordingly (if implemented to update).

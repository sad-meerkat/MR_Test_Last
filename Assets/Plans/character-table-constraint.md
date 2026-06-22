# Project Overview
- Game Title: MR Tabletop Character
- High-Level Concept: An MR experience where characters move on a virtual table.
- Players: Single player (MR)
- Render Pipeline: URP-Performant
- Target Platform: Android (Quest/MR)

# Game Mechanics
## Core Gameplay Loop
The player summons a character on a virtual table and moves it using XR controllers or gestures. The character can perform skills.
## Controls and Input Methods
- XR Input (Joystick/Touchpad) via `TableCharacterInput`.
- Hand Gestures for skill casting.

# UI
- Virtual Table interface for summoning and management.

# Key Asset & Context
- `Assets/Scripts/MR_Character/TableCharacter.cs`: Main script to modify for movement constraints.
- `Assets/Scripts/MR_Character/TableCharacterSummoner.cs`: Initializes the character and sets the table reference.
- `Assets/Scripts/MR_Character/TableCharacter.prefab`: The character prefab that will have the bounds settings.

# Implementation Steps
1. **Modify `TableCharacter.cs`**:
    - Add `m_TableBounds` field (Vector2) to define the walkable area size.
    - Update the `Move` method to calculate the target position, convert it to the table's local space, clamp it, and convert it back to world space.
    - Ensure the character stays within the specified boundaries relative to the `m_TableTransform`.

2. **Configure Prefab**:
    - Update the `TableCharacter` prefab in the inspector to set an appropriate default value for `m_TableBounds` (e.g., 0.6 x 0.6).

3. **Validation**:
    - Test movement in the scene to ensure the character stops at the table edges.

# Verification & Testing
- **Movement Test**: Move the character to the edge of the table using the joystick. It should stop and not fall off or move past the boundary.
- **Table Transformation Test**: Move or rotate the "Virtual Table" in the editor (if possible) or at runtime, and verify that the character's movement is still correctly constrained to the table's new orientation/position.
- **Summoning Test**: Re-summon the character and verify constraints are still active.

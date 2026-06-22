# Project Overview
- Game Title: MR_Test2
- High-Level Concept: MR character summoning and interaction using hand gestures.
- Players: Single player (MR)
- Render Pipeline: URP-Performant
- Target Platform: Android (Quest)

# Problem Analysis
- The user reports that skill effects (like the shield) apply physics forces to the summoned character.
- Investigation shows the `Cylinder` (Shield) prefab has a non-trigger collider on the `Default` layer.
- The `TableCharacter` (Summoned Character) has a non-kinematic Rigidbody on the `Default` layer.
- Consequently, when an effect is activated near the character, it physically pushes or blocks the character.

# Proposed Solution
1. **Collider Correction**: Set all colliders on skill effect prefabs to `isTrigger = true`. This prevents physical collisions (pushing) while still allowing detection if needed.
2. **Layer Isolation**: Create a dedicated physics layer for "Skills" and configure the Physics Collision Matrix so that "Skills" do not collide with "Default" (or the character's layer).
3. **Rigidbody Setting (Optional)**: If the character doesn't need to be physically pushed by anything, set its Rigidbody to `isKinematic = true`.

# Key Assets & Context
- `Assets/Scripts/MR_Character/TableCharacter.prefab`: The summoned character.
- `Assets/Samples/XR Hands/1.7.3/Gestures/Examples/Prefabs/Cylinder.prefab`: The shield effect.
- `PhysicsManager.asset`: Project collision settings.

# Implementation Steps
1. **Modify Cylinder Prefab**: Set the Collider on `Cylinder` to `isTrigger = true`.
   - Role: developer
   - Dependency: None
2. **Setup Physics Layer**: 
   - Define Layer 3 as "Skill".
   - Disable collision between "Skill" and "Default" in the Collision Matrix.
   - Role: developer
   - Dependency: Step 1
3. **Update Skill Prefabs**: Set the layer of all skill effect prefabs (`Cherrykatana2`, `Cylinder`, `Cherry`, `Katana`) to the new "Skill" layer.
   - Role: developer
   - Dependency: Step 2
4. **Character Optimization**: Set the `TableCharacter` Rigidbody to `isKinematic = true` to prevent unintended movement from other physics sources.
   - Role: developer
   - Dependency: None

# Verification & Testing
1. Summon the character.
2. Activate all skill effects (Thumbs Up, Palm Up, etc.) near the character.
3. Verify that the character is NO LONGER pushed or affected by the presence of the effects.

# Project Overview
- Game Title: MR_Test2
- High-Level Concept: Player hand skill effects vs Character attack effects.
- Goal: Prevent player hand effects (1st person) from physically pushing the summoned character, while allowing character effects to have physics.

# Problem Analysis
- Player's hand effects share the same physics layer as the character (Default).
- Hand-held weapons like the shield physically push the character when waved nearby.
- The user wants 1st person effects to be visual-only, but character effects to be physical.

# Proposed Solution: Layer-Based Role Separation
1. **Define Layer**: Create a dedicated layer `PlayerSkill` for 1st-person (hand-held) effects.
2. **Configure Physics**: Disable collision between `PlayerSkill` and `Default` (character) in the Physics Manager.
3. **Dynamic Layer Assignment**: Modify `SkillEffectSpawner.cs` so that any effect it spawns automatically inherits the layer of the Spawner object.
4. **Apply Layer**: Set all weapon spawners on the player's hands to the `PlayerSkill` layer.

# Key Assets & Context
- `Assets/Samples/XR Hands/1.7.3/Gestures/Examples/Scripts/SkillEffectSpawner.cs`: Spawning logic.
- `ProjectSettings/TagManager.asset`: Layer definitions.
- `ProjectSettings/DynamicsManager.asset`: Physics collision matrix.

# Implementation Steps
1. **Update SkillEffectSpawner.cs**:
   - In `OnGesturePerformed`, set the layer of the instantiated effect and all its children to the layer of the spawner's GameObject.
   - Role: developer
   - Dependency: None
2. **Define Physics Layer**:
   - Set Layer 3 to `PlayerSkill`.
   - Role: developer
   - Dependency: None
3. **Configure Collision Matrix**:
   - Disable collision between `PlayerSkill` and `Default`.
   - Role: developer
   - Dependency: Step 2
4. **Update Hand Weapons**:
   - Set the layer of `Cherrykatana`, `Shilrd`, `SkillSpawner_Shaka`, and `SkillSpawner_Fist` (and their anchors) to `PlayerSkill`.
   - Role: developer
   - Dependency: Step 2

# Verification & Testing
1. Summon the character.
2. Wave the shield (`Shilrd`) or sword (`Cherrykatana`) directly through the character.
3. Verify that the character is NOT pushed and there is no physical reaction.
4. Verify that the effects are still visually visible.

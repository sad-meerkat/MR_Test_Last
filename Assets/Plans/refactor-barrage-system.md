# Refactoring Combo Barrage System

Separating local hand visual logic from networked character-centric skill logic in `ComboBarrageLauncher`.

# Project Overview
- **Game Title**: MR Tabletop Fighting Game
- **High-Level Concept**: Refactor the barrage skill to separate local player feedback from character-based combat.
- **Players**: Multiplayer (Unity Netcode)

# Key Asset & Context
- `ComboBarrageLauncher.cs`: Handles hand visuals and combo detection.
- `CharacterBarrageManager.cs`: New script for character skill execution and networking.

# Implementation Steps

1. **Refactor `ComboBarrageLauncher.cs`**
    - **Description**: Remove "Character Centric Settings" and related spawning logic. Add `public event Action OnComboAction`.
    - **Assigned role**: developer
    - **Dependencies**: None
    - **Parallelizable**: No

2. **Create `CharacterBarrageManager.cs`**
    - **Description**: New script to handle character-side barrage spawning with ServerRpc/ClientRpc support.
    - **Assigned role**: developer
    - **Dependencies**: Step 1
    - **Parallelizable**: No

3. **Scene Setup**
    - **Description**: Attach `CharacterBarrageManager` to the manager object and configure prefabs.
    - **Assigned role**: developer
    - **Dependencies**: Step 2
    - **Parallelizable**: No

# Verification & Testing
- **Gesture Detection**: Verify the local hand visuals still work correctly.
- **Character Spawning**: Verify the character fires 5 projectiles in sequence.
- **Networking**: Verify projectiles and damage are synchronized across the network.

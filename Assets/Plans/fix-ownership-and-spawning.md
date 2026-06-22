# Project Overview
- Game Title: MR Tabletop Fighting Game
- Issue 1: Picking Sasuke causes both Sasuke and a Robot to spawn.
- Issue 2: Both spawned characters move simultaneously with one controller.
- Diagnosis:
  - **Ownership Missing**: `FighterController.cs` processes local input for every instance of the prefab in the scene.
  - **Spawning Redundancy**: `FightingGameManager.cs` spawns characters based on all occupied seats without strictly validating if a character choice was made or if the seat data is stale.

# Game Mechanics
## Movement Ownership
- Only the local owner of a character should be able to move it.
- Non-owned characters (opponents) should only update via network synchronization.

# Key Asset & Context
- `FighterController.cs`: Handles input and movement.
- `FightingGameManager.cs`: Handles spawning logic.

# Implementation Steps
1. **Fix Ownership in FighterController**:
   - Add `if (!IsOwner) return;` at the beginning of the movement logic.
   - Ensure the `m_PlayerInput` or input reading only happens for the owner.
2. **Refine Spawning in FightingGameManager**:
   - Remove the hardcoded fallback to `m_RobotPrefab` and `m_KnightPrefab`.
   - Add a check to only spawn if the seat's `playerID` matches a valid connected client.
   - Log the exact ClientID being assigned ownership for each spawn.

# Verification & Testing
1. **Ownership Test**:
   - Spawn two characters.
   - Move the controller. Verify only one character (the one you own) moves.
2. **Single Spawn Test**:
   - Pick Sasuke. Verify only Sasuke spawns at your seat.
   - Ensure no "Ghost Robots" appear in empty seats.

# Project Overview
- Game Title: MR Tabletop Fighting Game
- Goal: Implement character selection storage and spawning logic for Byakuya and Sasuke.
- Mechanics: 
  - Save the index of the selected character for each player.
  - Spawn the corresponding character prefab when the fight starts.

# Game Mechanics
## Spawning Logic
- The `FightingGameManager` will hold an array of prefabs.
- `SelectCharacterRpc` will update a `NetworkVariable` array (or separate variables) for each player's choice.
- `SpawnFighters` will use these choices to instantiate characters.

# UI
- (No changes needed, functionality already wired to `SelectCharacter(index)`).

# Key Asset & Context
- `FightingGameManager.cs`: Primary logic controller.
- `Assets/byakuya/byakuya.fbx` / `Assets/sasuke/sasuke.fbx`: Source models to be turned into fighter prefabs.
- `RobotFighter.prefab` / `KnightFighter.prefab`: References for required components (Netcode, Health, etc.).

# Implementation Steps
1. **Prepare Fighter Prefabs**:
   - Create new prefabs `ByakuyaFighter.prefab` and `SasukeFighter.prefab` based on `RobotFighter.prefab` but using the new FBX models.
   - Ensure they have `NetworkObject`, `FighterHealth`, and necessary scripts.
2. **Update FightingGameManager.cs**:
   - Add `NetworkVariable<int> m_P1Choice` and `m_P2Choice`.
   - Add a `GameObject[] m_CharacterPrefabs` array to the inspector.
   - Update `SelectCharacterRpc` to save the `slotIndex` into the corresponding choice variable.
   - Update `SpawnFighters` to use `m_CharacterPrefabs[choice]` instead of the hardcoded robot/knight.
3. **Configure Manager**:
   - Assign `ByakuyaFighter` and `SasukeFighter` prefabs to the `m_CharacterPrefabs` list in the `FightingGameManager` inspector.

# Verification & Testing
- Select Byakuya (Slot 0) as P1, Sasuke (Slot 1) as P2.
- Wait for timer or both ready.
- Verify that Byakuya spawns at P1 point and Sasuke at P2 point.

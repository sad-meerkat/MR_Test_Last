# Project Overview
- Game Title: MR Tabletop Fighting Game
- Issue: Sasuke and a Robot (Gundam) spawn together when Sasuke is selected.
- Cause: 
  1. `TableCharacterSummoner` component on `Virtual Table` is set to spawn a default character (Robot) on start.
  2. `FightingGameManager` had fallback fields for robots that might have been triggered.

# Game Mechanics
## Spawning
- Exclusive spawning by `FightingGameManager` based on player selection.

# Key Asset & Context
- `Virtual Table` -> `TableCharacterSummoner` component.
- `FightingGameManager` script and object.

# Implementation Steps
1. **Disable Conflicting Summoner**:
   - Deactivate the `TableCharacterSummoner` component on the `Virtual Table` object. This prevents the "Gundam" from appearing automatically at the start of the scene.
2. **Remove Hardcoded Fallbacks**:
   - Clear the `m_RobotPrefab` and `m_KnightPrefab` references in the `FightingGameManager` component.
   - This ensures that if a selection fails, the system doesn't accidentally spawn a robot as a backup.
3. **Double Check Character Prefabs**:
   - Ensure `m_CharacterPrefabs` array only contains Byakuya (Index 0) and Sasuke (Index 1).

# Verification & Testing
1. **Scene Start Test**: Start the scene and verify NO characters appear until the Fighting Game mode is started and a choice is made.
2. **Sasuke Selection Test**: Pick Sasuke and verify ONLY Sasuke appears.
3. **No Robot Test**: Verify that no robots appear during any part of the selection or fight phase.

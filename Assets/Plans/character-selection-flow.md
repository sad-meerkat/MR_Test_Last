# Project Overview
- Game Title: MR Tabletop Fighting Game
- High-Level Concept: Character selection phase before the fight.
- Goals: 
  - Hide character selection until "Start" is pressed.
  - Implement a 30-second selection timer.
  - Start the game only after both players select or time is up.

# Game Mechanics
## Game Flow States
1. **Idle**: Waiting for players to press "Fight!" button.
2. **Selecting**: 30s timer, players pick characters.
3. **Fighting**: Characters spawn, HUD appears, combat starts.

# UI
## Character Selection UI
- Add a 30s countdown text.
- Toggle visibility based on Game State.
## PreGameUI
- Initial state: Only "Fight!" button visible.

# Key Asset & Context
- `FightingGameManager.cs`: Update to handle `FightingGameState`.
- `CharacterSelectionUI`: Child of `PreGameUI`.
- `StartButton`: Child of `PreGameUI`.

# Implementation Steps
1. **Update CharacterSelectionUI Hierarchy**:
   - Add a `SelectionTimer` TextMeshPro element to the top right of the selection screen.
   - Set `CharacterSelectionUI` to be inactive by default.
2. **Modify FightingGameManager.cs**:
   - Add `enum FightingGameState { Idle, Selecting, Fighting }`.
   - Add `NetworkVariable<FightingGameState> m_GameState`.
   - Add a timer variable `m_SelectionTimer`.
   - Update `OnGameStateChanged` to handle UI visibility.
   - Implement `StartSelectingRpc` to transition from `Idle` to `Selecting`.
   - Implement logic to transition from `Selecting` to `Fighting` after 30s or when both ready.
3. **Update HUD and Prefabs**:
   - Ensure the `SpawnFighters` logic uses the selected characters (for now, keep the robot/knight defaults but trigger from the new state).

# Verification & Testing
- Start Game: "Fight!" button is shown, Selection UI is hidden.
- Click "Fight!": Button disappears, Selection UI appears with 30s timer.
- After 30s: Characters spawn and game starts.

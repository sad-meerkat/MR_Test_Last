# Project Overview
- Game Title: Tabletop MR Games
- High-Level Concept: Fixing the Fighting Game mode setup to ensure UI buttons work and network synchronization is correct.
- Players: Multiplayer (Netcode for GameObjects)
- Render Pipeline: URP
- Target Platform: Android (Quest/MR)

# Game Mechanics
## Core Gameplay Loop
Players select "Fighting Game" from the game mode menu. They press "Start" on the table UI, which spawns fighters and starts the match. The state is synced via `FightingGameManager` using `NetworkVariable` and RPCs.

# UI
- **Game Mode Fighting/PreGameUI**: Contains the "StartButton".
- **Game Mode Fighting/InGameUI**: Shown during the game.

# Key Asset & Context
- **FightingGameManager.cs**: The core logic script. It is a `NetworkBehaviour`.
- **Game Mode Fighting**: The scene object that should hold the manager and `NetworkObject`.
- **StartButton**: Needs to trigger the game start.

# Analysis of Issues
1. **Missing Component**: The `FightingGameManager` script is currently missing from the `Game Mode Fighting` object.
2. **Missing Reference**: The `StartButton`'s `OnClick` event has a `null` target.
3. **Network Spawning**: As a `NetworkBehaviour`, the object must have a `NetworkObject` and be part of the scene to spawn correctly when the network starts.

# Implementation Steps
1. **Restore FightingGameManager**:
   - Add the `FightingGameManager` component back to the `Virtual Table/Tabletop Games/Game Mode Fighting` object.
2. **Configure FightingGameManager**:
   - **Game Mode ID**: Set to `4`.
   - **Objects To Toggle**: (Optional) Add any environmental objects that should appear only in this mode.
   - **Pre Game UI**: Assign `Virtual Table/Tabletop Games/Game Mode Fighting/PreGameUI`.
   - **In Game UI**: Assign `Virtual Table/Tabletop Games/Game Mode Fighting/InGameUI`.
   - **Robot Prefab**: Assign `Assets/MRTabletopAssets/Games/FightingGame/Prefabs/RobotFighter.prefab`.
   - **Knight Prefab**: Assign `Assets/MRTabletopAssets/Games/FightingGame/Prefabs/KnightFighter.prefab`.
   - **Spawn Points**: Assign `SpawnPoint1`, `SpawnPoint2`, etc., from the children of `Game Mode Fighting`.
3. **Ensure NetworkObject**:
   - Verify that `Virtual Table/Tabletop Games/Game Mode Fighting` has a `NetworkObject` component.
4. **Fix UI Button**:
   - On `Virtual Table/Tabletop Games/Game Mode Fighting/PreGameUI/StartButton`, set the `OnClick` target to the `Game Mode Fighting` object and select the function `FightingGameManager.StartGameButtonPressed`.
5. **Verify GameModeManager**:
   - Ensure the `GameModeManager` (on `Tabletop Games`) correctly detects the `FightingGameManager` (it searches children for `IGameMode`).

# Verification & Testing
1. **Scene Validation**: Check that all references in `FightingGameManager` are assigned and no `null` fields exist.
2. **Button Check**: Confirm the button click event is correctly linked.
3. **Network Test**:
   - Run the game, start a Host session.
   - Select Fighting Game mode.
   - Press the "Fight!" button.
   - Verify that the RPC is sent without error and fighters are spawned at the spawn points.

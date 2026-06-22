# Project Overview
- Game Title: Tabletop MR Games
- High-Level Concept: A collection of mixed reality tabletop games including a fighting game mode.
- Players: Multiplayer (Networked)
- Render Pipeline: URP
- Target Platform: Android (Quest/MR)

# Game Mechanics
## Core Gameplay Loop
The fighting game mode allows players to spawn fighters and engage in a match. The match is started via a UI button that triggers an RPC call to the server.

# UI
- **PreGameUI**: Contains the "Start Game" button.
- **InGameUI**: Displays game status during a match.

# Key Asset & Context
- **FightingGameManager.cs**: Manages the fighting game state and handles the "Start Game" button click.
- **Game Mode Fighting**: The container GameObject for the fighting game logic and UI.

# Implementation Steps
1. **Add NetworkObject Component**:
   - Add a `NetworkObject` component to the `Virtual Table/Tabletop Games/Game Mode Fighting` GameObject.
   - This is required for `FightingGameManager` (a `NetworkBehaviour`) to send RPCs.
2. **Verify Configuration**:
   - Ensure `FightingGameManager` is correctly referencing the `PreGameUI` and `InGameUI`.
   - Ensure the `StartButton` in `PreGameUI` is still correctly calling `FightingGameManager.StartGameButtonPressed`.

# Verification & Testing
1. **Scene Check**:
   - Confirm `NetworkObject` is present on the `Game Mode Fighting` object in the inspector.
2. **Runtime Test**:
   - Start the `NetworkManager` (Host/Server).
   - Press the "Start" button in the `Game Mode Fighting` UI.
   - Verify that the `RpcException` no longer occurs and the game state changes.

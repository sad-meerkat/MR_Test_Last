# Project Overview
- Game Title: MR Tabletop Fighting Game
- High-Level Concept: An MR fighting game where players sit at a virtual table and summon fighters using hand gestures and motion.
- Players: 2-player local multiplayer (MR/Networked)
- Input System: XR Hands + NGO (Netcode for GameObjects)
- Render Pipeline: URP

# Game Mechanics
## Core Gameplay Loop
- Players join a seat at the table.
- Both players click "R u ready?" to start character selection.
- Players select their fighters.
- Players engage in battle using hand-motion-triggered skills.

## Controls and Input Methods
- **UI Interaction**: World-space buttons for readiness and selection.
- **Combat**: Hand swings detect velocity to trigger character attacks.

# Key Asset & Context
- `FightingGameManager.cs`: Main game logic controller.
- `FighterHealth.cs`: Handles character health and HUD updates.
- `CharacterSelectionUI`: World-space UI for picking characters.
- `Fighter Prefabs` (`ByakuyaFighter`, `SasukeFighter`): The networked characters.

# Implementation Steps
## Step 1: Fix Character Visibility (MR Position Sync) [COMPLETED]
- **Description**: The characters currently use world-space synchronization (`InLocalSpace = false`), which causes them to appear in the wrong location for clients whose "Virtual Table" is in a different world position.
- **Assigned role**: developer
- **Implementation**:
    - Modify the `NetworkTransform` component on `ByakuyaFighter` and `SasukeFighter` prefabs.
    - Set `InLocalSpace = true`.

## Step 2: Implement Dual Readiness for Game Start [COMPLETED]
- **Description**: Change the game start logic so that both seated players must click "R u ready?" before proceeding to character selection.
- **Assigned role**: developer
- **Implementation**:
    - **Modify `FightingGameManager.cs`**:
        - Add `private NetworkVariable<bool> m_P1PreReady` and `m_P2PreReady`.
        - Update `StartGameButtonPressed()` to call a new RPC: `TogglePreReadyRpc()`.
        - In `TogglePreReadyRpc`, identify the sender's seat and toggle their corresponding pre-ready variable.
        - Update `Update()` loop on the server to check if all seated players are pre-ready before transitioning `m_GameState` to `Selecting`.

## Step 3: Fix Health and HUD Synchronization [IN PROGRESS]
- **Description**: `FighterHealth.playerIndex` is currently a regular `int`, so it is not synchronized. This causes HUD updates to be incorrect on clients.
- **Assigned role**: developer
- **Implementation**:
    - **Modify `FighterHealth.cs`**:
        - Change `public int playerIndex` to `public NetworkVariable<int> playerIndex = new NetworkVariable<int>(0)`.
        - Update references to use `playerIndex.Value`.
        - Initialize the variable in `OnNetworkPreSpawn`.
    - **Modify `FightingGameManager.cs`**:
        - In `SpawnFighters()`, set `fighterHealth.playerIndex.Value = playerCounter`.

# Verification & Testing
- **Multiplayer Test**:
    1. Start a host and a client.
    2. Verify both can see the "R u ready?" button.
    3. Click on the host; verify the button stays (or changes state) and doesn't disappear immediately.
    4. Click on the client; verify the game transitions to `Selecting`.
    5. Select characters on both; verify both see both characters spawning correctly on their respective tables.
    6. Take damage and verify the HUD updates the correct player's health bar on both sides.

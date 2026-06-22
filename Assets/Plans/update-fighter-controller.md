# Project Overview
- Game Title: MR Tabletop Game
- High-Level Concept: Fighting game on a tabletop in Mixed Reality.
- Players: Multiplayer (Unity Netcode)
- Target Platform: Android (Quest/MR)

# Game Mechanics
## Core Gameplay Loop
Players control characters on a table, performing attacks and jumping to defeat opponents.
## Controls and Input Methods
Uses XR Input actions for attacking and jumping.

# Key Asset & Context
- `Assets/MRTabletopAssets/Games/FightingGame/Scripts/FighterController.cs`: The controller to be updated.
- `Assets/MRTabletopAssets/Scripts/Character/TableCharacter.cs`: The base class.

# Implementation Steps
## Step 1: Update FighterController.cs
**Description**: Replace the content of `FighterController.cs` with the corrected code including `m_JumpForce = 0.5f`.
**Assigned role**: developer
**Dependencies**: None
**Parallelizable**: No

# Verification & Testing
- Check for compilation errors in the Unity Console.
- Ensure the `Jump Force` field in the Inspector for `FighterController` shows `0.5`.
- Play the game and verify that the character jumps at the expected height.

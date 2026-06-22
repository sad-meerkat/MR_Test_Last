# Project Overview
- Game Title: MR Tabletop Fighting Game
- Issue: In Practice Mode (Host), the player controls the dummy instead of their own character because they own both. This causes attacks to hit the player instead of the dummy, making health reflection confusing.

# Implementation Steps

## Step 1: Distinguish Dummy from Player
- **Description**: Add a way to identify if a character is a dummy so the input system can ignore it.
- **Assigned role**: developer
- **Files**: `Assets/MRTabletopAssets/Scripts/Character/TableCharacter.cs`
- **Action**: Add `public bool isDummy;` to the class.
- **Dependencies**: None

## Step 2: Set Dummy Flag on Spawn
- **Description**: Mark the practice dummy as a dummy when it is spawned by the server.
- **Assigned role**: developer
- **Files**: `Assets/MRTabletopAssets/Games/FightingGame/Scripts/FightingGameManager.cs`
- **Action**: In `SpawnFighters`, when spawning the practice dummy, set `f.GetComponent<TableCharacter>().isDummy = true;`.
- **Dependencies**: Step 1

## Step 3: Prevent Dummy Control in TableCharacterInput
- **Description**: Modify the character search logic to only pick characters that are NOT dummies.
- **Assigned role**: developer
- **Files**: `Assets/MRTabletopAssets/Scripts/Character/TableCharacterInput.cs`
- **Action**: In the `Update` loop search, add `&& !character.isDummy` to the condition.
- **Dependencies**: Step 1

# Verification & Testing
1. **Play Mode**: Start a practice game as Host.
2. **Move/Attack**: Verify your character (Seat 0) moves and swings, not the dummy.
3. **Hit Dummy**: Hit the dummy and verify the **Right** health bar (Player 1) decreases.

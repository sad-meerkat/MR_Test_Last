# Project Overview
- Game Title: MR Test (XR Multiplayer)
- High-Level Concept: Tabletop MR character movement and interaction.
- Issue: The Virtual Table spawns out of reach (e.g., on the floor), making it difficult for seated players to interact or reposition it.

# Game Mechanics
## Core Gameplay Loop
- Players summon and control characters on a virtual table.
- Repositioning the table is done by grabbing virtual handles.

## Controls and Input Methods
- XRI Grab for table manipulation.
- A "Recenter" feature is needed to bring the table to a reachable height and distance automatically.

# Key Asset & Context
- `TableInitializer.cs`: Handles initial alignment.
- `TableManipulator.cs`: Handles player-relative repositioning.
- `OVRMultiplayerBootstrap.cs`: Generates the Lobby UI.
- `Virtual Table` GameObject: The root object to reposition.

# Implementation Steps

## Step 1: Create TableRecenter Script
- **Description**: Implement a new script `TableRecenter.cs` that repositions the `Virtual Table` in front of the player at a comfortable height (~40cm below eyes).
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes

## Step 2: Integrate Recenter Button into Lobby UI
- **Description**: Modify `OVRMultiplayerBootstrap.cs` to add a "Recenter Table" button in the Lobby UI. This allows users to fix reachability issues at any time.
- **Assigned role**: developer
- **Dependencies**: Step 1
- **Parallelizable**: No

## Step 3: Improve Initial Spawn Logic
- **Description**: Update `TableInitializer.cs` to use the camera's height and forward vector to place the table at a "desk height" by default, instead of just floor height.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes

# Verification & Testing
- **Initial Spawn Test**: Start the game and verify the table is at a reachable height (roughly chest/waist level when sitting) and distance (~50cm).
- **Recenter Button Test**: Move away from the table, open the Lobby UI, and press "Recenter Table". Verify the table moves to your new front position.
- **Manipulation Test**: Grab the table handles and ensure repositioning still works relative to the new base position.

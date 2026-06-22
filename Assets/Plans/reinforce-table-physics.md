# Project Overview
- Game Title: MR Test (XR Multiplayer)
- Issue: Characters fall through the table floor. This is likely due to the thin `MeshCollider` on the `Plane` not reliably stopping small objects (0.14m) under gravity.

# Game Mechanics
- Characters use physics to stay on the table.

# Key Asset & Context
- `TableBoundary.cs`: Generates walls for the table.
- `FightingGameManager.cs`: Spawns fighters at table level.
- `Virtual Table/TableSystem/TableTop/Plane`: The current floor.

# Implementation Steps

## Step 1: Reinforce Table Floor Collider
- **Description**: Modify `TableBoundary.cs` to generate a "Floor" `BoxCollider` in addition to the walls. This collider will have a thickness (e.g., 0.1m) to prevent tunneling.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes

## Step 2: Adjust FightingGameManager Spawn Logic
- **Description**:
    1. Raise the spawn points' Y position by `0.05m` during instantiation.
    2. Parent the spawned fighters to the manager or a container under the table.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes

## Step 3: Update Summoner Parenting
- **Description**: Update `TableCharacterSummoner.cs` to parent the spawned character to ensure it stays relative to the table system.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes

# Verification & Testing
- **Physics Test**: Summon a character and observe it landing on the table. It should not fall through.
- **Boundary Test**: Move the character to the edge and verify it is stopped by the walls and floor.
- **Parenting Test**: (Optional) If the table is moved, verify characters follow.

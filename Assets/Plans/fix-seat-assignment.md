# Project Overview
- Game Title: Tabletop Fighting Game (MR)
- Issue: Player seats are not auto-assigned correctly, and non-host players see seats as "Available" even when occupied.
- Root Causes: 
    1. `NullReferenceException` in `NetworkTableTopManager` due to unassigned slots in the `m_SeatButtons` array.
    2. Lack of retry logic in `UpdateNetworkedSeatsVisuals` when player data is not yet synchronized on clients.

# Game Mechanics
## Core Gameplay Loop
Players join a shared MR space around a virtual table. They must be assigned to seats to interact with the game.
## Controls and Input Methods
N/A

# UI
Seat status indicators on the virtual table.

# Key Asset & Context
- `NetworkTableTopManager.cs`: Manages seat data (`NetworkList`) and triggers UI updates.
- `m_SeatButtons`: Array of `TableTopSeatButton` components, currently containing null entries.

# Implementation Steps

## 1. Make NetworkTableTopManager robust against null buttons
- **Description**: Add null checks when iterating through `m_SeatButtons` in `NetworkTableTopManager.cs`.
- **Files**: `Assets/MRTabletopAssets/Scripts/Table/NetworkTableTopManager.cs`
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes

## 2. Implement Retry Logic for Visual Updates
- **Description**: Update `UpdateNetworkedSeatsVisuals` to handle cases where a player object is not yet found by using a coroutine-based retry (similar to `AssignSeatRpc`).
- **Files**: `Assets/MRTabletopAssets/Scripts/Table/NetworkTableTopManager.cs`
- **Assigned role**: developer
- **Dependencies**: Step 1
- **Parallelizable**: No

## 3. Clean up Inspector Setup (Optional but recommended)
- **Description**: Recommend the user to reduce the `m_SeatButtons` array size to match the actual number of seat buttons in the scene.
- **Files**: N/A (Manual task)
- **Assigned role**: explorer
- **Dependencies**: None
- **Parallelizable**: Yes

# Verification & Testing
1. **Multiplayer Test**: Join as a second player. Verify that Seat 1 (Host) is correctly marked as occupied with the host's name.
2. **Auto-Assignment Test**: Join the server. Verify that you are automatically teleported to an available seat and the UI updates for everyone.
3. **Robustness Test**: Ensure no `NullReferenceException` appears in the console when the list update logic runs.

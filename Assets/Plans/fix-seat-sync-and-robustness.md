# Project Overview
- Game Title: Tabletop Fighting Game (MR)
- High-Level Concept: Multiplayer MR fighting game where players occupy virtual seats around a table.
- Issue: Player seats are not auto-assigned correctly on connect, and synchronization issues lead to seats appearing as "Available" for clients even when occupied by the host.

# Game Mechanics
## Core Gameplay Loop
Players join a session, are assigned a seat, and control fighters on the tabletop.
## Controls and Input Methods
N/A

# UI
Seat UI buttons on the virtual table indicating occupancy and player names.

# Key Asset & Context
- `NetworkTableTopManager.cs`: The central script for seat management.
- `m_SeatButtons`: Array of `TableTopSeatButton` objects.

# Implementation Steps

## 1. Implement Robust Null Checking
- **Description**: Add null checks for all accesses to the `m_SeatButtons` array to prevent `NullReferenceException` if the array size in the Inspector doesn't match the scene objects.
- **Files**: `Assets/MRTabletopAssets/Scripts/Table/NetworkTableTopManager.cs`
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes

## 2. Enhance Synchronization with Retry Logic
- **Description**: Modify `UpdateNetworkedSeatsVisuals` to use a retry mechanism (coroutine) when a player's network object is not yet found. This ensures that "Available" labels are replaced with player names as soon as the data arrives.
- **Files**: `Assets/MRTabletopAssets/Scripts/Table/NetworkTableTopManager.cs`
- **Assigned role**: developer
- **Dependencies**: Step 1
- **Parallelizable**: No

# Verification & Testing
1. **Multiplayer Join Test**: Connect a second player (client). Verify that Seat 0 (Host) displays the host's name instead of "Available".
2. **Auto-Assignment Test**: Connect as a client. Verify you are automatically placed in an available seat and teleported there.
3. **Robustness Test**: Keep 4 slots in `m_SeatButtons` with 2 nulls. Verify no errors appear in the console.

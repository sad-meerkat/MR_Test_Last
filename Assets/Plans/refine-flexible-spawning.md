# Project Overview
- Game Title: MR Tabletop Fighting Game
- Issue: Sasuke is not spawning after disabling the default summoner and fallback prefabs.
- Root Cause Analysis: 
  - Stricter spawn logic (`choice >= 0`) might be failing if `m_P1Choice` or `m_P2Choice` isn't updated correctly or if seat data is stale.
  - The check `if (occupiedCount <= 1) allReady = m_P1Ready.Value;` assumes the local player is always P1 (Seat 0). If the player is in Seat 1, they are P2, and the condition fails.

# Game Mechanics
## Flexible Spawning
- Improve logic to detect *any* seated player and their choice, regardless of which specific seat they occupy.
- Add a "Default Choice" (Index 0: Byakuya) if a player is seated but hasn't clicked anything when the timer runs out.

# Key Asset & Context
- `FightingGameManager.cs`: Spawning and state transition logic.

# Implementation Steps
1. **Update State Transition Logic**:
   - Change `allReady` check to verify that *every* occupied seat has a corresponding "Ready" flag.
2. **Refine Spawn Selection**:
   - Instead of checking `i == 0` for P1, loop through all seats and match `NetworkManager.LocalClientId` to ensure the local choice is recorded correctly.
   - Use a fallback: If `choice == -1` (no selection) but the timer is up, force `choice = 0`.
3. **Robust Seat Detection**:
   - Add a refresh of seat data when the "Fight!" button is pressed to ensure the server knows exactly who is where.

# Verification & Testing
1. **Solo Test**: Sit in any seat, pick Sasuke, and verify spawn.
2. **Timer Test**: Sit in any seat, don't pick anything, and verify Byakuya (Index 0) spawns automatically after 30s.
3. **Ownership**: Verify the player can still only move their own spawned character.

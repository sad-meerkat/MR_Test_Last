# Project Overview
- Game Title: MR Tabletop Fighting Game
- Goal: Fix the "one-sided visibility" issue and ensure stable character spawning in a networked environment.
- Root Cause Hypothesis: 
  1. New fighter prefabs (Byakuya, Sasuke) are not registered in the `NetworkManager` Prefab List.
  2. Spawning occurs before seat occupancy data is fully synchronized on the server.
  3. Lack of ownership verification on spawning.

# Game Mechanics
## Spawning Phase
- Transition from `Selecting` to `Fighting` only when seat data is confirmed.
- Server-side authoritative spawning with ownership assignment.

# UI
- (No changes)

# Key Asset & Context
- `NetworkManager`: Needs to be updated with new prefabs.
- `FightingGameManager.cs`: Needs robust spawning logic.

# Implementation Steps
1. **Register Network Prefabs**:
   - Add `ByakuyaFighter.prefab` and `SasukeFighter.prefab` to the `NetworkManager`'s Prefabs list in the scene.
2. **Enhance FightingGameManager Spawning**:
   - Add a delay or validation check in `SpawnFighters` to ensure `m_TableManager.networkedSeats` is populated and matches the expected player count.
   - Add comprehensive logging: `[Server] Spawning [Prefab] for Player [ID] at Seat [Index]`.
   - Implement an explicit `WaitUntilReady` logic if necessary.
3. **Verify Network Object Settings**:
   - Ensure the new prefabs have "Global" visibility and "Scene Object" or "Prefab" status correctly set.

# Verification & Testing
- Run the game with two clients.
- Verify that both clients can see both characters.
- Check console logs for "Spawning" messages for both seats.

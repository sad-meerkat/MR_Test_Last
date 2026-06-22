# Auto Seat Assignment on Connection Plan

This plan implements automatic seat assignment and player teleportation upon connection, removing the need for players to manually click "Seat" buttons.

## Project Overview
- **Game Title**: Tabletop Fighting Game
- **Feature**: Auto-assign seats to players in connection order as soon as they join.
- **Root Cause of Manual Button Need**:
    - `NetworkTableTopManager.cs` actually attempts to auto-assign a seat via `RequestAnySeatFromHost()` inside `OnNetworkSpawn()`.
    - However, because player spawning and network state synchronization happen asynchronously, `XRINetworkGameManager.Instance.TryGetPlayerByID(playerID, out var player)` often fails during the initial spawn frame because the player isn't fully registered on all clients yet.
    - When it fails, it logs `Player with id {playerID} not found` and completely skips assigning the seat and teleporting the player's `XROrigin`.
    - Because of this, players remain unseated and must manually press a seat button to trigger the assignment again once they are fully loaded.

## Implementation Steps

### 1. Implement Retry/Delayed Seat Assignment in `NetworkTableTopManager.cs`
- Add a retry mechanism (`IEnumerator AssignSeatWithRetryDelayed`) when `TryGetPlayerByID` fails inside `AssignSeatRpc`.
- This ensures that if a player is still in the process of spawning/loading, the client will retry finding them and teleport them to their assigned seat as soon as they become registered.
- Update `OnPlayerStateChanged` on the server to auto-assign a seat to any newly connected player if they aren't seated yet.

#### Modified Methods in `NetworkTableTopManager.cs`:

```csharp
        void OnPlayerStateChanged(ulong playerID, bool connected)
        {
            if (!connected)
            {
                for (int i = 0; i < networkedSeats.Count; i++)
                {
                    if (networkedSeats[i].playerID == playerID)
                    {
                        ServerRemoveSeat(i);
                    }
                }

                UpdateNetworkedSeatsVisuals();
            }
            else // [New] Player connected!
            {
                // Auto-assign a seat if they don't have one yet
                bool alreadyHasSeat = false;
                for (int i = 0; i < networkedSeats.Count; i++)
                {
                    if (networkedSeats[i].isOccupied && networkedSeats[i].playerID == playerID)
                    {
                        alreadyHasSeat = true;
                        break;
                    }
                }

                if (!alreadyHasSeat)
                {
                    int availableSeat = GetAnyAvailableSeats();
                    if (availableSeat >= 0)
                    {
                        ServerAssignSeat(-1, availableSeat, playerID);
                        Debug.Log($"[NetworkTableTopManager] Auto-assigned Seat {availableSeat} to Player {playerID} on connect.");
                    }
                }
            }
        }

        [Rpc(SendTo.Everyone)]
        void AssignSeatRpc(int seatID, ulong playerID)
        {
            if (XRINetworkGameManager.Instance.TryGetPlayerByID(playerID, out var player))
            {
                m_SeatButtons[seatID].AssignPlayerToSeat(player);
                if (playerID == NetworkManager.Singleton.LocalClientId)
                {
                    m_SeatSystem.TeleportToSeat(seatID);
                }
            }
            else
            {
                Debug.LogWarning($"[NetworkTableTopManager] Player with id {playerID} not found yet. Retrying in background...");
                // [New] Start delayed retry coroutine
                StartCoroutine(AssignSeatWithRetryDelayed(seatID, playerID));
            }
        }

        // [New] Delayed assignment retry coroutine
        private System.Collections.IEnumerator AssignSeatWithRetryDelayed(int seatID, ulong playerID)
        {
            float timeout = 5f;
            float elapsed = 0f;
            while (elapsed < timeout)
            {
                yield return new WaitForSeconds(0.2f);
                elapsed += 0.2f;

                if (XRINetworkGameManager.Instance.TryGetPlayerByID(playerID, out var player))
                {
                    m_SeatButtons[seatID].AssignPlayerToSeat(player);
                    if (playerID == NetworkManager.Singleton.LocalClientId)
                    {
                        m_SeatSystem.TeleportToSeat(seatID);
                    }
                    Debug.Log($"[NetworkTableTopManager] Successfully assigned Seat {seatID} to Player {playerID} after retry.");
                    yield break;
                }
            }
            Debug.LogError($"[NetworkTableTopManager] Retry timed out. Player with id {playerID} not found.");
        }
```

## Verification & Testing
- **Connection Test**: Host a game and connect as a client.
- **Seat Assignment**: Verify that the host is automatically assigned to Seat 1, and the client is automatically assigned to Seat 2 upon connecting, without needing any manual button clicks.
- **Teleportation**: Verify that the client is automatically teleported to their seat coordinates (마주보는 위치) on connection.

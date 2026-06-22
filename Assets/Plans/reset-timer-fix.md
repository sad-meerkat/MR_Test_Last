# Reset Game Timer on Match Start Fix

This plan fixes the issue where starting from the 3rd match (or once 180 seconds of total play time have accumulated across matches), the game immediately ends with a "DRAW" message.

## Project Overview
- **Game Title**: Tabletop Fighting Game
- **Issue**: From the 3rd match onwards, the game immediately terminates with a "DRAW" screen.
- **Root Cause**:
    - The match timer on the server is controlled by `m_GameTimer`, which is a `NetworkVariable<float>`.
    - At the start of the game, `m_GameTimer` defaults to `180f`.
    - However, **the server never resets `m_GameTimer.Value` back to `180f` when a new match starts**.
    - The timer continuously counts down across matches. Once the total gameplay duration across all matches exceeds 180 seconds, `m_GameTimer.Value` reaches `0` and stays there.
    - On any subsequent match starting, the server's `m_GameTimer.Value` is already `<= 0`.
    - The server's `Update()` loop immediately triggers `HandleTimeout()`.
    - Since both players have just spawned and have equal (maximum) health, `HandleTimeout()` declares a tie and ends the match with a "DRAW" message immediately.

## Implementation Steps

### 1. Update `FightingGameManager.cs`
- Modify `OnGameModeStart()` to reset `m_GameTimer.Value` to `180f` on the server before starting the match and spawning fighters.

```csharp
        public void OnGameModeStart()
        {
            if (IsServer)
            {
                m_GameTimer.Value = 180f; // [Fix] Reset the server match timer to 180 seconds!
                SpawnFighters();
            }
            OnSummoned?.Invoke();
            
            if (FightingHUDManager.Instance != null)
            {
                FightingHUDManager.Instance.StartTimer();
            }
        }
```

## Verification & Testing
- **Multiplayer/Local Play Test**: Start the game and play through multiple rounds.
- **Consecutive Matches**: Verify that playing a 3rd or 4th consecutive match no longer causes an immediate "DRAW" termination.
- **Timer Sync**: Verify that both the client HUD timer and the server game timer start at 180 and tick down synchronously in every single match.

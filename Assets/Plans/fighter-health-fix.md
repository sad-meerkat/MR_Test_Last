# Player Health Synchronization Fix

This plan addresses the issue where the fighter's health starts at 100% on the HUD but suddenly drops to 50% upon spawning or taking the first hit.

## Project Overview
- **Game Title**: Tabletop Fighting Game
- **Issue**: Fighter health starts at 100% on the HUD but jumps to 50% on first update/hit.
- **Root Cause**: 
    - The fighter prefabs have `m_MaxHealth` set to `200f`, but the starting `health` NetworkVariable's internal value is serialized as `100f` in the prefabs.
    - On game start, the `FightingHUDManager` hardcodes the HUD health to `100/100` (100% full bar).
    - As soon as the network spawns or the fighter takes damage, `FighterHealth` updates the UI with the actual ratio: `health.Value / m_MaxHealth = 100 / 200 = 0.5` (50%). This causes the HUD to suddenly drop to `HP 50 / 100`.

## Implementation Steps

### 1. Update `FighterHealth.cs`
- Modify `OnNetworkSpawn()` to initialize `health.Value` to `m_MaxHealth` on the server when spawned.
- This ensures that the fighter's starting health always matches their configured maximum health (whether it's 200, 100, or any other value), preventing the 50% health drop bug.

```csharp
        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                health.Value = m_MaxHealth;
                Debug.Log($"[FighterHealth] Server initialized health to m_MaxHealth: {m_MaxHealth}");
            }
            health.OnValueChanged += (old, current) => UpdateUI();
            playerIndex.OnValueChanged += (old, current) => UpdateUI();
            UpdateUI();
        }
```

## Verification & Testing
- **Initialization Test**: Start the game and verify the HUD shows `HP 100 / 100` at the beginning of the match.
- **Update Test**: Verify that the HUD health does NOT immediately drop to 50% upon spawning.
- **Damage Test**: Hit the fighter and verify the health drops smoothly and proportionately from 100% (e.g. dropping to 95/100 instead of jumping straight to 50/100).

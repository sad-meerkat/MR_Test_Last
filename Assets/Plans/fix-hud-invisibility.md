# Project Overview
- Game Title: MR Tabletop Fighting Game
- Issue: `FightingHUD` and its children are invisible.
- Root Cause: `InGameUI` and `FightingHUD` local scales are set to `(0, 0, 0)`.

# UI Changes
- **InGameUI**: Reset `Local Scale` to **(0.01, 0.01, 0.01)** to match the tabletop world scale (similar to `PreGameUI`).
- **FightingHUD**: Reset `Local Scale` to **(1, 1, 1)** so it inherits the parent's scale correctly.
- **Positioning**: Ensure `FightingHUD` is positioned correctly within `InGameUI`.

# Implementation Steps
1. **Fix InGameUI Scale**: Use a script to set the local scale of `InGameUI`.
2. **Fix FightingHUD Scale**: Use a script to set the local scale of `FightingHUD`.
3. **Validate Hierarchy**: Ensure all children (`TopSection`, `Timer`, etc.) are active.

# Verification & Testing
- Start the game or trigger the `Fighting` state.
- Verify that the HUD (Health bars and Timer) appears at the top of the table.

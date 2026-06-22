# Project Overview
- Game Title: MR Tabletop Fighting Game
- Goal: Redesign the Fighting HUD to strictly follow the Tekken style (Top-Left/Top-Right Health Bars, Center Timer) and extend game duration to 3 minutes.

# Game Mechanics
## Game Duration
- Match timer will be set to 180 seconds (3 minutes).
- Updated in `FightingHUDManager.cs` and `FightingGameManager.cs`.

# UI
## FightingHUD Redesign
- **TopSection**: Anchored to the top of the HUD Canvas.
- **Player 1 Health Bar (Left)**: Horizontal bar, fills from center to left or stays consistent? Tekken bars typically fill from outside towards the center.
- **Player 2 Health Bar (Right)**: Mirrors Player 1.
- **Timer (Center)**: Bold text showing remaining seconds.
- **Aesthetics**: Bold outlines, high contrast colors (Yellow/Orange for health).

# Key Asset & Context
- `FightingHUDManager.cs`: Controls the timer and bar updates.
- `FightingHUD` (GameObject): The UI root.

# Implementation Steps
1. **Modify UI Hierarchy**:
   - Re-scale and re-position the `FightingHUD` to be wider and more prominent at the top of the table view.
   - Adjust `TopSection` to span the full width of the HUD.
   - Re-anchor `Player1` group to Top-Left and `Player2` group to Top-Right.
   - Center the `Timer` text between the two health bars.
2. **Update FightingHUDManager.cs**:
   - Change `gameTime` default to `180f`.
   - Update `StartTimer` to reset to 180s.
3. **Styling Improvements**:
   - Add a dark gradient background behind the health bars.
   - Ensure `SnapBillboard` is functioning correctly so it stays at the top of the player's view relative to the table.

# Verification & Testing
- Start a fight: Verify the timer starts at 180.
- Check UI layout: Verify bars are at the top corners and timer is centered.
- Damage test: Verify HP bars decrease correctly from the respective sides.

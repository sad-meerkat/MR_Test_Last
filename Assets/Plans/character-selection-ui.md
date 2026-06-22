# Project Overview
- Game Title: MR Tabletop Fighting Game
- High-Level Concept: A 1v1 fighting game played on a physical table in MR.
- Players: 2 Players (Local/Networked)
- Inspiration: Tekken (UI/Mechanics)
- Target Platform: Android (Quest 3/Pro)
- Render Pipeline: URP

# Game Mechanics
## Character Selection
- Players can choose from a grid of characters.
- Currently, 2 characters are available (Byakuya, Sasuke).
- 3 slots are reserved for future characters (empty).

# UI
## Character Selection HUD
- A World Space Canvas floating near the table.
- A horizontal grid of 5 character portraits.
- Tekken-style aesthetics (bold fonts, highlights).

# Key Asset & Context
- `Assets/Image/뱌쿠야 이미지.jpg` (Byakuya)
- `Assets/Image/사스케 이미지.jpg` (Sasuke)
- `FightingGameManager.cs`: Controls game state and spawning.

# Implementation Steps
1. **Prepare Assets**:
   - Change `Texture Type` of `뱌쿠야 이미지.jpg` and `사스케 이미지.jpg` to `Sprite (2D and UI)`.
2. **Create UI Hierarchy**:
   - Create `CharacterSelectionUI` under `Game Mode Fighting/PreGameUI`.
   - Setup a World Space Canvas with `SnapBillboard`.
   - Create a container with `Horizontal Layout Group`.
   - Add 5 Slot objects:
     - Slot 0: Image (Byakuya Sprite)
     - Slot 1: Image (Sasuke Sprite)
     - Slot 2-4: Image (Black Color)
3. **Styling**:
   - Add a Title "SELECT YOUR CHARACTER".
   - Add a background panel for the grid.
   - Use high-contrast colors (white/yellow/black) for a Tekken feel.

# Verification & Testing
- Open the scene and verify the Character Selection UI is visible in `PreGameUI` state.
- Check if images are correctly displayed.
- Verify the layout is responsive and billboards towards the player.

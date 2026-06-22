# Project Overview
- Game Title: Fighting Game (Tabletop)
- High-Level Concept: A tabletop fighting game where players select characters and fight in a virtual arena.
- Players: 2 Players (Local/Networked)
- Render Pipeline: URP-Performant
- Input System: New Input System

# Game Mechanics
## Character Selection
Players select from a grid of characters. Each selection should be highlighted with a player-specific color. If both players select the same character, a unique "both" color should be shown.

# UI
- **Character Selection UI**: A grid of slots, each representing a character.
- **Outline**: A highlight effect on the selected slot.

# Key Asset & Context
- **FightingGameManager.cs**: Manages the game state, player selections, and UI updates.
- **m_P1SelectColor, m_P2SelectColor, m_BothSelectColor**: Pre-defined colors for selection highlights.
- **UpdateSelectionVisuals()**: The method responsible for updating the selection UI.

# Implementation Steps
## 1. Update Character Selection Visuals Logic
- **Description**: Modify `FightingGameManager.cs` to apply player-specific colors to the selection outline.
- **File**: `Assets/MRTabletopAssets/Games/FightingGame/Scripts/FightingGameManager.cs`
- **Changes**:
    - Update `UpdateSelectionVisuals()` to check P1 and P2 selections independently.
    - Set the `Outline` object's `Image` color based on who selected it.
    - Use `m_P1SelectColor` for P1, `m_P2SelectColor` for P2, and `m_BothSelectColor` if both selected the same slot.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: No

# Verification & Testing
## Manual Verification
1. Start the game and enter the Character Selection state.
2. Have Player 1 select a character. Verify that the outline color matches `m_P1SelectColor` (default blue).
3. Have Player 2 select a different character. Verify that their outline color matches `m_P2SelectColor` (default red).
4. Have both players select the same character. Verify that the outline color matches `m_BothSelectColor` (default magenta).
5. Verify that deselecting or changing characters updates the colors correctly.

## Edge Cases
- Simultaneous selection: Ensure that when both players click the same character at roughly the same time, the color eventually settles on the "both" color due to NetworkVariable synchronization.
- Single player mode: Verify that only the player's color is shown.

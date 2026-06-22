# Project Overview
- Game Title: MR Tabletop Fighting Game
- Goal: Increase the size of the Character Selection UI and the Play (Start) button to improve visibility and usability in MR.

# UI Changes
## Character Selection UI
- Current Width: 0.1m (Too small)
- Target Width: ~1.2m
- Adjust `sizeDelta` of the root panel, title, grid, and individual slots to be ~12x larger.
- Increase font sizes for the Title and Timer.

## Start Button (Play Button)
- Current Width: 1.0m
- Target Width: ~1.5m
- Increase `localScale` or `sizeDelta` to make it more prominent.

# Key Asset & Context
- `PreGameUI` (Parent)
- `CharacterSelectionUI` (Child)
- `StartButton` (Child)

# Implementation Steps
1. **Resize CharacterSelectionUI**:
   - Set `CharacterSelectionUI` sizeDelta to `(120, 60)`.
   - Update `Background` to match.
   - Adjust `Title` anchored position and font size.
   - Adjust `SelectionTimer` anchored position and font size.
2. **Update Grid and Slots**:
   - Set `Grid` container size and spacing.
   - Set each `Slot` sizeDelta to `(20, 30)`.
   - Ensure images/sprites are preserved.
3. **Resize StartButton**:
   - Set `StartButton` localScale to `(4, 4, 4)`.
   - Adjust its vertical position to avoid overlapping the selection panel.

# Verification & Testing
- Activate `PreGameUI` in the Editor.
- Verify the UI elements are comfortably visible and correctly proportioned on the virtual table.
- Verify the "Fight!" button does not overlap the character slots.

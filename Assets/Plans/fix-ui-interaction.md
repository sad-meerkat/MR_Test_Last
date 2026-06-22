# Project Overview
- Game Title: MR Tabletop Fighting Game
- Issue: Character Selection UI buttons are not clickable.
- Cause: `CharacterSelectionUI` has its own `Canvas` and `GraphicRaycaster` while being a child of `PreGameUI` (which is already a World Space Canvas). This nesting conflicts with event processing.

# Game Mechanics
- (No changes)

# UI
## Character Selection UI
- Remove `Canvas`, `CanvasScaler`, and `GraphicRaycaster` from `CharacterSelectionUI`.
- Let it inherit the Canvas properties from `PreGameUI`.
- Reset local scale to `(1, 1, 1)` so it uses the parent's `0.01` scale.

# Key Asset & Context
- `CharacterSelectionUI` GameObject in `MR_Test_Backup UI.unity`.

# Implementation Steps
1. **Clean up CharacterSelectionUI**:
   - Remove `Canvas` component.
   - Remove `CanvasScaler` component.
   - Remove `GraphicRaycaster` component.
2. **Adjust Transform**:
   - Set `Local Scale` to `(1, 1, 1)`.
   - Adjust `Size Delta` and `Anchored Position` if necessary to fit within the `PreGameUI` bounds.
3. **Verify Button Components**:
   - Ensure all `Slot` objects have `Raycast Target` checked on their `Image` or `RawImage` components.

# Verification & Testing
- Click "Fight!" button: Verify Selection UI appears.
- Attempt to click character slots: Verify they respond (visual highlight or debug log).

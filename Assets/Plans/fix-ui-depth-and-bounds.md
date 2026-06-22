# Project Overview
- Game Title: MR Tabletop Fighting Game
- Issue: Character Selection UI is visible but not interactable.
- Diagnosis:
  1. **Z-Offset**: `CharacterSelectionUI` has a Z-local position of 14, while the `StartButton` is at 0. This depth difference often causes raycast issues in MR interaction.
  2. **Bounding Box**: Part of the expanded selection UI is outside the `PreGameUI` RectTransform bounds, which can clip raycasts.

# UI Changes
## Character Selection UI
- Set `Local Z Position` to **0**.
- Adjust `Anchored Y Position` to **40** to maintain separation from the "Fight!" button.

## PreGameUI (Parent)
- Increase `sizeDelta` to **(150, 150)** to fully encompass the larger child elements and ensure raycasts are processed for the entire area.

# Key Asset & Context
- `PreGameUI` (Parent Canvas)
- `CharacterSelectionUI` (GameObject)

# Implementation Steps
1. **Fix Depth**: Align `CharacterSelectionUI` to the same Z-plane as the parent and other buttons.
2. **Expand Interaction Area**: Resize the parent `PreGameUI` RectTransform.
3. **Verify Layers**: Ensure all elements are on the correct layer for the XR Interactor.

# Verification & Testing
- Open the scene and verify `CharacterSelectionUI` Z-position is 0.
- Start the game and attempt to click character slots.
- Verify visual highlights on hover (if applicable).

# Project Overview
- Issue: Health bar UI is invisible because its world scale is too large (800m wide) and its height is too low.
- Solution: Rescale `InGameUI` to a reasonable tabletop size (0.001) and set a proper world height.

# Implementation Steps

## Step 1: Rescale and Reposition InGameUI
- **Description**: In `FightingGameManager.cs`, set the `InGameUI` scale to `0.001` (to make the 800px HUD approx 80cm wide) and ensure the `FightingHUD` child is at a visible height.
- **Assigned role**: developer
- **Files**: `Assets/MRTabletopAssets/Games/FightingGame/Scripts/FightingGameManager.cs`
- **Action**:
    - Update `UpdateUIState` to:
      - Set `m_InGameUI.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f)`.
      - Set `m_InGameUI.transform.localPosition = Vector3.zero`.
      - Find the `FightingHUD` child and set its `localPosition` to `new Vector3(0, 600, 0)` (60cm height).
- **Dependencies**: None

# Verification & Testing
1. **Play Mode**: Start the game.
2. **Visual Check**: Verify the health bar UI appears floating above the table (approx 60cm high, 80cm wide).
3. **Health Sync**: Verify the bars still update when the dummy is hit.

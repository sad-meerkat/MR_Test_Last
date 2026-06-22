# Project Overview
- Game Title: MR Tabletop (Fighting Game Mode)
- Problem: 
    - "READY" button in the match result UI is facing the wrong way (backward/upside down).
    - The "Win/Complete" images in the match result UI are positioned too low.
- Target: `MatchResultUI` and `FightingHUD` under `InGameUI`.

# Game Mechanics
- When a game ends, `MatchResultUI` is displayed.
- It contains character-specific win images and a "READY" button to restart.

# Key Assets & Context
- `Assets/MRTabletopAssets/Games/FightingGame/Prefabs/InGameUI` (or scene instance)
- `MatchResultUI`: Child of `InGameUI`. Currently has `(0, 180, 0)` local rotation.
- `FightingHUD`: Child of `InGameUI`. Currently has `(0, 180, 0)` local rotation.
- `RuReadyButton_Result`: Child of `MatchResultUI`. Currently has `(0, 180, 0)` local rotation.
- `RawImage` / `RawImage (1)`: "Win" images in `MatchResultUI`. Currently at `Y = 0.55`.

# Implementation Steps
1.  **Correct UI Rotations**:
    - Select `MatchResultUI` and set its **Local Rotation** to `(0, 0, 0)`.
    - Select `FightingHUD` and set its **Local Rotation** to `(0, 0, 0)`.
    - Select `RuReadyButton_Result` (and any other flipped children) and set their **Local Rotation** to `(0, 0, 0)`.
    - This ensures all UI elements face the same direction as the parent `InGameUI` (which is controlled by `SeatBillboard`).
2.  **Adjust Win Image Position**:
    - Select `RawImage` and `RawImage (1)` inside `MatchResultUI`.
    - Increase their **Anchored Position Y** from `0.55` to something higher (e.g., `0.7` or `0.8`) as per user's "a bit low" feedback.

# Verification & Testing
1.  **Manual Test**:
    - Play the fighting game until a match ends.
    - Check the `MatchResultUI`.
    - **Verify**: The "READY" button text should be clearly readable and facing the player.
    - **Verify**: The win image should be positioned at a comfortable height (not too low).
    - **Verify**: `FightingHUD` should also face the player correctly during the match.

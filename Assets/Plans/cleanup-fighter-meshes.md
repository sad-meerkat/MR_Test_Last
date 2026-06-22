# Project Overview
- Game Title: MR Tabletop Fighting Game
- Issue: Sasuke/Byakuya spawn with a robot mesh (Gundam) attached.
- Root Cause: When creating the new fighter prefabs, the original robot mesh (named "scene") was not removed, resulting in two models being part of the same prefab.

# Implementation Steps
1. **Clean Fighter Prefabs**:
   - Open `ByakuyaFighter.prefab` and `SasukeFighter.prefab`.
   - Delete the child object named `scene` (which contains the original robot mesh).
2. **Verify Prefab Hierarchy**:
   - Ensure only the intended character model (`Model` child) and necessary logic components remain.

# Verification & Testing
- Spawn the characters in-game.
- Verify that only Sasuke or Byakuya is visible, with no overlapping robot mesh.

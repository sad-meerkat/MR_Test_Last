# Project Overview
- Game Title: MR Tabletop Fighting Game
- Issue: Character movement becomes inverted or incorrect ("keys swapped") after some time.
- Cause: `FighterController.cs` uses `transform.Translate(move, Space.Self)` where `move` is a direction vector from the joystick, but also updates `transform.localRotation` to face that direction. This creates a conflict where moving "Backward" relative to the world causes the character to face "Backward" and then move in its *own* negative Z (Forward relative to world), leading to inverted movement (moonwalking).

# Game Mechanics
## Movement Logic
- Movement should be relative to the **Table** (parent transform) or a fixed frame of reference.
- Joystick input should always correspond to the same direction on the table, regardless of which way the character is currently facing.

# Key Asset & Context
- `Assets/MRTabletopAssets/Games/FightingGame/Scripts/FighterController.cs`: Needs a logic fix for movement.

# Implementation Steps
1. **Analyze FighterController.cs**:
   - Identify the problematic `Translate(..., Space.Self)` and `LookRotation` lines.
2. **Implement Fixed-Reference Movement**:
   - Change the movement logic to use `localPosition` or `Space.World` to ensure joystick "Up" always moves in the same direction relative to the table.
   - Separate rotation from the movement vector application to avoid the "Moonwalking" feedback loop.
3. **Handle Parent-Relative Movement**:
   - Ensure the character moves along the Table's axes if it's parented to a table, which is best for MR/Tabletop consistency.

# Verification & Testing
1. **Manual Move Test**:
   - Push Joystick Up: Character should move "Away" and face "Away".
   - Push Joystick Down: Character should move "Towards" and face "Towards".
   - Verify that movement direction never flips/swaps regardless of how many times the character turns.
2. **Table Rotation Test**:
   - If possible, rotate the table 90 degrees and verify that Joystick "Up" still moves "Up on the table".

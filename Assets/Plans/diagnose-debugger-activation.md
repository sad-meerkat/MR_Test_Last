# Project Overview
- Game Title: MR Tabletop / Bleach MR
- Issue: `Hand Pose Debugger` remains inactive even after summoning a character in the `핸즈 스킬 적용` scene.
- Goal: Diagnose and fix the activation of `Hand Pose Debugger` and other gesture-related UI.

# Game Mechanics
## Summoning and Skill Activation
- Players summon a character using `TableCharacterSummoner`.
- Summoning should trigger the `OnSummoned` UnityEvent.
- `OnSummoned` is wired to activate `CharacterGestures`, `Hand Pose Debugger`, `Advanced Pose Debugger`, and `Hand Shape Debugger`.

# Key Asset & Context
- **Scene**: `Assets/Scenes/핸즈 스킬 적용.unity`
- **Script**: `Assets/Scripts/MR_Character/TableCharacterSummoner.cs`
- **Objects**: `Virtual Table` (Summoner), `Hand Pose Debugger`.

# Diagnostic Plan
1. **Run Play Mode Test**: Verify if the `OnSummoned` event actually fires and if the targets are correctly activated at runtime.
2. **Inspect Serialization**: Check if the persistent listeners are correctly saved in the scene file (done previously, appeared correct).
3. **Verify Code Path**: Ensure no runtime errors prevent the event from firing (e.g., `Instantiate` failing, null references).

# Implementation Steps (Hypothetical Fixes)
1. **Fix 1: Direct Reference in Code**: If the UnityEvent is unreliable, add direct serialized fields for the debuggers to `TableCharacterSummoner` and activate them in `Summon()`.
2. **Fix 2: Ensure Scene Loaded Correctly**: Verify if the `Hand Pose Debugger` is indeed in the scene and not a prefab instance that was lost.

# Verification & Testing
- Use a dedicated Play Mode test to automate the summoning and check activation states.
- Manual verification in the Unity Editor with the console open for errors.

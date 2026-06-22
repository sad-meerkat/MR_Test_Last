# Project Overview
- Game Title: Tabletop Fighting Game (MR)
- Goal: Fix performance lag in VR/MR builds caused by excessive logging and unoptimized effect management.
- Target Platform: Android (Meta Quest)

# Performance Analysis
1. **Logging**: Frequent `Debug.Log` calls in `Update` loops and trigger events cause overhead, especially in builds where log processing is still active.
2. **Effects**: `Instantiate` and `Destroy` are used for every projectile and visual effect (e.g., in `CharacterBarrageManager` and `FireballComboLauncher`). This causes memory allocation spikes and Garbage Collection (GC) pauses.
3. **CPU Heavy Updates**: `MicrophoneManager` processes audio spectrum data and updates `LineRenderer` every frame.

# Implementation Steps

## 1. Implement Object Pooling for Projectiles
- **Description**: Replace `Instantiate` and `Destroy` with a pooling system for frequently spawned objects like fireballs and barrage projectiles.
- **Files**: 
    - `Assets/MRTabletopAssets/Scripts/Character/CharacterBarrageManager.cs`
    - `Assets/MRTabletopAssets/Scripts/Character/FireballComboLauncher.cs`
    - `Assets/MRTabletopAssets/Scripts/Character/GestureProjectileLauncher.cs`
- **Assigned role**: developer
- **Dependencies**: Use existing `Pooler.cs` if compatible, otherwise implement a simple `SimpleObjectPool`.
- **Parallelizable**: Yes

## 2. Disable/Optimize Logs for Build
- **Description**: Wrap non-critical `Debug.Log` calls in `#if UNITY_EDITOR` blocks to ensure they don't run in the Android build.
- **Files**: 
    - `Assets/MRTabletopAssets/Scripts/Character/TableCharacterInput.cs`
    - `Assets/MRTabletopAssets/Scripts/Character/HandSwordSkill.cs`
    - `Assets/Scripts/VRButtonTouch.cs`
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes

## 3. Optimize Microphone Spectrum Analysis
- **Description**: 
    - Lower the frequency of `GetSpectrumData` calls (e.g., every 2 or 3 frames).
    - Disable the `LineRenderer` visualization by default in the build unless explicitly needed.
- **Files**: `Assets/Scripts/MicrophoneManager.cs`
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes

## 4. Optimize Physics for Projectiles
- **Description**: Change `CollisionDetectionMode.ContinuousDynamic` to `Discrete` for projectiles that don't move fast enough to skip through thin walls, or only use it for the local player's visual.
- **Files**: `Assets/MRTabletopAssets/Scripts/Character/CharacterBarrageManager.cs` etc.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes

# Verification & Testing
1. **Profiler Test**: Run the game with the Unity Profiler. Check the "CPU Usage" and "Memory" (GC Alloc) sections during combat.
2. **Log Verification**: Ensure the Android logcat doesn't show constant spamming from inputs or sword swings.
3. **Visual Integrity**: Ensure pooled objects reset their state (velocity, position, active status) correctly when reused.

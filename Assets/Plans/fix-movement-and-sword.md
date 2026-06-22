# Project Overview
- Goal: Fix character movement and resolve sword spawning/scaling conflicts.
- Issues:
    1. Character does not move with controller (snapping back due to server-authoritative NetworkTransform).
    2. Sword is too big and spawns at the wrong position (conflict with MRSkillManager world-sword).

# Implementation Steps

## Step 1: Fix Character Movement Authority
- **Description**: Replace the standard `NetworkTransform` with `ClientNetworkTransform` on the fighter prefabs to allow local movement to sync to the server.
- **Prefabs**: `Assets/MRTabletopAssets/Games/FightingGame/Prefabs/ByakuyaFighter.prefab`, `SasukeFighter.prefab`
- **Assigned role**: developer

## Step 2: Resolve Sword Conflicts
- **Description**: Remove the world-space sword from the automatic activation system.
- **Scene**: `Assets/Scenes/핸즈 스킬 적용.unity`
- **Object**: `Virtual Table/MRSkillManager`
- **Action**: Remove `Cherrykatana` from the `m_ObjectsToActivate` array.
- **Assigned role**: developer

## Step 3: Simplify Character Sword Scale
- **Description**: Simplify the sword activation logic to trust local prefab scaling instead of complex world-scale normalization.
- **File**: `Assets/MRTabletopAssets/Scripts/Character/TableCharacter.cs`
- **Changes**: Remove the manual `lossyScale` normalization in `SetSwordActive`.
- **Assigned role**: developer

# Verification & Testing
- **Movement**: Join as Host/Client. Move character. Verify no snapping occurs.
- **Sword**: Perform Fist gesture. Verify ONLY the hand-held sword appears (at correct scale).

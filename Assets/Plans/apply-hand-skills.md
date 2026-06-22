# Project Overview
- Game Title: MR Tabletop / Bleach MR
- High-Level Concept: An MR experience where players can trigger skills using hand gestures.
- Goal: Apply the hand gesture-based skill system from the `Bleach_OvrHands` scene to the `핸즈 스킬 적용` scene.

# Game Mechanics
## Core Gameplay Loop
The player uses specific hand gestures (Fist Bump, Shaka, Thumbs Up, Open Palm Up) to trigger visual skill effects that appear on their hands or weapons.
## Controls and Input Methods
- Hand Tracking (Meta Quest)
- Gesture Detection via `StaticHandGesture` and `XRHandTrackingEvents`.

# Key Asset & Context
- **Scenes**: 
    - Source: `Assets/Scenes/Bleach_OvrHands.unity`
    - Target: `Assets/Scenes/핸즈 스킬 적용.unity`
- **Scripts**:
    - `UnityEngine.XR.Hands.Samples.GestureSample.SkillEffectSpawner`
    - `UnityEngine.XR.Hands.Samples.GestureSample.StaticHandGesture`
- **Data Assets (SkillData)**:
    - `NewSkillData 1` (Shaka)
    - `NewSkillData 2` (Fist Bump)
    - `Skill 5` (Open Palm Up)
    - `Skill 6` (Thumbs Up)
- **Hand Poses**:
    - `Fist Bump.asset`
    - `Shaka.asset`
    - `Thumbs Up.asset`
    - `Open Palm Up.asset`

# Implementation Steps

## 1. Gesture Detection Setup
- Open the `핸즈 스킬 적용` scene.
- Navigate to `MRInteractionSetup/XR Origin (XR Rig)/CharacterGestures`.
- Create or configure child objects for each skill gesture:
    - **FistBumpGesture**: Add `StaticHandGesture`, set `Hand Shape Or Pose` to `Fist Bump.asset`. Link `Hand Tracking Events` to `RightHandGesture`'s tracking component.
    - **ShakaGesture**: Add `StaticHandGesture`, set `Hand Shape Or Pose` to `Shaka.asset`.
    - **ThumbsUpGesture**: Add `StaticHandGesture`, set `Hand Shape Or Pose` to `Thumbs Up.asset`.
    - **OpenPalmUpGesture**: Configure existing or add new, set `Hand Shape Or Pose` to `Open Palm Up.asset`.

## 2. Skill Spawner Setup (Weapons)
- Locate `Shilrd` and `Cherrykatana` in the scene.
- **Shilrd**: Add `SkillEffectSpawner`. Set `Skill Data` to `Skill 5`, `Gesture` to `OpenPalmUpGesture`.
- **Cherrykatana**: Add `SkillEffectSpawner`. Set `Skill Data` to `Skill 6`, `Gesture` to `ThumbsUpGesture`.

## 3. Skill Spawner Setup (Hands)
- Navigate to `MRInteractionSetup/XR Origin (XR Rig)/OVRCameraRig/TrackingSpace/RightHandAnchor`.
- Create a child object `SkillSpawner_Fist`: Add `SkillEffectSpawner`. Set `Skill Data` to `NewSkillData 2`, `Gesture` to `FistBumpGesture`.
- Create a child object `SkillSpawner_Shaka`: Add `SkillEffectSpawner`. Set `Skill Data` to `NewSkillData 1`, `Gesture` to `ShakaGesture`.

## 4. Verification
- Enter Play Mode with hand tracking enabled.
- Perform each gesture and verify:
    - Fist Bump triggers the "Fist" skill effect at the right hand.
    - Shaka triggers the "Shaka" skill effect.
    - Thumbs Up triggers the effect on `Cherrykatana`.
    - Open Palm Up triggers the effect on `Shilrd`.

# Verification & Testing
- **Gesture Reliability**: Ensure the `Minimum Hold Time` in `StaticHandGesture` is consistent with the source scene (default 0.2s).
- **Visual Alignment**: Check if `Spawn Offset` in `SkillData` assets correctly aligns the effects with hands/weapons.
- **Cleanup**: Verify effects are destroyed immediately when the gesture ends.

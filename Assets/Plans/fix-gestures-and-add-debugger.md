# Project Overview
- Goal: Fix hand gestures not working after summoning and integrate the Pose Debugger from `Bleach_OvrHands`.
- Implementation: Copy debugger assets, fix event wiring, and ensure all systems are correctly activated upon character summoning.

# Key Asset & Context
- **Source Scene**: `Assets/Scenes/Bleach_OvrHands.unity`
- **Target Scene**: `Assets/Scenes/핸즈 스킬 적용.unity`
- **Debuggers**: `Hand Pose Debugger`, `Advanced Pose Debugger`
- **Summoner**: `Virtual Table` with `TableCharacterSummoner`

# Implementation Steps

## 1. Import Debuggers
- Copy `Hand Pose Debugger` and `Advanced Pose Debugger` from the source scene to the target scene.
- Place them as root objects or under a specific container.
- Set them to be **Inactive** by default.

## 2. Fix Gesture Tracking References
- Ensure all `StaticHandGesture` components in `CharacterGestures` are linked to a valid `XRHandTrackingEvents` component that is part of the `XR Origin`.
- Specifically, verify if `RightHandGesture` and `LeftHandGesture` objects themselves should be active for the tracking to work.

## 3. Update Summoner Wiring
- Update the `OnSummoned` event in the `핸즈 스킬 적용` scene.
- Add listeners to activate:
    - `Hand Pose Debugger`
    - `Advanced Pose Debugger`

## 4. Troubleshooting & Verification
- Check if the `SkillEffectSpawner` on `SkillSpawner_Fist` and `SkillSpawner_Shaka` have correct `Spawn Point` references.
- Verify in Play Mode:
    - Debbugers appear after summoning.
    - Gesture recognition status is visible on the debugger.
    - Skills trigger at the correct locations.

# Verification & Testing
- **Debugger Visibility**: Confirm the UI/Gizmos for pose debugging appear after summoning.
- **Skill Triggering**: Confirm that each of the 4 gestures (Fist, Shaka, Thumbs Up, Palm Up) triggers its respective effect.
- **Console Check**: Monitor for "Missing Reference" or "Null Reference" errors during play.

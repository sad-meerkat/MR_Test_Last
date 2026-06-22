# Project Overview
- Goal: Enable hand skills only after a character has been summoned.
- Implementation: Add a summoning event to the character summoner script and use it to activate the hand gesture detection system.

# Game Mechanics
- Hand skills are initially locked.
- Character summoning triggers the unlocking of hand skills.

# Key Asset & Context
- **Script**: `Assets/Scripts/MR_Character/TableCharacterSummoner.cs`
- **Scene**: `Assets/Scenes/핸즈 스킬 적용.unity`
- **GameObject**: `CharacterGestures` (at `MRInteractionSetup/XR Origin (XR Rig)/CharacterGestures`)

# Implementation Steps

## 1. Modify TableCharacterSummoner Script
- Add `using UnityEngine.Events;`
- Add `[SerializeField] UnityEvent m_OnSummoned;`
- In the `Summon()` method, invoke `m_OnSummoned` after the character is instantiated.

## 2. Configure Scene
- Open `핸즈 스킬 적용` scene.
- Find the `CharacterGestures` GameObject.
- Set its active state to **False** (inactive) in the Inspector.
- Find the `Virtual Table` object which has the `TableCharacterSummoner` component.
- Add a persistent listener to the newly created `OnSummoned` event.
- Wire the listener to target the `CharacterGestures` GameObject and call `GameObject.SetActive(true)`.

# Verification & Testing
- **Pre-summoning Test**: Enter play mode, perform gestures (Fist, Shaka, etc.). No skills should appear.
- **Summoning Test**: Trigger the character summon (likely via UI button or Start if `m_SummonOnStart` is true).
- **Post-summoning Test**: Perform gestures. Skills should now trigger normally.
- **Scene Consistency**: Check if this affects other scenes (e.g., if other scenes expect gestures to be on by default). Note: `TableCharacterSummoner` modification adds a serialized event, but the wiring is per-scene.

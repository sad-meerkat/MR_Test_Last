# Project Overview
- Game Title: MR Tabletop Characters
- High-Level Concept: Players control characters (Sasuke, Byakuya) on a virtual tabletop using hand gestures and physical motions in MR.
- Players: Single player / Multiplayer (Netcode for GameObjects)
- Inspiration / Reference Games: Naruto, Bleach
- Tone / Art Direction: Anime / Stylized
- Target Platform: Android (Meta Quest)
- Render Pipeline: URP
- Input System: New Input System + XR Hands

# Game Mechanics
## Core Gameplay Loop
- Players perform hand gestures to trigger character-specific skills.
- Players perform "swing" motions with their hands to trigger sword attacks on the tabletop characters.
- Characters have unique skill combos (Sasuke: Fireball, Byakuya: Barrage).

## Controls and Input Methods
- **Hand Gestures**: Uses XR Hands `StaticHandGesture` components.
    - Naruto Combo: Fist → Open Palm → Point At.
    - Bleach Combo: 3 specific gestures (e.g., Katana Pose, etc.).
- **Physical Motion**: Hand velocity is used to detect sword swings.
- **UI**: Gesture feedback and cooldowns are displayed via World Space Canvas.

# UI
- Cooldown Icons: Circular fill images indicating skill availability.
- Gesture Status: Text or icon feedback showing the current state of a combo.

# Key Asset & Context
- `FireballComboLauncher.cs`: Handles Sasuke's fireball logic.
- `ComboBarrageLauncher.cs`: Handles Byakuya's barrage logic.
- `HandSwordSkill.cs`: Detects hand swings.
- `TableCharacter.cs`: Base character class for tabletop fighters.
- `CharacterSkillBridge.cs` (New): Orchestrates gesture logic between player hands and the active tabletop character.
- `SasukeFighter` Prefab: Naruto-themed character.
- `ByakuyaFighter` Prefab: Bleach-themed character.

# Implementation Steps
## Step 1: Standardize Hand System
- **Description**: Ensure the scene uses XR Hands as the primary source for gesture recognition to match the logic from the Naruto/Bleach scenes.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: No

## Step 2: Implement Character Skill Bridge
- **Description**: Create `CharacterSkillBridge.cs` to detect the currently controlled character and route events from `FireballComboLauncher` or `ComboBarrageLauncher` to that character.
- **Assigned role**: developer
- **Dependencies**: Step 1
- **Parallelizable**: Yes

## Step 3: Configure Naruto Logic (Sasuke)
- **Description**: Add `FireballComboLauncher` to the scene. Configure the `StaticHandGesture` sequence (Fist, Open Palm, Point At). Wire the "Fireball" launch event to trigger Sasuke's skill effect.
- **Assigned role**: developer
- **Dependencies**: Step 2
- **Parallelizable**: Yes

## Step 4: Configure Bleach Logic (Byakuya)
- **Description**: Add `ComboBarrageLauncher` to the scene. Configure the corresponding gesture sequence. Wire the "Barrage" event to trigger Byakuya's skill effect (Cherry Blossom projectiles).
- **Assigned role**: developer
- **Dependencies**: Step 2
- **Parallelizable**: Yes

## Step 5: Enhanced Sword Summoning & Swinging
- **Description**: 
    - Update `TableCharacter.cs` to automatically activate `m_SwordVisual` when `PerformSwordSwing()` is called.
    - Implement a `Coroutine` in `TableCharacter` to deactivate the sword after the animation finishes (or after a fixed duration).
    - Ensure `HandSwordSkill.cs` on the player's hand correctly triggers `TableCharacterInput.Instance.OnHandSwing()`.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes

## Step 6: UI Integration
- **Description**: Replicate the Gesture UI from the Naruto/Bleach scenes, including cooldown images and status text, and connect them to the launcher scripts.
- **Assigned role**: developer
- **Dependencies**: Steps 3, 4
- **Parallelizable**: Yes

# Verification & Testing
- **Gesture Test**: Verify that performing the Fist -> Open Palm -> Point At sequence triggers the fireball ONLY when Sasuke is selected.
- **Barrage Test**: Verify that the Bleach combo triggers the barrage ONLY when Byakuya is selected.
- **Swing Test**: Verify that moving the hand quickly (swing) while a "Katana" gesture is held causes the tabletop character to summon its sword and play the attack animation.
- **Visual Check**: Ensure the sword visual appears during the attack and disappears afterwards.
- **Multiplayer Check**: Verify that skill effects and sword visuals are synced across the network (via ServerRpc/ClientRpc).

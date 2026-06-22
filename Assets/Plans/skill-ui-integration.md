# Project Overview
- **Game Title**: MR Tabletop Fighter (Skill Integration)
- **High-Level Concept**: Integrating character-specific skill combos (Sasuke and Byakuya) with a centralized HUD UI (Hand Pose Debugger).
- **Players**: Single player (Hand Tracking)
- **Target Platform**: Android (Meta Quest)
- **Render Pipeline**: URP

# Game Mechanics
## Core Gameplay Loop
- Perform 3-gesture sequences to trigger special skills.
- Sasuke: Fist -> Open Palm -> Point At => Fireball.
- Byakuya: (To be determined, likely Fist -> Thumbs Up -> Palm Up) => Cherry Blossom Barrage.
- UI feedback via Hand Pose Debugger shows combo progress and cooldown.

# UI
- **Character Specific Debuggers**: Separate HUDs for Sasuke and Byakuya.
- **Sasuke Pose Debugger**: Custom HUD for Sasuke, replacing the default when Sasuke is active.
- **Byakuya Pose Debugger**: Custom HUD for Byakuya, replacing the default when Byakuya is active.
- **Combo Overlay**: Integrated into these debuggers to show progress and cooldown.

# Key Asset & Context
- **Scripts**: 
    - `CharacterSkillBridge.cs`: Modified to toggle specific debugger GameObjects.
    - `FireballComboLauncher.cs`: Logic for Sasuke's skill.
    - `ComboBarrageLauncher.cs`: Logic for Byakuya's skill.
    - `ComboDisplay.cs`: UI controller for step progress and cooldown.
- **Prefabs**:
    - `FireBall.prefab`: Particle effect for the fireball.
    - `CherryKatana.prefab`: Particle effect for Byakuya's skill.

# Implementation Steps
## 1. UI Integration (Scene: 핸즈 스킬 적용 2)
- **Description**: Add combo icon sets to the existing `Hand Pose Debugger`.
- **Details**:
    - Navigate to `MRInteractionSetup/Hand Pose Debugger/UI Canvas/Right Hand Gesture Detection/Gesture Icons`.
    - Create `FireballComboUI` and `BarrageComboUI` containers.
    - Add 3 Step Images and 1 Cooldown Image to each.
    - Attach `ComboDisplay` to each and link its fields.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes

## 2. CharacterSkillBridge Refactoring
- **Description**: Update `CharacterSkillBridge.cs` to toggle character-specific debuggers.
- **Details**:
    - Add `[SerializeField] private GameObject m_SasukeDebugger;`
    - Add `[SerializeField] private GameObject m_ByakuyaDebugger;`
    - Add `[SerializeField] private GameObject m_DefaultDebugger;`
    - Update `OnCharacterChanged` to manage active state of these GameObjects.
- **Assigned role**: developer
- **Dependencies**: Step 1
- **Parallelizable**: No

## 3. Scene Setup (핸즈 스킬 적용 2)
- **Description**: Add launchers and bridges to the scene and configure references.
- **Details**:
    - Create `SkillManager` GameObject.
    - Add `FireballComboLauncher`, `ComboBarrageLauncher`, and `CharacterSkillBridge`.
    - Link `FireballComboLauncher` to Right Hand gestures and `FireBall` prefab.
    - Link `ComboBarrageLauncher` to Byakuya's gestures and VFX prefabs.
    - Link `CharacterSkillBridge` to the launchers and the newly created UI.
- **Assigned role**: developer
- **Dependencies**: Step 1, 2
- **Parallelizable**: No

# Verification & Testing
- **Manual Check**: Select Sasuke -> Perform 3 gestures -> Verify Fireball fires and UI updates.
- **Manual Check**: Select Byakuya -> Perform 3 gestures -> Verify Barrage fires and UI updates.
- **UI Check**: Verify icons follow the gaze (GazeFollowUI).

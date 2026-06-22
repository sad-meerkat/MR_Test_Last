# Project Overview
- **Game Title**: MR Tabletop Fighter (Skill Integration)
- **High-Level Concept**: Integrating character-specific skill combos with character-specific HUD UIs (Sasuke/Byakuya Pose Debuggers).
- **Players**: Single player (Hand Tracking)
- **Target Platform**: Android (Meta Quest)
- **Render Pipeline**: URP

# Game Mechanics
## Core Gameplay Loop
- Perform 3-gesture sequences to trigger special skills.
- Sasuke: Fist -> Open Palm -> Point At => Fireball.
- Byakuya: Fist -> Thumbs Up -> Palm Up (Verification needed) => Cherry Blossom Barrage.
- UI feedback via dedicated character debuggers.

# UI
- **Sasuke Pose Debugger**: Custom HUD for Sasuke.
- **Byakuya Pose Debugger**: Custom HUD for Byakuya.
- **Hand Pose Debugger**: Default HUD for other characters.
- **Combo Overlay**: Integrated into these debuggers.

# Key Asset & Context
- **Scripts**: 
    - `CharacterSkillBridge.cs`: Toggles specific debugger GameObjects.
    - `FireballComboLauncher.cs`: Sasuke skill logic.
    - `ComboBarrageLauncher.cs`: Byakuya skill logic.
    - `ComboDisplay.cs`: UI controller for steps/cooldown.

# Implementation Steps
## 1. CharacterSkillBridge Update
- **Description**: Add debugger GameObject references and toggle logic.
- **Files**: `Assets/MRTabletopAssets/Scripts/Character/CharacterSkillBridge.cs`
- **Assigned role**: developer

## 2. ComboDisplay Setup in Scene
- **Description**: Attach and configure `ComboDisplay` components on the character-specific debuggers in the scene.
- **Details**: 
    - Attach to `Sasuke Pose Debugger/.../FireBall` and `Byakuya Pose Debugger/.../CherryKatana`.
    - Link step images and cooldown images.
- **Assigned role**: developer

## 3. Launcher Configuration
- **Description**: Link launchers to the gestures and UI components in the character-specific debuggers.
- **Details**:
    - `FireballComboLauncher` -> `Sasuke Pose Debugger` gestures.
    - `ComboBarrageLauncher` -> `Byakuya Pose Debugger` gestures.
- **Assigned role**: developer

## 4. Bridge Linkage
- **Description**: Finalize references in `CharacterSkillBridge` in the scene.
- **Assigned role**: developer

# Verification & Testing
- Select Sasuke -> verify Sasuke Debugger appears, Hand Pose Debugger disappears.
- Perform combo -> verify UI feedback and skill execution.
- Select Byakuya -> verify Byakuya Debugger appears.
- Select other/none -> verify Default Hand Pose Debugger appears.

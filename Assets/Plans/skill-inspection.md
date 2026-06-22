# Project Overview
- Game Title: MR Tabletop / Hands Skill Project
- High-Level Concept: An MR experience where hand gestures trigger skills and character actions on a virtual table.
- Players: Single player (MR)
- Target Platform: Android (Meta Quest)
- Input System: XR Hands (Gestures) + New Input System
- Render Pipeline: URP

# Game Mechanics
## Core Gameplay Loop
- Players use hand gestures (Thumbs Up, Palm Up, Shaka, Fist) to trigger special skills.
- A sword skill can be triggered by a physical swing motion.
- `MRSkillManager` controls which skill-related objects are active at any given time.

## Controls and Input Methods
- **Gestures**: Detected via `StaticHandGesture` components using XR Hands tracking.
- **Motion**: `HandSwordSkill` detects swing velocity to trigger attacks.

# Key Asset & Context
- `MRSkillManager`: Located on `Virtual Table`. Manages activation of skill spawners.
- `SkillEffectSpawner`: Handles instantiation of skill effects when a gesture is performed.
- `StaticHandGesture`: Configures the specific hand shape and handedness required.
- `Cherrykatana`: A skill spawner that appears to be missing from the activation list.

# Implementation Steps (Inspection Plan)
## Step 1: Check MRSkillManager Configuration
- **Description**: Verify if the "Cherrykatana" (or whichever skill is not working) is included in the `MRSkillManager`'s activation list.
- **Assigned role**: explorer
- **Dependencies**: None
- **Parallelizable**: Yes

## Step 2: Verify Gesture and Handedness
- **Description**: Check the `StaticHandGesture` component on the failing skill object. Verify:
    1. `Hand Tracking Events` is assigned (e.g., to `RightHandQuestVisual`).
    2. `Hand Shape Or Pose` is not null.
    3. Handedness matches the hand being used.
- **Assigned role**: explorer
- **Dependencies**: None
- **Parallelizable**: Yes

## Step 3: Check Console for Logs
- **Description**: Run the game and look for specific logs:
    - `[MRSkillManager] Activated: [Name]`
    - `[SkillEffectSpawner] Gesture 가 연결되지 않았습니다`
- **Assigned role**: explorer
- **Dependencies**: None
- **Parallelizable**: Yes

# Verification & Testing
- **Manual Check**: Add `Cherrykatana` to `MRSkillManager` and see if it activates.
- **Play Mode Test**: If possible, simulate gestures or check if the `m_WasDetected` flag in `StaticHandGesture` changes in the inspector during play.

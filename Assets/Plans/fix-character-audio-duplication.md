# Project Overview
- Game Title: Tabletop Fighting Game (MR)
- High-Level Concept: Character-based fighting game with voice and effect sounds triggered by movement and actions.
- Issue: Movement sounds (specifically "StartMoveVoice") play multiple times (duplication) and continue playing even after the character has stopped moving.

# Game Mechanics
## Core Gameplay Loop
Characters move on a tabletop. Sounds are triggered when movement starts, sprints begin, or jumps occur.
## Controls and Input Methods
XR Controllers or Gamepads provide Vector2 input for movement.

# UI
N/A (Audio focused)

# Key Asset & Context
- `TableCharacter.cs`: Contains the movement and audio trigger logic.
- `m_StartMoveVoice`, `m_SprintVoice`, `m_JumpVoice`: Audio clips used for different states.
- `AudioSource.PlayClipAtPoint`: Currently used method, which creates unmanaged audio sources that play to completion and can overlap.

# Implementation Steps
## 1. Add AudioSource Management to TableCharacter
- **Description**: Add a dedicated `AudioSource` for voices/effects to the `TableCharacter` class to allow for stopping and preventing overlaps.
- **Files**: `Assets/MRTabletopAssets/Scripts/Character/TableCharacter.cs`
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes

## 2. Refactor Move Audio Logic
- **Description**: 
    - Implement a cooldown for `m_StartMoveVoice` to prevent rapid re-triggering due to input jitter.
    - Replace `AudioSource.PlayClipAtPoint` with `m_AudioSource.Play()` or `m_AudioSource.PlayOneShot()`.
    - Stop the `AudioSource` when movement stops if a voice is playing.
    - Increase the movement threshold slightly (e.g., from 0.1 to 0.15) for better stability.
- **Files**: `Assets/MRTabletopAssets/Scripts/Character/TableCharacter.cs`
- **Assigned role**: developer
- **Dependencies**: Step 1
- **Parallelizable**: No

## 3. Refactor Other Audio Triggers
- **Description**: Update Sprint, Jump, and Lightning Ball audio triggers to use the managed `AudioSource` for consistency and control.
- **Files**: `Assets/MRTabletopAssets/Scripts/Character/TableCharacter.cs`
- **Assigned role**: developer
- **Dependencies**: Step 1
- **Parallelizable**: No

# Verification & Testing
1. **Manual Test**: Move the character slightly and stop. Verify the sound stops (or doesn't overlap excessively).
2. **Jitter Test**: Move the joystick very slightly near the threshold. Verify the sound doesn't trigger multiple times in a second.
3. **Sprint Test**: Start sprinting and stop. Verify the sprint voice behaves correctly.
4. **Network Test**: Ensure sounds still play correctly for the local owner.

# Project Overview
- Game Title: Tabletop Fighting Game (MR)
- High-Level Concept: A fighting game played on a tabletop using MR/XR.
- Players: Multiplayer (Netcode for GameObjects)
- Target Platform: Android (Meta Quest / Mobile)
- Input System: New Input System + XR Interaction Toolkit (XRI)

# Game Mechanics
## Core Gameplay Loop
Players control fighters on a tabletop to defeat their opponent.
## Controls and Input Methods
XR Controllers or Gamepads.

# UI
Haptic feedback is added to enhance the "hit" sensation.

# Key Asset & Context
- `TableCharacterInput.cs`: Singleton input manager.
- `FighterHealth.cs`: Character health and hit reaction logic.
- `HapticImpulsePlayer`: XRI component for triggering haptics on controllers.

# Implementation Steps
## 1. Add Haptic Support to TableCharacterInput
- **Description**: Add a helper method to `TableCharacterInput.cs` that triggers haptics on all available controllers (XR and Gamepad).
- **Files**: `Assets/MRTabletopAssets/Scripts/Character/TableCharacterInput.cs`
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes

## 2. Trigger Haptics on Damage
- **Description**: Modify `FighterHealth.cs` to call the haptic trigger when the local player's character is hit.
- **Files**: `Assets/MRTabletopAssets/Games/FightingGame/Scripts/FighterHealth.cs`
- **Assigned role**: developer
- **Dependencies**: Step 1
- **Parallelizable**: No

# Verification & Testing
1. **Manual Test**: Play the game in XR (or with a gamepad).
2. **Verify Hit**: Ensure that when the character takes damage, the controller vibrates.
3. **Owner Check**: Verify that haptics only trigger for the player who was hit (local owner), not for the other player.
4. **Edge Case**: Ensure no errors occur if no haptic devices are connected.

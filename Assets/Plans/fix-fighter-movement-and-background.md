# Project Overview
- Game Title: Tabletop MR Games
- High-Level Concept: Fixing fighting game character movement and the "green screen" background issue.
- Players: Multiplayer (Netcode)
- Target Platform: Android (Quest/MR)

# Analysis of Issues
1. **Movement**: `FighterController` has `m_MoveAction` and `m_AttackAction` fields, but they are `null` in the prefabs. They need to be linked to XR Input Actions.
2. **Green Background (Prefab Mode)**: When opening a prefab, Unity uses an "Environment" scene. If this scene is green, it's likely a project setting for MR testing.
3. **Green Background (In-Game)**: The `BackgroundLobby` object is active by default. Unlike other game modes, `FightingGameManager` doesn't toggle it off, so it remains visible "beyond the character".

# Implementation Steps

## 1. Fix Character Movement (Input Mapping)
- Open `KnightFighter` and `RobotFighter` prefabs.
- In the `FighterController` component:
  - **Move Action**: Link to an `InputActionReference`. Suggested: `XRI LeftHand Locomotion/Move`.
  - **Attack Action**: Link to an `InputActionReference`. Suggested: `XRI RightHand Interaction/Select`.
- **Note**: Ensure the `FighterLayer` in the controller is set to the layer assigned to the characters (e.g., "Default" or a custom "Fighter" layer if created) so they can hit each other.

## 2. Fix Green Background (In-Game)
- Select the `Virtual Table/Tabletop Games/Game Mode Fighting` object.
- In the `FightingGameManager` component:
  - Add `MRInteractionSetup/XR Origin (XR Rig)/BackgroundLobby` to the **Objects To Toggle** list.
  - This ensures that when the fighting game is hidden/shown, the lobby background is also toggled. In this project's logic, `ObjectsToToggle` are typically *enabled* when the mode is active, so we might need to verify if we want the lobby *off* during the game. 
  - **Better approach**: The `BackgroundLobby` should likely be disabled when any tabletop game starts if Passthrough (MR) is intended.

## 3. Fix Green Background (Prefab Editor)
- Go to `Edit > Project Settings > Editor`.
- Under the **Prefab Mode** section, check the **Environment** field.
- If it points to a scene with a green background, you can:
  - Clear the field to use the default empty background.
  - Or, create a simple scene with just a light and no background and assign it there.
- Alternatively, while in Prefab Mode, click the **Skybox** icon in the Scene View toolbar and toggle it or check the "Scene Settings" override.

# Verification & Testing
1. **Prefab Mode**: Open `KnightFighter`. Verify the background is no longer green.
2. **In-Game Movement**: Start the game as Host. Spawn characters. Use the Left Stick to move and Right Trigger to attack.
3. **In-Game Visuals**: Ensure the green `BackgroundLobby` disappears when the fighting game is active, showing the table clearly (and Passthrough if on device).

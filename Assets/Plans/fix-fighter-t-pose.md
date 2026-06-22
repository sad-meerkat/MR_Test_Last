# Project Overview
- Game Title: MR Test (XR Multiplayer)
- Issue: Characters move correctly but remain in a T-pose because the `Animator Controller` is missing from the character prefabs.

# Game Mechanics
- Characters use an `Animator` component on the child `Model` object.
- The `FighterController` script updates the "Speed" float parameter to trigger walk/idle transitions.

# Key Asset & Context
- `ByakuyaFighter.prefab`: `Animator` component found on child `Model`, but `runtimeAnimatorController` is null.
- `SasukeFighter.prefab`: `Animator` component found on child `Model`, but `runtimeAnimatorController` is null.
- `Assets/Anim.controller`: The character animator controller containing "Standing Idle", "Walking (1)", and the "Speed" parameter.

# Implementation Steps

## Step 1: Assign Animator Controller to ByakuyaFighter
- **Description**: Assign `Assets/Anim.controller` to the `Animator` component on the child `Model` of `ByakuyaFighter.prefab`.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes

## Step 2: Assign Animator Controller to SasukeFighter
- **Description**: Assign `Assets/Anim.controller` to the `Animator` component on the child `Model` of `SasukeFighter.prefab`.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes

# Verification & Testing
- **Visual Check**: Open the prefabs in the inspector and verify the `Animator` component has `Anim` assigned as the controller.
- **Play Mode Test**: Move the characters using the joystick and verify they transition from "Idle" to "Walking" animations.
- **Animator Window**: (Manual) Select a character in the hierarchy during Play Mode and observe the "Speed" parameter and state transitions in the Animator window.

# Project Overview
- Game Title: MR Test (XR Multiplayer)
- Issue: The 'X' button on the left controller does not trigger the character jump, even though the logic was implemented.
- Root Cause:
    1. The `XRI Default Input Actions` asset only has a `Jump` binding for the Right Hand's `PrimaryButton`.
    2. The `m_JumpAction` field in `TableCharacterInput` and `FighterController` is not assigned in the Inspector.

# Game Mechanics
- Jump: Pressing the 'X' button (PrimaryButton) on the Left Controller should trigger a jump.

# Key Asset & Context
- `Assets/Samples/XR Interaction Toolkit/3.3.0/Starter Assets/XRI Default Input Actions.inputactions`: Needs a Left Hand binding for Jump.
- `FighterController.cs`: Uses `m_JumpAction`.
- `TableCharacterInput.cs`: Uses `m_JumpAction`.

# Implementation Steps

## Step 1: Update Input Action Asset
- **Description**: Add a new binding to the `Jump` action in the `XRI Right Locomotion` map of `XRI Default Input Actions.inputactions`.
- **Binding Path**: `<XRController>{LeftHand}/{PrimaryButton}`.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes

## Step 2: Assign Jump Action Reference to Prefabs
- **Description**: Assign the `XRI Right Locomotion/Jump` action reference to the `m_JumpAction` field in the following prefabs:
    - `Assets/MRTabletopAssets/Games/FightingGame/Prefabs/ByakuyaFighter.prefab`
    - `Assets/MRTabletopAssets/Games/FightingGame/Prefabs/SasukeFighter.prefab`
- **Assigned role**: developer
- **Dependencies**: Step 1
- **Parallelizable**: Yes

## Step 3: Assign Jump Action Reference to Scene Object
- **Description**: Assign the `XRI Right Locomotion/Jump` action reference to the `m_JumpAction` field of the `TableCharacterInput` component on the `Virtual Table` object in the scene.
- **Assigned role**: developer
- **Dependencies**: Step 1
- **Parallelizable**: Yes

# Verification & Testing
- **Input Verification**: In Play Mode, press the 'X' button on the left controller. The character should perform a jump.
- **Animation Verification**: Ensure the "jumping" animation trigger is fired.
- **Network Verification**: (If applicable) Ensure the jump is synchronized across the network for fighters.

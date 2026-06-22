# Lightning Ball Mechanic Implementation

This plan outlines the implementation of a Lightning Ball mechanic for the tabletop characters, similar to the existing Sword mechanic.

## Project Overview
- **Goal**: Summon a Lightning Ball in the character's hand when the player performs a Grab gesture, and allow it to deal damage when swung.
- **Key Scripts**: `TableCharacter.cs`, `TableCharacterInput.cs`, `LightningBallOnGrab.cs`, `ProjectileDamage.cs`.

## Implementation Steps

### 1. Update `TableCharacter.cs`
- Add a reference for the Lightning Ball visual: `[SerializeField] GameObject m_LightningBallVisual;`.
- Add a `NetworkVariable<bool> m_LightningBallActiveNet` to synchronize its state.
- Implement `SetLightningBallActive(bool active)` which updates the local visual and synchronizes via `ServerRpc`.
- In `OnNetworkSpawn`, subscribe to `m_LightningBallActiveNet.OnValueChanged` to update the visual.

### 2. Update `TableCharacterInput.cs`
- Add `SetCharacterLightningBallActive(bool active)` which calls the corresponding method on the controlled character.

### 3. Update `LightningBallOnGrab.cs`
- Refactor the script to stop local instantiation at its own `m_Spawner`.
- Instead, call `TableCharacterInput.Instance.SetCharacterLightningBallActive(active)`.
- Use the gesture's performed/ended events to toggle the ball active/inactive.

### 4. Damage Logic
- Create `MeleeWeaponDamage.cs`:
    - Deals damage on `OnTriggerEnter`.
    - Handles `owner` exclusion like `ProjectileDamage`.
    - Does **not** destroy itself on hit.
- Attach `MeleeWeaponDamage.cs` to the Lightning Ball visual on the character prefab.

## Verification & Testing
- **Visual Sync**: Verify that when one player performs the Grab gesture, all players see the Lightning Ball in that character's hand.
- **Damage**: Verify that swinging the hand (triggering `OnHandSwing`) deals damage to the opponent if they are in range.
- **Cleanup**: Verify the Lightning Ball disappears when the gesture is released.

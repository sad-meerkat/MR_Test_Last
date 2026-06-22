# Project Overview
- Game Title: Tabletop MR Games
- High-Level Concept: Adding procedural motions to the fighting game characters to provide visual feedback for attacks and taking damage.
- Players: Multiplayer (Netcode)
- Target Platform: Android (Quest/MR)

# Game Mechanics
## Core Gameplay Loop
When a player presses the attack button, the character performs a "nudge" motion forward. If it hits the opponent, the opponent flashes or shakes.

# Key Asset & Context
- **FighterController.cs**: Needs a visual "Attack" motion.
- **FighterHealth.cs**: Needs a visual "Take Damage" feedback.

# Implementation Steps

## 1. Add Attack Motion to FighterController.cs
- Add an `AttackClientRpc` called from the `AttackServerRpc`.
- Inside `AttackClientRpc`, start a Coroutine that quickly moves the character's visual child forward and back.
- This provides immediate feedback even without an Animator.

## 2. Add Damage Feedback to FighterHealth.cs
- Modify `TakeDamage` to call a `DamageClientRpc`.
- Inside `DamageClientRpc`, trigger a visual effect like:
  - Shaking the character slightly.
  - (Optional) Flashing the character red (if materials allow).

## 3. Update Character Prefabs
- Ensure the `FighterLayer` is correctly set to `Default` (or the layer characters are on) as previously discussed.
- Ensure the `m_FighterLayer` in `FighterController` is set to include the characters.

# Verification & Testing
1. **Attack Test**: Press the attack button and verify the character performs a quick forward "jerk" or nudge.
2. **Damage Test**: Successfully hit an opponent and verify they react visually (shake).
3. **Multiplayer Test**: Verify that both players see the nudge and the reaction.

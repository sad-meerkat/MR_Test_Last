# Project Overview
- Game Title: MR Test (XR Multiplayer)
- Issue: Characters (Sasuke, Byakuya) fall through the table or appear at the wrong height because their CapsuleColliders are offset incorrectly.

# Game Mechanics
- Characters use Rigidbody physics and CapsuleColliders to stay on the table.

# Key Asset & Context
- `SasukeFighter.prefab`: CapsuleCollider height is 0.14, but Center Y is 1.0 (should be 0.07).
- `ByakuyaFighter.prefab`: CapsuleCollider height is 0.14, but Center Y is 1.0 (should be 0.07).
- `TableCharacter.prefab`: CapsuleCollider height is 2.0 (scale 0.1), Center Y is 1.0 (correct, base at 0).

# Implementation Steps

## Step 1: Fix Fighter Prefab Colliders
- **Description**: Update `SasukeFighter` and `ByakuyaFighter` prefabs to set their `CapsuleCollider.center.y` to `height / 2`. This ensures the bottom of the collider is at the character's feet (Y=0).
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes

## Step 2: Verification of TableCharacter
- **Description**: Double-check `TableCharacter` prefab. Although its math (Center 1.0, Height 2.0) puts the base at 0, I will explicitly verify it stays on the table during spawn.
- **Assigned role**: explorer
- **Dependencies**: Step 1
- **Parallelizable**: No

# Verification & Testing
- **Spawn Test**: Summon Sasuke or Byakuya and verify they stay on the table surface instead of falling through or "sinking".
- **Collision Test**: Verify characters collide with the table boundary walls.

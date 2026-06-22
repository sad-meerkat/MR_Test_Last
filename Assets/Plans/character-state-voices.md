# Character State-Based Voices Implementation Plan

This plan adds situational voice lines for characters when they get hit, start moving, sprint, or jump.

## Project Overview
- **Goal**: Increase character personality and feedback by playing 3D spatial audio clips during state changes.
- **Affected Files**:
    - `Assets/MRTabletopAssets/Games/FightingGame/Scripts/FighterHealth.cs` (Hit voice)
    - `Assets/MRTabletopAssets/Scripts/Character/TableCharacter.cs` (Move, Sprint, Jump voices)

## Implementation Steps

### 1. Update `FighterHealth.cs`
- Add `m_HitVoiceClip` field.
- Play the clip inside `PlayHitReactionClientRpc` to ensure all players hear the cry of pain when a character is struck.

### 2. Update `TableCharacter.cs`
- Add fields for `m_StartMoveVoice`, `m_SprintVoice`, and `m_JumpVoice`.
- Add state tracking (`m_WasMoving`) to detect the frame movement begins.
- Trigger voices in `Move()`, `SetSprinting()`, and `Jump()`.
- Ensure spatial audio is used (`AudioSource.PlayClipAtPoint`) so voices sound like they come from the characters on the table.

## Detailed Code Adjustments

### FighterHealth.cs
- **New Header**: `Voice Settings`
- **Logic**: Use `AudioSource.PlayClipAtPoint(m_HitVoiceClip, transform.position)` in the hit RPC.

### TableCharacter.cs
- **New Header**: `State Voices`
- **Logic**:
    - In `Move()`: If `!m_WasMoving && isMovingNow`, play `m_StartMoveVoice`.
    - In `SetSprinting()`: If `sprinting == true`, play `m_SprintVoice`.
    - In `Jump()`: Play `m_JumpVoice` on force application.

## Verification & Testing
- **Local Test**: Trigger each state (walk, sprint, jump, get hit) and confirm the correct sound plays.
- **Distance Test**: Move the VR camera away from the table and verify that the voices get quieter (Spatial/3D effect).
- **Multiplayer Test**: Ensure that when one player hits the other, both players hear the hit voice correctly.

# Project Overview
- Goal: Fine-tune character physics for tabletop scale.
- Issue: Standard physics values are too strong for tiny characters (14cm tall), making them move like race cars and jump like superheroes.

# Implementation Steps

## Step 1: Protect Fields and Improve Logic in TableCharacter.cs
- **Description**: Make movement fields accessible to children and add a robust Raycast-based ground check.
- **File**: `Assets/MRTabletopAssets/Scripts/Character/TableCharacter.cs`
- **Changes**:
    - Change `m_MoveSpeed` and `m_JumpForce` to `protected`.
    - Implement `IsGrounded()` using `Physics.Raycast`.
    - Update `Jump()` to use the new ground check.
- **Assigned role**: developer

## Step 2: Refine FighterController.cs
- **Description**: Ensure the subclass uses the base class fields correctly and respects owner-only physics application.
- **File**: `Assets/MRTabletopAssets/Games/FightingGame/Scripts/FighterController.cs`
- **Assigned role**: developer

## Step 3: Update Prefab Defaults
- **Description**: Set realistic values for tabletop scale.
- **ByakuyaFighter / SasukeFighter**:
    - `Move Speed`: 0.4
    - `Jump Force`: 0.15
- **Assigned role**: developer

# Verification & Testing
- **Jump Test**: Verify the character only jumps when on the floor and the height is small (a few centimeters).
- **Speed Test**: Verify the walking speed looks natural for the character's size on the table.

# Multiplayer Seat Positioning Fix

This plan addresses the issue where both players spawn in the exact same position, causing their cameras and hand tracking models to overlap.

## Project Overview
- **Game Title**: Tabletop Fighting Game
- **Issue**: In multiplayer builds, both players are positioned at the same coordinate, causing their hand models to overlap in the same spot.
- **Root Cause**:
    - The player's location is controlled by `TableSeatSystem.cs`.
    - When a player is assigned a seat, `NetworkTableTopManager` calls `m_SeatSystem.TeleportToSeat(seatID)`.
    - However, `TeleportToSeat` only rotates the player's `XROrigin` around the table center but **never actually translates/teleports the player's position** to the seat coordinates.
    - The actual positioning logic is written inside `ResetToSeatDefault()` in `TableSeatSystem.cs`, but this method is **never called anywhere in the project**.
    - As a result, both players remain at the default coordinates (`0,0,0`), overlapping each other.

## Implementation Steps

### 1. Update `TableSeatSystem.cs`
- Modify the `TeleportToSeat(int seatNum)` method to call `ResetToSeatDefault()` at the end of the teleportation sequence.
- This ensures that whenever a player is teleported to a seat (either at game start or when switching seats), their `XROrigin` position and rotation are physically aligned with the actual seat transform.

```csharp
        public void TeleportToSeat(int seatNum)
        {
            // Check for spectator seat or initial seat
            if (TableTop.k_CurrentSeat < 0)
            {
                TableTop.k_CurrentSeat = 0;
            }

            int prevSeat = TableTop.k_CurrentSeat;
            TableTop.k_CurrentSeat = seatNum;

            float currentAngle = GetRotationAngleBasedOnSeatNum(prevSeat);
            float newAngle = GetRotationAngleBasedOnSeatNum(seatNum);
            float rotationAmount = newAngle - currentAngle;
            m_XROrigin.transform.RotateAround(transform.position, transform.up, rotationAmount);
            m_OnSeatChanged.Invoke(seatNum);

            transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

            // [Fix] Call ResetToSeatDefault to actually position the player at the seat!
            ResetToSeatDefault();
        }
```

## Verification & Testing
- **Multiplayer Build Test**: Build and run the project with two clients.
- **Seat Assignment**: Join a game and verify that Player 1 and Player 2 are teleported to opposite sides of the table (Seat 1 and Seat 2).
- **Hand Alignment**: Verify that each player sees the other player's hand models across the table, and they no longer overlap in the center.

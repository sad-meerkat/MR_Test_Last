# Project Overview
- Issue: Health bars are visible but don't clearly show damage.
- Solution: Fix fill directions to match standard fighting games, add HP numerical text (e.g., 100/100), and ensure robust synchronization.

# Game Mechanics
- Players hit each other. Health decreases. 
- Top HUD should show visual bar decrease and numerical change.

# UI
- **FightingHUD**: Top-center HUD.
- **P1 Health Bar**: Should fill from Left (Origin 0).
- **P2 Health Bar**: Should fill from Right (Origin 1).
- **HP Text**: New TMP text objects on top of bars.

# Key Asset & Context
- `FightingHUDManager.cs`: Handles bar and text updates.
- `FighterHealth.cs`: Sends health updates.

# Implementation Steps

## Step 1: Enhance FightingHUDManager.cs
- **Description**: Add `TextMeshProUGUI` fields for P1 and P2 HP text. Update them in `UpdateHealth`.
- **Assigned role**: developer
- **Files**: `Assets/MRTabletopAssets/Games/FightingGame/Scripts/FightingHUDManager.cs`
- **Action**:
    - Add `public TextMeshProUGUI p1HealthText;` and `public TextMeshProUGUI p2HealthText;`.
    - In `UpdateHealth`, set the text to `(healthPercent * 100).ToString("0") + " / 100"`.
- **Dependencies**: None

## Step 2: Fix Fill Origins and Sync logic
- **Description**: Ensure the health bars decrease correctly from the outer edges towards the center.
- **Assigned role**: developer
- **Files**: `Assets/MRTabletopAssets/Games/FightingGame/Scripts/FightingHUDManager.cs`
- **Action**:
    - In `Start()` or `Awake()`, force `p1HealthBar.fillOrigin = 0` (Left) and `p2HealthBar.fillOrigin = 1` (Right).
- **Dependencies**: Step 1

## Step 3: Ensure FighterHealth Retries Sync
- **Description**: If the HUD is activated late, ensure the fighter sends its current health again.
- **Assigned role**: developer
- **Files**: `Assets/MRTabletopAssets/Games/FightingGame/Scripts/FighterHealth.cs`
- **Action**:
    - Add a small delay or a retry in `UpdateUI` if the HUD is not found immediately.
- **Dependencies**: None

# Verification & Testing
1. **Play Mode**: Hit the dummy.
2. **Visual Check**: Confirm the health bar shrinks from the side and the numbers decrease.
3. **Log Check**: Verify `[FightingHUDManager]` logs show the correct `fillAmount` values.

# Character Win/Lose Animations Implementation Plan

This plan outlines how to apply the victory ("win") and defeat ("lose") animations to the tabletop characters when a match ends.

## Project Overview
- **Game Title**: Tabletop Fighting Game
- **Feature**: Play victory/defeat animations for the characters when a game ends.
- **Tone**: Playful and arcade-like.

## Discovery & Assets Check
- **Animator Controller**: The characters (`SasukeFighter`, `ByakuyaFighter`) use the Animator Controller named `Anim` located at `Assets/Anim.controller`.
- **Existing States**: `Anim.controller` **already contains** pre-configured `win` and `lose` states in its Base Layer!
- **Animator Parameters**: The animator already has the `win` (Trigger) and `lose` (Trigger) parameters.
- **Transitions**: The animator has transitions from `Standing Idle` to `win` and `lose` that trigger when these parameters are activated, and then transition back to `Standing Idle`.

Because the animations and animator parameters already exist, we do not need to edit the animator or import new clips. We only need to trigger them in our game management code when a winner is decided.

## Implementation Steps

### 1. Update `FightingGameManager.cs`
- Modify the `OnWinnerChanged(int old, int current)` method, which is automatically called on all clients (and the server) when the game ends and a winner/tie is declared.
- Inside `OnWinnerChanged`, we will find all `FighterHealth` instances currently in the scene.
- For each character, we grab their `Animator` component and trigger `"win"` or `"lose"` based on their player index and the winning index.

```csharp
        private void OnWinnerChanged(int old, int current)
        {
            Debug.Log($"[FightingGameManager] OnWinnerChanged: {old} -> {current}");
            if (current == -1) return;
            
            string message = "";
            if (current == 0) message = "PLAYER 1 WINS!";
            else if (current == 1) message = "PLAYER 2 WINS!";
            else if (current == 2) message = "DRAW!";

            if (FightingHUDManager.Instance != null)
            {
                Debug.Log($"[FightingGameManager] Displaying winner message: {message}");
                FightingHUDManager.Instance.DisplayWinner(message);
            }
            else
            {
                Debug.LogWarning("[FightingGameManager] FightingHUDManager instance not found!");
            }

            // [New] Trigger Win/Lose Animations on the characters on all clients
            TriggerWinLoseAnimations(current);
        }

        private void TriggerWinLoseAnimations(int winnerIndex)
        {
            var fighters = Object.FindObjectsByType<FighterHealth>(FindObjectsSortMode.None);
            foreach (var f in fighters)
            {
                var animator = f.GetComponentInChildren<Animator>();
                if (animator == null) continue;

                int playerIdx = f.playerIndex.Value;

                if (winnerIndex == 0) // Player 1 Wins
                {
                    if (playerIdx == 0) animator.SetTrigger("win");
                    else if (playerIdx == 1) animator.SetTrigger("lose");
                }
                else if (winnerIndex == 1) // Player 2 Wins
                {
                    if (playerIdx == 1) animator.SetTrigger("win");
                    else if (playerIdx == 0) animator.SetTrigger("lose");
                }
                else if (winnerIndex == 2) // Draw
                {
                    // In case of a draw, both play the lose animation
                    animator.SetTrigger("lose");
                }
            }
        }
```

## Verification & Testing
- **Local Play Mode Test**: Play the game in the Unity Editor (forcing a single-player mode, or testing with two fighters).
- **Match End - Victory**: Finish the match by reducing the opponent's health to 0 or letting the timer expire with uneven health. Verify that:
    - The winning character plays their victory pose ("win").
    - The losing character plays their defeat animation ("lose").
- **Match End - Draw**: Let the timer expire with equal health. Verify that both characters play their defeat/draw animation.

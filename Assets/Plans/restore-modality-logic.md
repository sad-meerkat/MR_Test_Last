# Project Overview
- Game Title: MR Tabletop Fighting Game
- Goal: Restore Hand/Controller switching system and optimize player movement logic for table manipulation.

# Game Mechanics
## Modality Switching
- Re-linking hand interactors ensures the system can toggle between hand tracking and controller input seamlessly.
## Table Movement
- Switching the movement target to the common parent (`MRInteractionSetup`) ensures all rigs and input systems stay synchronized.

# Key Asset & Context
- `IndependentHandModalityManager`: Hand-Controller toggle logic.
- `TableManipulator.cs`: Table-to-Player coordinate shifting logic.

# Implementation Steps
1. **Restore Interactor References**:
   - Manually re-assign `Near-Far Interactor` components from the XR Origin hierarchy to the `IndependentHandModalityManager`.
2. **Refactor TableManipulator.cs**:
   - Change `m_XROrigin` reference or logic to target the parent container `MRInteractionSetup`.
   - Simplify `MovePlayer` logic to shift the entire setup as a single unit.
3. **Handle Visibility**:
   - Ensure the table manipulation handle visuals are active for user feedback.

# Verification & Testing
1. **Input Switch Test**: Verify hands disappear when controllers are picked up and vice versa.
2. **Table Move Test**: Verify the entire view and hands shift together when the table is "moved".

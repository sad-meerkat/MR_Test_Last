# Project Overview
- Game Title: Vehicle Inspection MR
- High-Level Concept: An MR application for vehicle inspection using license plate recognition (Tesseract OCR).
- Players: Single player (MR) or Local Multiplayer (Tabletop).
- Screen Orientation / Resolution: Landscape (MR Headset / Mobile).
- Render Pipeline: URP.

# Game Mechanics
## Core Gameplay Loop
1. User points the camera at a license plate.
2. Tesseract OCR recognizes the text.
3. Upon successful recognition, a "Next Step" button appears to proceed with the inspection.

# UI
- **Camera Feed**: Displayed on a RawImage (Texture (3)).
- **OCR Status**: Displayed on a Text element (CameraLogText).
- **Navigation**: "Next Step" button (NextButton_Transition).

# Key Asset & Context
- `TesseractDemoScript.cs`: Handles OCR and UI logic.
- `CameraCaptureManager.cs`: Handles camera feed and capture.
- `InspectionManager.cs`: Manages inspection steps (mentioned by user).
- `NextButton_Transition`: The button used to transition between inspection steps.

# Implementation Steps
## Phase 1: Fix Button Visibility Logic
1. **Modify TesseractDemoScript.cs**:
    - Ensure `nextButton` is hidden at Start.
    - Only activate `nextButton` when `_hasScannedSuccessfully` is true or manual input is provided.
    - (Wait, I already did this, but the user says it appears immediately in 4 directions).

## Phase 2: Fix "4 Directions" Issue (Seat-Specific UI)
The user reports that the button appears in 4 directions simultaneously. This is likely because the button is parented to a global Canvas that replicates its content for 4 players/sides in the MR Tabletop template.
1. **Change Parenting**:
    - Instead of parenting `NextButton_Transition` to the root `Canvas`, it should be childed to the specific UI area associated with the current seat or the "Sign" being inspected.
    - If there are 4 inspection zones (Front, Back, Left, Right), each should have its own local "Next" button.

2. **Update Script References**:
    - Ensure each `TesseractDemoScript` instance (if there are multiple) points to its *local* `nextButton` instead of a shared global one.

# Verification & Testing
1. Run the scene.
2. Verify that `NextButton_Transition` is not visible on any of the 4 sides at startup.
3. Perform a scan or enter text manually.
4. Verify that the "Next Step" button appears *only* on the side/seat where the action was performed.

# Project Overview
    - Game Title: MR_Test2 (Mobile VR/MR)
    - High-Level Concept: Tabletop Mixed Reality interaction.
    - Players: Single player
    - Render Pipeline: URP

# Implementation Steps
1. **Fix Shader Errors**:
    - Modify `Assets/LiteFireEffect/Shader/ParticlePremultiplyBlend.shader`: Replace manual depth texture declaration `sampler2D_float _CameraDepthTexture;` with the standard macro `UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);`.
    - Modify `Assets/LiteFireEffect/Shader/ParticleAddEffect.shader`: Replace manual depth texture declaration `sampler2D_float _CameraDepthTexture;` with the standard macro `UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);`.
    Assigned role: developer
    Dependencies: None
    Parallelizable: Yes

2. **Verify Build**: Re-check shader compilation in the Unity Editor console.
    Assigned role: explorer
    Dependencies: Step 1
    Parallelizable: No

# Verification & Testing
- Manual check: Ensure no "undeclared identifier" errors remain in the console.
- Build retry: The user can now proceed with the Android build.

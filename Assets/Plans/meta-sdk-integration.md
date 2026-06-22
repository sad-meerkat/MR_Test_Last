# Project Overview
- Game Title: MR Tabletop Multiplayer (Meta SDK Integration)
- High-Level Concept: Porting Meta XR SDK (specifically Simultaneous Hands and Controllers) into the official Unity MR Multiplayer Template (XRI + NGO) while maintaining network synchronization.
- Players: Multiplayer (Netcode for GameObjects)
- Target Platform: Meta Quest (Android)
- Render Pipeline: URP-Performant
- Unity Version: 6000.4.4f1

# Game Mechanics
## Core Gameplay Loop
The player interacts with tabletop MR elements using hands or controllers. The template handles the lobby, room joining, and object synchronization across the network.
## Controls and Input Methods
- **Simultaneous Hands and Controllers (SHC)**: Meta-specific feature allowing both hands and controllers to be tracked at the same time.
- **XRI Interaction**: Poke, Grab, and UI interaction using the XR Interaction Toolkit.

# Key Asset & Context
- **OVRCameraRig**: The core Meta tracking component.
- **XR Origin (XR Rig)**: The existing template's local player root.
- **XRINetworkPlayer**: Script responsible for syncing local head/hand positions to the network.
- **XRHandPoseReplicator**: Script responsible for syncing hand finger animations.
- **IndependentHandModalityManager**: Custom template script that handles switching between hands and controllers.

# Implementation Steps

## 1. Project & Package Configuration
1.  **Package Verification**: Ensure `com.meta.xr.sdk.core` (Meta XR Core SDK) is installed via Package Manager.
2.  **XR Plug-in Management**:
    - Go to `Edit > Project Settings > XR Plug-in Management`.
    - In the **Android** tab, ensure **OpenXR** is selected.
    - Under **OpenXR > Interaction Profiles**, add `Meta Quest Touch Plus Controller Profile`.
    - Under **OpenXR > Feature Groups**, enable **Meta Quest Support (Meta)**.
    - Enable **Simultaneous Hands and Controllers** feature in the OpenXR feature list.

## 2. Scene Hardware Integration (OVRCameraRig)
1.  **Instantiate OVRCameraRig**: Place the `OVRCameraRig.prefab` (from `Packages/com.meta.xr.sdk.core/Prefabs`) into the main scene as a sibling of `XR Origin (XR Rig)`.
2.  **Configure OVRManager**:
    - Select the `OVRCameraRig` object.
    - In `OVRManager` component:
        - Set `Hand Tracking Support` to **Controllers And Hands**.
        - Enable **Simultaneous Hands and Controllers**.
        - Set `Tracking Origin Type` to **Floor Level**.
3.  **Adjust XRI Tracking**:
    - On the `Main Camera` child of `XR Origin`, disable the `TrackedPoseDriver`. The camera will now be driven by `OVRCameraRig`'s `CenterEyeAnchor`.
    - On the `Left Controller` and `Right Controller` objects of `XR Origin`, disable the `TrackedPoseDriver`.

## 3. Implement Meta-to-XRI Bridge
1.  **Create Bridge Script**: Implement `MetaXRIBridge.cs` to link Meta's high-fidelity tracking to the template's XRI objects.
    - **Logic**: In `LateUpdate`, copy the World Position and Rotation from `OVRCameraRig` anchors to the corresponding `XR Origin` controller/hand objects.
    - **Visibility**: Update the active state of XRI objects based on `OVRInput.IsControllerConnected` and hand tracking availability.
2.  **Attach Bridge**: Attach this script to the `XR Origin (XR Rig)` and assign the references (OVR Anchors <-> XRI Objects).

## 4. Adapt Template Modality (IndependentHandModalityManager)
1.  **Modify Modality Logic**: The `IndependentHandModalityManager` currently enforces a strict "Hand OR Controller" switch.
2.  **Enable Simultaneous Mode**: Update the script to allow both `handObject` and `controllerObject` to be active if SHC is active and both are tracked.
3.  **Update References**: Ensure the script is aware that `TrackedPoseDriver` is disabled and relies on the Bridge for movement.

## 5. Verify Network Synchronization
1.  **XRINetworkPlayer Binding**: The `XRINetworkPlayer` script (IsOwner) copies positions from `m_HeadOrigin`, `m_LeftHandOrigin`, and `m_RightHandOrigin`.
2.  **Redirect Origins**: Update the references in `XRINetworkPlayer` (for the local owner) to point to the `OVRCameraRig` anchors (Head, Left Hand, Right Hand).
3.  **Hand Pose Replication**: Ensure `XRHandPoseReplicator` continues to receive finger data. Since the template uses `XRHandSkeletonDriver`, it should still work via OpenXR Hands, but we may need to bridge Meta's `OVRHand` data to `XRHand` if standard Unity Hands are unresponsive during SHC.

# Verification & Testing
1.  **Local Tracking Test**: Run on Quest. Verify that the camera and hands move correctly. Verify that you can see BOTH a controller and a hand tracking visualization if both are active (SHC).
2.  **Interaction Test**: Verify that Poke and Grab still work on tabletop objects using the Meta-tracked hands/controllers.
3.  **Network Test**: Open a build and an editor session (using XR Simulator or another headset). Verify that the local player's movements (driven by Meta SDK) are correctly replicated to the remote player's avatar via NGO.
4.  **SHC Transition Test**: Drop a controller and start using hand tracking. Pick up the controller. Verify the switch is smooth and replicated over the network.

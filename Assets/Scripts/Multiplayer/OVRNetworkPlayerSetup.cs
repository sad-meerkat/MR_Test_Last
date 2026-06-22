using UnityEngine;
using XRMultiplayer;

namespace OVRMultiplayer
{
    /// <summary>
    /// Bridge that connects OVRCameraRig head/hand transforms to XRINetworkPlayer.
    /// Place one in the scene; it auto-binds OVR anchors when the local player spawns.
    /// </summary>
    public class OVRNetworkPlayerSetup : MonoBehaviour
    {
        OVRCameraRig m_CameraRig;

        void Awake()
        {
            m_CameraRig = FindFirstObjectByType<OVRCameraRig>();
        }

        void OnEnable()
        {
            if (XRINetworkPlayer.LocalPlayer != null)
            {
                BindOVRTransforms(XRINetworkPlayer.LocalPlayer);
            }

            XRINetworkGameManager.Connected.Subscribe(OnConnectedChanged);
        }

        void OnDisable()
        {
            XRINetworkGameManager.Connected.Unsubscribe(OnConnectedChanged);
        }

        void OnConnectedChanged(bool connected)
        {
            if (connected && XRINetworkPlayer.LocalPlayer != null)
            {
                BindOVRTransforms(XRINetworkPlayer.LocalPlayer);
            }
        }

        void BindOVRTransforms(XRINetworkPlayer player)
        {
            if (m_CameraRig == null)
            {
                m_CameraRig = FindFirstObjectByType<OVRCameraRig>();
                if (m_CameraRig == null)
                {
                    Debug.LogError("[OVRNetworkPlayerSetup] OVRCameraRig not found.");
                    return;
                }
            }

            player.SetHandOrigins(m_CameraRig.leftHandAnchor, m_CameraRig.rightHandAnchor);

            Debug.Log("[OVRNetworkPlayerSetup] OVR hand anchors bound to network player.");
        }

        /// <summary>
        /// When XRINetworkPlayer's head origin is null (no XROrigin in OVR scenes),
        /// sync head transform from OVRCameraRig.centerEyeAnchor each frame.
        /// </summary>
        void LateUpdate()
        {
            if (XRINetworkPlayer.LocalPlayer == null) return;
            if (m_CameraRig == null) return;

            var player = XRINetworkPlayer.LocalPlayer;

            if (player.head != null && m_CameraRig.centerEyeAnchor != null)
            {
                player.head.SetPositionAndRotation(
                    m_CameraRig.centerEyeAnchor.position,
                    m_CameraRig.centerEyeAnchor.rotation);
            }
        }
    }
}

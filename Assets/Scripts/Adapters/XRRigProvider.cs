using Unity.XR.CoreUtils;
using UnityEngine;

/// <summary>
/// OVRCameraRig 또는 XROrigin을 자동 감지하여 공통 인터페이스를 제공하는 유틸리티 클래스.
/// OVRCameraRig 우선 탐색 → XROrigin 폴백 → Camera.main 폴백.
/// </summary>
public static class XRRigProvider
{
    static Transform s_RigTransform;
    static Transform s_CameraTransform;
    static Transform s_LeftHandAnchor;
    static Transform s_RightHandAnchor;
    static bool s_IsOVRRig;
    static bool s_Resolved;

    /// <summary>Rig 루트 Transform (OVRCameraRig 또는 XROrigin의 transform)</summary>
    public static Transform RigTransform
    {
        get
        {
            if (!s_Resolved) Resolve();
            return s_RigTransform;
        }
    }

    /// <summary>카메라(HMD) Transform</summary>
    public static Transform CameraTransform
    {
        get
        {
            if (!s_Resolved) Resolve();
            return s_CameraTransform;
        }
    }

    /// <summary>왼손 앵커 Transform</summary>
    public static Transform LeftHandAnchor
    {
        get
        {
            if (!s_Resolved) Resolve();
            return s_LeftHandAnchor;
        }
    }

    /// <summary>오른손 앵커 Transform</summary>
    public static Transform RightHandAnchor
    {
        get
        {
            if (!s_Resolved) Resolve();
            return s_RightHandAnchor;
        }
    }

    /// <summary>현재 OVRCameraRig 기반인지 여부</summary>
    public static bool IsOVRRig
    {
        get
        {
            if (!s_Resolved) Resolve();
            return s_IsOVRRig;
        }
    }

    /// <summary>캐시를 무효화하여 다음 접근 시 다시 탐색하도록 합니다.</summary>
    public static void Invalidate()
    {
        s_Resolved = false;
        s_RigTransform = null;
        s_CameraTransform = null;
        s_LeftHandAnchor = null;
        s_RightHandAnchor = null;
        s_IsOVRRig = false;
    }

    static void Resolve()
    {
        s_Resolved = true;

        // 1) OVRCameraRig 탐색
        var ovrRig = Object.FindFirstObjectByType<OVRCameraRig>();
        if (ovrRig != null)
        {
            s_IsOVRRig = true;
            s_RigTransform = ovrRig.transform;
            s_CameraTransform = ovrRig.centerEyeAnchor;
            s_LeftHandAnchor = ovrRig.leftHandAnchor;
            s_RightHandAnchor = ovrRig.rightHandAnchor;
            return;
        }

        // 2) XROrigin 폴백
        var xrOrigin = Object.FindFirstObjectByType<XROrigin>();
        if (xrOrigin != null)
        {
            s_IsOVRRig = false;
            s_RigTransform = xrOrigin.transform;
            s_CameraTransform = xrOrigin.Camera != null ? xrOrigin.Camera.transform : null;

            // XROrigin에서 핸드 앵커는 직접 제공되지 않으므로 null
            s_LeftHandAnchor = null;
            s_RightHandAnchor = null;
            return;
        }

        // 3) Camera.main 폴백
        s_IsOVRRig = false;
        var mainCam = Camera.main;
        if (mainCam != null)
        {
            s_RigTransform = mainCam.transform.parent != null ? mainCam.transform.parent : mainCam.transform;
            s_CameraTransform = mainCam.transform;
        }

        Debug.LogWarning("[XRRigProvider] No OVRCameraRig or XROrigin found. Using Camera.main fallback.");
    }
}

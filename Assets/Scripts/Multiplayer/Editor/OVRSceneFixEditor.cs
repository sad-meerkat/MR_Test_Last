using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Editor tool to fix duplicate camera issues in OVRMultiTest scene.
/// Removes manually created LeftEyeAnchor/RightEyeAnchor/CenterEyeAnchor outside the OVRCameraRig prefab.
/// Menu: Tools -> OVR Multiplayer -> Fix Duplicate Cameras
/// </summary>
public static class OVRSceneFixEditor
{
    [MenuItem("Tools/OVR Multiplayer/Fix Duplicate Cameras", false, 200)]
    public static void FixDuplicateCameras()
    {
        var ovrCameraRig = Object.FindFirstObjectByType<OVRCameraRig>();
        if (ovrCameraRig == null)
        {
            EditorUtility.DisplayDialog("Error", "OVRCameraRig not found in scene.", "OK");
            return;
        }

        Transform legitimateCenterEye = ovrCameraRig.centerEyeAnchor;
        Transform legitimateLeftEye = ovrCameraRig.leftEyeAnchor;
        Transform legitimateRightEye = ovrCameraRig.rightEyeAnchor;

        int removedCount = 0;

        var allCameras = Object.FindObjectsByType<Camera>(FindObjectsSortMode.None);
        foreach (var cam in allCameras)
        {
            if (cam.transform == legitimateCenterEye ||
                cam.transform == legitimateLeftEye ||
                cam.transform == legitimateRightEye)
            {
                continue;
            }

            string name = cam.gameObject.name;
            if (name == "LeftEyeAnchor" || name == "RightEyeAnchor" || name == "CenterEyeAnchor")
            {
                Debug.Log($"[Fix] Removing duplicate camera: {name} (InstanceID: {cam.gameObject.GetInstanceID()})");
                Undo.DestroyObjectImmediate(cam.gameObject);
                removedCount++;
            }
        }

        var allTransforms = Object.FindObjectsByType<Transform>(FindObjectsSortMode.None);
        foreach (var t in allTransforms)
        {
            if (t.gameObject.name == "TrackingSpace" && t.parent != ovrCameraRig.transform)
            {
                if (t.childCount == 0)
                {
                    Debug.Log($"[Fix] Removing empty TrackingSpace: {t.GetInstanceID()}");
                    Undo.DestroyObjectImmediate(t.gameObject);
                    removedCount++;
                }
            }
        }

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

        if (removedCount > 0)
        {
            EditorUtility.DisplayDialog("Fix Complete",
                $"Removed {removedCount} duplicate camera/object(s).\nSave the scene (Ctrl+S).",
                "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("OK",
                "No duplicate cameras found.\nScene is clean.",
                "OK");
        }
    }

    [MenuItem("Tools/OVR Multiplayer/Fix Duplicate Cameras", true)]
    public static bool FixDuplicateCamerasValidate()
    {
        return !Application.isPlaying;
    }
}

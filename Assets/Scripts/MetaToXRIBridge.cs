/*
2026-05-25 AI-Tag
This was created with the help of Assistant, a Unity Artificial Intelligence product.
*/
using System;
using UnityEditor;
using UnityEngine;

public class MetaToXRIBridge : MonoBehaviour
{
    [Header("Meta Anchors")]
    public Transform metaLeftAnchor;
    public Transform metaRightAnchor;
    public Transform metaHeadAnchor;

    [Header("XRI Targets (Template)")]
    public Transform xriLeftController;
    public Transform xriRightController;
    public Transform xriMainCamera;

    void LateUpdate()
    {
        // Meta의 트래킹 데이터를 XRI 오브젝트로 복사
        if(metaLeftAnchor) xriLeftController.SetPositionAndRotation(metaLeftAnchor.position, metaLeftAnchor.rotation);
        if(metaRightAnchor) xriRightController.SetPositionAndRotation(metaRightAnchor.position, metaRightAnchor.rotation);
        if(metaHeadAnchor) xriMainCamera.SetPositionAndRotation(metaHeadAnchor.position, metaHeadAnchor.rotation);
    }
}

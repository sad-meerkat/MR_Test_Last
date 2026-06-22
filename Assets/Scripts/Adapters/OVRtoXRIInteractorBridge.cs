using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// OVRCameraRig 환경에서 XRI DirectInteractor를 런타임 생성하여
/// XRGrabInteractable 기반 오브젝트(테이블 핸들 등)와 상호작용 가능하게 합니다.
/// OVRCameraRig에 이 컴포넌트를 추가하면 양손 앵커 하위에 DirectInteractor가 자동 생성됩니다.
/// </summary>
[RequireComponent(typeof(OVRCameraRig))]
public class OVRtoXRIInteractorBridge : MonoBehaviour
{
    [SerializeField, Tooltip("인터랙터에 사용할 SphereCollider 반지름")]
    float m_InteractionRadius = 0.05f;

    OVRCameraRig m_CameraRig;
    XRInteractionManager m_InteractionManager;

    void Start()
    {
        m_CameraRig = GetComponent<OVRCameraRig>();

        // XRInteractionManager가 없으면 자동 생성
        m_InteractionManager = FindFirstObjectByType<XRInteractionManager>();
        if (m_InteractionManager == null)
        {
            var managerGO = new GameObject("XRInteractionManager");
            m_InteractionManager = managerGO.AddComponent<XRInteractionManager>();
        }

        CreateInteractor(m_CameraRig.leftHandAnchor, "LeftHandInteractor");
        CreateInteractor(m_CameraRig.rightHandAnchor, "RightHandInteractor");
    }

    void CreateInteractor(Transform parent, string name)
    {
        if (parent == null)
        {
            Debug.LogWarning($"[OVRtoXRIInteractorBridge] {name}: parent anchor is null");
            return;
        }

        var interactorGO = new GameObject(name);
        interactorGO.transform.SetParent(parent, false);

        var collider = interactorGO.AddComponent<SphereCollider>();
        collider.isTrigger = true;
        collider.radius = m_InteractionRadius;

        var rb = interactorGO.AddComponent<Rigidbody>();
        rb.isKinematic = true;

        var interactor = interactorGO.AddComponent<NearFarInteractor>();
        interactor.interactionManager = m_InteractionManager;
    }
}

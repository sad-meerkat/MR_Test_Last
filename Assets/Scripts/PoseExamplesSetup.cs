using UnityEngine;

/// <summary>
/// PoseExamples 씬 전용 설정 스크립트.
/// 왼손은 컨트롤러(썸스틱)로 앞뒤좌우 이동,
/// 오른손은 핸드 트래킹으로 포즈 인식을 동시에 사용할 수 있도록 합니다.
/// OVRCameraRig 오브젝트에 붙여서 사용합니다.
/// </summary>
[RequireComponent(typeof(OVRCameraRig))]
public class PoseExamplesSetup : MonoBehaviour
{
    [SerializeField]
    [Tooltip("이동 속도 (m/s)")]
    float m_MoveSpeed = 2f;

    OVRCameraRig m_CameraRig;

    void Awake()
    {
        m_CameraRig = GetComponent<OVRCameraRig>();
    }

    void Start()
    {
        // 한 손은 컨트롤러, 다른 손은 핸드 트래킹을 동시에 사용 가능하게 설정
        OVRInput.EnableSimultaneousHandsAndControllers();
    }

    void Update()
    {
        // 왼쪽 컨트롤러 썸스틱 입력 읽기
        Vector2 input = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);

        if (input.sqrMagnitude < 0.01f)
            return;

        // HMD 기준 수평 forward / right 계산
        Transform eye = m_CameraRig.centerEyeAnchor;
        Vector3 forward = eye.forward;
        Vector3 right = eye.right;

        // 수평면으로 투영 (y 성분 제거)
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 move = (forward * input.y + right * input.x) * (m_MoveSpeed * Time.deltaTime);
        transform.position += move;
    }

    void OnDestroy()
    {
        OVRInput.DisableSimultaneousHandsAndControllers();
    }
}

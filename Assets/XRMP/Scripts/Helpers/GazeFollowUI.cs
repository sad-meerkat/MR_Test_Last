using UnityEngine;

namespace XRMP.Helpers
{
    /// <summary>
    /// World Space Canvas(또는 오브젝트)를 카메라 시선 앞에 부드럽게 따라오게 합니다.
    ///
    /// 사용법:
    ///   1. Gesture Detection 프리팹(또는 UI Canvas)에 이 컴포넌트를 추가합니다.
    ///   2. Eye Anchor 에 OVRCameraRig > TrackingSpace > CenterEyeAnchor 를 연결합니다.
    ///   3. Distance, Follow Speed, Vertical Offset 을 조정합니다.
    /// </summary>
    public class GazeFollowUI : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("OVRCameraRig의 CenterEyeAnchor Transform")]
        Transform m_EyeAnchor;

        [SerializeField]
        [Tooltip("눈으로부터 UI까지의 거리 (미터)")]
        float m_Distance = 1.5f;

        [SerializeField]
        [Tooltip("위치 추적 속도 (클수록 빠르게 따라옴)")]
        float m_FollowSpeed = 3f;

        [SerializeField]
        [Tooltip("시선 기준 높이 오프셋 (미터). 음수 = 아래, 양수 = 위")]
        float m_VerticalOffset = -0.2f;

        [SerializeField]
        [Tooltip("true = 시선의 수평 방향만 사용 (위아래를 봐도 UI 높이 고정)")]
        bool m_LockVertical = true;

        void LateUpdate()
        {
            if (m_EyeAnchor == null) return;

            // 시선 방향 계산
            Vector3 forward = m_EyeAnchor.forward;

            if (m_LockVertical)
            {
                // 수평 방향만 사용 (y 제거 후 정규화)
                forward.y = 0f;
                if (forward.sqrMagnitude < 0.001f)
                    forward = m_EyeAnchor.transform.parent != null
                        ? m_EyeAnchor.transform.parent.forward
                        : Vector3.forward;
                forward.Normalize();
            }

            // 목표 위치: 눈 앞 m_Distance 거리 + 수직 오프셋
            Vector3 targetPos = m_EyeAnchor.position
                + forward * m_Distance
                + Vector3.up * m_VerticalOffset;

            // 부드럽게 이동
            transform.position = Vector3.Lerp(
                transform.position,
                targetPos,
                Time.deltaTime * m_FollowSpeed);

            // 항상 카메라를 향하도록 회전
            Vector3 lookDir = transform.position - m_EyeAnchor.position;
            if (lookDir.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(lookDir);
        }
    }
}

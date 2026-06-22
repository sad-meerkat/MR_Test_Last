using UnityEngine;

namespace XRMP.LocalPlayer
{
    /// <summary>
    /// 왼쪽 컨트롤러 썸스틱으로 OVRCameraRig를 앞뒤 양옆으로 이동시킵니다.
    /// 이동 방향은 HMD(centerEyeAnchor)의 수평 방향 기준입니다.
    /// </summary>
    [RequireComponent(typeof(OVRCameraRig))]
    public class OVRLocomotion : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("이동 속도 (m/s)")]
        float m_MoveSpeed = 2f;

        OVRCameraRig m_CameraRig;

        void Awake()
        {
            m_CameraRig = GetComponent<OVRCameraRig>();
        }

        void Update()
        {
            Vector2 input = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);

            if (input.sqrMagnitude < 0.01f)
                return;

            // HMD 기준 수평 forward / right 계산
            Transform eye = m_CameraRig.centerEyeAnchor;
            Vector3 forward = eye.forward;
            Vector3 right   = eye.right;

            // 수평면으로 투영 (y 성분 제거)
            forward.y = 0f;
            right.y   = 0f;
            forward.Normalize();
            right.Normalize();

            Vector3 move = (forward * input.y + right * input.x) * (m_MoveSpeed * Time.deltaTime);
            transform.position += move;
        }

        /// <summary>이동 속도</summary>
        public float moveSpeed
        {
            get => m_MoveSpeed;
            set => m_MoveSpeed = value;
        }
    }
}

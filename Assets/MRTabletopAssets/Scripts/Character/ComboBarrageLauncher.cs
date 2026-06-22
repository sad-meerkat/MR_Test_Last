using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Hands.Samples.GestureSample;

namespace UnityEngine.XR.Templates.MRTTabletopAssets
{
    /// <summary>
    /// 3가지 제스처를 순서대로 수행하면 프리팹 5개가 플레이어의 손에 생성된 뒤,
    /// 1초 간격으로 시선 방향으로 날아갑니다.
    /// 모든 프리팹이 발사된 시점부터 쿨타임이 시작됩니다.
    /// </summary>
    public class ComboBarrageLauncher : MonoBehaviour
    {
        [Header("Gesture Sequence (순서대로 연결)")]
        [SerializeField] StaticHandGesture m_Gesture1;
        [SerializeField] StaticHandGesture m_Gesture2;
        [SerializeField] StaticHandGesture m_Gesture3;

        [Header("Barrage Settings")]
        [SerializeField]
        [Tooltip("발사할 프리팹들 (최대 5개)")]
        GameObject[] m_Prefabs = new GameObject[5];

        [SerializeField]
        [Tooltip("각 프리팹의 생성 위치 Transform (프리팹과 같은 순서)")]
        Transform[] m_SpawnPoints = new Transform[5];

        // --- 크기 조절을 위한 배율 변수 추가 ---
        [SerializeField]
        [Tooltip("플레이어 손에 소환되는 프리팹의 크기 배율")]
        float m_HandScaleMultiplier = 1.0f;
        // ------------------------------------

        [SerializeField]
        [Tooltip("발사 속도")]
        float m_Speed = 15f;

        [SerializeField]
        [Tooltip("프리팹 자동 소멸 시간 (초)")]
        float m_Lifetime = 5f;

        [SerializeField]
        [Tooltip("각 프리팹 발사 간격 (초)")]
        float m_FireInterval = 1f;

        [SerializeField]
        [Tooltip("모든 발사 완료 후 쿨타임 (초)")]
        float m_Cooldown = 10f;

        [SerializeField]
        [Tooltip("각 제스처 사이 제한시간 (초)")]
        float m_ComboTimeout = 5f;

        [SerializeField]
        [Tooltip("발사 전 X회전 애니메이션 시간 (초)")]
        float m_RotateDuration = 0.5f;

        [Header("UI")]
        [SerializeField]
        [Tooltip("쿨다운 표시 UI 게임오브젝트 (Image 컴포넌트 필요, Filled 타입)")]
        GameObject m_CooldownObject;

        // 중요: 이 이벤트를 통해 CharacterBarrageManager가 실행 신호를 받습니다.
        public event Action OnComboAction;

        private ComboDisplay m_ComboDisplay;

        enum ComboState
        {
            WaitingForFirst,
            WaitingForSecond,
            WaitingForThird
        }

        ComboState m_CurrentState = ComboState.WaitingForFirst;
        float m_LastGestureTime;
        bool m_IsFiring;
        bool m_IsOnCooldown;

        Image m_CooldownImage;
        float m_CooldownStartTime;
        bool m_IsCoolingDownUI;

        private WaitForSeconds m_FireIntervalWait;
        private WaitForSeconds m_CooldownWait;

        void Awake()
        {
            if (m_CooldownObject != null)
            {
                m_CooldownImage = m_CooldownObject.GetComponent<Image>();
                if (m_CooldownImage != null)
                    m_CooldownImage.fillAmount = 0f;
            }
            m_FireIntervalWait = new WaitForSeconds(m_FireInterval);
            m_CooldownWait = new WaitForSeconds(m_Cooldown);
        }

        void OnEnable()
        {
            if (m_Gesture1 != null) m_Gesture1.gesturePerformed.AddListener(OnGesture1);
            if (m_Gesture2 != null) m_Gesture2.gesturePerformed.AddListener(OnGesture2);
            if (m_Gesture3 != null) m_Gesture3.gesturePerformed.AddListener(OnGesture3);
        }

        void OnDisable()
        {
            if (m_Gesture1 != null) m_Gesture1.gesturePerformed.RemoveListener(OnGesture1);
            if (m_Gesture2 != null) m_Gesture2.gesturePerformed.RemoveListener(OnGesture2);
            if (m_Gesture3 != null) m_Gesture3.gesturePerformed.RemoveListener(OnGesture3);
        }

        void Update()
        {
            if (m_CurrentState != ComboState.WaitingForFirst &&
                !m_IsFiring &&
                Time.time - m_LastGestureTime > m_ComboTimeout)
            {
                ResetCombo();
            }

            if (m_IsCoolingDownUI)
            {
                float elapsed = Time.time - m_CooldownStartTime;
                float ratio = Mathf.Clamp01(elapsed / m_Cooldown);
                float fillAmount = 1f - ratio;

                if (m_CooldownImage != null && Mathf.Abs(m_CooldownImage.fillAmount - fillAmount) > 0.005f)
                    m_CooldownImage.fillAmount = fillAmount;

                if (m_ComboDisplay != null && m_ComboDisplay.CooldownImage != null && Mathf.Abs(m_ComboDisplay.CooldownImage.fillAmount - fillAmount) > 0.005f)
                    m_ComboDisplay.CooldownImage.fillAmount = fillAmount;

                if (ratio >= 1f)
                {
                    m_IsCoolingDownUI = false;
                    if (m_CooldownImage != null)
                        m_CooldownImage.fillAmount = 0f;
                    if (m_ComboDisplay != null && m_ComboDisplay.CooldownImage != null)
                        m_ComboDisplay.CooldownImage.fillAmount = 0f;
                }
            }
        }

        void OnGesture1()
        {
            if (m_IsFiring || m_IsOnCooldown) return;
            m_CurrentState = ComboState.WaitingForSecond;
            m_LastGestureTime = Time.time;

            if (m_ComboDisplay != null)
                m_ComboDisplay.SetStepStatus(0, true);
        }

        void OnGesture2()
        {
            if (m_IsFiring || m_IsOnCooldown) return;

            if (m_CurrentState != ComboState.WaitingForSecond)
            {
                ResetCombo();
                return;
            }
            if (Time.time - m_LastGestureTime > m_ComboTimeout)
            {
                ResetCombo();
                return;
            }

            m_CurrentState = ComboState.WaitingForThird;
            m_LastGestureTime = Time.time;

            if (m_ComboDisplay != null)
                m_ComboDisplay.SetStepStatus(1, true);
        }

        void OnGesture3()
        {
            if (m_IsFiring || m_IsOnCooldown) return;

            if (m_CurrentState != ComboState.WaitingForThird)
            {
                ResetCombo();
                return;
            }
            if (Time.time - m_LastGestureTime > m_ComboTimeout)
            {
                ResetCombo();
                return;
            }

            if (m_ComboDisplay != null)
                m_ComboDisplay.SetStepStatus(2, true);

            ResetCombo();
            StartCoroutine(FireBarrage());
        }

        IEnumerator FireBarrage()
        {
            m_IsFiring = true;

            Transform cam = Camera.main != null ? Camera.main.transform : null;
            if (cam == null)
            {
                m_IsFiring = false;
                yield break;
            }

            int count = Mathf.Min(m_Prefabs.Length, m_SpawnPoints.Length);
            GameObject[] handVisuals = new GameObject[count];

            // 1. 플레이어 손에만 비주얼 임시 생성 및 스케일 적용
            for (int i = 0; i < count; i++)
            {
                if (m_Prefabs[i] == null) continue;

                if (m_SpawnPoints[i] != null)
                {
                    handVisuals[i] = Instantiate(m_Prefabs[i], m_SpawnPoints[i]);
                    handVisuals[i].transform.localPosition = Vector3.zero;
                    handVisuals[i].transform.localRotation = Quaternion.identity;

                    // --- 프리팹 원래 크기에 인스펙터 스케일 값을 곱하여 적용 ---
                    handVisuals[i].transform.localScale = m_Prefabs[i].transform.localScale * m_HandScaleMultiplier;
                }
            }

            // 중요: 외부 매니저가 캐릭터 소환 작업을 처리하도록 이벤트 트리거
            OnComboAction?.Invoke();

            // 2. 플레이어 손 이펙트 발사 시퀀스 (회전 애니메이션 후 비행)
            for (int i = 0; i < count; i++)
            {
                GameObject handObj = handVisuals[i];

                if (handObj != null) handObj.transform.SetParent(null);

                StartCoroutine(LaunchLocalProjectileRoutine(handObj, cam.forward));

                if (i < count - 1)
                    yield return m_FireIntervalWait;
            }

            m_IsFiring = false;
            m_IsOnCooldown = true;
            StartCooldownUI();

            yield return m_CooldownWait;
            m_IsOnCooldown = false;
        }

        IEnumerator LaunchLocalProjectileRoutine(GameObject handObj, Vector3 direction)
        {
            float elapsed = 0f;
            Quaternion handStartRot = handObj != null ? handObj.transform.rotation : Quaternion.identity;
            Quaternion rotOffset = Quaternion.Euler(90f, 0f, 0f);

            while (elapsed < m_RotateDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / m_RotateDuration);
                if (handObj != null) handObj.transform.rotation = Quaternion.Slerp(handStartRot, handStartRot * rotOffset, t);
                yield return null;
            }

            if (handObj != null) SetupRigidbody(handObj, direction);
        }

        private void SetupRigidbody(GameObject obj, Vector3 direction)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb == null) rb = obj.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.linearVelocity = direction * m_Speed;
            Destroy(obj, m_Lifetime);
        }

        void StartCooldownUI()
        {
            m_CooldownStartTime = Time.time;
            m_IsCoolingDownUI = true;

            if (m_CooldownImage != null)
                m_CooldownImage.fillAmount = 1f;

            if (m_ComboDisplay != null && m_ComboDisplay.CooldownImage != null)
                m_ComboDisplay.CooldownImage.fillAmount = 1f;
        }

        void ResetCombo()
        {
            m_CurrentState = ComboState.WaitingForFirst;

            if (m_ComboDisplay != null)
                m_ComboDisplay.ResetSteps();
        }

        public void SetDisplay(ComboDisplay display)
        {
            m_ComboDisplay = display;
        }
    }
}
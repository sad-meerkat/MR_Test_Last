using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Hands.Samples.GestureSample;

namespace UnityEngine.XR.Templates.MRTTabletopAssets
{
    public class LightningBallOnGrab : MonoBehaviour
    {
        [Header("Gesture")]
        [SerializeField] StaticHandGesture m_GrabGesture;

        [Header("Lightning Ball Settings")]
        [SerializeField] GameObject m_LightningBallPrefab;
        [SerializeField] Transform m_Spawner;

        [Header("Spawn Transform Adjustments")]
        [SerializeField] Vector3 m_SpawnOffset = Vector3.zero;
        [SerializeField] Vector3 m_SpawnRotation = Vector3.zero;
        [SerializeField] float m_ScaleMultiplier = 1f;

        [Header("Timing")]
        [SerializeField] float m_Duration = 2f;
        [SerializeField] float m_Cooldown = 3f;

        [Header("UI")]
        [SerializeField] GameObject m_CooldownObject;

        [Header("Blocking")]
        [SerializeField] StaticHandGesture[] m_GesturesToDisable;

        [Header("Hand Swing Integration")]
        [SerializeField]
        [Tooltip("손목(R_Wrist)에 붙어 있는 HandSwordSkill 스크립트를 여기에 할당하세요.")]
        HandSwordSkill m_HandSwingDetector;

        GameObject m_ActiveEffect;
        float m_SpawnTime;
        float m_LastUseTime = -999f;
        Image m_CooldownImage;
        float m_CooldownDuration;
        float m_CooldownStartTime;
        bool m_IsCoolingDown;

        void Awake()
        {
            if (m_CooldownObject != null)
            {
                m_CooldownImage = m_CooldownObject.GetComponent<Image>();
                if (m_CooldownImage != null) m_CooldownImage.fillAmount = 0f;
            }
        }

        void OnEnable()
        {
            if (m_GrabGesture != null)
                m_GrabGesture.gesturePerformed.AddListener(OnGrabPerformed);
        }

        void OnDisable()
        {
            if (m_GrabGesture != null)
                m_GrabGesture.gesturePerformed.RemoveListener(OnGrabPerformed);
            DestroyEffect();
        }

        void Update()
        {
            if (m_ActiveEffect != null && Time.time - m_SpawnTime >= m_Duration)
                DestroyEffect();

            if (m_IsCoolingDown && m_CooldownImage != null)
            {
                float elapsed = Time.time - m_CooldownStartTime;
                float ratio = Mathf.Clamp01(elapsed / m_CooldownDuration);
                m_CooldownImage.fillAmount = 1f - ratio;
                if (ratio >= 1f)
                {
                    m_IsCoolingDown = false;
                    m_CooldownImage.fillAmount = 0f;
                }
            }
        }

        void OnGrabPerformed()
        {
            if (m_ActiveEffect != null || Time.time - m_LastUseTime < (m_Duration + m_Cooldown))
                return;

            // 1. 플레이어 손에 이펙트 생성 및 위치 조절
            if (m_LightningBallPrefab != null && m_Spawner != null)
            {
                m_ActiveEffect = Instantiate(m_LightningBallPrefab, m_Spawner);
                m_ActiveEffect.transform.localPosition = m_SpawnOffset;
                m_ActiveEffect.transform.localEulerAngles = m_SpawnRotation;
                m_ActiveEffect.transform.localScale = m_LightningBallPrefab.transform.localScale * m_ScaleMultiplier;
                m_SpawnTime = Time.time;
            }

            // 2. 캐릭터 손 연동
            if (TableCharacterInput.Instance != null)
                TableCharacterInput.Instance.SetCharacterLightningBallActive(true);

            // 3. 중요: 로컬 카타나 비주얼 없이 "속도 감지"만 활성화!
            if (m_HandSwingDetector != null)
            {
                m_HandSwingDetector.SetSwingOnlyActive(true);
            }

            m_LastUseTime = Time.time;
            SetOtherGestures(false);
            StartCooldownUI(m_Duration + m_Cooldown);
        }

        void StartCooldownUI(float duration)
        {
            if (m_CooldownImage == null) return;
            m_CooldownDuration = duration;
            m_CooldownStartTime = Time.time;
            m_IsCoolingDown = true;
            m_CooldownImage.fillAmount = 1f;
        }

        void DestroyEffect()
        {
            if (m_ActiveEffect != null)
            {
                Destroy(m_ActiveEffect);
                m_ActiveEffect = null;
                SetOtherGestures(true);
            }

            if (TableCharacterInput.Instance != null)
                TableCharacterInput.Instance.SetCharacterLightningBallActive(false);

            // 4. 감지 종료
            if (m_HandSwingDetector != null)
            {
                m_HandSwingDetector.SetSwingOnlyActive(false);
            }
        }

        void SetOtherGestures(bool enabled)
        {
            if (m_GesturesToDisable == null) return;
            foreach (var gesture in m_GesturesToDisable)
            {
                if (gesture != null) gesture.enabled = enabled;
            }
        }
    }
}
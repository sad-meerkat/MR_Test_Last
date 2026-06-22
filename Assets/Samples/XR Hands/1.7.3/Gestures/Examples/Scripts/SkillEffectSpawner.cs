using UnityEngine;

namespace UnityEngine.XR.Hands.Samples.GestureSample
{
    /// <summary>
    /// StaticHandGesture 의 포즈가 인식되면 SkillData 에 지정된 이펙트 프리팹을 생성하고,
    /// 포즈가 종료되면 제거합니다.
    ///
    /// 사용법:
    ///   1. 이 컴포넌트를 씬의 빈 GameObject에 추가합니다.
    ///   2. Skill Data 에 원하는 SkillData 에셋을 연결합니다.
    ///   3. Gesture 에 해당 포즈를 감지하는 StaticHandGesture 컴포넌트를 연결합니다.
    ///   4. Spawn Point 에 이펙트가 생성될 위치의 Transform 을 지정합니다.
    ///      (비워두면 이 GameObject 의 위치를 사용합니다)
    /// </summary>
    public class SkillEffectSpawner : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("포즈와 이펙트 프리팹이 정의된 SkillData 에셋")]
        SkillData m_SkillData;

        [SerializeField]
        [Tooltip("구독할 StaticHandGesture 컴포넌트")]
        StaticHandGesture m_Gesture;

        [SerializeField]
        [Tooltip("이펙트를 생성할 기준 Transform. 비워두면 이 오브젝트 위치 사용")]
        Transform m_SpawnPoint;

        GameObject m_ActiveEffect;

        void OnEnable()
        {
            if (m_Gesture == null)
            {
                Debug.LogWarning($"[SkillEffectSpawner] Gesture 가 연결되지 않았습니다: {name}", this);
                return;
            }

            m_Gesture.gesturePerformed.AddListener(OnGesturePerformed);
            m_Gesture.gestureEnded.AddListener(OnGestureEnded);
        }

        void OnDisable()
        {
            if (m_Gesture == null) return;

            m_Gesture.gesturePerformed.RemoveListener(OnGesturePerformed);
            m_Gesture.gestureEnded.RemoveListener(OnGestureEnded);

            DestroyActiveEffect();
        }

        void OnGesturePerformed()
        {
            if (m_SkillData == null || m_SkillData.skillEffectPrefab == null)
                return;

            // 이미 이펙트가 활성화되어 있으면 중복 생성하지 않음
            if (m_ActiveEffect != null)
                return;

            Transform spawnRoot = m_SpawnPoint != null ? m_SpawnPoint : transform;
            m_ActiveEffect = Instantiate(m_SkillData.skillEffectPrefab, spawnRoot);
            m_ActiveEffect.transform.localPosition = m_SkillData.spawnOffset;
            m_ActiveEffect.transform.localEulerAngles = m_SkillData.spawnRotation;
            m_ActiveEffect.transform.localScale = m_SkillData.skillEffectPrefab.transform.localScale * m_SkillData.scaleMultiplier;
        }

        void OnGestureEnded()
        {
            DestroyActiveEffect();
        }

        void DestroyActiveEffect()
        {
            if (m_ActiveEffect != null)
            {
                Destroy(m_ActiveEffect);
                m_ActiveEffect = null;
            }
        }

        /// <summary>현재 활성화된 이펙트 인스턴스 (없으면 null)</summary>
        public GameObject activeEffect => m_ActiveEffect;
    }
}

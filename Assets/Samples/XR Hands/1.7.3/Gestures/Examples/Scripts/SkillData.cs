using UnityEngine;

namespace UnityEngine.XR.Hands.Samples.GestureSample
{
    /// <summary>
    /// 포즈 하나와 그에 대응하는 스킬 이펙트 프리팹을 정의하는 ScriptableObject.
    /// HandPoses / HandShapes 폴더의 에셋을 handShapeOrPose 에 할당하세요.
    /// </summary>
    [CreateAssetMenu(fileName = "NewSkillData", menuName = "XR Hands/Skill Data")]
    public class SkillData : ScriptableObject
    {
        [SerializeField]
        [Tooltip("감지할 손 포즈 또는 형태 에셋 (XRHandPose / XRHandShape)")]
        ScriptableObject m_HandShapeOrPose;

        [SerializeField]
        [Tooltip("포즈가 인식되었을 때 생성할 이펙트 프리팹")]
        GameObject m_SkillEffectPrefab;

        [SerializeField]
        [Tooltip("스폰 기준점으로부터의 오프셋")]
        Vector3 m_SpawnOffset = Vector3.zero;

        [SerializeField]
        [Tooltip("스폰 시 적용할 회전 오프셋 (Euler)")]
        Vector3 m_SpawnRotation = Vector3.zero;

        [SerializeField]
        [Tooltip("스폰 시 적용할 스케일 배율")]
        float m_ScaleMultiplier = 1.0f;

        /// <summary>HandPoses 폴더의 XRHandPose 또는 XRHandShape 에셋</summary>
        public ScriptableObject handShapeOrPose => m_HandShapeOrPose;

        /// <summary>포즈 인식 시 생성될 이펙트 프리팹</summary>
        public GameObject skillEffectPrefab => m_SkillEffectPrefab;

        /// <summary>스폰 기준점으로부터의 위치 오프셋</summary>
        public Vector3 spawnOffset => m_SpawnOffset;

        /// <summary>스폰 시 적용할 회전 오프셋 (Euler)</summary>
        public Vector3 spawnRotation => m_SpawnRotation;

        /// <summary>스폰 시 적용할 스케일 배율</summary>
        public float scaleMultiplier => m_ScaleMultiplier;
    }
}

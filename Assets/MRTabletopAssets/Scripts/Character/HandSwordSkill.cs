using UnityEngine;
using UnityEngine.Events;

public class HandSwordSkill : MonoBehaviour
{
    [SerializeField] GameObject m_SwordVisual;
    [SerializeField] float m_SwingThreshold = 3.0f;
    [SerializeField] float m_Cooldown = 0.5f;
    public UnityEvent OnSwingDetected;

    private Vector3 m_LastPosition;
    private float m_Velocity;
    private bool m_IsSwordActive = false;
    private float m_LastSwingTime;

    void Start()
    {
        m_LastPosition = transform.position;
        if (m_SwordVisual != null) m_SwordVisual.SetActive(false);
    }

    // 기존 칼 소환 + 휘두름 감지 활성화
    public void SetSwordActive(bool active)
    {
        m_IsSwordActive = active;
        if (m_SwordVisual != null) m_SwordVisual.SetActive(active);
    }

    // 추가: 비주얼(칼)은 건들지 않고, 휘두름 속도 감지만 활성화
    public void SetSwingOnlyActive(bool active)
    {
        m_IsSwordActive = active;
    }

    void Update()
    {
        if (!m_IsSwordActive)
        {
            m_LastPosition = transform.position;
            return;
        }

        Vector3 currentPosition = transform.position;
        m_Velocity = (currentPosition - m_LastPosition).magnitude / Time.deltaTime;

        if (m_Velocity > m_SwingThreshold && Time.time - m_LastSwingTime > m_Cooldown)
        {
            m_LastSwingTime = Time.time;
            OnSwingDetected?.Invoke();
        }

        m_LastPosition = currentPosition;
    }
}
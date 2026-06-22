using UnityEngine;

public class MRSkillManager : MonoBehaviour
{
    [SerializeField] GameObject[] m_ObjectsToActivate;

    public void EnableSkills()
    {
        Debug.Log("[MRSkillManager] EnableSkills called.");
        if (m_ObjectsToActivate == null) return;

        foreach (var obj in m_ObjectsToActivate)
        {
            if (obj != null)
            {
                obj.SetActive(true);
                Debug.Log($"[MRSkillManager] Activated: {obj.name}");
            }
        }
    }
    
    public void DisableSkills()
    {
        if (m_ObjectsToActivate == null) return;
        foreach (var obj in m_ObjectsToActivate)
        {
            if (obj != null) obj.SetActive(false);
        }
    }
}

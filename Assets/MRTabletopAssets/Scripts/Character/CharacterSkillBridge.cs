using UnityEngine;

namespace UnityEngine.XR.Templates.MRTTabletopAssets
{
    public class CharacterSkillBridge : MonoBehaviour
    {
        [Header("HUD Debuggers")]
        [SerializeField] private GameObject m_SasukeDebugger;
        [SerializeField] private GameObject m_ByakuyaDebugger;
        [SerializeField] private GameObject m_DefaultDebugger;

        [Header("Skill Launchers")]
        [SerializeField] private FireballComboLauncher m_FireballLauncher;
        [SerializeField] private ComboBarrageLauncher m_BarrageLauncher;

        public void OnCharacterChanged(TableCharacter character)
        {
            if (character == null)
            {
                SetDebuggerActive(false, false, true);
                if (m_FireballLauncher != null) m_FireballLauncher.enabled = false;
                if (m_BarrageLauncher != null) m_BarrageLauncher.enabled = false;
                return;
            }

            string characterName = character.gameObject.name.ToLower();
            bool isSasuke = characterName.Contains("sasuke");
            bool isByakuya = characterName.Contains("byakuya");

            // Toggle HUD Debuggers
            SetDebuggerActive(isSasuke, isByakuya, !isSasuke && !isByakuya);

            // Toggle Launchers
            if (m_FireballLauncher != null) m_FireballLauncher.enabled = isSasuke;
            if (m_BarrageLauncher != null) m_BarrageLauncher.enabled = isByakuya;

            Debug.Log($"[CharacterSkillBridge] Character changed to {character.name}. Sasuke: {isSasuke}, Byakuya: {isByakuya}");
        }

        private void SetDebuggerActive(bool sasuke, bool byakuya, bool def)
        {
            if (m_SasukeDebugger != null) m_SasukeDebugger.SetActive(sasuke);
            if (m_ByakuyaDebugger != null) m_ByakuyaDebugger.SetActive(byakuya);
            if (m_DefaultDebugger != null) m_DefaultDebugger.SetActive(def);
        }
    }
}

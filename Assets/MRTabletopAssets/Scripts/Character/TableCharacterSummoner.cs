using UnityEngine;
using UnityEngine.Events;

namespace UnityEngine.XR.Templates.MRTTabletopAssets
{
    public class TableCharacterSummoner : MonoBehaviour
    {
        [SerializeField] GameObject m_CharacterPrefab;
        [SerializeField] Transform m_SpawnPoint;
        [SerializeField] TableCharacterInput m_InputHandler;
        [SerializeField] bool m_SummonOnStart = true;
        public UnityEvent OnSummoned;

        private GameObject m_CurrentCharacter;

        void Start()
        {
            if (m_SummonOnStart)
            {
                Summon();
            }
        }

        public void Summon()
        {
            if (m_CurrentCharacter != null)
            {
                Destroy(m_CurrentCharacter);
            }

            if (m_CharacterPrefab == null)
            {
                Debug.LogError("Character Prefab not assigned!", this);
                return;
            }

            Vector3 spawnPos = m_SpawnPoint != null ? m_SpawnPoint.position : transform.position;
            // Adjust Y slightly to be above table
            spawnPos += Vector3.up * 0.05f;

            m_CurrentCharacter = Instantiate(m_CharacterPrefab, spawnPos, Quaternion.identity);
            TableCharacter character = m_CurrentCharacter.GetComponent<TableCharacter>();
            Debug.Log($"[Test] TableCharacterSummoner: Character instantiated: {m_CurrentCharacter.name}. Has TableCharacter: {character != null}");
            
            if (character != null)
            {
                character.SetTable(transform); // Use summoner's transform as table reference
                if (m_InputHandler != null)
                {
                    m_InputHandler.SetCharacter(character);
                    Debug.Log($"[Test] TableCharacterSummoner: Character linked to InputHandler: {m_InputHandler.name}");
                }
            }

            OnSummoned?.Invoke();
        }

        [ContextMenu("Summon Character")]
        void TestSummon()
        {
            Summon();
        }
    }
}

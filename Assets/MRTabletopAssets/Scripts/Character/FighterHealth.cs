using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.XR.Templates.MRTTabletopAssets
{
    public class FighterHealth : NetworkBehaviour
    {
        [SerializeField] float m_MaxHealth = 200f;
        public NetworkVariable<float> health = new NetworkVariable<float>(200f);
        public NetworkVariable<int> playerIndex = new NetworkVariable<int>(0); // 0 for P1, 1 for P2
        [SerializeField] Image m_HealthBar;
        [SerializeField] FightingGameManager m_Manager;
        [Header("Voice Settings")]
        [SerializeField] AudioClip m_HitVoiceClip;

        protected override void OnNetworkPreSpawn(ref NetworkManager networkManager)
        {
            health.Initialize(this);
            playerIndex.Initialize(this);
            base.OnNetworkPreSpawn(ref networkManager);
        }

        void Awake()
        {
            m_Manager = FindFirstObjectByType<FightingGameManager>();
        }

        // FighterHealth.cs의 OnNetworkSpawn() 메서드를 찾아서 아래 코드로 교체합니다.

        public override void OnNetworkSpawn()
        {
            // [추가] 서버에서 생성 시 실제 체력을 최대 체력 값으로 꽉 채워줍니다.
            if (IsServer)
            {
                health.Value = m_MaxHealth;
                Debug.Log($"[FighterHealth] Server initialized health to m_MaxHealth: {m_MaxHealth}");
            }

            health.OnValueChanged += (old, current) => UpdateUI();
            playerIndex.OnValueChanged += (old, current) => UpdateUI();
            UpdateUI();
        }

        void UpdateUI()
        {
            // [주석 해제 및 공식 보정] 캐릭터 머리 위의 체력바 게이지를 실제 체력 비율에 맞춰 깎아줍니다.
            if (m_HealthBar != null && m_MaxHealth > 0)
            {
                m_HealthBar.fillAmount = health.Value / m_MaxHealth;
            }

            var hud = FightingHUDManager.Instance;
            if (hud == null)
            {
                hud = Object.FindFirstObjectByType<FightingHUDManager>();
            }

            if (hud != null)
            {
                hud.UpdateHealth(playerIndex.Value, health.Value, m_MaxHealth);
                Debug.Log($"[FighterHealth] Updating HUD for Player {playerIndex.Value}: {health.Value}/{m_MaxHealth}");
            }
            else
            {
                Debug.LogWarning("[FighterHealth] FightingHUDManager not found!");
            }
        }

        public void TakeDamage(float amount)
        {
            if (!IsServer) return;
            
            health.Value -= amount;
            
            // Trigger hit reaction on all clients
            PlayHitReactionClientRpc();

            if (health.Value <= 0)
            {
                health.Value = 0;
                if (m_Manager != null)
                    m_Manager.FighterDied();
            }
        }

        [Rpc(SendTo.Everyone)]
        private void PlayHitReactionClientRpc()
        {
            Animator animator = GetComponentInChildren<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("hit");
            }
            // [추가] 피격 음성 재생 (3D 사운드)
            if (m_HitVoiceClip != null)
            {
                AudioSource.PlayClipAtPoint(m_HitVoiceClip, transform.position);
            }
            if (IsOwner && TableCharacterInput.Instance != null)
            {
                // 강도 0.5, 지속시간 0.2초 (원하는 값으로 조절 가능)
                TableCharacterInput.Instance.SendHaptic(0.5f, 0.2f);
            }
        }
    }
}

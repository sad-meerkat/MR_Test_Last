using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityEngine.XR.Templates.MRTTabletopAssets
{
    public class FighterController : TableCharacter
    {
        [SerializeField] float m_AttackRange = 0.15f;
        [SerializeField] float m_Damage = 10f;
        [SerializeField] LayerMask m_FighterLayer;

        [SerializeField] InputActionReference m_AttackAction;

        private void OnEnable()
        {
            if (m_AttackAction != null) m_AttackAction.action.Enable();
        }

        private void OnDisable()
        {
            if (m_AttackAction != null) m_AttackAction.action.Disable();
        }

        void Update()
        {
            if (IsSpawned && !IsOwner) return;

            if (m_AttackAction != null && m_AttackAction.action.WasPressedThisFrame())
            {
                PerformSwordSwing();
            }
        }

        public override void PerformSwordSwing()
        {
            if (IsSpawned && !IsOwner) return;

            // base.PerformSwordSwing() handles animator trigger, sword visual, and cooldown
            base.PerformSwordSwing();
            
            // Damage is handled on server
            if (IsSpawned)
                AttackServerRpc();
            else
                LocalAttack();
        }

        private void LocalAttack()
        {
            // Local-only damage check (useful for testing without Netcode)
            Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward * 0.05f, m_AttackRange, m_FighterLayer);
            foreach (var hit in hits)
            {
                if (hit.gameObject != gameObject)
                {
                    if (hit.TryGetComponent<FighterHealth>(out var health))
                    {
                        health.TakeDamage(m_Damage);
                    }
                }
            }
        }

        [Rpc(SendTo.Server)]
        void AttackServerRpc()
        {
            // Damage calculation on server
            Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward * 0.05f, m_AttackRange, m_FighterLayer);
            foreach (var hit in hits)
            {
                if (hit.gameObject != gameObject)
                {
                    if (hit.TryGetComponent<FighterHealth>(out var health))
                    {
                        health.TakeDamage(m_Damage);
                    }
                }
            }
        }

        public override void Jump()
        {
            if (IsSpawned && !IsOwner) return;
            JumpServerRpc();
        }

        [Rpc(SendTo.Server)]
        void JumpServerRpc()
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null && Mathf.Abs(rb.linearVelocity.y) < 0.01f)
            {
                rb.AddForce(Vector3.up * 2f, ForceMode.Impulse);
                JumpClientRpc();
            }
        }

        [Rpc(SendTo.Everyone)]
        void JumpClientRpc()
        {
            if (m_Animator != null)
            {
                m_Animator.SetTrigger("jumping");
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + transform.forward * 0.05f, m_AttackRange);
        }
    }
}


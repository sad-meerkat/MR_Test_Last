using System;
using Unity.Netcode;
using UnityEngine;

namespace UnityEngine.XR.Templates.MRTTabletopAssets
{
    public class CharacterGestureSkillManager : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] GestureProjectileLauncher m_GestureLauncher;
        [SerializeField] FireballComboLauncher m_ComboLauncher; // 콤보 발사기 참조 추가

        [Header("Character Centric Settings")]
        [SerializeField] bool m_SpawnAtCharacter = true;
        [SerializeField] Vector3 m_CharacterSpawnOffset = new Vector3(0, 0.15f, 0.2f);
        [SerializeField] float m_ModelRotationOffset = 90f;

        [Space]
        [SerializeField] Vector3 m_VisualEffectScale = Vector3.one;
        [SerializeField] float m_ProjectileScaleMultiplier = 1.0f;
        [SerializeField] float m_Speed = 15f;
        [SerializeField] float m_Lifetime = 5f;

        [Header("Prefabs")]
        [SerializeField] GameObject m_CharacterEffectPrefab;
        [SerializeField] GameObject m_InvisibleProjectilePrefab;

        [Header("Audio Settings")]
        [SerializeField] AudioClip m_SkillVoiceClip;

        void OnEnable()
        {
            // 단일 제스처 런처 구독
            if (m_GestureLauncher != null)
            {
                m_GestureLauncher.OnGestureAction += OnGesturePerformed;
            }

            // 콤보 제스처 런처 구독
            if (m_ComboLauncher != null)
            {
                m_ComboLauncher.OnComboAction += OnGesturePerformed;
            }
        }

        void OnDisable()
        {
            if (m_GestureLauncher != null)
            {
                m_GestureLauncher.OnGestureAction -= OnGesturePerformed;
            }

            if (m_ComboLauncher != null)
            {
                m_ComboLauncher.OnComboAction -= OnGesturePerformed;
            }
        }

        // 제스처나 콤보가 성공했을 때 공통적으로 실행되는 로직
        void OnGesturePerformed(Vector3 handPos, Vector3 viewDirection)
        {
            TableCharacter character = TableCharacterInput.Instance?.controlledCharacter;

            if (character == null)
            {
                TableCharacter[] characters = UnityEngine.Object.FindObjectsByType<TableCharacter>(FindObjectsSortMode.None);
                foreach (var c in characters)
                {
                    if (c != null && c.gameObject.activeInHierarchy && !c.isDummy && (!c.IsSpawned || c.IsOwner))
                    {
                        character = c;
                        break;
                    }
                }
            }

            if (character == null) return;

            if (m_SpawnAtCharacter)
            {
                Quaternion visualRotation = character.transform.rotation * Quaternion.Euler(0, -m_ModelRotationOffset, 0);
                Vector3 charDirection = visualRotation * Vector3.forward;
                Vector3 charSpawnPos = character.transform.position + visualRotation * m_CharacterSpawnOffset;

                Vector3 finalScale = Vector3.Scale(character.transform.localScale, m_VisualEffectScale) * m_ProjectileScaleMultiplier;

                // 서버에 스킬 생성 요청
                SpawnCharacterSkillServerRpc(charSpawnPos, charDirection, finalScale, character.NetworkObjectId);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnCharacterSkillServerRpc(Vector3 position, Vector3 direction, Vector3 scale, ulong casterObjectId, ServerRpcParams rpcParams = default)
        {
            SpawnCharacterVisualClientRpc(position, direction, scale);

            if (m_InvisibleProjectilePrefab != null)
            {
                GameObject damageObj = Instantiate(m_InvisibleProjectilePrefab, position, Quaternion.LookRotation(direction));
                SetupProjectile(damageObj, direction, scale, false);

                var projDamage = damageObj.GetComponent<ProjectileDamage>();
                if (projDamage != null && NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(casterObjectId, out var netObj))
                {
                    projDamage.owner = netObj.gameObject;
                }

                var damageNetObj = damageObj.GetComponent<NetworkObject>();
                if (damageNetObj != null) damageNetObj.Spawn();

                if (projDamage != null && projDamage.owner != null)
                {
                    var casterCollider = projDamage.owner.GetComponent<Collider>();
                    var projCollider = damageObj.GetComponent<Collider>();
                    if (casterCollider != null && projCollider != null)
                    {
                        Physics.IgnoreCollision(projCollider, casterCollider);
                    }
                }
            }
        }

        [ClientRpc]
        private void SpawnCharacterVisualClientRpc(Vector3 position, Vector3 direction, Vector3 scale)
        {
            if (m_CharacterEffectPrefab != null)
            {
                GameObject visualObj = Instantiate(m_CharacterEffectPrefab, position, Quaternion.LookRotation(direction));

                // [추가] 음성 재생 (캐릭터 위치에서 입체감 있게 재생)
                if (m_SkillVoiceClip != null)
                {
                    AudioSource.PlayClipAtPoint(m_SkillVoiceClip, position);
                }
                var netObj = visualObj.GetComponent<NetworkObject>();
                if (netObj != null) Destroy(netObj);

                SetupProjectile(visualObj, direction, scale, true);
            }
        }

        private void SetupProjectile(GameObject projectile, Vector3 direction, Vector3 scale, bool isPureVisual)
        {
            if (projectile == null) return;

            projectile.transform.localScale = scale;

            if (isPureVisual)
            {
                var colliders = projectile.GetComponentsInChildren<Collider>();
                foreach (var col in colliders) col.enabled = false;

                var damageComponents = projectile.GetComponentsInChildren<ProjectileDamage>();
                foreach (var dmg in damageComponents) Destroy(dmg);
            }

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb == null) rb = projectile.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.linearVelocity = direction * m_Speed;

            Destroy(projectile, m_Lifetime);
        }
    }
}
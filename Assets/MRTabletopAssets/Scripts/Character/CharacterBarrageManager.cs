using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace UnityEngine.XR.Templates.MRTTabletopAssets
{
    public class CharacterBarrageManager : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] ComboBarrageLauncher m_Launcher;

        [Header("Character Centric Settings")]
        [SerializeField] Vector3 m_CharacterSpawnOffset = new Vector3(0, 0.15f, 0.2f);
        [SerializeField] float m_ProjectileScaleMultiplier = 1.0f;
        [SerializeField] float m_ModelRotationOffset = 90f;
        [SerializeField] float m_Speed = 15f;
        [SerializeField] float m_Lifetime = 5f;
        [SerializeField] float m_FireInterval = 0.5f;
        [SerializeField] float m_RotateDuration = 0.5f;

        [Header("Prefabs")]
        [SerializeField]
        [Tooltip("캐릭터에서 순서대로 보여줄 비주얼 이펙트 프리팹들 (최대 5개)")]
        GameObject[] m_CharacterEffectPrefabs = new GameObject[5]; // 기존 단일 프리팹에서 5개 배열로 수정!

        [SerializeField]
        [Tooltip("캐릭터에서 실제로 데미지를 줄 투명 투사체 프리팹 (보통 1개로 공용 사용)")]
        GameObject m_InvisibleProjectilePrefab;

        [Header("Audio Settings")]
        [SerializeField] AudioClip m_SkillVoiceClip;

        void OnEnable()
        {
            if (m_Launcher != null) m_Launcher.OnComboAction += OnComboPerformed;
        }

        void OnDisable()
        {
            if (m_Launcher != null) m_Launcher.OnComboAction -= OnComboPerformed;
        }

        void OnComboPerformed()
        {
            TableCharacter character = GetControlledCharacter();
            if (character == null) return;

            Quaternion visualRotation = character.transform.rotation * Quaternion.Euler(0, -m_ModelRotationOffset, 0);
            Vector3 direction = visualRotation * Vector3.forward;
            Vector3 basePos = character.transform.position + visualRotation * m_CharacterSpawnOffset;
            Vector3 scale = character.transform.localScale * m_ProjectileScaleMultiplier;

            // 서버에 Barrage 생성 및 발사 요청
            SpawnCharacterBarrageServerRpc(basePos, direction, scale, character.NetworkObjectId);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnCharacterBarrageServerRpc(Vector3 basePos, Vector3 direction, Vector3 scale, ulong casterObjectId)
        {
            // 모든 클라이언트에게 비주얼 연사 시작 명령 (위치 동기화를 위해 basePos 전달)
            SpawnVisualBarrageClientRpc(basePos, direction, scale);

            if (m_InvisibleProjectilePrefab != null)
            {
                // [추가] 음성 재생
                if (m_SkillVoiceClip != null)
                {
                    AudioSource.PlayClipAtPoint(m_SkillVoiceClip, basePos);
                }
                // 서버는 데미지 투사체 연사 시작
                StartCoroutine(ServerDamageRoutine(basePos, direction, scale, casterObjectId));
            }
        }

        // --- 서버 전용 물리/데미지 제어 루틴 ---
        IEnumerator ServerDamageRoutine(Vector3 basePos, Vector3 direction, Vector3 scale, ulong casterObjectId)
        {
            int count = 5;
            GameObject[] damageProjectiles = new GameObject[count];

            // 1. 서버에 데미지용 5개 미리 생성해서 나란히 배치
            for (int i = 0; i < count; i++)
            {
                if (m_InvisibleProjectilePrefab == null) continue;

                Vector3 spawnPos = basePos + (Quaternion.LookRotation(direction) * Vector3.right * (i - 2) * 0.15f);
                damageProjectiles[i] = Instantiate(m_InvisibleProjectilePrefab, spawnPos, Quaternion.LookRotation(direction));
                damageProjectiles[i].transform.localScale = scale;

                var projDamage = damageProjectiles[i].GetComponent<ProjectileDamage>();
                if (projDamage != null && NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(casterObjectId, out var netObj))
                {
                    projDamage.owner = netObj.gameObject;
                }

                var damageNetObj = damageProjectiles[i].GetComponent<NetworkObject>();
                if (damageNetObj != null) damageNetObj.Spawn();

                if (projDamage != null && projDamage.owner != null)
                {
                    var casterCollider = projDamage.owner.GetComponent<Collider>();
                    var projCollider = damageProjectiles[i].GetComponent<Collider>();
                    if (casterCollider != null && projCollider != null)
                    {
                        Physics.IgnoreCollision(projCollider, casterCollider);
                    }
                }
            }

            // 2. 딜레이 간격으로 하나씩 날려 보내기
            for (int i = 0; i < count; i++)
            {
                if (damageProjectiles[i] != null)
                {
                    StartCoroutine(LaunchProjectileRoutine(damageProjectiles[i], direction));
                }
                yield return new WaitForSeconds(m_FireInterval);
            }
        }

        // --- 클라이언트 전용 비주얼 연출 수신 ---
        [ClientRpc]
        private void SpawnVisualBarrageClientRpc(Vector3 basePos, Vector3 direction, Vector3 scale)
        {
            StartCoroutine(ClientVisualRoutine(basePos, direction, scale));
        }

        // --- 클라이언트 비주얼 생성 및 제어 루틴 ---
        IEnumerator ClientVisualRoutine(Vector3 basePos, Vector3 direction, Vector3 scale)
        {
            int count = 5;
            GameObject[] visuals = new GameObject[count];

            // 1. 클라이언트 전용 비주얼 이펙트 5개 나란히 선배치
            for (int i = 0; i < count; i++)
            {
                // 인덱스에 매칭되는 프리팹 선택 (할당 안 되어 있으면 스킵)
                GameObject prefabToSpawn = (m_CharacterEffectPrefabs != null && m_CharacterEffectPrefabs.Length > i)
                    ? m_CharacterEffectPrefabs[i]
                    : null;

                if (prefabToSpawn == null) continue;

                Vector3 spawnPos = basePos + (Quaternion.LookRotation(direction) * Vector3.right * (i - 2) * 0.15f);
                visuals[i] = Instantiate(prefabToSpawn, spawnPos, Quaternion.LookRotation(direction));
                visuals[i].transform.localScale = scale;

                var netObj = visuals[i].GetComponent<NetworkObject>();
                if (netObj != null) Destroy(netObj); // 비주얼은 로컬에서만 관리

                // 비주얼용은 콜라이더와 데미지 스크립트 제거
                var colliders = visuals[i].GetComponentsInChildren<Collider>();
                foreach (var col in colliders) col.enabled = false;

                var damageComponents = visuals[i].GetComponentsInChildren<ProjectileDamage>();
                foreach (var dmg in damageComponents) Destroy(dmg);
            }

            // 2. 시간차를 두고 차례대로 회전하고 날아가는 액션 실행
            for (int i = 0; i < count; i++)
            {
                if (visuals[i] != null)
                {
                    StartCoroutine(LaunchProjectileRoutine(visuals[i], direction));
                }
                yield return new WaitForSeconds(m_FireInterval);
            }
        }

        // --- 공통: 회전 애니메이션 후 발사 ---
        IEnumerator LaunchProjectileRoutine(GameObject obj, Vector3 direction)
        {
            float elapsed = 0f;
            Quaternion startRot = obj != null ? obj.transform.rotation : Quaternion.identity;
            Quaternion rotOffset = Quaternion.Euler(90f, 0f, 0f);

            while (elapsed < m_RotateDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / m_RotateDuration);
                if (obj != null) obj.transform.rotation = Quaternion.Slerp(startRot, startRot * rotOffset, t);
                yield return null;
            }

            if (obj != null) SetupRigidbody(obj, direction);
        }

        private void SetupRigidbody(GameObject obj, Vector3 direction)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb == null) rb = obj.AddComponent<Rigidbody>();
            rb.isKinematic = false; // 물리 적용 가능하게 설정
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
            rb.linearVelocity = direction * m_Speed;
            Destroy(obj, m_Lifetime);
        }

        private TableCharacter GetControlledCharacter()
        {
            if (TableCharacterInput.Instance?.controlledCharacter != null) return TableCharacterInput.Instance.controlledCharacter;
            foreach (var c in Object.FindObjectsByType<TableCharacter>(FindObjectsSortMode.None))
                if (c != null && c.gameObject.activeInHierarchy && !c.isDummy && (!c.IsSpawned || c.IsOwner)) return c;
            return null;
        }
    }
}
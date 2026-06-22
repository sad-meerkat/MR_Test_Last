using UnityEngine;
using Unity.Netcode;

namespace UnityEngine.XR.Templates.MRTTabletopAssets
{
    public class TableCharacter : NetworkBehaviour
    {
        [SerializeField] float m_MoveSpeed = 3.0f;
        [SerializeField] float m_RotationSpeed = 15f;
        [SerializeField] float m_RotationOffset = 0f;
        [SerializeField] GameObject m_SkillEffectPrefab;
        [SerializeField] Transform m_SkillSpawnPoint;
        [SerializeField] float m_JumpForce = 5f;
        [SerializeField] float m_SprintMultiplier = 2.0f;
        [SerializeField] protected Animator m_Animator;
        [SerializeField] GameObject m_SwordVisual;
        [SerializeField] GameObject m_LightningBallVisual;
        // TableCharacter.cs 필드 추가
        [SerializeField] AudioClip m_LightningBallVoice;
        [Header("State Voices")]
        [SerializeField] AudioClip m_StartMoveVoice;
        [SerializeField] AudioClip m_SprintVoice;
        [SerializeField] AudioClip m_JumpVoice;

        public bool isDummy;

        private NetworkVariable<bool> m_SwordActiveNet = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        private NetworkVariable<bool> m_LightningBallActiveNet = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        protected Rigidbody m_Rigidbody;
        private AudioSource m_VoiceSource; // 추가: 목소리 전용 오디오 소스
        private float m_LastVoiceTime;    // 추가: 중복 재생 방지 타이머
        private Transform m_TableTransform;
        private Vector2 m_InputVector;
        private float m_LastMoveLogTime;
        private Transform m_MainCameraTransform;
        private bool m_IsSprinting;
        private bool m_WasMoving = false; // 이동 상태 추적용

        void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            if (m_Rigidbody == null)
            {
                m_Rigidbody = gameObject.AddComponent<Rigidbody>();
            }

            m_Rigidbody.useGravity = true;
            m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            m_Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;

            if (m_Animator == null)
            {
                m_Animator = GetComponentInChildren<Animator>();
            }

            CacheMainCamera();

            // === 아래 내용을 추가하세요 ===
            m_VoiceSource = GetComponent<AudioSource>();
            if (m_VoiceSource == null)
            {
                m_VoiceSource = gameObject.AddComponent<AudioSource>();
            }
            m_VoiceSource.spatialBlend = 1.0f; // 3D 사운드 설정 (가까울수록 크게 들림)
        }

        private void CacheMainCamera()
        {
            if (m_MainCameraTransform == null)
            {
                var cam = Camera.main;
                if (cam != null)
                {
                    m_MainCameraTransform = cam.transform;
                }
            }
        }

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                if (TableCharacterInput.Instance != null)
                {
                    TableCharacterInput.Instance.SetCharacter(this);
                    Debug.Log($"[TableCharacter] {gameObject.name} (Owner) registered with TableCharacterInput.", this);
                }
                else
                {
                    // Search for it manually if Instance is not ready yet
                    var input = Object.FindFirstObjectByType<TableCharacterInput>();
                    if (input != null)
                    {
                        input.SetCharacter(this);
                        Debug.Log($"[TableCharacter] {gameObject.name} (Owner) registered with found TableCharacterInput.", this);
                    }
                    else
                    {
                        Debug.LogWarning($"[TableCharacter] {gameObject.name} (Owner) spawned but no TableCharacterInput found. It will try to link during Update.", this);
                    }
                }
            }

            m_SwordActiveNet.OnValueChanged += (oldValue, newValue) =>
            {
                Debug.Log($"[TableCharacter] {gameObject.name} SwordActive Changed: {newValue}");
                if (m_SwordVisual != null) m_SwordVisual.SetActive(newValue);
            };
            // Initial state sync
            if (m_SwordVisual != null) m_SwordVisual.SetActive(m_SwordActiveNet.Value);

            // 새 번개 구 동기화 추가
            m_LightningBallActiveNet.OnValueChanged += (oldVal, newVal) => {
                if (m_LightningBallVisual != null) m_LightningBallVisual.SetActive(newVal);
                if (newVal == true && m_LightningBallVoice != null)
                {
                    AudioSource.PlayClipAtPoint(m_LightningBallVoice, transform.position);
                }
            };
            if (m_LightningBallVisual != null) m_LightningBallVisual.SetActive(m_LightningBallActiveNet.Value);
        
        }

        public void SetLightningBallActive(bool active)
        {
            if (m_LightningBallVisual != null) m_LightningBallVisual.SetActive(active);
            if (IsSpawned) SetLightningBallActiveServerRpc(active);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetLightningBallActiveServerRpc(bool active)
        {
            m_LightningBallActiveNet.Value = active;
        }

        public void SetTable(Transform table)
        {
            m_TableTransform = table;
        }

        private float m_CurrentSpeedFloat;

        public void SetSprinting(bool sprinting)
        {
            if (m_IsSprinting == sprinting) return;
            m_IsSprinting = sprinting;

            // 질주 시작 시 음성 재생
            if (m_IsSprinting && m_SprintVoice != null) AudioSource.PlayClipAtPoint(m_SprintVoice, transform.position);
            if (m_Animator != null)
            {
                m_Animator.SetBool("naruto_run", m_IsSprinting);
            }
        }

        public virtual void Move(Vector2 input)
        {
            if (IsSpawned && !IsOwner) return;
            m_InputVector = input;

            float speed = input.magnitude;
            // 임계값을 0.15f로 높여서 미세한 떨림으로 인한 중복 재생 방지
            bool isMovingNow = speed > 0.15f;

            // [수정됨] 정지 상태에서 움직이기 시작할 때
            if (!m_WasMoving && isMovingNow)
            {
                // 1.5초 이내 중복 재생 방지 및 m_VoiceSource 사용
                if (m_StartMoveVoice != null && Time.time - m_LastVoiceTime > 1.5f)
                {
                    if (m_VoiceSource != null)
                    {
                        m_VoiceSource.clip = m_StartMoveVoice;
                        m_VoiceSource.Play();
                        m_LastVoiceTime = Time.time;
                    }
                }
            }
            // [추가됨] 움직이다가 멈췄을 때 소리 즉시 정지
            else if (m_WasMoving && !isMovingNow)
            {
                if (m_VoiceSource != null && m_VoiceSource.isPlaying && m_VoiceSource.clip == m_StartMoveVoice)
                {
                    m_VoiceSource.Stop();
                }
            }

            m_WasMoving = isMovingNow;

            if (m_Animator != null && Mathf.Abs(m_CurrentSpeedFloat - speed) > 0.01f)
            {
                m_CurrentSpeedFloat = speed;
                m_Animator.SetFloat("Speed", speed);
            }
        }

        void FixedUpdate()
        {
            if (IsSpawned && !IsOwner) return;

            if (m_InputVector.sqrMagnitude > 0.01f)
            {
                // Robust Camera Selection - Use Cached Camera
if (m_MainCameraTransform == null) CacheMainCamera();
                Transform camTransform = m_MainCameraTransform;
                
                Vector3 camForward, camRight;
if (camTransform != null)
                {
                    camForward = camTransform.forward;
                    camRight = camTransform.right;
                    camForward.y = 0;
                    camRight.y = 0;
                    camForward.Normalize();
                    camRight.Normalize();
                }
                else
                {
                    // FIXED: Use world-space defaults instead of self to avoid rotation feedback loops
                    camForward = Vector3.forward;
                    camRight = Vector3.right;
                }

                if (camForward.sqrMagnitude < 0.001f) camForward = Vector3.forward;
                if (camRight.sqrMagnitude < 0.001f) camRight = Vector3.right;

                Vector3 direction = camForward * m_InputVector.y + camRight * m_InputVector.x;
                if (direction.sqrMagnitude > 0.001f)
                {
                    direction.Normalize();

                    // Movement using MovePosition (better for NetworkTransform sync)
                    float currentSpeed = m_IsSprinting ? m_MoveSpeed * m_SprintMultiplier : m_MoveSpeed;
                    Vector3 movement = direction * currentSpeed * Time.fixedDeltaTime;
                    m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
                    
                    // Rotation
Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up) * Quaternion.Euler(0, m_RotationOffset, 0);
                    m_Rigidbody.MoveRotation(Quaternion.Slerp(m_Rigidbody.rotation, targetRotation, m_RotationSpeed * Time.fixedDeltaTime));
                    
                    // FIXED: Stop residual angular velocity
                    m_Rigidbody.angularVelocity = Vector3.zero;

                    Debug.DrawRay(transform.position + Vector3.up * 0.1f, direction * 0.5f, Color.cyan);
                }
            }
            else
            {
                // Stop horizontal movement quickly when no input
                m_Rigidbody.linearVelocity = new Vector3(0, m_Rigidbody.linearVelocity.y, 0);
                // FIXED: Ensure angular velocity is zeroed when not moving
                m_Rigidbody.angularVelocity = Vector3.zero;
            }
        }

        public virtual void Jump()
        {
            if (IsSpawned && !IsOwner) return;
            if (Mathf.Abs(m_Rigidbody.linearVelocity.y) < 0.01f)
            {
                m_Rigidbody.AddForce(Vector3.up * m_JumpForce, ForceMode.Impulse);
                if (m_JumpVoice != null) AudioSource.PlayClipAtPoint(m_JumpVoice, transform.position);
                if (m_Animator != null) m_Animator.SetTrigger("jumping");
            }
        }

        [SerializeField] float m_SkillCooldown = 0.5f;
        private float m_LastSkillTime;

        public virtual void PerformSwordSwing()
        {
            if (IsSpawned && !IsOwner) return;

            if (Time.time - m_LastSkillTime < m_SkillCooldown) return;
            m_LastSkillTime = Time.time;

            // 1. Handle Animation (Local trigger, NetworkAnimator handles sync if present)
            if (m_Animator != null)
            {
                m_Animator.SetTrigger("attack1");
                Debug.Log($"[TableCharacter] {gameObject.name} triggering attack1");
            }

            if (m_LightningBallVisual == null || !m_LightningBallVisual.activeSelf)
            {
                SetSwordActive(true);
            }
        }

        public virtual void CastSkill()
        {
            if (IsSpawned && !IsOwner) return;

            if (Time.time - m_LastSkillTime < m_SkillCooldown) return;
            m_LastSkillTime = Time.time;

            if (m_Animator != null)
            {
                m_Animator.SetTrigger("attack1");
            }
            
            if (m_SkillEffectPrefab != null)
            {
                if (IsSpawned)
                    SpawnSkillEffectServerRpc();
                else
                    LocalSpawnSkillEffect();
            }
        }

        private void LocalSpawnSkillEffect()
        {
            Transform spawnPoint = m_SkillSpawnPoint != null ? m_SkillSpawnPoint : transform;
            GameObject effect = Instantiate(m_SkillEffectPrefab, spawnPoint);
            Destroy(effect, 2f);
        }

        [ServerRpc]
        private void SpawnSkillEffectServerRpc()
        {
            Transform spawnPoint = m_SkillSpawnPoint != null ? m_SkillSpawnPoint : transform;
            GameObject effect = Instantiate(m_SkillEffectPrefab, spawnPoint);
            var netObj = effect.GetComponent<NetworkObject>();
            if (netObj != null) netObj.Spawn();
            Destroy(effect, 2f);
        }

        public void SetSwordActive(bool active)
        {
            // Always set local state for immediate feedback
            if (m_SwordVisual != null) m_SwordVisual.SetActive(active);

            // Sync if networked
            if (IsSpawned)
            {
                SetSwordActiveServerRpc(active);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetSwordActiveServerRpc(bool active)
        {
            m_SwordActiveNet.Value = active;
        }
}
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Hands.Samples.GestureSample;
using Unity.Netcode;

namespace UnityEngine.XR.Templates.MRTTabletopAssets
{
    public class GestureProjectileLauncher : NetworkBehaviour
    {
        [Header("Gesture")]
        [SerializeField] StaticHandGesture m_Gesture;

        [Header("Projectile Settings (Hand Visual)")]
        [SerializeField] GameObject m_Prefab;
        [SerializeField] Transform m_SpawnPoint;
        [SerializeField] float m_Speed = 15f;
        [SerializeField] float m_Lifetime = 5f;
        [SerializeField] float m_Cooldown = 3f;

        [Header("UI")]
        [SerializeField] GameObject m_CooldownObject;

        public event System.Action<Vector3, Vector3> OnGestureAction;

        float m_LastFireTime = -999f;
        Image m_CooldownImage;
        float m_CooldownStartTime;
        bool m_IsCoolingDown;

        private Transform m_MainCameraTransform;

        void Awake()
        {
            if (m_CooldownObject != null)
            {
                m_CooldownImage = m_CooldownObject.GetComponent<Image>();
                if (m_CooldownImage != null) m_CooldownImage.fillAmount = 0f;
            }
            CacheMainCamera();
        }

        private void CacheMainCamera()
        {
            if (m_MainCameraTransform == null)
            {
                var cam = Camera.main;
                if (cam != null) m_MainCameraTransform = cam.transform;
            }
        }

        void OnEnable()
        {
            if (m_Gesture != null) m_Gesture.gesturePerformed.AddListener(OnGesturePerformed);
        }

        void OnDisable()
        {
            if (m_Gesture != null) m_Gesture.gesturePerformed.RemoveListener(OnGesturePerformed);
        }

        void Update()
        {
            if (m_IsCoolingDown && m_CooldownImage != null)
            {
                float elapsed = Time.time - m_CooldownStartTime;
                float ratio = Mathf.Clamp01(elapsed / m_Cooldown);
                m_CooldownImage.fillAmount = 1f - ratio;

                if (ratio >= 1f)
                {
                    m_IsCoolingDown = false;
                    m_CooldownImage.fillAmount = 0f;
                }
            }
        }

        void OnGesturePerformed()
        {
            if (Time.time - m_LastFireTime < m_Cooldown) return;

            if (m_MainCameraTransform == null) CacheMainCamera();
            Transform cam = m_MainCameraTransform;
            if (cam == null) return;

            Vector3 direction = cam.forward;
            Vector3 handPos = m_SpawnPoint != null ? m_SpawnPoint.position : Vector3.zero;

            // 1. Local hand visual spawning
            if (m_Prefab != null && m_SpawnPoint != null)
            {
                SpawnLocalVisual(m_Prefab, handPos, direction, Vector3.one);
            }

            // 2. Invoke event for character-centric skills
            OnGestureAction?.Invoke(handPos, direction);

            // 3. Sync hand visual to other clients
            if (IsOwner || IsServer)
            {
                SpawnHandVisualServerRpc(handPos, direction);
            }

            m_LastFireTime = Time.time;
            StartCooldownUI();
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnHandVisualServerRpc(Vector3 handSpawnPos, Vector3 direction)
        {
            SpawnHandVisualClientRpc(handSpawnPos, direction, OwnerClientId);
        }

        [ClientRpc]
        private void SpawnHandVisualClientRpc(Vector3 handSpawnPos, Vector3 direction, ulong casterClientId)
        {
            if (NetworkManager.Singleton.LocalClientId != casterClientId)
            {
                if (m_Prefab != null)
                {
                    SpawnLocalVisual(m_Prefab, handSpawnPos, direction, Vector3.one);
                }
            }
        }

        private void SpawnLocalVisual(GameObject prefab, Vector3 pos, Vector3 direction, Vector3 scale)
        {
            if (prefab == null) return;
            
            GameObject localObj = Instantiate(prefab, pos, Quaternion.LookRotation(direction));
            
            var netObj = localObj.GetComponent<NetworkObject>();
            if (netObj != null) Destroy(netObj);
            
            SetupVisualEffect(localObj, direction, scale);
        }

        private void SetupVisualEffect(GameObject effect, Vector3 direction, Vector3 scale)
        {
            effect.transform.localScale = scale;
            
            var colliders = effect.GetComponentsInChildren<Collider>();
            foreach (var col in colliders) col.enabled = false;
            
            var damageComponents = effect.GetComponentsInChildren<ProjectileDamage>();
            foreach (var dmg in damageComponents) Destroy(dmg);
            
            Rigidbody rb = effect.GetComponent<Rigidbody>();
            if (rb == null) rb = effect.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Discrete; 
            rb.linearVelocity = direction * m_Speed;
            
            Destroy(effect, m_Lifetime);
        }

        void StartCooldownUI()
        {
            if (m_CooldownImage == null) return;
            m_CooldownStartTime = Time.time;
            m_IsCoolingDown = true;
            m_CooldownImage.fillAmount = 1f;
        }
    }
}

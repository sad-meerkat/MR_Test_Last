using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Hands.Samples.GestureSample;
using Unity.Netcode;

namespace UnityEngine.XR.Templates.MRTTabletopAssets
{
    public class FireballComboLauncher : NetworkBehaviour
    {
        [Header("Gesture Sequence")]
        [SerializeField] StaticHandGesture m_Gesture1_Fist;
        [SerializeField] StaticHandGesture m_Gesture2_OpenPalm;
        [SerializeField] StaticHandGesture m_Gesture3_PointAt;

        [Header("Fireball Settings (Hand Visual)")]
        [SerializeField] GameObject m_FireballPrefab;
        [SerializeField] Transform m_SpawnPoint;
        [SerializeField] Transform m_CameraTransform;
        [SerializeField] float m_ComboTimeout = 5f;
        [SerializeField] float m_FireballSpeed = 15f;
        [SerializeField] float m_FireballLifetime = 5f;
        [SerializeField] float m_Cooldown = 7f;

        [Header("UI")]
        [SerializeField] GameObject m_CooldownObject;

        // Event for CharacterGestureSkillManager to listen to
        public event System.Action<Vector3, Vector3> OnComboAction;

        enum ComboState { WaitingForFirst, WaitingForSecond, WaitingForThird }
        ComboState m_CurrentState = ComboState.WaitingForFirst;
float m_LastGestureTime;
        float m_LastFireTime = -999f;

        Image m_CooldownImage;
        float m_CooldownDuration;
        float m_CooldownStartTime;
        bool m_IsCoolingDown;

        private Transform m_MainCameraTransform;
        private ComboDisplay m_ComboDisplay;

        public void SetDisplay(ComboDisplay display) { m_ComboDisplay = display; }

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
            if (m_Gesture1_Fist != null) m_Gesture1_Fist.gesturePerformed.AddListener(OnFistPerformed);
            if (m_Gesture2_OpenPalm != null) m_Gesture2_OpenPalm.gesturePerformed.AddListener(OnOpenPalmPerformed);
            if (m_Gesture3_PointAt != null) m_Gesture3_PointAt.gesturePerformed.AddListener(OnPointAtPerformed);
        }

        void OnDisable()
        {
            if (m_Gesture1_Fist != null) m_Gesture1_Fist.gesturePerformed.RemoveListener(OnFistPerformed);
            if (m_Gesture2_OpenPalm != null) m_Gesture2_OpenPalm.gesturePerformed.RemoveListener(OnOpenPalmPerformed);
            if (m_Gesture3_PointAt != null) m_Gesture3_PointAt.gesturePerformed.RemoveListener(OnPointAtPerformed);
        }

        void Update()
        {
            if (m_CurrentState != ComboState.WaitingForFirst && Time.time - m_LastGestureTime > m_ComboTimeout)
            {
                ResetCombo();
            }

            if (m_IsCoolingDown)
            {
                float elapsed = Time.time - m_CooldownStartTime;
                float ratio = Mathf.Clamp01(elapsed / m_CooldownDuration);
                float fillAmount = 1f - ratio;

                if (m_CooldownImage != null) m_CooldownImage.fillAmount = fillAmount;
                if (m_ComboDisplay != null && m_ComboDisplay.CooldownImage != null) m_ComboDisplay.CooldownImage.fillAmount = fillAmount;

                if (ratio >= 1f)
                {
                    m_IsCoolingDown = false;
                    if (m_CooldownImage != null) m_CooldownImage.fillAmount = 0f;
                    if (m_ComboDisplay != null && m_ComboDisplay.CooldownImage != null) m_ComboDisplay.CooldownImage.fillAmount = 0f;
                }
            }
        }

        void OnFistPerformed()
        {
            if (Time.time - m_LastFireTime < m_Cooldown) return;
            m_CurrentState = ComboState.WaitingForSecond;
            m_LastGestureTime = Time.time;
            if (m_ComboDisplay != null) m_ComboDisplay.SetStepStatus(0, true);
        }

        void OnOpenPalmPerformed()
        {
            if (m_CurrentState != ComboState.WaitingForSecond || Time.time - m_LastGestureTime > m_ComboTimeout) { ResetCombo(); return; }
            m_CurrentState = ComboState.WaitingForThird;
            m_LastGestureTime = Time.time;
            if (m_ComboDisplay != null) m_ComboDisplay.SetStepStatus(1, true);
        }

        void OnPointAtPerformed()
        {
            if (m_CurrentState != ComboState.WaitingForThird || Time.time - m_LastGestureTime > m_ComboTimeout) { ResetCombo(); return; }
            if (m_ComboDisplay != null) m_ComboDisplay.SetStepStatus(2, true);
            LaunchFireball();
        }

        void LaunchFireball()
{
            Transform cam = m_CameraTransform != null ? m_CameraTransform : (Camera.main != null ? Camera.main.transform : null);
            if (cam == null) return;

            Vector3 direction = cam.forward;
            Vector3 handSpawnPos = m_SpawnPoint != null ? m_SpawnPoint.position : transform.position;

            // 1. Hand visual (Local feedback)
            if (m_FireballPrefab != null)
            {
                SpawnLocalVisual(m_FireballPrefab, handSpawnPos, direction, Vector3.one);
                // Synchronize hand visual for others
                SpawnHandVisualServerRpc(handSpawnPos, direction);
            }

            // 2. Notify manager to handle character skill
            OnComboAction?.Invoke(handSpawnPos, direction);

            m_LastFireTime = Time.time;
            ResetCombo();
            StartCooldownUI(m_Cooldown);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnHandVisualServerRpc(Vector3 position, Vector3 direction)
        {
            SpawnHandVisualClientRpc(position, direction, OwnerClientId);
        }

        [ClientRpc]
        private void SpawnHandVisualClientRpc(Vector3 position, Vector3 direction, ulong casterClientId)
        {
            // Don't spawn again for the caster as they already did it locally
            if (NetworkManager.Singleton.LocalClientId != casterClientId && m_FireballPrefab != null)
            {
                SpawnLocalVisual(m_FireballPrefab, position, direction, Vector3.one);
            }
        }

        private void SpawnLocalVisual(GameObject prefab, Vector3 pos, Vector3 direction, Vector3 scale)
        {
            GameObject localObj = Instantiate(prefab, pos, Quaternion.LookRotation(direction));
            var netObj = localObj.GetComponent<NetworkObject>();
            if (netObj != null) Destroy(netObj);

            SetupProjectileLocal(localObj, direction, scale, true);
        }

        private void SetupProjectileLocal(GameObject projectile, Vector3 direction, Vector3 scale, bool isPureVisual)
        {
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
            rb.linearVelocity = direction * m_FireballSpeed;

            Destroy(projectile, m_FireballLifetime);
        }

        void StartCooldownUI(float duration)
        {
            m_CooldownDuration = duration;
            m_CooldownStartTime = Time.time;
            m_IsCoolingDown = true;
            if (m_CooldownImage != null) m_CooldownImage.fillAmount = 1f;
            if (m_ComboDisplay != null && m_ComboDisplay.CooldownImage != null) m_ComboDisplay.CooldownImage.fillAmount = 1f;
        }

        void ResetCombo()
        {
            m_CurrentState = ComboState.WaitingForFirst;
            if (m_ComboDisplay != null) m_ComboDisplay.ResetSteps();
        }
    }
}
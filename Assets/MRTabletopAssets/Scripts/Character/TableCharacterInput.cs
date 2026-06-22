using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

namespace UnityEngine.XR.Templates.MRTTabletopAssets
{
    public class TableCharacterInput : MonoBehaviour
    {
        public static TableCharacterInput Instance { get; private set; }

        [SerializeField] InputActionReference m_MoveAction;
        [SerializeField] InputActionReference m_JumpAction;
        [SerializeField] InputActionReference m_SprintAction;
        [SerializeField] TableCharacter m_ControlledCharacter;
        public TableCharacter controlledCharacter => m_ControlledCharacter;

        private float m_NextSearchTime = 0f;
        private float m_LogTimer = 0f;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        void OnEnable()
        {
            if (m_MoveAction != null && m_MoveAction.action != null)
            {
                m_MoveAction.action.Enable();
            }
            if (m_SprintAction != null && m_SprintAction.action != null)
            {
                m_SprintAction.action.Enable();
            }
            if (m_JumpAction != null && m_JumpAction.action != null)
            {
                m_JumpAction.action.Enable();
            }
        }

        void OnDisable()
        {
            if (m_MoveAction != null && m_MoveAction.action != null)
            {
                m_MoveAction.action.Disable();
            }
            if (m_SprintAction != null && m_SprintAction.action != null)
            {
                m_SprintAction.action.Disable();
            }
            if (m_JumpAction != null && m_JumpAction.action != null)
            {
                m_JumpAction.action.Disable();
            }
        }

        void Start()
        {
            // Safety measure to ensure actions are enabled
            if (m_MoveAction != null && m_MoveAction.action != null)
            {
                m_MoveAction.action.Enable();
            }
            if (m_SprintAction != null && m_SprintAction.action != null)
            {
                m_SprintAction.action.Enable();
            }
            if (m_JumpAction != null && m_JumpAction.action != null)
            {
                m_JumpAction.action.Enable();
            }
        }

        void Update()
        {
            // Re-search if character is null OR inactive in hierarchy
            if (m_ControlledCharacter == null || !m_ControlledCharacter.gameObject.activeInHierarchy)
            {
                if (Time.time >= m_NextSearchTime)
                {
                    m_NextSearchTime = Time.time + 5.0f; // Check less frequently (every 5s)
                    
                    // Optimization: Use FindObjectsByType to iterate through all characters
                    TableCharacter[] characters = Object.FindObjectsByType<TableCharacter>(FindObjectsSortMode.None);
                    foreach (var character in characters)
                    {
                        if (character != null && character.gameObject.activeInHierarchy && !character.isDummy && (!character.IsSpawned || character.IsOwner))
                        {
                            SetCharacter(character);
                            break;
                        }
                    }
                }

                if (m_ControlledCharacter == null) return;
            }

            if (m_MoveAction != null && m_MoveAction.action != null)
            {
                Vector2 input = m_MoveAction.action.ReadValue<Vector2>();
                m_ControlledCharacter.Move(input);
            }

            if (m_SprintAction != null && m_SprintAction.action != null)
            {
                m_ControlledCharacter.SetSprinting(m_SprintAction.action.IsPressed());
            }

            if (m_JumpAction != null && m_JumpAction.action != null && m_JumpAction.action.WasPressedThisFrame())
{
                m_ControlledCharacter.Jump();
            }
        }

        public void SendHaptic(float amplitude, float duration)
        {
            // 1. 일반 게임패드 진동 처리
            if (Gamepad.current != null)
            {
                Gamepad.current.SetMotorSpeeds(amplitude, amplitude);
                // 일정 시간 후 진동 중지
                Invoke(nameof(StopGamepadHaptic), duration);
            }

            // 2. XR 컨트롤러 진동 처리 (XRI 컴포넌트 활용)
            var hapticPlayers = Object.FindObjectsByType<HapticImpulsePlayer>(FindObjectsSortMode.None);
            foreach (var player in hapticPlayers)
            {
                player.SendHapticImpulse(amplitude, duration);
            }
        }

        private void StopGamepadHaptic()
        {
            Gamepad.current?.SetMotorSpeeds(0, 0);
        }

        public void SetCharacterLightningBallActive(bool active)
        {
            if (m_ControlledCharacter != null)
                m_ControlledCharacter.SetLightningBallActive(active);
        }
        public void SetCharacter(TableCharacter character)
        {
            if (character == null) return;
            m_ControlledCharacter = character;

            // Notify CharacterSkillBridge
            var bridge = GetComponent<CharacterSkillBridge>();
            if (bridge != null)
            {
                bridge.OnCharacterChanged(character);
            }
        }

        // Called by Hand Gesture Events (e.g. Thumbs Up)
        public void OnGesturePerformed()
        {
            if (m_ControlledCharacter != null)
            {
                m_ControlledCharacter.CastSkill();
            }
            else
            {
            }
        }

        // Called by Hand Swing Detection
        public void OnHandSwing()
        {
            if (m_ControlledCharacter != null)
            {
                m_ControlledCharacter.PerformSwordSwing();
            }
            else
            {
                // Try emergency find
                TryFindCharacter();
                if (m_ControlledCharacter != null) m_ControlledCharacter.PerformSwordSwing();
            }
        }

        public void SetCharacterSwordActive(bool active)
        {
            if (m_ControlledCharacter != null)
            {
                m_ControlledCharacter.SetSwordActive(active);
            }
            else
            {
                // Try emergency find
                TryFindCharacter();
                if (m_ControlledCharacter != null) m_ControlledCharacter.SetSwordActive(active);
            }
        }

        private void TryFindCharacter()
        {
            TableCharacter[] characters = Object.FindObjectsByType<TableCharacter>(FindObjectsSortMode.None);
            foreach (var character in characters)
            {
                if (character != null && character.gameObject.activeInHierarchy && !character.isDummy && (!character.IsSpawned || character.IsOwner))
                {
                    SetCharacter(character);
                    break;
                }
            }
        }
    }
}

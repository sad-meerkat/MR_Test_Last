using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;
using XRMultiplayer;

namespace UnityEngine.XR.Templates.MRTTabletopAssets
{
    public enum FightingGameState { Idle, Selecting, Fighting }

    public class FightingGameManager : NetworkBehaviour, IGameMode
{
        [SerializeField] int m_GameModeID = 4;
        public int gameModeID => m_GameModeID;

        [SerializeField] GameObject[] m_ObjectsToToggle;
        [SerializeField] GameObject m_PreGameUI;
        [SerializeField] GameObject m_InGameUI;
        [SerializeField] GameObject m_StartButton;
        [SerializeField] GameObject m_CharacterSelectionUI;
        [SerializeField] GameObject m_SingleModePopup;
        [SerializeField] Color m_P1SelectColor = Color.blue;
        [SerializeField] Color m_P2SelectColor = Color.red;
        [SerializeField] Color m_BothSelectColor = Color.magenta;
        [SerializeField] TextMeshProUGUI m_SelectionTimerText;
        [SerializeField] GameObject[] m_CharacterPrefabs;

        [SerializeField] GameObject m_RobotPrefab;
        [SerializeField] GameObject m_KnightPrefab;

        [SerializeField] Transform m_SpawnPoint1;
        [SerializeField] Transform m_SpawnPoint2;
        [SerializeField] Transform m_SpawnPoint3;
        [SerializeField] Transform m_SpawnPoint4;

        public UnityEvent OnSummoned;

        private NetworkVariable<FightingGameState> m_GameState = new NetworkVariable<FightingGameState>(FightingGameState.Idle);
        private NetworkVariable<float> m_SelectionTimer = new NetworkVariable<float>(30f);
        private NetworkVariable<float> m_GameTimer = new NetworkVariable<float>(180f);
        private NetworkVariable<int> m_WinnerIndex = new NetworkVariable<int>(-1); // -1: None, 0: P1, 1: P2, 2: Draw
        private NetworkVariable<bool> m_P1PreReady = new NetworkVariable<bool>(false);
        private NetworkVariable<bool> m_P2PreReady = new NetworkVariable<bool>(false);
        private NetworkVariable<bool> m_P1Ready = new NetworkVariable<bool>(false);
        private NetworkVariable<bool> m_P2Ready = new NetworkVariable<bool>(false);
        private NetworkVariable<int> m_P1Choice = new NetworkVariable<int>(0);
        private NetworkVariable<int> m_P2Choice = new NetworkVariable<int>(0);
        private NetworkVariable<bool> m_ForcedSinglePlayer = new NetworkVariable<bool>(false);
        // FightingGameManager 클래스 내부에 추가
        private NetworkVariable<int> m_P1RoundWins = new NetworkVariable<int>(0);
        private NetworkVariable<int> m_P2RoundWins = new NetworkVariable<int>(0);
        private NetworkVariable<int> m_CurrentRound = new NetworkVariable<int>(1);

        // FightingGameManager.cs 내부

        [Header("Result UI")]
        [SerializeField]
        [Tooltip("캐릭터 프리팹 순서(0:Robot, 1:Knight 등)와 동일한 순서로 승리 이미지를 넣어주세요.")]
        private GameObject[] m_CharacterWinImages; // 캐릭터별 승리 이미지 배열

        [SerializeField]
        private GameObject m_RuReadyButton; // 최종 종료 후 나타날 버튼


        private List<GameObject> m_SpawnedFighters = new List<GameObject>();

        [SerializeField] NetworkTableTopManager m_TableManager;
        private Color m_OriginalStartButtonColor = Color.white;

        protected override void OnNetworkPreSpawn(ref NetworkManager networkManager)
        {
            m_GameState.Initialize(this);
            m_SelectionTimer.Initialize(this);
            m_GameTimer.Initialize(this);
            m_WinnerIndex.Initialize(this);
            m_P1PreReady.Initialize(this);
            m_P2PreReady.Initialize(this);
            m_P1Ready.Initialize(this);
            m_P2Ready.Initialize(this);
            m_P1Choice.Initialize(this);
            m_P2Choice.Initialize(this);
            m_ForcedSinglePlayer.Initialize(this);
            m_P1RoundWins.Initialize(this);
            m_P2RoundWins.Initialize(this);
            m_CurrentRound.Initialize(this);
            base.OnNetworkPreSpawn(ref networkManager);
        }

        void Start()
        {
            HideGameMode();
            m_GameState.OnValueChanged += OnGameStateChanged;
            
            if (m_TableManager == null)
                m_TableManager = FindFirstObjectByType<NetworkTableTopManager>();

            if (m_SingleModePopup != null) m_SingleModePopup.SetActive(false);
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                m_GameState.Value = FightingGameState.Idle;
                m_SelectionTimer.Value = 30f;
                m_P1PreReady.Value = false;
                m_P2PreReady.Value = false;
                m_P1Ready.Value = false;
                m_P2Ready.Value = false;
                m_ForcedSinglePlayer.Value = false;
            }

            m_P1PreReady.OnValueChanged += UpdateReadyVisuals;
            m_P2PreReady.OnValueChanged += UpdateReadyVisuals;

            m_P1Choice.OnValueChanged += UpdateSelectionVisuals;
            m_P2Choice.OnValueChanged += UpdateSelectionVisuals;
            m_P1Ready.OnValueChanged += UpdateSelectionVisuals;
            m_P2Ready.OnValueChanged += UpdateSelectionVisuals;
            m_WinnerIndex.OnValueChanged += OnWinnerChanged;

            UpdateUIState(m_GameState.Value);
            UpdateReadyVisuals();
            UpdateSelectionVisuals();
            if (m_WinnerIndex.Value != -1) OnWinnerChanged(-1, m_WinnerIndex.Value);

            // Late-join logic: If game is already fighting, initialize state
            if (m_GameState.Value == FightingGameState.Fighting)
            {
                OnGameModeStart();
                if (IsClient)
                {
                    StartCoroutine(LinkLocalCharacterDelayed());
                }
            }
        }

        public override void OnNetworkDespawn()
        {
            m_P1PreReady.OnValueChanged -= UpdateReadyVisuals;
            m_P2PreReady.OnValueChanged -= UpdateReadyVisuals;

            m_P1Choice.OnValueChanged -= UpdateSelectionVisuals;
            m_P2Choice.OnValueChanged -= UpdateSelectionVisuals;
            m_P1Ready.OnValueChanged -= UpdateSelectionVisuals;
            m_P2Ready.OnValueChanged -= UpdateSelectionVisuals;
            m_WinnerIndex.OnValueChanged -= OnWinnerChanged;

            base.OnNetworkDespawn();
        }

        private void OnWinnerChanged(int old, int current)
        {
            Debug.Log($"[FightingGameManager] OnWinnerChanged: {old} -> {current}");
            if (current == -1)
            {
                // 라운드 리셋 시 메시지 숨기기
                if (FightingHUDManager.Instance != null)
                    FightingHUDManager.Instance.winnerText.gameObject.SetActive(false);
                return;
            }

            string message = "";
            if (current == 0) message = "PLAYER 1 WINS!";
            else if (current == 1) message = "PLAYER 2 WINS!";
            else if (current == 2) message = "DRAW!";

            if (FightingHUDManager.Instance != null)
            {
                Debug.Log($"[FightingGameManager] Displaying winner message: {message}");
                FightingHUDManager.Instance.DisplayWinner(message);
            }
            else
            {
                Debug.LogWarning("[FightingGameManager] FightingHUDManager instance not found!");
            }

            // [추가] 경기 종료 시 승자/패자 캐릭터에게 win/lose 애니메이션 트리거를 전달합니다.
            TriggerWinLoseAnimations(current);
        }

        // [추가] 승리/패배 애니메이션 실행 헬퍼 함수
        private void TriggerWinLoseAnimations(int winnerIndex)
        {
            // 씬에 소환되어 있는 모든 파이터들을 검색
            var fighters = UnityEngine.Object.FindObjectsByType<FighterHealth>(FindObjectsSortMode.None);
            foreach (var f in fighters)
            {
                var animator = f.GetComponentInChildren<Animator>();
                if (animator == null) continue;

                int playerIdx = f.playerIndex.Value;

                if (winnerIndex == 0) // 1P 승리
                {
                    if (playerIdx == 0) animator.SetTrigger("win");
                    else if (playerIdx == 1) animator.SetTrigger("lose");
                }
                else if (winnerIndex == 1) // 2P 승리
                {
                    if (playerIdx == 1) animator.SetTrigger("win");
                    else if (playerIdx == 0) animator.SetTrigger("lose");
                }
                else if (winnerIndex == 2) // 무승부 (DRAW)
                {
                    // 무승부 시에는 양쪽 다 아쉬운 패배 포즈를 재생하도록 설정합니다.
                    animator.SetTrigger("lose");
                }
            }
        }

        private void OnGameStateChanged(FightingGameState old, FightingGameState current)
        {
            UpdateUIState(current);
            if (current == FightingGameState.Fighting) 
            {
                OnGameModeStart();
                // Client-side: Ensure the local character is linked to input
                if (IsClient)
                {
                    StartCoroutine(LinkLocalCharacterDelayed());
                }
            }
            else if (old == FightingGameState.Fighting) OnGameModeEnd();
        }

        private IEnumerator LinkLocalCharacterDelayed()
        {
            // Wait for characters to be spawned and ownership assigned
            float timeout = 2.0f;
            float elapsed = 0f;
            while (elapsed < timeout)
            {
                var characters = Object.FindObjectsByType<TableCharacter>(FindObjectsSortMode.None);
                foreach (var character in characters)
                {
                    if (character.IsOwner && !character.isDummy)
                    {
                        if (TableCharacterInput.Instance != null)
                        {
                            TableCharacterInput.Instance.SetCharacter(character);
                            Debug.Log($"[FightingGameManager] Manually linked owned character: {character.name}");
                            yield break;
                        }
                    }
                }
                elapsed += 0.2f;
                yield return new WaitForSeconds(0.2f);
            }
            Debug.LogWarning("[FightingGameManager] Failed to find owned character after timeout.");
        }

        private void UpdateUIState(FightingGameState state)
        {
            Debug.Log($"[FightingGameManager] UpdateUIState: {state}");

            if (m_StartButton != null) m_StartButton.SetActive(state == FightingGameState.Idle);
            if (m_CharacterSelectionUI != null) m_CharacterSelectionUI.SetActive(state == FightingGameState.Selecting);
            if (m_SingleModePopup != null) m_SingleModePopup.SetActive(false); // Hide popup on any state change
            
            if (m_InGameUI != null)
            {
                m_InGameUI.SetActive(state == FightingGameState.Fighting);
                // Ensure the UI is scaled up properly when active
                if (state == FightingGameState.Fighting)
                {
                    // Scale to 0.001 so 800px wide HUD = 0.8 meters
                    m_InGameUI.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
                    m_InGameUI.transform.localPosition = Vector3.zero;
                    
                    // Position the HUD child at 60cm world height (600 units at 0.001 scale)
                    Transform hud = m_InGameUI.transform.Find("FightingHUD");
                    if (hud != null)
                    {
                        hud.localPosition = new Vector3(0f, 600f, 0f);
                    }
                    
                    Debug.Log("[FightingGameManager] InGameUI Activated, Rescaled to 0.001, HUD positioned at height 600");
                }
            }

            if (m_PreGameUI != null)
            {
                // In Fighting state, hide the PreGameUI entirely to avoid duplicate timers/buttons
                m_PreGameUI.SetActive(state != FightingGameState.Fighting);
            }
        }

        void Update()
        {
            if (IsServer)
            {
                if (m_GameState.Value == FightingGameState.Idle)
                {
                    int seatedCount = 0;
                    bool allPreReady = true;
                    HashSet<ulong> uniquePlayers = new HashSet<ulong>();

                    if (m_TableManager != null)
                    {
                        for (int i = 0; i < m_TableManager.networkedSeats.Count; i++)
                        {
                            if (m_TableManager.networkedSeats[i].isOccupied)
                            {
                                seatedCount++;
                                uniquePlayers.Add(m_TableManager.networkedSeats[i].playerID);
                                bool isPreReady = (i == 0) ? m_P1PreReady.Value : (i == 1) ? m_P2PreReady.Value : false;
                                if (!isPreReady) allPreReady = false;
                            }
                        }
                    }

                    // Strict 2-player start (must be 2 different people) OR Forced Single Player
                    int uniqueCount = uniquePlayers.Count;
                    if ((uniqueCount == 2 && allPreReady) || (uniqueCount == 1 && allPreReady && m_ForcedSinglePlayer.Value))
                    {
                        StartSelecting();
                    }
                }
                else if (m_GameState.Value == FightingGameState.Selecting)
                {
                    m_SelectionTimer.Value -= Time.deltaTime;
                    
                    bool allSeatedPlayersReady = true;
                    int seatedCount = 0;

                    if (m_TableManager != null)
                    {
                        for (int i = 0; i < m_TableManager.networkedSeats.Count; i++)
                        {
                            if (m_TableManager.networkedSeats[i].isOccupied)
                            {
                                seatedCount++;
                                bool isReady = (i == 0) ? m_P1Ready.Value : (i == 1) ? m_P2Ready.Value : false;
                                if (!isReady) allSeatedPlayersReady = false;
                            }
                        }
                    }

                    // Transition if everyone is ready or time is up
                    if (seatedCount > 0 && (m_SelectionTimer.Value <= 0 || allSeatedPlayersReady))
                    {
                        m_GameState.Value = FightingGameState.Fighting;
                    }
                    else if (seatedCount == 0 && m_SelectionTimer.Value <= 0)
                    {
                        // No one seated but time is up? Reset.
                        m_GameState.Value = FightingGameState.Idle;
                    }
                }
                else if (m_GameState.Value == FightingGameState.Fighting)
                {
                    m_GameTimer.Value -= Time.deltaTime;
                    if (m_GameTimer.Value <= 0)
                    {
                        m_GameTimer.Value = 0;
                        HandleTimeout();
                    }
                }
            }

            if (m_GameState.Value == FightingGameState.Selecting && m_SelectionTimerText != null)
            {
                m_SelectionTimerText.text = Mathf.CeilToInt(m_SelectionTimer.Value).ToString();
            }

            if (m_GameState.Value == FightingGameState.Fighting && FightingHUDManager.Instance != null)
            {
                FightingHUDManager.Instance.gameTime = m_GameTimer.Value;
                FightingHUDManager.Instance.timerText.text = Mathf.CeilToInt(m_GameTimer.Value).ToString();
            }
        }

        private void HandleTimeout()
        {
            if (!IsServer) return;

            // Determine winner based on current health
            int winner = -1;
            float p1Health = -1f;
            float p2Health = -1f;

            var fighters = Object.FindObjectsByType<FighterHealth>(FindObjectsSortMode.None);
            foreach (var f in fighters)
            {
                if (f.playerIndex.Value == 0) p1Health = f.health.Value;
                else if (f.playerIndex.Value == 1) p2Health = f.health.Value;
            }

            if (p1Health > p2Health) winner = 0;
            else if (p2Health > p1Health) winner = 1;
            else winner = 2; // Draw

            EndGame(winner);
        }

        private void EndGame(int winnerIndex)
        {
            if (!IsServer) return;

            // [추가] 이미 라운드 승리 메시지가 떴다면(판정이 진행 중이라면) 무시
            if (m_WinnerIndex.Value != -1) return;

            if (winnerIndex == 0) m_P1RoundWins.Value++;
            else if (winnerIndex == 1) m_P2RoundWins.Value++;

            StartCoroutine(EndGameSequence(winnerIndex));
        }

        // 1. 서버에서 실행되는 게임 종료 시퀀스
        private IEnumerator EndGameSequence(int winnerIndex)
        {
            m_WinnerIndex.Value = winnerIndex; // HUD에 "PLAYER X WINS!" 표시 (NetworkVariable이라 자동 동기화됨)

            if (m_P1RoundWins.Value >= 2 || m_P2RoundWins.Value >= 2)
            {
                // === 최종 경기 종료 (2승 달성) ===
                int winningCharacterIdx = (winnerIndex == 0) ? m_P1Choice.Value : m_P2Choice.Value;

                // [핵심 수정] 서버에서만 켜지 않고, 모든 플레이어(Client)에게 결과 화면을 띄우라고 명령합니다.
                ShowMatchResultClientRpc(winningCharacterIdx);
            }
            else
            {
                // === 라운드 종료 (5초 후 다음 라운드) ===
                yield return new WaitForSeconds(5.0f);
                ResetRound();
            }
        }

        [Rpc(SendTo.Everyone)]
        private void ShowMatchResultClientRpc(int winningCharacterIdx)
        {
            // 코루틴을 통해 이미지 표시 후 8초 뒤 버튼 활성화
            StartCoroutine(MatchResultSequence(winningCharacterIdx));
        }

        private IEnumerator MatchResultSequence(int winningCharacterIdx)
        {
            // 승리 캐릭터 이미지 활성화
            if (winningCharacterIdx >= 0 && winningCharacterIdx < m_CharacterWinImages.Length)
            {
                if (m_CharacterWinImages[winningCharacterIdx] != null)
                    m_CharacterWinImages[winningCharacterIdx].SetActive(true);
            }

            // 8초 대기
            yield return new WaitForSeconds(8.0f);

            // 재시작 버튼 활성화
            if (m_RuReadyButton != null)
                m_RuReadyButton.SetActive(true);
        }


        private void ResetRound()
        {
            if (!IsServer) return;

            m_WinnerIndex.Value = -1; // 승리 메시지 숨기기
            m_CurrentRound.Value++;
            m_GameTimer.Value = 180f; // 타이머 초기화

            // 모든 캐릭터 체력 회복 및 위치 초기화
            var fighters = Object.FindObjectsByType<FighterHealth>(FindObjectsSortMode.None);
            foreach (var f in fighters)
            {
                f.health.Value = 200f; // 최대 체력으로 (FighterHealth의 m_MaxHealth 사용 권장)
                var animator = f.GetComponentInChildren<Animator>();
                if (animator != null) animator.Rebind();
                // 시작 위치로 텔레포트 (m_SpawnPoint1, 2 활용)
                int idx = f.playerIndex.Value;
                Transform startPoint = (idx == 0) ? m_SpawnPoint1 : m_SpawnPoint2;
                f.transform.position = startPoint.position;
                f.transform.rotation = startPoint.rotation;
            }
        }

        private void StartSelecting()
        {
            Debug.Log("[FightingGameManager] Server: Transitioning to Selecting state.");
            m_SelectionTimer.Value = 30f;
            m_P1PreReady.Value = false;
            m_P2PreReady.Value = false;
            m_P1Ready.Value = false;
            m_P2Ready.Value = false;
            m_P1Choice.Value = 0;
            m_P2Choice.Value = 0;
            m_WinnerIndex.Value = -1;
            m_GameState.Value = FightingGameState.Selecting;
        }

        public void ShowGameMode()
        {
            foreach (var obj in m_ObjectsToToggle) obj.SetActive(true);
            UpdateUIState(m_GameState.Value);
        }

        public void HideGameMode()
        {
            if (IsServer) 
            {
                m_GameState.Value = FightingGameState.Idle;
                m_P1PreReady.Value = false;
                m_P2PreReady.Value = false;
                m_ForcedSinglePlayer.Value = false;
            }
            foreach (var obj in m_ObjectsToToggle) obj.SetActive(false);
            if (m_PreGameUI != null) m_PreGameUI.SetActive(false);
            if (m_InGameUI != null) m_InGameUI.SetActive(false);
            if (m_SingleModePopup != null) m_SingleModePopup.SetActive(false);
            if (IsServer) CleanupFighters();
        }

        public void OnGameModeStart()
        {
            if (IsServer)
            {
                m_GameTimer.Value = 180f; // [추가] 새로운 경기가 시작될 때 서버 타이머를 180초로 리셋합니다!
                SpawnFighters();
            }
            OnSummoned?.Invoke();

            if (FightingHUDManager.Instance != null)
            {
                FightingHUDManager.Instance.StartTimer();
            }
        }

        public void OnGameModeEnd()
        {
            if (IsServer) CleanupFighters();

            if (FightingHUDManager.Instance != null)
            {
                FightingHUDManager.Instance.StopTimer();
            }
        }

        public void StartGameButtonPressed()
        {
            // Check seated count locally before sending RPC
            int seatedCount = 0;
            if (m_TableManager != null)
            {
                foreach (var seat in m_TableManager.networkedSeats)
                {
                    if (seat.isOccupied) seatedCount++;
                }
            }

            if (seatedCount == 1 && m_SingleModePopup != null)
            {
                m_SingleModePopup.SetActive(true);
            }
            else
            {
                TogglePreReadyRpc();
            }
        }

        public void ConfirmSinglePlayer()
        {
            ConfirmSinglePlayerRpc();
        }

        [Rpc(SendTo.Server)]
        void ConfirmSinglePlayerRpc(RpcParams rpcParams = default)
        {
            m_ForcedSinglePlayer.Value = true;
            // Also set pre-ready for the player who confirmed
            ulong clientId = rpcParams.Receive.SenderClientId;
            for (int i = 0; i < m_TableManager.networkedSeats.Count; i++)
            {
                if (m_TableManager.networkedSeats[i].isOccupied && m_TableManager.networkedSeats[i].playerID == clientId)
                {
                    if (i == 0) m_P1PreReady.Value = true;
                    else if (i == 1) m_P2PreReady.Value = true;
                    break;
                }
            }
        }

        [Rpc(SendTo.Server)]
        void TogglePreReadyRpc(RpcParams rpcParams = default)
        {
            ulong clientId = rpcParams.Receive.SenderClientId;
            if (m_TableManager != null)
            {
                for (int i = 0; i < m_TableManager.networkedSeats.Count; i++)
                {
                    if (m_TableManager.networkedSeats[i].isOccupied && m_TableManager.networkedSeats[i].playerID == clientId)
                    {
                        if (i == 0) m_P1PreReady.Value = !m_P1PreReady.Value;
                        else if (i == 1) m_P2PreReady.Value = !m_P2PreReady.Value;
                        break;
                    }
                }
            }
        }

        [Rpc(SendTo.Server)]
        public void RequestRestartMatchServerRpc()
        {
            // 1. 서버 상태 및 데이터 초기화
            m_GameState.Value = FightingGameState.Idle;
            m_P1PreReady.Value = false;
            m_P2PreReady.Value = false;
            m_P1RoundWins.Value = 0;
            m_P2RoundWins.Value = 0;
            m_P1Choice.Value = -1;
            m_P2Choice.Value = -1;
            m_P1Ready.Value = false;
            m_P2Ready.Value = false;
            m_WinnerIndex.Value = -1;

            // 2. 서버 측 UI 정리
            foreach (var img in m_CharacterWinImages) { if (img != null) img.SetActive(false); }
            if (m_RuReadyButton != null) m_RuReadyButton.SetActive(false);

            // 3. 캐릭터 모델 제거
            CleanupFighters();

            // 4. [핵심] 모든 클라이언트의 Pose Debugger 및 스킬 리셋 요청
            ResetClientUISkillsRpc();
        }

        [Rpc(SendTo.Everyone)]
        private void ResetClientUISkillsRpc()
        {
            // 기존 로직 (디버거 및 스킬 정리)
            var bridge = Object.FindFirstObjectByType<CharacterSkillBridge>();
            if (bridge != null) bridge.OnCharacterChanged(null);

            MonoBehaviour[] allComponents = Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
            foreach (var comp in allComponents)
            {
                if (comp != null && comp.GetType().Name == "MRSkillManager")
                {
                    comp.GetType().GetMethod("DisableSkills")?.Invoke(comp, null);
                    break;
                }
            }

            // [추가] 결과 화면 UI 숨기기 (모든 플레이어 화면에서)
            foreach (var img in m_CharacterWinImages) { if (img != null) img.SetActive(false); }
            if (m_RuReadyButton != null) m_RuReadyButton.SetActive(false);
        }

        [Rpc(SendTo.Server)]
        public void RequestHideGameModeServerRpc()
        {
            // 이 코드는 클라이언트가 호출해도 실제로는 '서버'에서 실행됩니다.
            HideGameMode();
        }

        public void SelectCharacter(int slotIndex)
        {
            SelectCharacterRpc(slotIndex);
        }

        private void UpdateReadyVisuals(bool previousValue, bool newValue) => UpdateReadyVisuals();

        private void UpdateReadyVisuals()
        {
            if (m_TableManager == null || m_StartButton == null) return;

            int mySeatIndex = -1;
            for (int i = 0; i < m_TableManager.networkedSeats.Count; i++)
            {
                if (m_TableManager.networkedSeats[i].isOccupied && m_TableManager.networkedSeats[i].playerID == NetworkManager.LocalClientId)
                {
                    mySeatIndex = i;
                    break;
                }
            }

            bool isReady = false;
            if (mySeatIndex == 0) isReady = m_P1PreReady.Value;
            else if (mySeatIndex == 1) isReady = m_P2PreReady.Value;

            Image btnImg = m_StartButton.GetComponent<Image>();
            if (btnImg != null)
            {
                btnImg.color = isReady ? Color.green : Color.white;
            }
        }

        private void UpdateSelectionVisuals(int previousValue, int newValue) => UpdateSelectionVisuals();
        private void UpdateSelectionVisuals(bool previousValue, bool newValue) => UpdateSelectionVisuals();

        private void UpdateSelectionVisuals()
        {
            if (m_CharacterSelectionUI == null) return;
            Transform grid = m_CharacterSelectionUI.transform.Find("Grid");
            if (grid == null) return;

            for (int i = 0; i < grid.childCount; i++)
            {
                Transform slot = grid.GetChild(i);
                Transform outline = slot.Find("Outline");
                if (outline != null)
                {
                    bool p1Selected = (i == m_P1Choice.Value && m_P1Ready.Value);
                    bool p2Selected = (i == m_P2Choice.Value && m_P2Ready.Value);
                    
                    if (p1Selected && p2Selected)
                    {
                        outline.gameObject.SetActive(true);
                        var img = outline.GetComponent<Image>();
                        if (img != null) img.color = m_BothSelectColor;
                    }
                    else if (p1Selected)
                    {
                        outline.gameObject.SetActive(true);
                        var img = outline.GetComponent<Image>();
                        if (img != null) img.color = m_P1SelectColor;
                    }
                    else if (p2Selected)
                    {
                        outline.gameObject.SetActive(true);
                        var img = outline.GetComponent<Image>();
                        if (img != null) img.color = m_P2SelectColor;
                    }
                    else
                    {
                        outline.gameObject.SetActive(false);
                    }
                }
            }
        }

        [Rpc(SendTo.Server)]
void SelectCharacterRpc(int slotIndex, RpcParams rpcParams = default)
        {
            ulong clientId = rpcParams.Receive.SenderClientId;
            if (m_TableManager != null)
            {
                for (int i = 0; i < m_TableManager.networkedSeats.Count; i++)
                {
                    if (m_TableManager.networkedSeats[i].isOccupied && m_TableManager.networkedSeats[i].playerID == clientId)
                    {
                        if (i == 0)
                        {
                            m_P1Choice.Value = slotIndex;
                            m_P1Ready.Value = true;
                        }
                        else if (i == 1)
                        {
                            m_P2Choice.Value = slotIndex;
                            m_P2Ready.Value = true;
                        }
                        break;
                    }
                }
            }
        }

        void SpawnFighters()
        {
            CleanupFighters();

            if (m_TableManager == null)
            {
                Debug.LogError("[FightingGameManager] Table Manager is null! Cannot spawn fighters.");
                return;
            }

            Debug.Log($"[FightingGameManager] Starting SpawnFighters. Current Seated Players: {m_TableManager.networkedSeats.Count}");

            Transform[] spawnPoints = { m_SpawnPoint1, m_SpawnPoint2, m_SpawnPoint3, m_SpawnPoint4 };
            int playerCounter = 0;
            
            bool p1Occupied = false;
            bool p2Occupied = false;

            for (int i = 0; i < m_TableManager.networkedSeats.Count; i++)
            {
                var seat = m_TableManager.networkedSeats[i];
                if (seat.isOccupied)
                {
                    if (i == 0) p1Occupied = true;
                    if (i == 1) p2Occupied = true;

                    if (i < spawnPoints.Length && spawnPoints[i] != null)
                    {
                        int choice = (i == 0) ? m_P1Choice.Value : (i == 1) ? m_P2Choice.Value : -1;
                        
                        // ONLY spawn if a valid character index was chosen.
                        if (m_CharacterPrefabs != null && choice >= 0 && choice < m_CharacterPrefabs.Length && m_CharacterPrefabs[choice] != null) 
                        {
                            GameObject prefab = m_CharacterPrefabs[choice];
                            Debug.Log($"[FightingGameManager] Spawning SEAT {i} for PlayerID {seat.playerID}. Prefab: {prefab.name}");
                            
                            var f = Instantiate(prefab, spawnPoints[i].position, spawnPoints[i].rotation, transform);
                            var fighterHealth = f.GetComponent<FighterHealth>();
                            if (fighterHealth != null)
                            {
                                fighterHealth.playerIndex.Value = playerCounter;
                                playerCounter++;
                            }
                            
                            var networkObject = f.GetComponent<NetworkObject>();
                            if (networkObject != null)
                            {
                                // Explicitly spawn with the ClientID assigned to this seat
                                networkObject.SpawnWithOwnership(seat.playerID);
                                m_SpawnedFighters.Add(f);
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"[FightingGameManager] Seat {i} is occupied but no valid character was selected (Choice: {choice}). Skipping spawn.");
                        }
                    }
                }
                else
                {
                    Debug.Log($"[FightingGameManager] Seat {i} is not occupied.");
                }
            }

            // Practice Mode Logic: If only one player is seated, spawn the other character as a dummy
            if (playerCounter == 1 && m_CharacterPrefabs != null && m_CharacterPrefabs.Length >= 2)
            {
                int dummySeatIndex = p1Occupied ? 1 : 0;
                int playerChoice = p1Occupied ? m_P1Choice.Value : m_P2Choice.Value;
                int dummyChoice = (playerChoice == 0) ? 1 : 0; // Select the character the player didn't pick

                if (dummySeatIndex < spawnPoints.Length && spawnPoints[dummySeatIndex] != null)
                {
                    GameObject prefab = m_CharacterPrefabs[dummyChoice];
                    Debug.Log($"[FightingGameManager] Spawning DUMMY at Seat {dummySeatIndex} (Choice: {dummyChoice}). Prefab: {prefab.name}");
                    
                    var f = Instantiate(prefab, spawnPoints[dummySeatIndex].position, spawnPoints[dummySeatIndex].rotation, transform);
                    
                    var tableChar = f.GetComponent<UnityEngine.XR.Templates.MRTTabletopAssets.TableCharacter>();
                    if (tableChar != null)
                    {
                        tableChar.isDummy = true;
                    }

                    var fighterHealth = f.GetComponent<FighterHealth>();
                    if (fighterHealth != null)
                    {
                        fighterHealth.playerIndex.Value = playerCounter; // Assign as P2 (index 1) for the HUD
                        playerCounter++;
                    }
                    
                    var networkObject = f.GetComponent<NetworkObject>();
                    if (networkObject != null)
                    {
                        networkObject.Spawn(); // Spawn as server-owned object
                        m_SpawnedFighters.Add(f);
                    }
                }
            }
            
            if (m_SpawnedFighters.Count == 0)
            {
                Debug.LogWarning("[FightingGameManager] No fighters were spawned! Check seat occupancy data.");
            }
            else
            {
                Debug.Log($"[Test] FightingGameManager: Fighters spawned. (Count: {m_SpawnedFighters.Count}).");
            }
        }

        void CleanupFighters()
        {
            foreach (var f in m_SpawnedFighters)
            {
                if (f != null && f.GetComponent<NetworkObject>().IsSpawned)
                    f.GetComponent<NetworkObject>().Despawn();
            }
            m_SpawnedFighters.Clear();
        }

        public void FighterDied()
        {
            if (IsServer && m_GameState.Value == FightingGameState.Fighting)
            {
                var fighters = Object.FindObjectsByType<FighterHealth>(FindObjectsSortMode.None);
                int winnerIndex = 2;
                float maxHealth = -1f;
                bool isTie = false;

                foreach (var f in fighters)
                {
                    if (f.health.Value > maxHealth)
                    {
                        maxHealth = f.health.Value;
                        winnerIndex = f.playerIndex.Value;
                        isTie = false;
                    }
                    else if (Mathf.Approximately(f.health.Value, maxHealth))
                    {
                        isTie = true;
                    }
                }

                if (isTie || maxHealth <= 0)
                {
                    winnerIndex = 2;
                }

                EndGame(winnerIndex);
            }
        }
    }
}

using System.Collections;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using XRMultiplayer;

namespace OVRMultiplayer
{
    /// <summary>
    /// Multiplayer lobby UI for OVRMultiTest scene (2 players).
    /// Placed on a World Space Canvas, uses XRINetworkGameManager API.
    /// </summary>
    public class OVRMultiplayerLobbyUI : MonoBehaviour
    {
        const int MAX_PLAYERS = 2;

        [Header("Panels")]
        [SerializeField] GameObject m_PanelAuthenticating;
        [SerializeField] GameObject m_PanelMainMenu;
        [SerializeField] GameObject m_PanelCreateRoom;
        [SerializeField] GameObject m_PanelRoomList;
        [SerializeField] GameObject m_PanelEnterCode;
        [SerializeField] GameObject m_PanelConnecting;
        [SerializeField] GameObject m_PanelConnected;
        [SerializeField] GameObject m_PanelError;

        [Header("Create Room")]
        [SerializeField] TMP_InputField m_RoomNameInput;

        [Header("Enter Code")]
        [SerializeField] TMP_InputField m_RoomCodeInput;

        [Header("Room List")]
        [SerializeField] Transform m_RoomListContent;
        [SerializeField] GameObject m_RoomSlotPrefab;
        [SerializeField] float m_RefreshInterval = 5f;

        [Header("Status Texts")]
        [SerializeField] TMP_Text m_ConnectingStatusText;
        [SerializeField] TMP_Text m_ErrorText;
        [SerializeField] TMP_Text m_ConnectedRoomText;
        [SerializeField] TMP_Text m_ConnectedPlayersText;
        [SerializeField] TMP_Text m_RoomCodeDisplayText;

        GameObject[] m_AllPanels;
        Coroutine m_RefreshRoutine;

        void Awake()
        {
            m_AllPanels = new[]
            {
                m_PanelAuthenticating,
                m_PanelMainMenu,
                m_PanelCreateRoom,
                m_PanelRoomList,
                m_PanelEnterCode,
                m_PanelConnecting,
                m_PanelConnected,
                m_PanelError
            };
        }

        void Start()
        {
            XRINetworkGameManager.CurrentConnectionState.Subscribe(OnConnectionStateChanged);
            XRINetworkGameManager.Connected.Subscribe(OnConnectedChanged);
            LobbyManager.status.Subscribe(OnStatusUpdated);

            if (XRINetworkGameManager.Instance != null)
            {
                XRINetworkGameManager.Instance.connectionFailedAction += OnConnectionFailed;
                XRINetworkGameManager.Instance.playerStateChanged += OnPlayerStateChanged;
            }

            UpdatePanelForCurrentState();
        }

        void OnDestroy()
        {
            XRINetworkGameManager.CurrentConnectionState.Unsubscribe(OnConnectionStateChanged);
            XRINetworkGameManager.Connected.Unsubscribe(OnConnectedChanged);
            LobbyManager.status.Unsubscribe(OnStatusUpdated);

            if (XRINetworkGameManager.Instance != null)
            {
                XRINetworkGameManager.Instance.connectionFailedAction -= OnConnectionFailed;
                XRINetworkGameManager.Instance.playerStateChanged -= OnPlayerStateChanged;
            }
        }

        // ─── Panel Navigation ───────────────────────────────────────

        void ShowPanel(GameObject panel)
        {
            foreach (var p in m_AllPanels)
            {
                if (p != null) p.SetActive(p == panel);
            }

            if (panel == m_PanelRoomList)
                StartRefreshing();
            else
                StopRefreshing();
        }

        void UpdatePanelForCurrentState()
        {
            if (XRINetworkGameManager.Connected.Value)
            {
                ShowPanel(m_PanelConnected);
                UpdateConnectedInfo();
                return;
            }

            var state = XRINetworkGameManager.CurrentConnectionState.Value;
            switch (state)
            {
                case XRINetworkGameManager.ConnectionState.None:
                case XRINetworkGameManager.ConnectionState.Authenticating:
                    ShowPanel(m_PanelAuthenticating);
                    break;
                case XRINetworkGameManager.ConnectionState.Authenticated:
                    ShowPanel(m_PanelMainMenu);
                    break;
                case XRINetworkGameManager.ConnectionState.Connecting:
                    ShowPanel(m_PanelConnecting);
                    break;
                case XRINetworkGameManager.ConnectionState.Connected:
                    ShowPanel(m_PanelConnected);
                    UpdateConnectedInfo();
                    break;
            }
        }

        // ─── Button Callbacks ────────────────────────────────────────

        /// <summary>Main Menu -> Create Room panel</summary>
        public void OnCreateRoomButton()
        {
            ShowPanel(m_PanelCreateRoom);
            if (m_RoomNameInput != null)
                m_RoomNameInput.text = "";
        }

        /// <summary>Main Menu -> Room List panel</summary>
        public void OnBrowseRoomsButton()
        {
            ShowPanel(m_PanelRoomList);
        }

        /// <summary>Main Menu -> Enter Code panel</summary>
        public void OnEnterCodeButton()
        {
            ShowPanel(m_PanelEnterCode);
            if (m_RoomCodeInput != null)
                m_RoomCodeInput.text = "";
        }

        /// <summary>Main Menu -> Quick Join</summary>
        public void OnQuickJoinButton()
        {
            ShowPanel(m_PanelConnecting);
            if (m_ConnectingStatusText != null)
                m_ConnectingStatusText.text = "Quick joining...";
            XRINetworkGameManager.Instance.QuickJoinLobby();
        }

        /// <summary>Create Room panel -> Confirm</summary>
        public void OnConfirmCreateRoom()
        {
            string roomName = m_RoomNameInput != null ? m_RoomNameInput.text : "";
            if (string.IsNullOrEmpty(roomName))
                roomName = $"{XRINetworkGameManager.LocalPlayerName.Value}'s Room";

            ShowPanel(m_PanelConnecting);
            if (m_ConnectingStatusText != null)
                m_ConnectingStatusText.text = $"Creating room: {roomName}";

            XRINetworkGameManager.Instance.CreateNewLobby(roomName, false, MAX_PLAYERS);
        }

        /// <summary>Enter Code panel -> Confirm</summary>
        public void OnConfirmEnterCode()
        {
            if (m_RoomCodeInput == null) return;
            string code = m_RoomCodeInput.text.Trim().ToUpper();
            if (code.Length < 5)
            {
                ShowError("Room code is too short. (min 5 chars)");
                return;
            }

            ShowPanel(m_PanelConnecting);
            if (m_ConnectingStatusText != null)
                m_ConnectingStatusText.text = $"Joining by code: {code}";

            XRINetworkGameManager.Instance.JoinLobbyByCode(code);
        }

        /// <summary>Join a specific lobby from the room list</summary>
        public void JoinLobby(Lobby lobby)
        {
            ShowPanel(m_PanelConnecting);
            if (m_ConnectingStatusText != null)
                m_ConnectingStatusText.text = $"Joining: {lobby.Name}";

            XRINetworkGameManager.Instance.JoinLobbySpecific(lobby);
        }

        /// <summary>Cancel connecting</summary>
        public void OnCancelButton()
        {
            XRINetworkGameManager.Instance.CancelMatchmaking();
            ShowPanel(m_PanelMainMenu);
        }

        /// <summary>Disconnect from room</summary>
        public void OnDisconnectButton()
        {
            XRINetworkGameManager.Instance.Disconnect();
            ShowPanel(m_PanelMainMenu);
        }

        /// <summary>Error -> Back to main menu</summary>
        public void OnBackToMainMenu()
        {
            ShowPanel(m_PanelMainMenu);
        }

        /// <summary>Refresh room list</summary>
        public void OnRefreshRoomList()
        {
            RefreshLobbies();
        }

        // ─── Event Handlers ─────────────────────────────────────────

        void OnConnectionStateChanged(XRINetworkGameManager.ConnectionState state)
        {
            UpdatePanelForCurrentState();
        }

        void OnConnectedChanged(bool connected)
        {
            if (connected)
            {
                ShowPanel(m_PanelConnected);
                UpdateConnectedInfo();
            }
        }

        void OnStatusUpdated(string status)
        {
            if (m_ConnectingStatusText != null)
                m_ConnectingStatusText.text = status;
        }

        void OnConnectionFailed(string reason)
        {
            ShowError(reason);
        }

        void OnPlayerStateChanged(ulong playerId, bool joined)
        {
            if (XRINetworkGameManager.Connected.Value)
                UpdateConnectedInfo();
        }

        // ─── Room List ──────────────────────────────────────────────

        void StartRefreshing()
        {
            StopRefreshing();
            RefreshLobbies();
            m_RefreshRoutine = StartCoroutine(AutoRefreshRoutine());
        }

        void StopRefreshing()
        {
            if (m_RefreshRoutine != null)
            {
                StopCoroutine(m_RefreshRoutine);
                m_RefreshRoutine = null;
            }
        }

        IEnumerator AutoRefreshRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(m_RefreshInterval);
                RefreshLobbies();
            }
        }

        async void RefreshLobbies()
        {
            if ((int)XRINetworkGameManager.CurrentConnectionState.Value < 2) return;

            if (m_RoomListContent != null)
            {
                foreach (Transform child in m_RoomListContent)
                    Destroy(child.gameObject);
            }

            try
            {
                QueryResponse response = await LobbyManager.GetLobbiesAsync();
                if (response.Results == null) return;

                foreach (var lobby in response.Results)
                {
                    if (LobbyManager.CheckForLobbyFilter(lobby)) continue;
                    if (LobbyManager.CheckForIncompatibilityFilter(lobby)) continue;
                    if (!LobbyManager.CanJoinLobby(lobby)) continue;

                    if (m_RoomSlotPrefab != null && m_RoomListContent != null)
                    {
                        var slotGo = Instantiate(m_RoomSlotPrefab, m_RoomListContent);
                        var slot = slotGo.GetComponent<OVRLobbySlotUI>();
                        if (slot != null)
                        {
                            slot.Setup(lobby, this);
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[OVRMultiplayerLobbyUI] Failed to refresh lobby list: {e.Message}");
            }
        }

        // ─── Helpers ─────────────────────────────────────────────────

        void UpdateConnectedInfo()
        {
            if (m_ConnectedRoomText != null)
                m_ConnectedRoomText.text = $"Room: {XRINetworkGameManager.ConnectedRoomName.Value}";

            if (m_RoomCodeDisplayText != null)
                m_RoomCodeDisplayText.text = $"Code: {XRINetworkGameManager.ConnectedRoomCode}";

            if (m_ConnectedPlayersText != null)
            {
                int count = Unity.Netcode.NetworkManager.Singleton != null
                    ? Unity.Netcode.NetworkManager.Singleton.ConnectedClientsIds.Count
                    : 0;
                m_ConnectedPlayersText.text = $"Players: {count} / {MAX_PLAYERS}";
            }
        }

        void ShowError(string message)
        {
            ShowPanel(m_PanelError);
            if (m_ErrorText != null)
                m_ErrorText.text = message;
        }
    }
}

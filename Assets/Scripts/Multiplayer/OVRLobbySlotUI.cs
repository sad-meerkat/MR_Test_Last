using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace OVRMultiplayer
{
    /// <summary>
    /// 방 목록에서 개별 로비를 표시하는 UI 슬롯.
    /// OVRMultiplayerLobbyUI의 m_RoomSlotPrefab으로 사용됩니다.
    /// </summary>
    public class OVRLobbySlotUI : MonoBehaviour
    {
        [SerializeField] TMP_Text m_RoomNameText;
        [SerializeField] TMP_Text m_PlayerCountText;
        [SerializeField] Button m_JoinButton;

        Lobby m_Lobby;
        OVRMultiplayerLobbyUI m_LobbyUI;

        public void Setup(Lobby lobby, OVRMultiplayerLobbyUI lobbyUI)
        {
            m_Lobby = lobby;
            m_LobbyUI = lobbyUI;

            if (m_RoomNameText != null)
                m_RoomNameText.text = lobby.Name;

            if (m_PlayerCountText != null)
                m_PlayerCountText.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";

            bool canJoin = lobby.Players.Count < lobby.MaxPlayers;

            if (m_JoinButton != null)
            {
                m_JoinButton.interactable = canJoin;
                m_JoinButton.onClick.AddListener(OnJoinClicked);
            }
        }

        void OnJoinClicked()
        {
            if (m_LobbyUI != null && m_Lobby != null)
                m_LobbyUI.JoinLobby(m_Lobby);
        }

        void OnDestroy()
        {
            if (m_JoinButton != null)
                m_JoinButton.onClick.RemoveListener(OnJoinClicked);
        }
    }
}

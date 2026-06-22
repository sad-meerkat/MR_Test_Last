using System;
using Unity.Netcode;
using XRMultiplayer;

namespace UnityEngine.XR.Templates.MRTTabletopAssets
{
    public class NetworkTableTopManager : NetworkBehaviour
    {
        public NetworkList<NetworkedSeat> networkedSeats;

        [SerializeField]
        TableSeatSystem m_SeatSystem;

        [SerializeField]
        TableTop m_TableTop;

        [SerializeField]
        TableTopSeatButton[] m_SeatButtons;

        void Awake()
        {
            networkedSeats = new NetworkList<NetworkedSeat>();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsServer)
            {
                networkedSeats.Clear();
                for (int i = 0; i < m_SeatButtons.Length; i++)
                {
                    networkedSeats.Add(new NetworkedSeat { isOccupied = false, playerID = 0 });
                }
            }

            UpdateNetworkedSeatsVisuals();
            networkedSeats.OnListChanged += OnOccupiedSeatsChanged;
            RequestAnySeatFromHost();

            if (IsServer)
            {
                XRINetworkGameManager.Instance.playerStateChanged += OnPlayerStateChanged;
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            foreach (var seatButton in m_SeatButtons)
            {
                if (seatButton != null)
                    seatButton.RemovePlayerFromSeat();
            }
            networkedSeats.OnListChanged -= OnOccupiedSeatsChanged;
            XRINetworkGameManager.Instance.playerStateChanged -= OnPlayerStateChanged;
            m_SeatSystem.TeleportToSeat(0);
            TableTop.k_CurrentSeat = -2;
        }

        private void OnOccupiedSeatsChanged(NetworkListEvent<NetworkedSeat> changeEvent)
        {
            UpdateNetworkedSeatsVisuals();
        }

        // NetworkTableTopManager.cs의 66행 부근 OnPlayerStateChanged 메서드를 아래 내용으로 수정

        void OnPlayerStateChanged(ulong playerID, bool connected)
        {
            if (!connected)
            {
                for (int i = 0; i < networkedSeats.Count; i++)
                {
                    if (networkedSeats[i].playerID == playerID)
                    {
                        if (i < m_SeatButtons.Length && m_SeatButtons[i] != null)
                            ServerRemoveSeat(i);
                    }
                }

                UpdateNetworkedSeatsVisuals();
            }
            else // [추가] 플레이어가 완전하게 접속했을 때 자동으로 빈자리 배정!
            {
                bool alreadyHasSeat = false;
                for (int i = 0; i < networkedSeats.Count; i++)
                {
                    if (networkedSeats[i].isOccupied && networkedSeats[i].playerID == playerID)
                    {
                        alreadyHasSeat = true;
                        break;
                    }
                }

                if (!alreadyHasSeat)
                {
                    int availableSeat = GetAnyAvailableSeats();
                    if (availableSeat >= 0)
                    {
                        ServerAssignSeat(-1, availableSeat, playerID);
                        Debug.Log($"[NetworkTableTopManager] Auto-assigned Seat {availableSeat} to Player {playerID} on connect.");
                    }
                }
            }
        }

        // 약 106번 줄 근처
        void UpdateNetworkedSeatsVisuals()
        {
            for (int i = 0; i < networkedSeats.Count; i++)
            {
                // 1. 버튼이 할당되지 않은 칸은 건너뜁니다 (Null 에러 방지)
                if (i >= m_SeatButtons.Length || m_SeatButtons[i] == null) continue;

                if (!networkedSeats[i].isOccupied)
                {
                    m_SeatButtons[i].SetOccupied(false);
                }
                else
                {
                    // 2. 플레이어 정보를 즉시 확인
                    if (XRINetworkGameManager.Instance.TryGetPlayerByID(networkedSeats[i].playerID, out var player))
                    {
                        m_SeatButtons[i].AssignPlayerToSeat(player);
                    }
                    else
                    {
                        // 3. 정보를 못 찾았다면, 찾을 때까지 배경에서 재시도 (중요!)
                        Debug.LogWarning($"[NetworkTableTopManager] Player {networkedSeats[i].playerID} not found yet for seat {i}. Retrying...");
                        StartCoroutine(AssignSeatWithRetryDelayed(i, networkedSeats[i].playerID));
                    }
                }
            }
        }

        public void RequestAnySeatFromHost()
        {
            RequestSeatServerRpc(NetworkManager.Singleton.LocalClientId, TableTop.k_CurrentSeat);
        }

        public void RequestSeat(int newSeatChoice)
        {
            RequestSeatServerRpc(NetworkManager.Singleton.LocalClientId, TableTop.k_CurrentSeat, newSeatChoice);
        }

        [Rpc(SendTo.Server)]
        void RequestSeatServerRpc(ulong localPlayerID, int currentSeatID, int newSeatID = -2)
        {
            if (newSeatID <= -2)    // Request any available seat
                newSeatID = GetAnyAvailableSeats();

            // First, check if this player is already in any seat and remove them
            for (int i = 0; i < networkedSeats.Count; i++)
            {
                if (networkedSeats[i].isOccupied && networkedSeats[i].playerID == localPlayerID)
                {
                    ServerRemoveSeat(i);
                }
            }

            if (newSeatID >= 0 && !IsSeatOccupied(newSeatID))
                ServerAssignSeat(-1, newSeatID, localPlayerID); // currentSeatID is handled above
            else if (newSeatID >= 0)
                Debug.Log("User tried to join an occupied seat");
        }

        int GetAnyAvailableSeats()
        {
            int availableSeat = -1;
            for (int i = 0; i < networkedSeats.Count; i++)
            {
                if (!networkedSeats[i].isOccupied)
                {
                    availableSeat = i;
                    return availableSeat;
                }
            }

            return availableSeat;
        }

        bool IsSeatOccupied(int seatID)
        {
            return seatID >= 0 && networkedSeats[seatID].isOccupied;
        }

        void ServerAssignSeat(int currentSeatID, int newSeatID, ulong localPlayerID)
        {
            if (currentSeatID >= 0)
            {
                ServerRemoveSeat(currentSeatID);
            }
            if (newSeatID >= 0)
            {
                networkedSeats[newSeatID] = new NetworkedSeat { isOccupied = true, playerID = localPlayerID };
            }

            UpdateNetworkedSeatsVisuals();

            AssignSeatRpc(newSeatID, localPlayerID);
        }

        void ServerRemoveSeat(int seatID)
        {
            networkedSeats[seatID] = new NetworkedSeat { isOccupied = false, playerID = 0 };
            UpdateNetworkedSeatsVisuals();
            RemovePlayerFromSeatRpc(seatID);
        }

        [Rpc(SendTo.Everyone)]
        void RemovePlayerFromSeatRpc(int seatID)
        {
            m_SeatButtons[seatID].RemovePlayerFromSeat();
        }

        [Rpc(SendTo.Everyone)]
        // NetworkTableTopManager.cs의 184행 부근 AssignSeatRpc 메서드와 그 하위에 코루틴을 아래 코드로 교체/추가해 줍니다.
        void AssignSeatRpc(int seatID, ulong playerID)
        {
            if (XRINetworkGameManager.Instance.TryGetPlayerByID(playerID, out var player))
            {
                m_SeatButtons[seatID].AssignPlayerToSeat(player);
                if (playerID == NetworkManager.Singleton.LocalClientId)
                {
                    m_SeatSystem.TeleportToSeat(seatID);
                }
            }
            else
            {
                Debug.LogWarning($"[NetworkTableTopManager] Player with id {playerID} not found yet. Retrying in background...");
                // [추가] 동기화 지연 대비 백그라운드 재시도 시작
                StartCoroutine(AssignSeatWithRetryDelayed(seatID, playerID));
            }
        }

        // [추가] 플레이어 정보가 동기화 완료될 때까지 안전하게 대기 후 배치해 주는 코루틴
        private System.Collections.IEnumerator AssignSeatWithRetryDelayed(int seatID, ulong playerID)
        {
            float timeout = 5f; // 최대 5초 대기
            float elapsed = 0f;
            while (elapsed < timeout)
            {
                yield return new WaitForSeconds(0.2f);
                elapsed += 0.2f;

                if (XRINetworkGameManager.Instance.TryGetPlayerByID(playerID, out var player))
                {
                    m_SeatButtons[seatID].AssignPlayerToSeat(player);
                    if (playerID == NetworkManager.Singleton.LocalClientId)
                    {
                        m_SeatSystem.TeleportToSeat(seatID);
                    }
                    Debug.Log($"[NetworkTableTopManager] Successfully auto-assigned Seat {seatID} to Player {playerID} after retry.");
                    yield break;
                }
            }
            Debug.LogError($"[NetworkTableTopManager] Retry timed out. Player with id {playerID} not found.");
        }

        public void TeleportToSpectatorSeat()
        {
            RequestSeat(-1);
        }
    }

    [Serializable]
    public struct NetworkedSeat : INetworkSerializable, IEquatable<NetworkedSeat>
    {
        public bool isOccupied;
        public ulong playerID;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref isOccupied);
            serializer.SerializeValue(ref playerID);
        }

        public readonly bool Equals(NetworkedSeat other)
        {
            return isOccupied == other.isOccupied && playerID == other.playerID;
        }
    }
}

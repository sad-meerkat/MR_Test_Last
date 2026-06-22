using UnityEngine;
using UnityEngine.UI;
using TMPro;
using XRMultiplayer;
//using Oculus.Interaction;

namespace OVRMultiplayer
{
    /// <summary>
    /// Place on an empty GameObject in OVRMultiTest scene to auto-configure multiplayer.
    ///
    /// Usage:
    /// 1. Add this component to an empty GameObject
    /// 2. Assign networkManagerPrefab, gameManagerPrefab in Inspector
    ///    (from Assets/XRMP/Prefabs/Managers/)
    /// 3. Play -> Lobby UI appears after authentication
    ///
    /// Lobby UI is generated at runtime via code.
    /// Assign lobbyUIPrefab to use a custom prefab instead.
    /// </summary>
    public class OVRMultiplayerBootstrap : MonoBehaviour
    {
        [Header("XRMP Prefabs (Assets/XRMP/Prefabs/Managers/)")]
        [Tooltip("Network Manager XR Multiplayer prefab")]
        [SerializeField] GameObject m_NetworkManagerPrefab;

        [Tooltip("XRI_Network_Game_Manager prefab")]
        [SerializeField] GameObject m_GameManagerPrefab;

        [Header("Optional: Custom Lobby UI Prefab")]
        [Tooltip("Leave empty to auto-generate at runtime")]
        [SerializeField] GameObject m_LobbyUIPrefab;

        [Header("UI Settings")]
        [Tooltip("Lobby UI spawn position (defaults to in front of camera)")]
        [SerializeField] Transform m_UISpawnPoint;

        [Tooltip("UI canvas scale (World Space)")]
        [SerializeField] float m_CanvasScale = 0.001f;

        OVRMultiplayerLobbyUI m_LobbyUI;
        TMP_FontAsset m_Font;

        void Awake()
        {
            EnsureNetworkManagers();
            EnsurePlayerHudNotification();
            EnsureOVRNetworkPlayerSetup();
        }

        void Start()
        {
            if (m_LobbyUIPrefab != null)
            {
                var uiInstance = Instantiate(m_LobbyUIPrefab);
                PositionUI(uiInstance.transform);
                MakeCanvasInteractable(uiInstance);
            }
            else
            {
                CreateLobbyUI();
            }
        }

        void PositionUI(Transform uiTransform)
        {
            if (m_UISpawnPoint != null)
            {
                uiTransform.position = m_UISpawnPoint.position;
                uiTransform.rotation = m_UISpawnPoint.rotation;
            }
            else
            {
                var cam = Camera.main;
                if (cam != null)
                {
                    uiTransform.position = cam.transform.position + cam.transform.forward * 1.5f;
                    uiTransform.rotation = Quaternion.LookRotation(
                        uiTransform.position - cam.transform.position);
                }
            }
        }

        void EnsureNetworkManagers()
        {
            if (Unity.Netcode.NetworkManager.Singleton == null && m_NetworkManagerPrefab != null)
            {
                var nm = Instantiate(m_NetworkManagerPrefab);
                nm.name = "Network Manager XR Multiplayer";
                DontDestroyOnLoad(nm);
            }

            if (XRINetworkGameManager.Instance == null && m_GameManagerPrefab != null)
            {
                var gm = Instantiate(m_GameManagerPrefab);
                gm.name = "XRI_Network_Game_Manager";
            }
        }

        void EnsurePlayerHudNotification()
        {
            if (PlayerHudNotification.Instance != null) return;

            var hudGo = new GameObject("PlayerHudNotification");
            var canvas = hudGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingOrder = 100;
            hudGo.AddComponent<CanvasScaler>();
            hudGo.AddComponent<GraphicRaycaster>();

            var canvasGroup = hudGo.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0;

            var layoutGo = new GameObject("Layout");
            layoutGo.transform.SetParent(hudGo.transform, false);
            var vlg = layoutGo.AddComponent<VerticalLayoutGroup>();
            vlg.childAlignment = TextAnchor.MiddleCenter;

            var textGo = new GameObject("Text");
            textGo.transform.SetParent(layoutGo.transform, false);
            var text = textGo.AddComponent<TextMeshProUGUI>();
            text.fontSize = 24;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.white;

            var hud = hudGo.AddComponent<PlayerHudNotification>();

            var type = typeof(PlayerHudNotification);
            var textField = type.GetField("m_Text", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var layoutField = type.GetField("m_LayoutGroupTransform", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var canvasGroupField = type.GetField("m_CanvasGroup", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            textField?.SetValue(hud, text);
            layoutField?.SetValue(hud, layoutGo.transform);
            canvasGroupField?.SetValue(hud, canvasGroup);

            layoutGo.SetActive(false);
        }

        void EnsureOVRNetworkPlayerSetup()
        {
            if (FindFirstObjectByType<OVRNetworkPlayerSetup>() == null)
            {
                var go = new GameObject("OVRNetworkPlayerSetup");
                go.AddComponent<OVRNetworkPlayerSetup>();
            }
        }

        /// <summary>
        /// Adds PointableCanvas so OVR hands/controllers can interact with the UI.
        /// </summary>
        void MakeCanvasInteractable(GameObject canvasGo)
        {
            var canvas = canvasGo.GetComponent<Canvas>();
            if (canvas == null) return;

            // PointableCanvas requires GraphicRaycaster
            if (canvasGo.GetComponent<GraphicRaycaster>() == null)
                canvasGo.AddComponent<GraphicRaycaster>();

            // Add PointableCanvas for Meta Interaction SDK
            //var pointable = canvasGo.GetComponent<PointableCanvas>();
            //if (pointable == null)
            //{
            //    pointable = canvasGo.AddComponent<PointableCanvas>();
            //    // Set the _canvas field via reflection (it's serialized private)
            //    var canvasField = typeof(PointableCanvas).GetField("_canvas",
            //        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            //    canvasField?.SetValue(pointable, canvas);
            //}
        }

        // ─── Runtime Lobby UI Creation ───────────────────────────────

        void CreateLobbyUI()
        {
            // Load TMP default font explicitly to avoid missing font at runtime
            m_Font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
            if (m_Font == null)
                m_Font = TMP_Settings.defaultFontAsset;

            var canvasGo = new GameObject("MultiplayerLobbyCanvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvasGo.AddComponent<CanvasScaler>();
            canvasGo.AddComponent<GraphicRaycaster>();

            // Make interactable with OVR hands/controllers
            MakeCanvasInteractable(canvasGo);

            var rt = canvasGo.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(800, 600);
            canvasGo.transform.localScale = Vector3.one * m_CanvasScale;

            if (m_UISpawnPoint != null)
            {
                canvasGo.transform.position = m_UISpawnPoint.position;
                canvasGo.transform.rotation = m_UISpawnPoint.rotation;
            }
            else
            {
                var cam = Camera.main;
                if (cam != null)
                {
                    canvasGo.transform.position = cam.transform.position + cam.transform.forward * 1.5f;
                    canvasGo.transform.rotation = Quaternion.LookRotation(
                        canvasGo.transform.position - cam.transform.position);
                }
            }

            var bgGo = new GameObject("Background");
            bgGo.transform.SetParent(canvasGo.transform, false);
            var bgImage = bgGo.AddComponent<Image>();
            bgImage.color = new Color(0.1f, 0.1f, 0.15f, 0.9f);
            var bgRt = bgGo.GetComponent<RectTransform>();
            bgRt.anchorMin = Vector2.zero;
            bgRt.anchorMax = Vector2.one;
            bgRt.offsetMin = Vector2.zero;
            bgRt.offsetMax = Vector2.zero;

            m_LobbyUI = canvasGo.AddComponent<OVRMultiplayerLobbyUI>();

            // ── Create Panels ──
            var panelAuth = CreatePanel(canvasGo.transform, "PanelAuth");
            CreateLabel(panelAuth, "Authenticating...", 32);

            var panelMain = CreatePanel(canvasGo.transform, "PanelMainMenu");
            CreateLabel(panelMain, "Multiplayer Lobby", 36);
            CreateButton(panelMain, "Create Room", () => m_LobbyUI.OnCreateRoomButton());
            CreateButton(panelMain, "Browse Rooms", () => m_LobbyUI.OnBrowseRoomsButton());
            CreateButton(panelMain, "Enter Code", () => m_LobbyUI.OnEnterCodeButton());
            CreateButton(panelMain, "Quick Join", () => m_LobbyUI.OnQuickJoinButton());

            var panelCreate = CreatePanel(canvasGo.transform, "PanelCreateRoom");
            CreateLabel(panelCreate, "Create Room", 32);
            var roomNameInput = CreateInputField(panelCreate, "Enter room name...");
            CreateButton(panelCreate, "Create", () => m_LobbyUI.OnConfirmCreateRoom());
            CreateButton(panelCreate, "<- Back", () => m_LobbyUI.OnBackToMainMenu());

            var panelRoomList = CreatePanel(canvasGo.transform, "PanelRoomList");
            CreateLabel(panelRoomList, "Room List", 32);
            var scrollContent = CreateScrollView(panelRoomList);
            CreateButton(panelRoomList, "Refresh", () => m_LobbyUI.OnRefreshRoomList());
            CreateButton(panelRoomList, "<- Back", () => m_LobbyUI.OnBackToMainMenu());

            var panelEnterCode = CreatePanel(canvasGo.transform, "PanelEnterCode");
            CreateLabel(panelEnterCode, "Join by Code", 32);
            var codeInput = CreateInputField(panelEnterCode, "Enter room code...");
            CreateButton(panelEnterCode, "Join", () => m_LobbyUI.OnConfirmEnterCode());
            CreateButton(panelEnterCode, "<- Back", () => m_LobbyUI.OnBackToMainMenu());

            var panelConnecting = CreatePanel(canvasGo.transform, "PanelConnecting");
            var connectingText = CreateLabel(panelConnecting, "Connecting...", 28);
            CreateButton(panelConnecting, "Cancel", () => m_LobbyUI.OnCancelButton());

            var panelConnected = CreatePanel(canvasGo.transform, "PanelConnected");
            var connectedRoomText = CreateLabel(panelConnected, "Room: -", 28);
            var roomCodeDisplay = CreateLabel(panelConnected, "Code: -", 22);
            var playersText = CreateLabel(panelConnected, "Players: 0/2", 24);
            CreateButton(panelConnected, "Disconnect", () => m_LobbyUI.OnDisconnectButton());

            var panelError = CreatePanel(canvasGo.transform, "PanelError");
            var errorText = CreateLabel(panelError, "Error", 24);
            CreateButton(panelError, "<- Back", () => m_LobbyUI.OnBackToMainMenu());

            var slotPrefab = CreateRoomSlotPrefab();

            // ── Assign SerializedFields via Reflection ──
            SetPrivateField(m_LobbyUI, "m_PanelAuthenticating", panelAuth);
            SetPrivateField(m_LobbyUI, "m_PanelMainMenu", panelMain);
            SetPrivateField(m_LobbyUI, "m_PanelCreateRoom", panelCreate);
            SetPrivateField(m_LobbyUI, "m_PanelRoomList", panelRoomList);
            SetPrivateField(m_LobbyUI, "m_PanelEnterCode", panelEnterCode);
            SetPrivateField(m_LobbyUI, "m_PanelConnecting", panelConnecting);
            SetPrivateField(m_LobbyUI, "m_PanelConnected", panelConnected);
            SetPrivateField(m_LobbyUI, "m_PanelError", panelError);
            SetPrivateField(m_LobbyUI, "m_RoomNameInput", roomNameInput);
            SetPrivateField(m_LobbyUI, "m_RoomCodeInput", codeInput);
            SetPrivateField(m_LobbyUI, "m_RoomListContent", scrollContent);
            SetPrivateField(m_LobbyUI, "m_RoomSlotPrefab", slotPrefab);
            SetPrivateField(m_LobbyUI, "m_ConnectingStatusText", connectingText.GetComponent<TMP_Text>());
            SetPrivateField(m_LobbyUI, "m_ErrorText", errorText.GetComponent<TMP_Text>());
            SetPrivateField(m_LobbyUI, "m_ConnectedRoomText", connectedRoomText.GetComponent<TMP_Text>());
            SetPrivateField(m_LobbyUI, "m_ConnectedPlayersText", playersText.GetComponent<TMP_Text>());
            SetPrivateField(m_LobbyUI, "m_RoomCodeDisplayText", roomCodeDisplay.GetComponent<TMP_Text>());
        }

        // ─── UI Helper Methods ───────────────────────────────────────

        GameObject CreatePanel(Transform parent, string name)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(40, 40);
            rt.offsetMax = new Vector2(-40, -40);

            var vlg = go.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = 20;
            vlg.childAlignment = TextAnchor.MiddleCenter;
            vlg.childControlWidth = true;
            vlg.childControlHeight = false;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.padding = new RectOffset(20, 20, 20, 20);

            go.SetActive(false);
            return go;
        }

        GameObject CreateLabel(GameObject parent, string text, int fontSize)
        {
            var go = new GameObject("Label");
            go.transform.SetParent(parent.transform, false);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            if (m_Font != null) tmp.font = m_Font;
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;

            var le = go.AddComponent<LayoutElement>();
            le.preferredHeight = fontSize + 20;

            return go;
        }

        void CreateButton(GameObject parent, string label, UnityEngine.Events.UnityAction onClick)
        {
            var go = new GameObject($"Btn_{label}");
            go.transform.SetParent(parent.transform, false);

            var image = go.AddComponent<Image>();
            image.color = new Color(0.2f, 0.4f, 0.8f, 1f);

            var btn = go.AddComponent<Button>();
            var colors = btn.colors;
            colors.highlightedColor = new Color(0.3f, 0.5f, 0.9f, 1f);
            colors.pressedColor = new Color(0.15f, 0.3f, 0.6f, 1f);
            btn.colors = colors;
            btn.onClick.AddListener(onClick);

            var le = go.AddComponent<LayoutElement>();
            le.preferredHeight = 60;

            var textGo = new GameObject("Text");
            textGo.transform.SetParent(go.transform, false);
            var tmp = textGo.AddComponent<TextMeshProUGUI>();
            if (m_Font != null) tmp.font = m_Font;
            tmp.text = label;
            tmp.fontSize = 24;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
            var textRt = textGo.GetComponent<RectTransform>();
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.offsetMin = Vector2.zero;
            textRt.offsetMax = Vector2.zero;
        }

        TMP_InputField CreateInputField(GameObject parent, string placeholder)
        {
            var go = new GameObject("InputField");
            go.transform.SetParent(parent.transform, false);

            var image = go.AddComponent<Image>();
            image.color = new Color(0.15f, 0.15f, 0.2f, 1f);

            var le = go.AddComponent<LayoutElement>();
            le.preferredHeight = 60;

            var textAreaGo = new GameObject("Text Area");
            textAreaGo.transform.SetParent(go.transform, false);
            var textAreaRt = textAreaGo.AddComponent<RectTransform>();
            textAreaRt.anchorMin = Vector2.zero;
            textAreaRt.anchorMax = Vector2.one;
            textAreaRt.offsetMin = new Vector2(10, 5);
            textAreaRt.offsetMax = new Vector2(-10, -5);
            textAreaGo.AddComponent<RectMask2D>();

            var placeholderGo = new GameObject("Placeholder");
            placeholderGo.transform.SetParent(textAreaGo.transform, false);
            var phTmp = placeholderGo.AddComponent<TextMeshProUGUI>();
            if (m_Font != null) phTmp.font = m_Font;
            phTmp.text = placeholder;
            phTmp.fontSize = 22;
            phTmp.fontStyle = FontStyles.Italic;
            phTmp.color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
            phTmp.alignment = TextAlignmentOptions.MidlineLeft;
            var phRt = placeholderGo.GetComponent<RectTransform>();
            phRt.anchorMin = Vector2.zero;
            phRt.anchorMax = Vector2.one;
            phRt.offsetMin = Vector2.zero;
            phRt.offsetMax = Vector2.zero;

            var textGo = new GameObject("Text");
            textGo.transform.SetParent(textAreaGo.transform, false);
            var textTmp = textGo.AddComponent<TextMeshProUGUI>();
            if (m_Font != null) textTmp.font = m_Font;
            textTmp.fontSize = 22;
            textTmp.color = Color.white;
            textTmp.alignment = TextAlignmentOptions.MidlineLeft;
            var textRt = textGo.GetComponent<RectTransform>();
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.offsetMin = Vector2.zero;
            textRt.offsetMax = Vector2.zero;

            var inputField = go.AddComponent<TMP_InputField>();
            inputField.textViewport = textAreaRt;
            inputField.textComponent = textTmp;
            inputField.placeholder = phTmp;
            inputField.fontAsset = textTmp.font;

            return inputField;
        }

        Transform CreateScrollView(GameObject parent)
        {
            var scrollGo = new GameObject("ScrollView");
            scrollGo.transform.SetParent(parent.transform, false);

            var scrollImage = scrollGo.AddComponent<Image>();
            scrollImage.color = new Color(0.08f, 0.08f, 0.12f, 0.8f);

            var scrollRect = scrollGo.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;

            var le = scrollGo.AddComponent<LayoutElement>();
            le.preferredHeight = 250;
            le.flexibleHeight = 1;

            var viewportGo = new GameObject("Viewport");
            viewportGo.transform.SetParent(scrollGo.transform, false);
            var vpRt = viewportGo.AddComponent<RectTransform>();
            vpRt.anchorMin = Vector2.zero;
            vpRt.anchorMax = Vector2.one;
            vpRt.offsetMin = Vector2.zero;
            vpRt.offsetMax = Vector2.zero;
            viewportGo.AddComponent<RectMask2D>();

            var contentGo = new GameObject("Content");
            contentGo.transform.SetParent(viewportGo.transform, false);
            var contentRt = contentGo.AddComponent<RectTransform>();
            contentRt.anchorMin = new Vector2(0, 1);
            contentRt.anchorMax = new Vector2(1, 1);
            contentRt.pivot = new Vector2(0.5f, 1);
            contentRt.sizeDelta = new Vector2(0, 0);

            var vlg = contentGo.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = 8;
            vlg.childControlWidth = true;
            vlg.childControlHeight = false;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.padding = new RectOffset(5, 5, 5, 5);

            var csf = contentGo.AddComponent<ContentSizeFitter>();
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            scrollRect.viewport = vpRt;
            scrollRect.content = contentRt;

            return contentGo.transform;
        }

        GameObject CreateRoomSlotPrefab()
        {
            var go = new GameObject("RoomSlotPrefab");
            go.SetActive(false);

            var image = go.AddComponent<Image>();
            image.color = new Color(0.18f, 0.18f, 0.25f, 1f);

            var hlg = go.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = 10;
            hlg.childAlignment = TextAnchor.MiddleCenter;
            hlg.childControlWidth = true;
            hlg.childControlHeight = true;
            hlg.childForceExpandWidth = false;
            hlg.childForceExpandHeight = true;
            hlg.padding = new RectOffset(15, 15, 5, 5);

            var le = go.AddComponent<LayoutElement>();
            le.preferredHeight = 55;

            var nameGo = new GameObject("RoomName");
            nameGo.transform.SetParent(go.transform, false);
            var nameTmp = nameGo.AddComponent<TextMeshProUGUI>();
            if (m_Font != null) nameTmp.font = m_Font;
            nameTmp.fontSize = 20;
            nameTmp.color = Color.white;
            nameTmp.alignment = TextAlignmentOptions.MidlineLeft;
            var nameLe = nameGo.AddComponent<LayoutElement>();
            nameLe.flexibleWidth = 1;

            var countGo = new GameObject("PlayerCount");
            countGo.transform.SetParent(go.transform, false);
            var countTmp = countGo.AddComponent<TextMeshProUGUI>();
            if (m_Font != null) countTmp.font = m_Font;
            countTmp.fontSize = 18;
            countTmp.color = new Color(0.7f, 0.7f, 0.7f);
            countTmp.alignment = TextAlignmentOptions.Center;
            var countLe = countGo.AddComponent<LayoutElement>();
            countLe.preferredWidth = 80;

            var btnGo = new GameObject("JoinBtn");
            btnGo.transform.SetParent(go.transform, false);
            var btnImage = btnGo.AddComponent<Image>();
            btnImage.color = new Color(0.2f, 0.6f, 0.3f, 1f);
            var btn = btnGo.AddComponent<Button>();
            var btnLe = btnGo.AddComponent<LayoutElement>();
            btnLe.preferredWidth = 100;

            var btnTextGo = new GameObject("Text");
            btnTextGo.transform.SetParent(btnGo.transform, false);
            var btnTmp = btnTextGo.AddComponent<TextMeshProUGUI>();
            if (m_Font != null) btnTmp.font = m_Font;
            btnTmp.text = "Join";
            btnTmp.fontSize = 18;
            btnTmp.alignment = TextAlignmentOptions.Center;
            btnTmp.color = Color.white;
            var btnTextRt = btnTextGo.GetComponent<RectTransform>();
            btnTextRt.anchorMin = Vector2.zero;
            btnTextRt.anchorMax = Vector2.one;
            btnTextRt.offsetMin = Vector2.zero;
            btnTextRt.offsetMax = Vector2.zero;

            var slotUI = go.AddComponent<OVRLobbySlotUI>();
            SetPrivateField(slotUI, "m_RoomNameText", nameTmp);
            SetPrivateField(slotUI, "m_PlayerCountText", countTmp);
            SetPrivateField(slotUI, "m_JoinButton", btn);

            DontDestroyOnLoad(go);
            return go;
        }

        // ─── Reflection Helper ───────────────────────────────────────

        static void SetPrivateField(object target, string fieldName, object value)
        {
            var field = target.GetType().GetField(fieldName,
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);
            if (field != null)
                field.SetValue(target, value);
            else
                Debug.LogWarning($"[OVRMultiplayerBootstrap] Field not found: {fieldName}");
        }
    }
}

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using OVRMultiplayer;

/// <summary>
/// Editor tool that auto-generates Lobby UI and RoomSlot prefabs.
/// Menu: Tools -> OVR Multiplayer -> Create Lobby UI Prefabs
/// </summary>
public static class OVRLobbyUIPrefabCreator
{
    const string k_PrefabFolder = "Assets/Scripts/Multiplayer/Prefabs";
    const string k_LobbyUIPrefabPath = k_PrefabFolder + "/OVRMultiplayerLobbyUI.prefab";
    const string k_RoomSlotPrefabPath = k_PrefabFolder + "/OVRLobbySlot.prefab";

    [MenuItem("Tools/OVR Multiplayer/Create Lobby UI Prefabs", false, 150)]
    public static void CreatePrefabs()
    {
        EnsureFolder(k_PrefabFolder);

        var roomSlotPrefab = CreateRoomSlotPrefab();
        var lobbyUIPrefab = CreateLobbyUIPrefab(roomSlotPrefab);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeObject = lobbyUIPrefab;

        EditorUtility.DisplayDialog("Prefabs Created",
            "Lobby UI prefabs have been created:\n\n" +
            $"  {k_LobbyUIPrefabPath}\n" +
            $"  {k_RoomSlotPrefabPath}\n\n" +
            "Run 'Setup Current Scene' to auto-assign them.",
            "OK");
    }

    [MenuItem("Tools/OVR Multiplayer/Create Lobby UI Prefabs", true)]
    static bool CreatePrefabsValidate() => !Application.isPlaying;

    // ─── Lobby UI Prefab ─────────────────────────────────────────

    static GameObject CreateLobbyUIPrefab(GameObject roomSlotPrefab)
    {
        var root = new GameObject("OVRMultiplayerLobbyUI");
        var canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        root.AddComponent<CanvasScaler>();
        root.AddComponent<GraphicRaycaster>();

        var rootRt = root.GetComponent<RectTransform>();
        rootRt.sizeDelta = new Vector2(800, 600);
        root.transform.localScale = Vector3.one * 0.001f;

        var bg = CreateChild<Image>(root.transform, "Background");
        bg.color = new Color(0.1f, 0.1f, 0.15f, 0.9f);
        Stretch(bg.GetComponent<RectTransform>());

        var lobbyUI = root.AddComponent<OVRMultiplayerLobbyUI>();

        // ── Panels ──
        var panelAuth = CreatePanel(root.transform, "PanelAuthenticating");
        var authLabel = CreateLabel(panelAuth.transform, "Authenticating...", 32);

        var panelMain = CreatePanel(root.transform, "PanelMainMenu");
        CreateLabel(panelMain.transform, "Multiplayer Lobby", 36);
        CreateButton(panelMain.transform, "Create Room", "OnCreateRoomButton");
        CreateButton(panelMain.transform, "Browse Rooms", "OnBrowseRoomsButton");
        CreateButton(panelMain.transform, "Enter Code", "OnEnterCodeButton");
        CreateButton(panelMain.transform, "Quick Join", "OnQuickJoinButton");

        var panelCreate = CreatePanel(root.transform, "PanelCreateRoom");
        CreateLabel(panelCreate.transform, "Create Room", 32);
        var roomNameInput = CreateInputField(panelCreate.transform, "Enter room name...");
        CreateButton(panelCreate.transform, "Create", "OnConfirmCreateRoom");
        CreateButton(panelCreate.transform, "<- Back", "OnBackToMainMenu");

        var panelRoomList = CreatePanel(root.transform, "PanelRoomList");
        CreateLabel(panelRoomList.transform, "Room List", 32);
        var scrollContent = CreateScrollView(panelRoomList.transform);
        CreateButton(panelRoomList.transform, "Refresh", "OnRefreshRoomList");
        CreateButton(panelRoomList.transform, "<- Back", "OnBackToMainMenu");

        var panelEnterCode = CreatePanel(root.transform, "PanelEnterCode");
        CreateLabel(panelEnterCode.transform, "Join by Code", 32);
        var codeInput = CreateInputField(panelEnterCode.transform, "Enter room code...");
        CreateButton(panelEnterCode.transform, "Join", "OnConfirmEnterCode");
        CreateButton(panelEnterCode.transform, "<- Back", "OnBackToMainMenu");

        var panelConnecting = CreatePanel(root.transform, "PanelConnecting");
        var connectingLabel = CreateLabel(panelConnecting.transform, "Connecting...", 28);
        CreateButton(panelConnecting.transform, "Cancel", "OnCancelButton");

        var panelConnected = CreatePanel(root.transform, "PanelConnected");
        var connectedRoomLabel = CreateLabel(panelConnected.transform, "Room: -", 28);
        var roomCodeLabel = CreateLabel(panelConnected.transform, "Code: -", 22);
        var playersLabel = CreateLabel(panelConnected.transform, "Players: 0/2", 24);
        CreateButton(panelConnected.transform, "Disconnect", "OnDisconnectButton");

        var panelError = CreatePanel(root.transform, "PanelError");
        var errorLabel = CreateLabel(panelError.transform, "Error", 24);
        CreateButton(panelError.transform, "<- Back", "OnBackToMainMenu");

        // ── Assign fields via SerializedObject ──
        var so = new SerializedObject(lobbyUI);
        so.FindProperty("m_PanelAuthenticating").objectReferenceValue = panelAuth;
        so.FindProperty("m_PanelMainMenu").objectReferenceValue = panelMain;
        so.FindProperty("m_PanelCreateRoom").objectReferenceValue = panelCreate;
        so.FindProperty("m_PanelRoomList").objectReferenceValue = panelRoomList;
        so.FindProperty("m_PanelEnterCode").objectReferenceValue = panelEnterCode;
        so.FindProperty("m_PanelConnecting").objectReferenceValue = panelConnecting;
        so.FindProperty("m_PanelConnected").objectReferenceValue = panelConnected;
        so.FindProperty("m_PanelError").objectReferenceValue = panelError;
        so.FindProperty("m_RoomNameInput").objectReferenceValue = roomNameInput;
        so.FindProperty("m_RoomCodeInput").objectReferenceValue = codeInput;
        so.FindProperty("m_RoomListContent").objectReferenceValue = scrollContent;
        so.FindProperty("m_RoomSlotPrefab").objectReferenceValue = roomSlotPrefab;
        so.FindProperty("m_ConnectingStatusText").objectReferenceValue = connectingLabel.GetComponent<TMP_Text>();
        so.FindProperty("m_ErrorText").objectReferenceValue = errorLabel.GetComponent<TMP_Text>();
        so.FindProperty("m_ConnectedRoomText").objectReferenceValue = connectedRoomLabel.GetComponent<TMP_Text>();
        so.FindProperty("m_ConnectedPlayersText").objectReferenceValue = playersLabel.GetComponent<TMP_Text>();
        so.FindProperty("m_RoomCodeDisplayText").objectReferenceValue = roomCodeLabel.GetComponent<TMP_Text>();
        so.ApplyModifiedPropertiesWithoutUndo();

        var prefab = PrefabUtility.SaveAsPrefabAsset(root, k_LobbyUIPrefabPath);
        Object.DestroyImmediate(root);

        Debug.Log($"[OVRLobbyUIPrefabCreator] Lobby UI prefab created: {k_LobbyUIPrefabPath}");
        return prefab;
    }

    // ─── RoomSlot Prefab ─────────────────────────────────────────

    static GameObject CreateRoomSlotPrefab()
    {
        var root = new GameObject("OVRLobbySlot");

        var image = root.AddComponent<Image>();
        image.color = new Color(0.18f, 0.18f, 0.25f, 1f);

        var hlg = root.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 10;
        hlg.childAlignment = TextAnchor.MiddleCenter;
        hlg.childControlWidth = true;
        hlg.childControlHeight = true;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = true;
        hlg.padding = new RectOffset(15, 15, 5, 5);

        var le = root.AddComponent<LayoutElement>();
        le.preferredHeight = 55;

        var nameGo = new GameObject("RoomName");
        nameGo.transform.SetParent(root.transform, false);
        var nameTmp = nameGo.AddComponent<TextMeshProUGUI>();
        nameTmp.fontSize = 20;
        nameTmp.color = Color.white;
        nameTmp.alignment = TextAlignmentOptions.MidlineLeft;
        var nameLe = nameGo.AddComponent<LayoutElement>();
        nameLe.flexibleWidth = 1;

        var countGo = new GameObject("PlayerCount");
        countGo.transform.SetParent(root.transform, false);
        var countTmp = countGo.AddComponent<TextMeshProUGUI>();
        countTmp.fontSize = 18;
        countTmp.color = new Color(0.7f, 0.7f, 0.7f);
        countTmp.alignment = TextAlignmentOptions.Center;
        var countLe = countGo.AddComponent<LayoutElement>();
        countLe.preferredWidth = 80;

        var btnGo = new GameObject("JoinBtn");
        btnGo.transform.SetParent(root.transform, false);
        var btnImage = btnGo.AddComponent<Image>();
        btnImage.color = new Color(0.2f, 0.6f, 0.3f, 1f);
        btnGo.AddComponent<Button>();
        var btnLe = btnGo.AddComponent<LayoutElement>();
        btnLe.preferredWidth = 100;

        var btnTextGo = new GameObject("Text");
        btnTextGo.transform.SetParent(btnGo.transform, false);
        var btnTmp = btnTextGo.AddComponent<TextMeshProUGUI>();
        btnTmp.text = "Join";
        btnTmp.fontSize = 18;
        btnTmp.alignment = TextAlignmentOptions.Center;
        btnTmp.color = Color.white;
        Stretch(btnTextGo.GetComponent<RectTransform>());

        var slotUI = root.AddComponent<OVRLobbySlotUI>();
        var so = new SerializedObject(slotUI);
        so.FindProperty("m_RoomNameText").objectReferenceValue = nameTmp;
        so.FindProperty("m_PlayerCountText").objectReferenceValue = countTmp;
        so.FindProperty("m_JoinButton").objectReferenceValue = btnGo.GetComponent<Button>();
        so.ApplyModifiedPropertiesWithoutUndo();

        var prefab = PrefabUtility.SaveAsPrefabAsset(root, k_RoomSlotPrefabPath);
        Object.DestroyImmediate(root);

        Debug.Log($"[OVRLobbyUIPrefabCreator] RoomSlot prefab created: {k_RoomSlotPrefabPath}");
        return prefab;
    }

    // ─── UI Helpers ──────────────────────────────────────────────

    static GameObject CreatePanel(Transform parent, string name)
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

    static GameObject CreateLabel(Transform parent, string text, int fontSize)
    {
        var go = new GameObject("Label_" + text.Replace(" ", ""));
        go.transform.SetParent(parent, false);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;

        var le = go.AddComponent<LayoutElement>();
        le.preferredHeight = fontSize + 20;

        return go;
    }

    static void CreateButton(Transform parent, string label, string methodName)
    {
        var go = new GameObject($"Btn_{label}");
        go.transform.SetParent(parent, false);

        var image = go.AddComponent<Image>();
        image.color = new Color(0.2f, 0.4f, 0.8f, 1f);

        var btn = go.AddComponent<Button>();
        var colors = btn.colors;
        colors.highlightedColor = new Color(0.3f, 0.5f, 0.9f, 1f);
        colors.pressedColor = new Color(0.15f, 0.3f, 0.6f, 1f);
        btn.colors = colors;

        var le = go.AddComponent<LayoutElement>();
        le.preferredHeight = 60;

        var textGo = new GameObject("Text");
        textGo.transform.SetParent(go.transform, false);
        var tmp = textGo.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 24;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        Stretch(textGo.GetComponent<RectTransform>());
    }

    static TMP_InputField CreateInputField(Transform parent, string placeholder)
    {
        var go = new GameObject("InputField");
        go.transform.SetParent(parent, false);

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
        phTmp.text = placeholder;
        phTmp.fontSize = 22;
        phTmp.fontStyle = FontStyles.Italic;
        phTmp.color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
        phTmp.alignment = TextAlignmentOptions.MidlineLeft;
        Stretch(placeholderGo.GetComponent<RectTransform>());

        var textGo = new GameObject("Text");
        textGo.transform.SetParent(textAreaGo.transform, false);
        var textTmp = textGo.AddComponent<TextMeshProUGUI>();
        textTmp.fontSize = 22;
        textTmp.color = Color.white;
        textTmp.alignment = TextAlignmentOptions.MidlineLeft;
        Stretch(textGo.GetComponent<RectTransform>());

        var inputField = go.AddComponent<TMP_InputField>();
        inputField.textViewport = textAreaRt;
        inputField.textComponent = textTmp;
        inputField.placeholder = phTmp;
        inputField.fontAsset = textTmp.font;

        return inputField;
    }

    static Transform CreateScrollView(Transform parent)
    {
        var scrollGo = new GameObject("ScrollView");
        scrollGo.transform.SetParent(parent, false);

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
        Stretch(vpRt);
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

    static T CreateChild<T>(Transform parent, string name) where T : Component
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        return go.AddComponent<T>();
    }

    static void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;

        var parts = path.Split('/');
        var current = parts[0];
        for (int i = 1; i < parts.Length; i++)
        {
            var next = current + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(current, parts[i]);
            current = next;
        }
    }
}

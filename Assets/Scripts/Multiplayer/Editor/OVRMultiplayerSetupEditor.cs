using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using OVRMultiplayer;

/// <summary>
/// Editor tool that auto-configures the multiplayer system in OVRMultiTest scene.
/// Menu: Tools -> OVR Multiplayer -> Setup Current Scene
/// </summary>
public static class OVRMultiplayerSetupEditor
{
    const string k_NetworkManagerPrefabPath = "Assets/XRMP/Prefabs/Managers/Network Manager XR Multiplayer.prefab";
    const string k_GameManagerPrefabPath = "Assets/XRMP/Prefabs/Managers/XRI_Network_Game_Manager.prefab";
    const string k_LobbyUIPrefabPath = "Assets/Scripts/Multiplayer/Prefabs/OVRMultiplayerLobbyUI.prefab";

    [MenuItem("Tools/OVR Multiplayer/Setup Current Scene", false, 100)]
    public static void SetupCurrentScene()
    {
        var networkManagerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(k_NetworkManagerPrefabPath);
        var gameManagerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(k_GameManagerPrefabPath);

        if (networkManagerPrefab == null)
        {
            EditorUtility.DisplayDialog("Error",
                $"Network Manager prefab not found:\n{k_NetworkManagerPrefabPath}", "OK");
            return;
        }

        if (gameManagerPrefab == null)
        {
            EditorUtility.DisplayDialog("Error",
                $"Game Manager prefab not found:\n{k_GameManagerPrefabPath}", "OK");
            return;
        }

        var existingBootstrap = Object.FindFirstObjectByType<OVRMultiplayerBootstrap>();
        if (existingBootstrap != null)
        {
            if (!EditorUtility.DisplayDialog("Already Exists",
                "OVRMultiplayerBootstrap already exists in the scene.\nDelete and recreate?",
                "Recreate", "Cancel"))
            {
                return;
            }
            Undo.DestroyObjectImmediate(existingBootstrap.gameObject);
        }

        // 1. Create Bootstrap GameObject
        var bootstrapGo = new GameObject("MultiplayerBootstrap");
        Undo.RegisterCreatedObjectUndo(bootstrapGo, "Create MultiplayerBootstrap");
        var bootstrap = bootstrapGo.AddComponent<OVRMultiplayerBootstrap>();

        var lobbyUIPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(k_LobbyUIPrefabPath);
        if (lobbyUIPrefab == null)
        {
            Debug.Log("[OVR Multiplayer Setup] Lobby UI prefab not found. " +
                       "Use Tools > OVR Multiplayer > Create Lobby UI Prefabs to generate, " +
                       "or it will be auto-generated at runtime.");
        }

        var so = new SerializedObject(bootstrap);
        so.FindProperty("m_NetworkManagerPrefab").objectReferenceValue = networkManagerPrefab;
        so.FindProperty("m_GameManagerPrefab").objectReferenceValue = gameManagerPrefab;
        so.FindProperty("m_CanvasScale").floatValue = 0.001f;
        if (lobbyUIPrefab != null)
            so.FindProperty("m_LobbyUIPrefab").objectReferenceValue = lobbyUIPrefab;
        so.ApplyModifiedProperties();

        // 2. Create UI Spawn Point (1.2m in front of camera)
        var spawnPointGo = new GameObject("LobbyUI_SpawnPoint");
        Undo.RegisterCreatedObjectUndo(spawnPointGo, "Create UI SpawnPoint");

        var cameraRig = Object.FindFirstObjectByType<OVRCameraRig>();
        if (cameraRig != null)
        {
            spawnPointGo.transform.SetParent(cameraRig.transform, false);
            spawnPointGo.transform.localPosition = new Vector3(0f, 1.3f, 1.2f);
            spawnPointGo.transform.localRotation = Quaternion.identity;
        }
        else
        {
            spawnPointGo.transform.position = new Vector3(0f, 1.3f, 1.2f);
        }

        so = new SerializedObject(bootstrap);
        so.FindProperty("m_UISpawnPoint").objectReferenceValue = spawnPointGo.transform;
        so.ApplyModifiedProperties();

        // 3. Create OVRNetworkPlayerSetup
        var playerSetupGo = new GameObject("OVRNetworkPlayerSetup");
        Undo.RegisterCreatedObjectUndo(playerSetupGo, "Create OVRNetworkPlayerSetup");
        playerSetupGo.AddComponent<OVRNetworkPlayerSetup>();

        // 4. Mark scene dirty
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

        string lobbyStatus = lobbyUIPrefab != null
            ? "  LobbyUI prefab assigned"
            : "  LobbyUI prefab not found (auto-generated at runtime)\n  -> Tools > OVR Multiplayer > Create Lobby UI Prefabs";

        Debug.Log("[OVR Multiplayer Setup] Setup complete!\n" +
                  "- MultiplayerBootstrap created\n" +
                  "- LobbyUI_SpawnPoint created\n" +
                  "- OVRNetworkPlayerSetup created\n" +
                  (lobbyUIPrefab != null ? "- LobbyUI prefab assigned\n" : "") +
                  "Save the scene (Ctrl+S)");

        EditorUtility.DisplayDialog("Setup Complete",
            "OVR Multiplayer setup is done!\n\n" +
            "Created objects:\n" +
            "  MultiplayerBootstrap (prefabs auto-assigned)\n" +
            "  LobbyUI_SpawnPoint (lobby UI position)\n" +
            "  OVRNetworkPlayerSetup (OVR<->Network bridge)\n" +
            lobbyStatus + "\n\n" +
            "Save the scene (Ctrl+S) and press Play.",
            "OK");

        Selection.activeGameObject = bootstrapGo;
    }

    [MenuItem("Tools/OVR Multiplayer/Setup Current Scene", true)]
    public static bool SetupCurrentSceneValidate()
    {
        return !Application.isPlaying;
    }
}

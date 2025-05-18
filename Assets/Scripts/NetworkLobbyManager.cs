using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkLobbyManager : MonoBehaviour
{
    public static NetworkLobbyManager Instance { get; private set; }

    [SerializeField] private string gameSceneName = "BaseScene";
    [SerializeField] private bool autoConnectInClone = true;
    
    public string CurrentLobbyId { get; private set; }
    public bool IsHost { get; private set; }
    public string PlayerId { get; private set; }

    private NetworkManager networkManager;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            PlayerId = System.Guid.NewGuid().ToString();
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        networkManager = GetComponent<NetworkManager>();
        if (networkManager == null)
        {
            networkManager = gameObject.AddComponent<NetworkManager>();
        }
        
        // Auto-connect logic for ParrelSync clones
        #if UNITY_EDITOR
        if (autoConnectInClone && ParrelSync.ClonesManager.IsClone())
        {
            // Auto-connect clients after a short delay
            Invoke("AutoConnectAsClient", 1.0f);
        }
        #endif
    }
    
    #if UNITY_EDITOR
    private void AutoConnectAsClient()
    {
        Debug.Log("Clone auto-connecting as client...");
        
        // For simplicity in testing, just create a temporary lobby name
        string tempLobbyId = "AutoLobby";
        JoinLobby(tempLobbyId);
        
        // Notify any UI that we're auto-connecting
        var uiManager = FindObjectOfType<LobbyUIManager>();
        if (uiManager != null)
        {
            uiManager.UpdateUIForAutoConnect();
        }
    }
    #endif

    public void CreateLobby(string lobbyName)
    {
        CurrentLobbyId = LobbyManager.CreateLobby(lobbyName, PlayerId);
        IsHost = true;
        
        // Set up as host
        networkManager.StartHost();
    }

    public void JoinLobby(string lobbyId)
    {
        if (LobbyManager.JoinLobby(lobbyId, PlayerId))
        {
            CurrentLobbyId = lobbyId;
            IsHost = false;
            
            // Set up as client
            networkManager.StartClient();
        }
    }

    public void StartGame()
    {
        if (IsHost)
        {
            // Only the host can start the game
            NetworkManager.Singleton.SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
        }
    }

    public void LeaveLobby()
    {
        if (!string.IsNullOrEmpty(CurrentLobbyId))
        {
            LobbyManager.LeaveLobby(CurrentLobbyId, PlayerId, IsHost);
            
            if (networkManager.IsConnectedClient)
            {
                networkManager.Shutdown();
            }
            
            CurrentLobbyId = null;
            SceneManager.LoadScene("LobbyScene");
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            LeaveLobby();
        }
    }
}
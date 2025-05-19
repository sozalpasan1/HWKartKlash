using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkLobbyManager : MonoBehaviour
{
    public static NetworkLobbyManager Instance { get; private set; }

    [SerializeField] private string gameSceneName = "BaseScene";
    
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
    }

    public void CreateLobby(string lobbyName)
{
    Debug.Log($"[NetworkLobbyManager] Creating lobby: {lobbyName}");
    
    // Check if NetworkManager is properly initialized
    if (networkManager == null)
    {
        Debug.LogError("[NetworkLobbyManager] networkManager is null!");
        return;
    }
    
    CurrentLobbyId = LobbyManager.CreateLobby(lobbyName, PlayerId);
    IsHost = true;
    
    Debug.Log($"[NetworkLobbyManager] Lobby created with ID: {CurrentLobbyId}, starting host...");
    
    // Set up as host
    networkManager.StartHost();
    
    Debug.Log($"[NetworkLobbyManager] Host started for lobby: {lobbyName} (ID: {CurrentLobbyId})");
    
    // Verify the lobby exists in the manager
    LobbyManager.LogLobbyState();
}

public void JoinLobby(string lobbyId)
{
    Debug.Log($"[NetworkLobbyManager] Attempting to join lobby with ID: {lobbyId}");
    
    // Before join, check if the lobby exists
    Debug.Log("[NetworkLobbyManager] Current lobbies before join attempt:");
    LobbyManager.LogLobbyState();
    
    if (LobbyManager.JoinLobby(lobbyId, PlayerId))
    {
        CurrentLobbyId = lobbyId;
        IsHost = false;
        
        Debug.Log($"[NetworkLobbyManager] Successfully joined lobby {lobbyId}, starting client...");
        
        // Set up as client
        networkManager.StartClient();
        
        Debug.Log($"[NetworkLobbyManager] Client started for lobby: {lobbyId}");
    }
    else
    {
        Debug.LogError($"[NetworkLobbyManager] Failed to join lobby with ID: {lobbyId}");
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
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System;

/// <summary>
/// Simple component to setup a kart racing game
/// with automatic connection to server or host creation.
/// </summary>
public class SimpleKartSetup : MonoBehaviour
{
    [Header("Player Prefab")]
    [SerializeField] private GameObject playerPrefab;
    
    [Header("Network Setup")]
    [SerializeField] private bool startAsHost = true;
    [SerializeField] private bool autoConnectAsClient = true;
    [SerializeField] private string serverIP = "127.0.0.1";
    [SerializeField] private ushort serverPort = 7777;
    
    [Header("Spawn Settings")]
    [SerializeField] private float spawnHeight = 0.5f;
    [SerializeField] private float spawnRadius = 10f;
    
    void Start()
    {
        Debug.Log($"[{DateTime.Now:HH:mm:ss.fff}] SimpleKartSetup starting...");
        SetupNetworking();
    }
    
    private void SetupNetworking()
    {
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError($"[{DateTime.Now:HH:mm:ss.fff}] NetworkManager.Singleton is null!");
            return;
        }

        // Validate player prefab
        if (playerPrefab == null)
        {
            Debug.LogError($"[{DateTime.Now:HH:mm:ss.fff}] Player prefab is not assigned!");
        }
        else
        {
            // Register the player prefab
            if (!NetworkManager.Singleton.NetworkConfig.Prefabs.Contains(playerPrefab))
            {
                NetworkManager.Singleton.NetworkConfig.Prefabs.Add(new NetworkPrefab { Prefab = playerPrefab });
                Debug.Log($"[{DateTime.Now:HH:mm:ss.fff}] Registered player prefab: {playerPrefab.name}");
            }
        }
        
        // Configure transport
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        if (transport != null)
        {
            transport.ConnectionData.Address = serverIP;
            transport.ConnectionData.Port = serverPort;
            Debug.Log($"[{DateTime.Now:HH:mm:ss.fff}] Configured transport - Address: {serverIP}, Port: {serverPort}");
        }
        else
        {
            Debug.LogError($"[{DateTime.Now:HH:mm:ss.fff}] UnityTransport component not found on NetworkManager!");
        }
        
        // Register connection/disconnection events
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        NetworkManager.Singleton.OnClientStopped += OnClientStopped;
        
        Debug.Log($"[{DateTime.Now:HH:mm:ss.fff}] Network events registered");
        
        // Log network configuration
        Debug.Log($"[{DateTime.Now:HH:mm:ss.fff}] Network Configuration - StartAsHost: {startAsHost}, AutoConnectAsClient: {autoConnectAsClient}");

        // Auto start client or host
        if (startAsHost)
        {
            StartHost();
        }
        else if (autoConnectAsClient)
        {
            StartClient();
        }
    }
    
    private void OnServerStarted()
    {
        Debug.Log($"[{DateTime.Now:HH:mm:ss.fff}] Server started successfully");
    }

    private void OnClientStopped(bool isHost)
    {
        Debug.Log($"[{DateTime.Now:HH:mm:ss.fff}] Client stopped. IsHost: {isHost}");
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"[{DateTime.Now:HH:mm:ss.fff}] Client connected with ID: {clientId}, IsServer: {NetworkManager.Singleton.IsServer}, IsHost: {NetworkManager.Singleton.IsHost}");
        
        if (NetworkManager.Singleton.IsServer && playerPrefab != null)
        {
            Vector3 spawnPos = GetRandomSpawnPosition();
            Debug.Log($"[{DateTime.Now:HH:mm:ss.fff}] Preparing to spawn kart for client {clientId} at {spawnPos}");
            
            try
            {
                GameObject playerKart = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
                NetworkObject networkObject = playerKart.GetComponent<NetworkObject>();
                
                if (networkObject != null)
                {
                    networkObject.SpawnAsPlayerObject(clientId);
                    Debug.Log($"[{DateTime.Now:HH:mm:ss.fff}] Successfully spawned kart for client {clientId} at {spawnPos}");
                }
                else
                {
                    Debug.LogError($"[{DateTime.Now:HH:mm:ss.fff}] Player prefab {playerPrefab.name} has no NetworkObject component!");
                    Destroy(playerKart);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{DateTime.Now:HH:mm:ss.fff}] Failed to spawn kart for client {clientId}: {ex.Message}");
            }
        }
    }
    
    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"[{DateTime.Now:HH:mm:ss.fff}] Client disconnected with ID: {clientId}");
    }
    
    private Vector3 GetRandomSpawnPosition()
    {
        // Use UnityEngine.Random to resolve ambiguity
        Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = new Vector3(randomCircle.x, spawnHeight, randomCircle.y);
        Debug.Log($"[{DateTime.Now:HH:mm:ss.fff}] Generated random spawn position: {spawnPos}");
        return spawnPos;
    }
    
    public void StartHost()
    {
        if (NetworkManager.Singleton != null)
        {
            try
            {
                NetworkManager.Singleton.StartHost();
                Debug.Log($"[{DateTime.Now:HH:mm:ss.fff}] Started as host. Local Client ID: {NetworkManager.Singleton.LocalClientId}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{DateTime.Now:HH:mm:ss.fff}] Failed to start host: {ex.Message}");
            }
        }
        else
        {
            Debug.LogError($"[{DateTime.Now:HH:mm:ss.fff}] Cannot start host: NetworkManager is null");
        }
    }
    
    public void StartClient()
    {
        if (NetworkManager.Singleton != null)
        {
            try
            {
                NetworkManager.Singleton.StartClient();
                Debug.Log($"[{DateTime.Now:HH:mm:ss.fff}] Started as client. Connecting to {serverIP}:{serverPort}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{DateTime.Now:HH:mm:ss.fff}] Failed to start client: {ex.Message}");
            }
        }
        else
        {
            Debug.LogError($"[{DateTime.Now:HH:mm:ss.fff}] Cannot start client: NetworkManager is null");
        }
    }
    
    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
            NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
            NetworkManager.Singleton.OnClientStopped -= OnClientStopped;
            Debug.Log($"[{DateTime.Now:HH:mm:ss.fff}] Network events unregistered in OnDestroy");
        }
    }
}
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System;

/// <summary>
/// A simplified network manager that handles player spawning
/// </summary>
public class SimpleNetworkManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private float spawnHeight = 0.5f;
    
    private List<ulong> connectedClients = new List<ulong>();
    
    // Predefined spawn positions
    private Vector3[] spawnPositions = new Vector3[] 
    {
        new Vector3(-5, 0.5f, 0),
        new Vector3(0, 0.5f, 0),
        new Vector3(5, 0.5f, 0),
        new Vector3(-5, 0.5f, 5),
        new Vector3(0, 0.5f, 5),
        new Vector3(5, 0.5f, 5)
    };

    void Start()
    {
        Debug.Log($"[{DateTime.Now:HH:mm:ss.fff}] SimpleNetworkManager starting...");
        
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
            Debug.Log($"[{DateTime.Now:HH:mm:ss.fff}] Player prefab assigned: {playerPrefab.name}");
            // Register the player prefab with NetworkManager
            NetworkManager.Singleton.AddNetworkPrefab(playerPrefab);
            Debug.Log($"[{DateTime.Now:HH:mm:ss.fff}] Registered player prefab with NetworkManager");
        }
        
        // Register events
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        NetworkManager.Singleton.OnClientStopped += OnClientStopped;
        
        Debug.Log($"[{DateTime.Now:HH:mm:ss.fff}] Network events registered");
    }

    private void OnServerStarted()
    {
        Debug.Log($"[{DateTime.Now:HH:mm:ss.fff}] Server started successfully");
    }

    private void OnClientStopped(bool isHost)
    {
        Debug.Log($"[{DateTime.Now:HH:mm:ss.fff}] Client stopped. IsHost: {isHost}");
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
            NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
            NetworkManager.Singleton.OnClientStopped -= OnClientStopped;
            Debug.Log($"[{DateTime.Now:HH:mm:ss.fff}] Network events unregistered in OnDestroy");
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"[{DateTime.Now:HH:mm:ss.fff}] Client connected: {clientId}, IsServer: {NetworkManager.Singleton.IsServer}, IsHost: {NetworkManager.Singleton.IsHost}");
        
        if (NetworkManager.Singleton.IsServer)
        {
            connectedClients.Add(clientId);
            Debug.Log($"[{DateTime.Now:HH:mm:ss.fff}] Added client {clientId} to connectedClients. Total clients: {connectedClients.Count}");
            SpawnPlayerForClient(clientId);
        }
    }

    private void OnClientDisconnect(ulong clientId)
    {
        Debug.Log($"[{DateTime.Now:HH:mm:ss.fff}] Client disconnected: {clientId}, IsServer: {NetworkManager.Singleton.IsServer}");
        
        if (NetworkManager.Singleton.IsServer)
        {
            connectedClients.Remove(clientId);
            Debug.Log($"[{DateTime.Now:HH:mm:ss.fff}] Removed client {clientId}. Remaining clients: {connectedClients.Count}");
        }
    }
    
    private void SpawnPlayerForClient(ulong clientId)
    {
        if (playerPrefab == null)
        {
            Debug.LogError($"[{DateTime.Now:HH:mm:ss.fff}] Cannot spawn player for client {clientId}: Player prefab is null!");
            return;
        }
        
        // Get spawn position
        Vector3 spawnPos = GetSpawnPosition(connectedClients.Count - 1);
        Debug.Log($"[{DateTime.Now:HH:mm:ss.fff}] Spawning player for client {clientId} at position: {spawnPos}");
        
        // Spawn the player
        GameObject playerObj = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        
        NetworkObject netObj = playerObj.GetComponent<NetworkObject>();
        if (netObj != null)
        {
            try
            {
                netObj.SpawnAsPlayerObject(clientId);
                Debug.Log($"[{DateTime.Now:HH:mm:ss.fff}] Successfully spawned player for client {clientId} at {spawnPos}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{DateTime.Now:HH:mm:ss.fff}] Failed to spawn player for client {clientId}: {ex.Message}");
                Destroy(playerObj);
            }
        }
        else
        {
            Debug.LogError($"[{DateTime.Now:HH:mm:ss.fff}] Player prefab {playerPrefab.name} doesn't have a NetworkObject component!");
            Destroy(playerObj);
        }
    }
    
    private Vector3 GetSpawnPosition(int index)
    {
        if (index >= 0 && index < spawnPositions.Length)
        {
            Debug.Log($"[{DateTime.Now:HH:mm:ss.fff}] Using predefined spawn position {index}: {spawnPositions[index]}");
            return spawnPositions[index];
        }
        
        // Fall back to random position using UnityEngine.Random
        float x = UnityEngine.Random.Range(-10f, 10f);
        float z = UnityEngine.Random.Range(-10f, 10f);
        Vector3 randomPos = new Vector3(x, spawnHeight, z);
        Debug.Log($"[{DateTime.Now:HH:mm:ss.fff}] Using random spawn position: {randomPos}");
        return randomPos;
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
                Debug.Log($"[{DateTime.Now:HH:mm:ss.fff}] Started as client. Connecting to server...");
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
    
    public void StartServer()
    {
        if (NetworkManager.Singleton != null)
        {
            try
            {
                NetworkManager.Singleton.StartServer();
                Debug.Log($"[{DateTime.Now:HH:mm:ss.fff}] Started as dedicated server");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{DateTime.Now:HH:mm:ss.fff}] Failed to start server: {ex.Message}");
            }
        }
        else
        {
            Debug.LogError($"[{DateTime.Now:HH:mm:ss.fff}] Cannot start server: NetworkManager is null");
        }
    }
}
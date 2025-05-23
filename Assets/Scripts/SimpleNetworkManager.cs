using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

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
        // Register the player prefab with NetworkManager if needed
        if (playerPrefab != null && NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.AddNetworkPrefab(playerPrefab);
        }
        
        // Register events
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
        }
    }

    private void OnDestroy()
    {
        // Unregister events
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"Client connected: {clientId}");
        
        if (NetworkManager.Singleton.IsServer)
        {
            connectedClients.Add(clientId);
            SpawnPlayerForClient(clientId);
        }
    }

    private void OnClientDisconnect(ulong clientId)
    {
        Debug.Log($"Client disconnected: {clientId}");
        
        if (NetworkManager.Singleton.IsServer)
        {
            connectedClients.Remove(clientId);
        }
    }
    
    private void SpawnPlayerForClient(ulong clientId)
    {
        if (playerPrefab == null)
        {
            Debug.LogError("Player prefab is not assigned!");
            return;
        }
        
        // Get spawn position
        Vector3 spawnPos = GetSpawnPosition(connectedClients.Count - 1);
        
        // Spawn the player
        GameObject playerObj = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        
        NetworkObject netObj = playerObj.GetComponent<NetworkObject>();
        if (netObj != null)
        {
            netObj.SpawnAsPlayerObject(clientId);
            Debug.Log($"Spawned player for client {clientId}");
        }
        else
        {
            Debug.LogError("Player prefab doesn't have a NetworkObject component!");
            Destroy(playerObj);
        }
    }
    
    private Vector3 GetSpawnPosition(int index)
    {
        // Use predefined positions or random if out of range
        if (index >= 0 && index < spawnPositions.Length)
        {
            return spawnPositions[index];
        }
        
        // Fall back to random position
        float x = Random.Range(-10f, 10f);
        float z = Random.Range(-10f, 10f);
        return new Vector3(x, spawnHeight, z);
    }
    
    // Call this to start as host (both server and client)
    public void StartHost()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.StartHost();
            Debug.Log("Started as host");
        }
    }
    
    // Call this to start as client only
    public void StartClient()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.StartClient();
            Debug.Log("Started as client");
        }
    }
    
    // Call this to start as dedicated server only
    public void StartServer()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.StartServer();
            Debug.Log("Started as dedicated server");
        }
    }
}
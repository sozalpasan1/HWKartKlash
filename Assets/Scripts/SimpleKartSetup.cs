using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

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
        SetupNetworking();
    }
    
    private void SetupNetworking()
    {
        // Register the player prefab with NetworkManager if needed
        if (playerPrefab != null)
        {
            if (!NetworkManager.Singleton.NetworkConfig.Prefabs.Contains(playerPrefab))
            {
                NetworkManager.Singleton.NetworkConfig.Prefabs.Add(new NetworkPrefab { Prefab = playerPrefab });
                Debug.Log("Registered player prefab with NetworkManager");
            }
        }
        
        // Note: IP configuration is now handled by CommandLineArgs
        // We keep the serialized fields for inspector/debugging purposes only
        
        // Register connection/disconnection events
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        
        // Auto start client or host if not started by CommandLineArgs
        if (!NetworkManager.Singleton.IsListening)
        {
            if (startAsHost)
            {
                StartHost();
            }
            else if (autoConnectAsClient)
            {
                StartClient();
            }
        }
    }
    
    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"Client connected with ID: {clientId}");
        
        // If we're the server, spawn a kart for this player
        if (NetworkManager.Singleton.IsServer && playerPrefab != null)
        {
            // Get a random position within spawn radius
            Vector3 spawnPos = GetRandomSpawnPosition();
            
            // Spawn the player's kart
            GameObject playerKart = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
            NetworkObject networkObject = playerKart.GetComponent<NetworkObject>();
            
            if (networkObject != null)
            {
                networkObject.SpawnAsPlayerObject(clientId);
                Debug.Log($"Spawned kart for client {clientId} at {spawnPos}");
            }
        }
    }
    
    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"Client disconnected with ID: {clientId}");
    }
    
    private Vector3 GetRandomSpawnPosition()
    {
        // Get a random position in a circle
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        return new Vector3(randomCircle.x, spawnHeight, randomCircle.y);
    }
    
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        Debug.Log("Started as host (server + client)");
    }
    
    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        Debug.Log("Started as client, connecting to server...");
    }
    
    private void OnDestroy()
    {
        // Unregister callbacks
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }
}
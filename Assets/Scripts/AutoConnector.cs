using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Net;
using System.Linq;

/// <summary>
/// Component that automatically connects to the server.
/// Edit the CONNECTION_MODE constant to change between host and client.
/// </summary>
public class AutoConnector : MonoBehaviour
{
    // EDIT THIS LINE to change connection mode:
    // 0 = Client, 1 = Host (server+client), 2 = Server only
    private const int CONNECTION_MODE = 1;
    
    [Header("Network Settings")]
    [SerializeField] private string serverIP = "127.0.0.1";
    [SerializeField] private ushort serverPort = 7777;
    [SerializeField] private float connectionTimeout = 10f;
    
    // Connection state
    private bool isConnecting = false;
    private float connectionTimer = 0f;
    
    void Start()
    {
        // Look for NetworkManager in scene
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager not found! Add a NetworkManager to your scene.");
            return;
        }
        
        // Register network callbacks
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        
        // Display local IP addresses to help with connections
        DisplayLocalIPAddresses();
        
        // Configure the server IP and port
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        if (transport != null)
        {
            transport.ConnectionData.Address = serverIP;
            transport.ConnectionData.Port = serverPort;
            Debug.Log($"Configured transport: {serverIP}:{serverPort}");
            
            // If this is a server, make sure it's listening on all interfaces
            if (CONNECTION_MODE == 1 || CONNECTION_MODE == 2)
            {
                // These settings make the server accessible from other machines
                transport.ConnectionData.ServerListenAddress = "0.0.0.0";
                Debug.Log("Set server to listen on all network interfaces");
            }
        }
        
        // Connect based on hardcoded connection mode
        switch (CONNECTION_MODE)
        {
            case 1: // Host
                Debug.Log("Starting as host (server + client)...");
                NetworkManager.Singleton.StartHost();
                break;
                
            case 2: // Server only
                Debug.Log("Starting as dedicated server...");
                NetworkManager.Singleton.StartServer();
                break;
                
            default: // Client
                Debug.Log("Starting as client, connecting to server...");
                isConnecting = true;
                NetworkManager.Singleton.StartClient();
                break;
        }
    }
    
    private void Update()
    {
        // Monitor connection progress
        if (isConnecting)
        {
            connectionTimer += Time.deltaTime;
            
            if (connectionTimer > connectionTimeout)
            {
                Debug.LogError($"Connection timed out! Could not connect to {serverIP}:{serverPort}");
                isConnecting = false;
                
                // Try restarting the connection
                if (NetworkManager.Singleton != null && !NetworkManager.Singleton.IsConnectedClient)
                {
                    Debug.Log("Attempting to reconnect...");
                    connectionTimer = 0f;
                    NetworkManager.Singleton.StartClient();
                }
            }
        }
    }
    
    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"Successfully connected with client ID: {clientId}");
        isConnecting = false;
    }
    
    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"Client disconnected: {clientId}");
        
        if (CONNECTION_MODE == 0 && clientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.LogError($"Disconnected from server! Reason: {NetworkManager.Singleton.DisconnectReason}");
        }
    }
    
    private void DisplayLocalIPAddresses()
    {
        try
        {
            // Get host name
            string hostName = Dns.GetHostName();
            Debug.Log($"Host Name: {hostName}");
            
            // Get host entry
            var hostEntry = Dns.GetHostEntry(hostName);
            
            // Get IP addresses
            var ipAddresses = hostEntry.AddressList
                .Where(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                .Select(ip => ip.ToString())
                .ToList();
            
            Debug.Log($"Available IP addresses: {string.Join(", ", ipAddresses)}");
            
            if (CONNECTION_MODE == 1 || CONNECTION_MODE == 2)
            {
                Debug.Log("If hosting, clients should connect to one of these IPs");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error getting IP addresses: {e.Message}");
        }
    }
    
    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }
}
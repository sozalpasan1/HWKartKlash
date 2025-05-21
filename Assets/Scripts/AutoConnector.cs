using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

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
    
    void Start()
    {
        // Look for NetworkManager in scene
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager not found! Add a NetworkManager to your scene.");
            return;
        }
        
        // Configure the server IP and port
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        if (transport != null)
        {
            transport.ConnectionData.Address = serverIP;
            transport.ConnectionData.Port = serverPort;
            Debug.Log($"Configured transport: {serverIP}:{serverPort}");
        }

        Debug.Log("LMAO...");
        
        // Connect based on hardcoded connection mode
        switch (CONNECTION_MODE)
        {
            case 1: // Host
                NetworkManager.Singleton.StartHost();
                Debug.Log("Started as host (server + client)");
                break;
                
            case 2: // Server only
                NetworkManager.Singleton.StartServer();
                Debug.Log("Started as dedicated server");
                break;
                
            default: // Client
                NetworkManager.Singleton.StartClient();
                Debug.Log("Started as client, connecting to server...");
                break;
        }
    }
}
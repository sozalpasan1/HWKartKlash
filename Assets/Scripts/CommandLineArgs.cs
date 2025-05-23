using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Net;
using System.Linq;

public class CommandLineArgs : MonoBehaviour
{
    [Header("Connection Settings")]
    [SerializeField] private float connectionTimeout = 10f;
    
    [Header("Editor-Only Settings")]
    [Tooltip("What network mode to use in editor (ignored in builds)")]
    [SerializeField] private NetworkMode editorNetworkMode = NetworkMode.None;
    [SerializeField] private string editorServerIP = "127.0.0.1";
    [SerializeField] private ushort editorServerPort = 7777;
    
    // Connection state
    private bool isConnecting = false;
    private float connectionTimer = 0f;
    
    // Enum for editor network mode selection
    public enum NetworkMode
    {
        None,       // Use command line args
        Host,       // Start as host
        Client,     // Start as client
        Server      // Start as server
    }
    
    void Start()
    {
        bool isHost = false;
        bool isClient = false;
        bool isServer = false;
        string ip = "127.0.0.1";
        ushort port = 7777;

        // Force stop NetworkManager if it's already started
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
        {
            Debug.Log("NetworkManager was already started. CommandLineArgs will override existing configuration.");
            NetworkManager.Singleton.Shutdown();
        }

        // Parse command line args for builds, use editor settings in editor
        #if UNITY_EDITOR
            // In editor, use the inspector settings
            Debug.Log("Running in editor, using editor network settings");
            
            switch (editorNetworkMode)
            {
                case NetworkMode.Host:
                    isHost = true;
                    break;
                case NetworkMode.Client:
                    isClient = true;
                    break;
                case NetworkMode.Server:
                    isServer = true;
                    break;
                case NetworkMode.None:
                    // Use command line args even in editor
                    ParseCommandLineArgs(out isHost, out isClient, out isServer, out ip, out port);
                    break;
            }
            
            // Override IP/Port if editor mode is set
            if (editorNetworkMode != NetworkMode.None)
            {
                ip = editorServerIP;
                port = editorServerPort;
            }
        #else
            // In builds, always use command line args
            ParseCommandLineArgs(out isHost, out isClient, out isServer, out ip, out port);
        #endif

        // Register callbacks and display IP addresses
        RegisterCallbacks();
        DisplayLocalIPAddresses();
        
        // Configure transport if we're going to start networking
        if (isHost || isClient || isServer)
        {
            ConfigureTransport(ip, port, isHost || isServer);
            StartNetworking(isHost, isClient, isServer);
        }
    }
    
    private void ParseCommandLineArgs(out bool isHost, out bool isClient, out bool isServer, out string ip, out ushort port)
    {
        string[] args = System.Environment.GetCommandLineArgs();
        isHost = false;
        isClient = false;
        isServer = false;
        ip = "127.0.0.1";
        port = 7777;
        
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-host") isHost = true;
            if (args[i] == "-client") isClient = true;
            if (args[i] == "-server") isServer = true;
            if (args[i] == "-ip" && i + 1 < args.Length) ip = args[i + 1];
            if (args[i] == "-port" && i + 1 < args.Length) ushort.TryParse(args[i + 1], out port);
        }
    }
    
    private void RegisterCallbacks()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
    }
    
    private void ConfigureTransport(string ip, ushort port, bool isServerMode)
    {
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        if (transport != null)
        {
            transport.ConnectionData.Address = ip;
            transport.ConnectionData.Port = port;
            Debug.Log($"Configured transport: {ip}:{port}");
            
            // If this is a server or host, make sure it's listening on all interfaces
            if (isServerMode)
            {
                // These settings make the server accessible from other machines
                transport.ConnectionData.ServerListenAddress = "0.0.0.0";
                Debug.Log("Set server to listen on all network interfaces");
            }
        }
    }
    
    private void StartNetworking(bool isHost, bool isClient, bool isServer)
    {
        if (isHost) 
        {
            Debug.Log("Starting as host (server + client)...");
            NetworkManager.Singleton.StartHost();
        }
        else if (isServer)
        {
            Debug.Log("Starting as dedicated server...");
            NetworkManager.Singleton.StartServer();
        }
        else if (isClient) 
        {
            Debug.Log("Starting as client, connecting to server...");
            isConnecting = true;
            NetworkManager.Singleton.StartClient();
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
                Debug.LogError($"Connection timed out! Could not connect to server");
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
        
        if (clientId == NetworkManager.Singleton.LocalClientId)
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
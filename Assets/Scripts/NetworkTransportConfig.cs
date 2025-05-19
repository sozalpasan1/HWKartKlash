using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkTransportConfig : MonoBehaviour
{
    [SerializeField] private ushort basePort = 7777;
    [SerializeField] private string connectAddress = "127.0.0.1"; // Default for local testing
    
    private void Awake()
    {
        var networkManager = GetComponent<NetworkManager>();
        if (networkManager == null) return;
        
        var transport = networkManager.GetComponent<UnityTransport>();
        if (transport == null) return;
        
        transport.ConnectionData.Address = connectAddress;
        
        #if UNITY_EDITOR
        // Use different ports for original and clone
        if (ParrelSync.ClonesManager.IsClone())
        {
            // Calculate a port offset for the clone
            string projectPath = Application.dataPath;
            int cloneOffset = 1;
            
            // Try to extract a number from the clone path if possible
            if (projectPath.Contains("_clone"))
            {
                int lastUnderscore = projectPath.LastIndexOf('_');
                string potentialNumber = projectPath.Substring(lastUnderscore + 1);
                if (int.TryParse(potentialNumber, out int result))
                {
                    cloneOffset = result;
                }
            }
            
            // Set a different port for the clone
            ushort clonePort = (ushort)(basePort + cloneOffset);
            transport.ConnectionData.Port = basePort; // Connect to main project port
            Debug.Log($"Clone using port: {clonePort}, connecting to: {basePort}");
        }
        else
        {
            // Original project uses the base port
            transport.ConnectionData.Port = basePort;
            Debug.Log($"Original project using port: {basePort}");
        }
        #else
        transport.ConnectionData.Port = basePort;
        #endif
    }
    void Start()
    {
        // Test the file system access at startup
        Debug.Log("[NetworkTransportConfig] Testing lobby file access...");
        LobbySync.VerifyFileAccess();
    }
}
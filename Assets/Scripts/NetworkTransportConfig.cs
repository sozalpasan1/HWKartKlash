using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkTransportConfig : MonoBehaviour
{
    [SerializeField] private ushort hostPort = 7777;
    [SerializeField] private string connectAddress = "127.0.0.1"; // Default for local testing
    
    private void Awake()
    {
        var networkManager = GetComponent<NetworkManager>();
        if (networkManager == null) return;
        
        var transport = networkManager.GetComponent<UnityTransport>();
        if (transport == null) return;
        
        // Basic configuration - always connect to localhost
        transport.ConnectionData.Address = connectAddress;
        
        #if UNITY_EDITOR
        // Simple approach: use different ports for host and clones
        if (ParrelSync.ClonesManager.IsClone())
        {
            // Clones always try to join the main project
            transport.ConnectionData.Port = hostPort;
            Debug.Log("Clone configured to connect to host on port: " + hostPort);
        }
        else
        {
            // Main project always hosts
            transport.ConnectionData.Port = hostPort;
            Debug.Log("Original project configured to host on port: " + hostPort);
        }
        #else
        transport.ConnectionData.Port = hostPort;
        #endif
    }
}
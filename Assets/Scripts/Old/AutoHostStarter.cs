using UnityEngine;
using Unity.Netcode;

/// <summary>
/// Automatically starts a host when the scene loads.
/// Attach this to an object in the scene to automatically start as a host.
/// </summary>
public class AutoHostStarter : MonoBehaviour
{
    [SerializeField] private bool autoStart = true;
    [SerializeField] private GameObject networkManagerPrefab;
    
    void Start()
    {
        if (autoStart)
        {
            if (NetworkManager.Singleton == null && networkManagerPrefab != null)
            {
                // Instantiate the Network Manager if it doesn't exist
                Instantiate(networkManagerPrefab);
            }
            
            if (NetworkManager.Singleton != null)
            {
                Debug.Log("Starting as host automatically");
                NetworkManager.Singleton.StartHost();
            }
            else
            {
                Debug.LogError("Cannot start host - NetworkManager not found!");
            }
        }
    }
}
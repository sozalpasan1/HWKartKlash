using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkManager))]
public class PlayerSpawner : MonoBehaviour
{
    [Header("Drag in your two prefabs here")]
    public GameObject capsulePrefab;   // assign your capsule prefab asset
    public GameObject cubePrefab;      // assign your cube prefab asset

    [Tooltip("Toggle before you hit Host/Client")]
    public bool spawnCube = false;

    private void Awake()
    {
        var nm = NetworkManager.Singleton;

        // 1) Turn on client‑side auto‑spawn
        nm.NetworkConfig.AutoSpawnPlayerPrefabClientSide = true;

        // 2) Hook the delegate that picks which prefab to use
        nm.OnFetchLocalPlayerPrefabToSpawn = () =>
        {
            return spawnCube ? cubePrefab : capsulePrefab;
        };

        // 3) Register both prefabs
        nm.AddNetworkPrefab(capsulePrefab);
        nm.AddNetworkPrefab(cubePrefab);
    }

    // These are what your UI buttons should call:
    public void Host()
    {
        if (!NetworkManager.Singleton.IsListening)
            NetworkManager.Singleton.StartHost();
    }

    public void Client()
    {
        if (!NetworkManager.Singleton.IsListening)
            NetworkManager.Singleton.StartClient();
    }
}

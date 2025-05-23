using UnityEngine;
using Unity.Netcode;

/// <summary>
/// Registers network prefabs with the NetworkManager. Handles setup of the default NetworkPrefabs.
/// </summary>
public class NetworkPrefabRegistrator : MonoBehaviour
{
    [SerializeField] private GameObject kartPrefab;
    
    void Awake()
    {
        if (NetworkManager.Singleton != null && kartPrefab != null)
        {
            RegisterNetworkPrefab(kartPrefab);
            Debug.Log("Registered kart prefab with NetworkManager");
        }
    }
    
    private void RegisterNetworkPrefab(GameObject prefab)
    {
        if (prefab.TryGetComponent(out NetworkObject networkObject))
        {
            NetworkManager.Singleton.AddNetworkPrefab(prefab);
        }
        else
        {
            Debug.LogError($"Prefab {prefab.name} does not have a NetworkObject component!");
        }
    }
}
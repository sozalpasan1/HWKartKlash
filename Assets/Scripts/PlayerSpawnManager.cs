using Unity.Netcode;
using UnityEngine;

public class PlayerSpawnManager : NetworkBehaviour
{
    [SerializeField] private Vector3[] spawnPoints = new Vector3[]
    {
        new Vector3(-5f, 1f, 0f),
        new Vector3(0f, 1f, 0f),
        new Vector3(5f, 1f, 0f),
        new Vector3(-5f, 1f, 5f),
        new Vector3(0f, 1f, 5f),
        new Vector3(5f, 1f, 5f),
        new Vector3(-5f, 1f, -5f),
        new Vector3(0f, 1f, -5f)
    };
    
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += SpawnPlayerForClient;
        }
    }
    
    private void SpawnPlayerForClient(ulong clientId)
    {
        // Fix the ambiguous operator error by explicitly converting types
        int spawnIndex = (int)(clientId % (ulong)spawnPoints.Length);
        // Alternative fix: int spawnIndex = (int)clientId % spawnPoints.Length;
        
        Vector3 spawnPos = spawnPoints[spawnIndex];
        
        // The player is already spawned by default in NetworkManager,
        // but we might want to position it properly
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out NetworkClient client))
        {
            if (client.PlayerObject != null)
            {
                client.PlayerObject.transform.position = spawnPos;
            }
        }
    }
    
    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= SpawnPlayerForClient;
        }
    }
}
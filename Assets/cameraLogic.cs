using Unity.Netcode;
using UnityEngine;

public class PlayerCameraManager : NetworkBehaviour
{
    public GameObject cameraPrefab;
    private GameObject playerCamera;
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        // Only create a camera for the local player
        if (IsOwner)
        {
            // Instantiate camera and attach it to the player
            playerCamera = Instantiate(cameraPrefab, transform);
            playerCamera.transform.localPosition = new Vector3(0, 2, -5); // Adjust as needed
        }
    }
}
using UnityEngine;
using Unity.Netcode;

/// <summary>
/// Ensures that the camera attached to a player's object is only active 
/// for the local player, and deactivates scene cameras when a player spawns.
/// </summary>
public class PlayerCameraSetup : NetworkBehaviour
{
    [Tooltip("The camera attached to this player object")]
    [SerializeField] private Camera playerCamera;
    
    [Tooltip("Should the main scene camera be disabled when this player spawns?")]
    [SerializeField] private bool disableSceneCamera = true;
    
    // Called after the GameObject is instantiated
    private void Start()
    {
        // Force-disable all cameras immediately
        DisableAllPlayerCameras();
        
        // If we're not in a networked context yet, wait for network spawn
        if (!IsSpawned)
        {
            Debug.Log($"Player not spawned yet. Will setup camera on spawn. IsOwner: {IsOwner}, ClientID: {OwnerClientId}");
            return;
        }
        
        // If we're already spawned, set up the camera immediately
        SetupCamera();
    }
    
    // Called when the object is spawned on the network
    public override void OnNetworkSpawn()
    {
        Debug.Log($"OnNetworkSpawn called. IsOwner: {IsOwner}, IsLocalPlayer: {IsLocalPlayer}, ClientID: {OwnerClientId}, LocalClientID: {NetworkManager.Singleton.LocalClientId}");
        
        // Setup camera configuration
        SetupCamera();
    }
    
    private void SetupCamera()
    {
        if (playerCamera == null)
        {
            Debug.LogError("Player camera reference not set in PlayerCameraSetup");
            return;
        }
        
        // Explicitly check if this is the local player object
        bool isLocalPlayer = NetworkManager.Singleton.LocalClientId == OwnerClientId;
        
        Debug.Log($"Setting up camera for player. IsLocalPlayer: {isLocalPlayer}, LocalClientID: {NetworkManager.Singleton.LocalClientId}, OwnerClientID: {OwnerClientId}");
        
        if (isLocalPlayer)
        {
            // This is our local player, enable its camera
            Debug.Log($"Activating camera for local player {OwnerClientId}");
            playerCamera.gameObject.SetActive(true);
            
            // Disable the main scene camera if requested
            if (disableSceneCamera)
            {
                Camera sceneCamera = Camera.main;
                if (sceneCamera != null && sceneCamera != playerCamera)
                {
                    Debug.Log("Disabling main scene camera");
                    sceneCamera.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            // This is not our local player, make sure its camera is disabled
            Debug.Log($"Disabling camera for non-local player {OwnerClientId}");
            playerCamera.gameObject.SetActive(false);
        }
    }
    
    private void DisableAllPlayerCameras()
    {
        if (playerCamera != null)
        {
            Debug.Log("Initially disabling player camera");
            playerCamera.gameObject.SetActive(false);
        }
    }
    
    public override void OnNetworkDespawn()
    {
        Debug.Log($"OnNetworkDespawn called. IsOwner: {IsOwner}, ClientID: {OwnerClientId}");
        
        // If we were the local player, try to reactivate the scene camera
        bool isLocalPlayer = NetworkManager.Singleton.LocalClientId == OwnerClientId;
        
        if (isLocalPlayer && disableSceneCamera)
        {
            Camera sceneCamera = Camera.main;
            if (sceneCamera != null && sceneCamera != playerCamera)
            {
                Debug.Log("Reactivating main scene camera");
                sceneCamera.gameObject.SetActive(true);
            }
        }
    }
    
    // For additional safety, check periodically
    private void Update()
    {
        if (IsSpawned && playerCamera != null)
        {
            bool isLocalPlayer = NetworkManager.Singleton.LocalClientId == OwnerClientId;
            bool cameraState = playerCamera.gameObject.activeSelf;
            
            // Correct the camera state if it's wrong
            if (isLocalPlayer && !cameraState)
            {
                Debug.LogWarning($"Correcting camera state: Enabling local player camera that was disabled");
                playerCamera.gameObject.SetActive(true);
            }
            else if (!isLocalPlayer && cameraState)
            {
                Debug.LogWarning($"Correcting camera state: Disabling non-local player camera that was enabled");
                playerCamera.gameObject.SetActive(false);
            }
        }
    }
}
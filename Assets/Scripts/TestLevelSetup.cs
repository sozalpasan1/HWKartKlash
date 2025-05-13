using UnityEngine;

public class TestLevelSetup : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject checkpointPrefab;
    [SerializeField] private GameObject platformPrefab;
    
    [Header("Setup")]
    [SerializeField] private bool createLevelOnStart = true;
    
    private void Start()
    {
        if (createLevelOnStart)
        {
            CreateLevel();
        }
    }
    
    [ContextMenu("Create Test Level")]
    public void CreateLevel()
    {
        // Create main platform
        GameObject mainPlatform = CreatePlatform(new Vector3(0, 0, 0), new Vector3(10, 1, 10));
        
        // Create secondary platform with checkpoint
        GameObject secondaryPlatform = CreatePlatform(new Vector3(15, 3, 0), new Vector3(5, 1, 5));
        GameObject checkpoint1 = CreateCheckpoint(new Vector3(15, 4, 0));
        
        // Create a third platform with another checkpoint
        GameObject thirdPlatform = CreatePlatform(new Vector3(30, 5, 0), new Vector3(5, 1, 5));
        GameObject checkpoint2 = CreateCheckpoint(new Vector3(30, 6, 0));
        
        // Create player and CheckpointManager if they don't exist
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null && playerPrefab != null)
        {
            player = Instantiate(playerPrefab, new Vector3(0, 2, 0), Quaternion.identity);
            player.tag = "Player";
            
            // Setup player components if not already set up
            if (player.GetComponent<CharacterController>() == null)
            {
                player.AddComponent<CharacterController>();
            }
            
            if (player.GetComponent<PlayerMovement>() == null)
            {
                player.AddComponent<PlayerMovement>();
            }
            
            if (player.GetComponent<PlayerRespawn>() == null)
            {
                PlayerRespawn respawn = player.AddComponent<PlayerRespawn>();
                respawn.deathY = -10f;
            }
            
            // Add camera if not present
            Camera playerCamera = player.GetComponentInChildren<Camera>();
            if (playerCamera == null)
            {
                GameObject cameraObj = new GameObject("PlayerCamera");
                cameraObj.transform.parent = player.transform;
                cameraObj.transform.localPosition = new Vector3(0, 0.7f, 0);
                cameraObj.AddComponent<Camera>();
                cameraObj.AddComponent<CameraController>();
            }
        }
        
        // Create CheckpointManager if it doesn't exist
        CheckpointManager manager = FindObjectOfType<CheckpointManager>();
        if (manager == null)
        {
            GameObject managerObj = new GameObject("CheckpointManager");
            manager = managerObj.AddComponent<CheckpointManager>();
            
            // Set default spawn point to first checkpoint or player position
            if (checkpoint1 != null)
            {
                manager.defaultSpawnPoint = checkpoint1.transform;
            }
            else if (player != null)
            {
                manager.defaultSpawnPoint = player.transform;
            }
        }
    }
    
    private GameObject CreatePlatform(Vector3 position, Vector3 scale)
    {
        GameObject platform;
        
        if (platformPrefab != null)
        {
            platform = Instantiate(platformPrefab, position, Quaternion.identity);
            platform.transform.localScale = scale;
        }
        else
        {
            platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
            platform.transform.position = position;
            platform.transform.localScale = scale;
            
            // Add to ground layer for ground checks
            platform.layer = 6; // Layer 6 is "Ground" by default
        }
        
        return platform;
    }
    
    private GameObject CreateCheckpoint(Vector3 position)
    {
        GameObject checkpoint;
        
        if (checkpointPrefab != null)
        {
            checkpoint = Instantiate(checkpointPrefab, position, Quaternion.identity);
        }
        else
        {
            checkpoint = new GameObject("Checkpoint");
            checkpoint.transform.position = position;
            
            // Create visual representation
            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            visual.transform.parent = checkpoint.transform;
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localScale = new Vector3(1, 0.1f, 1);
            
            // Add checkpoint component
            Checkpoint checkpointComponent = checkpoint.AddComponent<Checkpoint>();
            checkpointComponent.checkpointRenderer = visual.GetComponent<Renderer>();
            
            // Add collider
            SphereCollider collider = checkpoint.AddComponent<SphereCollider>();
            collider.isTrigger = true;
            collider.radius = 1.5f;
        }
        
        return checkpoint;
    }
}
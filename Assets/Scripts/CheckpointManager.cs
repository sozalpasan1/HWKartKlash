using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }
    
    [SerializeField] public Transform defaultSpawnPoint; // Changed from private to public
    private Transform currentCheckpoint;
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Set default spawn point as the initial checkpoint
        if (defaultSpawnPoint != null)
        {
            currentCheckpoint = defaultSpawnPoint;
        }
        else
        {
            Debug.LogWarning("No default spawn point assigned to CheckpointManager!");
        }
    }
    
    public void SetCheckpoint(Transform newCheckpoint)
    {
        currentCheckpoint = newCheckpoint;
        Debug.Log("Checkpoint set at: " + newCheckpoint.position);
    }
    
    public Vector3 GetRespawnPosition()
    {
        if (currentCheckpoint != null)
        {
            return currentCheckpoint.position;
        }
        else if (defaultSpawnPoint != null)
        {
            return defaultSpawnPoint.position;
        }
        else
        {
            Debug.LogError("No respawn position available!");
            return Vector3.zero;
        }
    }
    
    public Quaternion GetRespawnRotation()
    {
        if (currentCheckpoint != null)
        {
            return currentCheckpoint.rotation;
        }
        else if (defaultSpawnPoint != null)
        {
            return defaultSpawnPoint.rotation;
        }
        else
        {
            return Quaternion.identity;
        }
    }
}
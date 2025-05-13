using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] public float deathY = -10f; // Changed from private to public
    [SerializeField] private float respawnDelay = 1f;
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private GameObject respawnEffect;
    
    private CharacterController characterController;
    private PlayerMovement playerMovement;
    private bool isDead = false;
    
    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerMovement = GetComponent<PlayerMovement>();
        
        if (characterController == null)
        {
            Debug.LogWarning("No CharacterController attached to player with PlayerRespawn script");
        }
    }
    
    private void Update()
    {
        // Check if player fell off the map
        if (!isDead && transform.position.y < deathY)
        {
            Die();
        }
    }
    
    public void Die()
    {
        if (isDead) return;
        
        isDead = true;
        
        // Disable character controller and movement to prevent further movement
        if (characterController != null)
        {
            characterController.enabled = false;
        }
        
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }
        
        // Show death effect
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }
        
        // Respawn after delay
        Invoke("Respawn", respawnDelay);
    }
    
    private void Respawn()
    {
        if (CheckpointManager.Instance == null)
        {
            Debug.LogError("No CheckpointManager found in the scene!");
            return;
        }
        
        // Get respawn position and rotation from CheckpointManager
        Vector3 respawnPos = CheckpointManager.Instance.GetRespawnPosition();
        Quaternion respawnRot = CheckpointManager.Instance.GetRespawnRotation();
        
        // Disable character controller to teleport
        if (characterController != null)
        {
            characterController.enabled = false;
        }
        
        // Teleport to respawn position
        transform.position = respawnPos;
        transform.rotation = respawnRot;
        
        // Show respawn effect
        if (respawnEffect != null)
        {
            Instantiate(respawnEffect, transform.position, Quaternion.identity);
        }
        
        // Re-enable character controller and movement
        if (characterController != null)
        {
            characterController.enabled = true;
        }
        
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }
        
        isDead = false;
    }
}
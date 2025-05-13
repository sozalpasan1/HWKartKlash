using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private bool activateOnce = true;
    [SerializeField] private ParticleSystem activationEffect;
    [SerializeField] private AudioSource activationSound;
    [SerializeField] public Renderer checkpointRenderer; // Changed from private to public
    [SerializeField] private Color activatedColor = Color.green;
    [SerializeField] private Color deactivatedColor = Color.red;
    
    private bool hasBeenActivated = false;
    private Material checkpointMaterial;
    
    private void Start()
    {
        if (checkpointRenderer != null)
        {
            checkpointMaterial = checkpointRenderer.material;
            checkpointMaterial.color = deactivatedColor;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (activateOnce && hasBeenActivated)
                return;
                
            ActivateCheckpoint();
        }
    }
    
    private void ActivateCheckpoint()
    {
        // Set this as the current checkpoint
        CheckpointManager.Instance.SetCheckpoint(transform);
        
        // Change color to show activation
        if (checkpointMaterial != null)
        {
            checkpointMaterial.color = activatedColor;
        }
        
        // Play effects
        if (activationEffect != null)
        {
            activationEffect.Play();
        }
        
        if (activationSound != null)
        {
            activationSound.Play();
        }
        
        hasBeenActivated = true;
    }
}
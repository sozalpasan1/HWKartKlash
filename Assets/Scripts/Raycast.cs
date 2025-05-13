using UnityEngine;

public class SurfaceDetector : MonoBehaviour
{
    [Header("Raycast Settings")]
    public float raycastDistance = 1.0f;
    public float raycastOffset = 0.1f;
    public LayerMask surfaceLayerMask = -1; // Default to all layers
    
    [Header("Debug")]
    public bool showDebugRay = true;
    public Color debugRayColor = Color.green;
    
    // Current surface information
    private string currentSurfaceTag = "Road";
    private RaycastHit lastHit;
    private bool isGrounded = true;
    
    // Event for surface change
    public delegate void SurfaceChangeEvent(string newSurfaceTag);
    public event SurfaceChangeEvent OnSurfaceChange;
    
    void Update()
    {
        DetectSurface();
        
        // Debug visualization
        if (showDebugRay)
        {
            Debug.DrawRay(transform.position + Vector3.up * raycastOffset, 
                          Vector3.down * raycastDistance, 
                          isGrounded ? debugRayColor : Color.red);
        }
    }
    
    void DetectSurface()
    {
        // Cast a ray downward from slightly above the object
        Vector3 rayOrigin = transform.position + Vector3.up * raycastOffset;
        Ray ray = new Ray(rayOrigin, Vector3.down);
        
        // Check if we hit something
        if (Physics.Raycast(ray, out lastHit, raycastDistance, surfaceLayerMask))
        {
            isGrounded = true;
            
            // Get the tag of the surface we hit
            string surfaceTag = lastHit.collider.tag;
            
            // If the surface tag is empty, use the default
            if (string.IsNullOrEmpty(surfaceTag))
            {
                surfaceTag = "Road";
            }
            
            // Only trigger event if the surface has changed
            if (surfaceTag != currentSurfaceTag)
            {
                currentSurfaceTag = surfaceTag;
                
                // Trigger the surface change event
                if (OnSurfaceChange != null)
                {
                    OnSurfaceChange(currentSurfaceTag);
                }
            }
        }
        else
        {
            isGrounded = false;
        }
    }
    
    // Public methods to get surface information
    public string GetCurrentSurfaceTag()
    {
        return currentSurfaceTag;
    }
    
    public bool IsGrounded()
    {
        return isGrounded;
    }
    
    public RaycastHit GetLastHit()
    {
        return lastHit;
    }
}
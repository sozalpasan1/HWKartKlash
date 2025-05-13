using UnityEngine;

public class Traction : MonoBehaviour
{
    [System.Serializable]
    public class SurfaceSettings
    {
        public string surfaceTag;
        public float accelerationMultiplier = 1.0f;
        public float decelerationMultiplier = 1.0f;
        public float turnMultiplier = 1.0f;
    }

    [Tooltip("List of different surfaces and their traction properties")]
    public SurfaceSettings[] surfaceSettings;

    [Tooltip("Default surface settings when no specific surface is detected")]
    public SurfaceSettings defaultSurface;

    [Tooltip("How far down to check for the ground surface")]
    public float raycastDistance = 1.0f;

    [Tooltip("Which layers to consider as ground")]
    public LayerMask groundLayers;

    private SurfaceSettings currentSurface;
    private string currentSurfaceTag = "";

    private void Start()
    {
        // Initialize with default surface
        currentSurface = defaultSurface;
        
        // Create default surface if none exists
        if (defaultSurface == null)
        {
            defaultSurface = new SurfaceSettings
            {
                surfaceTag = "Default",
                accelerationMultiplier = 1.0f,
                decelerationMultiplier = 1.0f,
                turnMultiplier = 1.0f
            };
        }
    }

    private void Update()
    {
        DetectSurface();
    }

    private void DetectSurface()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, raycastDistance, groundLayers))
        {
            // Get the tag of the surface we're on
            string surfaceTag = hit.collider.tag;
            
            // Only update if the surface has changed
            if (surfaceTag != currentSurfaceTag)
            {
                currentSurfaceTag = surfaceTag;
                UpdateSurfaceSettings(surfaceTag);
            }
        }
        else
        {
            // If not on any surface, use default
            if (currentSurfaceTag != "")
            {
                currentSurfaceTag = "";
                currentSurface = defaultSurface;
            }
        }
    }

    // Returns the current surface tag (useful for debugging)
    public string GetCurrentSurfaceTag()
    {
        return currentSurfaceTag;
    }

    private void UpdateSurfaceSettings(string surfaceTag)
    {
        // Look for a matching surface in our settings
        foreach (SurfaceSettings surface in surfaceSettings)
        {
            if (surface.surfaceTag == surfaceTag)
            {
                currentSurface = surface;
                return;
            }
        }
        
        // If no matching surface found, use default
        currentSurface = defaultSurface;
    }

    // Public methods to get the current traction multipliers
    public float GetAccelerationMultiplier()
    {
        return currentSurface.accelerationMultiplier;
    }

    public float GetDecelerationMultiplier()
    {
        return currentSurface.decelerationMultiplier;
    }

    public float GetTurnMultiplier()
    {
        return currentSurface.turnMultiplier;
    }
}
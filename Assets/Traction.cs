// In your kart movement/controller script
private Traction tractionSystem;

void Start()
{
    tractionSystem = GetComponent<Traction>();
}

void Update()
{
    // When applying acceleration
    float accelerationMultiplier = tractionSystem.GetAccelerationMultiplier();
    float actualAcceleration = baseAcceleration * accelerationMultiplier;
    
    // When applying deceleration/braking
    float decelerationMultiplier = tractionSystem.GetDecelerationMultiplier();
    float actualDeceleration = baseDeceleration * decelerationMultiplier;
    
    // When turning
    float turnMultiplier = tractionSystem.GetTurnMultiplier();
    float actualTurnRate = baseTurnRate * turnMultiplier;
    
    // Apply these values to your movement calculations
}

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
    public float raycastDistance = 0.5f;

    [Tooltip("Which layers to consider as ground")]
    public LayerMask groundLayers;

    private SurfaceSettings currentSurface;
    private string currentSurfaceTag = "";

    private void Start()
    {
        // Initialize with default surface
        currentSurface = defaultSurface;
    }

    private void Update()
    {
        DetectSurface();
    }

    private void DetectSurface()
    {
        RaycastHit hit;
        // Cast a ray downward from the kart to detect the ground
        if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastDistance, groundLayers))
        {
            // Check if the hit object has a tag that matches any of our defined surfaces
            string surfaceTag = hit.collider.tag;
            
            // Only update if the surface has changed
            if (surfaceTag != currentSurfaceTag)
            {
                currentSurfaceTag = surfaceTag;
                UpdateSurfaceSettings(surfaceTag);
                
                // Debug output to see what surface we're on
                Debug.Log("Surface detected: " + surfaceTag);
            }
        }
        else
        {
            // If no surface detected (kart is airborne), use default settings
            if (currentSurfaceTag != "")
            {
                currentSurfaceTag = "";
                currentSurface = defaultSurface;
                Debug.Log("No surface detected, using default");
            }
        }
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
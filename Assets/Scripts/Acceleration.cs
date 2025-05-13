using UnityEngine;

public class Acceleration : MonoBehaviour
{
    [System.Serializable]
    public class SurfaceProperties
    {
        public string surfaceTag = "Road";
        public float accelerationMultiplier = 1.0f;
        public float decelerationMultiplier = 1.0f; // Friction effect
        public float maxSpeedMultiplier = 1.0f;     // Max speed modifier
        public float turnMultiplier = 1.0f;         // Turn radius modifier
    }
    
    // Default surface - for simplicity we'll just use one as requested
    public SurfaceProperties defaultSurface = new SurfaceProperties
    {
        surfaceTag = "Road",
        accelerationMultiplier = 1.0f,
        decelerationMultiplier = 1.0f,
        maxSpeedMultiplier = 1.0f,
        turnMultiplier = 1.0f
    };
    
    // Base movement settings
    [Header("Base Movement Settings")]
    public float baseAccelerationRate = 2.0f;
    public float baseDecelerationRate = 1.0f;
    public float baseMaxSpeed = 20.0f;
    public float baseTurnRadius = 5.0f;
    
    // Current state
    private float currentSpeed = 0.0f;
    private string currentSurfaceTag = "Road";
    
    // Public methods to get the current multipliers
    public float GetAccelerationMultiplier()
    {
        return defaultSurface.accelerationMultiplier;
    }
    
    public float GetDecelerationMultiplier()
    {
        return defaultSurface.decelerationMultiplier;
    }
    
    public float GetMaxSpeedMultiplier()
    {
        return defaultSurface.maxSpeedMultiplier;
    }
    
    public float GetTurnMultiplier()
    {
        return defaultSurface.turnMultiplier;
    }
    
    // Get the current max speed based on surface
    public float GetMaxSpeed()
    {
        return baseMaxSpeed * GetMaxSpeedMultiplier();
    }
    
    // Get the current turn radius based on surface
    public float GetTurnRadius()
    {
        return baseTurnRadius * GetTurnMultiplier();
    }
    
    // Get the current surface tag
    public string GetCurrentSurfaceTag()
    {
        return currentSurfaceTag;
    }
}
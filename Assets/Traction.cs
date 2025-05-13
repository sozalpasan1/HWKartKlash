using UnityEngine;

public class Traction : MonoBehaviour
{
    // Movement settings
    public float maxSpeed = 20.0f;
    public float maxReverseSpeed = 10.0f;
    public float acceleration = 40.0f;       // Increased for better responsiveness
    public float braking = 10.0f;
    public float turnSpeed = 80.0f;
    public float turnSpeedReduction = 0.7f;
    public float gravity = 20.0f;
   
    // Current motion state
    private float currentSpeed = 0.0f;
   
    // Components
    private Rigidbody rb;
    private Traction tractionSystem;

    // Direction setting - change this to match your kart's orientation
    public enum KartForwardDirection { Forward, Right, Left, Back }
    public KartForwardDirection kartForward = KartForwardDirection.Left; // Set this in the Inspector
   
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
       
        rb.mass = 50;  // Lighter mass for better acceleration
        rb.linearDamping = 0;
        rb.angularDamping = 1;
        rb.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        
        // Get the traction system component
        tractionSystem = GetComponent<Traction>();
        if (tractionSystem == null)
        {
            Debug.LogWarning("Traction component not found on kart. Adding one with default settings.");
            tractionSystem = gameObject.AddComponent<Traction>();
        }
    }
   
    void FixedUpdate()
    {
        float accelerationInput = Input.GetAxis("Vertical");
        float steeringInput = Input.GetAxis("Horizontal");
        
        // Get traction multipliers
        float accelerationMultiplier = tractionSystem != null ? tractionSystem.GetAccelerationMultiplier() : 1.0f;
        float decelerationMultiplier = tractionSystem != null ? tractionSystem.GetDecelerationMultiplier() : 1.0f;
        float turnMultiplier = tractionSystem != null ? tractionSystem.GetTurnMultiplier() : 1.0f;
       
        // Handle acceleration & braking with traction applied
        if (accelerationInput > 0) // Accelerating
        {
            currentSpeed += accelerationInput * acceleration * accelerationMultiplier * Time.fixedDeltaTime;
            currentSpeed = Mathf.Min(currentSpeed, maxSpeed);
        }
        else if (accelerationInput < 0) // Braking or reversing
        {
            if (currentSpeed > 0)
            {
                currentSpeed += accelerationInput * braking * decelerationMultiplier * Time.fixedDeltaTime;
            }
            else
            {
                currentSpeed += accelerationInput * acceleration * accelerationMultiplier * Time.fixedDeltaTime;
                currentSpeed = Mathf.Max(currentSpeed, -maxReverseSpeed);
            }
        }
        else // No input - slow down naturally
        {
            if (currentSpeed > 0)
            {
                currentSpeed -= braking * 0.5f * decelerationMultiplier * Time.fixedDeltaTime;
                currentSpeed = Mathf.Max(currentSpeed, 0);
            }
            else if (currentSpeed < 0)
            {
                currentSpeed += braking * 0.5f * decelerationMultiplier * Time.fixedDeltaTime;
                currentSpeed = Mathf.Min(currentSpeed, 0);
            }
        }
       
        float speedFactor = Mathf.Abs(currentSpeed) / maxSpeed;
        float turnAmount = steeringInput * turnSpeed * turnMultiplier * (1.0f - (speedFactor * turnSpeedReduction)) * Time.fixedDeltaTime;
       
        if (Mathf.Abs(currentSpeed) > 0.1f)
        {
            if (currentSpeed < 0)
                turnAmount = -turnAmount;
               
            transform.Rotate(0, turnAmount, 0);
        }
       
        // IMPORTANT: Get the correct movement direction based on kart orientation
        Vector3 movement = GetMovementDirection() * currentSpeed;
       
        rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);
        rb.AddForce(Vector3.down * gravity);
       
        Debug.Log("Speed: " + currentSpeed + " | Movement Vector: " + movement + " | Surface: " + 
                 (tractionSystem != null ? tractionSystem.GetCurrentSurfaceTag() : "Unknown"));
    }
   
    // This function returns the correct direction vector based on the kart's orientation
    private Vector3 GetMovementDirection()
    {
        switch (kartForward)
        {
            case KartForwardDirection.Forward:
                return transform.forward;
            case KartForwardDirection.Right:
                return transform.right;
            case KartForwardDirection.Left:
                return -transform.right;
            case KartForwardDirection.Back:
                return -transform.forward;
            default:
                return transform.forward;
        }
    }
}
    // Surface types and their properties
    [System.Serializable]
    public class SurfaceType
    {
        public string surfaceTag = "Road";
        public float accelerationMultiplier = 1.0f;
        public float decelerationMultiplier = 1.0f;
        public float turnMultiplier = 1.0f;
    }
    
    // Default surface types
    public SurfaceType[] surfaceTypes = new SurfaceType[]
    {
        new SurfaceType { surfaceTag = "Road", accelerationMultiplier = 1.0f, decelerationMultiplier = 1.0f, turnMultiplier = 1.0f },
        new SurfaceType { surfaceTag = "Grass", accelerationMultiplier = 0.5f, decelerationMultiplier = 0.7f, turnMultiplier = 0.6f },
        new SurfaceType { surfaceTag = "Sand", accelerationMultiplier = 0.4f, decelerationMultiplier = 0.6f, turnMultiplier = 0.5f },
        new SurfaceType { surfaceTag = "Ice", accelerationMultiplier = 0.3f, decelerationMultiplier = 0.1f, turnMultiplier = 0.3f }
    };
    
    // Current surface properties
    private SurfaceType currentSurface;
    private string currentSurfaceTag = "Road";
    
    void Start()
    {
        // Set default surface
        currentSurface = GetSurfaceByTag(currentSurfaceTag);
    }
    
    void Update()
    {
        // Check what surface we're on using raycasting
        CheckSurface();
    }
    
    private void CheckSurface()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out hit, 1.0f))
        {
            // Get the tag of the surface we hit
            string surfaceTag = hit.collider.tag;
            
            // Only update if the surface has changed
            if (surfaceTag != currentSurfaceTag)
            {
                currentSurfaceTag = surfaceTag;
                currentSurface = GetSurfaceByTag(surfaceTag);
            }
        }
    }
    
    private SurfaceType GetSurfaceByTag(string tag)
    {
        // Find the surface type that matches the tag
        foreach (SurfaceType surface in surfaceTypes)
        {
            if (surface.surfaceTag == tag)
                return surface;
        }
        
        // Return default surface if no match found
        return surfaceTypes[0];
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
    
    // Add this new method to get the current surface tag for debugging
    public string GetCurrentSurfaceTag()
    {
        return currentSurfaceTag;
    }
}

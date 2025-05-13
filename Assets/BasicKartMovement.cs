using UnityEngine;

public class KartController : MonoBehaviour
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
       
        // Debug information - uncomment if needed
        // Debug.Log("Speed: " + currentSpeed + " | Surface: " + 
        //          (tractionSystem != null ? tractionSystem.GetCurrentSurfaceTag() : "Unknown") +
        //          " | Traction: " + accelerationMultiplier);
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
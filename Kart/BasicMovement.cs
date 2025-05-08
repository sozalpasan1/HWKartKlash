using UnityEngine;

public class KartController : MonoBehaviour
{
    // Movement settings
    public float maxSpeed = 20.0f;          // Maximum forward speed
    public float maxReverseSpeed = 10.0f;    // Maximum reverse speed
    public float acceleration = 8.0f;        // How quickly we reach max speed
    public float braking = 10.0f;            // Braking power
    public float turnSpeed = 80.0f;          // How quickly the kart turns
    public float turnSpeedReduction = 0.7f;  // Reduce turning at high speeds
    public float gravity = 20.0f;            // Gravity force
    
    // Current motion state
    private float currentSpeed = 0.0f;
    private float currentRotation = 0.0f;
    
    // Components
    private Rigidbody rb;
    
    void Start()
    {
        // Get or add Rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        
        // Configure the Rigidbody for a kart
        rb.mass = 1000;
        rb.drag = 0;
        rb.angularDrag = 1;
        rb.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }
    
    void Update()
    {
        // Get input from WASD or arrow keys
        float accelerationInput = Input.GetAxis("Vertical");
        float steeringInput = Input.GetAxis("Horizontal");
        
        // Handle acceleration & braking
        if (accelerationInput > 0) // Accelerating
        {
            currentSpeed += accelerationInput * acceleration * Time.deltaTime;
            currentSpeed = Mathf.Min(currentSpeed, maxSpeed);
        }
        else if (accelerationInput < 0) // Braking or reversing
        {
            // If moving forward, brake
            if (currentSpeed > 0)
            {
                currentSpeed += accelerationInput * braking * Time.deltaTime;
            }
            else // If stopped or moving backward, accelerate in reverse
            {
                currentSpeed += accelerationInput * acceleration * Time.deltaTime;
                currentSpeed = Mathf.Max(currentSpeed, -maxReverseSpeed);
            }
        }
        else // No input - slow down naturally
        {
            if (currentSpeed > 0)
            {
                currentSpeed -= braking * 0.5f * Time.deltaTime;
                currentSpeed = Mathf.Max(currentSpeed, 0);
            }
            else if (currentSpeed < 0)
            {
                currentSpeed += braking * 0.5f * Time.deltaTime;
                currentSpeed = Mathf.Min(currentSpeed, 0);
            }
        }
        
        // Calculate turning amount - less turning at higher speeds for stability
        float speedFactor = Mathf.Abs(currentSpeed) / maxSpeed;
        float turnAmount = steeringInput * turnSpeed * (1.0f - (speedFactor * turnSpeedReduction)) * Time.deltaTime;
        
        // Only turn if we're moving
        if (Mathf.Abs(currentSpeed) > 0.1f)
        {
            // Reverse steering direction when going backward
            if (currentSpeed < 0)
                turnAmount = -turnAmount;
                
            transform.Rotate(0, turnAmount, 0);
        }
        
        // Apply movement in the direction the kart is facing
        Vector3 movement = transform.left * currentSpeed;
        
        // Apply movement and gravity
        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);
        rb.AddForce(Vector3.down * gravity);
    }
}
using UnityEngine;

public class JeepController : MonoBehaviour
{
    // Movement settings
    public float maxSpeed = 60.0f;         
    public float maxReverseSpeed = 10.0f;   
    public float acceleration = 40.0f;      
    public float braking = 10.0f;           
    public float turnSpeed = 80.0f;         
    public float turnSpeedReduction = 0.7f; 
    public float gravity = 20.0f;           
  
    // Current motion state
    private float currentSpeed = 0.0f;
  
    // Components
    private Rigidbody rb;

    // Direction setting - change this to match your jeep's orientation
    public enum JeepForwardDirection { Forward, Right, Left, Back }
    public JeepForwardDirection jeepForward = JeepForwardDirection.Left; // Set this in the Inspector
  
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
    }
  
    void FixedUpdate()
    {
        float accelerationInput = Input.GetAxis("Vertical");
        float steeringInput = Input.GetAxis("Horizontal");
      
        // Handle acceleration & braking
        if (accelerationInput > 0) // Accelerating
        {
            currentSpeed += accelerationInput * acceleration * Time.fixedDeltaTime;
            currentSpeed = Mathf.Min(currentSpeed, maxSpeed);
        }
        else if (accelerationInput < 0) // Braking or reversing
        {
            if (currentSpeed > 0)
            {
                currentSpeed += accelerationInput * braking * Time.fixedDeltaTime;
            }
            else
            {
                currentSpeed += accelerationInput * acceleration * Time.fixedDeltaTime;
                currentSpeed = Mathf.Max(currentSpeed, -maxReverseSpeed);
            }
        }
        else // No input - slow down naturally
        {
            if (currentSpeed > 0)
            {
                currentSpeed -= braking * 0.5f * Time.fixedDeltaTime;
                currentSpeed = Mathf.Max(currentSpeed, 0);
            }
            else if (currentSpeed < 0)
            {
                currentSpeed += braking * 0.5f * Time.fixedDeltaTime;
                currentSpeed = Mathf.Min(currentSpeed, 0);
            }
        }
      
        // FIXED: Use maxSpeed for turn calculation regardless of direction
        float speedFactor = Mathf.Abs(currentSpeed) / maxSpeed;
        float turnAmount = steeringInput * turnSpeed * (1.0f - (speedFactor * turnSpeedReduction)) * Time.fixedDeltaTime;
      
        if (Mathf.Abs(currentSpeed) > 0.1f)
        {
            // Adjust turn direction based on car's orientation and driving direction
            if (currentSpeed < 0)
            {
                // If in reverse, invert turning for natural feel
                turnAmount = -turnAmount;
            }
            
            // Apply the turning
            transform.Rotate(0, turnAmount, 0);
        }
      
        // Get the correct movement direction based on jeep orientation
        Vector3 movement = GetMovementDirection() * currentSpeed;
      
        rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);
        rb.AddForce(Vector3.down * gravity);
      
        Debug.Log("Speed: " + currentSpeed + " | Movement Vector: " + movement + " | Direction: " + jeepForward);
    }
  
    // This function returns the correct direction vector based on the jeep's orientation
    private Vector3 GetMovementDirection()
    {
        switch (jeepForward)
        {
            case JeepForwardDirection.Forward:
                return transform.forward;
            case JeepForwardDirection.Right:
                return transform.right;
            case JeepForwardDirection.Left:
                return -transform.right;
            case JeepForwardDirection.Back:
                return -transform.forward;
            default:
                return transform.forward;
        }
    }
}
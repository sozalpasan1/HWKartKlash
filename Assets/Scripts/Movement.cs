using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ArrowKeyMovement : MonoBehaviour
{
    public float forceAmount = 10f;
    private Rigidbody rb;
    private Acceleration accelerationSystem;

    private float currentSpeed = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        accelerationSystem = GetComponent<Acceleration>();
        
        // Add acceleration component if it doesn't exist
        if (accelerationSystem == null)
        {
            accelerationSystem = gameObject.AddComponent<Acceleration>();
        }
    }

    void FixedUpdate()
    {
        Vector3 force = Vector3.zero;
        bool hasInput = false;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            force += Vector3.forward;
            hasInput = true;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            force += Vector3.back;
            hasInput = true;
        }
            
        // Get multipliers from acceleration system
        float accelerationMultiplier = accelerationSystem.GetAccelerationMultiplier();
        float decelerationMultiplier = accelerationSystem.GetDecelerationMultiplier();
        float maxSpeed = accelerationSystem.GetMaxSpeed();
        float turnMultiplier = accelerationSystem.GetTurnMultiplier();
        
        // Handle acceleration and deceleration
        if (hasInput)
        {
            // Accelerate
            currentSpeed += accelerationMultiplier * Time.fixedDeltaTime * forceAmount;
            currentSpeed = Mathf.Min(currentSpeed, maxSpeed);
        }
        else
        {
            // Decelerate (friction)
            currentSpeed -= decelerationMultiplier * Time.fixedDeltaTime * forceAmount;
            currentSpeed = Mathf.Max(currentSpeed, 0f);
        }
        
        // Handle turning with modified turn radius
        float turnAmount = 0f;
        if (Input.GetKey(KeyCode.LeftArrow))
            turnAmount = -1f;
        if (Input.GetKey(KeyCode.RightArrow))
            turnAmount = 1f;
            
        // Apply turn based on turn multiplier
        if (Mathf.Abs(currentSpeed) > 0.1f)
        {
            transform.Rotate(0, turnAmount * turnMultiplier * Time.fixedDeltaTime * 60f, 0);
        }
        
        // Apply movement force
        Vector3 movement = transform.forward * currentSpeed;
        rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);
        
        // Apply gravity
        rb.AddForce(Vector3.down * 9.81f, ForceMode.Acceleration);
        
        // Debug info
        Debug.Log("Speed: " + currentSpeed + " | Max Speed: " + maxSpeed + 
                 " | Surface: " + accelerationSystem.GetCurrentSurfaceTag());
    }
}

using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float movementSpeed = 25f;       // Increased from 7 to 25
    [SerializeField] private float maxVelocity = 15f;         // Increased from 5 to 15
    [SerializeField] private float groundDrag = 1.5f;         // Reduced from 3 to 1.5
    
    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        // Only allow the owner to control this player
        if (!IsOwner) 
        {
            enabled = false;
            return;
        }
        
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Prevent tipping over
    }

    void Update()
    {
        // Check if player is grounded
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
        
        // Apply drag when grounded
        if (isGrounded)
        {
            rb.linearDamping = groundDrag;  // Fixed: linearDamping → drag
        }
        else
        {
            rb.linearDamping = 0;
        }
        
        // Limit velocity
        LimitVelocity();
    }
    
    void FixedUpdate()
    {
        // Get input values
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Create movement vector (X and Z only)
        Vector3 movement = new Vector3(horizontalInput, 0, verticalInput);
        
        // Normalize to prevent diagonal movement from being faster
        if (movement.magnitude > 1f)
        {
            movement.Normalize();
        }
        
        // Apply movement force - using ForceMode.VelocityChange for faster response
        if (isGrounded)
        {
            rb.AddForce(movement * movementSpeed, ForceMode.VelocityChange);
        }
    }
    
    private void LimitVelocity()
    {
        // Get velocity on horizontal plane only (X and Z)
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);  // Fixed: linearVelocity → velocity
        
        // Limit velocity if needed
        if (horizontalVelocity.magnitude > maxVelocity)
        {
            Vector3 limitedVelocity = horizontalVelocity.normalized * maxVelocity;
            rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);  // Fixed: linearVelocity → velocity
        }
    }
}
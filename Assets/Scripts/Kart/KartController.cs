using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartController : MonoBehaviour
{
    [Header("Kart Settings")]
    [SerializeField] private float maxSpeed = 15f;
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float turnSpeed = 120f;
    [SerializeField] private float gravity = 20f;
    [SerializeField] private float groundRayLength = 0.5f;
    
    [Header("Network")]
    public bool IsLocalPlayer = false;
    public string PlayerId = "";
    
    // Physics
    private float currentSpeed = 0f;
    private float currentRotation = 0f;
    private bool isGrounded = true;
    
    // Remote player interpolation
    private Vector3 targetPosition;
    private float targetRotation;
    private float interpolationSpeed = 10f;
    
    // Components
    private Rigidbody rb;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        targetPosition = transform.position;
        targetRotation = transform.eulerAngles.y;
    }
    
    private void Update()
    {
        if (IsLocalPlayer)
        {
            // Process input on the local player only
            HandleInput();
        }
        else
        {
            // For remote players, interpolate to target position and rotation
            InterpolateToTarget();
        }
    }
    
    private void FixedUpdate()
    {
        if (IsLocalPlayer)
        {
            // Only apply physics on the local player
            ApplyPhysics();
        }
    }
    
    private void HandleInput()
    {
        // Forward/Backward
        if (Input.GetKeyDown(KeyCode.W))
        {
            NetworkManager.Instance.SendInput("W", true);
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            NetworkManager.Instance.SendInput("W", false);
        }
        
        if (Input.GetKeyDown(KeyCode.S))
        {
            NetworkManager.Instance.SendInput("S", true);
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            NetworkManager.Instance.SendInput("S", false);
        }
        
        // Left/Right
        if (Input.GetKeyDown(KeyCode.A))
        {
            NetworkManager.Instance.SendInput("A", true);
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            NetworkManager.Instance.SendInput("A", false);
        }
        
        if (Input.GetKeyDown(KeyCode.D))
        {
            NetworkManager.Instance.SendInput("D", true);
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            NetworkManager.Instance.SendInput("D", false);
        }
        
        // Brake
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NetworkManager.Instance.SendInput("Space", true);
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            NetworkManager.Instance.SendInput("Space", false);
        }
        
        // Apply local physics based on input
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");
        
        // Acceleration/braking
        if (vertical > 0)
        {
            currentSpeed += acceleration * Time.deltaTime;
        }
        else if (vertical < 0)
        {
            currentSpeed -= acceleration * 0.5f * Time.deltaTime; // Slower in reverse
        }
        else
        {
            // Natural deceleration
            if (currentSpeed > 0)
            {
                currentSpeed -= acceleration * 0.3f * Time.deltaTime;
                if (currentSpeed < 0) currentSpeed = 0;
            }
            else if (currentSpeed < 0)
            {
                currentSpeed += acceleration * 0.3f * Time.deltaTime;
                if (currentSpeed > 0) currentSpeed = 0;
            }
        }
        
        // Apply braking
        if (Input.GetKey(KeyCode.Space))
        {
            currentSpeed *= 0.95f;
        }
        
        // Clamp speed
        currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed * 0.5f, maxSpeed);
        
        // Turning
        if (Mathf.Abs(currentSpeed) > 0.1f)
        {
            float turnAmount = horizontal * turnSpeed * Time.deltaTime;
            // Reduce turning at higher speeds
            float speedFactor = Mathf.Clamp01(1.0f - (Mathf.Abs(currentSpeed) / maxSpeed) * 0.5f);
            turnAmount *= (0.5f + speedFactor * 0.5f);
            
            // Apply the turn
            currentRotation += turnAmount;
        }
    }
    
    private void ApplyPhysics()
    {
        // Check if grounded
        RaycastHit hit;
        isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out hit, groundRayLength);
        
        Vector3 movement = transform.forward * currentSpeed;
        
        // Apply gravity if not grounded
        if (!isGrounded)
        {
            movement.y -= gravity * Time.deltaTime;
        }
        else
        {
            // Align with ground normal
            transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            
            // Apply rotation
            transform.rotation = Quaternion.Euler(0, currentRotation, 0);
        }
        
        // Move the kart
        if (rb != null)
        {
            rb.linearVelocity = movement;
        }
        else
        {
            transform.position += movement * Time.deltaTime;
        }
    }
    
    private void InterpolateToTarget()
    {
        // Smoothly move remote player to target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * interpolationSpeed);
        
        // Smoothly rotate to target rotation
        float currentYRot = transform.eulerAngles.y;
        
        // Handle wraparound for angle interpolation
        if (Mathf.Abs(targetRotation - currentYRot) > 180)
        {
            if (targetRotation > currentYRot)
            {
                currentYRot += 360;
            }
            else
            {
                targetRotation += 360;
            }
        }
        
        float newRot = Mathf.Lerp(currentYRot, targetRotation, Time.deltaTime * interpolationSpeed);
        transform.rotation = Quaternion.Euler(0, newRot % 360, 0);
    }
    
    public void SetTargetTransform(Vector3 position, float rotation, float speed)
    {
        targetPosition = position;
        targetRotation = rotation;
        
        // Adjust interpolation speed based on the actual kart speed
        // Faster karts need faster interpolation
        interpolationSpeed = 5f + Mathf.Abs(speed) * 0.5f;
    }
}
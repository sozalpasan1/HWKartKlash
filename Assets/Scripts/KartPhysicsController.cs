using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class KartPhysicsController : MonoBehaviour
{
    [Header("Kart Components")]
    public Transform centerOfMass;
    public List<WheelCollider> driveWheels = new List<WheelCollider>();
    public List<WheelCollider> steeringWheels = new List<WheelCollider>();
    public List<GameObject> wheelMeshes = new List<GameObject>();
    
    [Header("Drive Settings")]
    public float maxSpeed = 25f; // Maximum speed in m/s (multiply by 3.6 for km/h)
    public float maxReverseSpeed = 10f;
    public float acceleration = 8f; // Lower values = more gradual acceleration
    public float brakeForce = 15f;
    
    [Header("Steering Settings")]
    public float maxSteerAngle = 30f;
    public float steerSpeed = 1f; // How quickly steering responds
    public AnimationCurve steerCurve; // Steering angle adjustment based on speed
    
    [Header("Stability Settings")]
    public float downforce = 1.5f; // Multiplier for downward force
    public bool constrainRotation = true; // Should we use RigidBody constraints?
    
    // Runtime variables
    private Rigidbody rb;
    private float currentSpeed;
    private float currentSteerAngle;
    private float throttleInput;
    private float steerInput;
    private float brakeInput;
    private bool isGrounded;
    private float speedRatio; // Current speed as ratio of max speed (0-1)
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // Set center of mass low for stability
        if (centerOfMass != null)
        {
            rb.centerOfMass = centerOfMass.localPosition;
        }
        else
        {
            // Default to a low center of mass if none assigned
            rb.centerOfMass = new Vector3(0, -0.5f, 0);
        }
        
        // Apply constraints to prevent unwanted rotation
        if (constrainRotation)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
        
        // Configure wheel colliders
        SetupWheels();
    }
    
    private void SetupWheels()
    {
        // Configure wheel friction curves for better grip
        ConfigureWheelFriction(driveWheels);
        ConfigureWheelFriction(steeringWheels);
    }
    
    private void ConfigureWheelFriction(List<WheelCollider> wheels)
    {
        foreach (WheelCollider wheel in wheels)
        {
            // Configure forward friction for drive force
            WheelFrictionCurve forwardFriction = wheel.forwardFriction;
            forwardFriction.extremumSlip = 0.4f;
            forwardFriction.extremumValue = 1f;
            forwardFriction.asymptoteSlip = 0.8f;
            forwardFriction.asymptoteValue = 0.8f;
            forwardFriction.stiffness = 1.0f;
            wheel.forwardFriction = forwardFriction;
            
            // Configure sideways friction for better cornering
            WheelFrictionCurve sidewaysFriction = wheel.sidewaysFriction;
            sidewaysFriction.extremumSlip = 0.25f;
            sidewaysFriction.extremumValue = 1f;
            sidewaysFriction.asymptoteSlip = 0.5f;
            sidewaysFriction.asymptoteValue = 0.8f;
            sidewaysFriction.stiffness = 1.0f;
            wheel.sidewaysFriction = sidewaysFriction;
        }
    }
    
    private void Update()
    {
        // Get input
        throttleInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");
        brakeInput = Input.GetKey(KeyCode.Space) ? 1f : 0f;
        
        // Update wheel visuals
        UpdateWheelMeshes();
        
        // Calculate current speed
        currentSpeed = Vector3.Dot(rb.linearVelocity, transform.forward); // Only forward component
        speedRatio = Mathf.Abs(currentSpeed) / maxSpeed;
        
        // Check if grounded
        CheckGrounded();
    }
    
    private void FixedUpdate()
    {
        // Apply driving force
        ApplyDriveForce();
        
        // Apply steering
        ApplySteering();
        
        // Apply braking
        ApplyBraking();
        
        // Apply downforce for stability
        ApplyDownforce();
        
        // Limit max speed
        LimitSpeed();
    }
    
    private void ApplyDriveForce()
    {
        // Skip if no input or braking
        if (Mathf.Abs(throttleInput) < 0.1f || brakeInput > 0.1f)
        {
            foreach (WheelCollider wheel in driveWheels)
            {
                wheel.motorTorque = 0;
            }
            return;
        }
        
        // Only apply drive force when at least one wheel is grounded
        if (isGrounded)
        {
            // Calculate target speed based on input
            float targetSpeed = throttleInput > 0 ? maxSpeed : -maxReverseSpeed;
            
            // Calculate speed difference
            float speedDiff = targetSpeed - currentSpeed;
            
            // Calculate force based on how far we are from target speed
            float force = speedDiff * acceleration;
            
            // Convert to motor torque for wheels (simplified approach)
            float torquePerWheel = force * 50f / driveWheels.Count;
            
            // Apply torque to drive wheels
            foreach (WheelCollider wheel in driveWheels)
            {
                wheel.motorTorque = torquePerWheel;
            }
            
            // For very low speeds, apply a direct force to help get moving
            if (Mathf.Abs(currentSpeed) < 2f && Mathf.Abs(throttleInput) > 0.5f)
            {
                float startingForce = throttleInput * 500f;
                rb.AddForce(transform.forward * startingForce);
            }
        }
        else
        {
            // When in air, don't apply any drive force
            foreach (WheelCollider wheel in driveWheels)
            {
                wheel.motorTorque = 0;
            }
        }
    }
    
    private void ApplySteering()
    {
        // Calculate steering angle based on speed
        float speedFactor = steerCurve != null ? 
            steerCurve.Evaluate(speedRatio) : 
            1f - (speedRatio * 0.5f); // Default curve if none assigned
        
        // Calculate target angle
        float targetSteerAngle = steerInput * maxSteerAngle * speedFactor;
        
        // Gradually adjust current angle for smoother steering
        currentSteerAngle = Mathf.Lerp(currentSteerAngle, targetSteerAngle, Time.fixedDeltaTime * steerSpeed * 10f);
        
        // Apply to steering wheels
        foreach (WheelCollider wheel in steeringWheels)
        {
            wheel.steerAngle = currentSteerAngle;
        }
    }
    
    private void ApplyBraking()
    {
        // Calculate brake force
        float brakeForceToApply = 0;
        
        // Apply active braking when brake key is pressed
        if (brakeInput > 0.1f)
        {
            brakeForceToApply = brakeForce * 500f;
        }
        // Apply automatic braking when moving in opposite direction to input
        else if (throttleInput != 0 && Mathf.Sign(throttleInput) != Mathf.Sign(currentSpeed) && Mathf.Abs(currentSpeed) > 1f)
        {
            brakeForceToApply = brakeForce * 300f;
        }
        
        // Apply brake force to all wheels
        foreach (WheelCollider wheel in driveWheels)
        {
            wheel.brakeTorque = brakeForceToApply;
        }
        
        foreach (WheelCollider wheel in steeringWheels)
        {
            wheel.brakeTorque = brakeForceToApply;
        }
    }
    
    private void ApplyDownforce()
    {
        if (isGrounded)
        {
            // Apply more downforce at higher speeds
            float forceMagnitude = rb.mass * downforce * (1f + speedRatio);
            rb.AddForce(-transform.up * forceMagnitude);
        }
    }
    
    private void LimitSpeed()
    {
        // Only limit speed on ground
        if (isGrounded)
        {
            // Get forward velocity
            Vector3 forwardVelocity = transform.forward * currentSpeed;
            
            // Get lateral velocity (for drifting/sliding)
            Vector3 lateralVelocity = rb.linearVelocity - forwardVelocity;
            
            // Check if exceeding max speed
            if (currentSpeed > maxSpeed)
            {
                // Limit to max speed
                forwardVelocity = transform.forward * maxSpeed;
            }
            else if (currentSpeed < -maxReverseSpeed)
            {
                // Limit to max reverse speed
                forwardVelocity = transform.forward * -maxReverseSpeed;
            }
            
            // Recombine velocities
            rb.linearVelocity = forwardVelocity + lateralVelocity;
        }
    }
    
    private void UpdateWheelMeshes()
    {
        if (wheelMeshes.Count != driveWheels.Count + steeringWheels.Count)
        {
            Debug.LogWarning("Number of wheel meshes doesn't match wheel colliders!");
            return;
        }
        
        int index = 0;
        
        // Update drive wheel meshes
        foreach (WheelCollider wheel in driveWheels)
        {
            if (index < wheelMeshes.Count)
            {
                UpdateWheelMesh(wheel, wheelMeshes[index]);
                index++;
            }
        }
        
        // Update steering wheel meshes
        foreach (WheelCollider wheel in steeringWheels)
        {
            if (index < wheelMeshes.Count)
            {
                UpdateWheelMesh(wheel, wheelMeshes[index]);
                index++;
            }
        }
    }
    
    private void UpdateWheelMesh(WheelCollider collider, GameObject wheelMesh)
    {
        if (collider == null || wheelMesh == null) return;
        
        // Get wheel position and rotation from the collider
        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);
        rotation *= Quaternion.Euler(0, 0, 90);
        
        // Update mesh position and rotation
        wheelMesh.transform.position = position;
        wheelMesh.transform.rotation = rotation;
    }
    
    private void CheckGrounded()
    {
        isGrounded = false;
        
        // Check all wheels
        foreach (WheelCollider wheel in driveWheels)
        {
            if (wheel.isGrounded)
            {
                isGrounded = true;
                break;
            }
        }
        
        if (!isGrounded)
        {
            foreach (WheelCollider wheel in steeringWheels)
            {
                if (wheel.isGrounded)
                {
                    isGrounded = true;
                    break;
                }
            }
        }
    }
    
    // Debug visualization
    private void OnDrawGizmos()
    {
        // Draw center of mass
        Gizmos.color = Color.red;
        if (centerOfMass != null)
        {
            Gizmos.DrawSphere(transform.TransformPoint(centerOfMass.localPosition), 0.1f);
        }
        else if (Application.isPlaying && rb != null)
        {
            Gizmos.DrawSphere(transform.TransformPoint(rb.centerOfMass), 0.1f);
        }
    }
    
    // Helper methods for UI or external access
    public float GetSpeedKMH()
    {
        return currentSpeed * 3.6f; // Convert m/s to km/h
    }
    
    public float GetSpeedMPH()
    {
        return currentSpeed * 2.237f; // Convert m/s to mph
    }
    
    public bool IsGrounded()
    {
        return isGrounded;
    }
}
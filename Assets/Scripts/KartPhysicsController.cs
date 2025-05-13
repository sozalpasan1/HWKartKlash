using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class KartPhysicsController : MonoBehaviour
{
    [Header("Kart Components")]
    public Transform centerOfMass;
    public List<WheelCollider> driveWheels;
    public List<WheelCollider> steeringWheels;
    public List<GameObject> wheelMeshes;

    [Header("Engine Settings")]
    public AnimationCurve powerCurve;
    public float maxMotorTorque = 500f;
    public float maxSpeed = 30f;
    public float brakeTorque = 4000f;
    public float idleRPM = 800f;
    public float maxRPM = 6000f;
    public float currentRPM;

    [Header("Steering Settings")]
    public float maxSteerAngle = 30f;
    public float steerSpeed = 10f;
    public float steerSensitivity = 1f;
    public AnimationCurve steerCurve;

    [Header("Suspension Settings")]
    public float suspensionHeight = 0.2f;
    public float suspensionSpring = 35000f;
    public float suspensionDamper = 4500f;
    public float suspensionDistance = 0.3f;
    
    [Header("Drift Settings")]
    public float minSpeedForDrift = 8f;
    public float driftFactor = 0.7f;
    public float lateralGripFactor = 0.9f;
    
    [Header("Physics Settings")]
    public float downforce = 100f;
    public float airDrag = 0.1f;
    public float groundDrag = 3f;
    public LayerMask groundLayer;
    
    // References and runtime variables
    private Rigidbody rb;
    private float currentSteerAngle;
    private float motorInput;
    private float brakeInput;
    private float steerInput;
    private bool isDrifting;
    private bool isGrounded;
    private float currentSpeed;
    private float speedRatio;

    // Audio (add your own AudioSource components)
    public AudioSource engineSound;
    public AudioSource skidSound;
    private float enginePitch;

    // Visual effects (add your own particle systems)
    public ParticleSystem[] wheelSmoke;

private void Start()
{
    rb = GetComponent<Rigidbody>();
    
    if (centerOfMass != null)
    {
        // IMPORTANT: Make sure center of mass is very low
        Vector3 comPosition = centerOfMass.localPosition;
        // Override Y position to be very low
        comPosition.y = -0.5f;  
        rb.centerOfMass = comPosition;
        
        // Visual feedback (optional)
        centerOfMass.localPosition = comPosition;
    }
    else
    {
        Debug.LogWarning("No center of mass assigned to kart controller!");
        // Set a default low center of mass
        rb.centerOfMass = new Vector3(0, -0.5f, 0);
    }
    
    // Configure wheel colliders
    ConfigureWheelColliders();
}

    private void ConfigureWheelColliders()
    {
        JointSpring suspensionSpringSettings = new JointSpring();
        suspensionSpringSettings.spring = suspensionSpring;
        suspensionSpringSettings.damper = suspensionDamper;
        suspensionSpringSettings.targetPosition = 0.5f;

        foreach (WheelCollider wheel in driveWheels)
        {
            wheel.suspensionDistance = suspensionDistance;
            wheel.center = new Vector3(0, suspensionHeight, 0);
            wheel.suspensionSpring = suspensionSpringSettings;
            wheel.wheelDampingRate = 1f;
        }

        foreach (WheelCollider wheel in steeringWheels)
        {
            wheel.suspensionDistance = suspensionDistance;
            wheel.center = new Vector3(0, suspensionHeight, 0);
            wheel.suspensionSpring = suspensionSpringSettings;
            wheel.wheelDampingRate = 1f;
        }
    }

    private void Update()
    {
        // Get input
        motorInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");
        brakeInput = Input.GetKey(KeyCode.Space) ? 1f : 0f;
        
        // Update visuals
        UpdateWheelMeshes();
        
        // Update current speed (for UI and calculations)
        currentSpeed = rb.linearVelocity.magnitude * 3.6f; // km/h
        speedRatio = Mathf.Clamp01(currentSpeed / maxSpeed);
        
        // Update audio
        if (engineSound != null)
        {
            UpdateEngineSound();
        }
        
        // Update drift effects
        UpdateDriftEffects();
        
        // Check if we're grounded
        CheckGrounded();
    }

    private void FixedUpdate()
    {
        // Apply forces
        ApplyMotorTorque();
        ApplySteering();
        ApplyDownforce();
        ApplyDrag();
        ApplyBraking();
        
        // Apply drift physics
        if (ShouldDrift())
        {
            ApplyDrift();
        }
    }

    private void ApplyMotorTorque()
    {
        // Use a much simpler model that's less likely to cause problems
        float motorPower = motorInput * maxMotorTorque;
    
        // Only apply motor torque at very low values initially
        float safeMotorPower = Mathf.Clamp(motorPower, -300f, 300f);
    
        // Apply torque to drive wheels
        foreach (WheelCollider wheel in driveWheels)
        {
            wheel.motorTorque = safeMotorPower;
        }
    }
    
    private float EvaluatePowerCurve()
    {
        // Normalize RPM to 0-1 range for curve evaluation
        float normalizedRPM = (currentRPM - idleRPM) / (maxRPM - idleRPM);
        normalizedRPM = Mathf.Clamp01(normalizedRPM);
        
        // Use power curve or default to linear if not set
        if (powerCurve != null && powerCurve.keys.Length > 0)
        {
            return powerCurve.Evaluate(normalizedRPM);
        }
        else
        {
            // Default power curve peaking at 80% of max RPM
            return normalizedRPM * (1f - normalizedRPM * 0.2f) * 4f;
        }
    }
    
    private void CalculateEngineRPM()
    {
        // Get average RPM from drive wheels
        float avgWheelRPM = 0f;
        int wheelCount = 0;
        
        foreach (WheelCollider wheel in driveWheels)
        {
            avgWheelRPM += wheel.rpm;
            wheelCount++;
        }
        
        if (wheelCount > 0)
        {
            avgWheelRPM /= wheelCount;
        }
        
        // Calculate engine RPM
        float wheelToEngineRatio = 5f; // Gear ratio
        float targetRPM = Mathf.Abs(avgWheelRPM * wheelToEngineRatio) + idleRPM;
        
        // Add some RPM when accelerating but not moving yet
        if (Mathf.Abs(avgWheelRPM) < 10f && Mathf.Abs(motorInput) > 0.1f)
        {
            targetRPM += maxRPM * 0.3f * Mathf.Abs(motorInput);
        }
        
        // Smoothly adjust RPM
        currentRPM = Mathf.Lerp(currentRPM, targetRPM, Time.fixedDeltaTime * 5f);
        currentRPM = Mathf.Clamp(currentRPM, idleRPM, maxRPM);
    }

    private void ApplySteering()
    {
        // Make steering less sensitive at high speeds
        float speedFactor = steerCurve != null ? steerCurve.Evaluate(speedRatio) : 1f - (speedRatio * 0.7f);
        float targetSteerAngle = steerInput * maxSteerAngle * speedFactor * steerSensitivity;
        
        // Gradually adjust steering for smoother control
        currentSteerAngle = Mathf.Lerp(currentSteerAngle, targetSteerAngle, Time.fixedDeltaTime * steerSpeed);
        
        // Apply steering to steering wheels
        foreach (WheelCollider wheel in steeringWheels)
        {
            wheel.steerAngle = currentSteerAngle;
        }
    }

    private void ApplyBraking()
    {
        // Apply brake torque to all wheels
        float brakingForce = brakeInput * brakeTorque;
        
        // Apply reverse braking when going forward but pressing backward
        if (currentSpeed > 1f && motorInput < -0.1f)
        {
            brakingForce = Mathf.Abs(motorInput) * brakeTorque;
        }
        
        // Apply reverse braking when going backward but pressing forward
        if (currentSpeed > 1f && rb.linearVelocity.z < -0.1f && motorInput > 0.1f)
        {
            brakingForce = Mathf.Abs(motorInput) * brakeTorque;
        }
        
        foreach (WheelCollider wheel in driveWheels)
        {
            wheel.brakeTorque = brakingForce;
        }
        
        foreach (WheelCollider wheel in steeringWheels)
        {
            wheel.brakeTorque = brakingForce * 0.5f; // Less braking on steering wheels
        }
    }

    private void ApplyDownforce()
    {
        if (isGrounded)
        {
            // Apply downforce relative to speed
            float force = downforce * Mathf.Clamp01(speedRatio * speedRatio);
            rb.AddForce(-transform.up * force);
        }
    }
    
    private void ApplyDrag()
    {
        // Apply air or ground drag based on grounded state
        float dragFactor = isGrounded ? groundDrag : airDrag;
        rb.linearDamping = Mathf.Lerp(rb.linearDamping, dragFactor, Time.fixedDeltaTime * 3f);
    }
    
    private bool ShouldDrift()
    {
        // Check if conditions are right for drifting
        bool hasSpeed = currentSpeed > minSpeedForDrift;
        bool hasSteeringInput = Mathf.Abs(steerInput) > 0.5f;
        bool isBraking = brakeInput > 0.1f;
        
        // Start drifting
        if (hasSpeed && hasSteeringInput && isBraking)
        {
            isDrifting = true;
            return true;
        }
        
        // Continue drifting
        if (isDrifting && hasSpeed && hasSteeringInput)
        {
            return true;
        }
        
        // Stop drifting
        isDrifting = false;
        return false;
    }
    
    private void ApplyDrift()
    {
        // Reduce lateral friction on wheels
        foreach (WheelCollider wheel in driveWheels)
        {
            WheelFrictionCurve sideFriction = wheel.sidewaysFriction;
            sideFriction.stiffness = lateralGripFactor * driftFactor;
            wheel.sidewaysFriction = sideFriction;
        }
        
        // Add some side force to enhance the drift feel
        float driftForce = steerInput * driftFactor * rb.mass * speedRatio;
        rb.AddForce(transform.right * driftForce, ForceMode.Force);
    }
    
    private void ResetWheelFriction()
    {
        // Reset wheel friction to normal when not drifting
        foreach (WheelCollider wheel in driveWheels)
        {
            WheelFrictionCurve sideFriction = wheel.sidewaysFriction;
            sideFriction.stiffness = lateralGripFactor;
            wheel.sidewaysFriction = sideFriction;
        }
    }
    
    private void UpdateWheelMeshes()
    {
        // Make sure we have the right number of meshes
        if (wheelMeshes.Count != driveWheels.Count + steeringWheels.Count)
        {
            Debug.LogWarning("Number of wheel meshes does not match wheel colliders!");
            return;
        }
        
        int wheelIndex = 0;
        
        // Update drive wheel meshes
        foreach (WheelCollider wheel in driveWheels)
        {
            UpdateWheelMesh(wheel, wheelMeshes[wheelIndex]);
            wheelIndex++;
        }
        
        // Update steering wheel meshes
        foreach (WheelCollider wheel in steeringWheels)
        {
            UpdateWheelMesh(wheel, wheelMeshes[wheelIndex]);
            wheelIndex++;
        }
    }
    
    private void UpdateWheelMesh(WheelCollider collider, GameObject wheelMesh)
    {
        // Position and rotate wheel mesh to match collider
        if (collider && wheelMesh)
        {
            Vector3 position;
            Quaternion rotation;
            collider.GetWorldPose(out position, out rotation);
            rotation *= Quaternion.Euler(0, 0, 90);
            
            wheelMesh.transform.position = position;
            wheelMesh.transform.rotation = rotation;
        }
    }
    
    private void UpdateEngineSound()
    {
        if (engineSound != null)
        {
            // Map RPM to audio pitch
            float minPitch = 0.3f;
            float maxPitch = 1.5f;
            enginePitch = minPitch + ((currentRPM - idleRPM) / (maxRPM - idleRPM)) * (maxPitch - minPitch);
            
            // Apply pitch to engine sound
            engineSound.pitch = Mathf.Lerp(engineSound.pitch, enginePitch, Time.deltaTime * 2f);
            
            // Ensure the engine sound is playing
            if (!engineSound.isPlaying)
            {
                engineSound.Play();
            }
        }
    }
    
    private void UpdateDriftEffects()
    {
        if (isDrifting)
        {
            // Play skid sound
            if (skidSound != null && !skidSound.isPlaying)
            {
                skidSound.Play();
            }
            
            // Emit smoke particles
            if (wheelSmoke != null)
            {
                foreach (ParticleSystem smoke in wheelSmoke)
                {
                    if (!smoke.isEmitting)
                    {
                        smoke.Play();
                    }
                }
            }
        }
        else
        {
            // Stop skid sound
            if (skidSound != null && skidSound.isPlaying)
            {
                skidSound.Stop();
            }
            
            // Stop smoke particles
            if (wheelSmoke != null)
            {
                foreach (ParticleSystem smoke in wheelSmoke)
                {
                    if (smoke.isEmitting)
                    {
                        smoke.Stop();
                    }
                }
            }
            
            // Reset wheel friction
            ResetWheelFriction();
        }
    }
    
    private void CheckGrounded()
    {
        // Check if any wheel is touching the ground
        isGrounded = false;
        
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
    
    // Public methods that can be used for UI or other game systems
    
    public float GetSpeedKMH()
    {
        return currentSpeed;
    }
    
    public float GetSpeedMPH()
    {
        return currentSpeed * 0.621371f;
    }
    
    public float GetEngineRPM()
    {
        return currentRPM;
    }
    
    public bool IsDrifting()
    {
        return isDrifting;
    }

    // Utility function to calculate the bank angle based on terrain
    public float GetBankAngle()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, -Vector3.up, out hitInfo, 10f, groundLayer))
        {
            Vector3 normal = hitInfo.normal;
            float bankAngle = Vector3.SignedAngle(Vector3.up, normal, transform.forward);
            return bankAngle;
        }
        return 0f;
    }
}
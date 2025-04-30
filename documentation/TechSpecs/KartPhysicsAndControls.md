# Technical Specification: Kart Physics Implementation for Unity Go-Kart Game (P1 Scope)

## 1. Overview

This technical specification details the implementation of realistic kart physics for a Unity-based Go-Kart game. The system will provide responsive controls, realistic weight distribution, suspension, drifting mechanics, and terrain interaction.

## 2. Core Classes Structure

### 2.1. KartController

The central class orchestrating all kart physics components.

**Variables:**
```csharp
public class KartController : MonoBehaviour
{
    [SerializeField] private KartPhysics physics;
    [SerializeField] private KartInput input;
    [SerializeField] private KartEngine engine;
    [SerializeField] private KartWheel[] wheels;
    [SerializeField] private Transform centerOfMass;
    
    private Rigidbody kartRigidbody;
    private float currentSpeed;
    private float turnAngle;
    private bool isDrifting;
}
```

**Functions:**
```csharp
void Awake() // Initialize components and references
void Start() // Set initial state
void Update() // Handle per-frame updates (input, visuals)
void FixedUpdate() // Handle physics updates
void ApplyDrive(float throttleInput) // Apply driving force
void ApplySteering(float steeringInput) // Apply steering force
void ApplyBrake(float brakeInput) // Apply braking force
void CheckForDrift() // Determine if drifting conditions are met
void UpdateSpeedometer() // Update speed value for UI
```

### 2.2. KartPhysics

Handles core physics calculations and force application.

**Variables:**
```csharp
public class KartPhysics : MonoBehaviour
{
    [SerializeField] private float forwardAcceleration = 8f;
    [SerializeField] private float reverseAcceleration = 4f;
    [SerializeField] private float maxSpeed = 30f;
    [SerializeField] private float maxReverseSpeed = 10f;
    [SerializeField] private float turnStrength = 180f;
    [SerializeField] private float gripFactor = 0.95f;
    [SerializeField] private float driftFactor = 0.7f;
    [SerializeField] private float airDragMultiplier = 0.1f;
    [SerializeField] private float groundDragMultiplier = 3f;
    [SerializeField] private float gravity = 20f;
    [SerializeField] private LayerMask groundLayer;
    
    private Rigidbody rb;
    private bool isGrounded;
}
```

**Functions:**
```csharp
void Initialize() // Set up physics parameters
void ApplyForces(float throttle, float steering, float brake) // Apply all forces
void CalculateGroundNormal() // Calculate ground surface normal
void CalculateFriction() // Calculate surface friction
float CalculateTraction() // Calculate wheel traction
void ApplyGravity() // Apply custom gravity force
void ApplyDrag() // Apply air and ground resistance
bool IsGrounded() // Check if kart has ground contact
void AdjustForTerrain(TerrainType terrain) // Adjust physics for terrain type
```

### 2.3. KartInput

Manages player input collection and processing.

**Variables:**
```csharp
public class KartInput : MonoBehaviour
{
    private float throttleInput;
    private float steeringInput;
    private float brakeInput;
    private bool driftButtonPressed;
    private bool boostButtonPressed;
    
    // Input System references
    private InputAction throttleAction;
    private InputAction steeringAction;
    private InputAction brakeAction;
    private InputAction driftAction;
    private InputAction boostAction;
}
```

**Functions:**
```csharp
void Awake() // Set up input actions
void OnEnable() // Enable input actions
void OnDisable() // Disable input actions
void ProcessInput() // Process current frame's input
float GetThrottle() // Get throttle value (-1 to 1)
float GetSteering() // Get steering value (-1 to 1)
float GetBrake() // Get brake value (0 to 1)
bool IsDrifting() // Check if drift button is pressed
bool IsBoosting() // Check if boost button is pressed
```

### 2.4. KartWheel

Represents each individual wheel with physics properties.

**Variables:**
```csharp
public class KartWheel : MonoBehaviour
{
    [SerializeField] private Transform wheelTransform;
    [SerializeField] private Transform suspensionTransform;
    [SerializeField] private KartSuspension suspension;
    [SerializeField] private float radius = 0.4f;
    [SerializeField] private float mass = 15f;
    [SerializeField] private float grip = 1f;
    [SerializeField] private float brakeTorque = 4000f;
    [SerializeField] private bool isPowered = false;
    [SerializeField] private bool isSteering = false;
    
    private float currentRpm;
    private Vector3 worldVelocity;
    private bool isGrounded;
}
```

**Functions:**
```csharp
void Initialize() // Set up wheel parameters
void UpdatePosition(float suspensionTravel) // Update wheel based on suspension
void ApplyTorque(float torque) // Apply drive torque
void ApplyBraking(float brakeInput) // Apply braking force
void ApplySteering(float steeringAngle) // Apply steering angle
void UpdateVisuals() // Update visual rotation and position
float CalculateSlip() // Calculate slip ratio and angle
float GetGripFactor() // Get current grip based on conditions
Vector3 CalculateForwardForce() // Calculate forward force
Vector3 CalculateLateralForce() // Calculate side force
```

### 2.5. KartSuspension

Handles suspension physics for realistic wheel movement.

**Variables:**
```csharp
public class KartSuspension : MonoBehaviour
{
    [SerializeField] private float restLength = 0.5f;
    [SerializeField] private float minLength = 0.2f;
    [SerializeField] private float maxLength = 0.7f;
    [SerializeField] private float springStiffness = 30000f;
    [SerializeField] private float damperStiffness = 4000f;
    
    private float currentLength;
    private float previousLength;
    private float springForce;
    private float damperForce;
    private Vector3 suspensionForce;
}
```

**Functions:**
```csharp
void Initialize() // Set up suspension parameters
float CalculateSpringForce() // Calculate spring force from compression
float CalculateDamperForce() // Calculate damper force from velocity
Vector3 CalculateSuspensionForce() // Calculate total suspension force
bool CheckGroundContact(out RaycastHit hit) // Check for ground
float GetCurrentLength() // Get current suspension length
void UpdateSuspensionForce(RaycastHit groundHit) // Update based on ground
void ApplySuspensionForce(Rigidbody rb) // Apply force to rigidbody
```

### 2.6. KartEngine

Models engine behavior and power delivery.

**Variables:**
```csharp
public class KartEngine : MonoBehaviour
{
    [SerializeField] private float maxPower = 150f;
    [SerializeField] private float minRPM = 1000f;
    [SerializeField] private float maxRPM = 7000f;
    [SerializeField] private float redlineRPM = 6500f;
    [SerializeField] private float engineBraking = 300f;
    [SerializeField] private AnimationCurve powerCurve;
    [SerializeField] private AnimationCurve torqueCurve;
    [SerializeField] private float gearRatio = 3.5f;
    [SerializeField] private float differentialRatio = 4.1f;
    
    private float currentRPM;
    private float currentPower;
    private float currentTorque;
}
```

**Functions:**
```csharp
void Initialize() // Set up engine parameters
void UpdateRPM(float wheelRPM) // Update engine RPM from wheel speed
float CalculatePower(float throttleInput) // Calculate power output
float CalculateTorque(float throttleInput) // Calculate torque output
void SimulateEngineSound(AudioSource engineAudio) // Update engine audio
void ApplyEngineBraking() // Apply engine braking force
float GetPowerToWheels(float throttleInput) // Calculate power to wheels
```

### 2.7. KartStats

Stores and manages kart performance attributes.

**Variables:**
```csharp
public class KartStats : MonoBehaviour
{
    [SerializeField] private float topSpeed = 30f;
    [SerializeField] private float acceleration = 7f;
    [SerializeField] private float handling = 6f;
    [SerializeField] private float weight = 400f;
    [SerializeField] private float traction = 8f;
    [SerializeField] private float driftEfficiency = 5f;
    [SerializeField] private float boostPower = 5f;
}
```

**Functions:**
```csharp
void Initialize(KartType kartType) // Initialize based on kart type
void ApplyUpgrade(UpgradeType upgrade, float amount) // Apply an upgrade
KartStats GetModifiedStats(TerrainType terrain) // Get terrain-modified stats
void DebugLogStats() // Log all stats for debugging
```

## 3. Implementation Details

### 3.1. Physics System Flow

1. Each frame, the system operates as follows:
   - `KartController.Update()` processes non-physics updates
   - `KartController.FixedUpdate()` handles physics calculations
   - Input is collected from `KartInput`
   - Engine forces are calculated by `KartEngine`
   - Forces are applied through `KartPhysics`
   - Wheels and suspension are updated
   - Visual elements are updated

### 3.2. Wheel Physics Implementation

The wheel physics simulation will use a simplified model:

```csharp
// Inside KartWheel.CalculateForwardForce()
private Vector3 CalculateForwardForce()
{
    // Calculate wheel slip
    float slip = CalculateSlip();
    
    // Forward force is based on slip, grip and weight on wheel
    float forceAmount = slip * grip * mass * Physics.gravity.magnitude;
    
    // Clamp maximum force
    forceAmount = Mathf.Clamp(forceAmount, -maxForce, maxForce);
    
    // Convert to vector in world space
    return transform.forward * forceAmount;
}
```

### 3.3. Drift Mechanics

Drifting is implemented by reducing rear wheel grip when conditions are met:

```csharp
// Inside KartController.CheckForDrift()
private void CheckForDrift()
{
    // Check if drift button is pressed, kart is turning, and speed is sufficient
    if (input.IsDrifting() && Mathf.Abs(input.GetSteering()) > 0.5f && currentSpeed > 10f)
    {
        isDrifting = true;
        
        // Reduce grip on rear wheels
        foreach (KartWheel wheel in wheels)
        {
            if (!wheel.isSteering) // If it's a rear wheel
            {
                wheel.SetGripMultiplier(physics.driftFactor);
            }
        }
        
        // Increase steering response
        turnAngle = input.GetSteering() * physics.turnStrength * 1.2f;
        
        // Activate visual effects (particles, skid marks)
        ActivateDriftEffects();
    }
    else
    {
        isDrifting = false;
        
        // Reset grip on all wheels
        foreach (KartWheel wheel in wheels)
        {
            wheel.SetGripMultiplier(1.0f);
        }
        
        // Normal steering
        turnAngle = input.GetSteering() * physics.turnStrength;
        
        // Deactivate visual effects
        DeactivateDriftEffects();
    }
}
```

### 3.4. Suspension System

The suspension uses a spring-damper system:

```csharp
// Inside KartSuspension.CalculateSuspensionForce()
private Vector3 CalculateSuspensionForce()
{
    // Calculate spring force (F = k * x)
    springForce = springStiffness * (restLength - currentLength);
    
    // Calculate damper force (F = c * v)
    float velocity = (previousLength - currentLength) / Time.fixedDeltaTime;
    damperForce = damperStiffness * velocity;
    
    // Total force
    float totalForce = springForce + damperForce;
    
    // Convert to world space vector
    return transform.up * totalForce;
}
```

## 4. Integration with Unity Physics

The system operates within Unity's built-in physics engine but uses custom force calculations:

```csharp
// Inside KartPhysics.ApplyForces()
public void ApplyForces(float throttle, float steering, float brake)
{
    // Calculate drive force from engine
    Vector3 driveForce = transform.forward * engine.GetPowerToWheels(throttle);
    
    // Apply to rigidbody
    rb.AddForce(driveForce, ForceMode.Force);
    
    // Apply steering torque
    float turnTorque = steering * turnStrength * rb.mass;
    rb.AddTorque(transform.up * turnTorque, ForceMode.Force);
    
    // Apply suspension forces from each wheel
    foreach (KartWheel wheel in wheels)
    {
        // Get wheel forces
        Vector3 wheelForce = wheel.CalculateForwardForce() + wheel.CalculateLateralForce();
        
        // Apply at wheel position
        rb.AddForceAtPosition(wheelForce, wheel.transform.position, ForceMode.Force);
        
        // Apply braking
        if (brake > 0)
        {
            wheel.ApplyBraking(brake);
        }
    }
    
    // Apply custom gravity and drag
    ApplyGravity();
    ApplyDrag();
}
```

## 5. Performance Considerations

- Physics calculations are limited to FixedUpdate
- Raycasts use layermasks to only hit relevant objects
- Wheel collisions are simplified to raycasts rather than using colliders
- LOD (Level of Detail) for physics can be implemented based on distance from camera

This technical specification provides a complete framework for implementing realistic kart physics in Unity with all necessary classes, variables, and functions clearly defined. The system is designed to be modular, allowing for easy tuning and expansion while maintaining good performance.

# Kart Racing Game - Physics and Controls

## Overview
This document outlines the core physics and control systems for our kart racing game.

[copy this lmao](https://www.youtube.com/watch?v=cqATTzJmFDY)

## Kart Physics

### State Variables
The kart will maintain basic properties like position, rotation, and velocity vectors to track its state in the game world. There'll also be variables such as kart maximum speed, weight, etc that can be altered by power ups.

### Movement System
Will handle acceleration and deceleration based on player input. Implements basic physics concepts like friction and drag to create responsive yet fun arcade-style handling.

### Steering System
Controls how the kart turns and handles. Will include drift mechanics that balance realism with gameplay fun.

### Collision System
Utilizes Unity's built-in physics but with custom parameters to collisions that aren't frustrating.

## Control System

### Input Handling
Takes in user input directly to the kart physics to change how the kart will be controlled. These inputs will also be accesible as variables for later incase we add logic to make the wheels turn with the kart turning

### Physics Update
Main loop that processes inputs, calculates physics changes, and updates the kart state each frame. (most of this is handled behind the scenes by Unity, we just need to declare an update method)

## Power-up System

### Implementation
There will be an interface for powerups to temporarily modify kart variables such as max speed, acceleration, or handling characteristics.

---

# Kart Controls and technical details

## Player Movement System

```Note, all values given for each input are not final```

### Speed Management
- **Maximum Speed**: Depends on which car player is using. Between ~80mph and ~100mph
- **Rate of Acceleration**: Also depends on which car player is using
  - Should be somewhere between 20mph per second and 40mph per second
- **Deceleration**: Natural deceleration at all times (loses ~5% of current velocity per second)
    - Different karts can be more or less aerodynamic
- **Braking Deceleration**: Rapid deceleration when braking (-20mph per second)

### Turning Mechanics
- **Base Turn Speed**: ~45 degrees per second. But depends on handling
- **Focus Power**: However, as with all movements, this is increased by x1.5 if you're using the focus power
- **Turn Responsiveness**: 0.2 second delay between input and maximum turn angle
- **Stability Control**: Auto-adjusts kart stability when turning at high speeds

## Kart Physics System

### Physics Model Type
- **Simplified Arcade Physics**: Modified realistic physics model with focus on fun over simulation
- **Physics Update Rate**: 60 physics updates per second for consistent behavior across different hardware

### Surface Interaction
- **Surface Materials Database**:
  - Stone: Base speed (1.0)
  - Grass: Reduced speed (0.7)
  - Metal: Reduced speed (0.9)
- **Weather Effects on Physics**:
  - Rain: Reduced handling due to wet ground (-15%)
  - Heat: Increased handling due to better tire grip (+10%)
  - Snow: Speed reduction (-20%)

### Collision Response System
- **Kart Collisions**: (utilizes Unity's built in engine
  - Varied response based on collision velocity and angle (general karts physics)
  - Speed reduction on impact (based on collision angle)
  - Recovery assistance system to help flipped karts

## User Controls

### Core Control Scheme

#### Keyboard Default Controls
- **W**: Accelerate
- **S**: Brake/Reverse
- **A/D**: Turn Left/
- **Space**: Jump
- **Shift**: Use Collected Powerup
- **E**: Activate Teacher Special Ability
- **F**: Fire powerup or special ability (after activation)
- **P**: Activate powerup
- **V**: Push to talk in voice chat
- **M**: Mutes voice chat in multiplayer
- **Tab**: View Minimap with players as dots
- **Esc**: Pause Menu
### Visual Control Feedback
- **Speed Display**: Speedometer in corner showing current speed
- **Teacher Ability Cooldown**: Visual timer for special ability availability

### Special Controls

#### Teacher Special Abilities
- **Activation**: Press shift when ability is available
- **Cooldown System**: Each ability has a specific cooldown period before reuse
- **Targeting Controls** (for powerups/abilities that require aiming):
  - Aim will sweep around in a circle
  - Press f to fire the ability in the current aim direction

### User Interface Controls
- **Menu Navigation**: Arrow keys/WASD
- **Selection**: Enter
- **Back/Cancel**: Esc

## Relation to other core components

### Weather
- Weather affects friction, speed, and in hard mode, affects damage
- As described above

### Varying kart options
- Affects top speed, handling, aerodynamics, acceleration
- As described above

### Minimap
- Press tab to see minimap with each player as a dot on it

### Multiplayer
- Voice chat is available at all times based upon pressing V, regardless of distance to other cars

---

# Code Implementation Preview

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

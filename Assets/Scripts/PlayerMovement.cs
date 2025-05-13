using UnityEngine;

/// <summary>
/// Lightweight arcade‑style kart controller that does not rely on a Rigidbody.
/// Hold the drift key (Left Shift) while steering to initiate a drift.
/// Releasing the drift key gives a small speed boost proportional to drift time.
/// </summary>
public class KartController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float acceleration = 18f;
    [SerializeField] private float brakingDeceleration = 25f;
    [SerializeField] private float naturalDeceleration = 8f;
    [SerializeField] private float maxForwardSpeed = 22f;
    [SerializeField] private float maxReverseSpeed = 6f;

    [Header("Steering")]
    [SerializeField] private float steerSpeed = 155f;
    [Tooltip("0 → low speed, 1 → max speed. Value multiplies steerSpeed.")]
    [SerializeField] private AnimationCurve steerSpeedVsVelocity = AnimationCurve.EaseInOut(0f, 1f, 1f, 0.3f);

    [Header("Drift")]
    [SerializeField] private KeyCode driftKey = KeyCode.LeftShift;
    [SerializeField] private float driftExtraSteer = 35f;
    [SerializeField] private float driftFriction = 3f;
    [SerializeField] private float driftBoostMultiplier = 1.25f;
    [SerializeField] private float maxDriftTimeForBoost = 2.5f;

    private float currentSpeed;          // signed m/s ( + forward , − backward )
    private bool  isDrifting;
    private int   driftDirection;        // –1 = left, 1 = right
    private float driftTimer;

    private void Update()
    {
        float accelInput = Input.GetAxis("Vertical");     // –1…1
        float steerInput = Input.GetAxis("Horizontal");   // –1…1

        HandleSpeed(accelInput);
        HandleDrift(steerInput);
        HandleSteering(steerInput);

        // apply movement
        Vector3 velocity = transform.forward * currentSpeed;
        transform.position += velocity * Time.deltaTime;

        // simulate slide friction when drifting: slowly realign forward direction toward velocity
        if (isDrifting)
        {
            Vector3 forwardFlat = transform.forward;
            Vector3 velocityDir = velocity.sqrMagnitude > 0.01f ? velocity.normalized : forwardFlat;
            float realign = driftFriction * Time.deltaTime;
            transform.forward = Vector3.Slerp(forwardFlat, velocityDir, realign);
        }
    }

    private void HandleSpeed(float accelInput)
    {
        if (accelInput > 0f)
        {
            currentSpeed += accelInput * acceleration * Time.deltaTime;
            currentSpeed = Mathf.Clamp(currentSpeed, -maxReverseSpeed, maxForwardSpeed);
        }
        else if (accelInput < 0f)
        {
            currentSpeed += accelInput * brakingDeceleration * Time.deltaTime;
            currentSpeed = Mathf.Clamp(currentSpeed, -maxReverseSpeed, maxForwardSpeed);
        }
        else
        {
            // neither accelerating nor braking – apply natural drag
            float decel = naturalDeceleration * Mathf.Sign(currentSpeed) * Time.deltaTime;
            if (Mathf.Abs(decel) > Mathf.Abs(currentSpeed))
                currentSpeed = 0f;
            else
                currentSpeed -= decel;
        }
    }

    private void HandleSteering(float steerInput)
    {
        // add drift steering bonus
        float steerAmount = steerInput + (isDrifting ? driftDirection * driftExtraSteer / steerSpeed : 0f);

        // scale steer speed by velocity (less turning at high speed)
        float velRatio = Mathf.InverseLerp(0f, maxForwardSpeed, Mathf.Abs(currentSpeed));
        float steerCurve = steerSpeedVsVelocity.Evaluate(velRatio);
        float finalSteerSpeed = steerSpeed * steerCurve;

        transform.Rotate(0f, steerAmount * finalSteerSpeed * Time.deltaTime, 0f, Space.World);
    }

    private void HandleDrift(float steerInput)
    {
        bool wantsToDrift = Input.GetKey(driftKey);

        if (wantsToDrift && !isDrifting && Mathf.Abs(steerInput) > 0.1f && currentSpeed > 4f)
        {
            // start drift
            isDrifting    = true;
            driftDirection = steerInput > 0 ? 1 : -1;
            driftTimer     = 0f;
        }

        if (isDrifting)
        {
            driftTimer += Time.deltaTime;

            // end drift
            if (!wantsToDrift)
            {
                isDrifting = false;

                // calculate boost
                float t     = Mathf.Clamp01(driftTimer / maxDriftTimeForBoost);
                float boost = Mathf.Lerp(1f, driftBoostMultiplier, t);
                currentSpeed = Mathf.Min(currentSpeed * boost, maxForwardSpeed * 1.2f);
            }
        }
    }
}

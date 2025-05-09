using UnityEngine;

public class KartDriftController : MonoBehaviour
{
    public float driftFactor = 0.95f;
    public float normalGrip = 1f;
    public float driftGrip = 0.3f;
    public float maxDriftAngle = 30f;
    public KeyCode driftKey = KeyCode.Space;

    private Rigidbody rb;
    private bool isDrifting;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
        float driftInput = Input.GetKey(driftKey) ? 1 : 0;
        isDrifting = driftInput > 0;

        // Reduce lateral grip while drifting
        float currentGrip = isDrifting ? driftGrip : normalGrip;
        Vector3 sidewaysVelocity = transform.right * Vector3.Dot(rb.velocity, transform.right);
        rb.velocity -= sidewaysVelocity * (1 - currentGrip);

        // Optionally: Add visual tilt or skid effects here
    }
}

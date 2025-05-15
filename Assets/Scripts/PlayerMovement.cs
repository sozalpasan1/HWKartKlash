using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(NetworkObject))]
public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float movementSpeed = 25f;
    [SerializeField] private float maxVelocity    = 15f;
    [SerializeField] private float groundDrag     = 1.5f;
    [SerializeField] private float groundCheckDist = 1.1f;
    [SerializeField] private LayerMask groundMask;

    private Rigidbody rb;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;                     // prevent tipping
    }

    public override void OnNetworkSpawn()
    {
        // Only the owner should run this component at all
        if (!IsOwner)
            enabled = false;
    }

    private void Update()
    {
        // Ground check
        isGrounded = Physics.Raycast(transform.position, Vector3.down,
                                     groundCheckDist, groundMask);

        // Apply drag when grounded
        rb.linearDamping = isGrounded ? groundDrag : 0f;

        // Clamp horizontal speed
        Vector3 horizVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        if (horizVel.magnitude > maxVelocity)
        {
            Vector3 limited = horizVel.normalized * maxVelocity;
            rb.linearVelocity = new Vector3(limited.x, rb.linearVelocity.y, limited.z);
        }
    }

    private void FixedUpdate()
    {
        // now read input and move
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 dir = new Vector3(h, 0, v);

        if (dir.sqrMagnitude > 1f)
            dir.Normalize();

        if (isGrounded)
            rb.AddForce(dir * movementSpeed, ForceMode.VelocityChange);
    }
}

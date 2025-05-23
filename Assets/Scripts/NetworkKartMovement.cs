using Unity.Netcode;
using UnityEngine;

public class NetworkKartMovement : NetworkBehaviour
{
    [SerializeField] private float speed = 7f;
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private float driftFactor = 0.95f;
    [SerializeField] private float maxSpeed = 15f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float brakeForce = 15f;

    private float currentSpeed = 0f;
    private float horizontalInput = 0f;
    private float verticalInput = 0f;
    private Rigidbody rb;

    private NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>();
    private NetworkVariable<Quaternion> networkRotation = new NetworkVariable<Quaternion>();

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            // Local player initialization
            Debug.Log("I am the owner of this kart!");
            
            // Make sure we have a Rigidbody
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
                rb.mass = 100f;
                rb.linearDamping = 1f;
                rb.angularDamping = 5f;
                rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            }
        }
        else
        {
            // Non-owner initialization (for other players' karts)
            Debug.Log("This is another player's kart");
        }
    }

    void Update()
    {
        if (IsOwner)
        {
            // Get input
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
        }
        else
        {
            // Update position/rotation for non-owners (network interpolation)
            transform.position = Vector3.Lerp(transform.position, networkPosition.Value, Time.deltaTime * 10f);
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation.Value, Time.deltaTime * 10f);
        }
    }

    void FixedUpdate()
    {
        if (IsOwner)
        {
            MoveKart();
            UpdateNetworkPositionServerRpc(transform.position, transform.rotation);
        }
    }

    private void MoveKart()
    {
        // Acceleration/Braking
        if (verticalInput > 0)
        {
            currentSpeed += acceleration * Time.fixedDeltaTime;
        }
        else if (verticalInput < 0)
        {
            currentSpeed -= brakeForce * Time.fixedDeltaTime;
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.fixedDeltaTime);
        }

        // Clamp speed
        currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed/2, maxSpeed);

        // Apply forward movement
        Vector3 forwardMovement = transform.forward * currentSpeed;
        
        // Apply drift - sideways movement based on turning
        Vector3 rightMovement = transform.right * horizontalInput * Mathf.Abs(currentSpeed) * driftFactor;
        
        // Combine movements
        Vector3 movement = forwardMovement + rightMovement;
        rb.linearVelocity = movement;

        // Apply rotation
        if (Mathf.Abs(currentSpeed) > 0.1f)
        {
            float turnAmount = horizontalInput * rotationSpeed * Time.fixedDeltaTime;
            Quaternion turnRotation = Quaternion.Euler(0, turnAmount, 0);
            rb.MoveRotation(rb.rotation * turnRotation);
        }
    }

    [ServerRpc]
    private void UpdateNetworkPositionServerRpc(Vector3 position, Quaternion rotation)
    {
        networkPosition.Value = position;
        networkRotation.Value = rotation;
    }
}

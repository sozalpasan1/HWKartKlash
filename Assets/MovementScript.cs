using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MinimalWalk : MonoBehaviour
{
    public float speed = 2f;               // Walking speed (tweak for minimal feel)
    public float rotationSpeed = 360f;     // How fast the player rotates toward movement

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Lock rotation on X/Z so the player doesn't fall over
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void FixedUpdate()
    {
        // Get input
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // Create move direction relative to world
        Vector3 moveDirection = new Vector3(h, 0, v).normalized;

        // Move the rigidbody by directly setting velocity (preserve gravity)
        Vector3 currentVelocity = rb.linearVelocity;
        Vector3 targetVelocity = moveDirection * speed;
        rb.linearVelocity = new Vector3(targetVelocity.x, currentVelocity.y, targetVelocity.z);

        // Rotate player to face movement direction
        if (moveDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            rb.MoveRotation(newRotation);
        }
    }
}
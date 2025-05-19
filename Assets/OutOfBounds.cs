using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    private Vector3 respawnPoint;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Start position is initial respawn point
        respawnPoint = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        // If the player touches a checkpoint, update respawn position
        if (other.CompareTag("Checkpoint"))
        {
            respawnPoint = other.transform.position;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // If the player exits the bounds trigger, respawn
        if (other.CompareTag("Bounds"))
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        // Teleport to checkpoint
        transform.position = respawnPoint;

        // Reset velocity to stop unwanted momentum
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}

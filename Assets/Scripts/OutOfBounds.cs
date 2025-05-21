using System.Collections.Generic;
using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    private Vector3 respawnPoint;
    private Rigidbody rb;

    // Use HashSet to track which Bounds colliders we're inside
    private HashSet<Collider> currentBounds = new HashSet<Collider>();

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        respawnPoint = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            respawnPoint = other.transform.position;
        }

        if (other.CompareTag("Bounds"))
        {
            currentBounds.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Bounds"))
        {
            currentBounds.Remove(other);

            // Only respawn if we're no longer inside ANY bounds
            if (currentBounds.Count == 0)
            {
                Respawn();
            }
        }
    }

    private void Respawn()
    {
        transform.position = respawnPoint;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Clear and reset bounds to be safe
        currentBounds.Clear();
    }
}

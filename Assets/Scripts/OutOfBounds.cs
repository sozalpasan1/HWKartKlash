using System.Collections.Generic;
using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    private Vector3 respawnPoint;
    private Rigidbody rb;

    // Track how many bounds the player is inside
    private int boundsCount = 0;

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
            boundsCount++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Bounds"))
        {
            boundsCount--;

            // Only respawn if player is no longer in any Bounds colliders
            if (boundsCount <= 0)
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

        boundsCount = 0; // Reset in case something weird happens
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRespawn : MonoBehaviour
{
    public Vector3 playerPosition;
    [SerializeField] List<GameObject> checkpoints;
    [SerializeField] Vector3 vectorPoint;

    [SerializeField] private float fallThreshold = -10f; // Customize this as needed

    private Rigidbody rb;

    void Start()
    {
        playerPosition = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Check if player has fallen below threshold
        if (transform.position.y < fallThreshold)
        {
            Respawn();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            vectorPoint = other.transform.position;
            playerPosition = vectorPoint;
        }
    }

    private void Respawn()
    {
        transform.position = playerPosition;
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero; // Reset velocity
            rb.angularVelocity = Vector3.zero; // Optional: Reset spinning
        }
    }
}

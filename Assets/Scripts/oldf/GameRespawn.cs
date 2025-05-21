using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRespawn : MonoBehaviour
{
    public Vector3 playerPosition;
    [SerializeField] List<GameObject> checkpoints;
    [SerializeField] Vector3 vectorPoint;

    void Start()
    {
        // Initialize the spawn position to the starting point of the player
        playerPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            vectorPoint = other.transform.position;
            playerPosition = vectorPoint;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Bounds")) // Your custom-shaped bounds should have this tag
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        transform.position = playerPosition;
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
        }
    }
}
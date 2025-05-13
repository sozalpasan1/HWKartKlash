using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRespawn : MonoBehaviour
{
    public float threshold;
    public Vector3 playerPosition;
    [SerializeField] List<GameObject> checkpoints;
    [SerializeField] Vector3 vectorPoint;

    void FixedUpdate()
    {
        if (transform.position.y < threshold) {
            // transform.position = new Vector3(-0.3037853f, 2.1f, -10.92337f);
            transform.position = new Vector3(playerPosition.x, playerPosition.y, playerPosition.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Checkpoint"))
        {
            vectorPoint = other.transform.position;
            playerPosition = vectorPoint;
        }
    }
}

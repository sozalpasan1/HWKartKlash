using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRespawn : MonoBehaviour
{
    public float threshold;

    void FixedUpdate()
    {
        if (transform.position.y < threshold) {
            transform.position = new Vector3(-0.3037853f, 2.1f, -10.92337f);
        }
    }
}

using UnityEngine;

/// <summary>
/// Keeps the camera a fixed offset from its target and yaws left/right
/// when the user presses the ← / → arrow keys.
/// </summary>
public class ArrowOrbitCamera : MonoBehaviour
{
    public Transform target;                 // Drag the player here
    public Vector3   offset = new Vector3(0f, 2f, -6f);
    public float     yawSpeed = 90f;         // degrees per second

    private float currentYaw;                // accumulated yaw angle

    void LateUpdate()
    {
        if (!target) return;

        // 1. Read arrow‑key input  (‑1 left, +1 right)
        float yawInput = 0f;
        if      (Input.GetKey(KeyCode.LeftArrow))  yawInput = -1f;
        else if (Input.GetKey(KeyCode.RightArrow)) yawInput =  1f;

        currentYaw += yawInput * yawSpeed * Time.deltaTime;

        // 2. Position & rotate camera around the target
        Quaternion rot = Quaternion.Euler(0f, currentYaw, 0f);
        transform.position = target.position + rot * offset;
        transform.LookAt(target.position);
    }
}

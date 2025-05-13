using UnityEngine;

/// <summary>
/// Moves the scene camera so that it follows the GameObject
/// this script is attached to (typically your Player).
/// </summary>
public class PlayerCamera : MonoBehaviour
{
    [Header("Camera target & offset")]
    public Camera cam;                    // Drag the Main Camera here in the Inspector
    public Vector3 offset = new Vector3(0f, 2f, -5f);

    [Header("Follow behaviour")]
    [Range(0f, 1f)] public float smoothTime = 0.1f;   // 0  = snap;  1 = very slow
    private Vector3 velocity = Vector3.zero;

    [Header("Optional mouse orbit")]
    public bool enableOrbit = true;
    public float mouseSensitivity = 3f;   // degrees per pixel
    public float pitchMin = -30f, pitchMax = 70f;      // vertical look clamps
    private float yaw, pitch;

    void Awake()
    {
        if (cam == null) cam = Camera.main;
        Vector3 localRot = cam.transform.rotation.eulerAngles;
        yaw = localRot.y;
        pitch = localRot.x;
    }

    void LateUpdate()
    {
        if (cam == null) return;

        // 1. Optional orbit: adjust offset based on mouse input
        Quaternion orbitRot = Quaternion.identity;
        if (enableOrbit && Input.GetMouseButton(1))           // Right‑mouse drag
        {
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);
        }
        orbitRot = Quaternion.Euler(pitch, yaw, 0f);

        // 2. Desired camera position and rotation
        Vector3 desiredPos = transform.position + orbitRot * offset;

        // 3. Smoothly move camera into position
        cam.transform.position = Vector3.SmoothDamp(
            cam.transform.position, desiredPos, ref velocity, smoothTime);

        // 4. Always look at the player
        cam.transform.rotation = Quaternion.LookRotation(
            transform.position + Vector3.up * 1.2f - cam.transform.position); // look slightly above feet
    }
}

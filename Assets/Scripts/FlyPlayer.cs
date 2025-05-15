using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FlyPlayer : MonoBehaviour
{
    [Header("Speeds")]
    public float horizontalSpeed = 6f;   // WASD in the X‑Z plane
    public float verticalSpeed   = 6f;   // Space / LeftShift up‑down speed

    private CharacterController cc;

    void Awake() => cc = GetComponent<CharacterController>();

    void Update()
    {
        // --- 1. Collect input ---------------------------------------------
        float h = Input.GetAxis("Horizontal");   // A / D (left / right)
        float v = Input.GetAxis("Vertical");     // W / S (forward / back)

        float rise = 0f;                         // Space = up, L‑Shift = down
        if (Input.GetKey(KeyCode.Space))       rise += 1f;
        if (Input.GetKey(KeyCode.LeftShift))   rise -= 1f;

        // --- 2. Build a movement vector in local space --------------------
        Vector3 move =
              transform.right   * h
            + transform.forward * v
            + transform.up      * rise * (verticalSpeed / horizontalSpeed);

        // --- 3. Move without gravity --------------------------------------
        cc.Move(move * horizontalSpeed * Time.deltaTime);
    }
}

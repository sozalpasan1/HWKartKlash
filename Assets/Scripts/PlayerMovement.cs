using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Tuning")]
    public float moveSpeed = 6f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    private CharacterController cc;
    private Vector3 velocity;

    void Awake() => cc = GetComponent<CharacterController>();

    void Update()
    {
        // Ground check
        bool grounded = cc.isGrounded;
        if (grounded && velocity.y < 0) velocity.y = -2f;

        // WASD / arrow keys (uses the default “Horizontal” & “Vertical” axes)
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 move = transform.right * h + transform.forward * v;
        cc.Move(move * moveSpeed * Time.deltaTime);

        // Jump
        if (grounded && Input.GetButtonDown("Jump"))
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        // Gravity
        velocity.y += gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);
    }
}

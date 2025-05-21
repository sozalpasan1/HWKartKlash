using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ArrowKeyMovement : MonoBehaviour
{
    public float forceAmount = 10f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 force = Vector3.zero;

        if (Input.GetKey(KeyCode.UpArrow))
            force += Vector3.forward;
        if (Input.GetKey(KeyCode.DownArrow))
            force += Vector3.back;
        if (Input.GetKey(KeyCode.LeftArrow))
            force += Vector3.left;
        if (Input.GetKey(KeyCode.RightArrow))
            force += Vector3.right;

        rb.AddForce(force * forceAmount);
    }
}

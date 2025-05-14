using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour
{
    public float speed;
    public float rotationSpeed;
    public float jumpSpeed;
    private float ySpeed;
    private CharacterController controller;
    public bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float horizontalMove = Input.GetAxis("Horizontal");
        float verticalMove = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontalMove, 0, verticalMove);
        moveDirection.Normalize();
        float magnitude = moveDirection.magnitude;
        magnitude = Mathf.Clamp01(magnitude);
        // transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
        controller.SimpleMove(moveDirection * magnitude * speed);

        ySpeed += Physics.gravity.y * Time.deltaTime;
        if (Input.GetButtonDown("Jump")) {
            ySpeed = -0.5f;
        }

        Vector3 vel = moveDirection * magnitude;
        vel.y = ySpeed;
        // transform.Translate(vel * Time.deltaTime);
        controller.Move(vel * Time.deltaTime);

        if (controller.isGrounded)
        {
            ySpeed = -0.5f;
            isGrounded = true;
            if (Input.GetButtonDown("Jump")) {
                ySpeed = jumpSpeed;
                isGrounded = false;
            }
        }

        if (moveDirection != Vector3.zero) {
            Quaternion toRotate = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotate, rotationSpeed * Time.deltaTime);
        }
    }
}

using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 7f;
    [SerializeField] private float rotationSpeed = 500f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // create movement direction
        Vector3 movementDirection = new Vector3 (horizontalInput, 0, verticalInput);
        movementDirection.Normalize();

        // move the player
        transform.Translate(movementDirection * movementSpeed * Time.deltaTime, Space.World);

        
        // rotate the player to face the movement direction
        if (movementDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }

    }
}

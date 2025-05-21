using UnityEngine;

/// <summary>
/// Add this to your kart to help debug movement issues
/// </summary>
public class DebugLogger : MonoBehaviour
{
    void Update()
    {
        // Log input values
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        
        if (Mathf.Abs(horizontalInput) > 0.1f || Mathf.Abs(verticalInput) > 0.1f)
        {
            Debug.Log($"Input: H={horizontalInput:F2}, V={verticalInput:F2}");
        }
        
        // Log position and velocity
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null && rb.linearVelocity.magnitude > 0.1f)
        {
            Debug.Log($"Velocity: {rb.linearVelocity}, Speed: {rb.linearVelocity.magnitude:F2}");
        }
    }
}
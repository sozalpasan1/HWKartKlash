using UnityEngine;
using System;

/// <summary>
/// Add this to your kart to help debug movement issues
/// </summary>
public class DebugLogger : MonoBehaviour
{
    private Rigidbody rb;
    private float lastLogTime;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Debug.Log($"[{DateTime.Now:HH:mm:ss.fff}] DebugLogger initialized on {gameObject.name}");
        if (rb == null)
        {
            Debug.LogError($"[{DateTime.Now:HH:mm:ss.fff}] No Rigidbody found on {gameObject.name}");
        }
    }

    void Update()
    {
        // Only log every 0.5 seconds to avoid spam
        if (Time.time - lastLogTime < 0.5f) return;
        lastLogTime = Time.time;

        // Log input values
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        
        if (Mathf.Abs(horizontalInput) > 0.1f || Mathf.Abs(verticalInput) > 0.1f)
        {
            Debug.Log($"[{DateTime.Now:HH:mm:ss.fff}] Input on {gameObject.name}: H={horizontalInput:F2}, V={verticalInput:F2}");
        }
        
        // Log position and velocity
        if (rb != null && rb.linearVelocity.magnitude > 0.1f)
        {
            Debug.Log($"[{DateTime.Now:HH:mm:ss.fff}] {gameObject.name} - Position: {transform.position}, Velocity: {rb.linearVelocity}, Speed: {rb.linearVelocity.magnitude:F2}");
        }
        else if (rb == null)
        {
            Debug.LogWarning($"[{DateTime.Now:HH:mm:ss.fff}] Rigidbody is null on {gameObject.name}");
        }
    }
}
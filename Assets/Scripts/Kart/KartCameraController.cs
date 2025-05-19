using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartCameraController : MonoBehaviour
{
    [SerializeField] private Vector3 offset = new Vector3(0, 3, -5);
    [SerializeField] private float smoothSpeed = 10f;
    [SerializeField] private float lookAheadFactor = 0.5f;
    
    private Transform target;
    private Vector3 desiredPosition;
    private Quaternion desiredRotation;
    
    private void LateUpdate()
    {
        if (target == null)
        {
            // Find the local player's kart
            KartController[] karts = FindObjectsOfType<KartController>();
            foreach (KartController kart in karts)
            {
                if (kart.IsLocalPlayer)
                {
                    target = kart.transform;
                    break;
                }
            }
            
            if (target == null) return;
        }
        
        // Calculate desired position
        Vector3 desiredPosition = target.position + 
                                 target.forward * offset.z + 
                                 Vector3.up * offset.y + 
                                 target.right * offset.x;
        
        // Smoothly move towards that position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);
        
        // Look ahead of the kart slightly
        Vector3 lookPosition = target.position + target.forward * lookAheadFactor;
        
        // Smoothly rotate towards that position
        Quaternion desiredRotation = Quaternion.LookRotation(lookPosition - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * smoothSpeed);
    }
}
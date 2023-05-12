using System;
using System.Collections;
using System.Collections.Generic;
using ArduinoBluetoothAPI;
using TMPro;
using UnityEngine;

public class ProximityDetector : MonoBehaviour
{
    public string targetTag = "MovingObject";
    public float triggerRadius = 1.0f;
    public TextMeshProUGUI debugText;

    public BLE_manager_Arduino bleServicesManager;

    void Start()
    {
    }

    void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, triggerRadius);
        float minDistance = float.MaxValue;
        Vector3 closestDirection = Vector3.zero;

        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(targetTag))
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestDirection = hitCollider.transform.position - transform.position;
                }
            }
        }

        if (closestDirection != Vector3.zero)
        {
            Vector3 localDirection = transform.InverseTransformDirection(closestDirection);
            localDirection.Normalize();

            int front = localDirection.z > 0 ? 1 : 0;
            int back = localDirection.z < 0 ? 1 : 0;
            int left = localDirection.x < 0 ? 1 : 0;
            int right = localDirection.x > 0 ? 1 : 0;
            int up = localDirection.y > 0 ? 1 : 0;
            int down = localDirection.y < 0 ? 1 : 0;

            // Send motor intensity values based on proximity and direction
            int frontIntensity = front * (int)(255 * (1 - minDistance / triggerRadius));
            int backIntensity = back * (int)(255 * (1 - minDistance / triggerRadius));
            int leftIntensity = left * (int)(255 * (1 - minDistance / triggerRadius));
            int rightIntensity = right * (int)(255 * (1 - minDistance / triggerRadius));

            string motorCommand = $"{frontIntensity},{backIntensity},{leftIntensity},{rightIntensity}";
            UpdateDebugText(motorCommand);

            try
            {
                bleServicesManager.SendMotorCommand(motorCommand);
            }
            catch (Exception ex)
            {
                UpdateDebugText(ex.Message);
            }
            
        }
    }

    void UpdateDebugText(string message)
    {
        if (debugText != null)
        {
            debugText.text = message;
        }
    }
}

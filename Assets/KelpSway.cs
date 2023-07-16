using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KelpSway : MonoBehaviour
{
    public float maxAngle = 5f;
    public float swaySpeed = 1f;

    private Quaternion initialRotation;

    private void Start()
    {
        initialRotation = transform.rotation;
    }

    private void Update()
    {
        // Calculate sway angles
        float angleX = Mathf.Sin(Time.time * swaySpeed) * maxAngle;
        float angleY = Mathf.Cos(Time.time * swaySpeed) * maxAngle;

        // Apply sway rotation
        Quaternion swayRotation = Quaternion.Euler(angleY, angleX, 0f);
        transform.rotation = initialRotation * swayRotation;
    }
}

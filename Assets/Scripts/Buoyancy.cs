using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buoyancy : MonoBehaviour
{
    public float waterVelocityDrag = 0.99f;
    public float waterAngularDrag = 0.5f;

    [Range(1, 5)] public float strength;
    [Range(1, 5)] public float objectDepth;

    public bool active = true;

    #region Private Fields
    private int floaters;
    [HideInInspector]
    public Rigidbody rb;
    #endregion

    private void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
        floaters = transform.parent.childCount;

        if (rb.useGravity)
            rb.useGravity = false;
    }

    private void FixedUpdate()
    {
        if (rb == null)
            return;

        // Manual gravity subdivided based on the amount of floaters.
        rb.AddForceAtPosition((Physics.gravity * rb.mass) / floaters, transform.position, ForceMode.Acceleration);

        if (!active)
            return;

        float wH = WaveManager.instance.getHeight(transform.position.x, transform.position.z);

        // If the floater is below water
        if (transform.position.y < wH)
        {
            float submersion = Mathf.Clamp01(wH - transform.position.y) / objectDepth;
            float buoyancy = Mathf.Abs(Physics.gravity.y) * submersion * strength * rb.mass;

            // Buoyant Force
            rb.AddForceAtPosition(Vector3.up * buoyancy, transform.position, ForceMode.Acceleration);

            // Drag Force
            rb.AddForce(-rb.velocity * waterVelocityDrag * Time.fixedDeltaTime * strength, ForceMode.VelocityChange);

            // Torque Force
            rb.AddTorque(-rb.angularVelocity * waterAngularDrag * Time.fixedDeltaTime * strength, ForceMode.VelocityChange);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoverController : MonoBehaviour
{
    private Camera cam;
    private Rigidbody rb;

    private float roverSpeed = 10.0f;
    private float speedMult = 1.0f;
    private Vector3 moveDir;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cam = Camera.main;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        moveDir = cam.transform.forward * PlayerInputManager.instance.inputY + cam.transform.right * PlayerInputManager.instance.inputX;
    }

    void FixedUpdate()
    {
        rb.AddForce(moveDir.normalized * roverSpeed * speedMult, ForceMode.Acceleration);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class RoverController : MonoBehaviour
{
    private CinemachineImpulseSource impulse;
    private Rigidbody rb;
    public Transform folloCam;

    public LayerMask ground;
    private bool isGrounded = false;
    private bool landed = true;
    private float groundOffset = 3f;
    private float coyoteTime = 0.2f;
    private float coyoteCounter;
    private float roverSpeed = 10.0f;
    private float speedMult = 4.0f;
    private float jumpForce = 500f;
    private Vector3 moveDir;

    public GameObject scanner;
    private float landSpeed;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        impulse = GetComponent<CinemachineImpulseSource>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(new Vector3(transform.position.x, transform.position.y - groundOffset, transform.position.z), 0.1f, ground);

        if (isGrounded)
        {
            if (folloCam != null)
                moveDir = folloCam.transform.forward * -PlayerInputManager.instance.inputX + folloCam.transform.right * PlayerInputManager.instance.inputY;

            coyoteCounter = coyoteTime;
        }
        else
            coyoteCounter -= Time.deltaTime;

        if (Mathf.Abs(rb.velocity.y) > 0.1f)
            landSpeed = Mathf.Abs(rb.velocity.y);

        if (PlayerInputManager.instance.scan && scanner != null)
            Instantiate(scanner, transform.position, Quaternion.identity);
    }

    void FixedUpdate()
    {
        if (rb == null)
            return;

        rb.AddTorque(moveDir.normalized * roverSpeed * speedMult);
        if (isGrounded && PlayerInputManager.instance.jump)
            rb.AddForce(Vector3.up * jumpForce);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(landSpeed > 4f)
        {
            impulse.m_DefaultVelocity *= Mathf.Clamp(landSpeed / 10f, 0f, 6f);
            impulse.GenerateImpulse();
            impulse.m_DefaultVelocity = new Vector3(-1f, -1f, -1f);
        }
    }
}

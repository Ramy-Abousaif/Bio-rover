using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class RoverController : MonoBehaviour
{
    private CinemachineImpulseSource impulse;
    private Rigidbody rb;
    private SphereCollider sc;
    public Transform followCam;
    public GameObject draw;

    public LayerMask ground;
    private bool isGrounded = false;
    private bool landed = true;
    private float coyoteTime = 0.2f;
    private float scanCooldown = 2.0f;
    private float scanTimer = 0.0f;
    private float coyoteCounter;
    private float roverSpeed = 10.0f;
    private float speedMult = 4.0f;
    private float jumpForce = 500f;
    private Vector3 moveDir;
    private float maxSlopLimit = 12.5f;

    private float initialRadius = 1.5f;
    private float targetRadius = 3f;
    private Vector3 initialSize = new Vector3(3, 3, 3);
    private Vector3 targetSize = new Vector3(6, 6, 6);
    private float sizeShiftTime = 1.5f;
    private bool isChangingSize = false;

    public GameObject scanner;
    private float landSpeed;
    private float currentSpeed;
    private float prevSpeed;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        impulse = GetComponent<CinemachineImpulseSource>();
        rb = GetComponent<Rigidbody>();
        sc = GetComponent<SphereCollider>();
        scanTimer = scanCooldown;
        sc.radius = initialRadius;
        draw.transform.localScale = initialSize;
        if (followCam != null)
            rb.AddTorque(followCam.transform.forward * 25.0f);
    }

    void Update()
    {
        scanTimer += Time.deltaTime;
        float currentSpeed = Mathf.Sqrt((rb.velocity.x * rb.velocity.x) + (rb.velocity.z * rb.velocity.z));
        isGrounded = Physics.CheckSphere(new Vector3(transform.position.x, transform.position.y - sc.radius, transform.position.z), 0.5f, ground);

        if (isGrounded)
        {
            if (followCam != null)
                moveDir = followCam.transform.forward * -PlayerInputManager.instance.inputX + followCam.transform.right * PlayerInputManager.instance.inputY;

            coyoteCounter = coyoteTime;
        }
        else
            coyoteCounter -= Time.deltaTime;

        if (currentSpeed > 8f)
            prevSpeed = currentSpeed;

        if (Mathf.Abs(rb.velocity.y) > 0.1f)
            landSpeed = Mathf.Abs(rb.velocity.y);

        if (PlayerInputManager.instance.scan && scanner != null && scanTimer >= scanCooldown)
        {
            Instantiate(scanner, transform.position, Quaternion.identity);
            AudioManager.Instance.PlayOneShotWithParameters("Sonar", transform, ("Occluded", (transform.position.y > WaveManager.instance.getHeight(transform.position.x, transform.position.z)) ? 0f : 1f));
            scanTimer = 0.0f;
        }

        if (Input.GetKeyDown(KeyCode.E) && !isChangingSize)
            StartCoroutine(ChangeSize());

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, sc.radius + 1f, ground))
        {
            Debug.Log((Vector3.Angle(Vector3.up, hit.normal)));
            if ((Vector3.Angle(Vector3.up, hit.normal)) > maxSlopLimit + 0.1f)
            {
                var left = Vector3.Cross(hit.normal, Vector3.up);
                var slope = Vector3.Cross(hit.normal, left);
                rb.AddForce(slope * 50f * rb.mass);
            }
        }
    }

    void FixedUpdate()
    {
        if (rb == null)
            return;

        if (isChangingSize)
            return;

        rb.AddTorque(moveDir.normalized * roverSpeed * speedMult * rb.mass);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(landSpeed > 4f || prevSpeed > 10f)
            Shake();

        if(!isChangingSize)
            AudioManager.Instance.PlayOneShotWithParameters("BallLand", transform, ("Occluded", (transform.position.y > WaveManager.instance.getHeight(transform.position.x, transform.position.z)) ? 0f : 1f));
    }

    void Shake()
    {
        impulse.m_DefaultVelocity *= Mathf.Clamp(landSpeed / 10f, 0f, 6f);
        impulse.GenerateImpulse();
        impulse.m_DefaultVelocity = new Vector3(-1f, -1f, -1f);
    }

    IEnumerator ChangeSize()
    {
        isChangingSize = true;
        float time = 0f;

        if (initialRadius > targetRadius)
        {
            AudioManager.Instance.PlayOneShotWithParameters("Deflate", transform, ("Occluded", (transform.position.y > WaveManager.instance.getHeight(transform.position.x, transform.position.z)) ? 0f : 1f));
            maxSlopLimit = 12.5f;
        }
        else
        {
            AudioManager.Instance.PlayOneShotWithParameters("Inflate", transform, ("Occluded", (transform.position.y > WaveManager.instance.getHeight(transform.position.x, transform.position.z)) ? 0f : 1f));
            maxSlopLimit = 25f;
        }

        while (time < sizeShiftTime)
        {
            float newRadius = Mathf.Lerp(initialRadius, targetRadius, time / sizeShiftTime);
            Vector3 newSize = Vector3.Lerp(initialSize, targetSize, time / sizeShiftTime);
            sc.radius = newRadius;
            draw.transform.localScale = newSize;
            time += Time.deltaTime;
            yield return null;
        }

        Vector3 tempSize = initialSize;
        initialSize = targetSize;
        targetSize = tempSize;

        float tempRadius = initialRadius;
        initialRadius = targetRadius;
        targetRadius = tempRadius;

        isChangingSize = false;
    }
}

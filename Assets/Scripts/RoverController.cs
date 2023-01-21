using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;

public class RoverController : MonoBehaviour
{
    private CinemachineImpulseSource impulse;
    private Rigidbody rb;
    private SphereCollider sc;
    public Transform followCam;
    public GameObject draw;
    [SerializeField]
    private Marimo[] marimos;
    [SerializeField]
    private Buoyancy floater;

    public LayerMask ground;
    private bool isGrounded = false;
    private float scanCooldown = 2.0f;
    private float scanTimer = 0.0f;
    private Vector3 moveDir;
    private float maxSlope = 45f;
    private float minSlope = 12.5f;
    private float slopeLimit = 12.5f;

    private float initialRadius = 1.5f;
    private float targetRadius = 3f;
    private Vector3 initialSize = new Vector3(3, 3, 3);
    private Vector3 targetSize = new Vector3(6, 6, 6);
    private float sizeShiftTime = 1.5f;
    private bool isChangingSize = false;

    public GameObject smokeRing;
    public GameObject scanner;
    private float landSpeed;
    private float currentSpeed;
    private float prevSpeed;

    private float debugEnergy = 0;
    public TMP_Text energyLevels;

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
        // Fixes weird glitch that makes it so that ball's collider goes through floor if inflated without moving at the start of the game
        if (followCam != null)
            rb.AddTorque(followCam.transform.forward * 25.0f);
    }

    void Update()
    {
        float currentSpeed = Mathf.Sqrt((rb.velocity.x * rb.velocity.x) + (rb.velocity.z * rb.velocity.z));

        isGrounded = Physics.CheckSphere(new Vector3(transform.position.x, transform.position.y - sc.radius, transform.position.z), 0.5f, ground);

        if (isGrounded)
        {
            if (followCam != null)
                moveDir = followCam.transform.forward * PlayerInputManager.instance.inputX + followCam.transform.right * -PlayerInputManager.instance.inputY;
        }

        if (currentSpeed > 8f)
            prevSpeed = currentSpeed;

        if (Mathf.Abs(rb.velocity.y) > 0.1f)
            landSpeed = Mathf.Abs(rb.velocity.y);

        if (Input.GetKeyDown(KeyCode.E) && !isChangingSize)
            StartCoroutine(ChangeSize());

        floater.active = PlayerInputManager.instance.jump;

        Slope();
        Scan();
    }

    void FixedUpdate()
    {
        if (rb == null)
            return;

        if (isChangingSize)
            return;

        float[] energyUsage = CalculateEnergyUsage(moveDir.normalized);

        // Apply thrust to the ball based on the energy usage of each marimo
        ApplyThrust(energyUsage);

        energyLevels.text = "Energy Levels: " + ((debugEnergy / 1200f) * 100).ToString("0") + "%";
    }

    void Shake()
    {
        impulse.m_DefaultVelocity *= Mathf.Clamp(landSpeed / 10f, 0f, 6f);
        impulse.GenerateImpulse();
        impulse.m_DefaultVelocity = new Vector3(-1f, -1f, -1f);
    }

    void Slope()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, sc.radius + 1f, ground))
        {
            if ((Vector3.Angle(Vector3.up, hit.normal)) > slopeLimit + 0.1f)
            {
                var left = Vector3.Cross(hit.normal, Vector3.up);
                var slope = Vector3.Cross(hit.normal, left);
                rb.AddForce(slope * 50f * rb.mass);
            }
        }
    }

    void Scan()
    {
        scanTimer += Time.deltaTime;
        if (PlayerInputManager.instance.scan && scanner != null && scanTimer >= scanCooldown)
        {
            Instantiate(scanner, transform.position, Quaternion.identity);
            AudioManager.Instance.PlayOneShotWithParameters("Sonar", transform, ("Underwater", (transform.position.y > WaveManager.instance.getHeight(transform.position.x, transform.position.z)) ? 0f : 1f));
            scanTimer = 0.0f;
        }
    }

    IEnumerator ChangeSize()
    {
        isChangingSize = true;
        float time = 0f;

        if (initialRadius > targetRadius)
        {
            AudioManager.Instance.PlayOneShotWithParameters("Deflate", transform, ("Underwater", (transform.position.y > WaveManager.instance.getHeight(transform.position.x, transform.position.z)) ? 0f : 1f));
            slopeLimit = minSlope;
        }
        else
        {
            AudioManager.Instance.PlayOneShotWithParameters("Inflate", transform, ("Underwater", (transform.position.y > WaveManager.instance.getHeight(transform.position.x, transform.position.z)) ? 0f : 1f));
            slopeLimit = maxSlope;
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

    // Calculates the energy usage for each marimo based on the direction in which the ball should move
    float[] CalculateEnergyUsage(Vector3 moveDirection)
    {
        float[] energyUsage = new float[marimos.Length];

        for (int i = 0; i < marimos.Length; i++)
        {
            // Calculate the angle between the direction in which the ball should move and the direction of the marimo
            float angle = Vector3.Angle(moveDirection, marimos[i].transform.forward);

            // Calculate the energy usage based on the angle between the direction in which the ball should move and the direction of the marimo
            energyUsage[i] = Mathf.Lerp(0f, 0.1f, angle / 180f);
        }

        return energyUsage;
    }

    // Applies thrust to the ball based on the energy usage of each marimo
    void ApplyThrust(float[] energyUsage)
    {
        debugEnergy = 0;
        for (int i = 0; i < marimos.Length; i++)
        {
            debugEnergy += marimos[i].energy;
            // Check if there is enough energy in the marimo
            if (marimos[i].energy > 0f)
            {
                // Calculate the thrust force based on the energy usage of the marimo and the speed at which the ball should move
                Vector3 thrustForce = marimos[i].transform.forward * energyUsage[i] * 100f;

                // Apply the thrust force to the ball
                rb.AddTorque(thrustForce);

                // Reduce the energy in the marimo by the energy usage
                marimos[i].energy -= energyUsage[i];
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (landSpeed > 9f || prevSpeed > 10f)
        {
            Shake();
            Instantiate(smokeRing, collision.contacts[0].point, Quaternion.FromToRotation(smokeRing.transform.up ,collision.contacts[0].normal));
        }

        if (!isChangingSize)
            AudioManager.Instance.PlayOneShotWithParameters("BallLand", transform, ("Underwater", (transform.position.y > WaveManager.instance.getHeight(transform.position.x, transform.position.z)) ? 0f : 1f));
    }
}

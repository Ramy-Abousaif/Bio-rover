using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class AIController : MonoBehaviour
{
    private Rigidbody rb;
    private SphereCollider sc;
    public GameObject draw;
    [SerializeField]
    private Marimo[] marimos;
    [SerializeField]
    private Buoyancy floater;
    private bool ableToFloat;

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

    private float sumEnergy = 0;

    // AI STUFF
    private Seeker seeker;
    public Transform targetPos;
    public Path path;
    public float speed = 2;
    public float nextWaypointDistance = 3;
    private int currentWaypoint = 0;
    public bool reachedEndOfPath;
    public float repathRate = 0.5f;
    private float lastRepath = float.NegativeInfinity;
    private GraphNode randomNode;
    GridGraph grid;
    private float repathTimer = 0.0f;
    public float forceRepath = 10f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        sc = GetComponent<SphereCollider>();
        scanTimer = scanCooldown;
        sc.radius = initialRadius;
        draw.transform.localScale = initialSize;
        seeker = GetComponent<Seeker>();
        grid = AstarPath.active.data.gridGraph;
    }

    void Update()
    {
        float currentSpeed = Mathf.Sqrt((rb.velocity.x * rb.velocity.x) + (rb.velocity.z * rb.velocity.z));

        repathTimer += Time.deltaTime;

        isGrounded = Physics.CheckSphere(new Vector3(transform.position.x, transform.position.y - sc.radius, transform.position.z), 0.5f, ground);

        if (currentSpeed > 8f)
            prevSpeed = currentSpeed;

        if (Mathf.Abs(rb.velocity.y) > 0.1f)
            landSpeed = Mathf.Abs(rb.velocity.y);

        if (Input.GetKeyDown(KeyCode.E) && !isChangingSize && GameManager.instance.expandUpgrade >= 1)
            StartCoroutine(ChangeSize());

        floater.active = PlayerInputManager.instance.jump && ableToFloat;

        //Slope();
        Scan();
    }

    void FixedUpdate()
    {
        if (rb == null)
            return;

        if (isChangingSize)
            return;

        AIMovement();
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
            AudioManager.instance.PlayOneShotWithParameters("Sonar", transform, ("Underwater", (transform.position.y > WaveManager.instance.getHeight(transform.position.x, transform.position.z)) ? 0f : 1f));
            scanTimer = 0.0f;
        }
    }

    IEnumerator ChangeSize()
    {
        isChangingSize = true;
        float time = 0f;

        if (initialRadius > targetRadius)
        {
            AudioManager.instance.PlayOneShotWithParameters("Deflate", transform, ("Underwater", (transform.position.y > WaveManager.instance.getHeight(transform.position.x, transform.position.z)) ? 0f : 1f));
            slopeLimit = minSlope;
        }
        else
        {
            AudioManager.instance.PlayOneShotWithParameters("Inflate", transform, ("Underwater", (transform.position.y > WaveManager.instance.getHeight(transform.position.x, transform.position.z)) ? 0f : 1f));
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
    void ApplyForce(float[] energyUsage, float distanceToWaypoint)
    {
        sumEnergy = 0;
        for (int i = 0; i < marimos.Length; i++)
        {
            sumEnergy += marimos[i].energy;
            // Check if there is enough energy in the marimo
            if (marimos[i].energy > 0f)
            {
                // Slow down smoothly upon approaching the end of the path
                // This value will smoothly go from 1 to 0 as the agent approaches the last waypoint in the path.
                var speedFactor = reachedEndOfPath ? Mathf.Sqrt(distanceToWaypoint / nextWaypointDistance) : 1f;

                // Calculate the thrust force based on the energy usage of the marimo and the speed at which the ball should move
                Vector3 force = marimos[i].transform.forward * energyUsage[i] * 5f * speed * speedFactor;

                // Apply the thrust force to the ball
                rb.AddForce(-force);

                // Reduce the energy in the marimo by the energy usage
                marimos[i].energy -= energyUsage[i];
            }
        }
    }

    // Applies thrust to the ball based on the energy usage of each marimo
    void ApplyFloat(float[] energyUsage)
    {
        ableToFloat = false;
        if (transform.position.y < WaveManager.instance.getHeight(transform.position.x, transform.position.z))
        {
            for (int i = 0; i < marimos.Length; i++)
            {
                // Check if there is enough energy in the marimo
                if (marimos[i].energy > 0f)
                {
                    if (Vector3.Angle(Vector3.down, marimos[i].transform.forward) < 50)
                    {
                        ableToFloat = true;

                        energyUsage[i] = Mathf.Lerp(0f, 1f, Vector3.Angle(Vector3.down, marimos[i].transform.forward) / 180f);

                        // Reduce the energy in the marimo by the energy usage
                        if (floater.active)
                            marimos[i].energy -= energyUsage[i];
                    }
                }
            }
        }
    }

    private void AIMovement()
    {
        if (Time.time > lastRepath + repathRate && seeker.IsDone())
        {
            lastRepath = Time.time;
            // Start a new path to the targetPosition, call the the OnPathComplete function
            // when the path has been calculated (which may take a few frames depending on the complexity)
            seeker.StartPath(transform.position, targetPos.position, OnPathComplete);
        }

        if (path == null)
        {
            // We have no path to follow yet, so don't do anything
            return;
        }

        if (repathTimer >= forceRepath)
        {
            repathTimer = 0.0f;
            FindNewTarget();
        }

        // Direction to the next waypoint
        // Normalize it so that it has a length of 1 world unit
        moveDir = (path.vectorPath[currentWaypoint] - transform.position).normalized;

        // Check in a loop if we are close enough to the current waypoint to switch to the next one.
        // We do this in a loop because many waypoints might be close to each other and we may reach
        // several of them in the same frame.
        reachedEndOfPath = false;
        // The distance to the next waypoint in the path
        float distanceToWaypoint;
        while (true)
        {
            distanceToWaypoint = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
            if (distanceToWaypoint < nextWaypointDistance)
            {
                // Check if there is another waypoint or if we have reached the end of the path
                if (currentWaypoint + 1 < path.vectorPath.Count)
                {
                    currentWaypoint++;
                }
                else
                {
                    // Set a status variable to indicate that the agent has reached the end of the path.
                    // Use this to trigger some special code if your game requires that.
                    reachedEndOfPath = true;
                    FindNewTarget();
                    break;
                }
            }
            else
            {
                break;
            }
        }

        float[] energyUsage = CalculateEnergyUsage(moveDir.normalized);

        ApplyForce(energyUsage, distanceToWaypoint);
        ApplyFloat(energyUsage);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (landSpeed > 9f || prevSpeed > 10f)
        {
            Instantiate(smokeRing, collision.contacts[0].point, Quaternion.FromToRotation(smokeRing.transform.up ,collision.contacts[0].normal));
        }

        if (!isChangingSize)
            AudioManager.instance.PlayOneShotWithParameters("BallLand", transform, ("Underwater", (transform.position.y > WaveManager.instance.getHeight(transform.position.x, transform.position.z)) ? 0f : 1f));
    }

    private void FindNewTarget()
    {
        randomNode = grid.nodes[Random.Range(0, grid.nodes.Length)];
        targetPos.position = randomNode.RandomPointOnSurface();
    }

    public void OnPathComplete(Path p)
    {
        Debug.Log("A path was calculated. Did it fail with an error? " + p.error);

        // Path pooling. To avoid unnecessary allocations paths are reference counted.
        // Calling Claim will increase the reference count by 1 and Release will reduce
        // it by one, when it reaches zero the path will be pooled and then it may be used
        // by other scripts. The ABPath.Construct and Seeker.StartPath methods will
        // take a path from the pool if possible. See also the documentation page about path pooling.
        p.Claim(this);
        if (!p.error)
        {
            if (path != null) path.Release(this);
            path = p;
            // Reset the waypoint counter so that we start to move towards the first point in the path
            currentWaypoint = 0;
        }
        else
        {
            p.Release(this);
        }
    }

    private void OnEnable()
    {
        RaycastHit hit;
        if (Physics.Raycast(PlayerManager.instance.boat.transform.position, Vector3.down, out hit, Mathf.Infinity, ground))
        {
            targetPos.position = hit.point;
        }
    }
}

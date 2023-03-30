using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class AIController : MonoBehaviour
{
    private Rigidbody rb;
    private SphereCollider sc;
    public GameObject draw;
    public GameObject outline;
    public GameObject minimap;
    private Material minimapMat;
    [SerializeField]
    private Marimo[] marimos;
    [SerializeField]
    private Buoyancy floater;
    private bool floatToSurface;

    public LayerMask ground;
    private Vector3 moveDir;

    public GameObject smokeRing;

    private float explosionForce = 100f;
    private float explosionRadius = 30f;
    private float landSpeed;
    private float currentSpeed;
    private float prevSpeed;

    private float sumEnergy = 0;

    // AI STUFF
    private Seeker seeker;
    public Transform targetPos;
    public AIScan aiScan;
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
    [HideInInspector]
    public bool overrideTarget = false;
    public GameObject breakableTarget;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        sc = GetComponent<SphereCollider>();
        seeker = GetComponent<Seeker>();
        grid = AstarPath.active.data.gridGraph;
        floater.active = false;
        minimapMat = minimap.GetComponent<Renderer>().material;
    }

    private void Update()
    {
        minimapMat.SetFloat("_RoverPos", transform.position.y);

        if (PlayerInputManager.instance.upArrow)
        {
            floatToSurface = true;
            floater.active = true;
            minimapMat.SetInt("_isSinking", 0);
            if (transform.position.y < 0f)
                rb.AddForce(Vector3.up * 300f);
        }

        if (PlayerInputManager.instance.downArrow)
        {
            floatToSurface = false;
            floater.active = false;
            minimapMat.SetInt("_isSinking", 1);
        }

        if (breakableTarget == null)
            return;

        float squaredDistance = (breakableTarget.transform.position - transform.position).sqrMagnitude;

        if (squaredDistance <= explosionRadius * 5f)
            Explode();
    }

    void FixedUpdate()
    {
        if (rb == null)
            return;

        float currentSpeed = Mathf.Sqrt((rb.velocity.x * rb.velocity.x) + (rb.velocity.z * rb.velocity.z));

        if (currentSpeed > 8f)
            prevSpeed = currentSpeed;

        if (Mathf.Abs(rb.velocity.y) > 0.1f)
            landSpeed = Mathf.Abs(rb.velocity.y);

        AIMovement();
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

        if (Time.time > repathTimer + forceRepath)
        {
            repathTimer = Time.time;
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

        // float to mine if close enough to blow up
        if(breakableTarget != null)
        {
            if (breakableTarget.transform.position.y > transform.position.y)
            {
                if((new Vector3(breakableTarget.transform.position.x, 0, breakableTarget.transform.position.z)
                    - new Vector3(transform.position.x, 0, transform.position.z)).sqrMagnitude < 120f)
                {
                    floater.active = true;
                }
                else
                {
                    floater.active = false;
                }
            }
            else
            {
                floater.active = false;
            }
        }
        else
        {
            if (!floatToSurface)
                floater.active = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (landSpeed > 9f || prevSpeed > 10f)
            PoolManager.instance.SpawnSmokeRing(collision.contacts[0].point, Quaternion.FromToRotation(smokeRing.transform.up, collision.contacts[0].normal));
    }

    private void FindNewTarget()
    {
        if(!overrideTarget)
        {
            randomNode = grid.nodes[Random.Range(0, grid.nodes.Length)];
            targetPos.position = randomNode.RandomPointOnSurface();
        }
    }

    public void OverrideTarget(Vector3 newPos)
    {
        overrideTarget = true;
        targetPos.position = newPos;
    }

    public void OnPathComplete(Path p)
    {
        //Debug.Log("A path was calculated. Did it fail with an error? " + p.error);

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

    private void Explode()
    {
        if (!(GameManager.instance.explosionUpgrade >= 1))
            return;

        PoolManager.instance.SpawnExplosion(transform.position, Quaternion.identity);

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (var col in colliders)
        {
            Rigidbody rigidBody = col.GetComponent<Rigidbody>();

            if(rigidBody != null)
                rigidBody.AddExplosionForce(explosionForce, transform.position, explosionRadius / 2);
        }

        if (PlayerManager.instance.bc.activeRovers.Contains(transform.parent.gameObject))
            PlayerManager.instance.bc.activeRovers.Remove(transform.parent.gameObject);

        if (PlayerManager.instance.rc != null)
        {
            if (PlayerManager.instance.rc.aiRovers.Contains(this))
                PlayerManager.instance.rc.aiRovers.Remove(this);

            float squaredDistance = (PlayerManager.instance.rc.transform.position - transform.position).sqrMagnitude;

            if (squaredDistance <= explosionRadius * 20f)
                PlayerManager.instance.rc.Shake(20f);

            if(PlayerManager.instance.rc.aiRovers.Count <= 0)
                PlayerManager.instance.arrow.SetActive(false);
        }

        UIManager.instance.activeRoversText.text = PlayerManager.instance.bc.activeRovers.Count + " Active Rovers";
        UIManager.instance.storedRoversText.text = PlayerManager.instance.bc.storedRovers.Count + " Stored Rovers";
        AudioManager.instance.PlayOneShotWithParameters("Explosion", transform);

        if (breakableTarget != null)
        {
            PlayerManager.instance.arrow.SetActive(false);
            PoolManager.instance.SpawnPoof(breakableTarget.transform.position, Quaternion.identity);
            Destroy(breakableTarget);
        }

        Destroy(transform.parent.gameObject);
    }

    private void OnEnable()
    {
        minimapMat = minimap.GetComponent<Renderer>().material;
        floater.active = false;
        minimapMat.SetInt("_isSinking", 1);
        transform.gameObject.layer = LayerMask.NameToLayer("Bot");
        aiScan.transform.gameObject.SetActive(true);
        RaycastHit hit;
        if (Physics.Raycast(PlayerManager.instance.boat.transform.position, Vector3.down, out hit, Mathf.Infinity, ground))
        {
            targetPos.position = hit.point;
        }
    }

    private void OnDisable()
    {
        outline.SetActive(false);
        aiScan.transform.gameObject.SetActive(false);
    }
}

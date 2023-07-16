using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMovement : MonoBehaviour
{
    public GameObject boundsCollider;
    private BoxCollider Bounds;
    public float maxFlightHeight = 10f;
    public float flockingDistance = 5f;
    public float flockingDurationMin = 1f;
    public float flockingDurationMax = 5f;
    public float regularFlockingDuration = 1f;
    public float movementSpeed = 5f;
    public float followProbability = 0.2f; // Probability factor for following the player
    private GameObject player;
    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private bool isFlocking = false;
    private bool isFluidFlocking = false;
    private Vector3 individualOffset;

    private void Start()
    {
        Bounds = boundsCollider.GetComponent<BoxCollider>();
        initialPosition = transform.position;
        individualOffset = GetRandomOffset();
        targetPosition = GetRandomPosition();
        StartCoroutine(FlockAroundPlayer());
    }

    private void Update()
    {
        if (!isFlocking)
        {
            float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

            if (distanceToTarget > 0.1f)
            {
                float currentSpeed = movementSpeed * Mathf.Clamp01(distanceToTarget / flockingDistance);

                Vector3 movementDirection = (targetPosition - transform.position).normalized;

                Vector3 lookDirection = -movementDirection;

                Quaternion targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);

                transform.position += movementDirection * currentSpeed * Time.deltaTime;
            }
            else
            {
                targetPosition = GetRandomPosition();
            }
        }
    }

    private Vector3 GetRandomPosition()
    {
        Vector3 randomPosition = Bounds.bounds.center +
            new Vector3(Random.Range(-Bounds.bounds.extents.x, Bounds.bounds.extents.x),
                        Random.Range(-Bounds.bounds.extents.y, Bounds.bounds.extents.y),
                        Random.Range(-Bounds.bounds.extents.z, Bounds.bounds.extents.z));

        randomPosition.y = Mathf.Clamp(randomPosition.y, Bounds.bounds.min.y, Bounds.bounds.min.y + maxFlightHeight);

        randomPosition += individualOffset;

        return randomPosition;
    }

    private Vector3 GetRandomOffset()
    {
        return new Vector3(Random.Range(-flockingDistance, flockingDistance),
                           Random.Range(-flockingDistance, flockingDistance),
                           Random.Range(-flockingDistance, flockingDistance));
    }

    private IEnumerator FlockAroundPlayer()
    {
        while (true)
        {
            player = GameObject.FindGameObjectWithTag("Rover");

            if (player != null)
            {
                isFlocking = true;

                Vector3 flockCenter = player.transform.position;

                float fluidFlockingDuration = Random.Range(flockingDurationMin, flockingDurationMax);
                float fluidFlockingTimer = 0f;

                while (fluidFlockingTimer < fluidFlockingDuration)
                {
                    isFluidFlocking = true;

                    if (Random.value < followProbability)
                    {
                        // Follow the player
                        Vector3 followPosition = flockCenter +
                            new Vector3(Random.Range(-flockingDistance, flockingDistance),
                                        Random.Range(-flockingDistance, flockingDistance),
                                        Random.Range(-flockingDistance, flockingDistance));

                        followPosition.y = Mathf.Clamp(followPosition.y, Bounds.bounds.min.y, Bounds.bounds.min.y + maxFlightHeight);

                        transform.position = Vector3.MoveTowards(transform.position, followPosition, movementSpeed * Time.deltaTime);
                    }
                    else
                    {
                        // Continue regular flocking behavior
                        Vector3 randomPosition = flockCenter +
                            new Vector3(Random.Range(-flockingDistance, flockingDistance),
                                        Random.Range(-flockingDistance, flockingDistance),
                                        Random.Range(-flockingDistance, flockingDistance));

                        randomPosition.y = Mathf.Clamp(randomPosition.y, Bounds.bounds.min.y, Bounds.bounds.min.y + maxFlightHeight);

                        transform.position = Vector3.MoveTowards(transform.position, randomPosition, movementSpeed * Time.deltaTime);
                    }

                    fluidFlockingTimer += Time.deltaTime;
                    yield return null;
                }

                isFluidFlocking = false;

                float regularFlockingDuration = Random.Range(flockingDurationMin, flockingDurationMax);
                float regularFlockingTimer = 0f;

                while (regularFlockingTimer < regularFlockingDuration)
                {
                    isFlocking = true;

                    transform.position = Vector3.MoveTowards(transform.position, flockCenter, movementSpeed * Time.deltaTime);

                    regularFlockingTimer += Time.deltaTime;
                    yield return null;
                }

                isFlocking = false;

                individualOffset = GetRandomOffset();
            }
        }
    }
}

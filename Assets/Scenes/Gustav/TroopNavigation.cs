using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TroopNavigation : MonoBehaviour
{
    private NavMeshAgent agent;
    public List<GameObject> waypoints = new List<GameObject>();
    private BoxCollider collider;

    public bool inCombat = false;
    public bool holdPosition = false;
    public bool moveForwards = false;

    private CoverScript coverScript;

    public float waitTimeAtWaypoint = 1f; // Time to pause at each waypoint
    private float waitTimer = 0f;
    private bool isWaiting = false;

    private GameObject currentWaypoint;

    // Start is called before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        collider = GetComponent<BoxCollider>();
        coverScript = FindAnyObjectByType<CoverScript>();
        agent = GetComponent<NavMeshAgent>();

        GameObject[] findWaypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        foreach (GameObject waypoint in findWaypoints)
        {
            waypoints.Add(waypoint);
        }
        UpdateWaypoints();
    }

    GameObject closestWaypointInFront = null;
    float closestDistance = Mathf.Infinity;

    private void UpdateWaypoints()
    {
        waypoints.Sort((a, b) =>
        {
            float distanceA = Vector3.Distance(transform.position, a.transform.position);
            float distanceB = Vector3.Distance(transform.position, b.transform.position);
            return distanceA.CompareTo(distanceB);
        });

        agent.ResetPath();
        agent.isStopped = true;
    }

    void Update()
    {
        if (isWaiting)
        {
            // Increment the wait timer
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeAtWaypoint)
            {
                isWaiting = false;
                waitTimer = 0f;
                MoveToNextWaypoint();
            }
        }
        else
        {
            if (moveForwards)
            {
                MoveForwards();
            }
            else if (holdPosition)
            {
                HoldPosition();
            }
        }
    }

    public void MoveForwards()
    {
        UpdateWaypoints();
        closestWaypointInFront = null;
        closestDistance = Mathf.Infinity;

        // Get the troop's forward direction (relative to its current facing direction)
        Vector3 forwardDirection = transform.forward;

        foreach (GameObject waypoint in waypoints)
        {
            coverScript = waypoint.GetComponent<CoverScript>();

            if (Vector3.Distance(transform.position, waypoint.transform.position) < 1f ||
                (coverScript != null && coverScript.occupiedBy != "none" && coverScript.occupiedBy != name))
            {
                continue;
            }

            Vector3 directionToWaypoint = (waypoint.transform.position - transform.position).normalized;
            float dotProduct = Vector3.Dot(forwardDirection, directionToWaypoint);

            if (dotProduct > 0) // Waypoint is in front
            {
                float distance = Vector3.Distance(transform.position, waypoint.transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestWaypointInFront = waypoint;
                }
            }
        }

        if (closestWaypointInFront != null)
        {
            currentWaypoint = closestWaypointInFront; // Set current waypoint
            agent.isStopped = false;
            agent.SetDestination(currentWaypoint.transform.position);
        }
        else
        {
            agent.isStopped = true;
            Debug.Log($"{name} has no valid waypoint to move towards.");
        }
    }

    private void HoldPosition()
    {
        UpdateWaypoints();
        Vector3 forwardDirection = Vector3.forward;

        closestWaypointInFront = null;
        closestDistance = Mathf.Infinity;

        foreach (GameObject waypoint in waypoints)
        {
            coverScript = waypoint.GetComponent<CoverScript>();
            float distance = Vector3.Distance(transform.position, waypoint.transform.position);

            if (coverScript != null)
                if (coverScript.occupiedBy != "none" && coverScript.occupiedBy != name)
                    continue;

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestWaypointInFront = waypoint;
            }
        }

        float distanceToWaypoint = Vector3.Distance(transform.position, closestWaypointInFront.transform.position);

        if (distanceToWaypoint <= agent.stoppingDistance)
        {
            Quaternion targetRotation = Quaternion.LookRotation(forwardDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 200f);

            agent.isStopped = true;
            agent.ResetPath();
            return;
        }

        if (closestWaypointInFront != null)
        {
            agent.isStopped = false;
            agent.SetDestination(closestWaypointInFront.transform.position);

            Vector3 directionToTarget = (closestWaypointInFront.transform.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 200f);
        }
        else
        {
            agent.isStopped = true;
        }
    }

    private void MoveToNextWaypoint()
    {
        MoveForwards(); // Reuse MoveForwards logic to find the next valid waypoint
        Debug.Log($"{name} is moving to the next waypoint.");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"{name} triggered by {other.name}");
    }
}
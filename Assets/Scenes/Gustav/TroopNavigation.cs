using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions.Must;

public class TroopNavigation : MonoBehaviour
{
    private NavMeshAgent agent;
    public List<GameObject> waypoints = new List<GameObject>();
    private BoxCollider collider;

    public bool inCombat = false;
    public bool holdPosition = false;
    public bool moveForwards = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
        if (holdPosition)
        {
            HoldPosition();
        }
        else if (inCombat)
        {

        }
        else if (moveForwards)
        {
            MoveForwards();
        }
    }

    private CoverScript coverScript;
    public void MoveForwards()
    {
        UpdateWaypoints();
        closestWaypointInFront = null;
        closestDistance = Mathf.Infinity;

        // Get the troop's forward direction
        Vector3 forwardDirection = Vector3.forward;

        foreach (GameObject waypoint in waypoints)
        {
            coverScript = waypoint.GetComponent<CoverScript>();

            // Skip waypoints that are already reached or occupied
            if (Vector3.Distance(transform.position, waypoint.transform.position) < 1f || (coverScript != null && (coverScript.occupiedBy != "none" && coverScript.occupiedBy != name)))
            {
                continue;
            }

            Vector3 directionToWaypoint = (waypoint.transform.position - transform.position).normalized;

            // Ensure the waypoint is strictly in front of the troop
            float dotProduct = Vector3.Dot(forwardDirection, directionToWaypoint);
            if (dotProduct > 0) // In front of the troop
            {
                float distance = Vector3.Distance(transform.position, waypoint.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestWaypointInFront = waypoint;
                }
            }
        }

        // Move to the closest waypoint in front
        if (closestWaypointInFront != null)
        {
            agent.isStopped = false;
            agent.SetDestination(closestWaypointInFront.transform.position);
        }
        else
        {
            // Stop the agent if no valid waypoint is found
            agent.isStopped = true;
        }
    }

    private void HoldPosition()
    {
        UpdateWaypoints();
        Vector3 forwardDirection = Vector3.forward;

        // Reset the closest waypoint and distance
        closestWaypointInFront = null;
        closestDistance = Mathf.Infinity;

        // Find the closest unoccupied waypoint
        foreach (GameObject waypoint in waypoints)
        {
            coverScript = waypoint.GetComponent<CoverScript>();
            float distance = Vector3.Distance(transform.position, waypoint.transform.position);

            // Skip waypoints that are occupied
            if (coverScript != null)
                if (coverScript.occupiedBy != "none" && coverScript.occupiedBy != name)
                continue;

            // Check if this waypoint is the closest
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestWaypointInFront = waypoint;
            }
        }

        float distanceToWaypoint = Vector3.Distance(transform.position, closestWaypointInFront.transform.position);

        // Stop the agent if it's close enough
        if (distanceToWaypoint <= agent.stoppingDistance)
        {
            Quaternion targetRotation = Quaternion.LookRotation(forwardDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 200f);

            agent.isStopped = true;
            agent.ResetPath();
            return;
        }

        // If a closest waypoint is found, move to it
        if (closestWaypointInFront != null)
        {
            agent.isStopped = false;
            agent.SetDestination(closestWaypointInFront.transform.position);

            // Smoothly rotate toward the waypoint
            Vector3 directionToTarget = (closestWaypointInFront.transform.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 200f);
        }
        else
        {
            // Stop the agent if no valid waypoint is found
            agent.isStopped = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(name + "TRIGGGGGERRR");
    }
}

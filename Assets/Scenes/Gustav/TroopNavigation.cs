using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class TroopNavigation : MonoBehaviour
{
    private NavMeshAgent agent;
    public List<GameObject> waypoints = new List<GameObject>();

    public bool inCombat = false;
    public bool holdPosition = false;
    public bool forwards = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
        else if (forwards)
        {
            MoveForwards();
        }
    }

    private CoverScript coverScript;
    public void MoveForwards()
    {
        UpdateWaypoints();
        // Get the forward direction of the troop
        Vector3 forwardDirection = transform.forward;
        closestDistance = math.INFINITY;

        // Loop through waypoints to find the closest one in front
        foreach (GameObject waypoint in waypoints)
        {
            coverScript = waypoint.GetComponent<CoverScript>();

            // Skip waypoints that are already reached (adjust threshold as needed)
            if (Vector3.Distance(transform.position, waypoint.transform.position) < 1f || coverScript.occupied == true)
            {
                continue;
            }

            Vector3 directionToWaypoint = (waypoint.transform.position - transform.position).normalized;

            // Check if the waypoint is in front within a cone using the dot product
            float dotProduct = Vector3.Dot(forwardDirection, directionToWaypoint);
            if (dotProduct > 0) // Waypoint within 45-degree cone
            {
                float distance = Vector3.Distance(transform.position, waypoint.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestWaypointInFront = waypoint;
                }
            }
        }

        // Command the NavMeshAgent to move towards the closest waypoint in front, if found
        if (closestWaypointInFront != null)
        {
            agent.isStopped = false;
            agent.SetDestination(closestWaypointInFront.transform.position);

            // Ensure the agent faces the right direction
            Vector3 directionToTarget = (closestWaypointInFront.transform.position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(directionToTarget);
        }
    }

    private void HoldPosition()
    {
        UpdateWaypoints();

        // Reset the closest waypoint and distance
        closestWaypointInFront = null;
        closestDistance = Mathf.Infinity;

        // Find the closest unoccupied waypoint
        foreach (GameObject waypoint in waypoints)
        {
            coverScript = waypoint.GetComponent<CoverScript>();
            float distance = Vector3.Distance(transform.position, waypoint.transform.position);

            // Skip waypoints that are occupied
            if (coverScript != null && coverScript.occupied)
                continue;

            // Check if this waypoint is the closest
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestWaypointInFront = waypoint;
            }
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
}

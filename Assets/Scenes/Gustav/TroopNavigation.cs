using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class TroopNavigation : MonoBehaviour
{
    private NavMeshAgent agent;
    private List<GameObject> waypoints = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        GameObject[] findWaypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        foreach (GameObject waypoint in findWaypoints)
        {
            waypoints.Add(waypoint);
        }
        UpdateWaypoints();
    }

    private void UpdateWaypoints()
    {
        waypoints.Sort((a, b) =>
        {
            float distanceA = Vector3.Distance(transform.position, a.transform.position);
            float distanceB = Vector3.Distance(transform.position, b.transform.position);
            return distanceA.CompareTo(distanceB);
        });

        foreach (GameObject waypoint in waypoints)
        {
            float distance = Vector3.Distance(transform.position, waypoint.transform.position);
        }
    }

    public void MoveForwards()
    {
        UpdateWaypoints();

        // Get the forward direction of the player
        Vector3 forwardDirection = transform.forward;

        GameObject closestWaypointInFront = null;
        float closestDistance = Mathf.Infinity;

        // Loop through waypoints and check if they are in front of the player
        foreach (GameObject waypoint in waypoints)
        {
            // Skip the waypoint if it has already been reached
            if (Vector3.Distance(transform.position, waypoint.transform.position) < 1f) // Adjust threshold as needed
            {
                continue; // Skip this waypoint
            }

            Vector3 directionToWaypoint = (waypoint.transform.position - transform.position).normalized;

            // Check if the waypoint is in front of the player using the dot product
            float dotProduct = Vector3.Dot(forwardDirection, directionToWaypoint);

            // Only consider waypoints in front of the player (dotProduct > 0 means it's in front)
            if (dotProduct > 0)
            {
                float distance = Vector3.Distance(transform.position, waypoint.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestWaypointInFront = waypoint;
                }
            }

        }
    }


        // Update is called once per frame
    void Update()
    {
        
    }
}

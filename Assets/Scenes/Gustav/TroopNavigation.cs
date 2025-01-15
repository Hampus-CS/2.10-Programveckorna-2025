using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions.Must;

public enum TroopState
{
    MovingForward,
    HoldingPosition,
    SeekingCover
}

public class TroopNavigation : MonoBehaviour
{
    private NavMeshAgent agent;
    private TroopPersonalityScript personality;
    public List<GameObject> waypoints = new List<GameObject>();

    public bool holdPosition = false;
    public bool isAtCover = false;
    public bool canMove = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        coverScript = FindAnyObjectByType<CoverScript>();
        personality = GetComponent<TroopPersonalityScript>();
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = true;
        agent.ResetPath();

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


    private CoverScript coverScript;
    public void MoveForwards()
    {
        if (!holdPosition && canMove)
        {
            StartCoroutine(TimerBetweenNavigation());
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
        }

    }

    public void HoldPosition()
    {
            StartCoroutine(TimerBetweenNavigation());
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

                isAtCover = true;
                agent.isStopped = true;
                agent.ResetPath();
                return;
            }

            // If a closest waypoint is found, move to it
            if (closestWaypointInFront != null)
            {
                isAtCover = false;
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

    public void RunToNearestCover()
    {
        if (!holdPosition)
        {
            StartCoroutine(TimerBetweenNavigation());
            UpdateWaypoints();
            Vector3 forwardDirection = Vector3.forward;

            Debug.Log("running to nearst");

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

                isAtCover = true;
                agent.isStopped = true;
                agent.ResetPath();
                return;
            }

            if (closestWaypointInFront != null)
            {
                isAtCover = false;
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
}

    public void RunToBackToCover()
    {
        if (!holdPosition)
        {
            StartCoroutine(TimerBetweenNavigation());
            UpdateWaypoints();
            closestWaypointInFront = null;
            closestDistance = Mathf.Infinity;

            Debug.Log("running back");

            Vector3 forwardDirection = Vector3.forward;

            foreach (GameObject waypoint in waypoints)
            {
                coverScript = waypoint.GetComponent<CoverScript>();
                Vector3 directionToWaypoint = (waypoint.transform.position - transform.position).normalized;

                if (coverScript != null)
                    if (coverScript.occupiedBy != "none" && coverScript.occupiedBy != name)
                        continue;

                float dotProduct = Vector3.Dot(forwardDirection, directionToWaypoint);
                if (dotProduct < 0)
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
    }

    public void RunToEnemy(Transform enemy)
    {
        if (!holdPosition && canMove)
        {
            StartCoroutine(TimerBetweenNavigation());
            UpdateWaypoints();
            closestWaypointInFront = null;
            closestDistance = Mathf.Infinity;

            Debug.Log("running to enemy");

            Vector3 forwardDirection = Vector3.forward;

            foreach (GameObject waypoint in waypoints)
            {
                coverScript = waypoint.GetComponent<CoverScript>();
                Vector3 directionToWaypoint = (waypoint.transform.position - transform.position).normalized;

                if (coverScript != null)
                    if (coverScript.occupiedBy != "none" && coverScript.occupiedBy != name)
                        continue;

                float dotProduct = Vector3.Dot(forwardDirection, directionToWaypoint);

                if (dotProduct < 0)
                {
                    float distance = Vector3.Distance(transform.position, waypoint.transform.position);
                    if (distance <= personality.range)
                    {
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestWaypointInFront = waypoint;
                        }
                    }
                }
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
    }

    private IEnumerator TimerBetweenNavigation()
    {
        canMove = false;
        yield return new WaitForSeconds(5f);
        canMove = true;
    }

    public void RotateToEnemy(GameObject target)
    {
        Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 400);
    }

}

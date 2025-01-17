using System.Collections.Generic;
using System.Collections;
using UnityEngine.AI;
using UnityEngine;

public class BaseSoldier : MonoBehaviour
{
    private NavMeshAgent agent;
    private TroopPersonalityScript personality;
    private RangeColliderScript rangeColliderScript;
    public List<GameObject> waypoints = new List<GameObject>();

    public ParticleSystem bloodPartiles;

    private bool isTimerRunning = false;

    public bool inCombat = false;
    public bool holdPosition = false;
    public bool isAtCover = false;
    public bool canMove = true;

    private bool isFriendly;
    public bool isHostile; // New property to distinguish enemies from friendlies

    private int currentWaypointIndex = 0; // Tracks the current waypoint

    void Start()
    {
        Debug.Log($"isHostile before Start logic: {isHostile}");
        coverScript = FindAnyObjectByType<CoverScript>();
        personality = GetComponent<TroopPersonalityScript>();
        rangeColliderScript = GetComponentInChildren<RangeColliderScript>();
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = true;
        agent.ResetPath();

        GameObject[] findWaypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        foreach (GameObject waypoint in findWaypoints)
        {
            waypoints.Add(waypoint);
        }
        UpdateWaypoints();

        isFriendly = personality.isFriendly;

        // Initialize currentWaypointIndex
        currentWaypointIndex = isHostile ? waypoints.Count - 1 : 0;

        Debug.Log($"isHostile after Start logic: {isHostile}. Starting at waypoint index: {currentWaypointIndex}");
    }


    private void UpdateWaypoints()
    {
        // Sort waypoints in a fixed order (e.g., by name)
        waypoints.Sort((a, b) => a.name.CompareTo(b.name));

        // Reverse the order for hostile units to traverse the waypoints backward
        if (isHostile)
        {
            waypoints.Reverse();
        }

        Debug.Log($"Waypoints updated for {(isHostile ? "Enemy" : "Player")}. Waypoint count: {waypoints.Count}");
    }




    private CoverScript coverScript;

    // Modified MoveForwards for direction adjustment
    public void MoveForwards()
    {
        if (!holdPosition && canMove)
        {
            StartCoroutine(TimerBetweenNavigation());

            // Check if we have valid waypoints
            if (waypoints.Count == 0 || currentWaypointIndex < 0 || currentWaypointIndex >= waypoints.Count)
            {
                Debug.Log($"No valid waypoints for movement. Current index: {currentWaypointIndex}");
                agent.isStopped = true;
                return;
            }

            GameObject targetWaypoint = waypoints[currentWaypointIndex];
            float distanceToWaypoint = Vector3.Distance(transform.position, targetWaypoint.transform.position);

            // If the enemy reaches the current waypoint, update the index
            if (distanceToWaypoint <= agent.stoppingDistance)
            {
                Debug.Log($"Reached waypoint: {targetWaypoint.name}. Updating index.");

                if (isHostile)
                {
                    currentWaypointIndex--; // Enemies move backward through the list
                }
                else
                {
                    currentWaypointIndex++; // Players move forward through the list
                }

                // Stop if the index is out of bounds
                if (currentWaypointIndex < 0 || currentWaypointIndex >= waypoints.Count)
                {
                    Debug.Log("No more waypoints to move towards. Stopping.");
                    agent.isStopped = true;
                    return;
                }

                // Update the target waypoint
                targetWaypoint = waypoints[currentWaypointIndex];
                Debug.Log($"Updated target waypoint: {targetWaypoint.name}. New index: {currentWaypointIndex}");
            }

            // Move toward the current waypoint
            agent.isStopped = false;
            agent.SetDestination(targetWaypoint.transform.position);
            Debug.Log($"Moving to waypoint: {targetWaypoint.name}, Distance: {distanceToWaypoint}");
        }
    }



    // Modified HoldPosition for direction adjustment
    public void HoldPosition()
    {
        StartCoroutine(TimerBetweenNavigation());
        UpdateWaypoints();

        // Determine movement direction based on hostility
        Vector3 forwardDirection = isHostile ? Vector3.back : Vector3.forward;

        GameObject closestWaypointInFront = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject waypoint in waypoints)
        {
            coverScript = waypoint.GetComponent<CoverScript>();
            float distance = Vector3.Distance(transform.position, waypoint.transform.position);

            // Skip waypoints that are occupied or too close
            if (coverScript != null && coverScript.occupiedBy != "none" && coverScript.occupiedBy != name)
                continue;

            // Consider the waypoint if it is the closest
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestWaypointInFront = waypoint;
            }
        }

        // If no valid waypoints are found, stop the agent and return
        if (closestWaypointInFront == null)
        {
            agent.isStopped = true;
            Debug.Log("No valid waypoints for holding position.");
            return;
        }

        float distanceToWaypoint = Vector3.Distance(transform.position, closestWaypointInFront.transform.position);

        // If at the waypoint, rotate to face the correct direction
        if (distanceToWaypoint <= agent.stoppingDistance)
        {
            Quaternion targetRotation = Quaternion.LookRotation(forwardDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 200f);

            isAtCover = true;
            agent.isStopped = true;
            Debug.Log("Holding position at waypoint.");
            return;
        }

        // Otherwise, move to the waypoint and adjust rotation
        if (closestWaypointInFront != null)
        {
            isAtCover = false;
            agent.isStopped = false;
            agent.SetDestination(closestWaypointInFront.transform.position);

            Vector3 directionToTarget = (closestWaypointInFront.transform.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 200f);

            Debug.Log($"Moving to hold position at waypoint: {closestWaypointInFront.name}");
        }
        else
        {
            agent.isStopped = true;
        }
    }

    // Modified RunToNearestCover for direction adjustment
    public void RunToNearestCover()
    {
        if (!holdPosition)
        {
            StartCoroutine(TimerBetweenNavigation());
            UpdateWaypoints();
            Vector3 forwardDirection = isFriendly ? Vector3.forward : Vector3.back;

            GameObject closestWaypointInFront = null;
            float closestDistance = Mathf.Infinity;

            foreach (GameObject waypoint in waypoints)
            {
                coverScript = waypoint.GetComponent<CoverScript>();
                float distance = Vector3.Distance(transform.position, waypoint.transform.position);

                if (coverScript != null && coverScript.occupiedBy != "none" && coverScript.occupiedBy != name)
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

    // Modified RunToBackToCover for direction adjustment
    public void RunToBackToCover()
    {
        if (!holdPosition)
        {
            StartCoroutine(TimerBetweenNavigation());
            UpdateWaypoints();
            Vector3 forwardDirection = isFriendly ? Vector3.forward : Vector3.back;

            GameObject closestWaypointInFront = null;
            float closestDistance = Mathf.Infinity;

            foreach (GameObject waypoint in waypoints)
            {
                coverScript = waypoint.GetComponent<CoverScript>();
                Vector3 directionToWaypoint = (waypoint.transform.position - transform.position).normalized;

                if (coverScript != null && coverScript.occupiedBy != "none" && coverScript.occupiedBy != name)
                    continue;

                float dotProduct = Vector3.Dot(forwardDirection, directionToWaypoint);
                if (dotProduct < 0) // Only consider waypoints in the opposite direction for enemies
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

    // Modified RunToEnemy for direction adjustment
    public void RunToEnemy(Transform enemy)
    {
        if (!holdPosition && canMove)
        {
            StartCoroutine(TimerBetweenNavigation());
            UpdateWaypoints();
            Vector3 forwardDirection = isFriendly ? Vector3.right : Vector3.left;

            GameObject closestWaypointInFront = null;
            float closestDistance = Mathf.Infinity;

            foreach (GameObject waypoint in waypoints)
            {
                coverScript = waypoint.GetComponent<CoverScript>();
                Vector3 directionToWaypoint = (waypoint.transform.position - transform.position).normalized;

                if (coverScript != null && coverScript.occupiedBy != "none" && coverScript.occupiedBy != name)
                    continue;

                float dotProduct = Vector3.Dot(forwardDirection, directionToWaypoint);

                if (dotProduct < 0) // Only consider waypoints opposite for enemies
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

    public void RotateToEnemy(Vector3 targetPosition)
    {
        // Calculate direction to the target
        Vector3 directionToTarget = targetPosition - transform.position;

        // Check if direction is non-zero before using LookRotation
        if (directionToTarget != Vector3.zero)
        {
            // Normalize the direction to avoid issues with scale
            directionToTarget.Normalize();

            // Rotate towards the target
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 500f);
        }
    }

    private IEnumerator TimerBetweenNavigation()
    {
        if (isTimerRunning)
        {
            Debug.Log("Timer already running. Skipping.");
            yield break;
        }
        isTimerRunning = true;

        canMove = false;
        Debug.Log("Timer started. Movement disabled.");

        yield return new WaitForSeconds(3f);

        canMove = true;
        isTimerRunning = false;
        Debug.Log("Timer ended. Movement re-enabled.");
    }


    public void TakeDamage(int damage)
    {
        personality.health -= damage;
        Instantiate(bloodPartiles, transform.position, Quaternion.identity);

        if (personality.health <= 0)
        {
            Debug.Log("DEAD " + gameObject.name);
            // Handle death
        }
    }
}
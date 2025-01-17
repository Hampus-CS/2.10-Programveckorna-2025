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

    public bool inCombat = false;
    public bool holdPosition = false;
    public bool isAtCover = false;
    public bool canMove = true;

    private bool isFriendly;

    void Start()
    {
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
    }

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

    // Modified MoveForwards for direction adjustment
    public void MoveForwards()
    {
        if (!holdPosition && canMove)
        {
            StartCoroutine(TimerBetweenNavigation());
            UpdateWaypoints();

            // Determine the movement direction based on whether the unit is friendly or enemy
            Vector3 forwardDirection = isFriendly ? Vector3.forward : Vector3.forward;
            GameObject closestWaypoint = null;
            float closestDistance = Mathf.Infinity;

            foreach (GameObject waypoint in waypoints)
            {
                coverScript = waypoint.GetComponent<CoverScript>();

                // Skip waypoints that are too close or occupied
                if (Vector3.Distance(transform.position, waypoint.transform.position) < 1f ||
                    (coverScript != null && coverScript.occupiedBy != "none" && coverScript.occupiedBy != name))
                {
                    continue;
                }

                Vector3 directionToWaypoint = (waypoint.transform.position - transform.position).normalized;
                float dotProduct = Vector3.Dot(forwardDirection, directionToWaypoint);

                // Debugging: Check if we're moving in the correct direction
                Debug.Log($"dotProduct for waypoint {waypoint.name}: {dotProduct}");

                // Only consider waypoints in the correct direction
                if ((isFriendly && dotProduct > 0) || (!isFriendly && dotProduct < 0))
                {
                    float distance = Vector3.Distance(transform.position, waypoint.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestWaypoint = waypoint;
                    }
                }
            }

            // Move to the closest valid waypoint
            if (closestWaypoint != null)
            {
                agent.isStopped = false;
                agent.SetDestination(closestWaypoint.transform.position);
                Debug.Log($"Moving towards waypoint: {closestWaypoint.name}");
            }
            else
            {
                agent.isStopped = true;
                Debug.Log("No valid waypoint found to move towards.");
            }
        }
    }

    // Modified HoldPosition for direction adjustment
    public void HoldPosition()
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

        if (closestWaypointInFront == null)
        {
            agent.isStopped = true;
            return;
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
        canMove = false;
        yield return new WaitForSeconds(3f);
        canMove = true;
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
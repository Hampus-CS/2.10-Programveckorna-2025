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

            Vector3 forwardDirection = isFriendly ? Vector3.right : Vector3.left; // Right for friendly, left for enemy
            GameObject closestWaypointInFront = null;
            float closestDistance = Mathf.Infinity;

            foreach (GameObject waypoint in waypoints)
            {
                coverScript = waypoint.GetComponent<CoverScript>();

                if (Vector3.Distance(transform.position, waypoint.transform.position) < 1f || (coverScript != null && coverScript.occupiedBy != "none" && coverScript.occupiedBy != name))
                {
                    continue;
                }

                Vector3 directionToWaypoint = (waypoint.transform.position - transform.position).normalized;

                // Adjust based on isFriendly
                float dotProduct = Vector3.Dot(forwardDirection, directionToWaypoint);
                if (dotProduct > 0) // Only consider waypoints in the desired direction
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
            }
            else
            {
                agent.isStopped = true;
            }
        }
    }

    // Modified HoldPosition for direction adjustment
    public void HoldPosition()
    {
        StartCoroutine(TimerBetweenNavigation());
        UpdateWaypoints();
        Vector3 forwardDirection = isFriendly ? Vector3.right : Vector3.left;

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
            Vector3 forwardDirection = isFriendly ? Vector3.right : Vector3.left;

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

        if (personality.health <= 0)
        {
            Debug.Log("DEAD " + gameObject.name);
            // Handle death
        }
    }
}
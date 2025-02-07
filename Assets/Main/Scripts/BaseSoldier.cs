using System.Collections.Generic;
using System.Collections;
using UnityEngine.AI;
using UnityEngine;

public class BaseSoldier : MonoBehaviour
{
    public List<AudioClip> damageSounds = new List<AudioClip>();
    public List<AudioClip> walkingSounds = new List<AudioClip>();

    private AudioSource audioSource;

    private NavMeshAgent agent;
    private TroopPersonalityScript personality;
    private RangeColliderScript rangeColliderScript;
    public List<GameObject> waypoints = new List<GameObject>();

    private GameObject playerBase;
    private GameObject enemyBase;

    public ParticleSystem bloodPartiles;

    private bool isTimerRunning = false;

    public bool inCombat = false;
    public bool holdPosition = false;
    public bool isAtCover = false;
    public bool canMove = true;

    private bool isFriendly;
    public bool isHostile; // New property to distinguish enemies from friendlies

    private int currentWaypointIndex = 0; // Tracks the current waypoint

    Animator animator;

    private float agentTimer = 4f;


    private IEnumerator AgentTimer()
    {
        agent.enabled = false;
        yield return new WaitForSeconds(agentTimer);
        agent.enabled = true;
    }

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        audioSource.volume *= 0.0f;

        Debug.Log($"isHostile before Start logic: {isHostile}");
        coverScript = FindAnyObjectByType<CoverScript>();
        personality = GetComponent<TroopPersonalityScript>();
        rangeColliderScript = GetComponentInChildren<RangeColliderScript>();
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = true;
        agent.ResetPath();

        // Find the base GameObjects by name in the scene
        playerBase = GameObject.Find("PlayerBase"); // Ensure this matches the exact GameObject name
        enemyBase = GameObject.Find("EnemyBase");

        if (playerBase == null || enemyBase == null)
        {
            Debug.LogError("PlayerBase or EnemyBase GameObjects not found in the scene!");
        }

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

    /*
    private void UpdateWaypoints()
    {
        Vector3 position = transform.position;
        Vector3 basePos = new Vector3(-6.58f, 10f, 10f);

        waypoints.Sort((a, b) =>
            Vector3.Distance(a.transform.position, position)
            .CompareTo(Vector3.Distance(b.transform.position, position)));

        // Reverse the order for hostile units to traverse the waypoints backward
        if (isHostile)
        {
            waypoints.Reverse();
        }

        Debug.Log($"Waypoints updated for {(isHostile ? "Enemy" : "Player")}. Waypoint count: {waypoints.Count}");
    }
    */

    private void UpdateWaypoints()
    {
        if (playerBase == null || enemyBase == null)
        {
            Debug.LogError("Base GameObjects are not assigned or found!");
            return;
        }

        Vector3 position = transform.position;

        // Sorting waypoints based on distance
        waypoints.Sort((a, b) =>
            isHostile
                ? Vector3.Distance(b.transform.position, position).CompareTo(Vector3.Distance(a.transform.position, position)) // Enemies: farthest first
                : Vector3.Distance(a.transform.position, position).CompareTo(Vector3.Distance(b.transform.position, position)) // Players: closest first
        );

        Debug.Log($"Waypoints updated for {(isHostile ? "Enemy" : "Player")}. Waypoint count: {waypoints.Count}");
    }


    private void AddWaypoint(Vector3 position, string name, bool atStart)
    {
        GameObject waypoint = new GameObject(name);
        waypoint.transform.position = position;

        if (atStart)
            waypoints.Insert(0, waypoint);
        else
            waypoints.Add(waypoint);
    }

    private CoverScript coverScript;

    // Modified MoveForwards for direction adjustment //
    public void MoveForwards()
    {
        if (!holdPosition && canMove)
        {
            StartCoroutine(TimerBetweenNavigation());

            // Check for valid waypoints
            if (currentWaypointIndex < 0 || currentWaypointIndex >= waypoints.Count)
            {
                // Move directly to the target base when waypoints are exhausted
                GameObject targetBase = isHostile ? playerBase : enemyBase;
                agent.isStopped = false;
                agent.SetDestination(targetBase.transform.position);
                animator.SetBool("Walk", true);
                Debug.Log($"Moving directly to {(isHostile ? "PlayerBase" : "EnemyBase")}.");
                return;
            }

            // Move towards current waypoint
            GameObject targetWaypoint = waypoints[currentWaypointIndex];
            float distanceToWaypoint = Vector3.Distance(transform.position, targetWaypoint.transform.position);

            if (distanceToWaypoint <= agent.stoppingDistance)
            {
                if (isHostile)
                    currentWaypointIndex = Mathf.Max(0, currentWaypointIndex - 1); // Prevent negative index
                else
                    currentWaypointIndex = Mathf.Min(waypoints.Count - 1, currentWaypointIndex + 1);
            }

            // Update the target
            if (currentWaypointIndex >= 0 && currentWaypointIndex < waypoints.Count)
            {
                targetWaypoint = waypoints[currentWaypointIndex];
                agent.isStopped = false;
                agent.SetDestination(targetWaypoint.transform.position);
                animator.SetBool("Walk", true);
                Debug.Log($"Moving to waypoint: {targetWaypoint.name}");
            }
            else
            {
                // Move directly to the target base when waypoints are exhausted
                GameObject targetBase = isHostile ? playerBase : enemyBase;
                agent.isStopped = false;
                agent.SetDestination(targetBase.transform.position);
                animator.SetBool("Walk", true);
                Debug.Log($"Moving directly to {(isHostile ? "PlayerBase" : "EnemyBase")}.");
            }
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

        yield return new WaitForSeconds(0.5f);

        canMove = true;
        isTimerRunning = false;
        Debug.Log("Timer ended. Movement re-enabled.");
    }

    public void TakeDamage(int damage)
    {
        personality.health -= damage;
        Instantiate(bloodPartiles, transform.position, Quaternion.identity);

        // Handle death
        if (personality.health <= 0)
        {
            animator.SetBool("Die", true);
            StartCoroutine(DeathDelay());
            animator.SetBool("Die", false);
            Destroy(gameObject);

            Debug.Log("DEAD " + gameObject.name);
        }

        if (damage >= 10 && damageSounds.Count > 0) // Ensure there are sounds to play
        {
            int rand = Random.Range(0, damageSounds.Count); // Random index
            audioSource.PlayOneShot(damageSounds[rand]);    // Play the selected sound
        }
    }

    private IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(1.5f);
    }
}
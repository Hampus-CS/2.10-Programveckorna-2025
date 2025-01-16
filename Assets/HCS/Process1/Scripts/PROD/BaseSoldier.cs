using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent))]
public class BaseSoldier : MonoBehaviour
{
    [Header("Soldier Settings")]
    public bool IsPlayer; // Differentiates between player and enemy soldiers
    public float AttackDamage = 10f;
    public float AttackRange = 2f;
    public float AttackInterval = 1.5f; // Time between attacks

    [Header("Personality Settings")]
    public int health { get; set; }
    public int stress { get; set; }
    public int range { get; set; }
    public float accuracy { get; set; }
    public float suppresion { get; set; }
    public string personality { get; set; }

    [Header("Health UI")]
    public GameObject HealthBarPrefab; // Prefab for the health bar UI
    private Slider healthSlider; // The slider component of the health bar
    private GameObject healthBarInstance;

    private float currentHealth;
    private NavMeshAgent agent;
    private Transform currentTargetSlot;
    private bool isAtSlot = false;

    [Header("Combat")]
    private float lastAttackTime;
    private BaseSoldier targetEnemy; // Target enemy for combat

    private Animator animator;

    public LineManager CurrentTargetLine { get; private set; }

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();

        health = 100;
        stress = 0;
        range = 10;
        suppresion = 0;
        accuracy = 5;
        Personality();

        currentHealth = health;

        if (agent == null)
        {
            Debug.LogError($"{name}: NavMeshAgent component is missing! Ensure the prefab has a NavMeshAgent.");
        }

        // Set up the health bar
        if (HealthBarPrefab != null)
        {
            healthBarInstance = Instantiate(HealthBarPrefab, transform.position + Vector3.up * 1.5f, Quaternion.identity, transform);
            healthSlider = healthBarInstance.GetComponentInChildren<Slider>();
            UpdateHealthBar();
        }

        FindNextLine();
    }

    private void Update()
    {
        // Check for movement to slot
        if (!isAtSlot && currentTargetSlot != null)
        {

            MoveToSlot();
        }

        // Engage in combat if at slot
        if (isAtSlot)
        {
            EngageCombat();
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        // Update the health bar
        if (healthBarInstance != null)
        {
            var healthBarUI = healthBarInstance.GetComponent<HealthBarUI>();
            if (healthBarUI != null)
            {
                healthBarUI.UpdateHealthBar(currentHealth, health);
            }
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth / health;
        }
    }

    private void Die()
    {
        if (healthBarInstance != null)
        {
            Destroy(healthBarInstance);
        }

        // Notify the line about death
        if (CurrentTargetLine != null)
        {
            CurrentTargetLine.RemoveSoldier(gameObject, IsPlayer);
        }

        // Notify the Spawner
        var spawner = FindObjectOfType<Spawner>();
        if (spawner != null)
        {
            spawner.SoldierDied(IsPlayer);
        }

        // Play death animation
        animator.SetBool("Die", true);
        StartCoroutine(DelayDeath());

        print($"{name} has died.");
    }

    // Wait for the death animation to finish before destroying the object
    private IEnumerator DelayDeath()
    {
        yield return new WaitForSeconds(1.5f);
        animator.SetBool("Die", false);
        Destroy(gameObject);
    }

    public void SetTargetLine(LineManager targetLine)
    {
        if (targetLine == null)
        {
            Debug.LogError($"{name}: Target line is null! Cannot set target line.");
            return;
        }

        CurrentTargetLine = targetLine;

        if (CurrentTargetLine.slots == null || CurrentTargetLine.slots.Length == 0)
        {
            Debug.LogError($"{name}: Target line {CurrentTargetLine.name} has no slots assigned!");
            return;
        }

        MoveToSlot();
    }

    private void FindNextLine()
    {
        LineManager[] allLines = FindObjectsOfType<LineManager>();

        foreach (var line in allLines)
        {
            if (IsValidLine(line))
            {
                SetTargetLine(line);
                return;
            }
        }

        Debug.LogWarning($"{name} could not find a valid line!");
    }

    private bool IsValidLine(LineManager line)
    {
        if (IsPlayer)
        {
            return line.CurrentState != LineManager.LineState.EnemyOwned;
        }
        else
        {
            return line.CurrentState != LineManager.LineState.PlayerOwned;
        }
    }

    private void MoveToSlot()
    {
        if (agent == null)
        {

            // Attempt to find the NavMeshAgent if it wasn't initialized in Start()
            agent = GetComponent<NavMeshAgent>();
            if (agent == null)
            {
                Debug.LogError($"{name}: NavMeshAgent component is missing! Cannot move to slot.");
                return;
            }
        }

        if (CurrentTargetLine == null)
        {
            Debug.LogError($"{name}: CurrentTargetLine is null! Cannot move to slot.");
            return;
        }

        currentTargetSlot = CurrentTargetLine.GetFreeSlot();

        if (currentTargetSlot == null)
        {
            Debug.LogWarning($"{name}: No free slot available in line {CurrentTargetLine.name}. Holding position.");
            HoldPosition();
            return;
        }

        isAtSlot = false;
        agent.isStopped = false;
        animator.SetBool("Walk", true);
        agent.SetDestination(currentTargetSlot.position);
        Debug.Log($"{name} moving to slot at {currentTargetSlot.position}");
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == currentTargetSlot)
        {
            isAtSlot = true;
            agent.isStopped = true;
            Debug.Log($"{name} reached its assigned slot in line {CurrentTargetLine.name}");
        }
    }

    private void HoldPosition()
    {
        animator.SetBool("Walk", false);
        if (agent == null) return; // Prevent null reference
        agent.isStopped = true;
    }

    private void EngageCombat()
    {
        // Find the nearest enemy in range
        Collider[] hits = Physics.OverlapSphere(transform.position, AttackRange);

        foreach (var hit in hits)
        {
            var enemy = hit.GetComponent<BaseSoldier>();
            if (enemy != null && enemy.IsPlayer != IsPlayer)
            {
                targetEnemy = enemy;
                break;
            }
        }

        if (targetEnemy != null && Time.time >= lastAttackTime + AttackInterval)
        {
            animator.SetBool("Shoot", true);
            lastAttackTime = Time.time;
            targetEnemy.TakeDamage(AttackDamage);
            Debug.Log($"{name} attacked {targetEnemy.name} for {AttackDamage} damage.");
        }
    }

    private void RotateToFaceTarget(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }
    public void Personality()
    {
        int rand = Random.Range(1, 6);
        switch (rand)
        {
            case 1:
                personality = "Aggresive";
                health += 10;
                break;
            case 2:
                personality = "Coward";
                stress += 1;
                break;
            case 3:
                personality = "Sharpshooter";
                accuracy += 1;
                agent.speed *= 0.8f;
                break;
            case 4:
                personality = "Triggerhappy";
                accuracy -= 1;
                range += 5;
                break;
            case 5:
                personality = "Lightfooted";
                health -= 10;
                agent.speed *= 1.10f;
                break;
        }
    }
}
public interface ISoldierStats
{
    public int health { get; set; }
    public int stress { get; set; }
    public int range { get; set; }
    public float accuracy { get; set; }
    public float suppresion { get; set; }
    public string personality { get; set; }

    void Personality();
}

/// <summary>
/// Key Features
/// 
///     Unified Soldier Behavior:
///         - Differentiates between player and enemy soldiers using the IsPlayer flag.
///         - Manages health, movement, and combat within a single script.
/// 
///     Health and Visual Feedback:
///         - Includes a health bar system for visual damage indication.
///         - Updates health bars dynamically as soldiers take damage.
/// 
///     Combat System:
///         - Soldiers engage enemies within range and attack based on an interval.
///         - Supports combat in contested lines.
/// 
///     Slot and Line Integration:
///         - Soldiers move to assigned slots in their target line.
///         - Notify LineManager when occupying or leaving a line.
/// </summary>

// How to Use
// 1. Replace PlayerSoldier.cs, EnemySoldier.cs, and Health.cs with the new BaseSoldier.cs.
// 2. Add this script to all soldier prefabs.
// 3. Assign the IsPlayer flag in the Unity Inspector for player and enemy soldiers.
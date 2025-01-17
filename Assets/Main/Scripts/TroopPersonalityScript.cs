using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class TroopPersonalityScript : MonoBehaviour, ITroopInterfaceScript
{
    private GameObject targetedTroop;
    //private TroopNavigation troopNavigation;
    private NavMeshAgent agent;
    public int health { get; set; }
    public int stress { get; set; }
    public int range { get; set; }
    public float accuracy { get; set; }
    public float suppresion { get; set; }
    public string personality { get; set; }

    public bool isFriendly;
    public int startStress;
    public float baseAccuracy;
    public float accuracyDecrese = 0.05f;
    private float suppressionRecoveryRate = 0.15f; // Rate at which suppression decreases
    public int hurtStress;

    void Start()
    {
        if (gameObject.CompareTag("EnemyTroop"))
        {
            isFriendly = false;
        } else if (gameObject.CompareTag("FriendlyTroop"))
        {
            isFriendly = true;
        }

        //troopNavigation = GetComponent<TroopNavigation>();
        health = 100;
        stress = 0;
        range = 10;
        suppresion = 0;
        accuracy = 0.5f;
        Personality();

        startStress = stress;
        baseAccuracy = accuracy;

        agent = GetComponent<NavMeshAgent>();
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
                accuracy -= 0.08f;
                //agent.speed *= 0.85f;
                break;
            case 4:
                personality = "Triggerhappy";
                accuracy += 0.1f;
                range += 5;
                break;
            case 5:
                personality = "Lightfooted";
                health -= 10;
                //agent.speed *= 1.10f;
                break;
        }
    }

    public void Stress()
    {
        // Ensure stress never goes below zero
        if (stress < startStress)
        {
            stress = startStress;
        }

        // Ensure suppression is non-negative
        suppresion = Mathf.Max(0, suppresion);

        // Calculate stress as 1 for every 10 suppression points
        stress = Mathf.Clamp(Mathf.FloorToInt(suppresion / 10f), 0, 10); // Limit max stress to 100

        Debug.Log($"Suppression: {suppresion}, Calculated Stress: {stress}");

        // Trigger stress timer if stress is high enough
        if (stress > startStress)
        {
            StartCoroutine(StressTimer());
        }

        // Optional: Stress recovery logic
        if (stress > startStress)
        {
            stress -= Mathf.FloorToInt(Time.deltaTime * 0.05f); // Gradual stress decrease over time
        }
    }

    public void Suppression()
    {
        if (suppresion < 0)
        {
            suppresion = 0;
        }

        // Gradual suppression recovery
        if (suppresion > 0)
        {
            suppresion -= Time.deltaTime * suppressionRecoveryRate;
        }

        // Apply suppression effects to accuracy
        if (suppresion > 0)
        {
            float accuracyPenalty = accuracyDecrese * suppresion; // More suppression -> larger penalty
            accuracy = Mathf.Max(baseAccuracy + accuracyPenalty, 1f); // Ensure accuracy doesn't go above 1
        }
        else
        {
            // Gradual recovery of accuracy as suppression decreases
            accuracy = Mathf.Min(baseAccuracy, accuracy + suppressionRecoveryRate); // Recover to base accuracy
        }

        Debug.Log("Suppression: " + suppresion + ", Accuracy: " + accuracy);
    }

    private void Update()
    {
        Suppression();
        Stress();
        Debug.LogWarning("stress = " + stress);
        Debug.LogError("supress = " + suppresion);
    }

    private IEnumerator StressTimer()
    {
        yield return new WaitForSeconds(8);
        stress -= 1;
    }
}
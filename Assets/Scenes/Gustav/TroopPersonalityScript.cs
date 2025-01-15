using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.SearchService;

public class TroopPersonalityScript : MonoBehaviour, ITroopInterfaceScript
{
    private GameObject targetedTroop;
    private TroopNavigation troopNavigation;
    private NavMeshAgent agent;
    public int health { get; set; }
    public int stress { get; set; }
    public int range { get; set; }
    public float accuracy { get; set; }
    public float suppresion { get; set; }
    public string personality { get; set; }

    private int startStress;

    private void Start()
    {
        troopNavigation = GetComponent<TroopNavigation>();
        health = 100;
        stress = 0;
        range = 10;
        suppresion = 0;
        accuracy = 0.1f;
        Personality();

        startStress = stress;

        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        DeStress();
    }

    private void DeStress()
    {
        if (stress > 0)
        {
            float stressReduction = Time.deltaTime / 10;
            stress -= ((int)stressReduction);
            Debug.Log(stress);
        }
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
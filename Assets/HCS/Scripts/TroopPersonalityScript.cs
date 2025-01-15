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

    void Start()
    {
        //troopNavigation = GetComponent<TroopNavigation>();
        health = 100;
        stress = 0;
        range = 10;
        suppresion = 0;
        accuracy = 5;
        Personality();

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
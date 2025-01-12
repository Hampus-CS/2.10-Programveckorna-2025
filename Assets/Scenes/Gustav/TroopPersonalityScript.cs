using System.Collections;
using UnityEngine;

public class TroopPersonalityScript : MonoBehaviour, ITroopInterfaceScript
{
    private GameObject targetedTroop;
    private TroopNavigation troopNavigation;
    public int health { get; set; }
    public int stress { get; set; }
    public int range { get; set; }
    public float suppresion { get; set; }
    public string personality { get; set; }

    void Start()
    {
        troopNavigation = GetComponent<TroopNavigation>();
        health = 100;
        stress = 0;
        range = 10;
        suppresion = 0;
        Personality();
    }

    public void Personality()
    {
        int rand = Random.Range(1, 3);
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
        }
    }

    public void Shoot(Vector3 target, GameObject weapon)
    {
        
    }

    public void EnterCombat(Vector3 target, int stress)
    {
        Debug.Log("Entering combat with target: " + target + " Stress level: " + stress);
    }
}
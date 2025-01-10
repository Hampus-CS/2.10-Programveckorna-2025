using UnityEngine;

public class TroopPersonalityScript : MonoBehaviour, ITroopInterfaceScript
{
    private GameObject targetedTroop;
    public int health { get; set; }
    public int stress { get; set; }
    public string personality { get; set; }

    void Start()
    {
        health = 100;
        stress = 0;
        Personality();
    }

    void Update()
    {

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyTroop"))
        {
            
        }
    }
}
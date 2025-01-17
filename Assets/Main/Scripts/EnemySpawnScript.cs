using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawningScript : MonoBehaviour
{
    private float coolDown = 5;
    private bool canSpawn = true;

    public List<GameObject> troops = new List<GameObject>();

    public GameObject spawn1;
    public GameObject spawn2;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (canSpawn)
        {
            StartCoroutine(Cooldown());
            SpawnTroop(1);
        }
    }

    private void SpawnTroop(int times)
    {
        if (troops.Count > 0)
        {
            for (int i = 0; i < times; i++)
            {
                float spawnX = Random.Range(spawn2.transform.position.x, spawn1.transform.position.x);

                int rand = Random.Range(0, troops.Count);

                Vector3 spawnPosition = new Vector3(spawnX, spawn1.transform.position.y, spawn1.transform.position.z);

                // Instantiate the troop
                GameObject spawnedTroop = Instantiate(troops[rand], spawnPosition, Quaternion.identity);

                // Set the isHostile property for the spawned troop
                BaseSoldier baseSoldier = spawnedTroop.GetComponent<BaseSoldier>();
                if (baseSoldier != null)
                {
                    baseSoldier.isHostile = true; // Mark as hostile
                    Debug.Log($"Spawned enemy at {spawnPosition} with isHostile = {baseSoldier.isHostile}");
                }
                else
                {
                    Debug.LogError("Spawned object does not have a BaseSoldier component!");
                }
            }
        }
    }

    private IEnumerator Cooldown()
    {
        canSpawn = false;
        yield return new WaitForSeconds(coolDown);
        canSpawn = true;
    }
}

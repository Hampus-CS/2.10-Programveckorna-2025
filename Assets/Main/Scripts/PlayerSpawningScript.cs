using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSpawningScript : MonoBehaviour
{
    private float coolDown = 1;
    private int cost = 10;
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
            
            if (Input.GetKeyDown(KeyCode.Q))
            {
                StartCoroutine(Cooldown());

                SpawnTroop(1);
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                StartCoroutine(Cooldown());

                SpawnTroop(2);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(Cooldown());

                SpawnTroop(3);
            }
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
                Instantiate(troops[rand], spawnPosition, Quaternion.identity);
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

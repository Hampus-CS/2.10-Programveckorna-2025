using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Linq;

public class RangeColliderScript : MonoBehaviour
{
    private SphereCollider collider;
    private Weapon weapon;
    public List<GameObject> triggers = new List<GameObject>();
    private TroopPersonalityScript personality;

    bool isReady = false;
    private float range;
    public bool isFriendly;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        personality = GetComponentInParent<TroopPersonalityScript>();
        collider = GetComponent<SphereCollider>();
        weapon = GetComponentInParent<Weapon>();

        StartCoroutine(wait());
    }

    // Update is called once per frame
    void Update()
    {
        if (isReady)
        {
            EnemyByDistance();
            RemoveMissingObjects();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isReady)
        {
            Debug.Log("TRIGGER " + other.name);
            if (isFriendly && other.CompareTag("EnemyTroop"))
            {
                triggers.Add(other.gameObject);
            }
            if (!isFriendly && other.CompareTag("FriendlyTroop"))
            {
                triggers.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isFriendly && other.CompareTag("EnemyTroop"))
        {
            triggers.Remove(other.gameObject);
        }
        if (!isFriendly && other.CompareTag("FriendlyTroop"))
        {
            triggers.Remove(other.gameObject);
        }
    }

    private void EnemyByDistance()
    {
        triggers.Sort((enemy1, enemy2) =>
        Vector3.Distance(transform.position, enemy1.transform.position)
        .CompareTo(Vector3.Distance(transform.position, enemy2.transform.position)));
    }
    void RemoveMissingObjects()
    {
        // Remove all null objects from the list
        triggers.RemoveAll(item => item == null);
    }

    private IEnumerator wait()
    {
        yield return new WaitForSeconds(2);
        isFriendly = personality.isFriendly;
        isReady = true;
        range = weapon.range;
        collider.radius = range;
    }
}

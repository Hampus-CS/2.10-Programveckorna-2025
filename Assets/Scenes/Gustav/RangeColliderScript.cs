using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RangeColliderScript : MonoBehaviour
{
    private SphereCollider collider;
    public List<GameObject> triggers = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        collider = GetComponent<SphereCollider>();
        collider.radius = 10f;
    }

    // Update is called once per frame
    void Update()
    {
        EnemyByDistance();
        RemoveMissingObjects();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("TRIGGER " + other.name);
        if (other.CompareTag("EnemyTroop"))
        {
            triggers.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("EnemyTroop"))
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
}

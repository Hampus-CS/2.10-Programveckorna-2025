using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.iOS;

public class CoverScript : MonoBehaviour
{
    public string occupiedBy = "none";
    private int troopsInCollider = 0;

    private BoxCollider collider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        collider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position, collider.size / 2, Quaternion.identity);
        troopsInCollider = 0;

        foreach (Collider col in colliders)
        {
            if (col.gameObject.GetComponent<NavMeshAgent>())
            {
                troopsInCollider++;
                occupiedBy = col.name;
            }
        }

        if (troopsInCollider == 0)
        {
            occupiedBy = "none";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<NavMeshAgent>() && troopsInCollider == 0)
        {
            Debug.Log("TRIGGERS");
            troopsInCollider++;
            occupiedBy = other.name;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent <NavMeshAgent>())
        {
            troopsInCollider--;
        }
    }
}

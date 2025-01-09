using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CoverScript : MonoBehaviour
{
    public bool occupied = false;
    private int troopsInCollider = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (troopsInCollider >= 1)
        {
            occupied = true;
        } else
        {
            occupied = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<NavMeshAgent>())
        {
            troopsInCollider++;
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

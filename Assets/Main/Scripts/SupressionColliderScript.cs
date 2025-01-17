using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;

public class SupressionColliderScript : MonoBehaviour
{
    private TroopPersonalityScript personality;
    private SphereCollider collider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        collider = GetComponent<SphereCollider>();
        collider.radius = 5f;

        personality = GetComponentInParent<TroopPersonalityScript>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (personality.isFriendly)
        {
            if (other.CompareTag("EnemyBullet"))
            {
                Supress(other.transform);
            }
        }
        else if (!personality.isFriendly)
        {
            if (other.CompareTag("FriendlyBullet"))
            {
                Supress(other.transform);
            }
        }
    }

    private float distance;
    private float baseSuppression = 10f;
    private void Supress(Transform bullet)
    {
        distance = Vector3.Distance(transform.position, bullet.position);
        personality.suppresion += baseSuppression / distance;
    }
}
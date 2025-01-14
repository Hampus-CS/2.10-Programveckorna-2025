using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;

public class SupressionColliderScript : MonoBehaviour
{
    private TroopPersonalityScript troopPersonalityScript;
    private SphereCollider collider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        collider = GetComponent<SphereCollider>();
        collider.radius = 5f;

        troopPersonalityScript = FindAnyObjectByType<TroopPersonalityScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyBullet"))
        {
            Supress(other.transform);
        }
    }

    private float distance;
    private float baseSuppression = 10f;
    private void Supress(Transform bullet)
    {
        distance = Vector3.Distance(transform.position, bullet.position);
        troopPersonalityScript.suppresion += baseSuppression / distance;
        StartCoroutine(supressFading(distance));
    }

    private IEnumerator supressFading(float distance)
    {
        yield return new WaitForSeconds(collider.radius - distance);
        troopPersonalityScript.suppresion -= baseSuppression / distance;
    }
}

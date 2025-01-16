using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;
using UnityEditor.Rendering;

public class SupressionColliderScript : MonoBehaviour
{
    private BaseSoldier baseSoldier;
    private SphereCollider collider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        collider = GetComponent<SphereCollider>();
        collider.radius = 5f;

        baseSoldier = FindAnyObjectByType<BaseSoldier>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.CompareTag("FriendlyTroop"))
        {
            if (other.CompareTag("EnemyBullet"))
            {
                Supress(other.transform);
            }
        }
        else if (gameObject.CompareTag("EnemyTroop"))
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
        baseSoldier.suppresion += baseSuppression / distance;
        StartCoroutine(supressFading(distance));
    }

    private IEnumerator supressFading(float distance)
    {
        yield return new WaitForSeconds(collider.radius - distance);
        baseSoldier.suppresion -= baseSuppression / distance;
    }
}
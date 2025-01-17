using UnityEngine;

public class Bullet : MonoBehaviour
{
    float speed = 10f; //bulet speed
    public float range;  //bullet range
    public int damage; //bullet damage

    private Weapon weapon;
    private Vector3 startPosition; //where the bullet was spawned

    public bool isFriendly;
    public bool machingunBUllet;

    void Start()
    {
        weapon = FindAnyObjectByType<Weapon>();
        startPosition = transform.localPosition; //get starting position to calculate how far the buullet has traveled
        if (!machingunBUllet)
        {
           range = weapon.range;
        }
    }
    void Update()
    {
       transform.Translate(Vector3.forward * speed * Time.deltaTime); //bullet goes forward

        if(Vector3.Distance(startPosition, transform.position) >= range) //when bullet is at its maximum range it gets destroyed
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        BaseSoldier soldier = other.gameObject.GetComponent<BaseSoldier>();

        if (isFriendly && other.gameObject.CompareTag("EnemyTroop"))
        {
            if (soldier != null)
            {
                soldier.TakeDamage(damage);
            }
            Debug.Log("Bullet hit: " + other.gameObject.name);
            Destroy(gameObject); // Destroy the bullet
        }
        if (isFriendly && other.gameObject.CompareTag("FriendlyTroop"))
        {
            if (soldier != null)
            {
                soldier.TakeDamage(damage);
            }
            Debug.Log("Bullet hit: " + other.gameObject.name);
            Destroy(gameObject); // Destroy the bullet
        }
    }
}

/// <summary>
/// Key Features:
/// 
///     Bullet Mechanics:
///         - Controls bullet speed, range, and damage on collision.
///         - Automatically destroys bullets after reaching max range or hitting a target.
/// 
///     Integration with Weapon:
///         - Inherits damage and range values from the weapon that fired it.
///         - Ensures consistent behavior across different weapons.
/// 
///     Debugging Tools:
///         - Outputs collision events to the console for easier debugging.
/// </summary>

// How to Use
// 1. Attach this script to your bullet prefab GameObjects.
// 2. Customize speed and assign damage/range values dynamically from Weapon.cs.
// 3. Ensure collision handling works for all target types.
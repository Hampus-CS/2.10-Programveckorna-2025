using UnityEngine;

public class Bullet : MonoBehaviour
{
    float speed = 10f; //bulet speed
    public int range;  //bullet range
    public int damage; //bullet damage

    private Vector3 startPosition; //where the bullet was spawned

    void Start()
    {
        startPosition = transform.localPosition; //get starting position to calculate how far the buullet has traveled
    }
    void Update()
    {
       transform.Translate(Vector3.forward * speed * Time.deltaTime); //bullet goes forward

        if(Vector3.Distance(startPosition, transform.position) >= range) //when bullet is at its maximum range it gets destroyed
        {
            Destroy(gameObject);
        }
    }
    void OnCollisionEnter(Collision collision) //when bullet hits something
    {
        Debug.Log("Bullet hit: " + collision.gameObject.name);
        Destroy(gameObject); // Destroy the bullet
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
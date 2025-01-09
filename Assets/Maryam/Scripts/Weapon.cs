using System;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameManager1.Weapon weapon; //reference to the weapon class form gamemanager
    
    public Transform firePoint; //where bullets spawn
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }
    void Shoot()
    {
        if (weapon != null && weapon.bulletPrefab != null)
        {
            //spawns the bullets
           GameObject bulletInstance = Instantiate(
               weapon.bulletPrefab,
               firePoint.position, 
               firePoint.rotation
                
            );
            // here bullets damaga and range get decided
            Bullet bulletScript = bulletInstance.GetComponent<Bullet>();
            if (bulletScript != null) { 
            bulletScript.damage = weapon.damage; //bullets damage is equal to weapon damage
            bulletScript.range = weapon.range; //bullets range is equal to weapons range
            }
            else
            {
                Debug.LogWarning("Weapon or bullet prefab is not assigned");
            }
        }
    }
}




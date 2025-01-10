using System;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameManager1.Weapon weapon; //reference to the weapon class form gamemanager
    public Transform firePoint; //where bullets spawn

    private int damage = 10; //set to private so weapons stats can ony be modified from weapon gameobject or weapon script
    private int range = 5;// same as damage
   
   

    private void Start()
    {
        if(weapon != null)
        {
            damage = weapon.damage; //set damage from GameManager1
            range = weapon.range;  //set range from GameManager1
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }
    public void Shoot()
    {
        if (weapon != null || weapon.bulletPrefab == null)
        {
            Debug.LogWarning("weapon or bullet prefab is not assigned");
            return;
        }
        
          //spawns the bullets
          GameObject bulletInstance = Instantiate(
          weapon.bulletPrefab,
          firePoint.position, 
          firePoint.rotation
          );

            // here bullets damaga and range get decided
            Bullet bulletScript = bulletInstance.GetComponent<Bullet>();

            if (bulletScript != null)
            { 
            bulletScript.damage = damage; //bullets damage is equal to weapon damage
            bulletScript.range = range; //bullets range is equal to weapons range
            }
            else
            {
                Debug.LogWarning("Bullet prefab is not assigned");
            }
          
    }
    public int GetDamage()//int to acces weapons damage
    {
        return damage;
    }
    public int GetRange()//int to acces weapons damage
    {
        return range;
    }
    public void UpgradeDamage(int amount) //void for uppgrading weapons damage in shop
    {

        damage += amount;
        Debug.Log("Weapons damage increased by " + amount + ". New damage: " + amount + damage);
    }
    public void UpgradeRange(int amount)//void to uppgrade weapons range in shop
    {

        range += amount;
        Debug.Log("Weapons range increased by " + amount + ". New range: " + amount + range);
    }
}




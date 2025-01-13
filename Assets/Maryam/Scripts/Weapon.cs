using System;
using UnityEditor.Build;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    [Header("Weapon Stats")]
    [SerializeField]private int damage = 10; //set to private so weapons stats can ony be modified from weapon gameobject or weapon script
    [SerializeField] private int range = 5;// same as damage

    [Header("Upgrae Points")] //to claridy what these ints are for
    public int damageUpgradePoints = 2; //the amount of points the weapons damage will increase with every update
    public int rangeUpgradePoints = 3; //amount of range "points" the weapon will increase with every update

    [Header("Weapon Settings")]
    [SerializeField] private GameObject bulletPrefab; //bullet prefab specifik for each weapon
    public Transform firePoint; //where bullets spawn
    public float shootCoolDown = 1f; //cooldown for shooting
    private float lastShot = 0; //when last shot was shot
    public  int UpgradeDamageCost = 8;
    public int UpgradeRangeCost = 6;
    public int costIncreaseFactor = 2;



    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }
    void Shoot()
    {
        if(Time.time -lastShot>= shootCoolDown)
        {  //check if cooldown passed

         if (firePoint == null) //checks if weapon has firepoint
         {
            Debug.LogWarning("FirePoint not assigned");
            return;
         }
         if(bulletPrefab == null) //checks if weapon has bulletprefab
         {
            Debug.LogWarning("Bullet prefab has not been assigned for this weapon");
         }
        
          //spawns the bullets
          GameObject bulletInstance = Instantiate(
          bulletPrefab,
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
                Debug.LogWarning("Bullet script is missing on the bullet prefab");
            }

            lastShot = Time.time;
        }
        else
        {
            Debug.LogWarning("COOLDOWN");
        }
    }
   public Weapon CreateCopy()
    {
        Weapon newWeapon = new Weapon
        {
            damage = this.damage,
            range = this.range,
            
            bulletPrefab = this.bulletPrefab
        };
        return newWeapon;
    }
    private Weapon GetWeaponFromImage(RectTransform image)
    {
        string imageTag = image.tag;//gets tag from current image and find weapon with same tag
        GameObject weaponObject = GameObject.FindWithTag(imageTag);

        if (weaponObject != null)
        {
            return weaponObject.GetComponent<Weapon>();
        }
        return null; //no weapon found
    }

    public void UpgradeDamage() //void for uppgrading weapons damage in shop with a certain amount
    {
        if (GameManager1.Instance.SpendCurrency(UpgradeDamageCost))//Check if player has enough currency
        {
            damage += damageUpgradePoints; //inceases damage

            UpgradeDamageCost *= costIncreaseFactor; //increase upgrade cost
            Debug.Log("Weapons damage increased by " + damageUpgradePoints + ". New damage: " + damage);
        }
        else
        {
            Debug.LogWarning("Not eanough currency to upgrade");
        }
    }
    public void UpgradeRange() //void for uppgrading weapons damage in shop with a certain amount
    {
        if (GameManager1.Instance.SpendCurrency(UpgradeRangeCost))//Check if player has enough currency
        {
            range += rangeUpgradePoints;

            UpgradeDamageCost *= costIncreaseFactor;
            Debug.Log("Weapons range increased by " + rangeUpgradePoints + ". New range: " + range);
        }
        else
        {
            Debug.LogWarning("Not eanough currency to upgrade");
        }
    }
    
    
    //public getters for weapon stats
    public int GetDamage() => damage; 
    public int GetRange() => range;

    public int GetDamageUpgradePoints() => damageUpgradePoints;
    public int GetRangeUpgradePoints() => rangeUpgradePoints;

}




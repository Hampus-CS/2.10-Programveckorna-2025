using System;
using UnityEditor.Build;
using UnityEngine;
using TMPro;

public class Weapon : MonoBehaviour
{

    [Header("Weapon Stats")]
    [SerializeField] private int damage = 10; //set to private so weapons stats can ony be modified from weapon gameobject or weapon script
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
    public int costIncreaseFactor = 2; //how much the cost of items will increase
    private static int serialNumberCounter = 1; //Static to make sure each new weapon will have a number
    private string uniqueName; //stores the name/serial for weapons
    public float ReloadDuration = 3f; //reload duration
    public int AmmoCap = 10;
    public int  CurrentAmmo;
    public float ReloadTime = 0;

    private float accuracy = 0.1f;
    private TroopPersonalityScript personalityScript;

    private void Awake()
    {
        //give each weapon a unique serial number and update the weapons name
        uniqueName = $"{name.Replace("(Clone)", "").Trim()} #{serialNumberCounter:D4}"; //replaces Clone with a serial number in each weapon thats bought name
        serialNumberCounter++;

        gameObject.name = uniqueName; //make the gameobjects name the same as its name on the list

        CurrentAmmo = AmmoCap;

        personalityScript = GetComponent<TroopPersonalityScript>();
    }
    public string GetUniqueName()
    {
        return uniqueName; //for the debug to work in GM1
    }

    public void Shoot()
    {
        if (CurrentAmmo > 0)
        {
            if (Time.time - lastShot >= shootCoolDown)
            {  //check if cooldown passed

                if (firePoint == null) //checks if weapon has firepoint
                {
                    Debug.LogWarning("FirePoint not assigned");
                    return;
                }
                if (bulletPrefab == null) //checks if weapon has bulletprefab
                {
                    Debug.LogWarning("Bullet prefab has not been assigned for this weapon");
                }

                float spread = 1f / (accuracy * personalityScript.accuracy); 

                Vector3 shootDirection = firePoint.forward;
                // Randomly adjust direction based on accuracy factor
                shootDirection.x += UnityEngine.Random.Range(-accuracy, accuracy);  // Random adjustment for X axis
                shootDirection.y += UnityEngine.Random.Range(-accuracy, accuracy);  // Random adjustment for Y axis

                // Spawn the bullet and apply the modified direction
                GameObject bulletInstance = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(shootDirection));
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
                CurrentAmmo -= 1;
            }
        }
        else
        {
            StartReload(3f);
        }
    }
    private bool isReloading = false;
    public void StartReload(float reloadDuration)
    {
        if (isReloading) return; //prevents multiple reloads
        isReloading = true;
        ReloadTime = reloadDuration;
    }
    private void FixedUpdate()
    {
        if (!isReloading) return;
        ReloadTime -= Time.fixedDeltaTime;

        if(ReloadTime <= 0)
        {
            ReloadTime = 0;
            CurrentAmmo = AmmoCap;
            isReloading = false;

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

    public void UpgradeDamage()
    {
        if (GameManager1.Instance.currency >= UpgradeDamageCost)
        {
            GameManager1.Instance.currency -= UpgradeDamageCost;
            damage += damageUpgradePoints;
            UpgradeDamageCost += costIncreaseFactor; // Increase the cost for the next upgrade

            // Update UI
            GameManager1.Instance.uiManager.UpdateCurrency(GameManager1.Instance.currency);
            GameManager1.Instance.uiManager.UpdateDamageUpgrade(UpgradeDamageCost);

            Debug.Log($"Upgraded damage. New damage: {damage}, New upgrade cost: {UpgradeDamageCost}");
        }
        else
        {
            Debug.LogWarning("Not enough currency to upgrade damage.");
        }
    }
    public void UpgradeRange()
    {
        if (GameManager1.Instance.currency >= UpgradeRangeCost)
        {
            GameManager1.Instance.currency -= UpgradeRangeCost;
            range += rangeUpgradePoints;
            UpgradeRangeCost += costIncreaseFactor; // Increase the cost for the next upgrade

            // Update UI
            GameManager1.Instance.uiManager.UpdateCurrency(GameManager1.Instance.currency);
            GameManager1.Instance.uiManager.UpdateRangeUpgrade(UpgradeRangeCost);

            Debug.Log($"Upgraded range. New range: {range}, New upgrade cost: {UpgradeRangeCost}");
        }
        else
        {
            Debug.LogWarning("Not enough currency to upgrade range.");
        }
    }


    //public getters for weapon stats
    public int GetDamage() => damage; 
    public int GetRange() => range;

    public int GetDamageUpgradePoints() => damageUpgradePoints;
    public int GetRangeUpgradePoints() => rangeUpgradePoints;

}




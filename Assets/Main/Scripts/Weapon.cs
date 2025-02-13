using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class Weapon : MonoBehaviour
{
    private int damage = 10; //set to private so weapons stats can ony be modified from weapon gameobject or weapon script
    public float range2 = 20;

    [Header("Upgrade Points")] //to claridy what these ints are for
    public int damageUpgradePoints = 2; //the amount of points the weapons damage will increase with every update
    public int rangeUpgradePoints = 3; //amount of range "points" the weapon will increase with every update

    [Header("Weapon Settings")]
    [SerializeField] private GameObject bulletPrefab; //bullet prefab specifik for each weapon
    public Transform firePoint; //where bullets spawn
    public float shootCoolDown = 1f; //cooldown for shooting
    private float lastShot = 0; //when last shot was shot
    public int UpgradeDamageCost = 8;
    public int UpgradeRangeCost = 6;
    public int costIncreaseFactor = 2; //how much the cost of items will increase
    private static int serialNumberCounter = 1; //Static to make sure each new weapon will have a number
    private string uniqueName; //stores the name/serial for weapons
    public float ReloadDuration = 3f; //reload duration
    public int AmmoCap = 10;
    public int CurrentAmmo;
    public float ReloadTime = 0;
    public Sprite Icon;
    private bool isFriendly;

    private float accuracy = 5f;
    private TroopPersonalityScript personalityScript;
    private RangeColliderScript rangeColliderScript;

    Animator animator;

    private AudioSource audioSource;
    public List<AudioClip> shootingSounds = new List<AudioClip>();

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 1;
        animator = GetComponentInChildren<Animator>();

        //give each weapon a unique serial number and update the weapons name
        uniqueName = $"{name.Replace("(Clone)", "").Trim()} #{serialNumberCounter:D4}"; //replaces Clone with a serial number in each weapon thats bought name
        serialNumberCounter++;

        gameObject.name = uniqueName; //make the gameobjects name the same as its name on the list

        CurrentAmmo = AmmoCap;

        personalityScript = GetComponent<TroopPersonalityScript>();
        rangeColliderScript = FindAnyObjectByType<RangeColliderScript>();

        if (gameObject.CompareTag("EnemyTroop"))
        {
            isFriendly = false;
        }
        else if (gameObject.CompareTag("FriendlyTroop"))
        {
            isFriendly = true;
        }
    }

    public string GetUniqueName()
    {
        return uniqueName; //for the debug to work in GM1
    }

    public void Shoot(Vector3 target)
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

                animator.SetBool("Shoot", true);

                float spread = 1f / (accuracy * personalityScript.accuracy);

                Vector3 shootDirection = (target - firePoint.position).normalized;
                // Randomly adjust direction based on accuracy factor
                shootDirection.x += UnityEngine.Random.Range(-spread, spread);  // Random adjustment for X axis
                shootDirection.y += UnityEngine.Random.Range(-spread, spread);  // Random adjustment for Y axis

                // Spawn the bullet and apply the modified direction
                GameObject bulletInstance = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(shootDirection));
                int rand = UnityEngine.Random.Range(0, shootingSounds.Count); // Random index
                audioSource.PlayOneShot(shootingSounds[rand]);    // Play the selected sound
                // here bullets damaga and range get decided
                Bullet bulletScript = bulletInstance.GetComponent<Bullet>();

                if (bulletScript != null)
                {
                    bulletScript.damage = damage; //bullets damage is equal to weapon damage
                    bulletScript.range = range2; //bullets range is equal to weapons range
                    bulletScript.isFriendly = isFriendly;
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
            animator.SetBool("Shoot", false);
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

        if (ReloadTime <= 0)
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
            range2 = this.range2,

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
        if (GameManager.Instance.TrySpendScrap(UpgradeDamageCost))
        {
            damage += damageUpgradePoints;
            UpgradeDamageCost += costIncreaseFactor; // Increase the cost for the next upgrade

            // Update UI
            GameManager.Instance.weaponUI.UpdateCurrency(GameManager.Instance.GetScrap());
            GameManager.Instance.weaponUI.UpdateDamageUpgrade(UpgradeDamageCost);

            Debug.Log($"Upgraded damage. New damage: {damage}, New upgrade cost: {UpgradeDamageCost}");
        }
        else
        {
            Debug.LogWarning("Not enough scrap to upgrade damage.");
        }
    }

    public void UpgradeRange()
    {
        if (GameManager.Instance.TrySpendScrap(UpgradeRangeCost))
        {
            range2 += rangeUpgradePoints;
            UpgradeRangeCost += costIncreaseFactor; // Increase the cost for the next upgrade

            // Update UI
            GameManager.Instance.weaponUI.UpdateCurrency(GameManager.Instance.GetScrap());
            GameManager.Instance.weaponUI.UpdateRangeUpgrade(UpgradeRangeCost);

            Debug.Log($"Upgraded range. New range: {range2}, New upgrade cost: {UpgradeRangeCost}");
        }
        else
        {
            Debug.LogWarning("Not enough scrap to upgrade range.");
        }
    }


    //public getters for weapon stats
    public int GetDamage() => damage;
    public float GetRange() => range2;

    public int GetDamageUpgradePoints() => damageUpgradePoints;
    public int GetRangeUpgradePoints() => rangeUpgradePoints;

}

/// <summary>
/// Key Features:
/// 
///     Weapon Customization:
///         - Supports damage, range, and upgrade point adjustments.
///         - Provides unique names for each weapon instance.
/// 
///     Shooting Mechanics:
///         - Handles shooting cooldowns, accuracy, and ammo.
///         - Supports reload mechanics and bullet behavior.
/// 
///     Integration with GameManager and UI:
///         - Uses GameManager to check and deduct scrap for upgrades.
///         - Updates UI elements like currency and upgrade costs.
/// 
///     Debugging Tools:
///         - Outputs weapon-related actions to the console for clarity.
/// </summary>

// How to Use
// 1. Attach this script to your weapon prefab GameObjects.
// 2. Assign bullet prefabs and fire points in the Inspector.
// 3. Call UpgradeDamage() and UpgradeRange() to apply upgrades.
// 4. Use Shoot() to fire bullets and manage cooldowns.
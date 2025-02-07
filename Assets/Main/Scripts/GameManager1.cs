using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using static GameManager;



public class GameManager1 : MonoBehaviour
{
    public static GameManager1 Instance; //Will be used to check how many gamemanager instances there are
    
    public int currency = 1000; //currency for player
    public List<Weapon> purchasedWeapons = new List<Weapon>();//list of purchased weapon
    public List<WeaponPrefabInfo> weaponPrefabs;//list of weapon prefabs
    public WeaponUI weaponUI;

    private void Awake()
    { //makes sure theres only one instance of GameManager1
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("There are multiple GameManager1's");
        }
    }
    private void Start()
    {
        weaponUI.UpdateCurrency(currency); //wowwww
    }

    [System.Serializable]
    public class WeaponPrefabInfo
    {
        public string tag;
        public GameObject weaponPrefab;
    }
    

    public void BuyWeapon(string tag, int cost)
    {
        if (currency >= cost)
        {
            foreach (var weaponInfo in weaponPrefabs)
            {

                if (weaponInfo.tag == tag)
                {
                    GameObject newWeapon = Instantiate(weaponInfo.weaponPrefab);
                    Weapon weaponComponent = newWeapon.GetComponent<Weapon>();

                    if(weaponComponent != null)
                    {
                        purchasedWeapons.Add(weaponComponent);

                        Debug.Log($" Bought weapon; {weaponComponent.GetUniqueName()}");

                        currency -= cost;
                        weaponUI.UpdateCurrency(currency);
                      
                       
                    }
                    return;
                } 
            }
            Debug.LogWarning($"No weapon prefab found with the tag {tag}");
        }
        else
        {
            Debug.LogWarning("Not enough cash");
        }
    }
    public bool SpendCurrency(int amount)
    {
        if(currency >= amount)
        {
            currency -= amount;
            return true;
        }
        else
        {
            return false;
        }
    }

    
    public void AddCurrency(int amount)
    {
        currency += amount;
    }
   
    public ShopUIManager shopUIManager;

    public Weapon GetWeaponFromImage(RectTransform image)
    {
        if (image == null)
        {
            Debug.LogWarning("Image is null.");
            return null;
        }

        // Check if the image's tag matches any weapon prefab
        string imageTag = image.tag; // Ensure your images have proper tags
        foreach (var weaponInfo in weaponPrefabs)
        {
            if (weaponInfo.tag == imageTag)
            {
                GameObject weaponPrefab = weaponInfo.weaponPrefab;
                if (weaponPrefab != null)
                {
                    return weaponPrefab.GetComponent<Weapon>();
                }
            }
        }

        Debug.LogWarning($"No weapon prefab associated with the image tag: {imageTag}");
        return null;
    }
    public void UpgradeCurrentWeaponDamage()
    {
        int currentIndex = shopUIManager.GetCurrentWeaponIndex();
        string[] weaponNames = { "Pistol", "Rifle", "Shotgun" };

        if (currentIndex >= 0 && currentIndex < weaponNames.Length)
        {
            string currentWeaponName = weaponNames[currentIndex];
            WeaponStock weaponStock = GameManager.Instance.GetWeaponByName(currentWeaponName); // Now returns WeaponStock

            if (weaponStock != null)
            {
                weaponStock.UpgradeDamage(); // Use WeaponStock's UpgradeDamage method
                Debug.Log($"Upgraded damage for: {currentWeaponName}");
            }
            else
            {
                Debug.LogWarning($"No weapon found with the name: {currentWeaponName}");
            }
        }
        else
        {
            Debug.LogWarning("Invalid weapon index.");
        }
    }


    public void UpgradeCurrentWeaponRange()
    {
        int currentIndex = shopUIManager.GetCurrentWeaponIndex();
        string[] weaponNames = { "Pistol", "Rifle", "Shotgun" };

        if (currentIndex >= 0 && currentIndex < weaponNames.Length)
        {
            string currentWeaponName = weaponNames[currentIndex];
            WeaponStock weaponStock = GameManager.Instance.GetWeaponByName(currentWeaponName); // Now returns WeaponStock

            if (weaponStock != null)
            {
                weaponStock.UpgradeRange(); // Use WeaponStock's UpgradeRange method
                Debug.Log($"Upgraded range for: {currentWeaponName}");
            }
            else
            {
                Debug.LogWarning($"No weapon found with the name: {currentWeaponName}");
            }
        }
        else
        {
            Debug.LogWarning("Invalid weapon index.");
        }
    }


}
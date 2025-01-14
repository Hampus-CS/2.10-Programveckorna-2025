using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using TMPro;



public class GameManager1 : MonoBehaviour
{
    public static GameManager1 Instance; //Will be used to check how many gamemanager instances there are
    
    public int currency = 1000; //currency for player
    public List<Weapon> purchasedWeapons = new List<Weapon>();//list of purchased weapon
    public List<WeaponPrefabInfo> weaponPrefabs;//list of weapon prefabs
    public UIManager uiManager;

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
        uiManager.UpdateCurrency(currency); //wowwww
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
                        uiManager.UpdateCurrency(currency);
                       
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
        RectTransform currentImage = shopUIManager.GetCurrentImage();
        Weapon weapon = GetWeaponFromImage(currentImage); 

        if (weapon != null)
        {
            weapon.UpgradeDamage(); 
            
        }
        else
        {
            Debug.LogWarning("No weapon with same tag as this image");
        }

    }
    public void UpgradeCurrentWeaponRange()
    {
        RectTransform currentImage = shopUIManager.GetCurrentImage();
        Weapon weapon = GetWeaponFromImage(currentImage); //gets tag of current image

        if (weapon != null)
        {
            weapon.UpgradeRange(); //calls method from weapon script
            
        }
        else
        {
            Debug.LogWarning("No weapon with same tag as this image");
        }

    }
}
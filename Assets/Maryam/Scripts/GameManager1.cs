using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;



public class GameManager1 : MonoBehaviour
{
    public static GameManager1 Instance { get; private set; } //Will be used to check how many gamemanager instances there are
    public GameObject bulletPrefab;
    public int currency = 1000; //currency for player
    public List<Weapon> purchasedWeapons = new List<Weapon>();//list of purchased weapon

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
    public class WeaponPrefabInfo
    {
        public string tag;
        public GameObject weaponPrefab;
    }
    public List<WeaponPrefabInfo> weaponPrefabs;

    public void BuyWeapon(string weaponTag, int cost)
    {
        if (currency >= cost)
        {
            WeaponPrefabInfo weaponInfo = weaponPrefabs.Find(w => w.tag == weaponTag);

            if(weaponInfo== null || weaponInfo.weaponPrefab == null)
            {
                Debug.LogWarning("No weapon prefab found with tag {weaponTag");
                return;
               
            }
            currency -= cost;
            //instantiate Prefab
            GameObject newWeaponObject = Instantiate(weaponInfo.weaponPrefab);

            Weapon newWeapon = newWeaponObject.GetComponent<Weapon>();
            if(newWeapon != null)
            {
                purchasedWeapons.Add(newWeapon);
                Debug.Log($"weapon purchased!");
            }
            else
            {
                Debug.LogWarning("purchased weapon dont have weapon component");

            }
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
    public GameObject GetBulletPrefab()
    {
        return bulletPrefab;
    }
    public ShopUIManager shopUIManager;

    public Weapon GetWeaponFromImage(RectTransform image)
    {
        string imageTag = image.tag;//gets tag from current image and find weapon with same tag
        GameObject weaponObject = GameObject.FindWithTag(imageTag);

        if (weaponObject != null) 
        {
            return weaponObject.GetComponent<Weapon>();
        }
        return null; //no weapon found
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
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class GameManager : MonoBehaviour
{
    [Header("Important Values")]
    [SerializeField] private int scrap = 1000; // Currency for the player
    [SerializeField] private int mp; // Manpower
    [SerializeField] private int cp; // Command power
    [SerializeField] private int rp; // Research points

    [Header("Weapon Management")]
    public List<Weapon> purchasedWeapons = new List<Weapon>(); // List of purchased weapons
    public List<WeaponStock> stockpile = new List<WeaponStock>(); // Unified weapon stockpile
    public List<WeaponPrefabInfo> weaponPrefabs; // List of weapon prefabs

    [Header("References")]
    public static GameManager Instance { get; private set; }

    [Header("WeaponUI")]
    public WeaponUI weaponUI; // Assign this in the Unity Inspector

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveGame(SaveHandler saveHandler)
    {
        SaveHandler.GameState gameState = new()
        {
            Scrap = scrap,
            Stockpile = stockpile.Select(w => new SaveHandler.WeaponData
            {
                Name = w.Name,
                Quantity = w.Quantity,
                Tier = w.Tier
            }).ToList()
        };
        saveHandler.Save(gameState);
    }

    public void LoadGame(SaveHandler saveHandler)
    {
        SaveHandler.GameState gameState = saveHandler.Load();
        if (gameState == null) return;

        scrap = gameState.Scrap;

        stockpile.Clear();
        foreach (var weaponData in gameState.Stockpile)
        {
            stockpile.Add(new WeaponStock(weaponData.Name, weaponData.Quantity, weaponData.Tier));
        }
    }


    // Currency Management
    public bool TrySpendScrap(int amount)
    {
        if (scrap >= amount)
        {
            scrap -= amount;
            return true;
        }
        return false;
    }

    public void AddScrap(int amount)
    {
        scrap += amount;
    }

    public int GetScrap() => scrap;

    // Stockpile Management
    public void AddWeaponToStockpile(string weaponName, int initialQuantity, int tier) // "w" stands for weapon
    {
        var existingWeapon = stockpile.FirstOrDefault(w => w.Name == weaponName);
        if (existingWeapon != null)
        {
            existingWeapon.Quantity += initialQuantity;
        }
        else
        {
            stockpile.Add(new WeaponStock(weaponName, initialQuantity, tier));
        }
    }

    public WeaponStock GetBestWeapon() // "w" stands for weapon
    {
        return stockpile
            .Where(w => w.Quantity > 0 || w.Quantity == -1) // Only available weapons
            .OrderByDescending(w => w.Tier) // Sort by tier
            .FirstOrDefault();
    }

    public void UseWeapon(string weaponName)
    {
        var weapon = stockpile.FirstOrDefault(w => w.Name == weaponName);
        if (weapon != null && (weapon.Quantity > 0 || weapon.Quantity == -1))
        {
            if (weapon.Quantity > 0) weapon.Quantity--;
        }
        else
        {
            Debug.LogWarning($"Weapon {weaponName} is out of stock.");
        }
    }

    // Weapon Management
    [System.Serializable]
    public class WeaponPrefabInfo
    {
        public string tag;
        public GameObject weaponPrefab;
    }

    public void BuyWeapon(string tag, int cost)
    {
        if (TrySpendScrap(cost))
        {
            foreach (var weaponInfo in weaponPrefabs)
            {
                if (weaponInfo.tag == tag)
                {
                    GameObject newWeapon = Instantiate(weaponInfo.weaponPrefab);
                    Weapon weaponComponent = newWeapon.GetComponent<Weapon>();

                    if (weaponComponent != null)
                    {
                        purchasedWeapons.Add(weaponComponent);

                        // Add to stockpile
                        AddWeaponToStockpile(weaponComponent.GetUniqueName(), 1, weaponComponent.GetDamage());
                        Debug.Log($"Bought weapon: {weaponComponent.GetUniqueName()}");
                    }
                    return;
                }
            }
            Debug.LogWarning($"No weapon prefab found with the tag {tag}");
        }
        else
        {
            Debug.LogWarning("Not enough scrap.");
        }
    }

    public void UpgradeWeaponDamage(RectTransform currentImage, int cost)
    {
        Weapon weapon = GetWeaponFromImage(currentImage);

        if (weapon != null && TrySpendScrap(cost))
        {
            weapon.UpgradeDamage();
        }
        else
        {
            Debug.LogWarning("Cannot upgrade weapon damage. Either no weapon found or insufficient scrap.");
        }
    }

    public void UpgradeWeaponRange(RectTransform currentImage, int cost)
    {
        Weapon weapon = GetWeaponFromImage(currentImage);

        if (weapon != null && TrySpendScrap(cost))
        {
            weapon.UpgradeRange();
        }
        else
        {
            Debug.LogWarning("Cannot upgrade weapon range. Either no weapon found or insufficient scrap.");
        }
    }

    public Weapon GetWeaponFromImage(RectTransform image)
    {
        if (image == null)
        {
            Debug.LogWarning("Image is null.");
            return null;
        }

        string imageTag = image.tag;
        foreach (var weaponInfo in weaponPrefabs)
        {
            if (weaponInfo.tag == imageTag)
            {
                GameObject weaponPrefab = weaponInfo.weaponPrefab;
                return weaponPrefab?.GetComponent<Weapon>();
            }
        }

        Debug.LogWarning($"No weapon prefab associated with the image tag: {imageTag}");
        return null;
    }

    [System.Serializable]
    public class WeaponStock
    {
        public string Name;
        public int Quantity; // -1 means infinite
        public int Tier;

        public WeaponStock(string name, int quantity, int tier)
        {
            Name = name;
            Quantity = quantity;
            Tier = tier;
        }
    }
}

/// <summary>
/// Key Features:
/// 
///     Centralized Game State:
///         - Manages resources (scrap, manpower, etc.) and stockpile data.
///         - Handles weapon purchases and upgrades.
/// 
///     Stockpile Management:
///         - Tracks purchased weapons and their availability.
///         - Automatically updates stockpile when weapons are bought or used.
/// 
///     Save and Load Integration:
///         - Saves and restores game state, including stockpile and resources.
///         - Works seamlessly with SaveHandler.
/// 
///     Debugging Tools:
///         - Outputs key actions like purchases and upgrades for better traceability.
/// </summary>

// How to Use
// 1. Attach this script to a GameObject managing the game state.
// 2. Assign references for WeaponUI, weapon prefabs, and stockpile data in the Inspector.
// 3. Use methods like BuyWeapon(), UseWeapon(), and GetBestWeapon() for game logic.
// 4. Call SaveGame() and LoadGame() to persist or retrieve the game state.
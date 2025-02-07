using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Important Values")]
    public int scrap = 1000; // Currency for the player
    [SerializeField] private int rp; // Research
    [SerializeField] private int mp; // Manpower

    [Header("Resource Gain Settings")]
    [SerializeField] private float resourceGainInterval = 1f; // Time interval (seconds)
    [SerializeField] private int scrapGainAmount = 10; // Scrap added per interval
    [SerializeField] private int rpGainAmount = 5; // RP added per interval
    private Coroutine resourceGainCoroutine; // Store reference to the coroutine

    [Header("Weapon Management")]
    public List<WeaponStock> stockpile = new List<WeaponStock>();
    public List<GameObject> weaponPanels;

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

    private void Start()
    {
        // Start the resource gain coroutine
        StartResourceGain();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            BaseSoldier[] soldiers = FindObjectsOfType<BaseSoldier>();

            // Loop through each TroopAI and do something with it
            foreach (BaseSoldier troop in soldiers)
            {
                if (troop.holdPosition)
                {
                    troop.holdPosition = false;
                }
                else
                {
                    troop.holdPosition = true;
                }
            }
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
            stockpile.Add(new WeaponStock(weaponData.Name, weaponData.Quantity));
        }
    }

    public void StartResourceGain()
    {
        if (resourceGainCoroutine == null) // Avoid starting multiple coroutines
        {
            resourceGainCoroutine = StartCoroutine(AddResourcesOverTime());
        }
    }

    private IEnumerator AddResourcesOverTime()
    {
        while (true) // Run indefinitely
        {
            AddScrap(scrapGainAmount); // Add scrap
            AddRP(rpGainAmount);       // Add RP

            Debug.Log($"Added {scrapGainAmount} scrap and {rpGainAmount} RP. Total Scrap: {scrap}, Total RP: {rp}");

            yield return new WaitForSeconds(resourceGainInterval); // Wait for the next interval
        }
    }

    public void AddScrap(int amount)
    {
        scrap += amount;
        // Optionally trigger UI updates or events here
    }

    public void AddRP(int amount)
    {
        rp += amount;
        // Optionally trigger UI updates or events here
    }

    public void AddManpower(int amount)
    {
        mp += amount;
        Debug.Log($"Manpower increased by {amount}. Current manpower: {mp}");
    }

    public void ReduceResearchGain(int amount)
    {
        rp = Mathf.Max(0, rp - amount); // Ensure research points don't go negative
        Debug.Log($"Research points reduced by {amount}. Current research points: {rp}");
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

    public int GetScrap() => scrap;
    public int GetResearchPoints() => rp;
    public int GetManpower() => mp;

    /// <summary>
    /// Gör finare om tid finns:

    private bool isPlayerShootingEnabled = false;
    private float playerWeaponCooldown = 1.0f; // Default cooldown

    public void EnablePlayerShooting()
    {
        if (!isPlayerShootingEnabled)
        {
            isPlayerShootingEnabled = true;
            Debug.Log("Player shooting has been enabled. Rooftop positions are now active.");
        }
        else
        {
            Debug.Log("Player shooting is already enabled.");
        }
    }

    public void ReducePlayerWeaponCooldown(float percentage)
    {
        if (isPlayerShootingEnabled)
        {
            float reductionFactor = 1 - (percentage / 100f);
            playerWeaponCooldown *= reductionFactor;
            Debug.Log($"Player weapon cooldown reduced by {percentage}%. New cooldown: {playerWeaponCooldown}s.");
        }
        else
        {
            Debug.LogWarning("Player shooting is not enabled. Cannot modify weapon cooldown.");
        }
    }

    public float GetPlayerWeaponCooldown() => playerWeaponCooldown;
    
    
    /*
    // Stockpile Management
    public void AddWeaponToStockpile(string weaponName, int initialQuantity, int tier, Sprite icon = null)
    {
        var existingWeapon = stockpile.FirstOrDefault(w => w.Name == weaponName);
        if (existingWeapon != null)
        {
            existingWeapon.Quantity += initialQuantity;
            Debug.Log($"Updated {weaponName} quantity to {existingWeapon.Quantity}");
        }
        else
        {
            stockpile.Add(new WeaponStock(weaponName, initialQuantity, tier, icon));
            Debug.Log($"Added {weaponName} to stockpile with quantity {initialQuantity}");
        }
    }
    */
    /*
    public WeaponStock GetBestWeapon() // "w" stands for weapon
    {
        return stockpile
            .Where(w => w.Quantity > 0 || w.Quantity == -1) // Only available weapons
            .OrderByDescending(w => w.Tier) // Sort by tier
            .FirstOrDefault();
    }
    */

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

    public void BuyWeapon(string weaponName, int cost)
    {
        if (TrySpendScrap(cost))
        {
            var weaponStock = stockpile.FirstOrDefault(w => w.Name == weaponName);

            if (weaponStock != null)
            {
                // If the weapon already exists, just increase the quantity
                weaponStock.Quantity = Mathf.Min(weaponStock.Quantity + 1, 999);
                Debug.Log($"Bought {weaponName}. Quantity now: {weaponStock.Quantity}");
            }
            else
            {
                // No need to create a weapon, just track its quantity
                weaponStock = new WeaponStock(weaponName, 1);
                stockpile.Add(weaponStock);

                Debug.Log($"Bought new weapon: {weaponName}. Quantity now: {weaponStock.Quantity}");
            }
        }
        else
        {
            Debug.LogWarning("Not enough scrap to buy the weapon.");
        }
    }


    private Weapon FindWeaponInSceneOrPrefab(string weaponName)
    {
        // Example of finding it in the scene
        return FindObjectsOfType<Weapon>().FirstOrDefault(w => w.name == weaponName);
    }



    public void UseWeaponForSoldier()
    {
        var weapon = stockpile
            .Where(w => w.Quantity > 0)
            .OrderByDescending(w => w.Quantity)
            .FirstOrDefault();

        if (weapon != null)
        {
            weapon.Quantity--;
            UpdateWeaponUI(weapon.Name);
        }
    }

    private void UpdateWeaponUI(string weaponName)
    {
        var panel = weaponPanels.FirstOrDefault(p => p.name == weaponName);
        if (panel != null)
        {
            var quantityText = panel.GetComponentInChildren<TMP_Text>();
            var weapon = stockpile.FirstOrDefault(w => w.Name == weaponName);
            quantityText.text = weapon != null ? weapon.Quantity.ToString() : "0";
        }
    }

    [System.Serializable]
    public class WeaponStock
    {
        public string Name;
        public int Quantity;
        public int Damage;
        public int Range;

        public WeaponStock(string name, int quantity)
        {
            Name = name;
            Quantity = quantity;
            Damage = 10;  // Default damage
            Range = 5;    // Default range
        }

        public void UpgradeDamage()
        {
            Damage += 5;  // Increase damage by 5 (or any value you prefer)
            Debug.Log($"{Name} damage upgraded to: {Damage}");
        }

        public void UpgradeRange()
        {
            Range += 2;  // Increase range by 2 (adjust as needed)
            Debug.Log($"{Name} range upgraded to: {Range}");
        }
    }

    public WeaponStock GetWeaponByName(string weaponName)
    {
        return stockpile.FirstOrDefault(w => w.Name == weaponName);
    }


    //------------------------------------------------------------------

    /*
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
    */

    /*
    [System.Serializable]
    public class WeaponStock
    {
        public string Name;
        public int Quantity; // -1 means infinite
        public int Tier;
        public Sprite Icon; // Add the icon property

        public WeaponStock(string name, int quantity, int tier, Sprite icon = null)
        {
            Name = name;
            Quantity = quantity;
            Tier = tier;
            Icon = icon;
        }
    }
    */

    //------------------------------------------------------------------
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
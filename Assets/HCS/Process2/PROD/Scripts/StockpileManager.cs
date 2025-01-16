using System.Linq;
using UnityEngine;

public class StockpileManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Buttons buttons;
    /*
    private void Start()
    {
        // Example stockpile setup (if needed during initialization)
        gameManager.AddWeaponToStockpile("Pistol", -1, 0); // Infinite weapon
        gameManager.AddWeaponToStockpile("Shotgun", 5, 2);
        gameManager.AddWeaponToStockpile("Rifle", 3, 3);
    }
    */
    public void BuyWeapon(string weaponName, int cost)
    {
        gameManager.BuyWeapon(weaponName, cost);
        buttons.UpdateStockpileUI();
    }

    public void UseWeapon(string weaponName)
    {
        gameManager.UseWeapon(weaponName);
        buttons.UpdateStockpileUI();
    }

}

/// <summary>
/// Key Features:
/// 
///     Stockpile Management:
///         - Updates the stockpile UI to reflect available weapons.
///         - Handles weapon usage and removal from the stockpile.
/// 
///     GameManager Integration:
///         - Relies on GameManager to manage weapon stockpile data.
///         - Updates UI via Buttons.cs to ensure consistency.
/// 
///     Simplified Weapon Usage:
///         - Enables soldiers to use weapons dynamically.
///         - Tracks weapon availability and infinite-stock weapons.
/// </summary>

// How to Use
// 1. Attach this script to the stockpile panel GameObject.
// 2. Assign references for GameManager and Buttons in the Inspector.
// 3. Use BuyWeapon() and UseWeapon() for stockpile operations.
// 4. Update UI dynamically through the Buttons script.
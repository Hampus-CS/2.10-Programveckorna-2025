using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StockpileManager : MonoBehaviour
{
    public List<WeaponTemp> Weapons { get; private set; } = new List<WeaponTemp>();

    [SerializeField] private GameManager gameManager;

    private void Start()
    {
        // Lägg till exempelvapen
        Weapons.Add(new WeaponTemp("Pistol", 0, -1)); // Oändligt vapen
        Weapons.Add(new WeaponTemp("Shotgun", 2, 5));
        Weapons.Add(new WeaponTemp("Rifle", 3, 2));

        UpdateUI();
    }
    
    public void BuyWeapon(string weaponName, int cost)
    {
        if (gameManager.TrySpendScrap(cost))
        {
            var weapon = Weapons.FirstOrDefault(weapon => weapon.Name == weaponName);
            if (weapon != null)
            {
                weapon.Quantity++;
                UpdateUI();
            }
        }
        else
        {
            Debug.Log("Not enough scrap!");
        }
    }
    
    public void UseWeapon(string weaponName)
    {
        var weapon = Weapons.FirstOrDefault(weapon => weapon.Name == weaponName);
        if (weapon != null)
        {
            if (weapon.Quantity > 0 || weapon.Quantity == -1) // Kontrollera att vapnet inte är slut
            {
                if (weapon.Quantity > 0) weapon.Quantity--;
                Debug.Log($"Used {weaponName}. Remaining: {weapon.Quantity}");
                UpdateUI();
            }
            else
            {
                Debug.Log("Out of stock!");
            }
        }
    }

    private void UpdateUI()
    {
        // Sortera vapnen efter Tier
        Weapons = Weapons.OrderByDescending(weapon => weapon.Tier).ToList();

        // Uppdatera UI med vapnen (implementera ditt UI här)
        foreach (var weapon in Weapons)
        {
            Debug.Log($"{weapon.Name} - Tier: {weapon.Tier}, Quantity: {(weapon.Quantity == -1 ? "∞" : weapon.Quantity.ToString())}");
        }
    }
}

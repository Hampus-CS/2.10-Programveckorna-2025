using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponUI : MonoBehaviour
{
    public TMP_Text currencyText;
    public TMP_Text UpgradeDamageCostText;
    public TMP_Text UpgradeRangeCostText;
    public TMP_Text WeaponCostText;

    private void Start()
    {
        UpdateDamageUpgrade(50);
        UpdateRangeUpgrade(50);
        UpdateWeaponCost(50);
    }

    public void UpdateCurrency(int currency)
    {
        currencyText.text = $"Currency: {currency}";
    }
    public void UpdateDamageUpgrade(int UpgradeDamageCost)
    {
        UpgradeDamageCostText.text = $"Upgrade Cost: {UpgradeDamageCost}";
    }
    public void UpdateRangeUpgrade(int UpgradeRangeCost)
    {
        UpgradeRangeCostText.text = $"Upgrade Cost: {UpgradeRangeCost}";
    }
    public void UpdateWeaponCost(int WeaponCost)
    {
        WeaponCostText.text = $"Weapon Cost: {WeaponCost}";
    }
}

/// <summary>
/// Key Features:
/// 
///     UI Updates:
///         - Dynamically updates text fields for currency and costs.
///         - Displays current weapon costs and upgrade prices.
/// 
///     Seamless Integration:
///         - Called by Weapon and GameManager scripts to reflect game state.
///         - Ensures UI consistency after purchases or upgrades.
/// 
///     Simple API:
///         - Provides straightforward methods to update individual UI elements.
/// </summary>

// How to Use
// 1. Attach this script to a GameObject managing weapon-related UI.
// 2. Assign text fields for currency, costs, and upgrades in the Inspector.
// 3. Call UpdateCurrency(), UpdateDamageUpgrade(), or UpdateRangeUpgrade() as needed.
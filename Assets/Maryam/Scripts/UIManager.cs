using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TMP_Text currencyText;
    public TMP_Text UpgradeDamageCostText;
    public TMP_Text UpgradeRangeCostText;
    public TMP_Text WeaponCostText;

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

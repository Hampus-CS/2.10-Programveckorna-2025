using UnityEngine;

public class WeaponTemp : MonoBehaviour
{
    public string Name { get; private set; }
    public int Tier { get; private set; }
    public int Quantity { get; set; } // -1 betyder oändligt

    public WeaponTemp(string name, int tier, int quantity)
    {
        Name = name;
        Tier = tier;
        Quantity = quantity;
    }
}

using UnityEngine;



public class GameManager1 : MonoBehaviour
{
    [System.Serializable]
    public class Weapon
    {
        public GameObject bulletPrefab; // Bullet prefab associated with this weapon
        public int damage = 10;
        public int range = 5;

        public Weapon(GameObject bulletPrefab, int damage, int range)
        {
            this.bulletPrefab = bulletPrefab;
            this.damage = damage;
            this.range = range;
        }
    }

    public Weapon[] weapons; // Array of weapons

    public ShopUIManager shopUIManager; // Reference to ShopUIManager

    // Method to find a weapon based on the tag of the image
    public Weapon GetWeaponFromTag(RectTransform image)
    {
        string imageTag = image.tag; // Get the tag of the current image
        foreach (var weapon in weapons)
        {
            if (weapon.bulletPrefab != null && weapon.bulletPrefab.CompareTag(imageTag))
            {
                return weapon;
            }
        }

        return null; // No matching weapon found
    }

    // Method to upgrade the damage of the weapon currently in the center of the shop
    public void UpgradeCurrentWeaponDamage(int amount)
    {
        RectTransform currentImage = shopUIManager.GetCurrentImage();
        Weapon weapon = GetWeaponFromTag(currentImage);

        if (weapon != null)
        {
            // Upgrade the weapon's damage directly
            weapon.damage += amount;
            Debug.Log("Weapon's damage increased by " + amount + ". New damage: " + weapon.damage);
        }
        else
        {
            Debug.LogWarning("No weapon associated with the current image.");
        }
    }

    // Method to upgrade the range of the weapon currently in the center of the shop
    public void UpgradeCurrentWeaponRange(int amount)
    {
        RectTransform currentImage = shopUIManager.GetCurrentImage();
        Weapon weapon = GetWeaponFromTag(currentImage);

        if (weapon != null)
        {
            // Upgrade the weapon's range directly
            weapon.range += amount;
            Debug.Log("Weapon's range increased by " + amount + ". New range: " + weapon.range);
        }
        else
        {
            Debug.LogWarning("No weapon associated with the current image.");
        }
    }
}
using UnityEngine;



public class GameManager1 : MonoBehaviour
{
    [System.Serializable]
    public class Weapon
    {
        public GameObject bulletPrefab; // bulletprefab from weapon script
        public int damage = 10;
        public int range = 5;

        public Weapon(GameObject bulletPrefab, int damage, int range)
        {
            this.bulletPrefab = bulletPrefab;
            this.damage = damage;
            this.range = range;

        }
    }
    public Weapon[] weapons;

    public ShopUIManager shopUIManager;
    public Weapon GetWeaponFromTag(RectTransform image)
    {
        string imageTag = image.tag; 
        foreach(var weapon in weapons)
        {
            if (weapon.bulletPrefab != null && weapon.bulletPrefab.CompareTag(imageTag))
            {
                return weapon;
            }

        }
        return null;
    }
    public void UpgradeCurrentWeaponDamage(int amount)
    {
        RectTransform currentImage = shopUIManager.GetCurrentImage();
        Weapon weapon = GetWeaponFromTag(currentImage);

        if (weapon != null)
        {
            weapon.range += amount;
            Debug.Log("Weapons range increase by " + amount + ". New range" + weapon.range);
        }
        else
        {
            Debug.LogWarning("No weapon with same tag as this image");
        }

    }
}
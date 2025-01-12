using UnityEngine;



public class GameManager1 : MonoBehaviour
{
    public static GameManager1 Instance { get; private set; } //Will be used to check how many gamemanager instances there are
    public GameObject bulletPrefab;

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
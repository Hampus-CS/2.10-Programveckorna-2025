using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class ShopUIManager : MonoBehaviour
{
    //script to manage shop UI stuff
    public GameObject shopButton;
    public GameObject shopPanel;
    public RectTransform[] images;//assign the images recttransform/hitbox in inspector
    private float moveDistance = 1000f; //how far the images will move 
    public float moveDuration = 0.5f; //how long it will take the images
    private Vector3[] initialPositions;
    private int currentIndex = 0; //tracks what image is being wieved so u cant go to far right/left
    public int weaponCost = 20;
    public Buttons buttons;

    private string[] weaponNames = { "Pistol", "Rifle", "Shotgun" };

    public TMP_Text weaponDisplayText; // Displays current weapon
    public TMP_Text[] weaponCountTexts; // Assign Weapon1, Weapon2, Weapon3 TMP_Texts


    public void Start()
    {
        //here the position of these images are saved
        initialPositions = new Vector3[images.Length];
        for (int i = 0; i < images.Length; i++)
        {
            initialPositions[i] = images[i].anchoredPosition;
        }

        UpdateWeaponDisplay();
    }

    public void BuyWeaponButton()
    {
        string[] weaponNames = { "Pistol", "Rifle", "Shotgun" };
        int currentIndex = GetCurrentWeaponIndex();
        string weaponName = weaponNames[currentIndex];

        GameManager.Instance.BuyWeapon(weaponName, 50); // Purchase the weapon

        UpdateWeaponUI(currentIndex); // Ensure this updates the UI
    }


    public void BuyWeaponButton(string weaponName, int cost)
    {
        if (string.IsNullOrEmpty(weaponName))
        {
            Debug.LogWarning("Weapon name is missing!");
            return;
        }

        GameManager.Instance.BuyWeapon(weaponName, cost);
        Debug.Log($"Bought weapon: {weaponName} for {cost} scrap.");
    }


    public void MoveRight()
    {
        if (currentIndex >= images.Length - 1)
        {
            Debug.Log("Can't scroll further right.");
            return;
        }

        currentIndex++;
        UpdateWeaponDisplay();

        for (int i = 0; i < images.Length; i++)
        {
            Vector3 targetPosition = initialPositions[i] - Vector3.right * moveDistance * currentIndex;
            StartCoroutine(MoveImage(images[i], targetPosition));
        }
    }

    public void MoveLeft()
    {
        if (currentIndex <= 0)
        {
            Debug.Log("Can't scroll further left.");
            return;
        }

        currentIndex--;
        UpdateWeaponDisplay();

        for (int i = 0; i < images.Length; i++)
        {
            Vector3 targetPosition = initialPositions[i] - Vector3.right * moveDistance * currentIndex;
            StartCoroutine(MoveImage(images[i], targetPosition));
        }
    }

    private IEnumerator MoveImage(RectTransform image, Vector3 targetPosition)
    {
        Vector3 startPosition = image.anchoredPosition;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            image.anchoredPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        image.anchoredPosition = targetPosition;
    }

    private void UpdateWeaponDisplay()
    {
        if (weaponDisplayText != null)
        {
            weaponDisplayText.text = weaponNames[currentIndex];
        }

        Debug.Log($"Current weapon: {weaponNames[currentIndex]} (Index: {currentIndex})");
    }

    public void UpdateWeaponUI(int weaponIndex)
    {
        string[] weaponNames = { "Pistol", "Rifle", "Shotgun" };
        string weaponName = weaponNames[weaponIndex];

        var weaponStock = GameManager.Instance.GetWeaponByName(weaponName);

        if (weaponStock != null && weaponCountTexts.Length > weaponIndex)
        {
            weaponCountTexts[weaponIndex].text = weaponStock.Quantity.ToString(); // Update the quantity
        }
    }

    public int GetCurrentWeaponIndex()
    {
        return currentIndex; // Returns the currently selected weapon index
    }


    /*

    public RectTransform GetCurrentImage()//here the current image on screen will be located to uppgrade specific weapons in the GameManager script
    {
        return images[currentIndex];
    }

    public void UpgradeCurrentWeaponDamage()
    {

        RectTransform currentImage = GetCurrentImage();
        Weapon weapon = GameManager.Instance.GetWeaponFromImage(currentImage);
        if (weapon != null)
        {
            weapon.UpgradeDamage();
        }
    }

    public void UpgradeCurrentWeaponRange()
    {
        RectTransform currentImage = GetCurrentImage();
        Weapon weapon = GameManager.Instance.GetWeaponFromImage(currentImage);
        if (weapon != null)
        {
            weapon.UpgradeRange();
        }
    }
    */
    public void ReduceWeaponCostByPercentage(float percentage)
    {
        if (percentage <= 0 || percentage > 100)
        {
            Debug.LogWarning("Invalid percentage for cost reduction.");
            return;
        }

        float reductionFactor = 1 - (percentage / 100f);
        weaponCost = Mathf.RoundToInt(weaponCost * reductionFactor);
        Debug.Log($"Weapon cost reduced to {weaponCost}. Reduction: {percentage}%");
    }


}

/// <summary>
/// Key Features:
/// 
///     Weapon Purchasing:
///         - Allows players to weweapons through the shop UI.
///         - Automatically deducts scrap and updates the stockpile.
/// 
///     Weapon Navigation:
///         - Scrolls through available weapons with smooth animations.
///         - Highlights the currently selected weapon.
/// 
///     Integration with GameManager:
///         - Fetches weapon details and calls relevant GameManager methods.
///         - Ensures consistent data flow between UI and game state.
/// </summary>

// How to Use
// 1. Attach this script to your shop panel GameObject.
// 2. Assign weapon images, shop buttons, and GameManager in the Inspector.
// 3. Use OpenShop() and CloseShop() to toggle the shop UI visibility.
// 4. Call BuyWeaponButton() to handle weapon purchases.
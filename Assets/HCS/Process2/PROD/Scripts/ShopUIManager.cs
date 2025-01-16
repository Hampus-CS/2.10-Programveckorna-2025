using UnityEngine;
using UnityEngine.UI;

public class ShopUIManager : MonoBehaviour
{
    //script to manage shop UI stuff
    public GameObject shopButton;
    public GameObject shopPanel;
    public RectTransform[] images;//assign the images recttransform/hitbox in inspector
    public float moveDistance = 200f; //how far the images will move 
    public float moveDuration = 0.5f; //how long it will take the images
    private Vector3[] initialPositions;
    private int currentIndex = 0; //tracks what image is being wieved so u cant go to far right/left
    public int weaponCost = 20;

    public void Start()
    {
        //here the position of these images are saved
        initialPositions = new Vector3[images.Length];
        for (int i = 0; i < images.Length; i++)
        {
            initialPositions[i] = images[i].anchoredPosition;
        }
    }
   
    public void BuyWeaponButton() //the button used to by weapons, checks what image is selected for the moment
    {
        RectTransform currentImage = GetCurrentImage();
        Debug.Log($"Current image: {currentImage.name}");

        if(currentImage == null)
        {
            Debug.LogWarning("No weapon image selected rn");
            return;
        }
        Weapon weapon = GameManager1.Instance.GetWeaponFromImage(currentImage);
        if (weapon == null) {
            Debug.LogWarning("No weapon connected to current image");
            return;
        }
        GameManager1.Instance.BuyWeapon(weapon.tag, weaponCost); //if current image has tag the BuyWeapon void is called from gamemanager1
    }
    
    public void OpenShop() {
        shopButton.SetActive(false); //Hide shop button when shop is open
        shopPanel.SetActive(true); //SHow pannel when shop is open
    
    }

   

    public void MoveRight()//if this void is called the images will move right
    {
        if(currentIndex >= images.Length -1)//chechs if index has reached its maximum value to stop from scrolling
        {
            Debug.Log("Cant scroll further right");
            return;
        }
        currentIndex++;
        for(int i = 0; i < images.Length; i++){

            Vector3 targetPosition = initialPositions[i] - Vector3.right * moveDistance * currentIndex;
            StartCoroutine(MoveImage(images[i], targetPosition));
           
        }
    }
  
    public void MoveLeft()//if this void is called the images will move left
    {
        if (currentIndex <= 0)//cant scroll to far left
        {
            Debug.Log("Cant scroll further left");
            return;
        }
        currentIndex--;

        for (int i = 0; i < images.Length; i++)
        {

            Vector3 targetPosition = initialPositions[i] - Vector3.right * moveDistance * currentIndex;
            StartCoroutine(MoveImage(images[i], targetPosition));
            
        }
    }
    private System.Collections.IEnumerator MoveImage(RectTransform image, Vector3 targetposition)
    {
        Vector3 startPosition = image.anchoredPosition;
        float elapsedTime = 0f;
        while (elapsedTime < moveDuration)
        {
            image.anchoredPosition = Vector3.Lerp(startPosition, targetposition, elapsedTime / moveDuration); //image moves from start position to target position for its set duration
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        image.anchoredPosition = targetposition;
    }
    public RectTransform GetCurrentImage()//here the current image on screen will be located to uppgrade specific weapons in the GameManager1 script
    {
        return images[currentIndex];
    }
    public void CloseShop()
    {
        shopButton.SetActive(true); // Show shop buton when shop closed
        shopPanel.SetActive(false); //hide panel
    }
    public void UpgradeCurrentWeaponDamage()
    {
        RectTransform currentImage = GetCurrentImage();
        Weapon weapon = GameManager1.Instance.GetWeaponFromImage(currentImage);
        if (weapon != null)
        {
            weapon.UpgradeDamage();

        }
    }
    public void UpgradeCurrentWeaponRange()
    {
        RectTransform currentImage = GetCurrentImage();
        Weapon weapon = GameManager1.Instance.GetWeaponFromImage(currentImage);
        if (weapon != null)
        {
            weapon.UpgradeRange();
        }
    }

}

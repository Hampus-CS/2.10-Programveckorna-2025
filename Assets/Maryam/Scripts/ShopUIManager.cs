using UnityEngine;

public class ShopUIManager : MonoBehaviour
{
    //script to manage shop UI stuff
    public GameObject shopButton;
    public GameObject shopPanel;

    public void OpenShop() {
        shopButton.SetActive(false); //Hide shop button when shop is open
        shopPanel.SetActive(true); //SHow pannel when shop is open
    
    }
    public void CloseShop()
    {
        shopButton.SetActive(true); // Show shop buton when shop closed
        shopPanel.SetActive(false); //hide panel
    }
}

using UnityEngine;

public class PlayerCurrency : MonoBehaviour
{
    public int currentCurrency = 100;

    public bool SpendCurrency(int amount) { 
      if(currentCurrency >= amount)
      {
            currentCurrency -= amount;
            return true; //Purchased succesfully
      }
        else
        {
            Debug.Log("Not enough cash");
            return false; //NOT puchased succesfully not enough cash
        }
    }
    public void AddCurrency(int amount) {
        currentCurrency += amount;
    
    }
}

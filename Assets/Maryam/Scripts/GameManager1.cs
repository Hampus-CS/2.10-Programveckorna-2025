using UnityEngine;

public class GameManager1 : MonoBehaviour
{
    

    [System.Serializable]
    public class Weapon 
    {
        public GameObject DamageButton;
        public GameObject RangeButton;
        public GameObject bulletPrefab;
        public int damage = 10;
        public int range = 5;

        public Weapon(GameObject bullet, int damage, int range)
        {
            this.bulletPrefab = bullet;
            this.damage = damage;
            this.range = range;
        }


    }
    
    
}

using UnityEngine;

public class GameManager1 : MonoBehaviour
{
    

     [System.Serializable]
    public class Weapon
    {
        
       [SerializeField] public GameObject bulletPrefab; //prefab for bullets
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

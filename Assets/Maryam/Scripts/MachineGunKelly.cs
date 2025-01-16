using Unity.VisualScripting;
using UnityEngine;

public class MachineGunKelly : MonoBehaviour //går inte att ändra namn tyvärr :/
{
    public GameObject bulletPrefab; //prefab for MG bullets
    public Transform firePoint; //where bullets coma from
    public float bulletSpeed = 100f; //how fast the bullet is
    public float accuracy = 5f; //the offset for accuracy
    private int maxammo = 40; //maxammo for MG
    private int currentAmmo;
    private float coolDown = 120f; //100 second cooldown to start shooting again
    private float coolDownTimer = 0f;
    private bool isCoolingDown = false;
    float WaitBetweenShots = 0.3f; //the time the gun will wait in between each shot
    float lastShot = 0f;

    private void Start()
    {
        currentAmmo = maxammo; //sets current ammo to maxammo
    }
    void Update()
    {
        if (isCoolingDown)
        {
            coolDownTimer -= Time.deltaTime;
            if (coolDownTimer <= 0f)
            {
                Reload();
            }
        }
        if (!isCoolingDown)
        {
            Shoot();
        }
    }
    

    public void Shoot()
    {
        if (Time.time - lastShot >= WaitBetweenShots)
        {
                if (currentAmmo > 0)
                {

                    GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation); //instantiate bullet at firepoint
                    float randomAngle = Random.Range(-accuracy, accuracy); //accuracy
                    Vector3 direction = Quaternion.Euler(0, randomAngle, 0) * firePoint.forward;



                    Rigidbody bulletRB = bullet.GetComponent<Rigidbody>();
                    if (bulletRB != null)
                    {
                        bulletRB.linearVelocity = direction * bulletSpeed;
                    }
                    currentAmmo--;
                }
                if (currentAmmo <= 0) //start cooldown when no ammo
                {
                    StartCoolDown();
                }

                lastShot = Time.time;
        }
      
    }
    public void StartCoolDown()
    {
        isCoolingDown=true;
        coolDownTimer = coolDown;

    }
    private void Reload()
    {
        isCoolingDown = false;
        currentAmmo = maxammo;//reload
    }
  
}

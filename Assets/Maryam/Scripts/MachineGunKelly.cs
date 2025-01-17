using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MachineGunKelly : MonoBehaviour //går inte att ändra namn tyvärr :/
{
    public GameObject bulletPrefab; //prefab for MG bullets
    public Transform firePoint; //where bullets coma from
    public float bulletSpeed = 5f; //how fast the bullet is
    public float accuracy = 0.4f; //the offset for accuracy
    private int maxammo = 40; //maxammo for MG
    private int currentAmmo;
    private float coolDown = 10f; //100 second cooldown to start shooting again
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
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 10f; // Set the fixed distance from the camera (adjust this value based on the depth of your scene)
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        // Calculate direction to the mouse in world space
        Vector3 directionToMouse = worldPosition - transform.position;
        directionToMouse.z = 0f; // Ensure no change in the z-axis (if you're working in 2D view or want to restrict rotation to a plane)

        // Rotate towards the mouse position
        if (directionToMouse != Vector3.zero) // Make sure the direction is valid
        {
            Quaternion lookRotation = Quaternion.LookRotation(Vector3.forward, directionToMouse); // Rotate only on the XY plane
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 100f * Time.deltaTime); // Smooth rotation
        }

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
                float spread = 1f / accuracy;

                Vector3 shootDirection = (Input.mousePosition - firePoint.position).normalized;
                // Randomly adjust direction based on accuracy factor
                shootDirection.x += UnityEngine.Random.Range(-spread, spread);  // Random adjustment for X axis
                shootDirection.y += UnityEngine.Random.Range(-spread, spread);  // Random adjustment for Y axis

                // Spawn the bullet and apply the modified direction
                GameObject bulletInstance = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(shootDirection));
                Bullet bullet = bulletInstance.GetComponent<Bullet>();
                bullet.machingunBUllet = true;

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
        isCoolingDown = true;
        coolDownTimer = coolDown;
    }
    private void Reload()
    {
        isCoolingDown = false;
        currentAmmo = maxammo;//reload
    }

    private IEnumerator reload()
    {
        yield return new WaitForSeconds(coolDownTimer);
        
    }
}
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
    private bool isCoolingDown = false;
    float WaitBetweenShots = 0.3f; //the time the gun will wait in between each shot
    float lastShot = 0f;
    private bool uppgrade = false;



    public Transform cannonBarrel; // The part of the cannon that rotates
    public Camera playerCamera; // The camera used for aiming
    public float aimSpeed = 5f; // Speed of aiming

    void Update()
    {
        AimCannon();
        if (Input.GetMouseButton(0))
        {
            FireCannon();
        }
    }

    void AimCannon()
    {
        // Create a ray from the camera through the mouse position
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);
        int layerMask = ~LayerMask.GetMask("IgnoreCannon");

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask, QueryTriggerInteraction.Ignore))
        {
                // Get the direction to the hit point
                Vector3 aimDirection = (hit.point - cannonBarrel.position).normalized;

                // Rotate the cannon to aim smoothly
                Quaternion targetRotation = Quaternion.LookRotation(aimDirection);
                cannonBarrel.rotation = Quaternion.Slerp(cannonBarrel.rotation, targetRotation, Time.deltaTime * aimSpeed);
        }
    }

    private void FireCannon()
    {
        if (!isCoolingDown)
        {
            StartCoroutine(cooldown());
            GameObject bullet = Instantiate(bulletPrefab, cannonBarrel.position, cannonBarrel.rotation);
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            bulletScript.machingunBUllet = true;

            if (uppgrade)
            {
                bulletScript.damage = 2;
                bulletScript.range = 100;
                bulletScript.speed = 45;
            }
            else
            {
                bulletScript.damage = 1;
                bulletScript.range = 100;
                bulletScript.speed = 35;
            }

            bullet.transform.localScale = new Vector3(6, 1, 2);
        }
    }

    private IEnumerator cooldown()
    {
        isCoolingDown = true;
        yield return new WaitForSeconds(0.2f);
        isCoolingDown = false;
    }
}
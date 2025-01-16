using UnityEngine;

public class MachineGunKelly : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 100f;
    public float accuracy = 5f;
    void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            Shoot();
        }
    }

    public void Shoot()
    {

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        float randomAngle = Random.Range(-accuracy, accuracy);
        Vector3 direction = Quaternion.Euler(0, randomAngle, 0) * firePoint.forward;

        Rigidbody bulletRB = bullet.GetComponent<Rigidbody>();
        if(bulletRB != null)
        {
            bulletRB.linearVelocity = direction * bulletSpeed;  
        }

    }
}

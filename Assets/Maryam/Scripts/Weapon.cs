using System;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Weapon weapon;
    public Transform firePoint;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }
    void Shoot()
    {

    }
}




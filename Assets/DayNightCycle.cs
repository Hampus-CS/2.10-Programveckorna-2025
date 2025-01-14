using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public Light directionalLight; 
    public float dayDuration = 120f;

    private float rotationSpeed;

    void Start()
    {
        rotationSpeed = 360f / dayDuration;
    }

    void Update()
    {
        directionalLight.transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);
    }
}

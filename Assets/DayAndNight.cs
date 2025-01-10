using UnityEngine;

public class DayAndNightCycle : MonoBehaviour
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

        if (directionalLight.transform.eulerAngles.x > 360f)
        {
            directionalLight.transform.eulerAngles = new Vector3(0f, directionalLight.transform.eulerAngles.y, directionalLight.transform.eulerAngles.z);
        }
    }
}

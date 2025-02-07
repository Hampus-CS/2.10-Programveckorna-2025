using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public float panSpeed = 20f;
    public float panLimit = 50f;
    public float scrollSpeed = 10;

    private void Start()
    {
        transform.position = new Vector3(100, 30, 40);
    }

    void Update()
    {
        Vector3 pos = transform.position;

        if (Input.GetKey(KeyCode.A))
        {
            pos.z -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            pos.z += panSpeed * Time.deltaTime;
        }
        
        pos.z = Mathf.Clamp(pos.z, 25, 275);
        pos.y = Mathf.Clamp(pos.y, 25, 35);
        pos.x = Mathf.Clamp(pos.x, 102, 110);

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        pos.y -= scroll * scrollSpeed * 100f * Time.deltaTime;
        pos.x -= scroll * scrollSpeed * 80 * Time.deltaTime;

        transform.position = pos;
    }
}

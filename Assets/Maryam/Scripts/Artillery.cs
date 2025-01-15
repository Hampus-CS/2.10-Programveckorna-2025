using UnityEngine;

public class Artillery : MonoBehaviour
{
    public float objectHeight = 0.01f; // Fixed Y position in world space
    public GameObject ArtilleryMark;
    public GameObject Nuke;
    private void Start()
    {
        ArtilleryMark.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ArtilleryMark.SetActive(false);
        }
        // Get the mouse position in screen space
        Vector3 mouseScreenPosition = Input.mousePosition;
        float screenDepth = Mathf.Abs(Camera.main.transform.position.z);

        // Convert the mouse position to world space
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(
            new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, screenDepth));

        // Set the fixed Y position
        mouseWorldPosition.y = objectHeight;

        // Move the object to the mouse's world position
        ArtilleryMark.transform.position = mouseWorldPosition;
    }
    void SpawnArtillery()
    {
        
    }
   public void ShowArtilleryMark()
    {
            ArtilleryMark.SetActive(true);
            Debug.Log("Show mark");
    }
}
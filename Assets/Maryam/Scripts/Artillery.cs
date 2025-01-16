using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;

public class Artillery : MonoBehaviour
{
    public float GroundDistance = 20; // How high up the nuke will spawn
    public GameObject ArtilleryMark;
    public GameObject artilleryPrefab;
    public float NukeImpactSpeed = 5f; //just how long itll take the nuke to hit the ground
    public float SpawnHeight = 20f; //WHERE IT SPAWN
    public int ArtilleryCost = 200; //how much it cost

    private void Start()
    {

        ArtilleryMark.SetActive(false);
    }

    void Update()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;

        float screenDepth = Mathf.Abs(Camera.main.transform.position.z);

        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(
        new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, screenDepth));

        mouseWorldPosition.y = GroundDistance;
        transform.position = mouseWorldPosition;

        if (ArtilleryMark.activeSelf) //if mark is showing u can nuke
        {
            ArtilleryMark.transform.position = mouseWorldPosition;
            if (Input.GetMouseButtonDown(0))
            {
                SpawnArtillery(mouseWorldPosition);
            }

        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {

            ArtilleryMark.SetActive(false);
        }

    }
    public void SpawnArtillery(Vector3 target)
    {
        if (GameManager1.Instance.currency >= ArtilleryCost)
        {
            Vector3 impactZone = new Vector3(target.x, target.y + SpawnHeight, target.z);
            GameObject Artillery = Instantiate(artilleryPrefab, impactZone, Quaternion.identity);

            StartCoroutine(MoveArtillery(Artillery, target));
            GameManager1.Instance.currency -= ArtilleryCost;
            GameManager1.Instance.uiManager.UpdateCurrency(GameManager1.Instance.currency);

        }
        else
        {
            Debug.LogWarning("Not enough for this bomb");
        }
    }
    System.Collections.IEnumerator MoveArtillery(GameObject Artillery, Vector3 target)
    {
        while (Artillery != null && Vector3.Distance(Artillery.transform.position, target) > 0.1f)
        {
            Artillery.transform.position = Vector3.MoveTowards(
                Artillery.transform.position,
                target,
                NukeImpactSpeed * Time.deltaTime
                );
            yield return null;
        }
        if (Artillery != null)
        {
            MissileBehavoiur missileBehaviour = Artillery.GetComponent<MissileBehavoiur>();
            if (missileBehaviour != null)
            {

                missileBehaviour.impactZone();
            }
            Destroy(Artillery);
            Debug.Log("Impact");

        }

    }
    public void ShowArtilleryMark()
    {
        ArtilleryMark.SetActive(true);
        Debug.Log("Show mark");

    }

}
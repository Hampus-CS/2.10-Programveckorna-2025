using UnityEngine;

public class GameManager : MonoBehaviour
{
    private TroopNavigation troopNavigation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        troopNavigation = FindObjectOfType<TroopNavigation>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            troopNavigation.forwards = true;
            troopNavigation.holdPosition = false;
        }
        if (Input.GetKey(KeyCode.S))
        {
            troopNavigation.forwards = false;
            troopNavigation.holdPosition = true;
        }
    }
}

using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private TroopNavigation troopNavigation;
    private List<GameObject> friendlyTroops = new List<GameObject>();

    private bool hold = false;
    private bool forward = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        troopNavigation = FindObjectOfType<TroopNavigation>();
    }

    // Update is called once per frame
    void Update()
    {
        friendlyTroops.Clear();
        GameObject[] findTroops = GameObject.FindGameObjectsWithTag("FriendlyTroop");
        foreach (GameObject troop in findTroops)
        {
            friendlyTroops.Add(troop);
            troopNavigation = troop.GetComponent<TroopNavigation>();

            if (hold)
            {
                troopNavigation.holdPosition = true;
            }
            else if (forward)
            {
                troopNavigation.moveForwards = true;
            }
           
        }

        if (Input.GetKey(KeyCode.W))
        {
            forward = true;
            hold = false;
        }
        if (Input.GetKey(KeyCode.S))
        {
            forward = false;
            hold = true;
        }
    }
}

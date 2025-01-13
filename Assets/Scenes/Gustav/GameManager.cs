using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [Header("Important Values")]
    [SerializeField] private int commandPower;
    [SerializeField] private int manPower;
    [SerializeField] private int scrap = 1000;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool TrySpendScrap(int amount)
    {
        if (scrap >= amount)
        {
            scrap -= amount;
            Debug.Log($"Scrap spent: {amount}. Remaining scrap: {scrap}");
            return true;
        }
        Debug.Log("Not enough scrap.");
        return false;
    }

    public int GetScrap() => scrap; // Getter om du vill visa scrap i UI, borde fungera, har inte testat! Halft hjärndöd när jag la till detta. ;)


    //////

    private TroopNavigation troopNavigation;
    private List<GameObject> friendlyTroops = new List<GameObject>();

    private bool hold = false;
    private bool forward = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
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
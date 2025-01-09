using UnityEngine;

public class GameManager : MonoBehaviour
{

    [Header("Important Values")]
    [SerializeField] private int commandPower;
    [SerializeField] private int manPower;
    [SerializeField] private int scrap = 1000;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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


}

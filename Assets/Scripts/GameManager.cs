using UnityEngine;

public class GameManager : MonoBehaviour
{

    [Header("Important Values")]
    [SerializeField] private int commandPower;
    [SerializeField] private int manPower;
    [SerializeField] private int scrap;

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
}

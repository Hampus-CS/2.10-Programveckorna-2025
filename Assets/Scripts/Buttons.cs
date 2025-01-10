using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class Buttons : MonoBehaviour
{
    [Header("All Menus")]
    public List<GameObject> menus = new();

    [Header("Skill tree")]
    private Dictionary<int, ISkillInfo> skillInfoHandlers = new(); // Hanterar skills dynamiskt

    [Header("Stockpile UI")]
    [SerializeField] private GameObject buttonTemplate; // Template for weapon buttons
    [SerializeField] private Transform content; // Scroll View content
    [SerializeField] private Text scrapText; // Text for Scrap
    [SerializeField] private StockpileManager stockpileManager; // Link to stockpile manager
    [SerializeField] private GameManager gameManager; // Management of Scrap
    private List<GameObject> stockpileButtons = new();

    private void Start()
    {
        Time.timeScale = 0;

        // Viktiga MenyinstÃ¤llningar
        menus[0].SetActive(false); // game
        menus[1].SetActive(false); // pause
        menus[2].SetActive(true); // start
        menus[3].SetActive(false); // settings
        menus[4].SetActive(false); // skill tree

        // Registrera skills
        skillInfoHandlers[1] = new SkillInfo(5); // Skill 1 Info (index 5 i menus)
        skillInfoHandlers[2] = new SkillInfo(6); // Skill 2 Info (index 6 i menus)
        skillInfoHandlers[3] = new SkillInfo(7); // Skill 3 Info (index 7 i menus)
        skillInfoHandlers[4] = new SkillInfo(8); // Skill 4 Info (index 8 i menus)
        skillInfoHandlers[5] = new SkillInfo(9); // Skill 5 Info (index 9 i menus)
        skillInfoHandlers[6] = new SkillInfo(10); // Skill 6 Info (index 10 i menus)

        /*
         0 = game
         1 = pause
         2 = start
         3 = settings
         4 = skill tree
         5 = skill 1 info
         6 = skill 2 info
        */

        UpdateStockpileUI();
    }

    /// <summary>
    /// Main
    /// </summary>

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menus[1].activeSelf == false)
            {
                Pause();
            }
            else if (menus[1].activeSelf == true)
            {
                Resume();
            }
        }
    }

    public void Play()
    {
        menus[2].SetActive(false);
        menus[0].SetActive(true);

        Time.timeScale = 1;
        print("TimeScale " + Time.timeScale);
    }

    public void Settings()
    {
        menus[2].SetActive(false);
        menus[3].SetActive(true);
    }

    public void SettingsDone()
    {
        menus[3].SetActive(false);
        menus[2].SetActive(true);
    }

    public void Pause()
    {
        Time.timeScale = 0;
        print("TimeScale " + Time.timeScale);

        menus[0].SetActive(false);
        menus[1].SetActive(true);
    }

    public void Resume()
    {
        menus[1].SetActive(false);
        menus[0].SetActive(true);

        Time.timeScale = 1;
        print("TimeScale " + Time.timeScale);
    }

    public void Quit()
    {
        Application.Quit();
        print("Quit");
    }

    public void Save()
    {
        print("Save");
    }

    /// <summary>
    /// Skill Tree UI
    /// </summary>

    public void SkillTree()
    {
        Time.timeScale = 0;
        menus[0].SetActive(false);
        menus[4].SetActive(true); 
        Debug.Log("Ãppna tree");
    }

    public void SkillInfo(int skillId)
    {
        if (skillInfoHandlers.TryGetValue(skillId, out ISkillInfo skillInfo))
        {
            Time.timeScale = 0;
            skillInfo.ShowSkillInfo(menus);
        }
    }

    public void SkillsDone()
    {
        menus[5].SetActive(false);
        menus[4].SetActive(true);
        Debug.Log("Ãppna skill tree");
    }

    /// <summary>
    /// Stockpile UI
    /// </summary>

    public void ShowStockpileMenu()
    {
        Time.timeScale = 0;
        menus[0].SetActive(false); // Close main menu
        menus[5].SetActive(true); // Open weapon menu
        UpdateStockpileUI();
    }

    public void UpdateStockpileUI()
    {
        // Remove old buttons
        foreach (var button in stockpileButtons)
        {
            Destroy(button);
        }
        stockpileButtons.Clear();

        // Update Scrap-text
        scrapText.text = $"Scrap: {gameManager.GetScrap()}";

        // Create new buttons for weapon
        foreach (var weapon in stockpileManager.Weapons)
        {
            GameObject button = Instantiate(buttonTemplate, content);
            button.transform.Find("Text").GetComponent<Text>().text =
                $"{weapon.Name} (x{(weapon.Quantity == -1 ? "â" : weapon.Quantity.ToString())})";

            // Add click event
            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                stockpileManager.UseWeapon(weapon.Name);
                UpdateStockpileUI();
            });

            stockpileButtons.Add(button);
        }
    }

}

/// <summary>
/// Interface for showing skill-info.
/// </summary>

public interface ISkillInfo
{
    void ShowSkillInfo(List<GameObject> menus);
}

/// <summary>
/// Implementation of skill tree Inteface for management of skills.
/// </summary>

public class SkillInfo : ISkillInfo
{
    private int skillMenuIndex;

    public SkillInfo(int skillMenuIndex)
    {
        this.skillMenuIndex = skillMenuIndex;
    }

    public void ShowSkillInfo(List<GameObject> menus)
    {
        menus[4].SetActive(false); // Close skill tree
        menus[skillMenuIndex].SetActive(true); // Open relevent skill-info
        Debug.Log($"Ãppna skill {skillMenuIndex} info");
    }
}
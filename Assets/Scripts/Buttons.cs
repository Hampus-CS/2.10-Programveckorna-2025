using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using JetBrains.Annotations;
using Unity.VisualScripting.FullSerializer;

public class Buttons : MonoBehaviour
{
    [Header("All Menus")]
    public List<GameObject> menus = new();

    [Header("Buttons")]
    public List<Button> button = new();

    [Header("Skill tree")]
    public SkillManager skillManager;
    private Dictionary<int, ISkillInfo> skillInfoHandlers = new(); // Hanterar skills dynamiskt
    [SerializeField] private List<SkillButtonMapping> skillButtonMappings = new(); // Skill Buttons Mapping
    private int currentSkillMenuIndex = -1;

    [Header("Stockpile UI")]
    [SerializeField] private GameObject buttonTemplate; // Template for weapon buttons
    [SerializeField] private Transform content; // Scroll View content
    [SerializeField] private TMP_Text scrapText; // Text for Scrap
    [SerializeField] private StockpileManager stockpileManager; // Link to stockpile manager
    [SerializeField] private GameManager gameManager; // Management of Scrap
    private List<GameObject> stockpileButtons = new();


    private void Start()
    {
        Time.timeScale = 0;

        // Deactivating all panels at the start of the game
        foreach (GameObject menu in menus)
        {
            if (menu != null)
            {
                menu.SetActive(false);
            }
        }

        menus[2].SetActive(true);

        foreach (var skillInfo in skillInfoHandlers.Values)
        {
            if (skillInfo is SkillInfo skill)
            {
                menus[skill.skillMenuIndex].SetActive(false); // Deactivate the menu associated with the skill
            }
        }


        button[0].gameObject.SetActive(true);
        button[1].gameObject.SetActive(false);

        // Reg skills
        skillInfoHandlers[1] = new SkillInfo(6); // Skill 1 Info 
        skillInfoHandlers[2] = new SkillInfo(7); // Skill 2 Info 
        skillInfoHandlers[3] = new SkillInfo(8); // Skill 3 Info 
        skillInfoHandlers[4] = new SkillInfo(9); // Skill 4 Info 
        skillInfoHandlers[5] = new SkillInfo(10); // Skill 5 Info
        skillInfoHandlers[6] = new SkillInfo(11); // Skill 6 Info
        skillInfoHandlers[7] = new SkillInfo(12); // Skill 7 Info
        skillInfoHandlers[8] = new SkillInfo(13); // Skill 8 Info
        skillInfoHandlers[9] = new SkillInfo(14); // Skill 9 Info
        skillInfoHandlers[10] = new SkillInfo(15); // Skill 10 Info
        skillInfoHandlers[11] = new SkillInfo(16); // Skill 11 Info
        skillInfoHandlers[12] = new SkillInfo(17); // Skill 12 Info
        skillInfoHandlers[13] = new SkillInfo(18); // Skill 13 Info
        skillInfoHandlers[14] = new SkillInfo(19); // Skill 14 Info
        skillInfoHandlers[15] = new SkillInfo(20); // Skill 15 Info
        skillInfoHandlers[16] = new SkillInfo(21); // Skill 16 Info

        for (int i = 0; i < skillButtonMappings.Count; i++)
        {
            var mapping = skillButtonMappings[i];
            if (mapping.button != null)
            {
                int skillIndex = i + 1; // Antag att index 0 representerar färdighet 1
                mapping.button.onClick.AddListener(() => UnlockSkillByIndex(skillIndex));
            }
        }

        /*
         0 = game
         1 = pause
         2 = start
         3 = settings
         4 = skill tree
         5 = SidePanel
         6 = skill 1 info
         7 = skill 2 info
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
        Time.timeScale = 1;

        menus[2].SetActive(false);
        menus[0].SetActive(true);

    }

    public void Settings()
    {
        Time.timeScale = 0;
        menus[3].SetActive(true);
    }

    public void CloseSettings()
    {
        Time.timeScale = 1;
        if (menus[2].activeSelf == true)
        {
            menus[3].SetActive(false);
        }
        else
        {
            menus[3].SetActive(false);
            menus[0].SetActive(true);
        }
    }

    public void OpenSidePanel()
    {
        button[0].gameObject.SetActive(false);
        button[1].gameObject.SetActive(true);
        menus[5].SetActive(true);
    }
    public void CloseSidePanel()
    {
        button[0].gameObject.SetActive(true);
        button[1].gameObject.SetActive(false);
        menus[5].SetActive(false);
    }
    public void Pause()
    {
        Time.timeScale = 0;

        menus[0].SetActive(false);
        menus[1].SetActive(true);
    }

    public void Resume()
    {
        Time.timeScale = 1;

        menus[1].SetActive(false);
        menus[0].SetActive(true);
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

    [System.Serializable]
    public class SkillButtonMapping
    {
        public Button button;
        public string skillName;
    }

    public void SkillTree()
    {
        Time.timeScale = 0;
        menus[0].SetActive(false); // Close game view
        menus[4].SetActive(true);  // Open skill tree
    }

    public void CloseSkillTree()
    {
        Time.timeScale = 1;
        menus[4].SetActive(false); // Close skill tree
        menus[0].SetActive(true); // Open game view
    }

    public void SkillInfo(int skillId)
    {
        if (skillInfoHandlers.TryGetValue(skillId, out ISkillInfo skillInfo))
        {
            Time.timeScale = 0;
            skillInfo.ShowSkillInfo(menus);
        }
    }

    public void CloseSkillInfo()
    {
        if (currentSkillMenuIndex != -1)
        {
            menus[currentSkillMenuIndex].SetActive(false); // Close the relevant skill-info menu
            currentSkillMenuIndex = -1; // Reset the current skill menu index
        }
        menus[4].SetActive(true);  // Open skill tree
    }

    public void UnlockSkillByIndex(int skillIndex)
    {
        if (skillIndex > 0 && skillIndex <= skillButtonMappings.Count)
        {
            var skillName = skillButtonMappings[skillIndex - 1].skillName;
            if (skillManager != null)
            {
                skillManager.Unlock(skillName);
                print($"Unlocked {skillName}");
            }
            else
            {
                Debug.LogWarning("SkillManager is not assigned.");
            }
        }
        else
        {
            Debug.LogWarning("Invalid skill index.");
        }
    }

    /// <summary>
    /// Stockpile UI
    /// </summary>

    public void ShowStockpileMenu()
    {
        Time.timeScale = 0;
        menus[0].SetActive(false); // Close Main menu
        menus[5].SetActive(true); // Open Weapon menu
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
            button.transform.Find("Text").GetComponent<TMP_Text>().text = $"{weapon.Name} (x{(weapon.Quantity == -1 ? "and" : weapon.Quantity.ToString())})";

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
    public int skillMenuIndex;

    public SkillInfo(int skillMenuIndex)
    {
        this.skillMenuIndex = skillMenuIndex;
    }

    public void ShowSkillInfo(List<GameObject> menus)
    {
        menus[skillMenuIndex].SetActive(true); // Open relevent skill-info
    }
}
﻿using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class Buttons : MonoBehaviour
{
    [Header("GameManager")]
    [SerializeField] private GameManager gameManager; // Reference to GameManager

    [Header("All Menus")]
    public List<GameObject> menus = new();

    [Header("Buttons")]
    public List<Button> button = new();

    [Header("Skill tree")]
    public SkillManager skillManager;
    private Dictionary<int, ISkillInfo> skillInfoHandlers = new(); // Hanterar skills dynamiskt
    [SerializeField] private List<SkillButtonMapping> skillButtonMappings = new(); // Skill Buttons Mapping

    [Header("Stockpile UI")]
    [SerializeField] private GameObject weaponImageTemplate;  // Assign the prefab in Unity
    [SerializeField] private Transform content;  // Parent where the prefab will be instantiated
    [SerializeField] private TMP_Text scrapText; // Scrap display text

    private List<GameObject> stockpileImages = new(); // List to track created images


    [Header("Save UI")]
    [SerializeField] private SaveHandler saveHandler;
    [SerializeField] private Settings settings;


    private List<GameObject> stockpileButtons = new(); // Dynamic list of stockpile buttons

    private bool isSkillInfoActive = false;

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
        //button[2].gameObject.SetActive(false);
        //button[3].gameObject.SetActive(false);

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
        */

        //UpdateStockpileUI();
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
        Debug.Log("CloseSidePanel");
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

    public void SaveGame()
    {
        gameManager.SaveGame(saveHandler);
        settings.SaveSettings(saveHandler);
        Debug.Log("Game and settings saved.");
    }

    public void LoadGame()
    {
        gameManager.LoadGame(saveHandler);
        settings.LoadSettings(saveHandler);
        //UpdateStockpileUI(); // Directly update the UI here
        Debug.Log("Game and settings loaded.");
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

    public void OpenShop()
    {
        menus[22].SetActive(true); // Open shop
    }

    public void CloseShop()
    {
        menus[22].SetActive(false); // Close shop
    }
    public void OnBuyWeaponButton(string tag, int cost)
    {
        GameManager.Instance.BuyWeapon(tag, cost);
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
        if (skillInfoHandlers.TryGetValue(skillId, out ISkillInfo skillInfo) && isSkillInfoActive == false)
        {
            Time.timeScale = 0;
            skillInfo.ShowSkillInfo(menus);
            isSkillInfoActive = true;
            Debug.LogWarning("Skillinfo redan öppet");
        }
    }

    public void CloseSkillInfo()
    {
        isSkillInfoActive = false;
        Debug.LogWarning("Skillinfo inte öppen");
    }
   
    public void UnlockSkillByIndex(int skillIndex)
    {
        if (skillIndex > 0 && skillIndex <= skillButtonMappings.Count)
        {
            var skillName = skillButtonMappings[skillIndex - 1].skillName;
            Debug.Log($"Attempting to unlock skill: '{skillName}'");  // Added quotes to catch hidden spaces

            if (skillManager != null)
            {
                isSkillInfoActive = false;
                // Compare with registered skills
                foreach (var key in skillManager.skills.Keys)
                {
                    Debug.Log($"Comparing with registered skill: '{key}' - Match: {key == skillName}");
                }

                skillManager.Unlock(skillName);
                Debug.Log($"Skill '{skillName}' unlocked successfully.");
                
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
        //UpdateStockpileUI();
    }
    /*
    public void UpdateStockpileUI()
    {
        Debug.Log("Updating Stockpile UI...");

        if (gameManager == null || weaponImageTemplate == null || content == null || scrapText == null)
        {
            Debug.LogError("Missing UI references in Buttons.cs!");
            return;
        }

        // Clear previous UI elements
        foreach (var item in stockpileImages)
        {
            Destroy(item);
        }
        stockpileImages.Clear();

        // Update scrap text
        scrapText.text = $"Scrap: {gameManager.GetScrap()}";

        // Get sorted stockpile (higher tier first)
        var sortedStockpile = gameManager.stockpile.OrderByDescending(w => w.Tier).ToList();

        Debug.Log($"Stockpile UI Count: {sortedStockpile.Count}");

        foreach (var weapon in sortedStockpile)
        {
            GameObject newItem = Instantiate(weaponImageTemplate, content);
            newItem.SetActive(true);

            // Assign Image and Text
            Image weaponImage = newItem.transform.Find("WeaponImage")?.GetComponent<Image>();
            TMP_Text quantityText = newItem.transform.Find("QuantityText")?.GetComponent<TMP_Text>();

            if (weaponImage != null && weapon.Icon != null)
            {
                weaponImage.sprite = weapon.Icon;
            }

            if (quantityText != null)
            {
                quantityText.text = $"x{weapon.Quantity}";
            }

            Debug.Log($"Displaying {weapon.Name} x{weapon.Quantity} in UI");
            stockpileImages.Add(newItem);
        }
    }
    */
    public void EnableWeaponButtons()
    {
        button[2].gameObject.SetActive(true);
        button[3].gameObject.SetActive(true);
    }

    public void BuyWeapon(string weaponName, int cost)
    {
        if (gameManager != null)
        {
            if (gameManager.TrySpendScrap(cost)) // Check if the player can afford it
            {
                gameManager.BuyWeapon(weaponName, cost);
            }
            else
            {
                Debug.LogWarning("Not enough scrap to buy the weapon.");
            }
        }
        else
        {
            Debug.LogError("GameManager reference is not assigned in Buttons.cs.");
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
using System.Collections.Generic;
using System.IO;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class SkillManager : MonoBehaviour
{
    public Dictionary<string, ISkill> skills = new();

    TroopPersonalityScript troopPersonalityScript;
    ShopUIManager shopUIManager;
    Buttons buttons;

    void Start()
    {
        // Initialize skills
        //AddSkill(new WeaponSmith(50));
        AddSkill(new LongerBarrels(50));
        AddSkill(new HigherQualityAmmunition(50));
        AddSkill(new StandardisedProduction(50));
        AddSkill(new RapidFire(50));
        AddSkill(new LargerAmmunitionCapacity(50));
        AddSkill(new MedicalSupport(50));
        AddSkill(new MeditationExercises(50));
        AddSkill(new IceDubbs(50));
        AddSkill(new DisciplineTraining(50));
        AddSkill(new Birdwatching(50));
        AddSkill(new HeavierArmor(50));
        AddSkill(new BedsForScrap(50));
        AddSkill(new ArmTheScientists(50));
        AddSkill(new RooftopKorean(50));
        AddSkill(new IncreasedTrainFirepower(50));


        // Example usage
        /*
        UnlockSkill("WeapondSmith");
        UseSkill("WeapondSmith");
        UseSkill("WeapondSmith"); // Locked skill
        DisplaySkills();
        */

        foreach (var skill in skills)
        {
            Debug.Log($"Registered skill: {skill.Key}");
        }
    }

    public void AddSkill(ISkill skill)
    {
        if (!skills.ContainsKey(skill.Name))
        {
            skills[skill.Name] = skill;
            Debug.Log($"Skill '{skill.Name}' added.");
        }
        else
        {
            Debug.LogWarning($"Skill '{skill.Name}' already exists.");
        }
    }
    /*
    public void Unlock(string skillName)
    {
        Debug.Log($"Attempting to unlock: {skillName}");
        if (skills.TryGetValue(skillName, out ISkill skill))
        {
            Debug.Log($"Found skill: {skill.Name}");
            if (!skill.IsUnlocked)
            {
                if (GameManager.Instance.TrySpendScrap(skill.Cost))
                {
                    skill.Unlock();
                    Debug.Log($"Unlocked: {skill.Name}");
                    if (skill is BaseSkill baseSkill && !baseSkill.RequiresTarget)
                    {
                        Debug.Log($"Calling Use() for: {baseSkill.Name}");
                        baseSkill.Use(); // This should trigger Execute()
                    }
                }
                else
                {
                    Debug.LogWarning($"Not enough scrap for {skillName}");
                }
            }
            else
            {
                Debug.Log($"{skillName} is already unlocked.");
            }
        }
        else
        {
            Debug.LogWarning($"Skill '{skillName}' not found.");
        }
    }
    */
    public void Unlock(string skillName)
    {
        Debug.Log($"Trying to unlock: '{skillName}'");

        if (skills.TryGetValue(skillName, out ISkill skill))
        {
            Debug.Log($"Skill found: '{skill.Name}'");
        }
        else
        {
            Debug.LogWarning($"Skill '{skillName}' not found!");

            // Debug: List potential close matches
            foreach (var key in skills.Keys)
            {
                if (key.Trim() == skillName.Trim())
                {
                    Debug.LogWarning($"Close match found: '{key}' after trimming.");
                }
            }
        }
    }

    public void UseSkill(string skillName, GameObject target = null)
    {
        if (skills.TryGetValue(skillName, out ISkill skill))
        {
            if (skill is BaseSkill baseSkill)
            {
                baseSkill.Use(target); // Pass the target or null
            }
            else
            {
                Debug.LogWarning($"Skill '{skillName}' is not a BaseSkill.");
            }
        }
        else
        {
            Debug.LogWarning($"Skill '{skillName}' does not exist.");
        }
    }

    public void DisplaySkills()
    {
        Debug.Log("Skills Status:");
        foreach (var skill in skills.Values)
        {
            Debug.Log($"{skill.Name} - {(skill.IsUnlocked ? "Unlocked" : $"Locked (Cost: {skill.Cost})")}");
        }
    }
}

/// <summary>
/// Interface for all skills
/// </summary>
/// 
public interface ISkill
{
    string Name { get; }
    bool IsUnlocked { get; set; }
    int Cost { get; }

    void Unlock();
    void Use(GameObject target);
}

/// <summary>
/// Base class for skills
/// </summary>
public abstract class BaseSkill : ISkill
{
    protected Buttons button;

    public string Name { get; private set; }
    public bool IsUnlocked { get; set; }
    public int Cost { get; private set; }
    public bool RequiresTarget { get; private set; } // Flag to indicate if a target is required

    protected BaseSkill(string name, int cost, bool requiresTarget = false)
    {
        Name = name;
        Cost = cost;
        RequiresTarget = requiresTarget;
        IsUnlocked = false;
    }

    public void Use(GameObject target = null)
    {
        if (IsUnlocked)
        {
            if (RequiresTarget && target == null)
            {
                Debug.LogWarning($"Skill '{Name}' requires a target, but none was provided.");
                return;
            }

            Execute(target);
        }
        else
        {
            Debug.Log($"Skill '{Name}' is locked. Unlock it to use.");
        }
    }

    protected abstract void Execute(GameObject target = null);

    public void Unlock()
    {
        IsUnlocked = true;
        Debug.Log($"Skill '{Name}' unlocked.");
    }
}

/// <summary>
/// Skills
/// </summary>

/*
public class WeaponSmith : BaseSkill
{
    public WeaponSmith(int cost) : base("WeaponSmith", cost) { }
    
    protected override void Execute(GameObject target = null)
    {
        Debug.Log("K�r execute");
    }
}
*/

public class LongerBarrels : BaseSkill
{
    public LongerBarrels(int cost) : base("LongerBarrels", cost) { }

    protected override void Execute(GameObject target)
    {
        if (target != null)
        {
            TroopPersonalityScript troopPersonality = target.GetComponent<TroopPersonalityScript>();
            if (troopPersonality != null && troopPersonality.isFriendly)
            {
                troopPersonality.range += 1;
                Debug.Log($"Increased range for {target.name} to {troopPersonality.range}");
            }
            else
            {
                Debug.LogWarning($"Target {target.name} is not a friendly soldier.");
            }
        }
    }
}

public class HigherQualityAmmunition : BaseSkill
{
    public HigherQualityAmmunition(int cost) : base("HigherQualityAmmunition", cost) { }

    protected override void Execute(GameObject target)
    {
        if (target != null)
        {
            Weapon weapon = target.GetComponentInChildren<Weapon>();
            if (weapon != null)
            {
                weapon.UpgradeDamage();
                Debug.Log($"Increased weapon damage for {target.name}.");
            }
        }
    }
}

public class StandardisedProduction : BaseSkill
{
    public StandardisedProduction(int cost) : base("StandardisedProduction", cost) { }

    protected override void Execute(GameObject target = null)
    {
        GameManager.Instance.scrap += 100;
        Debug.Log("Weapon costs reduced by 25%.");
    }
}

public class RapidFire : BaseSkill
{
    public RapidFire(int cost) : base("RapidFire", cost) { }

    protected override void Execute(GameObject target)
    {
        if (target != null)
        {
            Weapon weapon = target.GetComponentInChildren<Weapon>();
            if (weapon != null)
            {
                weapon.shootCoolDown *= 0.8f;
                Debug.Log($"Increased fire rate for {target.name}. New cooldown: {weapon.shootCoolDown}");
            }
        }
    }
}
public class LargerAmmunitionCapacity : BaseSkill
{
    public LargerAmmunitionCapacity(int cost) : base("LargerAmmunitionCapacity", cost, requiresTarget: true) { }

    protected override void Execute(GameObject target)
    {
        TroopPersonalityScript troopPersonality = target.GetComponent<TroopPersonalityScript>();
        if (troopPersonality != null && troopPersonality.isFriendly)
        {
            Weapon weapon = target.GetComponentInChildren<Weapon>();
            if (weapon != null)
            {
                weapon.AmmoCap += 10; // Increase magazine size by 10
                Debug.Log($"Increased ammo capacity for {target.name}. New capacity: {weapon.AmmoCap}");
            }
            else
            {
                Debug.LogWarning($"No weapon found for {target.name}.");
            }
        }
        else
        {
            Debug.LogWarning($"Target {target.name} is not a friendly soldier.");
        }
    }
}


public class MedicalSupport : BaseSkill
{
    public MedicalSupport(int cost) : base("MedicalSupport", cost, requiresTarget: true) { }

    protected override void Execute(GameObject target)
    {
        TroopPersonalityScript troopPersonality = target.GetComponent<TroopPersonalityScript>();
        if (troopPersonality != null && troopPersonality.isFriendly)
        {
            troopPersonality.health += 20; // Heal troops
            Debug.Log($"Healed {target.name}. New health: {troopPersonality.health}");
        }
        else
        {
            Debug.LogWarning($"Target {target.name} is not a friendly soldier.");
        }
    }
}

public class MeditationExercises : BaseSkill
{
    public MeditationExercises(int cost) : base("MeditationExercises", cost, requiresTarget: false) { }

    protected override void Execute(GameObject target = null)
    {
        TroopPersonalityScript[] soldiers = Object.FindObjectsOfType<TroopPersonalityScript>();

        foreach (var soldier in soldiers)
        {
            if (soldier.isFriendly)
            {
                soldier.stress = Mathf.Max(0, soldier.stress - 2); // Reduce stress but ensure it doesn't go below 0
                Debug.Log($"Reduced stress for {soldier.name}. Current stress: {soldier.stress}");
            }
        }

        Debug.Log("MeditationExercises applied to all friendly soldiers.");
    }
}

public class IceDubbs : BaseSkill
{
    public IceDubbs(int cost) : base("IceDubbs", cost, requiresTarget: false) { }

    protected override void Execute(GameObject target = null)
    {
        TroopPersonalityScript[] soldiers = Object.FindObjectsOfType<TroopPersonalityScript>();

        foreach (var soldier in soldiers)
        {
            if (soldier.isFriendly)
            {
                NavMeshAgent agent = soldier.GetComponent<NavMeshAgent>();
                if (agent != null)
                {
                    agent.speed += 1.0f; // Increase movement speed by 1.0
                    Debug.Log($"Increased speed for {soldier.name}. New speed: {agent.speed}");
                }
                else
                {
                    Debug.LogWarning($"No NavMeshAgent found for {soldier.name}.");
                }
            }
        }

        Debug.Log("IceDubbs applied to all friendly soldiers.");
    }
}

public class DisciplineTraining : BaseSkill
{
    public DisciplineTraining(int cost) : base("DisciplineTraining", cost, requiresTarget: false) { }

    protected override void Execute(GameObject target = null)
    {
        TroopPersonalityScript[] soldiers = Object.FindObjectsOfType<TroopPersonalityScript>();

        foreach (var soldier in soldiers)
        {
            if (soldier.isFriendly)
            {
                soldier.health += 5;       // Increase health by 5
                soldier.accuracy += 0.05f; // Increase accuracy by 0.05
                soldier.range += 1;        // Increase range by 1

                Debug.Log($"Upgraded {soldier.name}: Health={soldier.health}, Accuracy={soldier.accuracy}, Range={soldier.range}");
            }
        }

        Debug.Log("DisciplineTraining applied to all friendly soldiers.");
    }
}

public class Birdwatching : BaseSkill
{
    public Birdwatching(int cost) : base("Birdwatching", cost, requiresTarget: false) { }

    protected override void Execute(GameObject target = null)
    {
        TroopPersonalityScript[] soldiers = Object.FindObjectsOfType<TroopPersonalityScript>();

        foreach (var soldier in soldiers)
        {
            if (soldier.isFriendly)
            {
                soldier.accuracy = Mathf.Min(1f, soldier.accuracy + 0.1f); // Increase accuracy but cap at 1.0
                Debug.Log($"Increased accuracy for {soldier.name}. Current accuracy: {soldier.accuracy}");
            }
        }

        Debug.Log("Birdwatching applied to all friendly soldiers.");
    }
}

public class HeavierArmor : BaseSkill
{
    public HeavierArmor(int cost) : base("HeavierArmor", cost, requiresTarget: false) { }

    protected override void Execute(GameObject target = null)
    {
        TroopPersonalityScript[] soldiers = Object.FindObjectsOfType<TroopPersonalityScript>();

        foreach (var soldier in soldiers)
        {
            if (soldier.isFriendly)
            {
                // Increase health
                soldier.health += 20;

                // Reduce movement speed
                NavMeshAgent agent = soldier.GetComponent<NavMeshAgent>();
                if (agent != null)
                {
                    agent.speed = Mathf.Max(0.5f, agent.speed - 0.5f); // Ensure speed doesn't go below 0.5
                    Debug.Log($"Increased health and reduced speed for {soldier.name}: Health={soldier.health}, Speed={agent.speed}");
                }
                else
                {
                    Debug.LogWarning($"No NavMeshAgent found for {soldier.name}. Health increased only.");
                }
            }
        }

        Debug.Log("HeavierArmor applied to all friendly soldiers.");
    }
}

public class BedsForScrap : BaseSkill
{
    public BedsForScrap(int cost) : base("BedsForScrap", cost, requiresTarget: false) { }

    protected override void Execute(GameObject target = null)
    {
        // Increase player's income
        GameManager gameManager = GameManager.Instance;
        if (gameManager != null)
        {
            int incomeIncrease = 50; // Example income boost
            gameManager.AddScrap(incomeIncrease);
            Debug.Log($"Player income increased by {incomeIncrease}. Current scrap: {gameManager.GetScrap()}");
        }
        else
        {
            Debug.LogError("GameManager instance not found. Cannot increase player income.");
        }

        // Increase stress for all friendly soldiers
        TroopPersonalityScript[] soldiers = Object.FindObjectsOfType<TroopPersonalityScript>();
        foreach (var soldier in soldiers)
        {
            if (soldier.isFriendly)
            {
                soldier.stress += 2; // Increase stress by 2
                Debug.Log($"Increased stress for {soldier.name}. Current stress: {soldier.stress}");
            }
        }

        Debug.Log("BedsForScrap applied globally: income increased, stress raised.");
    }
}

public class ArmTheScientists : BaseSkill
{
    public ArmTheScientists(int cost) : base("ArmTheScientists", cost, requiresTarget: false) { }

    protected override void Execute(GameObject target = null)
    {
        GameManager gameManager = GameManager.Instance;
        if (gameManager != null)
        {
            int manpowerIncrease = 10; // Example manpower boost
            int researchPenalty = 5;  // Example research point reduction

            gameManager.AddManpower(manpowerIncrease);
            gameManager.ReduceResearchGain(researchPenalty);

            Debug.Log($"Manpower increased by {manpowerIncrease}. Research points reduced by {researchPenalty}.");
        }
        else
        {
            Debug.LogError("GameManager instance not found. Cannot modify manpower or research gain.");
        }
    }
}

public class RooftopKorean : BaseSkill
{
    public RooftopKorean(int cost) : base("RooftopKorean", cost, requiresTarget: false) { }

    protected override void Execute(GameObject target = null)
    {
        GameManager gameManager = GameManager.Instance;
        if (gameManager != null)
        {
            gameManager.EnablePlayerShooting();
        }
        else
        {
            Debug.LogError("GameManager instance not found. Cannot enable player shooting.");
        }
    }
}

public class IncreasedTrainFirepower : BaseSkill
{
    public IncreasedTrainFirepower(int cost) : base("IncreasedTrainFirepower", cost, requiresTarget: false) { }

    protected override void Execute(GameObject target = null)
    {
        GameManager gameManager = GameManager.Instance;
        if (gameManager != null)
        {
            float cooldownReductionPercentage = 20f; // Reduce cooldown by 20%
            gameManager.ReducePlayerWeaponCooldown(cooldownReductionPercentage);
        }
        else
        {
            Debug.LogError("GameManager instance not found. Cannot increase train firepower.");
        }
    }
}
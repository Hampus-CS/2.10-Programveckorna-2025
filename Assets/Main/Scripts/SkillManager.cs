using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    private Dictionary<string, ISkill> skills = new();

    void Start()
    {
        // Initialize skills
        AddSkill(new WeaponSmith(50));
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
    public void Unlock(string skillName)
    {
        if (skills.TryGetValue(skillName, out ISkill skill))
        {
            if (!skill.IsUnlocked)
            {
                if (GameManager.Instance.TrySpendScrap(skill.Cost))
                {
                    skill.Unlock();
                }
                else
                {
                    Debug.LogWarning($"Not enough scrap to unlock '{skillName}'. Cost: {skill.Cost}");
                }
            }
            else
            {
                Debug.Log($"Skill '{skillName}' is already unlocked.");
            }
        }
        else
        {
            Debug.LogWarning($"Skill '{skillName}' does not exist.");
        }
    }

    public void UseSkill(string skillName)
    {
        if (skills.TryGetValue(skillName, out ISkill skill))
        {
            skill.Use();
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
public interface ISkill
{
    string Name { get; }
    bool IsUnlocked { get; set; }
    int Cost { get; }

    void Unlock();
    void Use();
}

/// <summary>
/// Base class for skills
/// </summary>
public abstract class BaseSkill : ISkill
{
    public string Name { get; private set; }
    public bool IsUnlocked { get; set; }
    public int Cost { get; private set; }

    protected BaseSkill(string name, int cost)
    {
        Name = name;
        Cost = cost;
        IsUnlocked = false;
    }

    public void UnlockSkill()
    {
        IsUnlocked = true;
        Debug.Log($"Skill '{Name}' unlocked.");
    }

    public void Use()
    {
        if (IsUnlocked)
        {
            Execute();
        }
        else
        {
            Debug.Log($"Skill '{Name}' is locked. Unlock it to use.");
        }
    }
    protected abstract void Execute();

    public void Unlock()
    {
        IsUnlocked = true;
        Debug.Log($"Skill '{Name}' has been unlocked.");
    }
}

public class WeaponSmith : BaseSkill
{
    public WeaponSmith(int cost) : base("WeaponSmith", cost) { }

    protected override void Execute()
    {
        // Implement dash logic here
    }
}

public class LongerBarrels : BaseSkill
{
    public LongerBarrels(int cost) : base("LongerBarrels", cost) { }

    protected override void Execute()
    {
        // Implement dash logic here
    }
}

public class HigherQualityAmmunition : BaseSkill
{
    public HigherQualityAmmunition(int cost) : base("HigherQualityAmmunition", cost) { }

    protected override void Execute()
    {
        // Implement dash logic here
    }
}

public class StandardisedProduction : BaseSkill
{
    public StandardisedProduction(int cost) : base("StandardisedProduction", cost) { }

    protected override void Execute()
    {
        // Implement dash logic here
    }
}

public class RapidFire : BaseSkill
{
    public RapidFire(int cost) : base("RapidFire", cost) { }

    protected override void Execute()
    {
        // Implement dash logic here
    }
}

public class LargerAmmunitionCapacity : BaseSkill
{
    public LargerAmmunitionCapacity(int cost) : base("LargerAmmunitionCapacity", cost) { }

    protected override void Execute()
    {
        // Implement dash logic here
    }
}

public class MedicalSupport : BaseSkill
{
    public MedicalSupport(int cost) : base("MedicalSupport", cost) { }

    protected override void Execute()
    {
        // Implement dash logic here
    }
}

public class MeditationExercises : BaseSkill
{
    public MeditationExercises(int cost) : base("MeditationExercises", cost) { }

    protected override void Execute()
    {
        // Implement dash logic here
    }
}

public class IceDubbs : BaseSkill
{
    public IceDubbs(int cost) : base("IceDubbs", cost) { }

    protected override void Execute()
    {
        // Implement dash logic here
    }
}

public class DisciplineTraining : BaseSkill
{
    public DisciplineTraining(int cost) : base("DisciplineTraining", cost) { }

    protected override void Execute()
    {
        // Implement dash logic here
    }
}

public class Birdwatching : BaseSkill
{
    public Birdwatching(int cost) : base("Birdwatching", cost) { }

    protected override void Execute()
    {
        // Implement dash logic here
    }
}

public class HeavierArmor : BaseSkill
{
    public HeavierArmor(int cost) : base("HeavierArmor", cost) { }

    protected override void Execute()
    {
        // Implement dash logic here
    }
}

public class BedsForScrap : BaseSkill
{
    public BedsForScrap(int cost) : base("BedsForScrap", cost) { }

    protected override void Execute()
    {
        // Implement dash logic here
    }
}
public class ArmTheScientists : BaseSkill
{
    public ArmTheScientists(int cost) : base("ArmTheScientists", cost) { }

    protected override void Execute()
    {
        // Implement dash logic here
    }
}
public class RooftopKorean: BaseSkill
{
    public RooftopKorean(int cost) : base("RooftopKorean", cost) { }

    protected override void Execute()
    {
        // Implement dash logic here
    }
}
public class IncreasedTrainFirepower : BaseSkill
{
    public IncreasedTrainFirepower(int cost) : base("IncreasedTrainFirepower", cost) { }

    protected override void Execute()
    {
        // Implement dash logic here
    }
}

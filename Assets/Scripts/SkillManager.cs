using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    private Dictionary<string, ISkill> skills = new();

    void Start()
    {
        // Initialize skills
        AddSkill(new WeapondSmith(/*pris på skill*/));
        AddSkill(new LongerBarrels(/*pris på skill*/));
        AddSkill(new HigherQualityAmmunition(/*pris på skill*/));
        AddSkill(new StandardisedProduction(/*pris på skill*/));
        AddSkill(new RapidFire(/*pris på skill*/));
        AddSkill(new LargerAmmunitionCapacity(/*pris på skill*/));
        AddSkill(new MedicalSupport(/*pris på skill*/));
        AddSkill(new MeditationExercises(/*pris på skill*/));
        AddSkill(new IceDubbs(/*pris på skill*/));
        AddSkill(new DisciplineTraining(/*pris på skill*/));
        AddSkill(new Birdwatching(/*pris på skill*/));
        AddSkill(new HeavierArmor(/*pris på skill*/));
        AddSkill(new BedsForScrap(/*pris på skill*/));

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
    public void UnlockSkill(string skillName)
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

    public void Unlock()
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
}

public class WeapondSmith : BaseSkill
{
    public WeapondSmith(int cost) : base("WeapondSmith", cost) { }

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

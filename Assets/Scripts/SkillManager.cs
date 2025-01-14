using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    private Dictionary<string, ISkill> skills = new();

    void Start()
    {
        // Initialize skills
        AddSkill(new DoubleJumpSkill(50));
        AddSkill(new DashSkill(30));
        AddSkill(new ShieldSkill(40));

        // Example usage
        UnlockSkill("Double Jump");
        UseSkill("Double Jump");
        UseSkill("Dash"); // Locked skill
        DisplaySkills();
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

// Specific skill implementations
public class DoubleJumpSkill : BaseSkill
{
    public DoubleJumpSkill(int cost) : base("Double Jump", cost) { }
    
    protected override void Execute()
    {
        // Implement double jump logic here
    }
}

public class DashSkill : BaseSkill
{
    public DashSkill(int cost) : base("Dash", cost) { }

    protected override void Execute()
    {
        // Implement dash logic here
    }
}

public class ShieldSkill : BaseSkill
{
    public ShieldSkill(int cost) : base("Shield", cost) { }

    protected override void Execute()
    {
        // Implement shield logic here
    }
}
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    public bool IsAlive => currentHealth > 0;

    // Referens till den linje soldaten är registrerad i
    public Line CurrentLine { get; set; }

    public bool IsPlayer { get; set; } // Anger om det är en spelarsoldat eller fiendesoldat

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (!IsAlive) return;

        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. Remaining health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (CurrentLine != null)
        {
            CurrentLine.RemoveSoldier(gameObject, IsPlayer);
        }

        Destroy(gameObject);
        Debug.Log($"{gameObject.name} has died.");
    }
}
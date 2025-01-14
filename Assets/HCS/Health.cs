using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    public bool IsAlive => currentHealth > 0;

    // Referens till den linje soldaten �r registrerad i
    public Line CurrentLine { get; set; }

    public bool IsPlayer { get; set; } // Anger om det �r en spelarsoldat eller fiendesoldat

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

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

        // Minska r�kningen av soldater
        var spawner = FindObjectOfType<SoldierSpawner>();
        if (spawner != null)
        {
            spawner.DecreaseSoldierCount();
        }

        // F�rst�r gameobject
        Destroy(gameObject);
    }
}
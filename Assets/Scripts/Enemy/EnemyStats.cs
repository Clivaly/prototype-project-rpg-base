using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public string enemyName = "Inimigo";
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }
}

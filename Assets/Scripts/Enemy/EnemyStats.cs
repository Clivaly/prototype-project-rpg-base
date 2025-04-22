using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Vida")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Referências")]
    public Animator animator; // arraste o Animator aqui no Inspector
    private bool isDead = false;

    [Header("Nome")]
    public string enemyName = "Inimigo";

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return; // Evita dano após a morte

        currentHealth -= amount;
        //currentHealth = Mathf.Max(currentHealth, 0f); // Garante que não fique negativo
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        Debug.Log("Inimigo tomou dano! Vida atual: " + currentHealth);

        if (currentHealth <= 0)
            Die();
    }

    public void Die()
    {
        if (isDead) return; // evita chamar duas vezes
        isDead = true;

        Debug.Log("Inimigo morreu!");
        // Para completamente o inimigo
        if (animator != null)
        {
            animator.ResetTrigger("Jump"); // se tiver
            animator.ResetTrigger("Speed"); // se usar isso
            animator.SetFloat("Speed", 0f);
            animator.SetTrigger("Die");
        }

        //if (animator != null)
        //    animator.SetTrigger("Die");

        // Só desativa se tiver o componente
        var controller = GetComponent<CharacterController>();
        if (controller != null)
            controller.enabled = false;

        // aqui você pode adicionar efeitos visuais, loot, remover seleção etc.
    }

}

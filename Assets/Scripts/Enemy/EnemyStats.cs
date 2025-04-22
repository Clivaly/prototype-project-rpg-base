using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Vida")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Refer�ncias")]
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
        if (isDead) return; // Evita dano ap�s a morte

        currentHealth -= amount;
        //currentHealth = Mathf.Max(currentHealth, 0f); // Garante que n�o fique negativo
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

        // S� desativa se tiver o componente
        var controller = GetComponent<CharacterController>();
        if (controller != null)
            controller.enabled = false;

        // aqui voc� pode adicionar efeitos visuais, loot, remover sele��o etc.
    }

}

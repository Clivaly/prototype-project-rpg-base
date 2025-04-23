using System.Collections;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Vida")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Refer�ncias")]
    private bool isDead = false;

    [Header("Visual")]
    public Transform visualHolder;

    [Header("Nome")]
    public string enemyName = "Inimigo";

    private Animator animator;
    void Awake()
    {
        currentHealth = maxHealth;

        // Se n�o foi setado manualmente, tenta pegar do visualHolder se tiver
        if (animator == null && visualHolder != null)
        {
            animator = visualHolder.GetComponentInChildren<Animator>();
            if (animator == null)
                Debug.LogWarning($"{enemyName}: Animator n�o encontrado no visualHolder!");
        }

        // Se mesmo assim n�o achou, tenta um fallback geral
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
            if (animator == null)
                Debug.LogWarning($"{enemyName}: Animator n�o encontrado no GameObject nem em filhos!");
        }
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
        else
        {
            Debug.LogWarning("Animator n�o est� atribu�do nesse inimigo!");
        }

        //if (animator != null)
        //    animator.SetTrigger("Die");

        // S� desativa se tiver o componente
        var controller = GetComponent<CharacterController>();
        if (controller != null)
            controller.enabled = false;

        // Ativa gravidade no Rigidbody para cair com f�sica
        //Rigidbody rb = GetComponent<Rigidbody>();
        //if (rb != null)
        //{
        //    rb.isKinematic = false; // Permite a f�sica agir
        //    rb.useGravity = true;
        //}

        // Faz o inimigo descer at� o ch�o
        //SnapToGround();
        //StartCoroutine(SmoothSnapToGround());
        StartCoroutine(WaitThenSnapToGround(3.9f)); // tempo exato da anima��o


        // aqui voc� pode adicionar efeitos visuais, loot, remover sele��o etc.
    }

    IEnumerator WaitThenSnapToGround(float delay)
    {
        yield return new WaitForSeconds(delay);
        yield return StartCoroutine(SmoothSnapToGround());
    }


    IEnumerator SmoothSnapToGround()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 2f;

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 5f, LayerMask.GetMask("Default")))
        {
            float modelHeightOffset = 0.05f;
            Vector3 startPos = transform.position;
            Vector3 targetPos = new Vector3(startPos.x, hit.point.y + modelHeightOffset, startPos.z);

            float duration = 0.3f; // tempo total da descida
            float elapsed = 0f;

            while (elapsed < duration)
            {
                transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            // Garante que fica na posi��o final exata
            transform.position = targetPos;
        }
    }


    //void SnapToGround()
    //{
    //    Vector3 rayOrigin = transform.position + Vector3.up * 2f;

    //    if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 5f, LayerMask.GetMask("Default")))
    //    {
    //        float modelHeightOffset = 0.05f;

    //        // Posi��o alvo (ch�o + offset)
    //        Vector3 targetPosition = new Vector3(
    //            transform.position.x,
    //            hit.point.y + modelHeightOffset,
    //            transform.position.z
    //        );

    //        // Suaviza o ajuste ao ch�o
    //        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 1f);
    //    }
    //}

}

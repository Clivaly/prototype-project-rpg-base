using UnityEngine;

public class EnemyTriggerDamage : MonoBehaviour
{
    public EnemyStats stats;
    public float damage = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            stats.TakeDamage(damage);
            Debug.Log($"{stats.enemyName} levou {damage} de dano!");
        }
    }
}

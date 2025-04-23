using UnityEngine;

public class EnemyTarget : MonoBehaviour, ITargetable
{
    //[Header("Feedback visual")]
    private GameObject selectionCircle;

    private EnemyStats stats;

    private void Awake()
    {
        stats = GetComponent<EnemyStats>();

        // Tenta encontrar automaticamente o círculo se não estiver setado
        if (selectionCircle == null)
        {
            var marker = GetComponentInChildren<SelectionCircleMarker>(true);
            if (marker != null)
            {
                selectionCircle = marker.gameObject;
                Debug.Log($"[EnemyTarget] Círculo de seleção encontrado automaticamente: {selectionCircle.name}", this);
            }
            else
            {
                Debug.LogWarning("[EnemyTarget] Nenhum SelectionCircleMarker encontrado!");
            }
        }
    }

    public void Select()
    {
        Debug.Log("Selecionado!");
        if (selectionCircle != null)
            selectionCircle.SetActive(true); // Mostra o círculo
    }

    public void Deselect()
    {
        if (selectionCircle != null)
            selectionCircle.SetActive(false); // Esconde o círculo
    }

    public Transform GetTransform() => transform;

    public string GetDisplayName() => stats != null ? stats.enemyName : "Inimigo";

    public TargetType GetTargetType() => TargetType.Enemy;

    public float GetCurrentHealth() => stats != null ? stats.currentHealth : 0;

    public float GetMaxHealth() => stats != null ? stats.maxHealth : 1;
}

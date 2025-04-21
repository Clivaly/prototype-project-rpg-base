using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TargetUIController : MonoBehaviour
{
    [Header("UI Referências")]
    public GameObject targetPanel;
    public TextMeshProUGUI nameText;
    public Image healthBarFill; // <- Usando Image ao invés de Slider!

    private Transform targetTransform; // <- Guarda a posição do inimigo
    private Camera mainCamera;         // <- Pra converter posição mundo para tela

    private EnemyStats currentTarget;

    public void ShowTarget(EnemyStats target)
    {
        currentTarget = target;
        targetTransform = target.transform;
        mainCamera = Camera.main;

        if (targetPanel != null)
            targetPanel.SetActive(true);

        nameText.text = target.enemyName;
        UpdateHealthBar(); // Atualiza na hora
    }

    public void HideTarget()
    {
        currentTarget = null;
        if (targetPanel != null)
            targetPanel.SetActive(false);
    }

    void Update()
    {
        if (currentTarget != null)
        {
            UpdateHealthBar(); // Atualiza em tempo real
            UpdateTargetPosition();
        }
    }

    void UpdateHealthBar()
    {
        float fillAmount = Mathf.Clamp01(currentTarget.currentHealth / currentTarget.maxHealth);
        healthBarFill.fillAmount = fillAmount;
    }

    void UpdateTargetPosition()
    {
        if (targetTransform == null) return;

        Vector3 worldPosition = targetTransform.position + Vector3.up * 2.5f; // offset pra UI aparecer acima da cabeça
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);

        targetPanel.transform.position = screenPosition;
    }

}

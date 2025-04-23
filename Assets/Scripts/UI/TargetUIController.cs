using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TargetUIController : MonoBehaviour
{
    [Header("UI Referências")]
    public GameObject targetPanel;
    public TextMeshProUGUI nameText;
    public Image healthBarFill;

    private ITargetable currentTarget;
    private Transform targetTransform;
    private Camera mainCamera;


    public void ShowTarget(ITargetable target)
    {
        currentTarget = target;
        targetTransform = target.GetTransform();
        mainCamera = Camera.main;

        if (targetPanel != null)
            targetPanel.SetActive(true);

        nameText.text = target.GetDisplayName();

        // Verifica tipo de alvo
        bool isEnemy = target.GetTargetType() == TargetType.Enemy;

        // Ativa ou desativa a barra de vida baseada no tipo do alvo
        GameObject healthBarContainer = healthBarFill.transform.parent.gameObject;
        if (healthBarContainer != null)
            healthBarContainer.SetActive(isEnemy);

        // Atualiza vida imediatamente se for inimigo
        if (isEnemy)
            UpdateHealthBar();
    }


    //public void ShowTarget(ITargetable target)
    //{
    //    currentTarget = target;
    //    targetTransform = target.GetTransform();
    //    mainCamera = Camera.main;

    //    if (targetPanel != null)
    //        targetPanel.SetActive(true);

    //    nameText.text = target.GetDisplayName();

    //    // Só mostra a barra de vida se for Enemy
    //    if (target.GetTargetType() == TargetType.Enemy)
    //        UpdateHealthBar();
    //    else
    //        healthBarFill.transform.parent.gameObject.SetActive(false); // Esconde o container da barra
    //}

    public void HideTarget()
    {
        currentTarget = null;
        if (targetPanel != null)
            targetPanel.SetActive(false);

        //healthBarFill.transform.parent.gameObject.SetActive(true); // Reativa pro próximo alvo
    }
    //public void HideTarget()
    //{
    //    currentTarget = null;

    //    if (targetPanel != null)
    //        targetPanel.SetActive(false);

    //    // Esconde a barra de vida sempre
    //    GameObject healthBarContainer = healthBarFill.transform.parent.gameObject;
    //    if (healthBarContainer != null)
    //        healthBarContainer.SetActive(false);
    //}


    void Update()
    {
        if (currentTarget != null)
        {
            if (currentTarget.GetTargetType() == TargetType.Enemy)
                UpdateHealthBar();

            UpdateTargetPosition();
        }
    }

    void UpdateHealthBar()
    {
        float fillAmount = Mathf.Clamp01(currentTarget.GetCurrentHealth() / currentTarget.GetMaxHealth());
        healthBarFill.fillAmount = fillAmount;
    }

    void UpdateTargetPosition()
    {
        if (targetTransform == null) return;

        Vector3 worldPosition = targetTransform.position + Vector3.up * 2.5f;
        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPosition);
        Vector3 currentPos = targetPanel.transform.position;

        targetPanel.transform.position = Vector3.Lerp(currentPos, screenPos, Time.deltaTime * 10f);
    }
}

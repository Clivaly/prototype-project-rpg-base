using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTargeting : MonoBehaviour
{
    [Header("Alvo atual")]
    private EnemyTarget currentTarget;

    [Header("Detecção")]
    public LayerMask enemyMask; // Layer que representa os inimigos

    [Header("UI Target")]
    public TargetUIController targetUI;

    private InputActions inputActions;

    void Awake()
    {
        inputActions = new InputActions();
    }

    void OnEnable()
    {
        inputActions.Enable();

        // Escuta o clique do mouse (para selecionar alvos)
        inputActions.Player.Click.performed += OnClick;

        // Escuta a tecla de cancelamento (ex: ESC)
        inputActions.Player.CancelTarget.performed += OnCancelTarget;
    }

    void OnDisable()
    {
        inputActions.Player.Click.performed -= OnClick;
        inputActions.Player.CancelTarget.performed -= OnCancelTarget;
        inputActions.Disable();
    }

    // Quando o jogador clica com o mouse
    void OnClick(InputAction.CallbackContext ctx)
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, enemyMask))
        {
            EnemyTarget enemy = hit.collider.GetComponentInParent<EnemyTarget>();

            if (enemy != null)
            {
                SelectTarget(enemy);
                return; // Impede que o clique "vaze" pra outras ações (ex: movimento)
            }
        }

        // Agora não desmarca mais o alvo ao clicar fora!
        // Isso evita que clique no chão cancele o alvo durante o combate
    }

    // Quando o jogador aperta a tecla ESC (CancelTarget)
    void OnCancelTarget(InputAction.CallbackContext ctx)
    {
        if (currentTarget != null)
        {
            ClearTarget(); // Se tem alvo, desmarca
        }
        else
        {
            Debug.Log("Abrir menu de pausa futuramente...");
            // Aqui no futuro você pode chamar: PauseManager.Instance.OpenPauseMenu();
        }
    }

    void SelectTarget(EnemyTarget newTarget)
    {
        // Remove highlight anterior
        if (currentTarget != null)
            currentTarget.Deselect();

        currentTarget = newTarget;
        currentTarget.Select();

        if (targetUI != null)
        {
            EnemyStats stats = newTarget.GetComponent<EnemyStats>();
            if (stats != null)
                targetUI.ShowTarget(stats);
        }
    }

    void ClearTarget()
    {
        if (currentTarget != null)
        {
            currentTarget.Deselect();
            currentTarget = null;

            // Esconde UI
            if (targetUI != null)
                targetUI.HideTarget();
        }
    }

}

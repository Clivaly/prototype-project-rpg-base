using UnityEngine;
using UnityEngine.InputSystem;

public class ClickToMoveInput : MonoBehaviour
{
    [Header("Camadas")]
    public LayerMask groundMask;  // Onde pode clicar para andar
    public LayerMask enemyMask;   // Layer de inimigos (para impedir movimento ao clicar neles)

    [Header("Referências")]
    public PlayerMotor motor;     // Motor de movimento (já está referenciado)

    private InputActions inputActions;

    void Awake()
    {
        inputActions = new InputActions();
    }

    void OnEnable()
    {
        inputActions.Player.Enable();

        inputActions.Player.Click.performed += OnClick;
        inputActions.Player.ToggleRun.performed += _ => motor.ToggleRun();
    }

    void OnDisable()
    {
        inputActions.Player.Click.performed -= OnClick;
        inputActions.Player.ToggleRun.performed -= _ => motor.ToggleRun();
        inputActions.Player.Disable();
    }

    void OnClick(InputAction.CallbackContext context)
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        // Primeiro: verifica se clicou num inimigo
        if (Physics.Raycast(ray, out RaycastHit enemyHit, 100f, enemyMask))
        {
            // Se clicou em inimigo, não faz nada (quem trata é o PlayerTargeting)
            return;
        }

        // Segundo: Se clicou no chão, move até lá
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundMask))
        {
            motor.WalkTo(hit.point); // Envia para o motor o ponto clicado
        }
    }
}

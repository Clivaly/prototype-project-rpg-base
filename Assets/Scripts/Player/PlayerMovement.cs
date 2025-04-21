using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public PlayerMotor motor; // Referência para o PlayerMotor (onde o movimento acontece)

    private InputActions inputActions; // Input Actions do novo sistema da Unity

    void Awake()
    {
        inputActions = new InputActions(); // Cria a instância do sistema de input
    }

    void OnEnable()
    {
        inputActions.Player.Enable(); // Ativa o mapa de ações "Player"

        // Quando apertar o botão de pulo, chama o método Jump() do motor
        inputActions.Player.Jump.performed += ctx => motor.Jump();

        // Quando apertar a tecla de alternar corrida (ex: "/"), muda o modo de correr
        inputActions.Player.ToggleRun.performed += ctx => motor.ToggleRun();
    }

    void OnDisable()
    {
        inputActions.Player.Disable(); // Desativa o input quando o script estiver inativo

        // Remove o evento de pulo pra evitar erros de memória
        inputActions.Player.Jump.performed -= ctx => motor.Jump();
    }

    void Update()
    {
        // Lê o input de movimento (WASD ou setas)
        Vector2 moveInput = inputActions.Player.Move.ReadValue<Vector2>();

        // Envia o comando de movimento para o motor (ele cuida da lógica toda)
        motor.Move(moveInput);
    }
}

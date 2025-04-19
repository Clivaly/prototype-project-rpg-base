using UnityEngine;
using UnityEngine.InputSystem;

public class ClickToMoveInput : MonoBehaviour
{
    public LayerMask groundMask;  // Máscara que define o que é considerado "chão"
    public PlayerMotor motor;     // Referência para o PlayerMotor (quem faz o personagem andar)

    private InputActions inputActions;

    void Awake()
    {
        inputActions = new InputActions(); // Cria as ações do sistema de input
    }

    void OnEnable()
    {
        inputActions.Player.Enable(); // Ativa os controles do jogador

        // Evento: quando clicar com o mouse, chama OnClick()
        inputActions.Player.Click.performed += OnClick;

        // Evento: quando apertar a tecla de correr/andar, troca o modo no motor
        inputActions.Player.ToggleRun.performed += _ => motor.ToggleRun();
    }

    void OnDisable()
    {
        inputActions.Player.Click.performed -= OnClick;
        inputActions.Player.ToggleRun.performed -= _ => motor.ToggleRun();
        inputActions.Player.Disable(); // Desativa as ações
    }

    void OnClick(InputAction.CallbackContext context)
    {
        // Faz um Raycast da posição do mouse na tela para o mundo 3D
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        // Se o raio bater em algo no "chão"
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundMask))
        {
            // Envia a posição clicada para o motor se mover até lá
            motor.WalkTo(hit.point);
        }
    }
}




///Versao antiga
//using UnityEngine;
//using UnityEngine.InputSystem;

//[RequireComponent(typeof(CharacterController))]
//public class ClickToMoveInput : MonoBehaviour
//{
//    [Header("Referências")]
//    public Animator animator; // arraste seu Animator no Inspector
//    public LayerMask groundMask;

//    [Header("Velocidade")]
//    public float moveSpeed = 5f;
//    public float stoppingDistance = 0.2f;

//    private CharacterController controller;
//    private Camera mainCamera;
//    private InputActions inputActions;

//    private Vector3 targetPosition;
//    private bool hasTarget = false;
//    private bool isWalking = false; // Alterna entre correr/andar

//    void Awake()
//    {
//        controller = GetComponent<CharacterController>();
//        mainCamera = Camera.main;
//        inputActions = new InputActions();
//    }

//    void OnEnable()
//    {
//        inputActions.Player.Enable();
//        inputActions.Player.Click.performed += OnClick;
//        inputActions.Player.ToggleRun.performed += _ => isWalking = !isWalking;
//    }

//    void OnDisable()
//    {
//        inputActions.Player.Click.performed -= OnClick;
//        inputActions.Player.ToggleRun.performed -= _ => isWalking = !isWalking;
//        inputActions.Player.Disable();
//    }

//    void Update()
//    {
//        if (hasTarget)
//            MoveToTarget();

//        // Se não está se movendo, seta velocidade 0
//        if (!hasTarget && animator != null)
//            animator.SetFloat("Speed", 0f);
//    }

//    void OnClick(InputAction.CallbackContext context)
//    {
//        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
//        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundMask))
//        {
//            targetPosition = hit.point;
//            hasTarget = true;
//        }
//    }

//    void MoveToTarget()
//    {
//        Vector3 direction = targetPosition - transform.position;
//        direction.y = 0f;
//        float distance = direction.magnitude;

//        if (distance < stoppingDistance)
//        {
//            hasTarget = false;
//            animator?.SetFloat("Speed", 0f);
//            return;
//        }

//        Vector3 moveDir = direction.normalized;
//        float speed = isWalking ? moveSpeed / 2f : moveSpeed;

//        controller.Move(moveDir * speed * Time.deltaTime);

//        // Rotaciona suavemente para direção do movimento
//        if (moveDir != Vector3.zero)
//        {
//            Quaternion lookRot = Quaternion.LookRotation(moveDir);
//            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 10f);
//        }

//        // Atualiza animação com base na velocidade real
//        if (animator != null)
//        {
//            float currentSpeed = controller.velocity.magnitude;
//            animator.SetFloat("Speed", currentSpeed / moveSpeed); // Normaliza entre 0 e 1
//        }
//    }
//}

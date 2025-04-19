using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public PlayerMotor motor; // Refer�ncia para o PlayerMotor (onde o movimento acontece)

    private InputActions inputActions; // Input Actions do novo sistema da Unity

    void Awake()
    {
        inputActions = new InputActions(); // Cria a inst�ncia do sistema de input
    }

    void OnEnable()
    {
        inputActions.Player.Enable(); // Ativa o mapa de a��es "Player"

        // Quando apertar o bot�o de pulo, chama o m�todo Jump() do motor
        inputActions.Player.Jump.performed += ctx => motor.Jump();

        // Quando apertar a tecla de alternar corrida (ex: "/"), muda o modo de correr
        inputActions.Player.ToggleRun.performed += ctx => motor.ToggleRun();
    }

    void OnDisable()
    {
        inputActions.Player.Disable(); // Desativa o input quando o script estiver inativo

        // Remove o evento de pulo pra evitar erros de mem�ria
        inputActions.Player.Jump.performed -= ctx => motor.Jump();
    }

    void Update()
    {
        // L� o input de movimento (WASD ou setas)
        Vector2 moveInput = inputActions.Player.Move.ReadValue<Vector2>();

        // Envia o comando de movimento para o motor (ele cuida da l�gica toda)
        motor.Move(moveInput);
    }
}




// Versao antiga
//using UnityEngine;
//using UnityEngine.InputSystem;

//[RequireComponent(typeof(CharacterController))]
//public class PlayerMovement : MonoBehaviour
//{
//    [Header("Refer�ncias")]
//    public Transform cameraTransform;           // A c�mera que define a dire��o de movimento
//    public Transform visualHolder;              // O objeto que inclina (tilt), ex: modelo 3D do personagem

//    [Header("Velocidade")]
//    public float moveSpeed = 7f;
//    public float acceleration = 3f;
//    public float deceleration = 15f;

//    [Header("Modos de movimento")]
//    public bool isRunning = true; // come�a correndo por padr�o

//    [Header("Rota��o")]
//    public float rotationSpeed = 10f;
//    public float tiltAmount = 2f;

//    [Header("Pulo")]
//    public float jumpForce = 8f;
//    public float gravity = -20f;

//    [Header("Camera")]
//    public Transform cameraFollowAnchor; // agora vai reconhecer

//    [Header("Anima��o")]
//    public PlayerAnimatorController animatorController;

//    private CharacterController controller;
//    private InputActions inputActions;

//    private Vector3 currentVelocity = Vector3.zero;
//    private float verticalVelocity = 0f;
//    private Vector2 moveInput;

//    void Awake()
//    {
//        inputActions = new InputActions();
//        controller = GetComponent<CharacterController>();
//    }

//    void OnEnable()
//    {
//        inputActions.Player.Enable();
//        inputActions.Player.Jump.performed += OnJump;
//        inputActions.Player.ToggleRun.performed += _ => isRunning = !isRunning;
//    }

//    void OnDisable()
//    {
//        inputActions.Player.Jump.performed -= OnJump;
//        inputActions.Player.Disable();
//    }

//    void Update()
//    {
//        moveInput = inputActions.Player.Move.ReadValue<Vector2>();
//        // C�lculo da dire��o baseado na c�mera
//        Vector3 forward = cameraTransform.forward;
//        Vector3 right = cameraTransform.right;
//        forward.y = 0f;
//        right.y = 0f;
//        forward.Normalize();
//        right.Normalize();

//        Vector3 desiredDirection = (forward * moveInput.y + right * moveInput.x).normalized;

//        if (desiredDirection.magnitude > 0.1f)
//        {
//            float targetSpeed = isRunning ? moveSpeed : (moveSpeed / 2f);
//            currentVelocity = Vector3.Lerp(currentVelocity, desiredDirection * targetSpeed, acceleration * Time.deltaTime);

//            // Rota��o
//            Quaternion targetRotation = Quaternion.LookRotation(desiredDirection);
//            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
//        }
//        else
//        {
//            currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, deceleration * Time.deltaTime);
//        }

//        // Atualiza anima��o com valor m�nimo de corte
//        float speedPercent = currentVelocity.magnitude / moveSpeed;

//        // Corrige falsos positivos de movimento (anima��o continuando mesmo parado)
//        if (moveInput == Vector2.zero && currentVelocity.magnitude < 0.1f)
//            speedPercent = 0f;

//        //if (speedPercent < 0.05f) 
//        //    speedPercent = 0f;

//        if (animatorController != null)
//            animatorController.UpdateAnimation(speedPercent);

//        // Pulo e gravidade
//        if (controller.isGrounded && verticalVelocity < 0)
//            verticalVelocity = -2f;
//        else
//            verticalVelocity += gravity * Time.deltaTime;

//        Vector3 finalVelocity = currentVelocity;
//        finalVelocity.y = verticalVelocity;

//        controller.Move(finalVelocity * Time.deltaTime);

//        ApplyTilt();
//    }

//    void LateUpdate()
//    {
//        if (cameraFollowAnchor != null)
//            cameraFollowAnchor.rotation = Quaternion.identity;
//    }

//    void OnJump(InputAction.CallbackContext context)
//    {
//        if (controller.isGrounded)
//        {
//            verticalVelocity = jumpForce;

//            if (animatorController != null)
//                animatorController.TriggerJump();
//        }
//    }

//    void ApplyTilt()
//    {
//        if (visualHolder == null || currentVelocity.magnitude < 0.1f)
//            return;

//        Vector3 localVelocity = transform.InverseTransformDirection(currentVelocity);

//        // Inclina��o frente e tras
//        //float tiltX = Mathf.Clamp(-localVelocity.z * tiltAmount, -tiltAmount, tiltAmount);
//        // Inclina��o apenas lateral (estilo OSRS)
//        float tiltZ = Mathf.Clamp(localVelocity.x * -tiltAmount * 0.3f, -tiltAmount, tiltAmount);

//        Quaternion tiltRotation = Quaternion.Euler(0, 0, tiltZ);

//        visualHolder.localRotation = Quaternion.Slerp(visualHolder.localRotation, tiltRotation, Time.deltaTime * 1f);
//    }
//}

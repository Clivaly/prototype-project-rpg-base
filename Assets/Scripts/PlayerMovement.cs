using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Refer�ncias")]
    public Transform cameraTransform;

    [Header("Velocidade")]
    public float moveSpeed = 5f;
    public float acceleration = 10f;
    public float deceleration = 15f;

    [Header("Rota��o")]
    public float rotationSpeed = 10f;
    public float tiltAmount = 10f;

    [Header("Pulo")]
    public float jumpForce = 8f;
    public float gravity = -20f;

    private CharacterController controller;
    private InputActions inputActions;

    private Vector3 currentVelocity = Vector3.zero;     // Velocidade horizontal
    private float verticalVelocity = 0f;                // Velocidade vertical (pulo e gravidade)
    private Vector2 moveInput;

    void Awake()
    {
        inputActions = new InputActions();
        controller = GetComponent<CharacterController>();
    }

    void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Jump.performed += OnJump; // Escuta o evento de pulo
    }

    void OnDisable()
    {
        inputActions.Player.Jump.performed -= OnJump;
        inputActions.Player.Disable();
    }

    void Update()
    {
        // Input do jogador (teclado ou controle)
        moveInput = inputActions.Player.Move.ReadValue<Vector2>();

        // Dire��es com base na c�mera
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // Dire��o de movimento desejada
        Vector3 desiredDirection = (forward * moveInput.y + right * moveInput.x).normalized;

        // Decide se acelera ou freia
        if (desiredDirection.magnitude > 0.1f)
        {
            currentVelocity = Vector3.MoveTowards(currentVelocity, desiredDirection * moveSpeed, acceleration * Time.deltaTime);

            // Rotaciona para a dire��o do movimento
            Quaternion targetRotation = Quaternion.LookRotation(desiredDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, deceleration * Time.deltaTime);
        }

        // Gravidade
        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f; // Mant�m no ch�o sem "grudar"
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        // Combina movimento horizontal com vertical
        Vector3 finalVelocity = currentVelocity;
        finalVelocity.y = verticalVelocity;

        // Move o personagem
        controller.Move(finalVelocity * Time.deltaTime);

        // Inclina��o (tilt)
        ApplyTilt();
    }

    void OnJump(InputAction.CallbackContext context)
    {
        // S� pula se estiver no ch�o
        if (controller.isGrounded)
        {
            verticalVelocity = jumpForce;
        }
    }

    void ApplyTilt()
    {
        if (currentVelocity.magnitude > 0.1f)
        {
            Vector3 localVelocity = transform.InverseTransformDirection(currentVelocity);
            float tiltX = -localVelocity.z * tiltAmount;
            float tiltZ = localVelocity.x * tiltAmount;
            Quaternion tiltRotation = Quaternion.Euler(tiltX, transform.eulerAngles.y, tiltZ);
            transform.rotation = Quaternion.Slerp(transform.rotation, tiltRotation, Time.deltaTime * 5f);
        }
    }
}



// Vers�o com Lerp
//using UnityEngine;
//using UnityEngine.InputSystem;

//[RequireComponent(typeof(CharacterController))]
//public class PlayerMovement : MonoBehaviour
//{
//    public Transform cameraTransform;
//    public float moveSpeed = 5f;
//    public float rotationSpeed = 1f;
//    public float tiltAmount = 15f;
//    public float tiltSpeed = 5f;
//    public float acceleration = 6f;     // Quanto mais alto, mais r�pido acelera
//    public float deceleration = 10f;     // Quanto mais alto, mais r�pido freia

//    private CharacterController controller;
//    private InputActions inputActions;
//    private Vector3 currentVelocity = Vector3.zero; // Velocidade atual aplicada
//    private float currentTilt = 0f;

//    void Awake()
//    {
//        inputActions = new InputActions();
//        controller = GetComponent<CharacterController>();
//    }

//    void OnEnable()
//    {
//        inputActions.Player.Enable();
//    }

//    void OnDisable()
//    {
//        inputActions.Player.Disable();
//    }

//    void Update()
//    {
//        Vector2 moveInput = inputActions.Player.Move.ReadValue<Vector2>();

//        Vector3 forward = cameraTransform.forward;
//        Vector3 right = cameraTransform.right;

//        forward.y = 0;
//        right.y = 0;
//        forward.Normalize();
//        right.Normalize();

//        // Dire��o desejada baseada no input e c�mera
//        Vector3 targetDirection = forward * moveInput.y + right * moveInput.x;

//        // Decide se deve acelerar ou frear
//        float speedLerp = (targetDirection.sqrMagnitude > 0.01f) ? acceleration : deceleration;

//        // Acelera ou desacelera suavemente a velocidade atual
//        currentVelocity = Vector3.Lerp(currentVelocity, targetDirection * moveSpeed, Time.deltaTime * speedLerp);

//        // Move usando a velocidade atual suavizada
//        controller.Move(currentVelocity * Time.deltaTime);

//        // Gira o personagem na dire��o do movimento, se estiver se movendo
//        if (currentVelocity.sqrMagnitude > 0.01f)
//        {
//            Quaternion targetRotation = Quaternion.LookRotation(currentVelocity);
//            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
//        }

//        // Inclina��o lateral (tilt)
//        float targetTilt = -moveInput.x * tiltAmount;
//        currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.deltaTime * tiltSpeed);
//        ApplyTilt();
//    }

//    void ApplyTilt()
//    {
//        Quaternion currentRotation = transform.rotation;
//        Quaternion tiltRotation = Quaternion.Euler(0f, currentRotation.eulerAngles.y, currentTilt);
//        transform.rotation = tiltRotation;
//    }
//}


// Vers�o com MoveTowards
//using UnityEngine;
//using UnityEngine.InputSystem;

//[RequireComponent(typeof(CharacterController))]
//public class PlayerMovement : MonoBehaviour
//{
//    [Header("Refer�ncias")]
//    public Transform cameraTransform;

//    [Header("Velocidade")]
//    public float moveSpeed = 5f;
//    public float acceleration = 10f;
//    public float deceleration = 15f;

//    [Header("Rota��o")]
//    public float rotationSpeed = 10f;
//    public float tiltAmount = 10f; // �ngulo m�ximo de inclina��o do cubo

//    private CharacterController controller;
//    private InputActions inputActions;

//    private Vector3 currentVelocity = Vector3.zero; // Velocidade atual
//    private Vector2 moveInput; // Input vindo do teclado/controlador

//    void Awake()
//    {
//        inputActions = new InputActions();
//        controller = GetComponent<CharacterController>();
//    }

//    void OnEnable()
//    {
//        inputActions.Player.Enable(); // Ativa os inputs do novo sistema
//    }

//    void OnDisable()
//    {
//        inputActions.Player.Disable(); // Desativa ao sair
//    }

//    void Update()
//    {
//        // 1. L� o input (vetor 2D: X = esquerda/direita, Y = frente/tr�s)
//        moveInput = inputActions.Player.Move.ReadValue<Vector2>();

//        // 2. Calcula dire��o com base na c�mera
//        Vector3 forward = cameraTransform.forward;
//        Vector3 right = cameraTransform.right;

//        // 3. Remove inclina��o da c�mera (mant�m movimento no plano XZ)
//        forward.y = 0f;
//        right.y = 0f;
//        forward.Normalize();
//        right.Normalize();

//        // 4. Dire��o desejada (normalizada pra n�o somar mais de 1)
//        Vector3 desiredDirection = (forward * moveInput.y + right * moveInput.x).normalized;

//        // 5. Decide entre acelerar ou frear (se tem input)
//        if (desiredDirection.magnitude > 0.1f)
//        {
//            // Acelera suavemente na dire��o desejada
//            currentVelocity = Vector3.MoveTowards(currentVelocity, desiredDirection * moveSpeed, acceleration * Time.deltaTime);

//            // Rotaciona suavemente para a dire��o de movimento
//            Quaternion targetRotation = Quaternion.LookRotation(desiredDirection);
//            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
//        }
//        else
//        {
//            // Freia suavemente quando solta o input
//            currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, deceleration * Time.deltaTime);
//        }

//        // 6. Move o personagem com o CharacterController
//        controller.Move(currentVelocity * Time.deltaTime);

//        // 7. Inclina o cubo levemente (tilt visual)
//        ApplyTilt();
//    }

//    void ApplyTilt()
//    {
//        // Inclina��o com base na dire��o atual (eixo local)
//        if (currentVelocity.magnitude > 0.1f)
//        {
//            // Converte a velocidade global para local (pra saber se t� indo pra frente, lado, etc)
//            Vector3 localVelocity = transform.InverseTransformDirection(currentVelocity);

//            // Inclina��o nos eixos X e Z com base na dire��o local
//            float tiltX = -localVelocity.z * tiltAmount; // frente/tr�s
//            float tiltZ = localVelocity.x * tiltAmount;  // esquerda/direita

//            // Aplica rota��o inclinada suavemente
//            Quaternion tiltRotation = Quaternion.Euler(tiltX, transform.eulerAngles.y, tiltZ);
//            transform.rotation = Quaternion.Slerp(transform.rotation, tiltRotation, Time.deltaTime * 5f);
//        }
//    }
//}





//using UnityEngine;
//using UnityEngine.InputSystem;

//[RequireComponent(typeof(CharacterController))]
//public class PlayerMovement : MonoBehaviour
//{
//    public Transform cameraTransform;
//    public float moveSpeed = 5f;
//    public float acceleration = 10f;
//    public float deceleration = 15f;
//    public float rotationSpeed = 2f;

//    private CharacterController controller;
//    private InputActions inputActions;

//    private Vector3 currentVelocity = Vector3.zero; // velocidade atual
//    private Vector2 moveInput; // input do novo sistema

//    void Awake()
//    {
//        inputActions = new InputActions();
//        controller = GetComponent<CharacterController>();
//    }

//    void OnEnable()
//    {
//        inputActions.Player.Enable();
//    }

//    void OnDisable()
//    {
//        inputActions.Player.Disable();
//    }

//    void Update()
//    {
//        // L� input
//        moveInput = inputActions.Player.Move.ReadValue<Vector2>();

//        // Calcula dire��o com base na c�mera
//        Vector3 forward = cameraTransform.forward;
//        Vector3 right = cameraTransform.right;

//        forward.y = 0f;
//        right.y = 0f;
//        forward.Normalize();
//        right.Normalize();

//        Vector3 desiredDirection = (forward * moveInput.y + right * moveInput.x).normalized;

//        // Decide entre acelerar ou frear
//        if (desiredDirection.magnitude > 0.1f)
//        {
//            currentVelocity = Vector3.MoveTowards(currentVelocity, desiredDirection * moveSpeed, acceleration * Time.deltaTime);

//            // Rotaciona suavemente o cubo
//            Quaternion targetRotation = Quaternion.LookRotation(desiredDirection);
//            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
//        }
//        else
//        {
//            currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, deceleration * Time.deltaTime);
//        }

//        // Aplica movimento
//        controller.Move(currentVelocity * Time.deltaTime);
//    }
//}

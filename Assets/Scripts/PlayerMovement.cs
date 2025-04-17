using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Referências")]
    public Transform cameraTransform;           // A câmera que define a direção de movimento
    public Transform visualHolder;              // O objeto que inclina (tilt), ex: modelo 3D do personagem

    [Header("Velocidade")]
    public float moveSpeed = 5f;
    public float acceleration = 10f;
    public float deceleration = 15f;

    [Header("Rotação")]
    public float rotationSpeed = 10f;
    public float tiltAmount = 10f;

    [Header("Pulo")]
    public float jumpForce = 8f;
    public float gravity = -20f;

    [Header("Camera")]
    public Transform cameraFollowAnchor; // agora vai reconhecer

    private CharacterController controller;
    private InputActions inputActions;

    private Vector3 currentVelocity = Vector3.zero;
    private float verticalVelocity = 0f;
    private Vector2 moveInput;

    //private float rotationVelocity;

    void Awake()
    {
        inputActions = new InputActions();
        controller = GetComponent<CharacterController>();
    }

    void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Jump.performed += OnJump;
    }

    void OnDisable()
    {
        inputActions.Player.Jump.performed -= OnJump;
        inputActions.Player.Disable();
    }

    //void Update()
    //{
    //    // 1. Lê o input do jogador
    //    moveInput = inputActions.Player.Move.ReadValue<Vector2>();

    //    // 2. Direção baseada na câmera
    //    Vector3 forward = cameraTransform.forward;
    //    Vector3 right = cameraTransform.right;
    //    forward.y = 0f;
    //    right.y = 0f;
    //    forward.Normalize();
    //    right.Normalize();

    //    Vector3 desiredDirection = (forward * moveInput.y + right * moveInput.x).normalized;

    //    // 3. Movimento horizontal
    //    if (desiredDirection.magnitude > 0.1f)
    //    {
    //        currentVelocity = Vector3.Lerp(currentVelocity, desiredDirection * moveSpeed, acceleration * Time.deltaTime);

    //        // ROTACAO SUAVE SEM TRANCO
    //        Vector3 currentEuler = transform.eulerAngles;
    //        float targetY = Mathf.Atan2(desiredDirection.x, desiredDirection.z) * Mathf.Rad2Deg;
    //        float smoothedY = Mathf.SmoothDampAngle(currentEuler.y, targetY, ref rotationVelocity, 0.1f); // suavização de 0.1s
    //        transform.rotation = Quaternion.Euler(0f, smoothedY, 0f);
    //    }
    //    else
    //    {
    //        currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, deceleration * Time.deltaTime);
    //    }

    //    // 4. Gravidade
    //    if (controller.isGrounded && verticalVelocity < 0)
    //    {
    //        verticalVelocity = -2f;
    //    }
    //    else
    //    {
    //        verticalVelocity += gravity * Time.deltaTime;
    //    }

    //    // 5. Combina movimentação
    //    Vector3 finalVelocity = currentVelocity;
    //    finalVelocity.y = verticalVelocity;

    //    controller.Move(finalVelocity * Time.deltaTime);

    //    // 6. Inclinação visual (Tilt)
    //    ApplyTilt();
    //}


    void Update()
    {


        moveInput = inputActions.Player.Move.ReadValue<Vector2>();

        // Define direção com base na câmera
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 desiredDirection = (forward * moveInput.y + right * moveInput.x).normalized;

        if (desiredDirection.magnitude > 0.1f)
        {
            currentVelocity = Vector3.Lerp(currentVelocity, desiredDirection * moveSpeed, acceleration * Time.deltaTime);

            // Rotaciona o player na direção do movimento
            Quaternion targetRotation = Quaternion.LookRotation(desiredDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, deceleration * Time.deltaTime);
        }

        // Pulo e gravidade
        if (controller.isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;
        else
            verticalVelocity += gravity * Time.deltaTime;

        Vector3 finalVelocity = currentVelocity;
        finalVelocity.y = verticalVelocity;

        controller.Move(finalVelocity * Time.deltaTime);

        ApplyTilt();
    }

    void LateUpdate()
    {
        if (cameraFollowAnchor != null)
            cameraFollowAnchor.rotation = Quaternion.identity;
    }


    void OnJump(InputAction.CallbackContext context)
    {
        if (controller.isGrounded)
            verticalVelocity = jumpForce;
    }

    void ApplyTilt()
    {
        if (visualHolder == null || currentVelocity.magnitude < 0.1f)
            return;

        Vector3 localVelocity = transform.InverseTransformDirection(currentVelocity);

        // Inclinação frente e tras
        //float tiltX = Mathf.Clamp(-localVelocity.z * tiltAmount, -tiltAmount, tiltAmount);
        // Inclinação apenas lateral (estilo OSRS)
        float tiltZ = Mathf.Clamp(localVelocity.x * tiltAmount, -tiltAmount, tiltAmount);

        Quaternion tiltRotation = Quaternion.Euler(0, 0, tiltZ);
        //Quaternion tiltRotation = Quaternion.Euler(tiltX, 0, tiltZ);

        visualHolder.localRotation = Quaternion.Slerp(visualHolder.localRotation, tiltRotation, Time.deltaTime * 3f);
    }

}

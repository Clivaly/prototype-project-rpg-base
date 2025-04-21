using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMotor : MonoBehaviour
{
    [Header("Refer�ncias")]
    public Transform cameraTransform;           // C�mera usada pra calcular dire��o do movimento
    public Transform visualHolder;              // Parte visual que inclina com movimento (Tilt)
    public PlayerAnimatorController animatorController; // Controlador de anima��es

    [Header("Movimento")]
    public float moveSpeed = 6f;                // Velocidade de corrida
    public float acceleration = 3f;            // Acelera��o do movimento
    public float deceleration = 12f;            // Desacelera��o quando para
    public float rotationSpeed = 10f;           // Velocidade de rota��o do corpo
    public float jumpForce = 8f;                // For�a do pulo
    public float gravity = -20f;                // For�a da gravidade
    public float tiltAmount = 2f;               // Inclina��o visual (Tilt)

    private CharacterController controller;     // Componente que move o personagem
    private Vector3 currentVelocity = Vector3.zero; // Velocidade atual (X/Z)
    private float verticalVelocity = 0f;        // Velocidade vertical (Y)
    private bool isRunning = true;              // Alterna entre andar/correr
    private bool isMovingToTarget = false;      // Flag se est� indo at� um ponto clicado
    private Vector3 targetPosition;             // Ponto que foi clicado para andar at� l�

    void Awake()
    {
        controller = GetComponent<CharacterController>(); // Pega o CharacterController
    }

    void Update()
    {
        if (isMovingToTarget)
            HandleClickMovement(); // Se est� em modo clique, move at� o ponto

        ApplyGravity();  // Aplica gravidade
        ApplyTilt();     // Aplica inclina��o visual (Tilt)
    }

    public void ToggleRun()
    {
        isRunning = !isRunning; // Alterna entre correr/andar
    }

    public void Move(Vector2 input)
    {
        // Calcula a dire��o com base na c�mera
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 direction = (forward * input.y + right * input.x).normalized;

        if (direction.magnitude > 0.1f)
        {
            float targetSpeed = isRunning ? moveSpeed : moveSpeed / 2f;

            // Suaviza o movimento at� a velocidade desejada
            currentVelocity = Vector3.Lerp(currentVelocity, direction * targetSpeed, acceleration * Time.deltaTime);

            // Rotaciona o personagem para a dire��o de movimento
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
        }
        else
        {
            // Suaviza a parada
            currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, deceleration * Time.deltaTime);
        }

        UpdateAnimation(); // Atualiza a anima��o com base na velocidade

        // Junta velocidade vertical e horizontal
        Vector3 velocity = currentVelocity;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime); // Move o personagem
    }

    public void WalkTo(Vector3 point)
    {
        targetPosition = point;       // Guarda o ponto clicado
        isMovingToTarget = true;      // Ativa o modo de andar at� ele
    }

    private void HandleClickMovement()
    {
        Vector3 dir = targetPosition - transform.position;
        dir.y = 0f;
        float dist = dir.magnitude;

        // Se chegou bem perto, para de andar
        if (dist < 0.2f)
        {
            isMovingToTarget = false;

            // Zera velocidade real e visual
            currentVelocity = Vector3.zero;

            // Garante que o CharacterController pare
            controller.Move(Vector3.zero);

            // Atualiza a anima��o para "parado"
            animatorController?.UpdateAnimation(0f);

            return;
        }

        float targetSpeed = isRunning ? moveSpeed : moveSpeed / 2f;
        Vector3 moveDir = dir.normalized;
        controller.Move(moveDir * targetSpeed * Time.deltaTime);

        // Rotaciona suavemente para o ponto
        if (moveDir != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * rotationSpeed);
        }

        currentVelocity = moveDir * targetSpeed;
        UpdateAnimation();
    }

    public void Jump()
    {
        if (controller.isGrounded)
        {
            verticalVelocity = jumpForce;

            if (animatorController != null)
                animatorController.TriggerJump(); // Ativa a anima��o de pulo
        }
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f; // Mant�m colado no ch�o
        else
            verticalVelocity += gravity * Time.deltaTime;
    }

    private void ApplyTilt()
    {
        if (visualHolder == null || currentVelocity.magnitude < 0.1f)
            return;

        Vector3 localVelocity = transform.InverseTransformDirection(currentVelocity);
        float tiltZ = Mathf.Clamp(localVelocity.x * -tiltAmount * 0.3f, -tiltAmount, tiltAmount);
        Quaternion tiltRot = Quaternion.Euler(0, 0, tiltZ);
        visualHolder.localRotation = Quaternion.Slerp(visualHolder.localRotation, tiltRot, Time.deltaTime * 2f);
    }

    private void UpdateAnimation()
    {
        if (animatorController != null)
        {
            float speedPercent = currentVelocity.magnitude / moveSpeed;

            // Arredonda valores muito pequenos para 0 (evita ficar "correndo" com 0.01f)
            if (speedPercent < 0.05f)
                speedPercent = 0f;

            animatorController?.UpdateAnimation(speedPercent);
        }
    }

}

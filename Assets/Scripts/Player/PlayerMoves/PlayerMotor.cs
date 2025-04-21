using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMotor : MonoBehaviour
{
    [Header("Referências")]
    public Transform cameraTransform;           // Câmera usada pra calcular direção do movimento
    public Transform visualHolder;              // Parte visual que inclina com movimento (Tilt)
    public PlayerAnimatorController animatorController; // Controlador de animações

    [Header("Movimento")]
    public float moveSpeed = 6f;                // Velocidade de corrida
    public float acceleration = 3f;            // Aceleração do movimento
    public float deceleration = 12f;            // Desaceleração quando para
    public float rotationSpeed = 10f;           // Velocidade de rotação do corpo
    public float jumpForce = 8f;                // Força do pulo
    public float gravity = -20f;                // Força da gravidade
    public float tiltAmount = 2f;               // Inclinação visual (Tilt)

    private CharacterController controller;     // Componente que move o personagem
    private Vector3 currentVelocity = Vector3.zero; // Velocidade atual (X/Z)
    private float verticalVelocity = 0f;        // Velocidade vertical (Y)
    private bool isRunning = true;              // Alterna entre andar/correr
    private bool isMovingToTarget = false;      // Flag se está indo até um ponto clicado
    private Vector3 targetPosition;             // Ponto que foi clicado para andar até lá

    void Awake()
    {
        controller = GetComponent<CharacterController>(); // Pega o CharacterController
    }

    void Update()
    {
        if (isMovingToTarget)
            HandleClickMovement(); // Se está em modo clique, move até o ponto

        ApplyGravity();  // Aplica gravidade
        ApplyTilt();     // Aplica inclinação visual (Tilt)
    }

    public void ToggleRun()
    {
        isRunning = !isRunning; // Alterna entre correr/andar
    }

    public void Move(Vector2 input)
    {
        // Calcula a direção com base na câmera
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

            // Suaviza o movimento até a velocidade desejada
            currentVelocity = Vector3.Lerp(currentVelocity, direction * targetSpeed, acceleration * Time.deltaTime);

            // Rotaciona o personagem para a direção de movimento
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
        }
        else
        {
            // Suaviza a parada
            currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, deceleration * Time.deltaTime);
        }

        UpdateAnimation(); // Atualiza a animação com base na velocidade

        // Junta velocidade vertical e horizontal
        Vector3 velocity = currentVelocity;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime); // Move o personagem
    }

    public void WalkTo(Vector3 point)
    {
        targetPosition = point;       // Guarda o ponto clicado
        isMovingToTarget = true;      // Ativa o modo de andar até ele
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

            // Atualiza a animação para "parado"
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
                animatorController.TriggerJump(); // Ativa a animação de pulo
        }
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f; // Mantém colado no chão
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

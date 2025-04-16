using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Transform cameraTransform;
    public float moveSpeed = 5f;

    private CharacterController controller;
    private InputActions inputActions;

    void Awake()
    {
        inputActions = new InputActions();
        controller = GetComponent<CharacterController>();
    }

    void OnEnable()
    {
        inputActions.Player.Enable();
    }

    void OnDisable()
    {
        inputActions.Player.Disable();
    }

    void Update()
    {
        // L� o input diretamente a cada frame
        Vector2 moveInput = inputActions.Player.Move.ReadValue<Vector2>();

        // Se n�o houver input, sai
        if (moveInput.sqrMagnitude < 0.01f) return;

        // Orienta��o da c�mera
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // Remove componente vertical
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        // Dire��o final
        Vector3 moveDirection = forward * moveInput.y + right * moveInput.x;

        // Move o personagem
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);

        // Gira o personagem na dire��o que est� andando
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2f);
        }
    }
}

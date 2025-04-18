using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerManager : MonoBehaviour
{
    public ClickToMoveInput clickToMove;
    public PlayerMovement playerMovement;

    private InputActions inputActions;
    private bool isClickToMoveActive = false;

    void Awake()
    {
        inputActions = new InputActions();
    }

    void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.ToggleMode.performed += _ => ToggleMode();
    }

    void OnDisable()
    {
        inputActions.Player.ToggleMode.performed -= _ => ToggleMode();
        inputActions.Disable();
    }

    void Start()
    {
        ApplyControlMode();
    }

    void ToggleMode()
    {
        isClickToMoveActive = !isClickToMoveActive;
        ApplyControlMode();
    }

    void ApplyControlMode()
    {
        if (clickToMove != null)
            clickToMove.enabled = isClickToMoveActive;

        if (playerMovement != null)
            playerMovement.enabled = !isClickToMoveActive;

        Debug.Log("Modo atual: " + (isClickToMoveActive ? "Click to Move" : "WASD"));
    }
}

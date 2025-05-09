using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Input Action Asset")]
    [SerializeField]
    private InputActionAsset playerControls;

    [Header("Action Map Name Reference")]
    [SerializeField]
    private string actionMapName = "Player";

    [Header("Action Name Reference")]
    [SerializeField]
    private string movement = "Move";

    [SerializeField]
    private string rotation = "Look";

    [SerializeField]
    private string jump = "Jump";

    [SerializeField]
    private string sprint = "Sprint";

    [SerializeField]
    private string crouch = "Crouch";

    private InputAction MovementAction;
    private InputAction RotationAction;
    private InputAction JumpAction;
    private InputAction SprintAction;
    private InputAction CrouchAction;

    public Vector2 MovementInput { get; private set; }
    public Vector2 RotationInput { get; private set; }
    public bool JumpTriggered { get; private set; }
    public bool SprintTriggered { get; private set; }
    public bool CrouchTriggered { get; private set; }

    private void Awake()
    {
        InputActionMap mapReference = playerControls.FindActionMap(actionMapName);
        MovementAction = mapReference.FindAction(movement);
        RotationAction = mapReference.FindAction(rotation);
        JumpAction = mapReference.FindAction(jump);
        SprintAction = mapReference.FindAction(sprint);
        CrouchAction = mapReference.FindAction(crouch);
        SubscribeActionValuesToInputEvents();
    }

    private void SubscribeActionValuesToInputEvents()
    {
        MovementAction.performed += inputInfo => MovementInput = inputInfo.ReadValue<Vector2>();
        MovementAction.canceled += inputInfo => MovementInput = Vector2.zero;

        RotationAction.performed += inputInfo => RotationInput = inputInfo.ReadValue<Vector2>();
        RotationAction.canceled += inputInfo => RotationInput = Vector2.zero;

        JumpAction.performed += inputInfo => JumpTriggered = true;
        JumpAction.canceled += inputInfo => JumpTriggered = false;

        SprintAction.performed += inputInfo => SprintTriggered = !SprintTriggered;

        CrouchAction.performed += inputInfo => CrouchTriggered = !CrouchTriggered;
    }

    private void OnEnable()
    {
        playerControls.FindActionMap(actionMapName).Enable();
    }

    private void OnDisable()
    {
        playerControls.FindActionMap(actionMapName).Disable();
    }
}

using UnityEngine;


public enum MovementState
{
    Walking,
    Running,
    Jumping,
    Crouching
}

public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField]
    private float walkSpeed = 3.0f;
    [SerializeField]
    private float sprintMultiplier = 2.0f;
    [SerializeField]
    private float crouchMultiplier = 0.5f;

    public bool Spinting => playerInputHandler.SprintTriggered;

    [Header("Jump Parameters")]
    [SerializeField]
    private float jumpForce = 5.0f;
    [SerializeField]
    private float gravityMultiplier = 1.0f;

    [Header("Look Parameters")]
    [SerializeField]
    private float mouseSensitivity = 1.0f;
    [SerializeField]
    private float upDownLookRange = 80f;

    [Header("Crouching")]
    [SerializeField]
    private float crouchHeight = 1.2f;
    [SerializeField]
    private Vector3 crouchCenter = new(0,0.595f,0);
    [SerializeField]
    private float standHeight;
    private Vector3 standCenter;
    public bool Crouching => playerInputHandler.CrouchTriggered;

    [Header("Values")]
    public MovementState State;


    [Header("References")]
    [SerializeField]
    private CharacterController characterController;
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private PlayerInputHandler playerInputHandler;

    private Vector3 currentMovement;
    private float verticalRotation;

    private float CurrentSpeed => walkSpeed * (playerInputHandler.SprintTriggered ? sprintMultiplier : playerInputHandler.CrouchTriggered ? crouchMultiplier : 1);


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //Crouch values
        standCenter = characterController.center;
        standHeight = characterController.height;
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    private Vector3 CalculateWorldDirection() 
    {
        Vector3 inputDirection = new Vector3(playerInputHandler.MovementInput.x, 0f, playerInputHandler.MovementInput.y);

        Vector3 worldDirection = transform.TransformDirection(inputDirection);

        return worldDirection.normalized;
    }

    private void HandleJumping() 
    {
        if (characterController.isGrounded)
        {
            currentMovement.y = -0.5f;

            //animator.IsInTransition()
            if (playerInputHandler.JumpTriggered && !playerInputHandler.CrouchTriggered) 
            {
                currentMovement.y = jumpForce;
            }
        }
        else 
        {
            currentMovement.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
        }
    }

    private void HandleMovement() 
    {
        Vector3 worldDirection = CalculateWorldDirection();
        currentMovement.x = worldDirection.x * CurrentSpeed;
        currentMovement.z = worldDirection.z * CurrentSpeed;

        HandleJumping();
        HandleCrouch();
        characterController.Move(currentMovement * Time.deltaTime);
    }

    private void ApplyHorizontalRotation(float rotationAmount) 
    {
        transform.Rotate(0, rotationAmount, 0);
    }

    private void ApplyVerticalRotation(float rotationAmount) 
    {
        verticalRotation = Mathf.Clamp(verticalRotation - rotationAmount, -upDownLookRange, upDownLookRange);
        mainCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    private void HandleRotation() 
    {
        float mouseXRotation = playerInputHandler.RotationInput.x * mouseSensitivity;
        float mouseYRotation = playerInputHandler.RotationInput.y * mouseSensitivity;

        ApplyHorizontalRotation(mouseXRotation);
        ApplyVerticalRotation(mouseYRotation);
    }

    private void HandleCrouch() 
    {
        if (characterController.isGrounded) 
        {
            Vector3 targetCenter = standCenter;
            float targetHeight = standHeight;

            if (playerInputHandler.CrouchTriggered && !playerInputHandler.SprintTriggered) 
            {
                targetCenter = crouchCenter;
                targetHeight = crouchHeight;
            }
            characterController.height = targetHeight;
            characterController.center = targetCenter;
        }
    }

}

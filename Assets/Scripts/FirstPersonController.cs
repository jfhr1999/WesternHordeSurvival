using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;


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
    [SerializeField]
    private float animationBlendSpeed = 5.9f;

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
    private Vector3 crouchCenter = new(0, 0.595f, 0);
    [SerializeField]
    private float standHeight;
    private Vector3 standCenter;

    private bool IsCrouching = false;
    private float CurrentTargetHeight;
    private Vector3 CurrentTargetCenter;
    private float CurrentCameraPosition;

    [Header("Combat")]
    [SerializeField]
    private Weapon CurrentWeapon;



    [Header("Values")]
    public MovementState State;


    [Header("References")]
    [SerializeField]
    private CharacterController characterController;
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private PlayerInputHandler playerInputHandler;
    [SerializeField]
    private CharacterAnimationController characterAnimationController;

    private Vector3 currentMovement;
    private float verticalRotation;
    private float startingCameraPostition;
    

    private float CurrentSpeed => walkSpeed * (playerInputHandler.SprintTriggered ? sprintMultiplier : playerInputHandler.CrouchTriggered ? crouchMultiplier : 1);

    //Jump Auxiliary
    private bool applyJump = false;
    public bool ApplyJump { get => applyJump; set => applyJump = value; }


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //Crouch values
        standCenter = characterController.center;
        standHeight = characterController.height;
        startingCameraPostition = mainCamera.transform.position.y;
        CurrentCameraPosition = startingCameraPostition;
        CurrentTargetHeight = standHeight;
        CurrentTargetCenter = standCenter;
        playerInputHandler.AttackAction.performed += OnAttackPerformed;
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    private Vector3 CalculateWorldDirection() 
    {
        Vector3 inputDirection = new Vector3(playerInputHandler.MovementInput.x, 0f, playerInputHandler.MovementInput.y);
        inputDirection = Vector3.ClampMagnitude(inputDirection, CurrentSpeed);

        Vector3 worldDirection = transform.TransformDirection(inputDirection);

        return worldDirection.normalized;
    }

    private void HandleJumping() 
    {
        if (characterController.isGrounded)
        {
            currentMovement.y = -0.5f;

            if (playerInputHandler.JumpTriggered && !playerInputHandler.CrouchTriggered && !IsCrouching) 
            {
                characterAnimationController.Jump();
                
            }
            else if (!playerInputHandler.CrouchTriggered && !IsCrouching)
            {
                ExecuteJump();
            }
        }
        else 
        {
            currentMovement.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
        }
    }

    private void ExecuteJump() 
    {
        if (!applyJump) return;
        currentMovement.y = jumpForce;
        characterAnimationController.ResetJumpTrigger();
        applyJump = false;
    }

    private void HandleMovement() 
    {
       
        Vector3 worldDirection = CalculateWorldDirection();
        currentMovement.x = worldDirection.x * CurrentSpeed;
        currentMovement.z = worldDirection.z * CurrentSpeed;

        HandleJumping();
        HandleCrouch();

        
        //Handle the inputs for animations https://www.youtube.com/watch?v=xWHsS7ju3m8
        float xParam = Mathf.Lerp(transform.InverseTransformDirection(currentMovement).x, characterController.transform.forward.x * CurrentSpeed, animationBlendSpeed * Time.deltaTime);
        float yParam = Mathf.Lerp(transform.InverseTransformDirection(currentMovement).z, characterController.transform.forward.z * CurrentSpeed, animationBlendSpeed * Time.deltaTime);
        float zParam = Mathf.Lerp(transform.InverseTransformDirection(currentMovement).y, characterController.transform.forward.z * CurrentSpeed, animationBlendSpeed * Time.deltaTime);

        characterAnimationController.Move(xParam, yParam);
        characterController.Move(currentMovement * Time.deltaTime);
        UpdateCurrentState();
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
            if (playerInputHandler.CrouchTriggered && !playerInputHandler.SprintTriggered) 
            {
                if (!IsCrouching)
                {
                    Crouch();
                }
            }
            else
            {
                if (IsCrouching)
                {
                    TryStandUp();
                }
            }

            characterController.height = Mathf.Lerp(characterController.height, CurrentTargetHeight, Time.deltaTime * 3);
            characterController.center = Vector3.Lerp(characterController.center, CurrentTargetCenter, Time.deltaTime * 3);
            mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, CurrentCameraPosition, mainCamera.transform.position.z);

            characterAnimationController.Crouch(IsCrouching);
            
        }
    }

    private void Crouch()
    {
        IsCrouching = true;
        CurrentTargetHeight = crouchHeight;
        CurrentTargetCenter = crouchCenter;
        CurrentCameraPosition = crouchHeight;
    }

    private void UpdateCurrentState()
    {
        characterAnimationController.Falling(!characterController.isGrounded);
        characterAnimationController.Grounded(characterController.isGrounded);
    }
    
    private void TryStandUp()
    {
        Vector3 rayOrigin = transform.position + characterController.center + Vector3.up * (characterController.height / 2f);
        float castDistance = standHeight - crouchHeight + 0.1f; // Add a small buffer

        if (!Physics.Raycast(rayOrigin, Vector3.up, 3))
        {
            IsCrouching = false;
            CurrentCameraPosition = startingCameraPostition;
            CurrentTargetCenter = standCenter;
            CurrentTargetHeight = standHeight;
        }
    }

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        CurrentWeapon.Attack();
    }
}

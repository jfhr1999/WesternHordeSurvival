using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameObject CharacterModel;

    [SerializeField]
    private Animator CharacterAnimator;

    [SerializeField]
    private FirstPersonController FPSController;

    //Animator variables
    private int _xVelHash;
    private int _yVelHash;
    private int _zVelHash;
    private int _jumpHash;
    private int _fallingHash;
    private int _GroundHash;
    private int _CroucHash;

    private void Start()
    {
        CharacterAnimator.SetTrigger("Idle");
        _xVelHash = Animator.StringToHash("xVelocity");
        _yVelHash = Animator.StringToHash("yVelocity");
        _zVelHash = Animator.StringToHash("zVelocity");
        _jumpHash = Animator.StringToHash("Jump");
        _fallingHash = Animator.StringToHash("Falling");
        _GroundHash = Animator.StringToHash("Grounded");
        _CroucHash = Animator.StringToHash("Crouch");
    }

    public void Move(float x, float y) 
    {
        CharacterAnimator.SetFloat(_xVelHash, x);
        CharacterAnimator.SetFloat(_yVelHash, y);
    }

    public void VericalMove(float z)
    {
        CharacterAnimator.SetFloat(_zVelHash, z);
    }

    public void Crouch(bool trigger) 
    {
        CharacterAnimator.SetBool(_CroucHash, trigger);
    }

    public void Jump()
    {
        CharacterAnimator.SetTrigger(_jumpHash);
    }
    public void ResetJumpTrigger()
    {
        CharacterAnimator.ResetTrigger(_jumpHash);
    }

    public void Falling(bool value)
    {
        CharacterAnimator.SetBool(_fallingHash, value);
    }

    public void Grounded(bool value)
    {
        CharacterAnimator.SetBool(_GroundHash, value);
    }

    public void ApplyJumpListener()
    {
        FPSController.ApplyJump = true;
    }

}

using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameObject CharacterModel;

    [SerializeField]
    private Animator CharacterAnimator;

    //Animator variables
    private int _xVelHash;
    private int _yVelHash;

    private void Start()
    {
        CharacterAnimator.SetTrigger("Idle");
        _xVelHash = Animator.StringToHash("xVelocity");
        _yVelHash = Animator.StringToHash("yVelocity");
    }

    public void Move(float x, float y) 
    {
        CharacterAnimator.SetFloat(_xVelHash, x);
        CharacterAnimator.SetFloat(_yVelHash, y);
    }

    public void Crouch(bool trigger) 
    {
        CharacterAnimator.SetBool("Crouch", trigger);
    }

    public void Rotate(Quaternion rot) 
    {
        //CharacterModel.transform.rotation = rot;
    }


}

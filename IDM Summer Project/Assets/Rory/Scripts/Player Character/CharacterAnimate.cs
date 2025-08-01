using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimate : MonoBehaviour
{
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void SetAnimationState(PlayerAnimState state)
    {
        switch (state)
        {
            case PlayerAnimState.idle:
                animator.Play("Idle");
                break;
            case PlayerAnimState.jump:
                animator.Play("Jump");
                break;
            case PlayerAnimState.run:
                animator.Play("Run");
                break;
            default:
                animator.Play("Idle");
                break;
        }
    }

}
public enum PlayerAnimState
{
    idle,
    run,
    jump
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class CharacterAnimate : MonoBehaviour
{
    Animator animator;
    [SerializeField] CharacterManager manager;
    [SerializeField] public EventReference bellSound;

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetAnimationState(PlayerAnimState state)
    {
        animator.SetBool("isRunning", false);
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
                animator.SetBool("isRunning", true);
                break;
            case PlayerAnimState.fall:
                animator.Play("Fall");
                break;
            case PlayerAnimState.glide:
                animator.Play("Glide");
                break;
            default:
                animator.Play("Idle");
                break;
        }
    }
    private void Update()
    {
        animator.SetBool("isHolding", manager.holding != null);
        if (manager.stats.input.Call())
        {
            RuntimeManager.PlayOneShot(bellSound);
            animator.SetTrigger("onBell");
        }
    }

}
public enum PlayerAnimState
{
    idle,
    run,
    jump,
    fall,
    glide,
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimate : MonoBehaviour
{
    Animator animator;
    [SerializeField] CharacterStats stats;

    bool isHolding;
    // Start is called before the first frame update
    void Awake()
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
    private void Update()
    {
        isHolding = stats.sheep.HoldingSheep();
    }
}
public enum PlayerAnimState
{
    idle,
    run,
    jump
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterState
{
    protected const float _RUNSPEED = 15;
    protected const float _JUMPHEIGHT = 4;
    protected const float _BUFFERTIME = 0.2f;
    protected const float _COYOTETIME = 0.2f;

    public static InputHandler input;
    public static Rigidbody playerRb;
    public abstract void Update();
    public abstract CharacterState NewState();
    protected bool IsGrounded()
    {
        return playerRb.velocity.y >= -0.05f;
    }
    public virtual void Enter() 
    {
        //Debug.Log(this);
    }
    
    public virtual void Move(float speed)
    {
        playerRb.AddForce(input.GetMoveDirection() * speed);
    }
}
public class CharacterIdle : CharacterState
{
    public override void Update()
    {}
    public override CharacterState NewState()
    {
        if (!IsGrounded()) return new CharacterOffLedge();
        if (input.Jump())
        {
            return new CharacterJump();
        }
        if (input.GetMoveDirection().magnitude > 0)
        {
            return new CharacterRunning();
        }
        return null;
    }
}
public class CharacterRunning : CharacterState
{
    public override void Update()
    {
        Move(_RUNSPEED);
    }
    public override CharacterState NewState()
    {
        if (!IsGrounded()) return new CharacterOffLedge();
        if (input.Jump())
        {
            return new CharacterJump();
        }
        if (input.GetMoveDirection().magnitude == 0)
        {
            return new CharacterIdle();
        }
        return null;
    }
}
public class CharacterJump : CharacterState
{
    public override void Enter()
    {
        base.Enter();
        playerRb.velocity = new Vector3(playerRb.velocity.x, 0, playerRb.velocity.z);
        playerRb.AddForce(Vector3.up * _JUMPHEIGHT, ForceMode.Impulse);
    }
    public override void Update()
    {
        Move(_RUNSPEED);
    }
    public override CharacterState NewState()
    {
        if (playerRb.velocity.y < -0.05f) return new CharacterFall();
        return null;
    }
}
public class CharacterFall : CharacterState
{
    float buffer = 0;
    public override void Update()
    {
        Move(_RUNSPEED);
        buffer += Time.fixedDeltaTime;
    }
    public override CharacterState NewState()
    {
        if (input.Jump()) buffer = -_BUFFERTIME;
        if (IsGrounded() && buffer < 0) return new CharacterJump();
        if (IsGrounded()) return new CharacterIdle();
        return null;
    }
}public class CharacterOffLedge : CharacterState
{
    float timer = 0;
    
    public override void Update()
    {
        Move(_RUNSPEED);
        timer += Time.fixedDeltaTime;
    }
    public override CharacterState NewState()
    {
        if (input.Jump()) return new CharacterJump();
        if (timer > _COYOTETIME) return new CharacterFall(); 
        if (IsGrounded()) return new CharacterIdle();
        return null;
    }
}

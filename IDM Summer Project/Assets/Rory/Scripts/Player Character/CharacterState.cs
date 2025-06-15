using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterState
{
    protected CharacterStats stats;
    public CharacterState(CharacterStats stats) 
    {
        this.stats = stats;
    }
    public abstract void Update();
    public abstract CharacterState NewState();
    protected bool IsGrounded()
    {
        return stats.rb.velocity.y >= -0.05f;
    }
    public virtual void Enter() 
    {
        //Debug.Log(this);
    }
    
    public virtual void Move(float speed)
    {
        stats.rb.AddForce(stats.input.GetMoveDirection() * speed);
    }
}
public class CharacterIdle : CharacterState
{
    public CharacterIdle(CharacterStats stats): base(stats) { }
    public override void Update()
    {}
    public override CharacterState NewState()
    {
        if (!IsGrounded()) return new CharacterOffLedge(stats);
        if (stats.input.Jump())
        {
            return new CharacterJump(stats);
        }
        if (stats.input.GetMoveDirection().magnitude > 0)
        {
            return new CharacterRunning(stats);
        }
        return null;
    }
}
public class CharacterRunning : CharacterState
{
    public CharacterRunning(CharacterStats stats) : base(stats) { }
    public override void Update()
    {
        Move(stats.runSpeed);
    }
    public override CharacterState NewState()
    {
        if (!IsGrounded()) return new CharacterOffLedge(stats);
        if (stats.input.Jump())
        {
            return new CharacterJump(stats);
        }
        if (stats.input.GetMoveDirection().magnitude == 0)
        {
            return new CharacterIdle(stats);
        }
        return null;
    }
}
public class CharacterJump : CharacterState
{
    public CharacterJump(CharacterStats stats) : base(stats) { }
    public override void Enter()
    {
        base.Enter();
        stats.rb.velocity = new Vector3(stats.rb.velocity.x, 0, stats.rb.velocity.z);
        stats.rb.AddForce(Vector3.up * stats.jumpHeight, ForceMode.Impulse);
    }
    public override void Update()
    {
        Move(stats.runSpeed);
    }
    public override CharacterState NewState()
    {
        if (stats.rb.velocity.y < -0.05f) return new CharacterFall(stats);
        return null;
    }
}
public class CharacterFall : CharacterState
{
    public CharacterFall(CharacterStats stats) : base(stats) { }
    float bufferTimer = 0;
    public override void Update()
    {
        Move(stats.runSpeed);
        bufferTimer += Time.fixedDeltaTime;
    }
    public override CharacterState NewState()
    {
        if (stats.input.Jump()) bufferTimer = -stats.bufferTime;
        if (IsGrounded() && bufferTimer < 0) return new CharacterJump(stats);
        if (IsGrounded()) return new CharacterIdle(stats);
        return null;
    }
}public class CharacterOffLedge : CharacterState
{
    public CharacterOffLedge(CharacterStats stats) : base(stats) { }
    float timer = 0;
    public override void Update()
    {
        Move(stats.runSpeed);
        timer += Time.fixedDeltaTime;
    }
    public override CharacterState NewState()
    {
        if (stats.input.Jump()) return new CharacterJump(stats);
        if (timer > stats.coyoteTime) return new CharacterFall(stats); 
        if (IsGrounded()) return new CharacterIdle(stats);
        return null;
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
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
        if(stats.rb.velocity.y >= -0.05f){
            stats.input.Grounded();
            return true;
        }
        return false;
    }
    public virtual void Enter() 
    {
        Debug.Log(this);
    }
    
    public virtual void Move(float speed)
    {
        //stats.rb.AddForce(stats.input.GetMoveDirection() * speed);
        Vector3 moveDir = stats.input.GetMoveDirection() * speed;
        stats.rb.velocity = new Vector3(moveDir.x, stats.rb.velocity.y, moveDir.z);

        if(moveDir.magnitude > 0)
        {
            Vector3 lookDir = stats.rb.velocity;
            lookDir.y = 0f;
            stats.rb.transform.rotation = Quaternion.LookRotation(lookDir, Vector3.up);
        }
        
    }
}
public class CharacterIdle : CharacterState
{
    public CharacterIdle(CharacterStats stats): base(stats) { }
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
        stats.input.OnJump();
        stats.rb.velocity = new Vector3(stats.rb.velocity.x, 0, stats.rb.velocity.z);
        stats.rb.AddForce(Vector3.up * stats.jumpHeight, ForceMode.Impulse);
    }
    public override void Update()
    {
        Move(stats.runSpeed);
    }
    public override CharacterState NewState()
    {
        if (stats.input.DoubleJump() && stats.input.Jump()) return new CharacterDoubleJump(stats);
        if (stats.rb.velocity.y < -0.05f) return new CharacterFall(stats);
        return null;
    }
}
public class CharacterFall : CharacterState
{
    public CharacterFall(CharacterStats stats) : base(stats) { }
    public override void Update()
    {
        Move(stats.runSpeed);
    }
    public override CharacterState NewState()
    {

        if (IsGrounded() && stats.input.Jump()) return new CharacterJump(stats);
        if (stats.input.DoubleJump() && stats.input.Jump()) return new CharacterDoubleJump(stats);
        if (stats.input.Glide()) return new CharacterGlide(stats);
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
public class CharacterDoubleJump : CharacterState
{
    public CharacterDoubleJump(CharacterStats stats) : base(stats) { }
    public override void Enter()
    {
        base.Enter();
        stats.input.OnJump();
        stats.input.OnDouble();
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
public class CharacterGlide : CharacterState
{
    public CharacterGlide(CharacterStats stats) : base(stats) { }
    public override void Enter()
    {
        base.Enter();
    }
    public override void Update()
    {
        Move(stats.runSpeed);
        Vector3 cVel = stats.rb.velocity;
        stats.rb.velocity = new Vector3(cVel.x, -0.5f, cVel.z);
    }
    public override CharacterState NewState()
    {
        if (IsGrounded() && stats.input.Jump()) return new CharacterJump(stats);
        if (!stats.input.Glide()) return new CharacterFall(stats);
        if (IsGrounded()) return new CharacterIdle(stats);
        return null;
    }
}

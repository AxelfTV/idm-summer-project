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
        float rayLength = 0.8f;
        float sphereRadius = 0.4f;
        Vector3 origin = stats.rb.position + Vector3.up * 0.1f;

        return Physics.SphereCast(origin, sphereRadius, Vector3.down,out RaycastHit hit, rayLength, LayerMask.GetMask("Ground"));
    }
    public virtual void Enter() 
    {
        //Debug.Log(this);
    }
    public virtual void Exit()
    {

    }
    
    protected void Move()
    {
        //stats.rb.AddForce(stats.input.GetMoveDirection() * speed);
        Vector3 moveDir = stats.input.GetMoveDirection() * stats.runSpeed;
        stats.rb.velocity = new Vector3(moveDir.x, stats.rb.velocity.y, moveDir.z);

        
        FaceMoveDirection();
        

        //stick to ground
        stats.rb.AddForce(Vector3.down * stats.downwardAcceleration);
    }
    protected void AirMove()
    {
        Vector3 moveDir = stats.input.GetMoveDirection();
        
        if(moveDir == Vector3.zero)
        {
            Vector3 negDir = new Vector3(-stats.rb.velocity.x, 0,-stats.rb.velocity.z).normalized;
            stats.rb.AddForce(negDir * stats.airMovePower);
        }
        else
        {
            stats.rb.AddForce(moveDir * stats.airMovePower);
        }
        Vector3 horVel = Vector3.ProjectOnPlane(stats.rb.velocity, Vector3.up);
        if(horVel.magnitude > stats.airMoveMaxSpeed) 
        {
            stats.rb.velocity = new Vector3(0, stats.rb.velocity.y, 0) + horVel.normalized * stats.airMoveMaxSpeed;
        }

        FaceMoveDirection();
    }
    protected void Jump(float power)
    {
        stats.rb.velocity = new Vector3(stats.rb.velocity.x, 0, stats.rb.velocity.z);
        stats.rb.AddForce(Vector3.up * power, ForceMode.Impulse);
    }
    protected void Fall()
    {
        if(stats.rb.velocity.y > 0)
        {
            stats.rb.AddForce(Vector3.down * stats.downwardAcceleration * 5);
        }
        else
        {
            stats.rb.AddForce(Vector3.down * stats.downwardAcceleration);
        }
        
    }
    protected void Glide()
    {
        Vector3 cVel = stats.rb.velocity;
        stats.rb.velocity = new Vector3(cVel.x, -0.5f, cVel.z);
        stats.WhileGlide();


        FaceMoveDirection();
    }
    protected void FaceMoveDirection()
    {
        if (stats.input.GetMoveDirection().magnitude == 0) return;
        Vector3 lookDir = stats.rb.velocity;
        lookDir.y = 0f;
        stats.rb.transform.rotation = Quaternion.LookRotation(lookDir.normalized, Vector3.up);
    }
}
public class CharacterIdle : CharacterState
{
    public CharacterIdle(CharacterStats stats): base(stats) { }
    public override void Enter()
    {
        base.Enter();
        stats.Grounded();
        stats.animator.SetAnimationState(PlayerAnimState.idle);
    }
    public override void Update()
    {
        Move();
    }
    public override CharacterState NewState()
    {
        if (!IsGrounded()) return new CharacterOffLedge(stats);
        if (stats.input.Jump() || stats.JumpBuffered())
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
    public override void Enter()
    {
        base.Enter();
        stats.animator.SetAnimationState(PlayerAnimState.run);
    }
    public override void Update()
    {
        Move();
    }
    public override CharacterState NewState()
    {
        if (!IsGrounded()) return new CharacterOffLedge(stats);
        if (stats.input.Jump() || stats.JumpBuffered())
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
    float jumpTimer = 0;
    public CharacterJump(CharacterStats stats) : base(stats) { }
    public override void Enter()
    {
        base.Enter();
        stats.OnJump();
        Jump(stats.jumpPower);
        stats.animator.SetAnimationState(PlayerAnimState.jump);
    }
    public override void Update()
    {
        jumpTimer += Time.fixedDeltaTime;

        AirMove();
    }
    public override CharacterState NewState()
    {
        if (stats.CanDouble() && stats.input.Jump()) return new CharacterDoubleJump(stats);
        if (jumpTimer >= stats.jumpLength) return new CharacterFall(stats);
        return null;
    }
}
public class CharacterFall : CharacterState
{
    public CharacterFall(CharacterStats stats) : base(stats) { }

    public override void Enter()
    {
        base.Enter();
        
        stats.animator.SetAnimationState(PlayerAnimState.fall);
    }
    public override void Update()
    {
        AirMove();
        Fall();
        
    }
    public override CharacterState NewState()
    {

        if (IsGrounded() && stats.input.Jump()) return new CharacterJump(stats);
        if (stats.CanDouble() && stats.input.Jump()) return new CharacterDoubleJump(stats);
        if (stats.input.Glide() && stats.CanGlide()) return new CharacterGlide(stats);
        if (IsGrounded()) return new CharacterIdle(stats);
        return null;
    }
}public class CharacterOffLedge : CharacterState
{
    public CharacterOffLedge(CharacterStats stats) : base(stats) { }
    float timer = 0;
    public override void Update()
    {
        AirMove();
        Fall();
        timer += Time.fixedDeltaTime;
        //should be fall
        stats.animator.SetAnimationState(PlayerAnimState.fall);
    }
    public override CharacterState NewState()
    {
        if (stats.input.Jump() || stats.JumpBuffered()) return new CharacterJump(stats);
        if (timer > stats.coyoteTime) return new CharacterFall(stats); 
        if (IsGrounded()) return new CharacterIdle(stats);
        return null;
    }
}
public class CharacterDoubleJump : CharacterState
{
    public CharacterDoubleJump(CharacterStats stats) : base(stats) { }
    float jumpTimer = 0;
    public override void Enter()
    {
        base.Enter();
        stats.OnJump();
        stats.OnDouble();
        Jump(stats.doubleJumpPower);
        stats.animator.SetAnimationState(PlayerAnimState.glide);
    }
    public override void Update()
    {
        AirMove();
        jumpTimer += Time.fixedDeltaTime;
    }
    public override CharacterState NewState()
    {
        if (jumpTimer >= stats.doubleJumpLength) return new CharacterFall(stats);
        return null;
    }
}
public class CharacterGlide : CharacterState
{
    public CharacterGlide(CharacterStats stats) : base(stats) { }
    public override void Enter()
    {
        base.Enter();
        stats.animator.SetAnimationState(PlayerAnimState.glide);
        stats.glideSoundInstance.start();
    }
    public override void Exit()
    {
        base.Exit();
        stats.glideSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

    }
    public override void Update()
    {
        AirMove();
        Glide();
    }
    public override CharacterState NewState()
    {
        if (IsGrounded() && stats.input.Jump()) return new CharacterJump(stats);
        if (!stats.input.Glide() || !stats.CanGlide()) return new CharacterFall(stats);
        if (IsGrounded()) return new CharacterIdle(stats);
        return null;
    }
}
public class CharacterGlideBoost : CharacterState 
{
    Vector3 direction;
    float timer = 0;
	public CharacterGlideBoost(CharacterStats stats, Vector3 direction) : base(stats) 
    {
        this.direction = direction;
    }
	public override void Enter()
	{
		base.Enter();
        stats.Grounded();
        stats.animator.SetAnimationState(PlayerAnimState.glide);
    }
	public override void Update()
	{
        stats.rb.velocity = direction * stats.glideBoostStrength;
        timer += Time.deltaTime;
	}
	public override CharacterState NewState()
	{
		if (IsGrounded() && (stats.input.Jump() || stats.JumpBuffered())) return new CharacterJump(stats);
		if (timer > stats.glideBoostTime) return new CharacterFall(stats);
		if (IsGrounded()) return new CharacterIdle(stats);
		return null;
	}
}
public class CharacterBounce : CharacterState
{
    float bouncePower;
    public CharacterBounce(CharacterStats stats, float power) : base(stats) 
    {
        bouncePower = power;
    }
    float jumpTimer = 0;
    public override void Enter()
    {
        base.Enter();
        stats.OnJump();
        stats.Grounded();
        Jump(bouncePower);
        stats.animator.SetAnimationState(PlayerAnimState.jump);
    }
    public override void Update()
    {
        AirMove();
        jumpTimer += Time.fixedDeltaTime;
    }
    public override CharacterState NewState()
    {
        if (jumpTimer >= stats.doubleJumpLength) return new CharacterFall(stats);
        return null;
    }
}

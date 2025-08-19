using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class SheepState
{
    protected SheepStats stats;
    public bool holding;
    public SheepState(SheepStats stats)
    {
        this.stats = stats;
    }
    public abstract void Update();
    public abstract SheepState NewState();
   
    public virtual void Enter()
    {
        //Debug.Log(this);
    }
    public virtual void Exit()
    {

    }
    protected void ScaleSheep(float targetScale)
    {
        
        float currentScaleMult = stats.transform.localScale.x/stats.baseScale;

        float nextScale = Mathf.Lerp(currentScaleMult, targetScale, stats.scaleSpeed * Time.fixedDeltaTime);

        stats.transform.localScale = new Vector3(nextScale, nextScale, nextScale);
    }
    protected bool IsGrounded()
    {
        float rayLength = 0.2f;
        Vector3 origin = stats.rb.position + Vector3.down * 0.5f + Vector3.up * 0.1f;

        return Physics.Raycast(origin, Vector3.down, rayLength, LayerMask.GetMask("Ground"));
    }
    protected void PointInDirectionOfMove()
    {
        Vector3 moveDir = stats.rb.velocity;
        moveDir.ProjectOntoPlane(Vector3.up);
        moveDir.Normalize();
        stats.rb.transform.rotation = Quaternion.LookRotation(moveDir, Vector3.up);
    }
    protected Vector3 VectorToFollow()
    {
        return (stats.followPosition.position - stats.rb.position);
    }
    protected Vector3 VectorToHold()
    {
        return (stats.holdPosition.position - stats.rb.position);
    }
    protected Vector3 VectorToPlayer()
    {
        return (stats.player.transform.position - stats.rb.position);
    }
    protected void TravelToPosition(Vector3 position)
    {
        float distToPosition = (stats.transform.position - position).magnitude + 1;
        stats.rb.transform.position = Vector3.MoveTowards(stats.rb.position, position, (stats.travelSpeed + distToPosition) * Time.fixedDeltaTime);
    }
    protected void FollowMode()
    {
        stats.rb.useGravity = true;
        //stats.rb.transform.localScale = Vector3.one;
        stats.col.isTrigger = false;
        stats.rb.velocity = Vector3.zero;

        stats.bouncePad.SetActive(false);
    }
    protected void TravelMode()
    {
        stats.rb.useGravity = false;
        //stats.rb.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        stats.col.isTrigger = true;
        stats.rb.velocity = Vector3.zero;

        stats.bouncePad.SetActive(false);
    }
    protected void HoldMode()
    {
        stats.rb.useGravity = false;
        //stats.rb.transform.localScale = Vector3.one;
        stats.col.isTrigger = true;

        stats.bouncePad.SetActive(false);
    }
    public virtual bool Grab()
    {
        return false;
    }
    public virtual void Throw(){}
}
public class SheepIdle : SheepState
{
    public SheepIdle(SheepStats stats) : base(stats) { }
    public override void Enter()
    {
        base.Enter();
        FollowMode();
    }
    public override void Update()
    {
        ScaleSheep(1);
    }
    public override SheepState NewState()
    {
        if (VectorToFollow().magnitude > stats.idleDist) return new SheepFollow(stats);
        if (stats.input.Call()) return new SheepToHold(stats);
        return null;
    }
}
public class SheepFollow : SheepState
{
    public SheepFollow(SheepStats stats) : base(stats) { }
    public override void Enter()
    {
        base.Enter();
        FollowMode();
    }
    public override void Update()
    {
        Vector3 toFollow = VectorToFollow();
        stats.rb.AddForce(stats.followSpeed * toFollow.normalized);
        PointInDirectionOfMove();

        ScaleSheep(1);
    }
    public override SheepState NewState()
    {
        if (VectorToFollow().magnitude < stats.idleDist) return new SheepIdle(stats);
        if (VectorToPlayer().magnitude > stats.returnFromFollowDist) return new SheepReturn(stats);
        if (stats.input.Call()) return new SheepToHold(stats);
        return null;
    }
}

public class SheepReturn : SheepState
{
    public SheepReturn(SheepStats stats) : base(stats) { }
    public override void Enter()
    {
        base.Enter();
        TravelMode();
    }
    public override void Update()
    {
        TravelToPosition(stats.player.transform.position);
        PointInDirectionOfMove();

        ScaleSheep(0.5f);
    }
    public override SheepState NewState()
    {
        if (VectorToPlayer().magnitude < stats.followFromReturnDist && stats.rb.position.y >= stats.followPosition.position.y ) return new SheepFollow(stats);
        if (stats.input.Call()) return new SheepToHold(stats);
        return null;
    }
}

public class SheepToHold : SheepState
{
    public SheepToHold(SheepStats stats) : base(stats) { }
    public override void Enter()
    {
        base.Enter();
        TravelMode();
    }
    public override void Update()
    {
        TravelToPosition(stats.holdPosition.position);

        ScaleSheep(0.5f);
    }
    public override SheepState NewState()
    {
        if (VectorToHold().magnitude < 1) 
        {
            //attempt to enter players hold
            if (stats.player.SheepToHold(stats.sheep)) return new SheepHold(stats);
            //if unable
            else return new SheepReturn(stats);
        } 
        if (stats.input.Call()) return new SheepReturn(stats);
        return null;
    }
}
public class SheepHold : SheepState
{
    bool toThrow;
    public SheepHold(SheepStats stats) : base(stats) {}
    public override void Enter()
    {
        holding = true;
        base.Enter();
        HoldMode();
        stats.rb.transform.position = stats.holdPosition.position;
    }
    public override void Exit()
    {
        holding = false;
    }
    public override void Update()
    {
        float scaleValue;

        if (stats.player.state is CharacterDoubleJump) scaleValue = stats.doubleJumpScale;

        else scaleValue = stats.player.stats.glideSliderValue == 1 ? 1 : stats.player.stats.glideSliderValue + stats.glideMaxScale - 1;

        ScaleSheep(scaleValue);
    }
    public override SheepState NewState()
    {
        if (stats.input.Call()) 
        {
            stats.player.Drop();
            return new SheepReturn(stats); 
        }
        if (toThrow) return new SheepThrow(stats);
        return null;
    }
    public override void Throw()
    {
        toThrow = true;
    }
}

public class SheepThrow : SheepState
{
    void Fire()
    {
        Vector3 throwDir = (stats.player.transform.forward + Vector3.up).normalized;
        stats.rb.AddForce(throwDir * stats.throwDist, ForceMode.Impulse);
    }
    public SheepThrow(SheepStats stats) : base(stats) { }
    public override void Enter()
    {
        base.Enter();
        FollowMode();
        stats.rb.position = stats.holdPosition.position;
        Fire();
    }
    public override void Update()
    {
        ScaleSheep(1);
        stats.rb.AddForce(Vector3.down * stats.throwDownForce, ForceMode.Acceleration);
    }
    public override SheepState NewState()
    {
        if (IsGrounded()) return new SheepGround(stats);
        if (VectorToPlayer().magnitude > stats.returnFromThrowDist) return new SheepReturn(stats);
        if (stats.input.Call()) return new SheepToHold(stats);
        return null;
    }
}

public class SheepGround : SheepState
{
    bool toGrab;
    public SheepGround(SheepStats stats) : base(stats) { }
    public override void Enter()
    {
        base.Enter();
        stats.gameObject.layer = 6;
        stats.rb.velocity = Vector3.zero;
        stats.bouncePad.SetActive(true);
    }
	public override void Exit()
	{
		base.Exit();
        stats.gameObject.layer = 7;
	}
    public override void Update()
    {
        stats.rb.velocity = Vector3.zero;
        ScaleSheep(1);
    }
    public override SheepState NewState()
    {
        if (stats.input.Call()) return new SheepToHold(stats);
        //if (VectorToPlayer().magnitude > stats.returnFromThrowDist) return new SheepReturn(stats);
        if (toGrab) return new SheepHold(stats);
        return null;
    }
    public override bool Grab()
    {
        toGrab = true;
        return true;
    }
}
public class SheepFrozen : SheepState 
{
    public SheepFrozen(SheepStats stats) : base(stats) { }
    public override void Enter()
    {
        base.Enter();
        stats.gameObject.layer = 0;
    }
    public override void Exit()
    {
        base.Exit();
        stats.gameObject.layer = 7;
    }
    public override void Update()
    {
        ScaleSheep(1);
    }
    public override SheepState NewState()
    {
        return null;
    }
}

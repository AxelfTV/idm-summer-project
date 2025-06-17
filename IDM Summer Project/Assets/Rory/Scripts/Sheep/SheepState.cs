using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class SheepState
{
    protected SheepStats stats;
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
    protected bool IsGrounded()
    {
        float rayLength = 0.2f;
        Vector3 origin = stats.rb.position + Vector3.down * 0.5f + Vector3.up * 0.1f;

        return Physics.Raycast(origin, Vector3.down, rayLength, LayerMask.GetMask("Ground"));
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
        stats.rb.transform.position = Vector3.MoveTowards(stats.rb.position, position, stats.travelSpeed * Time.fixedDeltaTime);
    }
    protected void FollowMode()
    {
        stats.rb.useGravity = true;
        stats.rb.transform.localScale = Vector3.one;
        stats.col.isTrigger = false;
        stats.rb.velocity = Vector3.zero;

        stats.bouncePad.SetActive(false);
    }
    protected void TravelMode()
    {
        stats.rb.useGravity = false;
        stats.rb.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        stats.col.isTrigger = true;

        stats.bouncePad.SetActive(false);
    }
    protected void HoldMode()
    {
        stats.rb.useGravity = false;
        stats.rb.transform.localScale = Vector3.one;
        stats.col.isTrigger = true;

        stats.bouncePad.SetActive(false);
    }
    
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
    }
    public override SheepState NewState()
    {
        if (VectorToHold().magnitude < 0.3f) return new SheepHold(stats);
        //if (stats.input.Interact()) return new SheepThrow(stats);
        if (stats.input.Call()) return new SheepReturn(stats);
        return null;
    }
}
public class SheepHold : SheepState
{
    public SheepHold(SheepStats stats) : base(stats) { }
    public override void Enter()
    {
        base.Enter();
        HoldMode();
        stats.rb.transform.position = stats.holdPosition.position;
    }
    public override void Update()
    {
        //will be handled by player when interacting is implemented
        stats.rb.position = stats.holdPosition.position;
    }
    public override SheepState NewState()
    {
        if (stats.input.Interact()) return new SheepThrow(stats);
        if (stats.input.Call()) return new SheepReturn(stats);
        return null;
    }
}

public class SheepThrow : SheepState
{
    void Throw()
    {
        Vector3 throwDir = stats.player.transform.forward + Vector3.up;
        stats.rb.velocity = throwDir * stats.throwDist;
    }
    public SheepThrow(SheepStats stats) : base(stats) { }
    public override void Enter()
    {
        base.Enter();
        FollowMode();
        stats.rb.position = stats.holdPosition.position;
        Throw();
    }
    public override void Update()
    {
        
    }
    public override SheepState NewState()
    {
        if (IsGrounded()) return new SheepGround(stats);
        if (stats.input.Call()) return new SheepToHold(stats);
        return null;
    }
}

public class SheepGround : SheepState
{
    public SheepGround(SheepStats stats) : base(stats) { }
    public override void Enter()
    {
        base.Enter();
        stats.rb.velocity = Vector3.zero;
        stats.bouncePad.SetActive(true);
    }
    public override void Update()
    {
        stats.rb.velocity = Vector3.zero;
    }
    public override SheepState NewState()
    {
        if (stats.input.Call()) return new SheepToHold(stats);
        return null;
    }
}

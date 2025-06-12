using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterState
{
    public static InputHandler input;
    public static GameObject player;

    public abstract void Update();
    public abstract CharacterState NewState();
}
public class CharacterIdle : CharacterState
{
    public override void Update()
    {
        
    }
    public override CharacterState NewState()
    {
        if(input.GetMoveDirection().magnitude > 0)
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
        player.GetComponent<Rigidbody>().AddForce(input.GetMoveDirection() * 15);
    }
    public override CharacterState NewState()
    {
        if (input.GetMoveDirection().magnitude == 0)
        {
            return new CharacterIdle();
        }
        return null;
    }
}

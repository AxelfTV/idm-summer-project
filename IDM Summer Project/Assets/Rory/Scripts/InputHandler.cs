using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler
{
    Vector3 move;
    public Vector3 GetMoveDirection()
    {
        move = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            move += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.D))
        {
            move += Vector3.right;
        }
        if (Input.GetKey(KeyCode.A))
        {
            move += Vector3.left;
        }
        if (Input.GetKey(KeyCode.S))
        {
            move += Vector3.back;
        }
        move.Normalize();
        return move;
    }
}

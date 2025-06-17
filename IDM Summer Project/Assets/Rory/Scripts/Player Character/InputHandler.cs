using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
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
        return GetCameraForwardRotation() * move;
    }
    public bool Jump()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }
    public bool Call()
    {
        return Input.GetKeyDown(KeyCode.LeftControl);
    }
    public bool Interact()
    {
        return Input.GetKeyDown(KeyCode.LeftShift);
    }
    Quaternion GetCameraForwardRotation()
    {
        return Quaternion.LookRotation(Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized, Vector3.up);
        //return Quaternion.FromToRotation(Vector3.forward, Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized);
    }
}

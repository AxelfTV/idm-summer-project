using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [SerializeField] float jumpBuffer = 0.5f;
    bool jumpBuffered;
    bool canDouble;
    Vector3 move;

    public SheepManager sheep;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StopCoroutine("JumpBuffer");
            StartCoroutine("JumpBuffer");
        }
    }
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
        return Input.GetKeyDown(KeyCode.Space) || jumpBuffered;
        
    }
    public void OnJump()
    {
        jumpBuffered = false;
    }
    IEnumerator JumpBuffer()
    {
        jumpBuffered = true;
        yield return new WaitForSeconds(jumpBuffer);
        jumpBuffered = false;
    }
    public bool JumpOver()
    {
        return !Input.GetKey(KeyCode.Space);
    }
    public bool Call()
    {
        return Input.GetKeyDown(KeyCode.LeftControl);
    }
    public bool Interact()
    {
        return Input.GetKeyDown(KeyCode.LeftShift);
    }
    public bool Glide()
    {
        return sheep.HoldingSheep() && Input.GetKey(KeyCode.Space);
    }
    public bool DoubleJump()
    {
        return sheep.HoldingSheep() && canDouble;
    }
    public void OnDouble()
    {
        canDouble = false;
    }
    public void Grounded()
    {
        canDouble = true;
    }
    Quaternion GetCameraForwardRotation()
    {
        return Quaternion.LookRotation(Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized, Vector3.up);
    }
}

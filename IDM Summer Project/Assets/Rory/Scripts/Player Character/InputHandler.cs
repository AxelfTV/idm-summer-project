using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private PlayerInput controls;
    private Vector2 moveInput;
    private bool jumpPressed;
    private bool jumpHeld;
    private bool callPressed;
    private bool interactPressed;

    private void Awake()
    {
        controls = new PlayerInput();

        // Listen to Move (Vector2)
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Player.Jump.performed += ctx =>
        {
            jumpPressed = true;
            jumpHeld = true;
        };

        controls.Player.Jump.canceled += ctx => jumpHeld = false;

        controls.Player.Call.performed += ctx => callPressed = true;

        controls.Player.Interact.performed += ctx => interactPressed = true;

        controls.Player.Jump.performed += ctx => jumpPressed = true;
    }
    private void LateUpdate()
    {
        jumpPressed = false;
        callPressed = false;
        interactPressed = false;
    }
    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
    public Vector3 GetMoveDirection()
    {
        
        Vector3 move = new Vector3(moveInput.x,0,moveInput.y);
        /*
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
        */
        return GetCameraForwardRotation() * move;
    }
    public bool Jump()
    {
        return jumpPressed;
    }
    public bool Call()
    {
        return callPressed;
    }
    public bool Interact()
    {
        return interactPressed;
    }
    public bool Glide()
    {
        return jumpHeld;
    }
    Quaternion GetCameraForwardRotation()
    {
        return Quaternion.LookRotation(Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized, Vector3.up);
    }
}

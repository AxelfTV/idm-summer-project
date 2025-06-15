using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public InputHandler input;
    [NonSerialized] public Rigidbody rb;

    public float runSpeed = 15;
    public float jumpHeight = 8;
    public float bufferTime = 0.2f;
    public float coyoteTime = 0.2f;
    // Start is called before the first frame update
    void Start()
    {
        input = new InputHandler();
        rb = GetComponent<Rigidbody>();
    }
}

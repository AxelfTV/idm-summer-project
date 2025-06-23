using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [NonSerialized] public InputHandler input;
    [NonSerialized] public Rigidbody rb;
    public Transform holdPosition;
    public Transform sheepFollowPosition;

    [SerializeField] public float runSpeed = 15;
    [SerializeField] public float jumpHeight = 8;
    [SerializeField] public float jumpLength = 1f;
    [SerializeField] public float coyoteTime = 0.2f;
    [SerializeField] public float downwardAcceleration = 1f;
    [SerializeField] public float airMoveSpeed = 1f;
    // Start is called before the first frame update
    void Awake()
    {
        input = GetComponent<InputHandler>();
        rb = GetComponent<Rigidbody>();
    }
}

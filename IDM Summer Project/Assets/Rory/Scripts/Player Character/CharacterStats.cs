using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

public class CharacterStats : MonoBehaviour
{
    [NonSerialized] public InputHandler input;
    [NonSerialized] public Rigidbody rb;
    [NonSerialized] public SheepManager sheep;

    [SerializeField] public CharacterAnimate animator;
    [SerializeField] public Transform holdPosition;
    [SerializeField] public Transform sheepFollowPosition;

    [SerializeField] public float runSpeed = 15;
    [SerializeField] public float jumpPower = 8;
    [SerializeField] public float jumpLength = 1f;
    [SerializeField] public float jumpBuffer = 0.5f;
    [SerializeField] public float doubleJumpPower = 8;
    [SerializeField] public float doubleJumpLength = 1f;
    [SerializeField] public float coyoteTime = 0.2f;
    [SerializeField] public float downwardAcceleration = 1f;
    [SerializeField] public float airMovePower = 1f;
    [SerializeField] public float airMoveMaxSpeed = 1f;
    [SerializeField] public float grabRange = 1f;
    [SerializeField] public float glideTime = 2f;
    [SerializeField] public float glideBoostStrength = 20;
    [SerializeField] public float glideBoostTime = 0.5f;
    [SerializeField] public EventReference jumpSound;

    private bool isJumpBuffered = false;
    float glideEnergy;
    private bool canDouble = true;

    public float glideSliderValue
    {
        get { return glideEnergy / glideTime; }
    }

    //Temporary slider container
    [SerializeField] Slider slider;
    // Start is called before the first frame update
    void Awake()
    {
        input = GetComponent<InputHandler>();
        rb = GetComponent<Rigidbody>();


        glideEnergy = glideTime;
    }
    private void Update()
    {
        if (input.Jump())
        {
            StopCoroutine("JumpBuffer");
            StartCoroutine("JumpBuffer");
        }
        
        if(slider != null) slider.value = Mathf.Lerp(slider.value, glideSliderValue,Time.deltaTime * 20);
    }
    public void OnJump()
    {
        RuntimeManager.PlayOneShot(jumpSound);
        isJumpBuffered = false;
    }
    public bool JumpBuffered()
    {
        return isJumpBuffered;
    }
    IEnumerator JumpBuffer()
    {
        isJumpBuffered = true;
        yield return new WaitForSeconds(jumpBuffer);
        isJumpBuffered = false;
    }
    public bool CanDouble()
    {
        return sheep.HoldingSheep() && canDouble;
    }
    public void OnDouble()
    {
        canDouble = false;
    }
    public bool CanGlide()
    {
        return glideEnergy > 0 && sheep.HoldingSheep();
    }
    public void WhileGlide()
    {
        glideEnergy -= Time.fixedDeltaTime;
    }
    public void Grounded()
    {
        canDouble = true;
        glideEnergy = glideTime;
    }
}

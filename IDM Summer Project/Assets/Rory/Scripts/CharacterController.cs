using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    
    InputHandler input;
    CharacterState state;
    // Start is called before the first frame update
    void Start()
    {
        input = new InputHandler();
        CharacterState.player = gameObject;
        CharacterState.input = input;
        state = new CharacterIdle();
    }

    // Update is called once per frame
    void Update()
    {
        CharacterState newState = state.NewState();
        if(newState != null) state = newState;
    }
    private void FixedUpdate()
    {
        state.Update();
    }
}

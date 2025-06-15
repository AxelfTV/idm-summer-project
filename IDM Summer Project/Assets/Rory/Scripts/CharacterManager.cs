using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{

    CharacterStats stats;
    CharacterState state;
    // Start is called before the first frame update
    void Start()
    {
        stats = GetComponent<CharacterStats>();
        state = new CharacterIdle(stats);
    }

    // Update is called once per frame
    void Update()
    {
        CharacterState newState = state.NewState();
        if (newState != null) 
        { 
            state = newState;
            state.Enter();
        }
    }
    private void FixedUpdate()
    {
        state.Update();
    }
}

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
        //ignore collisions with sheep
        Physics.IgnoreLayerCollision(6, 7);

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
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bouncy"))
        {
            Debug.Log("Bouncy");
            state = new CharacterJump(stats);
            state.Enter();
        }
    }
}

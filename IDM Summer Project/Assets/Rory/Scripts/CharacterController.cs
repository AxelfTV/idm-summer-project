using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterController : MonoBehaviour
{
    
    InputHandler input;
    CharacterState state;
    // Start is called before the first frame update
    void Start()
    {
        input = new InputHandler();
        CharacterState.playerRb = GetComponent<Rigidbody>();
        CharacterState.input = input;
        state = new CharacterIdle();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepManager : MonoBehaviour
{
    SheepStats stats;
    SheepState state;
    // Start is called before the first frame update
    void Start()
    {
        stats = GetComponent<SheepStats>();
        state = new SheepFollow(stats);
    }

    // Update is called once per frame
    void Update()
    {
        SheepState newState = state.NewState();
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

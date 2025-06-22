using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepManager : MonoBehaviour, IHoldable
{
    SheepStats stats;
    SheepState state;

    // Start is called before the first frame update
    void Start()
    {
        stats = GetComponent<SheepStats>();
        state = new SheepFollow(stats);
        state.Enter();
    }

    // Update is called once per frame
    void Update()
    {
        SheepState newState = state.NewState();
        if (newState != null)
        {
            state.Exit();
            state = newState;
            state.Enter();
        }
    }
    private void FixedUpdate()
    {
        state.Update();
    }
    public bool Grab()
    {
        if (state.Grab())
        {
            state = new SheepHold(stats);
            state.Enter();
            return true;
        }
        return false;
    }
    public void Throw()
    {
        state.Throw();
    }
    public GameObject GetGameObject()
    {
        return gameObject;
    }
    public bool HoldingSheep()
    {
        return state.holding;
    }
}

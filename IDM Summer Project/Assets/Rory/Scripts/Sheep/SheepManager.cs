using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepManager : MonoBehaviour, IHoldable
{
    public SheepStats stats;
    SheepState state;

    bool active = false;

    // Start is called before the first frame update
    void Start()
    {
        stats = GetComponent<SheepStats>();
        state = new SheepFrozen(stats);
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
            state.Exit();
            state = new SheepHold(stats);
            state.Enter();
            return true;
        }
        return false;
    }
    public void Throw(Vector3 direction)
    {
        state.Throw();
    }
    public void Drop()
    {
        state.Exit();
        state = new SheepIdle(stats);
        state.Enter();
    }
    public GameObject GetGameObject()
    {
        return gameObject;
    }
    public bool HoldingSheep()
    {
        return state.holding;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !active)
        {
            active = true;
            state.Exit();
            state = new SheepFollow(stats);
            state.Enter();
            Destroy(GetComponent<BoxCollider>());
        }
    }
}
